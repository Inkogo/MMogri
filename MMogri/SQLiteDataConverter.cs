using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMogri.Utils
{
    public abstract class SQLiteDataConverter
    {
        public abstract object ConvertRead(object o);

        public abstract object CovertWrite(object o);
    }


    public class GuidToStringConverter : SQLiteDataConverter
    {
        override public object ConvertRead(object o)
        {
            return new System.Guid((string)o);
        }

        override public object CovertWrite(object o)
        {
            if (o == null)
                o = System.Guid.NewGuid();
            return ((System.Guid)o).ToString();
        }
    }

    public class EnumToStringConverter<T> : SQLiteDataConverter
    {
        public override object ConvertRead(object o)
        {
            return System.Enum.Parse(typeof(T), (string)o);
        }

        public override object CovertWrite(object o)
        {
            return o.ToString();
        }
    }

    public class DateTimeToStringConverter : SQLiteDataConverter
    {
        public override object ConvertRead(object o)
        {
            return DateTime.Parse((string)o);
        }

        public override object CovertWrite(object o)
        {
            return ((DateTime)o).ToString();
        }
    }
}
