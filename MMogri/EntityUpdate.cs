using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MMogri
{
    class EntityUpdate : ClientUpdate
    {
        public Guid entityId;

        public bool changePosition;
        public int x;
        public int y;

        public EntityUpdate(Guid mapId, Guid entityId, int x = -1, int y = -1) : base(mapId, ClientUpdateType.entityUpdate)
        {
            this.entityId = entityId;
            if (x >= 0 || y >= 0)
            {
                changePosition = true;
                this.x = x;
                this.y = y;
            }
        }

        public override void WriteToBytes(BinaryWriter writer)
        {
            writer.Write(entityId.ToString());

            writer.Write(changePosition);
            if (changePosition)
            {
                writer.Write(x);
                writer.Write(y);
            }
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
