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
        LuaScript lua;

        public ScriptableObject()
        {
            if (luaPath != null)
            {
                lua = new LuaScript(luaPath);
            }

            data = new Dictionary<string, byte[]>();
        }

        protected object[] CallLuaCallback(string s, params object[] o)
        {
            if (lua != null && s != null && s != "")
              return  lua.CallFunc(s, o);
            return null;
        }
    }
}
