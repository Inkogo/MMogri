using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

namespace MMogri.Network
{
    public class Connection
    {
        public Socket sock;
        SendObject sObj;

        public Connection(Socket s)
        {
            sock = s;
            BeginReceive();
        }

        void BeginReceive()
        {
            sObj = new SendObject();
            this.sock.BeginReceive(
                    sObj.buffer, 0,
                    SendObject.bufferSize,
                    SocketFlags.None,
                    new AsyncCallback(this.OnBytesReceived),
                    this);
        }

        protected void OnBytesReceived(IAsyncResult result)
        {
            int nBytesRec = this.sock.EndReceive(result);
            if (nBytesRec <= 0)
            {
                this.sock.Close();
                return;
            }

            NetworkRequest r = NetworkRequest.FromBytes(sObj.buffer);
            NetworkHandler.Instance.ProcessNetworkRequest(r);

            BeginReceive();
        }
    }
}