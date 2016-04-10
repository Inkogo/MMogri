using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using MMogri.Debugging;

namespace MMogri.Core
{
    class CmdConsole
    {
        public class Cmd
        {
            public string actionName;
            public List<string> parameters;

            public Cmd()
            {
                actionName = "";
                parameters = new List<string>();
            }
        }

        Dictionary<string, MethodInfo> methods;

        static CmdConsole _instance;

        public static CmdConsole Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CmdConsole();
                return _instance;
            }
        }

        public CmdConsole()
        {
            methods = new Dictionary<string, MethodInfo>();
            GenAssemblyMethodList();
        }

        public void ExecCmd(string s)
        {
            using (StringReader reader = new StringReader(s))
            {
                Cmd cmd = new Cmd();

                if (!ReadTillChar(reader, ' ', out cmd.actionName))
                {
                    bool done = false;
                    while (!done)
                    {
                        string p;
                        done = ReadTillChar(reader, ' ', out p);
                        cmd.parameters.Add(p);
                    }
                }
                CallAction(cmd);
            }
        }

        bool ReadTillChar(StringReader reader, Char endChar, out string s)
        {
            s = "";
            while (reader.Peek() >= 0)
            {
                Char c = (Char)reader.Read();
                if (c == endChar) return false;
                s += c;
            }
            return true;
        }

        void CallAction(Cmd cmd)
        {
            if (!methods.ContainsKey(cmd.actionName)) {
                Debug.Log(cmd.actionName + " is not an action!");
                return; }

            MethodInfo m = methods[cmd.actionName];
            object[] p = new object[cmd.parameters.Count];
            ParameterInfo[] i = m.GetParameters();

            if( i.Length != cmd.parameters.Count)
            {
                Debug.Log("wrong parameter count!");
                return;
            }

            for (int n = 0; n < i.Length; n++)
            {
                ParameterInfo pi = i[n];
                p[n] = ParseParameter(pi, cmd.parameters[n]);
            }
            methods[cmd.actionName].Invoke(null, p);
        }

        object ParseParameter(ParameterInfo pi, string par)
        {
            Type t = pi.ParameterType;
            if (t == typeof(int))
            {
                int i;
                if (int.TryParse(par, out i))
                {
                    return (object)i;
                }
            }
            else if (t == typeof(float))
            {
                float f;
                if (float.TryParse(par, out f))
                {
                    Debug.Log(f);
                    return (object)f;
                }
            }
            else if (t == typeof(string))
            {
                return (object)par;
            }
            return null;
        }

        void GenAssemblyMethodList()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (MethodInfo i in t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    Core.Cmd c = i.GetCustomAttribute<Core.Cmd>();
                    if (c != null)
                        methods.Add(i.Name, i);
                }
            }
        }

        [Core.Cmd]
        public static void HelpParameters(string s)
        {
            if (!Instance.methods.ContainsKey(s))
            {
                Debug.Log("Method: " + s + " not found!");
            }
            MethodInfo m = Instance.methods[s];
            ParameterInfo[] p = m.GetParameters();
            string t = "Method " + s + ": ";
            for (int i = 0; i < p.Length; i++)
            {
                t += p[i].Name + ": (" + p[i].ParameterType.ToString() + ") ";
            }
            Debug.Log(t);
        }

        [Core.Cmd]
        public static void HelpList()
        {
            foreach(MethodInfo m in Instance.methods.Values)
            {
                Debug.Log(m.Name);
            }
        }

    }
}