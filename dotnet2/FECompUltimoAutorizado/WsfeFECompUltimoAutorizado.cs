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
        var output = new OutputWriter<WsAfipCommon.SRWsfe.FERecuperaLastCbteResponse>();
        output.verbose = _verboseMode;

        try
        {
            var remoteAddress = new System.ServiceModel.EndpointAddress(new Uri(_url));
            var wsfeService = new WsAfipCommon.SRWsfe.ServiceSoapClient(new System.ServiceModel.BasicHttpsBinding(), remoteAddress);

            var feAuthRequest = new WsAfipCommon.SRWsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            WsAfipCommon.SRWsfe.FERecuperaLastCbteResponse feResponse;
            feResponse = wsfeService.FECompUltimoAutorizado(feAuthRequest, ptoVenta, cbteTipo);

            output.escribeEnXml(archivoSalida, feResponse);
        }
        catch (Exception excepcionAlMandarFE)
        {

            Console.WriteLine("***EXCEPCION AL MANDAR EL PEDIDO DE FE:");
            Console.WriteLine(excepcionAlMandarFE.Message);
            Console.WriteLine(excepcionAlMandarFE.Source);


            output.escribeEnXml(archivoSalida, excepcionAlMandarFE);

            return -11;

        }

        return 0;
    }
}
