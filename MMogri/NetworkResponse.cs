using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMogri.Network;

namespace MMogri
{
    public class NetworkResponse : NetworkMessage
    {
        public enum ResponseType
        {
            Undefined,
            AccountLogin,
            AccountCreate,
            KeybindsInfo,
        }

        public enum ErrorCode
        {
            None,
            SecurityFailed,
            DataNotFound,
        }

        public ResponseType type;
        public ErrorCode error;
        public byte[] data;

        public NetworkResponse ()
        {
            type = ResponseType.Undefined;
            error = ErrorCode.None;
            data = new byte[0];
        }

        public void AppendObject(Action<BinaryWriter> w)
        {
            byte[] b;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    w(writer);
                    b = stream.ToArray();
                }
            }
            
            byte[] encoded = new byte[data.Length + b.Length];
            Buffer.BlockCopy(data, 0, encoded, 0, data.Length);
            Buffer.BlockCopy(b, 0, encoded, data.Length, b.Length);
            data = encoded;
        }

        public void ReadObject(Action<BinaryReader> r)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    r(reader);
                }
            }
        }

        protected override void ReadData(BinaryReader reader)
        {
            type = (ResponseType)reader.ReadInt32();
            error = (ErrorCode)reader.ReadInt32();
            data = reader.ReadBytes((int)reader.BaseStream.Length);
        }

        protected override void WriteData(BinaryWriter write)
        {
            write.Write((int)type);
            write.Write((int)error);
            write.Write(data);
        }
    }
}
