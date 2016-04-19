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
        abstract public object OnDeserialize(SerializeReader reader, string s);

        //move this in writer?
        protected object CreateGenericReferenceType(Type n, Type[] typeArgs)
        {
            Type construct = n.MakeGenericType(typeArgs);
            return Activator.CreateInstance(construct);
        }
    }

    class DeserializeConverterString : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            return s;
        }
    }

    class DeserializeConverterInt : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            int i;
            if (int.TryParse(s, out i))
                return i;
            return 0;
        }
    }

    class DeserializeConverterFloat : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            float f;
            if (float.TryParse(s, out f))
                return f;
            return 0f;
        }
    }

    class DeserializeConverterShort : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            short t;
            if (short.TryParse(s, out t))
                return t;
            return 0;
        }
    }

    class DeserializeConverterByte : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            byte b;
            if (byte.TryParse(s, out b))
                return b;
            return 0;
        }
    }

    class DeserializeConverterBoolean : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            bool b;
            if (bool.TryParse(s, out b))
                return b;
            return false;
        }
    }

    class DeserializeConverterList : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            //read list here!

            using (StringReader r = new StringReader(s))
            {
                int length = (int)reader.Test(reader.ReadNext(r), typeof(int));
                string typeName = (string)reader.Test(reader.ReadNext(r), typeof(string));
                Type t = Type.GetType(typeName);
                IList list = (IList)CreateGenericReferenceType(typeof(List<>), new Type[] { t });

                for (int i = 0; i < length; i++)
                {
                    list.Add(reader.Test(reader.ReadNext(r), t));
                }

                return list;
            }
        }
    }

    class DeserializeConverterDictionary : DeserializeConverter
    {
        override public object OnDeserialize(SerializeReader reader, string s)
        {
            //read list here!

            using (StringReader r = new StringReader(s))
            {
                int length = (int)reader.Test(reader.ReadNext(r), typeof(int));
                string typeName0 = (string)reader.Test(reader.ReadNext(r), typeof(string));
                Type t0 = Type.GetType(typeName0);

                string typeName1 = (string)reader.Test(reader.ReadNext(r), typeof(string));
                Type t1 = Type.GetType(typeName1);

                IDictionary dict = (IDictionary)CreateGenericReferenceType(typeof(Dictionary<,>), new Type[] { t0, t1 });

                for (int i = 0; i < length; i++)
                {
                    dict.Add(reader.Test(reader.ReadNext(r), t0), reader.Test(reader.ReadNext(r), t1));
                }

                return dict;
            }
        }
    }
}
