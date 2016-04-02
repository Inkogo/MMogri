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
        string serverPath;
        GameLoader loader;
        GameCore core;
        LuaHandler lua;
        LoginHandler login;

        Dictionary<Guid, Player> activePlayers;

        public ServerMain(string serverPath, GameWindow window)
        {
            this.serverPath = serverPath;

            loader = new GameLoader();
            core = new GameCore(loader);
            activePlayers = new Dictionary<Guid, Player>();

            loader.Load(ServerPath);

            lua = new LuaHandler();
            login = new LoginHandler(ServerPath);

            lua.RegisterObserved("Core", core);
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
        }

        public void ProcessNetworkRequest(NetworkRequest r, Guid g)
        {
            NetworkResponse resp = new NetworkResponse();

            switch (r.requestType)
            {
                case NetworkRequest.RequestType.JoinAccount:
                    //0=name, 1=passwordGuid
                    Account a = login.GetAccount(r.requestParams[0], new Guid(r.requestParams[1]));
                    //resp.type = NetworkResponse.ResponseType.AccountLogin;
                    resp.error = a == null ? NetworkResponse.ErrorCode.FailedLogin : NetworkResponse.ErrorCode.None;
                    //resp.AppendObject<Guid>(NetworkResponse.ResponsePackage.ResponseType.)
                    //resp.obj = a.Id.ToString();
                    NetworkHandler.Instance.SendNetworkResponse(g, resp);

                    break;
                case NetworkRequest.RequestType.CreateAccount:
                    login.CreateAccount(r.requestParams[0], new Guid(r.requestParams[1]));

                    break;
                case NetworkRequest.RequestType.CreatePlayer:
                    login.CreatePlayer(r.requestParams[0], new Guid(r.requestParams[1]), loader.GetMap("Test Map").Id, 3, 3);
                    break;
                case NetworkRequest.RequestType.JoinPlayer:
                    RegisterPlayer(login.GetPlayersOfAccount(new Guid(r.requestParams[0])).First(), g);
                    break;
                case NetworkRequest.RequestType.PlayerInput:
                    lua.CallFunc(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serverPath, "testLua.lua"), r.requestParams[0], activePlayers[g]);
                    break;
            }
        }

        void RegisterPlayer(Player p, Guid g)
        {
            if (activePlayers.ContainsKey(g)) return;
            activePlayers.Add(g, p);
        }

        //Keybind[] AutoGenKeybinds()
        //{
        //    lua.GetAllFuncs();
        //}

        string ServerPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serverPath);
            }
        }
    }
}