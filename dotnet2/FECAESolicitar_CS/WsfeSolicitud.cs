using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AfipWebservice
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

        public bool send(long cuit, string archivoFactura, string archivoSalida)
        {
            InputReader reader = new InputReader();
            NamespaceDefault.Wsfe.FECAERequest fecaeReq = reader.leeInputEnXml(archivoFactura);

            NamespaceDefault.Wsfe.FEAuthRequest feAuthRequest = new NamespaceDefault.Wsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            NamespaceDefault.Wsfe.Service wsfeService = new NamespaceDefault.Wsfe.Service();
            wsfeService.Url = _url;

            NamespaceDefault.Wsfe.FECAEResponse feResponse;
            feResponse = wsfeService.FECAESolicitar(feAuthRequest, fecaeReq);

            OutputWriter output = new OutputWriter();
            output.verbose = _verboseMode;
            //output.serializeRequest("testing_factura_request.xml", fecaeReq);

            output.escribirRespuestaFacturaXml(archivoSalida, feResponse);
            return true;
        }
    }
}
