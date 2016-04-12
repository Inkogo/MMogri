using System;
using System.Collections;
using System.IO;

namespace MMogri.Network
{
    [System.Serializable]
    public class NetworkRequest : NetworkMessage
    {
        public enum RequestType
        {
            JoinAccount,
            ChangePassword,
            ResetPassword,
            CreateAccount,
            JoinPlayer,
            CreatePlayer,
            Leave,
            GetKeybinds,
            PlayerInput,
            ClientMessage,
            Disconnect,
        }

        public RequestType requestType;
        public string requestAction;
        public string[] requestParams;

        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write((int)requestType);
            writer.Write(requestAction);
            if (requestParams != null)
            {
                writer.Write(requestParams.Length);
                foreach (string p in requestParams)
                    writer.Write(p);
            }
            else
                writer.Write(0);
            }

        protected override void ReadData(BinaryReader reader)
        {
            requestType = (RequestType)reader.ReadInt32();
            requestAction = reader.ReadString();
            requestParams = new string[reader.ReadInt32()];
            for (int i = 0; i < requestParams.Length; i++)
                requestParams[i] = reader.ReadString();
        }
    }
}