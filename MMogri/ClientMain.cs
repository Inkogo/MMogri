﻿using MMogri.Input;
using MMogri.Network;
using MMogri.Renderer;
using MMogri.Utils;
using System;
using System.IO;

namespace MMogri
{
    class ClientMain
    {
        ClientInf clientInf;
        Keybind[] keybinds;

        GameWindow window;
        InputHandler input;

        LoginScreen loginScreen;
        MapScreen mapScreen;

        bool inputLock;

        public ClientMain(ClientInf inf, GameWindow window, InputHandler input)
        {
            clientInf = inf;

            this.window = window;
            this.input = input;

            loginScreen = new LoginScreen(window, input, this);
            mapScreen = new MapScreen(window, input);

            NetworkHandler.Instance.ConnectToServer(System.Net.IPAddress.Parse(inf.ip), inf.port, this);
        }

        public void ClientTick()
        {
            if (keybinds == null || inputLock) return;

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
                    inputLock = true;
                }
            }
        }

        public void ProcessNetworkResponse(NetworkResponse r)
        {
            switch (r.type)
            {
                case NetworkResponse.ResponseType.AccountLogin:
                    if (r.error == NetworkResponse.ErrorCode.None)
                    {
                        string[] allPlayers = null;
                        Guid sessionId = new Guid();
                        Guid accountId = new Guid();

                        r.ReadObject((BinaryReader p) =>
                        {
                            allPlayers = new string[p.ReadInt32()]; for (int i = 0; i < allPlayers.Length; i++) allPlayers[i] = p.ReadString(); sessionId = new Guid(p.ReadString()); accountId = new Guid(p.ReadString());
                        });

                        loginScreen.LoginPlayer(allPlayers, accountId, sessionId);      //change this! delegate?
                    }
                    else
                    {
                        Console.WriteLine("Wrong username or password!");
                        loginScreen.LoginAccount();
                    }
                    break;
                case NetworkResponse.ResponseType.PlayerJoined:
                    Console.WriteLine("Successfully joined player!");
                    if (!LoadKeybinds())
                        RequestDefaultKeybinds();

                    break;
                case NetworkResponse.ResponseType.KeybindsInfo:
                    Keybind[] k = null;
                    r.ReadObject((BinaryReader p) =>
                    {
                        k = new Keybind[p.ReadInt32()];
                        for (int i = 0; i < k.Length; i++)
                        {
                            k[i] = new Keybind(p.ReadString(), (KeyCode)p.ReadInt32(), (KeyCode)p.ReadInt32());
                        }
                    });
                    keybinds = k;
                    SaveKeybinds();

                    break;
                case NetworkResponse.ResponseType.Update:
                    inputLock = false;
                    Console.WriteLine("Change! " + System.DateTime.Now.ToString("mm:ss:ff"));

                    //mapScreen.UpdateMap();
                    break;
            }
        }

        public void OnJoinServer()
        {
            //loginScreen.Start();

            loginScreen.LoginAccount();
        }

        public void RequestLoginAccount(string name, Guid password)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.JoinAccount,
                requestAction = "none",
                requestParams = new string[]
            {
                  name,
                  password.ToString(),
            }
            });
        }

        public void RequestCreateAccount(string name, Guid password)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.CreateAccount,
                requestAction = "none",
                requestParams = new string[]
            {
                name,
                password.ToString(),
            }
            });
        }

        public void RequestSpawn(string p, Guid account, Guid sessionId)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.JoinPlayer,
                requestAction = "none",
                requestParams = new string[]
            {
                    p,
                    account.ToString(),
                    sessionId.ToString(),
            }
            });
        }

        public void RequestCreatePlayer(string p, Guid account, Guid sessionId)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.CreatePlayer,
                requestAction = "none",
                requestParams = new string[]
            {
                p,
                account.ToString(),
                sessionId.ToString(),
            }
            });
        }

        public void RequestChangePassword(Guid oldP, Guid newP, Guid account, Guid sessionId)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.ChangePassword,
                requestAction = "none",
                requestParams = new string[]
            {
                oldP.ToString(),
                newP.ToString(),
                account.ToString(),
                sessionId.ToString(),
            }
            });
        }

        public void RequestResetPassword(string email)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.ResetPassword,
                requestAction = "none",
                requestParams = new string[]
            {
                email,
            }
            });
        }

        public void RequestDefaultKeybinds()
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.GetKeybinds,
                requestAction = "none",
            });
        }

        void SaveKeybinds()
        {
            Directory.CreateDirectory(ClientPath);
            FileUtils.SaveToXml<Keybind[]>(keybinds, KeybindsPath);
        }

        bool LoadKeybinds()
        {
            if (File.Exists(KeybindsPath))
            {
                keybinds = FileUtils.LoadFromXml<Keybind[]>(KeybindsPath);
                return false;
            }
            else
                return false;
        }

        string ClientPath
        {
            get
            {
                return (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, clientInf.name));
            }
        }

        string KeybindsPath
        {
            get
            {
                return Path.Combine(ClientPath, "keybindings.xml");
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

