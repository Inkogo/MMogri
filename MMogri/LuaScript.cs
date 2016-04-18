using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Scripting
{
    class LuaScript
    {
        string path;
        public LuaScript(string path)
        {
            this.path = path;
            LuaHandler.Instance.CreateLua(path);
        }

        public object[] CallFunc(string func, params object[] objs)
        {
            return LuaHandler.Instance.CallFunc(path, func, objs);
        }
    }
}
