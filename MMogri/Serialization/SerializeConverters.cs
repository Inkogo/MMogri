using System.Collections;

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

                for (int i = 0; i < list.Count; i++)
                {
                    writer.WriteEntry("Value_" + i.ToString(), list[i]);
                }
                writer.WriteEnd();
            }
        }
    }

    class SerializeConverterArray : SerializeConverter
    {
        override public void OnSerialize(SerializeWriter writer, object o)
        {
            System.Array arr = (System.Array)o;
            if (arr != null)
            {
                writer.WriteStart();
                writer.WriteEntry("Length", arr.Length);

                for (int i = 0; i < arr.Length; i++)
                {
                    writer.WriteEntry("Value_" + i.ToString(), arr.GetValue(i));
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