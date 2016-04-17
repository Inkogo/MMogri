using System;
using System.Collections;
using System.Collections.Generic;
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

        public void WriteStart()
        {
            Append("{\n");
            indent++;
        }

        public void WriteEnd()
        {
            indent--;
            Append("}");
        }

        public void Write(object e)
        {
            Append('"' + e.ToString() + '"');
        }

        public void WriteEntry(string s, object e)
        {
            for (int i = 0; i < indent; i++) stringB.Append("  ");

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

            foreach (Type t in converters.Keys)
            {
                Type nt = o.GetType();
                if (nt.IsSubclassOf(t) || (nt.IsGenericType && nt.GetGenericTypeDefinition() == t) || nt == t)
                {
                    converters[t].OnSerialize(this, o);
                    return;
                }
            }
        }

        public override string ToString()
        {
            return stringB.ToString();
        }
    }
}
