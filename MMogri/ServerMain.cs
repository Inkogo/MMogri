using MMogri.Core;
using MMogri.Gameplay;
using MMogri.Network;
using MMogri.Renderer;
using MMogri.Scripting;
using MMogri.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MMogri
{
    class ServerMain
    {
        ServerInf serverInf;
        string serverPath;
        GameLoader loader;
        GameCore core;
        LuaHandler lua;
        LoginHandler login;
        CmdConsole cmdHandler;

        Dictionary<Guid, Player> activePlayers;
        ConcurrentStack<ClientUpdate> clientUpdates;
        int serverTick;
        int lastSave;

        public ServerMain(ServerInf serverInf, string serverPath, GameWindow window)
        {
            this.serverInf = serverInf;
            this.serverPath = serverPath;

            loader = new GameLoader();
            core = new GameCore(loader, AddClientUpdate);
            activePlayers = new Dictionary<Guid, Player>();
            clientUpdates = new ConcurrentStack<ClientUpdate>();
            cmdHandler = new CmdConsole();

            loader.Load(ServerDirectory);
            login = new LoginHandler(ServerDirectory);

            lua = new LuaHandler();
            lua.RegisterGlobalClass("Core", core);
            //lua.Run(Path.Combine(ServerPath, "testLua.lua"));       //make this nicer!

            MMogri.Network.NetworkHandler.Instance.StartServer(serverInf.port, this);
        }

        public void ServerTick()
        {
            lock (clientUpdates)
            {
                foreach (Guid g in activePlayers.Values.Select(x => x.mapId).Distinct().ToList())
                {
                    Map m = loader.GetMap(g);
                    foreach (Entity e in m.localEntities)
                    {
                        e.OnTick();
                    }
                }

                foreach (Guid m in clientUpdates.Select(x => x.mapId).Distinct())       //this is not thread save! netHander changes this array async! needs some sort of locking
                {
                    NetworkResponse p = new NetworkResponse();
                    p.type = NetworkResponse.ResponseType.Update;
                    ClientUpdate[] up = clientUpdates.Where(x => x.mapId == m).ToArray();
                    p.AppendObject((BinaryWriter w) =>
                    {
                        w.Write(up.Length);
                        foreach (ClientUpdate u in up)
                            w.Write(u.ToBytes());
                    });
                    loader.GetMap(m).isDirty = true;

                    foreach (Guid g in activePlayers.Keys.Where(x => activePlayers[x].mapId.Equals(m)))
                    {
                        NetworkHandler.Instance.SendNetworkResponse(g, p);
                    }
                }
                clientUpdates.Clear();

                serverTick++;
                if (serverTick - lastSave > serverInf.saveTickInterval)
                {
                    lastSave = serverTick;

                    if (loader.SaveMaps())
                    {
                        Console.WriteLine("Saving...");
                    }
                }
            }
        }

        public void ProcessNetworkRequest(NetworkRequest r, Guid g)
        {
            switch (r.requestType)
            {
                case NetworkRequest.RequestType.JoinAccount:
                    {
                        //0=name, 1=password
                        Account a = login.GetAccount(r.requestParams[0], new Guid(r.requestParams[1]));
                        RespondAccountJoined(a, g);
                    }
                    break;
                case NetworkRequest.RequestType.ChangePassword:
                    {
                        //0=oldPassword, 1=newPassword, 2=account, 3=sessionID
                        Account a = login.FindAccount(new Guid(r.requestParams[2]));
                        if (a.ComparePassword(new Guid(r.requestParams[0])) && login.ValidateSessionId(new Guid(r.requestParams[2]), r.requestParams[3]))
                        {
                            login.ChangePassword(a.email, new Guid(r.requestParams[0]), new Guid(r.requestParams[1]));
                            RespondPasswordChanged(true, g);
                        }
                        else
                            RespondPasswordChanged(false, g);
                    }
                    break;
                case NetworkRequest.RequestType.ResetPassword:
                    {
                        //0=email
                        login.ResetPassword(r.requestParams[0], serverInf.authentificationEmail, serverInf.authentificationEmailPassword);
                    }
                    break;
                case NetworkRequest.RequestType.CreateAccount:
                    {
                        //0=email, 1=password
                        string mail = r.requestParams[0].ToLower();
                        if (!serverInf.validateEmail || mail.IsEmailAdress())
                        {
                            if (login.FindAccount(mail) == null)
                            {
                                login.CreateAccount(mail, new Guid(r.requestParams[1]));
                            }
                        }
                    }
                    break;
                case NetworkRequest.RequestType.CreatePlayer:
                    //0=playerName, 1=account, 2=sessionID
                    {
                        if (login.ValidateSessionId(new Guid(r.requestParams[1]), r.requestParams[2]))
                        {
                            Player pl = login.CreatePlayer(r.requestParams[0], new Guid(r.requestParams[1]), loader.GetMap("Test Map").Id, 3, 3);
                            RegisterPlayer(pl, g);
                            RespondClientState(pl, g);
                        }
                    }
                    break;
                case NetworkRequest.RequestType.JoinPlayer:
                    {
                        //0=playerName, 1=account, 2=sessionID
                        Player pp = login.FindPlayer(new Guid(r.requestParams[1]), r.requestParams[0]);
                        if (login.ValidateSessionId(new Guid(r.requestParams[1]), r.requestParams[2]))
                        {
                            RegisterPlayer(pp, g);
                            RespondClientState(pp, g);
                        }
                    }
                    break;
                case NetworkRequest.RequestType.GetKeybinds:
                    {
                        //0=playerState
                        string p = loader.GetPlayerState(r.requestParams[0]).keybindPath;
                        RespondKeybinds(loader.GetKeybindsByPath(p), g);
                    }
                    break;
                case NetworkRequest.RequestType.PlayerInput:
                    {
                        //0-n=stringParams
                        loader.GetPlayerState(activePlayers[g].playerState).OnAction(r.requestAction, new object[] { activePlayers[g] }.Concat(r.requestParams));
                    }
                    break;
                case NetworkRequest.RequestType.ClientMessage:
                    {
                        //0=msg
                        string msg = r.requestParams[0];
                        if (msg[0] == '/')
                        {
                            cmdHandler.ExecCmd(msg.TrimStart());
                        }
                        else
                        {
                            Player pl = activePlayers[g];
                            NetworkResponse n = new NetworkResponse();
                            n.type = NetworkResponse.ResponseType.ChatMessage;
                            n.AppendObject((BinaryWriter w) => w.Write(pl.name + ": " + msg));

                            foreach (Guid i in activePlayers.Keys.Where(x => activePlayers[x].mapId == pl.mapId))
                            {
                                NetworkHandler.Instance.SendNetworkResponse(i, n);
                            }
                        }
                    }
                    break;
                case NetworkRequest.RequestType.Disconnect:
                    {
                        Player pl = activePlayers[g];
                        login.UpdatePlayer(pl);
                        UnregisterPlayer(g);
                    }
                    break;
            }
        }

        void AddClientUpdate(ClientUpdate c)
        {
            lock (clientUpdates)
            {
                clientUpdates.Push(c);
            }
        }

        void RegisterPlayer(Player p, Guid g)
        {
            if (activePlayers.ContainsKey(g)) return;
            activePlayers.Add(g, p);
            Debugging.Debug.Log(p.name + " joined the server!");
        }

        void UnregisterPlayer(Guid g)
        {
            if (!activePlayers.ContainsKey(g)) return;
            Debugging.Debug.Log(activePlayers[g].name + " left the server!");

            activePlayers.Remove(g);
        }

        void RespondAccountJoined(Account a, Guid g)
        {
            NetworkResponse p = new NetworkResponse();
            p.type = NetworkResponse.ResponseType.AccountLogin;
            if (a == null)
                p.error = NetworkResponse.ErrorCode.SecurityFailed;
            else
            {
                Player[] players = login.GetPlayersOfAccount(a.Id);
                p.AppendObject((BinaryWriter w) =>
                {
                    w.Write(players.Length); foreach (Player pl in players) w.Write(pl.name); w.Write(login.GenSessionToken(a.Id).ToString()); w.Write(a.Id.ToString());
                });
            }
            NetworkHandler.Instance.SendNetworkResponse(g, p);
        }

        void RespondPasswordChanged(bool b, Guid g)
        {
            NetworkResponse p = new NetworkResponse();
            p.type = NetworkResponse.ResponseType.Undefined;        //change later?
            p.error = b ? NetworkResponse.ErrorCode.None : NetworkResponse.ErrorCode.SecurityFailed;
            NetworkHandler.Instance.SendNetworkResponse(g, p);
        }

        void RespondKeybinds(Keybind[] keybinds, Guid g)
        {
            NetworkResponse p = new NetworkResponse();
            p.type = NetworkResponse.ResponseType.KeybindsInfo;
            if (keybinds == null)
                p.error = NetworkResponse.ErrorCode.DataNotFound;
            p.AppendObject((BinaryWriter w) =>
            {
                w.Write(activePlayers[g].playerState);
                w.Write(keybinds.Length);
                foreach (Keybind b in keybinds)
                {
                    w.Write(b.action); w.Write((int)b.key); w.Write((int)b.altKey);
                }
            });
            NetworkHandler.Instance.SendNetworkResponse(g, p);
        }

        void RespondClientState(Player pl, Guid g)
        {
            NetworkResponse p = new NetworkResponse();
            p.type = NetworkResponse.ResponseType.ClientState;
            Map m = loader.GetMap(pl.mapId);
            if (m == null)
                p.error = NetworkResponse.ErrorCode.DataNotFound;

            //not sure if I like this? write it to stream directly maybe? 
            byte[] u = new ClientGameState(
                                m,
                                activePlayers[g].x,
                                activePlayers[g].y,
                                loader.GetTileset.GetTileType(m[activePlayers[g].x, activePlayers[g].y].tileTypeId).name,
                                loader.GetPlayerState(activePlayers[g].playerState).name
                                ).ToBytes();

            p.AppendObject((BinaryWriter w) =>
            {
                w.Write(u.Length);
                w.Write(u);
            });
            NetworkHandler.Instance.SendNetworkResponse(g, p);
        }

        string ServerDirectory
        {
            get
            {
                return Path.GetDirectoryName(serverPath);
            }
        }
    }
}