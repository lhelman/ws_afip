using System;

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

    public int send(long cuit, string archivoFactura, string archivoSalida)
    {
        InputReader<WsAfipCommon.SRWsfe.FECAERequest> reader = new InputReader<WsAfipCommon.SRWsfe.FECAERequest>(new WsAfipCommon.SRWsfe.FECAERequest());
        WsAfipCommon.SRWsfe.FECAERequest fecaeReq = reader.leeInputEnXml(archivoFactura);

        try
        {
            System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(new Uri(_url));
            WsAfipCommon.SRWsfe.ServiceSoapClient wsfeService = new WsAfipCommon.SRWsfe.ServiceSoapClient(new System.ServiceModel.BasicHttpsBinding(), remoteAddress);

            WsAfipCommon.SRWsfe.FEAuthRequest feAuthRequest = new WsAfipCommon.SRWsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            WsAfipCommon.SRWsfe.FECAEResponse feResponse;
            feResponse = wsfeService.FECAESolicitar(feAuthRequest, fecaeReq);

            OutputWriter<WsAfipCommon.SRWsfe.FECAERequest, WsAfipCommon.SRWsfe.FECAEResponse> output = new OutputWriter<WsAfipCommon.SRWsfe.FECAERequest, WsAfipCommon.SRWsfe.FECAEResponse>();
            output.verbose = _verboseMode;

            output.escribirRespuestaFacturaXml(archivoSalida, feResponse);
        }
        catch (Exception excepcionAlMandarFE)
        {

            Console.WriteLine("***EXCEPCION AL MANDAR EL PEDIDO DE FE:");
            Console.WriteLine(excepcionAlMandarFE.Message);
            Console.WriteLine(excepcionAlMandarFE.Source);

            OutputWriter<WsAfipCommon.SRWsfe.FECompConsultaReq, WsAfipCommon.SRWsfe.FECompConsultaResponse> output = new OutputWriter<WsAfipCommon.SRWsfe.FECompConsultaReq, WsAfipCommon.SRWsfe.FECompConsultaResponse>();
            output.verbose = _verboseMode;

            output.escribirRespuestaFacturaXml(archivoSalida, excepcionAlMandarFE);

            return -11;

        }

        return 0;
    }
}
