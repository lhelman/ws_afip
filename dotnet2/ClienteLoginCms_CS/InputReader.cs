using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ClienteLoginCms_CS
{
    public class InputReader
    {
        private bool _verboseMode;
        public bool verbose { set { _verboseMode = value; } }

        public Wsfe.FECAERequest leeInputEnXml(string archivo)
        {
            Wsfe.FECAERequest fecaeReq = new Wsfe.FECAERequest();

            System.Xml.Serialization.XmlSerializer reader = new
   System.Xml.Serialization.XmlSerializer(fecaeReq.GetType());

            // Read the XML file.
            System.IO.StreamReader file =
               new System.IO.StreamReader(archivo);

            // Deserialize the content of the file into a Book object.
            fecaeReq = (Wsfe.FECAERequest)reader.Deserialize(file);

            return fecaeReq;
        }
    }
}
