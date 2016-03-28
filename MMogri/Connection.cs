using System.Collections;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;
using System.IO;

namespace MMogri.Network
{
    public class Connection
    {
        public Socket sock;
        SendObject sObj;

        int expectedSize = -1;
        byte[] outBuffer;
        int partition = 0;

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

            int bufferOffset = 0;
            if (expectedSize <= 0)
            {
                expectedSize = BitConverter.ToInt32(sObj.buffer, 0);
                outBuffer = new byte[expectedSize];
                bufferOffset = sizeof(int);
            }
            int s = expectedSize - partition;
            Buffer.BlockCopy(sObj.buffer, bufferOffset, outBuffer, partition, s < (SendObject.bufferSize - bufferOffset) ? s : (SendObject.bufferSize - bufferOffset));
            partition += sObj.buffer.Length - bufferOffset;

            if (partition >= expectedSize)
            {
                Debugging.Debug.Log(expectedSize + " bytes got!");
                NetworkRequest r = new NetworkRequest();
                r.FromBytes(outBuffer);
                //Do domething with request! get to it serverMain OR clientMain

                expectedSize = -1;
                partition = 0;
                outBuffer = new byte[0];
            }

            BeginReceive();
        }
    }
}