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
        public static void SaveToXml<T>(T t, string path)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
                Encoding = Encoding.Unicode,
            };

            using (StreamWriter sw = new StreamWriter(path))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(sw, xmlSettings))
                {
                    XmlSerializer s = new XmlSerializer(typeof(T));
                    s.Serialize(xmlWriter, t);
                }
            }
        }

        public static T LoadFromXml<T>(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.Unicode))
            {
                XmlSerializer s = new XmlSerializer(typeof(T));
                T t = (T)s.Deserialize(sr);
                return t;
            }
        }
    }
}
