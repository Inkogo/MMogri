using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MMogri.Network
{
    [System.Serializable]
    abstract public class NetworkMessage
    {
        public void FromBytes(byte[] b)
        {
            using (MemoryStream stream = new MemoryStream(b))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    ReadData(reader);
                }
            }
        }

        public byte[] ToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    WriteData(writer);
                    
                    return stream.ToArray();
                }
            }
        }

        abstract protected void WriteData(BinaryWriter write);

        abstract protected void ReadData(BinaryReader reader);
    }
}
