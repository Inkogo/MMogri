using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMogri.Serialization
{
    abstract class SerializeConverter
    {
        abstract public void OnSerialize(SerializeWriter writer, object o);
    }

    class SerializeConverterValueType : SerializeConverter
    {
        override public void OnSerialize(SerializeWriter writer, object o)
        {
            writer.Write(o);
        }
    }

    class SerializeConverterList : SerializeConverter
    {
        override public void OnSerialize(SerializeWriter writer, object o)
        {
            IList list = (IList)o;
            if (list != null)
            {
                writer.WriteStart();
                writer.WriteEntry("Length", list.Count);
                writer.WriteEntry("Type", list.GetType().GetGenericArguments()[0].Name);

                for (int i = 0; i < list.Count; i++)
                {
                    writer.WriteEntry(i.ToString(), list[i]);
                }
                writer.WriteEnd();
            }
        }
    }

    class SerializeConverterDictionary : SerializeConverter
    {
        override public void OnSerialize(SerializeWriter writer, object o)
        {
            IDictionary dict = (IDictionary)o;
            if (dict != null)
            {
                writer.WriteStart();
                writer.WriteEntry("Length", dict.Count);
                writer.WriteEntry("Key_Type", dict.GetType().GetGenericArguments()[0].Name);
                writer.WriteEntry("Value_Type", dict.GetType().GetGenericArguments()[1].Name);

                IEnumerator keys = dict.Keys.GetEnumerator();
                IEnumerator values = dict.Values.GetEnumerator();

                for (int i = 0; i < dict.Count; i++)
                {
                    keys.MoveNext();
                    values.MoveNext();

                    writer.WriteEntry("Key_" + i.ToString(), keys.Current);
                    writer.WriteEntry("Value_" + i.ToString(), values.Current);
                }
                writer.WriteEnd();
            }
        }
    }
}