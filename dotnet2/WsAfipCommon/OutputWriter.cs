using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public class OutputWriter<WsfeType>
{
    private bool _verboseMode;
    public bool verbose { set { _verboseMode = value; } }


    public void escribeEnXmlGeneric<t>(string archivoOutput, t some_object)
    {
        Console.WriteLine("DEBUGLEOH ESCRIBE EN XML");

        XmlSerializer serializer = new XmlSerializer(typeof(t));
        using (TextWriter writer = new StreamWriter(archivoOutput))
        {
            Console.WriteLine("DEBUGLEOH SERIALIZE.serialize");
            serializer.Serialize(writer, some_object);
        }
    }

    public void escribeEnXml(string archivoOutput, WsfeType req)
    {
        escribeEnXmlGeneric(archivoOutput, req);
    }

    /*
     * <?xml version="1.0" encoding="utf-8"?>
<OutputParaExcepcion xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
<mensaje>El mensaje de error</mensaje>
<source>AfipWebservice</source>
</OutputParaExcepcion>
     */
    public void escribeEnXml(string archivoOutput, Exception exepcion)
    {
        OutputParaExepcion e = new OutputParaExepcion();
        e.mensaje = exepcion.Message;
        e.source = exepcion.Source;

        escribeEnXmlGeneric(archivoOutput, e);
    }

    private class OutputParaExepcion
    {
        public string mensaje;
        public string source;
    }
}
