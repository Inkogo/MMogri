using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MMogri
{
    [System.Serializable]
    public class CharacterStats : ICompressable
    {
        public int strength;
        public int dexterity;
        public int constitution;
        public int intelligence;
        public int wisdom;
        public int charisma;

        public byte[] ToBytes()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(mem))
                {
                    writer.Write(strength);
                    writer.Write(dexterity);
                    writer.Write(constitution);
                    writer.Write(intelligence);
                    writer.Write(wisdom);
                    writer.Write(charisma);

                    return mem.ToArray();
                }
            }
        }

        public void FromBytes(byte[] b)
        {
            using (MemoryStream mem = new MemoryStream(b))
            {
                using (BinaryReader reader = new BinaryReader(mem))
                {
                    strength = reader.ReadInt32();
                    dexterity = reader.ReadInt32();
                    constitution = reader.ReadInt32();
                    intelligence = reader.ReadInt32();
                    wisdom = reader.ReadInt32();
                    charisma = reader.ReadInt32();
                }
            }
        }
    }
}
