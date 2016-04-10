using System.Net;
using System.Net.Sockets;
using System;
using MMogri.Core;
using MMogri.Debugging;
using MMogri.Utils;

namespace MMogri.Network
{
    class NetworkHandler
    {
        Server server;
        Connection<NetworkResponse> connection;

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

        public void StartServer(int port, ServerMain serverMain)
        {
            server = new Server();
            server.StartServer(port, (Guid g, NetworkMessage m) => serverMain.ProcessNetworkRequest((NetworkRequest)m, g));
        }

        public void ConnectToServer(IPAddress ip, int port, ClientMain clientMain)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                s.Connect(ip, port);
                connection = new Connection<NetworkResponse>(s);
                connection.OnReceiveMessage = (NetworkResponse m) => clientMain.ProcessNetworkResponse(m);
                clientMain.OnJoinServer();
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Debug.Log(e.Message);
            }
        }

        public void SendNetworkRequest(NetworkRequest r)
        {
            connection.sock.Send(PackNetworkMessage(r));
        }

        public void SendNetworkResponse(Guid connection, NetworkResponse r)
        {
            server.connections[connection].sock.Send(PackNetworkMessage(r));
        }

        public byte[] PackNetworkMessage(NetworkMessage r)
        {
            byte[] b = r.ToBytes();
            byte[] i = BitConverter.GetBytes(b.Length);

            byte[] encoded = new byte[b.Length + i.Length];
            Buffer.BlockCopy(i, 0, encoded, 0, i.Length);
            Buffer.BlockCopy(b, 0, encoded, i.Length, b.Length);

            return encoded;
        }

        //[ICmd]
        //public static void CreateServer(int port)
        //{
        //    Instance.StartServer(port);
        //}

        //[ICmd]
        //public static void JoinServer(string ip, int port)
        //{
        //    IPAddress i;
        //    if (IPAddress.TryParse(ip, out i))
        //        Instance.ConnectToServer(i, port);
        //    else
        //        Debug.Log("Ip is not valid!");
        //}

        [Cmd]
        public void LeaveServer()
        {
            connection.sock.Disconnect(false);
            connection = null;
        }

        [Cmd]
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

        //[ICmd]
        //public static void TestRun()
        //{
        //    CreateServer(25565);
        //    //string p = GetPublicIp();
        //    JoinServer("192.168.0.10", 25565);
        //}

        //[ICmd]
        //public static void TestRequest()
        //{
        //    NetworkRequest r = new NetworkRequest()
        //    {
        //        requestAction = "test abc",
        //        requestParams = new string[] { "test01", "test02" },
        //        requestType = NetworkRequest.RequestType.PlayerInput
        //    };
        //    Instance.SendNetworkRequest(r);
        //}

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