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
    public InputReader(WsfeRequestType paramFeReq)
    {
        feReq = paramFeReq;
    }

    public WsfeRequestType leeInputEnXml(string archivo)
    {
        System.Xml.Serialization.XmlSerializer r1 = new System.Xml.Serialization.XmlSerializer(feReq.GetType());
        using(var reader1= System.IO.File.OpenText(archivo))
        {
            feReq = (WsfeRequestType)r1.Deserialize(reader1);
        }
        Console.WriteLine("DEBUGLEOH: ARCHIVO={0} DATOS={1}", archivo, feReq);
        return feReq;

        System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(feReq.GetType());

        // Read the XML file.
        System.IO.StreamReader file =
           new System.IO.StreamReader(archivo);

        Console.WriteLine("DEBUGLEOH: ARCHIVO={0} DATOS={1} TYPE={2}", archivo, reader.Deserialize(file), feReq.GetType()); file =new System.IO.StreamReader(archivo);
        // Deserialize the content of the file into a Book object.
        feReq = (WsfeRequestType)reader.Deserialize(file);
        
        return feReq;
    }
}
