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
            if(nodeFactura == null || !int.TryParse(nodeFactura.InnerText, out f.CantReg) )
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
            List<Wsfe.FECAEDetRequest> fecaeDetRequests = new List<Wsfe.FECAEDetRequest>();

            fecaeDetRequest.DocNro = f.DocNro;
            fecaeDetRequest.DocTipo = f.DocTipo;
            fecaeDetRequest.ImpTotal = f.ImpTotal;
            fecaeDetRequest.CbteDesde = f.CbteDesde;
            fecaeDetRequest.CbteHasta = f.CbteHasta;
            fecaeDetRequest.Concepto = f.Concepto;
            fecaeDetRequest.ImpTotal = f.ImpTotal;
            fecaeDetRequest.ImpNeto = f.ImpNeto;
            fecaeDetRequest.ImpIVA = f.ImpIVA;
            
            fecaeDetRequest.MonId = f.MonId;
            fecaeDetRequest.MonCotiz = f.MonCotiz;

            Wsfe.Tributo tributo = new Wsfe.Tributo();
            tributo.Id = f.tributoIva_id;
            tributo.BaseImp = f.tributoIva_BaseImp;
            tributo.Importe = f.tributoIva_Importe;

            List<Wsfe.Tributo> tributos = new List<Wsfe.Tributo>();
            tributos.Add(tributo);
            fecaeDetRequest.Tributos = tributos.ToArray();

            fecaeDetRequests.Add(fecaeDetRequest);
            
            fecaeReq.FeCabReq = fecabRequest;
            fecaeReq.FeDetReq = fecaeDetRequests.ToArray();

            Wsfe.FECAEResponse feResponse;
            feResponse = wsfeService.FECAESolicitar(feAuthRequest, fecaeReq);
            bool errorsFound = false;
            foreach( Wsfe.Err error in feResponse.Errors )
            {
                Console.WriteLine("DEBUGLEOH en bucle de error");
                Console.WriteLine("Response error: {0} [{1}]", error.Msg, error.Code);
                errorsFound = true;
            }

            if (errorsFound)
            {
                throw new Exception("Error al llamar al ws de factura electronica");
            }
            

            Console.WriteLine("DEBUGLEOH despues del catch");
            Console.WriteLine("Response cabResultado: {0}", feResponse.FeCabResp.Resultado);
            Console.WriteLine("DEBUGLEOH despues del cabResultado");
            foreach ( Wsfe.FECAEDetResponse detResponse in feResponse.FeDetResp )
            {
                Console.WriteLine("Response detResultado Concepto={0} CAE={1} [Resultado={3}]", detResponse.Concepto, detResponse.CAE, detResponse.Resultado);            
            }
            Console.WriteLine("DEBUGLEOH antes del fin");
            return "";
        }
    }
}
