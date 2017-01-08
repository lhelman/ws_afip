using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using NamespaceDefault;

namespace AfipWebservice
{
    public class InputReader
    {
        private bool _verboseMode;
        public bool verbose { set { _verboseMode = value; } }

        public NamespaceDefault.Wsfe.FECAERequest leeInputEnXml(string archivo)
        {
            NamespaceDefault.Wsfe.FECAERequest fecaeReq = new NamespaceDefault.Wsfe.FECAERequest();

            System.Xml.Serialization.XmlSerializer reader = new
   System.Xml.Serialization.XmlSerializer(fecaeReq.GetType());

            // Read the XML file.
            System.IO.StreamReader file =
               new System.IO.StreamReader(archivo);

            // Deserialize the content of the file into a Book object.
            fecaeReq = (NamespaceDefault.Wsfe.FECAERequest)reader.Deserialize(file);

            return fecaeReq;
        }
    }
}
