using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LogSystem.xmlHandler
{
    [XmlRoot("Notification")]
    public class NotificationXML
    {
        public NotificationXML()
        {
            Status = 1;
            Count = 1;
            Process = -1;
            Pipe = -1;
        }

        public NotificationXML(int status, int count, int process, int pipe)
        {
            Status = status;
            Count = count;
            Process = process;
            Pipe = pipe;
        }

        [XmlElement("Status")]
        public int Status { get; set; }

        [XmlElement("Count")]
        public int Count { get; set; }

        [XmlElement("Process")]
        public int Process { get; set; }

        [XmlElement("Pipe")]
        public int Pipe { get; set; }

        /// <summary>
        /// convert the class to xml
        /// </summary>
        /// <returns>the xml response</returns>
        public string ToXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(stringwriter, this, ns);
                xmlDoc.LoadXml(stringwriter.ToString());

                //return stringwriter.ToString();
                return xmlDoc.InnerXml;
            }
        }
    }
}
