using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Network;

namespace MMogri
{
    class NetworkResponse : NetworkMessage
    {
        public enum ResponseType
        {
            PlayersInfo,
            CharacterClassesInfo,
            RacesInfo,
            MapData,
            PlayerData,
            MapChange,
            GlobalChange,
            EntityChange,
        }

        public ResponseType type;
        public byte[] data;

        protected override void ReadData(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        protected override void WriteData(BinaryWriter write)
        {
            throw new NotImplementedException();
        }
    }
}
