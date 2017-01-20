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


    public int pruebas(long cuit, string archivoFactura, string archivoSalida)
    {
        var x = new WsAfipCommon.SRWsfe.FECAERequest();
        x.FeCabReq = new WsAfipCommon.SRWsfe.FECAECabRequest();
        x.FeCabReq.CantReg = 1;
        x.FeCabReq.CbteTipo = 1;
        x.FeCabReq.PtoVta = 4;
        //x.FeDetReq
        var y = new WsAfipCommon.SRWsfe.FECAEDetRequest();
        y.Concepto = 1;
        y.DocTipo = 80;
        y.DocNro = 27122195118;
        y.CbteDesde = 581;
        y.CbteHasta = 581;
        y.CbteFch = "20161014";
        y.ImpTotal = 17700.40;
        y.ImpTotConc = 0;
        y.ImpNeto = 15456.57;
        y.ImpOpEx = 0;
        y.ImpTrib = 0;
        y.ImpIVA = 3245.88;
        y.MonId = "PES";
        y.MonCotiz = 1;
        var z = new WsAfipCommon.SRWsfe.AlicIva();

        z.Id = 5;
        z.BaseImp = 15456.57;
        z.Importe = 3245.88;

        y.Iva = new WsAfipCommon.SRWsfe.AlicIva[] { z };

        x.FeDetReq = new WsAfipCommon.SRWsfe.FECAEDetRequest[] { y };

        var output2 = new OutputWriter<WsAfipCommon.SRWsfe.FECAERequest>();
        output2.verbose = _verboseMode;

        output2.escribeEnXml("lalala.xml", x);
        if (_verboseMode) { Console.WriteLine("***despues OutputWriter2**"); }

        return 0;
    }

    public int send(long cuit, string archivoFactura, string archivoSalida)
    {

        InputReader< WsAfipCommon.SRWsfe.FECAERequest> reader = new InputReader<WsAfipCommon.SRWsfe.FECAERequest>(new WsAfipCommon.SRWsfe.FECAERequest());
        WsAfipCommon.SRWsfe.FECAERequest fecaeReq = reader.leeInputEnXml(archivoFactura);

        var output = new OutputWriter<WsAfipCommon.SRWsfe.FECAEResponse>();
        output.verbose = _verboseMode;

        Console.WriteLine("fecaeReq {0}", fecaeReq.FeCabReq.CbteTipo);
        try
        {

            if (_verboseMode) { Console.WriteLine("***ADENTRO TRY**"); }

            System.ServiceModel.EndpointAddress remoteAddress = new System.ServiceModel.EndpointAddress(new Uri(_url));
            if (_verboseMode) { Console.WriteLine("***respues EndpointAddress**"); Console.WriteLine(remoteAddress); }
            WsAfipCommon.SRWsfe.ServiceSoapClient wsfeService = new WsAfipCommon.SRWsfe.ServiceSoapClient(new System.ServiceModel.BasicHttpsBinding(), remoteAddress);
            if (_verboseMode) { Console.WriteLine("***despues ServiceSoapClient**"); }

            WsAfipCommon.SRWsfe.FEAuthRequest feAuthRequest = new WsAfipCommon.SRWsfe.FEAuthRequest();
            feAuthRequest.Cuit = cuit;
            feAuthRequest.Token = _wsaaToken;
            feAuthRequest.Sign = _wsaaSign;

            if (feAuthRequest == null) { Console.WriteLine("feAuthRequest es nulo"); }
            if (fecaeReq == null) { Console.WriteLine("fecaeReq es nulo"); }
            WsAfipCommon.SRWsfe.FECAEResponse feResponse;
            if (_verboseMode) { Console.WriteLine("***FECAESolicitar: feAuthRequest:**"); Console.WriteLine(feAuthRequest); Console.WriteLine("***FECAESolicitar: fecaeReq:**"); Console.WriteLine(fecaeReq); }
            feResponse = wsfeService.FECAESolicitar(feAuthRequest, fecaeReq);
            if (_verboseMode) { Console.WriteLine("***despues FECAESolicitar**"); Console.WriteLine(feResponse); }

            if (_verboseMode) { Console.WriteLine("***despues OutputWriter**"); }

            output.escribeEnXml(archivoSalida, feResponse);
            if (_verboseMode) { Console.WriteLine("***despues escribirRespuestaFacturaXML**"); }
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
