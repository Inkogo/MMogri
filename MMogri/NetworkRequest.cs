using System.Collections;
using System.IO;

namespace MMogri.Network
{
    [System.Serializable]
    public class NetworkRequest
    {
        public enum RequestType
        {
            CallMethod, DirectCall
        }

        public enum RequestTarget
        {
            Server, Others
        }

        public RequestType requestType;
        public RequestTarget requestTarget;
        public byte[] data;

        public static NetworkRequest FromBytes(byte[] b)
        {
            using (MemoryStream stream = new MemoryStream(b))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    NetworkRequest req = new NetworkRequest();

                    req.requestTarget = (RequestTarget)reader.ReadInt32();
                    req.requestType = (RequestType)reader.ReadInt32();
                    int count = reader.ReadInt32();
                    req.data = reader.ReadBytes(count);

                    return req;
                }
            }
        }

        public byte[] ToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write((int)requestTarget);
                    writer.Write((int)requestType);
                    writer.Write(data.Length);
                    writer.Write(data);

                    return stream.ToArray();
                }
            }
        }

        public override string ToString()
        {
            return requestType + ", " + requestTarget + ", " + "Data size: " + data.Length;
        }
    }
}