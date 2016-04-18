using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Serialization
{
    abstract class DeserializeConverter
    {
        abstract public object OnDeserialize(SerializeReader reader, string s);
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

    //class DeserializeConverterList : DeserializeConverter
    //{
    //    //override public object OnDeserialize(SerializeReader reader, string s)
    //    //{
    //    //    string t = reader.ReadNext();
    //    //    int length = (int)reader.DeserializeValue(t, typeof(int));

    //    //    string t0 = reader.ReadNext();
    //    //    string type = (string)reader.DeserializeValue(t0, typeof(string));

    //    //    Type tt = Type.GetType(type);

    //    //    IList list = (IList)CreateGenericReferenceType(tt);

    //    //    for (int i = 0; i < length; i++)
    //    //    {
    //    //        string z = reader.ReadNext();
    //    //        object k = reader.DeserializeString(z, tt);
    //    //        list.Add(k);
    //    //    }

    //    //    return list;
    //    //}

    //    object CreateGenericReferenceType(Type t)
    //    {
    //        Type n = typeof(List<>);
    //        Type[] typeArgs = { typeof(string) };
    //        Type construct = n.MakeGenericType(typeArgs);
    //        return Activator.CreateInstance(construct);
    //    }
    //}
}
