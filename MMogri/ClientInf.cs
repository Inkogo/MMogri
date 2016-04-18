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
