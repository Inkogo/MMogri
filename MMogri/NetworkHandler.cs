using System.Net;
using System.Net.Sockets;
using System;
using MMogri.Core;
using MMogri.Debugging;
using MMogri.Utils;

namespace MMogri.Network
{
    public class NetworkHandler
    {
        Server server;
        Connection connection;

        static NetworkHandler _instance;
        public static NetworkHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NetworkHandler();
                return _instance;
            }
        }

        public void StartServer(int port)
        {
            server = new Server();
            server.StartServer(port);
        }

        public void ConnectToServer(IPAddress ip, int port)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                s.Connect(ip, port);
                connection = new Connection(s);

            }
            catch (System.Net.Sockets.SocketException e)
            {
                Debug.Log(e.Message);
            }
        }

        public void SendToServer(byte[] b)
        {
            connection.sock.Send(b);
        }

        public void SendToClient(Guid g, byte[] b)
        {
            server.connections[g].sock.Send(b);
        }

        public void ProcessNetworkRequest(NetworkRequest r)
        {
            Debug.Log(r.ToString());
        }

        [ICmd]
        public static void CreateServer(int port)
        {
            Instance.StartServer(port);
        }

        [ICmd]
        public static void JoinServer(string ip, int port)
        {
            IPAddress i;
            if (IPAddress.TryParse(ip, out i))
                Instance.ConnectToServer(i, port);
            else
                Debug.Log("Ip is not valid!");
        }

        [ICmd]
        public void LeaveServer()
        {
            connection.sock.Disconnect(false);
            connection = null;
        }

        [ICmd]
        public static string GetPublicIp()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            Console.WriteLine(a4);

            return a4;
        }

        [ICmd]
        public static void SendDirectMessageToServer(string s)
        {
            Instance.SendToServer(new NetworkRequest()
            {
                requestTarget = NetworkRequest.RequestTarget.Server,
                requestType = NetworkRequest.RequestType.DirectCall,
                data = ValueConverter.ConvertToBytes(typeof(string), s)
            }.ToBytes());
        }

        [ICmd]
        public static void SendDirectMessageToClients(string s)
        {
            foreach (Guid g in Instance.server.connections.Keys)
            {
                Instance.SendToClient(g, new NetworkRequest()
                {
                    requestTarget = NetworkRequest.RequestTarget.Others,
                    requestType = NetworkRequest.RequestType.DirectCall,
                    data = ValueConverter.ConvertToBytes(typeof(string), s)
                }.ToBytes());
            }
        }

        [ICmd]
        public static void DebugConnections()
        {
            Debug.Log(Instance.server.connections.Count);
        }

//Remove this later!
        [ICmd]
        public static void TestSendMapData()
        {
            MMogri.Gameplay.Map m = new Gameplay.Map("Test", 40, 40);

            Instance.SendToServer(new NetworkRequest()
            {
                requestTarget = NetworkRequest.RequestTarget.Server,
                requestType = NetworkRequest.RequestType.DirectCall,
                data = ValueConverter.ConvertToBytes(typeof(Gameplay.Map), m)
            }.ToBytes());
        }

        [ICmd]
        public static void TestRun ()
        {
            CreateServer(25565);
            string p = GetPublicIp();
            JoinServer(p, 25565);
        }

        public bool IsClient
        {
            get
            {
                return connection != null;
            }
        }

        public bool IsServer
        {
            get
            {
                return server != null;
            }
        }
    }
}