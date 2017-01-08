using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public class OutputWriter<WsfeRequestType, WsfeResponseType>
{
    public class OutputParaExcepcion
    {
        public string mensaje;
        public string source;
    }

    private bool _verboseMode;
    public bool verbose { set { _verboseMode = value; } }


    public void serializeRequest(string archivoOutput, WsfeRequestType req)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(WsfeRequestType));
        using (TextWriter writer = new StreamWriter(archivoOutput))
        {
            serializer.Serialize(writer, req);
        }
    }

    public void escribirRespuestaFacturaXml(string archivoOutput, WsfeResponseType response)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(WsfeResponseType));
        using (TextWriter writer = new StreamWriter(archivoOutput))
        {
            serializer.Serialize(writer, response);
        }
    }

    /*
     * <?xml version="1.0" encoding="utf-8"?>
<OutputParaExcepcion xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
<mensaje>El mensaje de error</mensaje>
<source>AfipWebservice</source>
</OutputParaExcepcion>
     */
    public void escribirRespuestaFacturaXml(string archivoOutput, Exception exepcion)
    {
        OutputParaExcepcion e = new OutputParaExcepcion();
        e.mensaje = exepcion.Message;
        e.source = exepcion.Source;

        XmlSerializer serializer = new XmlSerializer(typeof(OutputParaExcepcion));
        using (TextWriter writer = new StreamWriter(archivoOutput))
        {
            serializer.Serialize(writer, e);
        }
    }
}
