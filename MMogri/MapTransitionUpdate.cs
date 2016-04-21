using System;
using System.IO;

namespace MMogri
{
    class MapTransitionUpdate : ClientUpdate
    {
        Guid oldMap;
        Guid targetPlayer;

        public MapTransitionUpdate(Guid newMap, Guid oldMap, Guid targetPlayer) : base(newMap, ClientUpdateType.mapChangeUpdate)
        {
            this.oldMap = oldMap;
            this.targetPlayer = targetPlayer;
        }

        public override void WriteToBytes(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void ReadFromBytes(BinaryReader r)
        {
            throw new NotImplementedException();
        }

        public override void FromBytes(byte[] b)
        {
            throw new NotImplementedException();
        }
    }
}
