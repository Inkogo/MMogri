using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Gameplay;
using MMogri.Network;

namespace MMogri
{
    class ClientHandler
    {
        Guid mapId;
        public Player player;
        public Map map;

        [RPC("RequestMapData")]
        public static void RequestMapData ()
        {
            //NetworkHandler.Instance.ServerRequest(new NetworkRequest() {
            //    requestType = NetworkRequest.RequestType.CallMethod,    //change this! every entry in delegate is one callback in networkHandler. classes can subscribe to callbacks!
            //});
        }
    }
}
