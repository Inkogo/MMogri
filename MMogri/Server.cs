using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using MMogri.Debugging;

namespace MMogri.Network
{
    public class Server
    {
        Socket sock;
        Action<Guid, NetworkRequest> callback;
        public Dictionary<Guid, Connection<NetworkRequest>> connections;

        public void StartServer(int p, Action<Guid, NetworkMessage> c)
        {
            int port = p;
            IPAddress addr = IPAddress.Any;
            connections = new Dictionary<Guid, Connection<NetworkRequest>>();

            this.sock = new Socket(
                addr.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(addr, port));
            this.sock.Listen(100);
            this.sock.BeginAccept(this.OnConnectRequest, sock);
            this.callback = c;
            Debug.Log("Started Server at port: " + port);
        }

        void OnConnectRequest(IAsyncResult result)
        {
            Socket sock = (Socket)result.AsyncState;

            Debug.Log("Client connected to server! Ip: " + sock.LocalEndPoint);

            Connection<NetworkRequest> newConn = new Connection<NetworkRequest>(sock.EndAccept(result));
            Guid guid = Guid.NewGuid();
            connections.Add(guid, newConn);
            newConn.OnReceiveMessage = (NetworkRequest m) => callback(guid, m);
            sock.BeginAccept(this.OnConnectRequest, sock);
        }
    }
}