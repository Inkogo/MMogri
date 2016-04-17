using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MMogri.Scripting
{
    [System.Serializable]
    public abstract class ScriptableObject
    {
        public Dictionary<string, byte[]> data;

        public string luaPath;
        LuaHandler lua;

        public ScriptableObject()
        {
            if (luaPath != null)
            {
                lua = new LuaHandler(luaPath);
                //lua.RegisterObserved("Core", core);
            }

            data = new Dictionary<string, byte[]>();
        }

        protected void CallLuaCallback(string s, params object[] o)
        {
            if (lua != null && s != null && s != "")
                lua.CallFunc(s, o);
        }

        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("Keys", data.Keys.ToArray(), typeof(string[]));
        //    info.AddValue("Values", data.Values.ToArray(), typeof(byte[][]));
        //}

        //public ScriptableObject(SerializationInfo info, StreamingContext context)
        //{
        //    string[] k = (string[])info.GetValue("Keys", typeof(string[]));
        //    byte[][] v = (byte[][])info.GetValue("Values", typeof(byte[][]));
        //    data = Enumerable.Range(0, k.Length).ToDictionary(i => k[i], i => v[i]);
        //}
    }
}
