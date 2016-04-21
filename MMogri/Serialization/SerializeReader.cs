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
            {typeof(char), new DeserializeConverterChar() },
            {typeof(Enum), new DeserializeConverterEnum() },
            {typeof(Guid), new DeserializeConverterGuid() },
            {typeof(List<>), new DeserializeConverterList() },
            {typeof(Dictionary<,>), new DeserializeConverterDictionary() },
            {typeof(Array), new DeserializeConverterArray() },
        };

        public T Deserialize<T>(string path) where T : new()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return (T)DeserializeObject(reader.ReadToEnd(), typeof(T));
            }
        }

        //creates an object of Type t and deserializes all lines of string o. 
        public object DeserializeObject(string s, Type nt)
        {
            if (s == "null")
                return null;

            foreach (Type t in converters.Keys)
            {
                if (nt.IsSubclassOf(t) || (nt.IsGenericType && nt.GetGenericTypeDefinition() == t) || nt == t)
                {
                    return converters[t].OnDeserialize(this, s, nt);
                }
            }
            return DeserializeValue(s, nt);
        }

        //takes a line zB "i=3" and deserializes it to an object of Type t
        public object DeserializeLine(string s, Type t)
        {
            string member = ReadMember(s);
            string value = ReadValue(s);
            if (member != null && value != null)
            {
                return DeserializeObject(value, t);
            }
            return null;
        }

        //takes member and value and assigns it to an object via reflection
        public MemberInfo DeserializeComplex(string s, Type t)
        {
            string member = ReadMember(s);
            MemberInfo m = t.GetMember(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)[0];
            if (m.GetCustomAttribute<System.NonSerializedAttribute>() != null) return null;

            return m;
        }

        //takes a value string zB "5" and converts it into an object of Type t
        public object DeserializeValue(string o, Type t)
        {
            object boxed = Activator.CreateInstance(t);
            using (StringReader reader = new StringReader(o))
            {
                while (reader.Peek() >= 0)
                {
                    string s = ReadNext(reader);
                    if (s == string.Empty) break;
                    //PROBLEM!!!
                    //I get the target type of complex variable from deserializeComplex.
                    //I need the type for deserialize line!
                    //I also need the value from deserialieLine for writing into complex!
                    MemberInfo i = DeserializeComplex(s, t);
                    object n = DeserializeLine(s, ((FieldInfo)i).FieldType);
                    ((FieldInfo)i).SetValue(boxed, n);
                }
                return boxed;
            }
        }

        public string ReadMember(string s)
        {
            if (s.Length == 0 || s[0] == '-') return null;

            int ind = s.IndexOf('=');
            if (ind >= 0)
            {
                return s.Substring(0, ind);
            }
            return null;
        }

        public string ReadValue(string s)
        {
            if (s.Length == 0 || s[0] == '-') return null;

            int ind = s.IndexOf('=');
            if (ind >= 0)
            {
                return s.Substring(ind + 1, s.Length - ind - 1);
            }
            return null;
        }


        //replace this with state machine to fix N depth problems!
        public string ReadNext(StringReader reader)
        {
            StringBuilder b = new StringBuilder();

            int layer = 0;
            bool val = false;

            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                //STATE: DEEP
                if (layer > 0)
                {
                    if (c == '{')
                    {
                        layer++;
                    }
                    if (c == '}')
                    {
                        layer--;
                        if (layer == 0) continue;
                    }
                }
                //STATE: VAL
                else if (val)
                {
                    if (c == '\n' || c == '\r')
                    {
                        continue;
                    }
                    if (c == '"')
                    {
                        val = false;
                        continue;
                    }
                }
                //STATE: DEFAULT
                else
                {
                    if (c == '\n' || c == '\r')
                    {
                        continue;
                    }
                    if (c == ' ')
                    {
                        continue;
                    }
                    if (c == '{')
                    {
                        layer++;
                        continue;
                    }
                    if (c == '"')
                    {
                        val = true;
                        continue;
                    }
                    if (layer == 0 && c == ';')
                    {
                        return b.ToString();
                    }
                }

                b.Append(c);

            }
            return b.ToString();
        }
    }
}
