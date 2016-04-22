using System;
using System.IO;

namespace MMogri
{
    class PlayerUpdate : ClientUpdate
    {
        Guid playerId;
        string playerState;

        public PlayerUpdate(Guid mapId, Guid playerId, string playerState) : base(mapId, ClientUpdateType.mapChangeUpdate)
        {
            this.playerId = playerId;
            this.playerState = playerState;
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
