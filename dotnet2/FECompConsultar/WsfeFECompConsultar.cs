using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

class WsfeFECompConsultar
{
    private string _wsaaToken;
    private string _wsaaSign;
    private string _url;

    private bool _verboseMode;
    public bool verbose { set { _verboseMode = value; } }

    public WsfeFECompConsultar(string url, string wsaaToken, string wsaaSign)
    {
        this._wsaaToken = wsaaToken;
        this._wsaaSign = wsaaSign;
        this._url = url;
    }

    public int send(long cuit, int cbteTipo, long cbteNro, int ptoVenta, string archivoSalida)
    {
        try
        {
            WsAfipCommon.SRWsfe.FECompConsultaReq feCompConsultaReq = new WsAfipCommon.SRWsfe.FECompConsultaReq();

            feCompConsultaReq.CbteTipo = cbteTipo;
            feCompConsultaReq.CbteNro = cbteNro;
            feCompConsultaReq.PtoVta = ptoVenta;
            System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(new Uri(_url));
            WsAfipCommon.SRWsfe.ServiceSoapClient wsfeService = new WsAfipCommon.SRWsfe.ServiceSoapClient(new System.ServiceModel.BasicHttpsBinding(), remoteAddress);

            WsAfipCommon.SRWsfe.FEAuthRequest feAuthRequest = new WsAfipCommon.SRWsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            WsAfipCommon.SRWsfe.FECompConsultaResponse feResponse;
            feResponse = wsfeService.FECompConsultar(feAuthRequest, feCompConsultaReq);

            OutputWriter<WsAfipCommon.SRWsfe.FECompConsultaReq, WsAfipCommon.SRWsfe.FECompConsultaResponse> output = new OutputWriter<WsAfipCommon.SRWsfe.FECompConsultaReq, WsAfipCommon.SRWsfe.FECompConsultaResponse>();
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
