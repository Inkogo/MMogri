using System.IO;
using System.Reflection;
using System.Text;

namespace MMogri.Serialization
{
    public class Serializer
    {
        static string ReadNext(StreamReader reader, char delimeter)
        {
            StringBuilder b = new StringBuilder();
            char? outCase = null;
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                if (outCase != null && c == (char)outCase) { outCase = null; continue; }
                else if(outCase != null && c !=(char)outCase) { }
                else if (c == '"') { outCase = '"'; continue; }
                else if (c == '{') { outCase = '}'; continue; }
                else if (c == '\n' || c == '\r') { continue; }
                else if (outCase == null && c == delimeter) { return b.ToString(); }

                b.Append(c);
            }
            return b.ToString();
        }

        public static void Serialize<T>(string path, T t) where T : new()
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                MemberInfo[] m = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                SerializeWriter w = new SerializeWriter();
                foreach (MemberInfo i in m)
                {
                    if (i.GetCustomAttribute<System.NonSerializedAttribute>() != null) continue;

                    object value = null;
                    if (i.MemberType == MemberTypes.Field)
                    {
                        value = ((FieldInfo)i).GetValue(t);
                    }
                    //else if (i.MemberType == MemberTypes.Property)
                    //{
                    //    if (((PropertyInfo)i).GetMethod != null)
                    //        value = ((PropertyInfo)i).GetValue(t);
                    //}
                    else continue;

                    w.WriteEntry(i.Name, value);
                    //writer.WriteLine(i.Name + "=" + (value != null ? value : "NULL") + ";");
                }
                writer.Write(w.ToString());
            }
        }

        public static T Deserialize<T>(string path) where T : new()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                object boxed = new T();
                SerializeReader r = new SerializeReader();

                while (reader.Peek() >= 0)
                {
                    string s = ReadNext(reader, ';');
                    if (s.Length == 0) break;

                    if (s[0] == '-') continue;

                    int ind = s.IndexOf('=');
                    if (ind >= 0)
                    {
                        string member = s.Substring(0, ind);
                        string value = s.Substring(ind + 1, s.Length - ind - 1);

                        MemberInfo m = typeof(T).GetMember(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)[0];
                        if (m.GetCustomAttribute<System.NonSerializedAttribute>() != null) continue;

                        if (m.MemberType == MemberTypes.Field)
                        {
                            r.ReadEntry(value, ((FieldInfo)m).FieldType);
                        }

                        //MemberInfo m = typeof(T).GetMember(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)[0];
                        //if (m.GetCustomAttribute<System.NonSerializedAttribute>() != null) continue;

                        //if (m.MemberType == MemberTypes.Field)
                        //{
                        //((FieldInfo)m).SetValue(boxed, Convert(value, ((FieldInfo)m).FieldType));
                        //}
                        //else if (m.MemberType == MemberTypes.Property)
                        //{
                        //    if (((PropertyInfo)m).SetMethod != null)
                        //        ((PropertyInfo)m).SetValue(boxed, Convert(value, ((PropertyInfo)m).PropertyType));
                        //}
                    }
                }

                return (T)boxed;
            }
        }
    }
}