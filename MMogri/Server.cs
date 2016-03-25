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
        int port;
        IPAddress addr;

        public Dictionary<Guid, Connection> connections;

        public void StartServer(int p)
        {
            port = p;
            addr = IPAddress.Any;
            connections = new Dictionary<Guid, Connection>();

            this.sock = new Socket(
                addr.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(this.addr, this.port));
            this.sock.Listen(100);
            this.sock.BeginAccept(this.OnConnectRequest, sock);
            Debug.Log("Started Server at port: " + port);
        }

        void OnConnectRequest(IAsyncResult result)
        {
            Socket sock = (Socket)result.AsyncState;

            Debug.Log("Client connected to server! Ip: " + sock.LocalEndPoint);

            Connection newConn = new Connection(sock.EndAccept(result));
            connections.Add(Guid.NewGuid(), newConn);
            sock.BeginAccept(this.OnConnectRequest, sock);
        }
    }
}