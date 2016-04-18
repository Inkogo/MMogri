using MMogri.Debugging;
using NLua;
using System.Collections.Generic;
using System.Reflection;

namespace MMogri.Scripting
{
    class LuaHandler
    {
        Dictionary<string, Lua> lua;
        Dictionary<string, object> globalClasses;

        static LuaHandler _instance;

        public static LuaHandler Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LuaHandler();
                return _instance;
            }
        }

        public LuaHandler()
        {
            lua = new Dictionary<string, Lua>();
            globalClasses = new Dictionary<string, object>();
        }

        public void RegisterGlobalClass(string name, object o)
        {
            globalClasses.Add(name, o);
            foreach (string s in lua.Keys)
                RegisterClass(s, name, o);
        }

        public void RegisterClass(string path, string name, object o)
        {
            Lua l = lua[path];
            MethodInfo[] m = o.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo i in m)
            {
                LuaFunc f = i.GetCustomAttribute<LuaFunc>();
                if (f != null)
                {
                    l.RegisterFunction(name + "_" + i.Name, o, i);
                }
            }
            l[name] = o;
        }

        public void CreateLua(string path)
        {
            if (lua.ContainsKey(path)) return;

            Lua l = new Lua();
            InitLua(l, path);
            foreach (string s in globalClasses.Keys)
            {
                RegisterClass(s, s, globalClasses[s]);
            }

            lua.Add(path, l);
        }

        public object[] CallFunc(string n, string func, object[] o)
        {
            LuaFunction f = lua[n][func] as LuaFunction;
            if (f != null)
            {
                try
                {
                    return f.Call(o);
                }
                catch (NLua.Exceptions.LuaScriptException e)
                {
                    Debug.Log(e.Message);
                }
            }
            return null;
        }


        void InitLua(Lua l, string path)
        {
            try
            {
                l.DoFile(path);
            }
            catch (NLua.Exceptions.LuaScriptException e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}