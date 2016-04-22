using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MMogri.Serialization
{
    abstract class DeserializeConverter
    {
        abstract public object OnDeserialize(SerializeReader reader, string s, Type t);

        //move this in writer?
        protected object CreateGenericReferenceType(Type n, Type[] typeArgs)
        {
            Type construct = n.MakeGenericType(typeArgs);
            return Activator.CreateInstance(construct);
        }
    }

    class DeserializeConverterString : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            return s;
        }
    }

    class DeserializeConverterChar : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            if (s == null || s.Length == 0) return null;
            return s[0];
        }
    }

    class DeserializeConverterEnum : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            return Enum.Parse(t, s, true);
        }
    }

    class DeserializeConverterInt : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;
            return 0;
        }
    }

    class DeserializeConverterFloat : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            float f;
            if (float.TryParse(s, out f))
                return f;
            return 0f;
        }
    }

    class DeserializeConverterShort : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            short st;
            if (short.TryParse(s, out st))
                return st;
            return 0;
        }
    }

    class DeserializeConverterByte : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            byte b;
            if (byte.TryParse(s, out b))
                return b;
            return 0;
        }
    }

    class DeserializeConverterBoolean : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            bool b;
            if (bool.TryParse(s, out b))
                return b;
            return false;
        }
    }

    class DeserializeConverterGuid : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            Guid g;
            if (Guid.TryParse(s, out g))
                return g;
            return Guid.Empty;
        }
    }

    class DeserializeConverterList : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {   
            using (StringReader r = new StringReader(s))
            {
                int length = (int)reader.DeserializeLine(reader.ReadNext(r), typeof(int));

                IList list = (IList)Activator.CreateInstance(t);

                Type arg = t.GetGenericArguments()[0];

                for (int i = 0; i < length; i++)
                {
                    list.Add(reader.DeserializeLine(reader.ReadNext(r), arg));
                }

                return list;
            }
        }
    }

    class DeserializeConverterDictionary : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            using (StringReader r = new StringReader(s))
            {
                int length = (int)reader.DeserializeLine(reader.ReadNext(r), typeof(int));

                IDictionary dict = (IDictionary)Activator.CreateInstance(t);
                Type[] typeArgs = t.GetGenericArguments();

                for (int i = 0; i < length; i++)
                {
                    object o0 = reader.DeserializeLine(reader.ReadNext(r), typeArgs[0]);
                    object o1 = reader.DeserializeLine(reader.ReadNext(r), typeArgs[1]);
                    dict.Add(o0, o1);
                }

                return dict;
            }
        }
    }

    class DeserializeConverterArray : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s, Type t)
        {
            using (StringReader r = new StringReader(s))
            {
                int length = (int)reader.DeserializeLine(reader.ReadNext(r), typeof(int));

                Array arr = (Array)Activator.CreateInstance(t, length);

                for (int i = 0; i < length; i++)
                {
                    arr.SetValue(reader.DeserializeLine(reader.ReadNext(r), t.GetElementType()), i);
                }

                return arr;
            }
        }
    }
}