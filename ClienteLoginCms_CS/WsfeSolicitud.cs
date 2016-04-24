using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClienteLoginCms_CS
{
    class WsfeSolicitud
    {
        private String _wsaaToken;
        private String _wsaaSign;

        private bool _verboseMode;
        public bool verbose { set { _verboseMode = value; } }

        public WsfeSolicitud(String wsaaToken, String wsaaSign)
        {
            this._wsaaToken = wsaaToken;
            this._wsaaSign = wsaaSign;
        }

        private Factura readFacturaXml(string archivoFactura)
        {
            Factura f = new Factura();

            XmlDocument doc = new XmlDocument();
            doc.Load(archivoFactura);
            XmlNode nodeFactura;

            nodeFactura = doc.SelectSingleNode("/factura/CantReg");
            if( !int.TryParse(nodeFactura.InnerText, out f.CantReg) )
            {
                throw new System.ArgumentException("Hay que especificar 'CantReg' en la factura", "original");
            }
            
            nodeFactura = doc.SelectSingleNode("/factura/CbteTipo");
            if (!int.TryParse(nodeFactura.InnerText, out f.CbteTipo))
            {
                throw new System.ArgumentException("Hay que especificar 'CbteTipo' en la factura", "original");
            }
            
            nodeFactura = doc.SelectSingleNode("/factura/PtoVta");
            if (!int.TryParse(nodeFactura.InnerText, out f.PtoVta))
            {
                throw new System.ArgumentException("Hay que especificar 'PtoVta' en la factura", "original");
            }

            if (_verboseMode)
            {
                Console.WriteLine("Factura {0}", f);
            }
            
            return f;
        }

        public String send(long cuit, String xmlInputFile)
        {

            Factura f = readFacturaXml(xmlInputFile);

            // Send FEAuthRequest
            //("asdf", objTicketRespuesta.Token, objTicketRespuesta.Sign);
            Wsfe.FEAuthRequest feAuthRequest = new Wsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            Wsfe.Service wsfeService = new Wsfe.Service();

            //            feResponse = wsfeService.FECAEAConsultar(feAuthRequest, Periodo, orden);
            Wsfe.FECAERequest fecaeReq = new Wsfe.FECAERequest();

            Wsfe.FECAECabRequest fecabRequest = new Wsfe.FECAECabRequest();
            fecabRequest.CantReg = f.CantReg;
            fecabRequest.CbteTipo = f.CbteTipo;
            fecabRequest.PtoVta = f.PtoVta;
            
            Wsfe.FEDetRequest fedetReq = new Wsfe.FEDetRequest();
            Wsfe.FECAEDetRequest fecaeDetRequest = new Wsfe.FECAEDetRequest();
            Wsfe.FECAEDetRequest[] fecaeDetRequests = new Wsfe.FECAEDetRequest[1];
            // TODO: fecaeDetRequest.DocNro = 1;

            fecaeDetRequests[0] = fecaeDetRequest;
            
            fecaeReq.FeCabReq = fecabRequest;
            fecaeReq.FeDetReq = fecaeDetRequests;

            Wsfe.FECAEResponse feResponse;
            feResponse = wsfeService.FECAESolicitar(feAuthRequest, fecaeReq);

            return "";
        }
    }
}
