using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri
{
    [System.Serializable]
    public class ClientInf
    {
        public string ip;
        public int port;
        public string name;

        public ClientInf() { }

        public ClientInf(string ip, int port, string name)
        {
            this.ip = ip;
            this.port = port;
            this.name = name;
        }
    }
}
