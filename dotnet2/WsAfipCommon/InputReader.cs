using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

public class InputReader<WsfeRequestType>
{
    private bool _verboseMode;
    public bool verbose { set { _verboseMode = value; } }
    WsfeRequestType feReq;
    public InputReader(WsfeRequestType feReq)
    {
        feReq = this.feReq;
    }

    public WsfeRequestType leeInputEnXml(string archivo)
    {
        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(feReq.GetType());

        // Read the XML file.
        System.IO.StreamReader file =
           new System.IO.StreamReader(archivo);

        // Deserialize the content of the file into a Book object.
        feReq = (WsfeRequestType)reader.Deserialize(file);
        
        return feReq;
    }
}
