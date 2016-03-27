using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace MMogri.Utils
{
    class FileUtils
    {
        static readonly Encoding encoding = Encoding.Unicode;

        public static void SaveToXml<T>(T t, string path)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = encoding,
            };

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs, encoding))
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(sw, xmlSettings))
                    {
                        XmlSerializer s = new XmlSerializer(typeof(T));
                        s.Serialize(xmlWriter, t);
                    }
                }
            }
        }

        public static T LoadFromXml<T>(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader stream = new StreamReader(fs, encoding))
                {
                    XmlSerializer s = new XmlSerializer(typeof(T));
                    return (T)s.Deserialize(new XmlTextReader(stream));
                }
            }
        }
    }
}
