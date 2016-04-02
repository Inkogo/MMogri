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
        public class ResponsePackage
        {
            public enum ResponseType
            {
                AccountLogin,
                AccountCreate,
                KeybindsInfo,
            }

            public ResponseType type;
            public byte[] data;

            public ResponsePackage (ResponseType t, byte[] b)
            {
                type = t;
                data = b;
            }

            public byte[] ToBytes ()
            {
                byte[] i = BitConverter.GetBytes((int)type);

                byte[] encoded = new byte[data.Length + i.Length];
                Buffer.BlockCopy(i, 0, encoded, 0, i.Length);
                Buffer.BlockCopy(data, 0, encoded, i.Length, data.Length);

                return encoded;
            }
        }

        public enum ErrorCode
        {
            None,
            FailedLogin,
        }

        public ErrorCode error;
        public List<ResponsePackage> packages;

        public void AppendObject<T>(ResponsePackage.ResponseType r, T t)
        {
            byte[] b = Utils.ValueConverter.ConvertToBytes(typeof(T), t);
            packages.Add(new ResponsePackage(r, b));
        }

        protected override void ReadData(BinaryReader reader)
        {
            error = (ErrorCode)reader.ReadInt32();
            packages = new List<ResponsePackage>();
            for (int i = 0; i < reader.ReadInt32(); i++) {
                //figure out way to read package from stream
                //ResponsePackage r = new ResponsePackage();
                //packages.Add(r.FromBytes(reader.Read));
            }
        }

        protected override void WriteData(BinaryWriter write)
        {
            write.Write((int)error);
            write.Write((int)packages.Count);
            foreach (ResponsePackage p in packages)
                write.Write(p.ToBytes());
        }
    }
}
