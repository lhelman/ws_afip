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

    public bool send(long cuit, int cbteTipo, long cbteNro, int ptoVenta, string archivoSalida)
    {
        WsAfipCommon.Wsfe.FECompConsultaReq feCompConsultaReq = new WsAfipCommon.Wsfe.FECompConsultaReq();
        feCompConsultaReq.CbteTipo = cbteTipo;
        feCompConsultaReq.CbteNro = cbteNro;
        feCompConsultaReq.PtoVta = ptoVenta;

        WsAfipCommon.Wsfe.FEAuthRequest feAuthRequest = new WsAfipCommon.Wsfe.FEAuthRequest();
        feAuthRequest.Cuit = cuit;
        feAuthRequest.Token = _wsaaToken;
        feAuthRequest.Sign = _wsaaSign;

        WsAfipCommon.Wsfe.Service wsfeService = new WsAfipCommon.Wsfe.Service();
        wsfeService.Url = _url;

        WsAfipCommon.Wsfe.FECompConsultaResponse feResponse;
        feResponse = wsfeService.FECompConsultar(feAuthRequest, feCompConsultaReq);

        OutputWriter<WsAfipCommon.Wsfe.FECompConsultaReq, WsAfipCommon.Wsfe.FECompConsultaResponse> output = new OutputWriter<WsAfipCommon.Wsfe.FECompConsultaReq,WsAfipCommon.Wsfe.FECompConsultaResponse>();
        output.verbose = _verboseMode;

        output.escribirRespuestaFacturaXml(archivoSalida, feResponse);
        return true;
    }
}
