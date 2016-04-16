using System.IO;
using System.Reflection;
using System.Text;

namespace MMogri.Utils
{
    class Serializer
    {
        static string ReadNext(StreamReader reader, char delimeter)
        {
            StringBuilder b = new StringBuilder();
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                if (c == delimeter)
                {
                    return b.ToString();
                }

                if (c == '\n' || c == '\r')
                    continue;

                b.Append(c);
            }
            return b.ToString();
        }

        static object Convert(string s, System.Type t)
        {
            if (t == typeof(int))
            {
                int i;
                if (int.TryParse(s, out i))
                    return i;
                else return 0;
            }
            else if (t == typeof(float))
            {
                float f;
                if (float.TryParse(s, out f))
                    return f;
                else
                    return 0f;
            }
            else if (t == typeof(bool))
            {
                bool b;
                if (bool.TryParse(s, out b))
                    return b;
                return false;
            }
            else if (t == typeof(string))
                return s;
            else return null;
        }

        public static void Serialize<T>(string path, T t) where T : new()
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                MemberInfo[] m = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.GetField);

                foreach (MemberInfo i in m)
                {
                    if (i.GetCustomAttribute<System.NonSerializedAttribute>() != null) continue;

                    object value = null;
                    if (i.MemberType == MemberTypes.Field)
                        value = ((FieldInfo)i).GetValue(t);
                    else if (i.MemberType == MemberTypes.Property)
                        value = ((PropertyInfo)i).GetValue(t);

                    if (value != null)
                        writer.WriteLine(i.Name + "=" + value + ";");
                }
            }
        }

        public static T Deserialize<T>(string path) where T : new()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                object boxed = new T();

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
                        MemberInfo m = typeof(T).GetMember(member)[0];
                        if (m.GetCustomAttribute<System.NonSerializedAttribute>() != null) continue;

                        if (m.MemberType == MemberTypes.Field)
                        {
                            ((FieldInfo)m).SetValue(boxed, Convert(value, ((FieldInfo)m).FieldType));
                        }
                        else if (m.MemberType == MemberTypes.Property)
                        {
                            ((PropertyInfo)m).SetValue(boxed, Convert(value, ((PropertyInfo)m).PropertyType));
                        }
                    }
                }

                return (T)boxed;
            }
        }
    }
}