using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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

        StringReader reader;

        const char delimeter = ';';

        public SerializeReader(string s)
        {
            reader = new StringReader(s);
        }

        public object DeserializeString(string s, Type t)
        {
            object o = converters[t].OnDeserialize(this, s);
            return o;
        }

        public string ReadNext()
        {
            StringBuilder b = new StringBuilder();
            char? outCase = null;
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();

                if (outCase != null && c == (char)outCase) { outCase = null; continue; }
                else if (outCase != null && c != (char)outCase) { }
                else if (c == '"') { outCase = '"'; continue; }
                else if (c == '{') { outCase = '}'; continue; }
                else if (c == '\n' || c == '\r') { continue; }
                else if (outCase == null && c == delimeter) { return b.ToString(); }

                b.Append(c);
            }
            return b.ToString();
        }
    }
}
