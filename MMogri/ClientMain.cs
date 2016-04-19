using MMogri.Input;
using MMogri.Network;
using MMogri.Renderer;
using MMogri.Utils;
using System;
using System.IO;
using System.Collections.Generic;

namespace MMogri
{
    class ClientMain
    {
        ClientInf clientInf;
        string clientPath;
        Dictionary<string, Keybind[]> loadedKeybinds;

        GameWindow window;
        InputHandler input;

        LoginScreen loginScreen;
        MapScreen mapScreen;

        ClientGameState gameState;
        bool inputLock;

        public ClientMain(ClientInf inf, string path, GameWindow window, InputHandler input)
        {
            clientInf = inf;
            clientPath = path;

            this.window = window;
            this.input = input;

            loginScreen = new LoginScreen(window, input, this);
            mapScreen = new MapScreen(window, input);
            loadedKeybinds = new Dictionary<string, Keybind[]>();
            gameState = new ClientGameState();

            NetworkHandler.Instance.ConnectToServer(System.Net.IPAddress.Parse(inf.ip), inf.port, this);
        }

        public void ClientTick()
        {
            if (CurrentKeybinds == null || inputLock) return;

            input.CatchInput();
            foreach (Keybind b in CurrentKeybinds)
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
            //default input
            if (input.GetKey(KeyCode.H))
                foreach (Keybind k in CurrentKeybinds) Console.Write("[" + k.key + "] " + k.action);
        }

        public void ProcessNetworkResponse(NetworkResponse r)
        {
            switch (r.type)
            {
                case NetworkResponse.ResponseType.AccountLogin:
                    {
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
                    }
                    break;
                case NetworkResponse.ResponseType.ClientState:
                    {
                        Console.WriteLine("Successfully joined player!");

                        //serialize clientState for the first time here?
                        //s.FromBytes()
                        //string n = null;
                        ClientGameState s = null;
                        r.ReadObject((BinaryReader p) =>
                        {
                            s = new ClientGameState()
                            {
                                mapName = p.ReadString(),

                            };
                            //n = p.ReadString();
                        });

                        if (!loadedKeybinds.ContainsKey(s.playerState) && !LoadKeybinds(s.playerState))
                            RequestDefaultKeybinds();
                    }
                    break;
                case NetworkResponse.ResponseType.KeybindsInfo:
                    {
                        string n = null;
                        Keybind[] k = null;
                        r.ReadObject((BinaryReader p) =>
                        {
                            n = p.ReadString();
                            k = new Keybind[p.ReadInt32()];
                            for (int i = 0; i < k.Length; i++)
                            {
                                k[i] = new Keybind(p.ReadString(), (KeyCode)p.ReadInt32(), (KeyCode)p.ReadInt32());
                            }
                        });

                        loadedKeybinds.Add(n, k);
                        SaveKeybinds(n, k);
                    }
                    break;
                case NetworkResponse.ResponseType.Update:
                    {
                        inputLock = false;
                        Console.WriteLine("Change! " + System.DateTime.Now.ToString("mm:ss:ff"));

                        //mapScreen.UpdateMap();
                    }
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

        void SaveClientInf()
        {
            Utils.FileUtils.SaveToXml<ClientInf>(clientInf, clientPath);
        }

        //not sure if I like this...
        void SaveKeybinds(string playerState, Keybind[] k)
        {
            string path = Path.Combine(ClientDirectory, playerState + ".xml");
            FileUtils.SaveToXml<Keybind[]>(k, path);
            clientInf.keybinds.Add(playerState, path);

            SaveClientInf();
        }

        bool LoadKeybinds(string playerState)
        {
            if (clientInf.keybinds.ContainsKey(playerState))
            {
                loadedKeybinds.Add(playerState, FileUtils.LoadFromXml<Keybind[]>(clientInf.keybinds[playerState]));
                return false;
            }
            else
                return false;
        }

        Keybind[] CurrentKeybinds
        {
            get
            {
                if (loadedKeybinds.ContainsKey(gameState.playerState))
                    return loadedKeybinds[gameState.playerState];
                return null;
            }
        }

        string ClientDirectory
        {
            get
            {
                return Path.GetDirectoryName(clientPath);
            }
        }
    }
}