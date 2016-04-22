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
        public event EventHandler OnDisconnect;

        ClientInf clientInf;
        string clientPath;
        Dictionary<string, Keybind[]> loadedKeybinds;

        GameWindow window;
        InputHandler input;

        LoginScreen loginScreen;
        MapScreen mapScreen;

        ClientGameState gameState;
        bool inputLock;
        bool textParser;
        bool isWriting;

        public ClientMain(ClientInf inf, string path, GameWindow window, InputHandler input, Action<object, EventArgs> OnClientClose)
        {
            clientInf = inf;
            clientPath = path;

            this.window = window;
            this.input = input;

            loginScreen = new LoginScreen(window, input, this);
            mapScreen = new MapScreen(window, input);
            loadedKeybinds = new Dictionary<string, Keybind[]>();
            gameState = new ClientGameState();

            OnDisconnect += new EventHandler(OnClientClose);

            NetworkHandler.Instance.ConnectToServer(System.Net.IPAddress.Parse(inf.ip), inf.port, this);
        }

        public void ClientTick()
        {
            if (CurrentKeybinds == null || inputLock) return;

            if (textParser)
            {
                if (!isWriting)
                {
                    isWriting = true;
                    input.CatchLine((string s) =>
                    {
                        RequestActionCommand(s);
                        isWriting = false;
                    });
                }
            }
            else
            {
                input.CatchInput();
                foreach (Keybind b in CurrentKeybinds)
                {
                    if (input.GetKey(b.key, b.altKey))
                    {
                        RequestAction(b.action, null);
                        inputLock = true;
                    }
                }
                if (input.GetKey(KeyCode.Escape))
                    DisconnectClient();
                else if (input.GetKey(KeyCode.F1))
                    ToggleTextParser();
                else if (input.GetKey(KeyCode.F2))
                    ListAllKeybinds();
            }
        }

        void DisconnectClient()
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.Disconnect,
                requestAction = "none",
            });
            NetworkHandler.Instance.DisconnectClient();
            OnDisconnect(this, null);
        }

        void ToggleTextParser()
        {
            textParser = !textParser;
        }

        void ListAllKeybinds()
        {
            foreach (Keybind k in CurrentKeybinds)
                Console.WriteLine("[" + k.key + "] " + k.action);
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
                            string sessionToken = null;
                            Guid accountId = new Guid();

                            r.ReadObject((BinaryReader p) =>
                            {
                                allPlayers = new string[p.ReadInt32()]; for (int i = 0; i < allPlayers.Length; i++) allPlayers[i] = p.ReadString(); sessionToken = p.ReadString(); accountId = new Guid(p.ReadString());
                            });

                            loginScreen.LoginPlayer(allPlayers, accountId, sessionToken);      //change this! delegate?
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

                        ClientGameState s = null;
                        r.ReadObject((BinaryReader p) =>
                        {
                            s = new ClientGameState()
                            {
                                mapName = p.ReadString(),
                            };
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

        public void RequestSpawn(string p, Guid account, string sessionId)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.JoinPlayer,
                requestAction = "none",
                requestParams = new string[]
            {
                    p,
                    account.ToString(),
                    sessionId,
            }
            });
        }

        public void RequestCreatePlayer(string p, Guid account, string sessionId)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.CreatePlayer,
                requestAction = "none",
                requestParams = new string[]
            {
                p,
                account.ToString(),
                sessionId,
            }
            });
        }

        public void RequestChangePassword(Guid oldP, Guid newP, Guid account, string sessionId)
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
                sessionId,
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

        public void RequestActionCommand(string s)
        {
            if (s == null || s.Length == 0) return;

            int ind = s.IndexOf(' ');
            if (ind > 0)
            {
                string action = s.Substring(0, ind);
                string arg = s.Substring(ind + 1, s.Length - ind - 1);
                RequestAction(action, arg.Length == 0 ? null : new string[] { arg });
            }
        }

        public void RequestAction(string s, string[] args)
        {
            NetworkHandler.Instance.SendNetworkRequest(new NetworkRequest()
            {
                requestType = NetworkRequest.RequestType.PlayerInput,
                requestAction = s,
                requestParams = args,
            });
        }

        void SaveClientInf()
        {
            Utils.FileUtils.SaveToMog<ClientInf>(clientInf, clientPath);
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
                if (loadedKeybinds != null && gameState != null && gameState.playerState != null && loadedKeybinds.ContainsKey(gameState.playerState))
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