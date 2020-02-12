using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LogSystem.xmlHandler
{
    [XmlRoot("Response")]
    public class ResponseXML
    {
        public ResponseXML()
        {
            Status = 0;
        }

        public ResponseXML(int status, int pID, int sID)
        {
            Status = status;
            ProgramID = pID;
            SubProgramID = sID;
            Value = new List<string>();
        }

        public ResponseXML(int status, int pID, int sID, string value)
        {
            Status = status;
            ProgramID = pID;
            SubProgramID = sID;
            Value = new List<string>();
            Value.Add(value);
        }

        public ResponseXML(int status, int pID, int sID, List<string> values)
        {
            Status = status;
            ProgramID = pID;
            SubProgramID = sID;
            Value = values;
        }

        [XmlElement("Status")]
        public int Status { get; set; }

        [XmlElement("ProgramID")]
        public int ProgramID { get; set; }

        [XmlElement("SubProgramID")]
        public int SubProgramID { get; set; }

        [XmlElement("Value")]
        public List<string> Value { get; set; }

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
