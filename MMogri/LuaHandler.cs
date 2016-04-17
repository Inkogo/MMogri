using MMogri.Debugging;
using NLua;
using System.Reflection;

namespace MMogri.Scripting
{
    class LuaHandler
    {
        Lua lua;
        string path;

        public LuaHandler(string s)
        {
            lua = new Lua();
            path = s;
            Run();
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

        public void Run()
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

        public void CallFunc(string func, object[] o)
        {
            LuaFunction f = lua[func] as LuaFunction;
            if (f != null)
            {
                f.Call(o);
            }
        }
    }
}