using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace PatientPortal.Infrastructure.Utility
{
    public class Serializer
    {
        public T Deserialize<T>(string input, string rootElementName) where T : class
        {
            var stringReader = new System.IO.StringReader(input);
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = rootElementName;
            xRoot.IsNullable = true;
            XmlSerializer xs = new XmlSerializer(typeof(T), xRoot);
            return xs.Deserialize(stringReader) as T;
        }

        public string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }

        public string SerializeToXML<T>(T ObjectToSerialize)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, ObjectToSerialize);
                    xml = sww.ToString();
                }
            }
            return xml;
        }
    }
}