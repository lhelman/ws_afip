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

        public Factura leeFacturaEnXml(string archivoFactura)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(archivoFactura);
            return leeFacturaDeXmlDocument(doc);
        }

        public Factura leeFacturaEnXml(StringReader stringXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stringXml);
            return leeFacturaDeXmlDocument(doc);
        }

        private Factura leeFacturaDeXmlDocument(XmlDocument doc)
        {
            Factura f = new Factura();

            XmlNode nodeFactura;

            nodeFactura = doc.SelectSingleNode("/factura/CantReg");
            if (nodeFactura == null || !int.TryParse(nodeFactura.InnerText, out f.CantReg))
            {
                throw new System.ArgumentException("Hay que especificar 'CantReg' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/CbteTipo");
            if (nodeFactura == null || !int.TryParse(nodeFactura.InnerText, out f.CbteTipo))
            {
                throw new System.ArgumentException("Hay que especificar 'CbteTipo' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/PtoVta");
            if (nodeFactura == null || !int.TryParse(nodeFactura.InnerText, out f.PtoVta))
            {
                throw new System.ArgumentException("Hay que especificar 'PtoVta' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/DocNro");
            if (nodeFactura == null || !long.TryParse(nodeFactura.InnerText, out f.DocNro))
            {
                throw new System.ArgumentException("Hay que especificar 'DocNro' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/DocTipo");
            if (nodeFactura == null || !int.TryParse(nodeFactura.InnerText, out f.DocTipo))
            {
                throw new System.ArgumentException("Hay que especificar 'DocTipo' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/CbteDesde");
            if (nodeFactura == null || !long.TryParse(nodeFactura.InnerText, out f.CbteDesde))
            {
                throw new System.ArgumentException("Hay que especificar 'CbteDesde' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/CbteHasta");
            if (nodeFactura == null || !long.TryParse(nodeFactura.InnerText, out f.CbteHasta))
            {
                throw new System.ArgumentException("Hay que especificar 'CbteHasta' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/Concepto");
            if (nodeFactura == null || !int.TryParse(nodeFactura.InnerText, out f.Concepto))
            {
                throw new System.ArgumentException("Hay que especificar 'Concepto' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/ImpTotal");
            if (nodeFactura == null || !double.TryParse(nodeFactura.InnerText, out f.ImpTotal))
            {
                throw new System.ArgumentException("Hay que especificar 'ImpTotal' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/ImpNeto");
            if (nodeFactura == null || !double.TryParse(nodeFactura.InnerText, out f.ImpNeto))
            {
                throw new System.ArgumentException("Hay que especificar 'ImpNeto' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/ImpIVA");
            if (nodeFactura == null || !double.TryParse(nodeFactura.InnerText, out f.ImpIVA))
            {
                throw new System.ArgumentException("Hay que especificar 'ImpIVA' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/MonId");
            if (nodeFactura == null)
            {
                throw new System.ArgumentException("Hay que especificar 'MonId' en la factura", "original");
            }
            f.MonId = nodeFactura.InnerText;

            nodeFactura = doc.SelectSingleNode("/factura/MonCotiz");
            if (nodeFactura == null || !double.TryParse(nodeFactura.InnerText, out f.MonCotiz))
            {
                throw new System.ArgumentException("Hay que especificar 'MonCotiz' en la factura", "original");
            }

            f.tributoIva_id = 5;

            nodeFactura = doc.SelectSingleNode("/factura/tributoIva_BaseImp");
            if (nodeFactura == null || !double.TryParse(nodeFactura.InnerText, out f.tributoIva_BaseImp))
            {
                throw new System.ArgumentException("Hay que especificar 'tributoIva_BaseImp' en la factura", "original");
            }

            nodeFactura = doc.SelectSingleNode("/factura/tributoIva_Importe");
            if (nodeFactura == null || !double.TryParse(nodeFactura.InnerText, out f.tributoIva_Importe))
            {
                throw new System.ArgumentException("Hay que especificar 'tributoIva_Importe' en la factura", "original");
            }



            if (_verboseMode)
            {
                Console.WriteLine("Factura Input {0}", f);
            }

            return f;
        }


    }
}
