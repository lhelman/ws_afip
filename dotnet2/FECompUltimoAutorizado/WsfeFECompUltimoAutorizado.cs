using System;

class WsfeFECompUltimoAutorizado
{
    private string _wsaaToken;
    private string _wsaaSign;
    private string _url;

    private bool _verboseMode;
    public bool verbose { set { _verboseMode = value; } }

    public WsfeFECompUltimoAutorizado(string url, string wsaaToken, string wsaaSign)
    {
        this._wsaaToken = wsaaToken;
        this._wsaaSign = wsaaSign;
        this._url = url;
    }

    public int send(long cuit, int cbteTipo, int ptoVenta, string archivoSalida)
    {
        try
        {
            WsAfipCommon.SRWsfe.FECompUltimoAutorizadoRequest feCompUltimoAutorizadoReq = new WsAfipCommon.SRWsfe.FECompUltimoAutorizadoRequest();

            feCompUltimoAutorizadoReq.Body.CbteTipo = cbteTipo;
            feCompUltimoAutorizadoReq.Body.PtoVta = ptoVenta;
            System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(new Uri(_url));
            WsAfipCommon.SRWsfe.ServiceSoapClient wsfeService = new WsAfipCommon.SRWsfe.ServiceSoapClient(new System.ServiceModel.BasicHttpsBinding(), remoteAddress);

            WsAfipCommon.SRWsfe.FEAuthRequest feAuthRequest = new WsAfipCommon.SRWsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            WsAfipCommon.SRWsfe.FERecuperaLastCbteResponse feResponse;
            feResponse = wsfeService.FECompUltimoAutorizado(feAuthRequest, ptoVenta, cbteTipo);
            OutputWriter<WsAfipCommon.SRWsfe.FECompUltimoAutorizadoRequest, WsAfipCommon.SRWsfe.FERecuperaLastCbteResponse> output = new OutputWriter<WsAfipCommon.SRWsfe.FECompUltimoAutorizadoRequest, WsAfipCommon.SRWsfe.FERecuperaLastCbteResponse>();
            output.verbose = _verboseMode;

            output.escribirRespuestaFacturaXml(archivoSalida, feResponse);
        }
        catch (Exception excepcionAlMandarFE)
        {

            Console.WriteLine("***EXCEPCION AL MANDAR EL PEDIDO DE FE:");
            Console.WriteLine(excepcionAlMandarFE.Message);
            Console.WriteLine(excepcionAlMandarFE.Source);

            OutputWriter<WsAfipCommon.SRWsfe.FECompUltimoAutorizadoRequest, WsAfipCommon.SRWsfe.FERecuperaLastCbteResponse> output = new OutputWriter<WsAfipCommon.SRWsfe.FECompUltimoAutorizadoRequest, WsAfipCommon.SRWsfe.FERecuperaLastCbteResponse>();
            output.verbose = _verboseMode;

            output.escribirRespuestaFacturaXml(archivoSalida, excepcionAlMandarFE);

            return -11;

        }

        return 0;
    }
}
