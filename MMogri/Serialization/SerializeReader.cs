using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MMogri.Serialization
{
    class SerializeReader
    {
        static Dictionary<Type, DeserializeConverter> converters = new Dictionary<Type, DeserializeConverter>()
        {
            {typeof(int), new DeserializeConverterInt() },
            {typeof(string), new DeserializeConverterString() },
            {typeof(bool), new DeserializeConverterBoolean() },
        };

        const char delimeter = ';';

        public SerializeReader()
        {
            //reader = new StringReader(s);
        }


        public T Deserialize<T>(string path) where T : new()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                //object boxed = new T();
                return (T)DeserializeObject(reader.ReadToEnd(), typeof(T));
            }
        }

        public object DeserializeObject(string o, Type t)
        {
            object boxed = Activator.CreateInstance(t);
            using (StringReader reader = new StringReader(o))
            {
                while (reader.Peek() >= 0)
                {
                    string s = ReadNext(reader);
                    if (s.Length == 0) break;

                    if (s[0] == '-') continue;

                    int ind = s.IndexOf('=');
                    if (ind >= 0)
                    {
                        string member = s.Substring(0, ind);
                        string value = s.Substring(ind + 1, s.Length - ind - 1);

                        MemberInfo m = t.GetMember(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)[0];
                        if (m.GetCustomAttribute<System.NonSerializedAttribute>() != null) continue;

                        if (m.MemberType == MemberTypes.Field)
                        {
                            object n = DeserializeValue(value, ((FieldInfo)m).FieldType);
                            ((FieldInfo)m).SetValue(boxed, n);
                        }
                    }
                }

                return boxed;
            }
        }

        public object DeserializeValue(string s, Type t)
        {
            object o = null;
            if (converters.ContainsKey(t))
                converters[t].OnDeserialize(this, s);
            else
                o = DeserializeObject(s, t);
            return o;
        }

        public string ReadNext(StringReader reader)
        {
            StringBuilder b = new StringBuilder();
            char? outCase = null;
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                if (c == '\n' || c == '\r') { continue; }
                if(outCase != '"' && (c==' ')) { continue; }
                else if (outCase != null && c == (char)outCase) { outCase = null; continue; }
                else if (outCase != null && c != (char)outCase) { }
                else if (c == '"') { outCase = '"'; continue; }
                else if (c == '{') { outCase = '}'; continue; }
                else if (outCase == null && c == delimeter) { return b.ToString(); }

                b.Append(c);
            }
            return b.ToString();
        }
    }
}
