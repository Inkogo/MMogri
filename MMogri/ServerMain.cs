using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using MMogri.Gameplay;
using MMogri.Core;
using MMogri.Scripting;
using MMogri.Input;
using MMogri.Renderer;
using MMogri.Network;
using MMogri.Security;

namespace MMogri
{
    class ServerMain
    {
        ServerInf serverInf;
        GameLoader loader;
        GameCore core;
        LuaHandler lua;
        LoginHandler login;
        CmdConsole cmdHandler;

        Dictionary<Guid, Player> activePlayers;
        List<MapUpdate> mapUpdates;

        public ServerMain(ServerInf serverInf, GameWindow window)
        {
            this.serverInf = serverInf;

            loader = new GameLoader();
            core = new GameCore(loader, (MapUpdate m) => mapUpdates.Add(m));
            activePlayers = new Dictionary<Guid, Player>();
            mapUpdates = new List<MapUpdate>();
            cmdHandler = new CmdConsole();

            loader.Load(ServerPath);

            lua = new LuaHandler();
            login = new LoginHandler(ServerPath);

            lua.RegisterObserved("Core", core);
            lua.Run(Path.Combine(ServerPath, "testLua.lua"));       //make this nicer!
        }

        public void ServerTick()
        {
            foreach (Guid g in activePlayers.Values.Select(x => x.mapId).Distinct().ToList())
            {
                Map m = loader.GetMap(g);
                foreach (Tile t in m.tiles)
                {
                    t.OnTick();
                }
                foreach (Entity e in m.entities)
                {
                    e.OnTick();
                }
            }

            foreach (Guid m in mapUpdates.Select(x => x.mapId).Distinct())
            {
                NetworkResponse p = new NetworkResponse();
                p.type = NetworkResponse.ResponseType.MapUpdate;
                MapUpdate[] up = mapUpdates.Where(x => x.mapId == m).ToArray();
                p.AppendObject((BinaryWriter w) =>
                {
                    w.Write(up.Length);
                    foreach (MapUpdate u in up)
                        w.Write(u.ToBytes());
                });

                foreach (Guid g in activePlayers.Keys.Where(x => activePlayers[x].mapId.Equals(m)))
                {
                    NetworkHandler.Instance.SendNetworkResponse(g, p);
                }
            }
            mapUpdates.Clear();
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
                        if (a.ComparePassword(new Guid(r.requestParams[0])) && login.ValidateSessionId(new Guid(r.requestParams[2]), new Guid(r.requestParams[3])))
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
                        if (login.FindAccount(r.requestParams[0]) == null)
                            login.CreateAccount(r.requestParams[0], new Guid(r.requestParams[1]));
                    }
                    break;
                case NetworkRequest.RequestType.CreatePlayer:
                    //0=playerName, 1=account, 2=sessionID
                    {
                        if (login.ValidateSessionId(new Guid(r.requestParams[1]), new Guid(r.requestParams[2])))
                        {
                            Player pl = login.CreatePlayer(r.requestParams[0], new Guid(r.requestParams[1]), loader.GetMap("Test Map").Id, 3, 3);
                            RegisterPlayer(pl, g);
                        }
                    }
                    break;
                case NetworkRequest.RequestType.JoinPlayer:
                    {
                        //0=playerName, 1=account, 2=sessionID
                        Player pp = login.FindPlayer(new Guid(r.requestParams[1]), r.requestParams[0]);
                        if (login.ValidateSessionId(new Guid(r.requestParams[1]), new Guid(r.requestParams[2])))
                        {
                            RegisterPlayer(pp, g);
                            RespondPlayerJoined(true, g);
                        }
                        else
                            RespondPlayerJoined(false, g);
                    }
                    break;
                case NetworkRequest.RequestType.GetKeybinds:
                    {
                        Keybind[] keybinds = Utils.FileUtils.LoadFromXml<Keybind[]>(Path.Combine(ServerPath, "keybindings.xml"));
                        RespondKeybinds(keybinds, g);
                    }
                    break;
                case NetworkRequest.RequestType.PlayerInput:
                    {
                        //handle params here later!
                        lua.CallFunc(Path.Combine(ServerPath, "testLua.lua"), r.requestAction, activePlayers[g]);
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
                    w.Write(players.Length); foreach (Player pl in players) w.Write(pl.name); w.Write(login.GenSessionId(a.Id).ToString()); w.Write(a.Id.ToString());
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

        void RespondPlayerJoined(bool b, Guid g)
        {
            NetworkResponse p = new NetworkResponse();
            p.type = NetworkResponse.ResponseType.PlayerJoined;
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
                w.Write(keybinds.Length);
                foreach (Keybind b in keybinds)
                {
                    w.Write(b.action); w.Write((int)b.key); w.Write((int)b.altKey);
                }
            });
            NetworkHandler.Instance.SendNetworkResponse(g, p);
        }

        void RespondMap(Map m, Guid g)
        {
            NetworkResponse p = new NetworkResponse();
            p.type = NetworkResponse.ResponseType.MapInfo;
            if (m == null)
                p.error = NetworkResponse.ErrorCode.DataNotFound;
            p.AppendObject((BinaryWriter w) =>
            {
                w.Write(m.name);
                w.Write(m.sizeX);
                w.Write(m.sizeY);
                foreach (Tile t in m.tiles)
                {
                    w.Write(t.covered);
                    w.Write(t.lightLvl);
                    w.Write(t.itemType);
                    w.Write(t.tileType);
                }
            });
        }

        string ServerPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serverInf.name);
            }
        }
    }
}