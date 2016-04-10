using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using MMogri.Debugging;

namespace MMogri.Scripting
{
    class LuaHandler
    {
        Lua lua;

        public LuaHandler()
        {
            lua = new Lua();
        }

        public void RegisterObserved(string name, object o)
        {
            MethodInfo[] m = o.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo i in m)
            {
                LuaFunc f = i.GetCustomAttribute<LuaFunc>();
                if (f != null)
                {
                    lua.RegisterFunction(name + "_" + i.Name, o, i);
                }
            }
            lua[name] = o;
        }

        public void Run(string path)
        {
            try
            {
                lua.DoFile(path);
            }
            catch (NLua.Exceptions.LuaScriptException e)
            {
                Debug.Log(e.Message);
            }
        }

        public void CallFunc(string path, string func, object o)
        {
            //Run(path);
            LuaFunction f = lua[func] as LuaFunction;
            if (f != null)
            {
                f.Call(o);
            }
        }
    }
}