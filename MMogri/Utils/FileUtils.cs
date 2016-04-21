using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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

        public static void SaveToMog<T>(T t, string path) where T : new()
        {
            Serialization.SerializeWriter writer = new Serialization.SerializeWriter();
            writer.Serialize<T>(path, t);
        }

        public static T LoadFromMog<T>(string path) where T : new()
        {
            Serialization.SerializeReader reader = new Serialization.SerializeReader();
            return (T)reader.Deserialize<T>(path);
        }
    }
}
