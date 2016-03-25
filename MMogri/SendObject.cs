using System.Collections;
using System.Net.Sockets;
using System.Text;

namespace MMogri.Network
{
    public class SendObject
    {
        public Socket workSocket = null;
        public const int bufferSize = 1024;
        public byte[] buffer = new byte[bufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}