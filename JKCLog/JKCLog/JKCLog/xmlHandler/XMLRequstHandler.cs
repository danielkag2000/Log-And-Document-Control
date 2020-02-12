using LogSystem.xmlHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LogSystem
{
    /// <summary>
    /// handle xml request files
    /// </summary>
    class XMLRequstHandler : IHandler
    {
        //XmlDocument doc;
        RequestXML request;
        DBClient dbClient;

        public XMLRequstHandler(string str)
        {
            //doc = new XmlDocument();
            //doc.LoadXml(str);
            this.dbClient = new DBClient();
            request = RequestXML.LoadFromXMLString(str);
        }

        //private Class objectType;
        public string Handle()
        {
            List<string> parameters = new List<string>();
            parameters.Add($"{request.ProgramID}");
            parameters.Add($"{request.SubProgramID}");
            parameters.AddRange(request.Func.Params);
            string status = Globals.functions[request.Func.id](parameters, dbClient);
            return status;
        }

    }
}
