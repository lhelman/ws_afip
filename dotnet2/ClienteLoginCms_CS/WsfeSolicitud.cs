using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClienteLoginCms_CS
{
    class WsfeSolicitud
    {
        private string _wsaaToken;
        private string _wsaaSign;
        private string _url;

        private bool _verboseMode;
        public bool verbose { set { _verboseMode = value; } }

        public WsfeSolicitud(string url, string wsaaToken, string wsaaSign)
        {
            this._wsaaToken = wsaaToken;
            this._wsaaSign = wsaaSign;
            this._url = url;
        }

        public string send(long cuit, Factura f)
        {
            // Send FEAuthRequest
            //("asdf", objTicketRespuesta.Token, objTicketRespuesta.Sign);
            Wsfe.FEAuthRequest feAuthRequest = new Wsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            Wsfe.Service wsfeService = new Wsfe.Service();
            wsfeService.Url = _url;

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
