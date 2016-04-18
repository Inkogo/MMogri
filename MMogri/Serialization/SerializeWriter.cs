using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MMogri.Serialization
{
    public class SerializeWriter
    {
        static Dictionary<Type, SerializeConverter> converters = new Dictionary<Type, SerializeConverter>()
        {
            {typeof(List<>), new SerializeConverterList() },
            {typeof(int), new SerializeConverterValueType() },
            {typeof(string), new SerializeConverterValueType() },
            {typeof(byte), new SerializeConverterValueType() },
            {typeof(Dictionary<,>), new SerializeConverterDictionary() },
        };

        StringBuilder stringB;
        int indent;

        public SerializeWriter()
        {
            indent = 0;
            stringB = new StringBuilder();
        }

        public void Serialize<T>(string path, T t) where T : new()
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                SerializeObject(t);

                writer.Write(ToString());
            }
        }


        void SerializeObject(Object t)
        {
            WriteStart();

            MemberInfo[] m = t.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            foreach (MemberInfo i in m)
            {
                if (i.GetCustomAttribute<System.NonSerializedAttribute>() != null) continue;

                object value = null;
                if (i.MemberType == MemberTypes.Field)
                {
                    value = ((FieldInfo)i).GetValue(t);
                }
                else continue;

                WriteEntry(i.Name, value);
            }

            WriteEnd();
        }

        public void WriteStart()
        {
            Append("{\n");
            indent++;
        }

        public void WriteEnd()
        {
            indent--;
            WriteIndent();
            Append("}");
        }

        public void WriteIndent ()
        {
            for (int i = 0; i < indent; i++) stringB.Append("  ");
        }

        public void Write(object e)
        {
            Append('"' + e.ToString() + '"');
        }

        public void WriteEntry(string s, object e)
        {
            WriteIndent();
            Append(s + "=");
            SerializeValue(e);
            Append(";\n");
        }

        void Append(string s)
        {
            stringB.Append(s);
        }

        void SerializeValue(object o)
        {
            if (o == null)
            {
                Append("NULL");
                return;
            }

            //look for converter
            foreach (Type t in converters.Keys)
            {
                Type nt = o.GetType();
                if (nt.IsSubclassOf(t) || (nt.IsGenericType && nt.GetGenericTypeDefinition() == t) || nt == t)
                {
                    converters[t].OnSerialize(this, o);
                    return;
                }
            }
            //no converter found! serialize it directly!
            SerializeObject(o);
        }

        public override string ToString()
        {
            return stringB.ToString();
        }
    }
}
