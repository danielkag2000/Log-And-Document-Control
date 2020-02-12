using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogSystem.xmlHandler
{
    [XmlRoot("Request")]
    public class RequestXML
    {
        [XmlElement("ProgramID")]
        public string ProgramID { get; set; }

        [XmlElement("SubProgramID")]
        public string SubProgramID { get; set; }

        [XmlElement("Function")]
        public Function Func { get; set; }

        /// <summary>
        /// convert the xml to RequestXML object
        /// </summary>
        /// <param name="xmlText">the xml</param>
        /// <returns>the object which the xml presents</returns>
        public static RequestXML LoadFromXMLString(string xmlText)
        {
            using (var stringReader = new System.IO.StringReader(xmlText))
            {
                var serializer = new XmlSerializer(typeof(RequestXML));
                return serializer.Deserialize(stringReader) as RequestXML;
            }
        }
    }

    public class Function
    {
        [XmlElement("FunctionID")]
        public int id { get; set; }

        [XmlElement("Param")]
        public List<string> Params { get; set; }
    }
}
