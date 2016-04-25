using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ClienteLoginCms_CS
{

    public class OutputWriter
    {
        public class OutputParaExcepcion
        {
            public string mensaje;
            public string source;
        }

        private bool _verboseMode;
        public bool verbose { set { _verboseMode = value; } }

        public void escribirRespuestaFacturaXml(string archivoOutput, Wsfe.FECAEResponse response)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Wsfe.FECAEResponse));
            using (TextWriter writer = new StreamWriter(archivoOutput))
            {
                serializer.Serialize(writer, response);
            }
        }

        /*
         * <?xml version="1.0" encoding="utf-8"?>
<OutputParaExcepcion xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <mensaje>El mensaje de error</mensaje>
  <source>ClienteLoginCms_CS</source>
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
}
