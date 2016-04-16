using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Core
{
    [System.Serializable]
    public struct ServerInf
    {
        public string name;
        public int maxPlayers;
        public int port;

        public string authentificationEmail;
        public string authentificationEmailPassword;

        public bool validateEmail;

        public int saveTickInterval;
    }
}