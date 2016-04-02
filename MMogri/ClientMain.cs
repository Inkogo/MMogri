using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Gameplay;
using MMogri.Core;
using MMogri.Scripting;
using MMogri.Input;
using System.IO;
using MMogri.Utils;
using MMogri.Renderer;
using MMogri.Network;
using System.Net;

namespace MMogri
{
    class ClientMain
    {
        string serverName;
        Keybind[] keybinds;

        GameWindow window;
        InputHandler input;

        public ClientMain(GameWindow window, InputHandler input)
        {
            this.window = window;
            this.input = input;
        }

        public void ClientTick()
        {
            if (keybinds == null) return;

            input.CatchInput();
            foreach (Keybind b in keybinds)
            {
                if (input.GetKey(b.key, b.altKey))
                {
                    NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
                    {
                        requestType = NetworkRequest.RequestType.PlayerInput,
                        requestAction = b.action,
                    });
                }
            }
        }

        public void ProcessNetworkResponse(NetworkResponse r)
        {
            foreach (NetworkResponse.ResponsePackage p in r.packages)
            {
                switch (p.type)
                {
                    case NetworkResponse.ResponsePackage.ResponseType.AccountCreate:
                        break;
                    case NetworkResponse.ResponsePackage.ResponseType.KeybindsInfo:
                        break;
                }
            }
            //Debugging.Debug.Log(r.type + ", " + r.error);
            //switch (r.type)
            //{
            //    case NetworkResponse.ResponseType.AccountLogin:
            //        if (r.error == NetworkResponse.ErrorCode.None)
            //            accountId = new Guid(r.obj);
            //        break;
            //    case NetworkResponse.ResponseType.KeybindsInfo:
            //        keybinds = ValueConverter.ConvertToValue(typeof(Keybind[]), r.obj);
            //}
            //STUUUUUUFF!
        }

        void Test()
        {
            if (File.Exists(Path.Combine(ClientPath, "keybindings.xml")))
                keybinds = FileUtils.LoadFromXml<Keybind[]>(Path.Combine(ClientPath, "keybindings.xml"));
            else
                NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
                {
                    requestType = NetworkRequest.RequestType.GetKeybinds,
                });
        }

        string ClientPath
        {
            get
            {
                return (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, serverName));
            }
        }
    }
}



//while (true)
//{
//    input.CatchInput();
//    if (input.GetKey(KeyCode.A))
//    {
//        NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
//        {
//            requestType = NetworkRequest.RequestType.JoinAccount,
//            requestAction = "none",
//            requestParams = new string[]
//            {
//                "Inko",
//                "dammit".ToGuid().ToString(),
//            }
//        });
//    }
//    if (input.GetKey(KeyCode.T))
//    {
//        NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
//        {
//            requestType = NetworkRequest.RequestType.CreateAccount,
//            requestAction = "none",
//            requestParams = new string[]
//            {
//                "Inko",
//                "dammit".ToGuid().ToString(),
//            }
//        });
//    }
//    if (input.GetKey(KeyCode.P))
//    {
//        Debugging.Debug.Log(accountId);
//        NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
//        {
//            requestType = NetworkRequest.RequestType.CreatePlayer,
//            requestAction = "none",
//            requestParams = new string[]
//            {
//                "Mac",
//                accountId.ToString(),
//            }
//        });
//    }
//}

