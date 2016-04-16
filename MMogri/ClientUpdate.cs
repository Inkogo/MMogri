using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MMogri
{
    abstract public class ClientUpdate : ICompressable
    {
        public enum ClientUpdateType
        {
            mapUpdate = 0,
            entityUpdate = 1,
            //playerUpdate = 2,
            //worldUpdate = 3,
        }

        static Dictionary<short, Type> updateTypes = new Dictionary<short, Type>()
        {
            { 0, typeof(MapUpdate)},
            { 1, typeof(EntityUpdate)},
//{ 2, typeof(PlayerUpdate)},
//{ 3, typeof(WorldUpdate)},
        };

        public Guid mapId;
        public ClientUpdateType type;

        public ClientUpdate(Guid m, ClientUpdateType t)
        {
            mapId = m;
            type = t;
        }

        public byte[] ToBytes()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(mem))
                {
                    writer.Write((short)GetTypeId());
                    WriteToBytes(writer);

                    return mem.ToArray();
                }
            }
        }

        public static void CreateFromBytes(byte[] b)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryReader reader = new BinaryReader(mem))
                {
                    short id = reader.ReadInt16();
                    ClientUpdate t = System.Activator.CreateInstance(updateTypes[id]) as ClientUpdate;
                    t.ReadFromBytes(reader);
                }
            }
        }

        abstract public void WriteToBytes(BinaryWriter w);
        abstract public void ReadFromBytes(BinaryReader r);
        abstract public void FromBytes(byte[] b);

        int GetTypeId()
        {
            Type t = this.GetType();
            foreach (short i in updateTypes.Keys)
                if (updateTypes[i] == t) return i;
            return -1;
        }
    }
}
