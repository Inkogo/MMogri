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
        LuaHandler lua;

        public ServerMain (string serverPath)
        {
            this.serverPath = serverPath;
        }

        public void Start(GameWindow window)
        {
            GameLoader loader = new GameLoader();
            GameCore core = new GameCore(loader);

            loader.Load(ServerPath);
            lua = new LuaHandler();
            lua.RegisterObserved("Core", core);
        }

        void ProcessNetworkRequest(NetworkMessage m, Connection c)
        {
            NetworkRequest r = (NetworkRequest)m;

            switch (r.requestType)
            {
                case NetworkRequest.RequestType.JoinAccount:
                    Account a = LoginHandler.Instance.GetAccount(r.requestParams[0], new Guid(r.requestParams[1]));
                    //NetworkHandler.Instance.SendNetworkResponse(null);
                    break;
                case NetworkRequest.RequestType.PlayerInput:
                    Player p = null;        //somehow get player from connection
                    lua.CallFunc(Path.Combine(ServerPath, "testLua.lua"), r.requestAction, p);
                    break;
            }
        }

        string ServerPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serverPath);
            }
        }
    }
}