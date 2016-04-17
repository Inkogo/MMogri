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

        abstract public void OnDeserialize(SerializeReader reader);
    }

    class SerializeConverterValueType : SerializeConverter
    {
        override public void OnSerialize(SerializeWriter writer, object o)
        {
            writer.Write(o);
        }

        override public void OnDeserialize(SerializeReader reader)
        {
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
                for (int i = 0; i < list.Count; i++)
                {
                    writer.WriteEntry(i.ToString(), list[i]);
                }
                writer.WriteEnd();
            }
        }

        override public void OnDeserialize(SerializeReader reader)
        {      
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

        override public void OnDeserialize(SerializeReader reader)
        {
        }
    }

}