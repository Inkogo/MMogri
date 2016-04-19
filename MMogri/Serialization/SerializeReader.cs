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
                        {typeof(byte), new DeserializeConverterByte() },
            {typeof(List<>), new DeserializeConverterList() },
                        {typeof(Dictionary<,>), new DeserializeConverterDictionary() },
        };

        const char delimeter = ';';

        public SerializeReader()
        {
        }


        public T Deserialize<T>(string path) where T : new()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return (T)DeserializeObject(reader.ReadToEnd(), typeof(T));
            }
        }

        //clean up this mess!
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

        //meeeeeessssssyyyyyy
        public object Test(string s, Type t)
        {
            if (s.Length == 0 || s[0] == '-') return null;

            int ind = s.IndexOf('=');
            if (ind >= 0)
            {
                string value = s.Substring(ind + 1, s.Length - ind - 1);

                return DeserializeValue(value, t);
            }
            return null;
        }

        public object DeserializeValue(string s, Type nt)
        {
            foreach (Type t in converters.Keys)
            {
                if (nt.IsSubclassOf(t) || (nt.IsGenericType && nt.GetGenericTypeDefinition() == t) || nt == t)
                {
                    return converters[t].OnDeserialize(this, s);
                }
            }
            return DeserializeObject(s, nt);
        }

        //replace this with state machine to fix N depth problems!
        public string ReadNext(StringReader reader)
        {
            StringBuilder b = new StringBuilder();
            char? outCase = null;
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                if (c == '\n' || c == '\r') { continue; }
                if (outCase != '"' && (c == ' ')) { continue; }
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
