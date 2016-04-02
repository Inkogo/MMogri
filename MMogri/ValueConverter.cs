using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using MMogri.Gameplay;

namespace MMogri.Utils
{
    public static class ValueConverter
    {
        static Dictionary<Type, Converter> converters = new Dictionary<Type, ValueConverter.Converter>()
    {
        { typeof(string), new ConverterString() },
        { typeof(int), new ConverterInt() },
        { typeof(float), new ConverterFloat() },
        { typeof(Map), new ConverterMap() },
    };

        public static object ConvertToValue(Type t, byte[] b)
        {
            if (!converters.ContainsKey(t)) return null;

            return converters[t].ToValue(b);
        }

        public static byte[] ConvertToBytes(Type t, object o)
        {
            if (!converters.ContainsKey(t)) return null;

            return converters[t].ToBytes(o);
        }

        public abstract class Converter
        {
            public object ToValue(byte[] b)
            {
                using (MemoryStream stream = new MemoryStream(b))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        return ReadBytes(reader);
                    }
                }
            }
            public byte[] ToBytes(object o)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        WriteObject(writer, o);
                        return stream.ToArray();
                    }
                }
            }
            public abstract object ReadBytes(BinaryReader r);
            public abstract void WriteObject(BinaryWriter w, object o);
        }

        public class ConverterString : Converter
        {
            public override object ReadBytes(BinaryReader r)
            {
                return r.ReadString();
            }
            public override void WriteObject(BinaryWriter w, object o)
            {
                w.Write((string)o);
            }
        }

        public class ConverterInt : Converter
        {
            public override object ReadBytes(BinaryReader r)
            {
                return r.ReadInt32();
            }
            public override void WriteObject(BinaryWriter w, object o)
            {
                w.Write((int)o);
            }
        }

        public class ConverterFloat : Converter
        {
            public override object ReadBytes(BinaryReader r)
            {
                return r.ReadSingle();
            }
            public override void WriteObject(BinaryWriter w, object o)
            {
                w.Write((float)o);
            }
        }

        public class ConverterMap : Converter
        {
            public override object ReadBytes(BinaryReader r)
            {
                return Map.FromBytes(r);
            }
            public override void WriteObject(BinaryWriter w, object o)
            {
                ((Map)o).WriteBytes(w);
            }
        }
    }
}
