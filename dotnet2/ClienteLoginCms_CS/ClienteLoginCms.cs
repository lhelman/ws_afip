//<summary> 
//EJEMPLO - Consola cliente del webservice de autenticacion y autorizacion 
//</summary> 
//<description> 
// Consume el metodo LoginCms del WSAA 
// Muestra en stdout el login ticket response 
//</description> 
//<version>8.5.26.1200</version> 
//<usage> 
// clientelogincms (opciones) ... 
// opciones: 
// -s servicio ID del servicio de negocio 
// -w url URL del WSDL del WSAA 
// -c certificado Ruta del certificado (con clave privada) 
// -v on|off Salida detallada on/off 
// -? Muestra ayuda de uso 
//</usage> 
//<platform>.NET Framework 2.0</platform> 
//<disclaimer> 
// El Departamento de Arquitectura Informatica de la AFIP (DeArIn/AFIP), pone a disposicion 
// el siguiente codigo para su utilizacion con el WebService de Facturacion Electronica (WSFE) 
// de la AFIP. 
// 
// El mismo no puede ser re-distribuido, publicado o descargado en forma total o parcial, ya sea 
// en forma electronica, mecanica u optica, sin la autorizacion de DeArIn/AFIP. El uso no 
// autorizado del mismo esta prohibido. 
// 
// DeArIn/AFIP no asume ninguna responsabilidad de los errores que pueda contener el codigo ni la 
// obligacion de subsanar dichos errores o informar de la existencia de los mismos. 
// 
// DeArIn/AFIP no asume ninguna responsabilidad que surja de la utilizacion del codigo, ya sea por 
// utilizacion ilegal de patentes, perdida de beneficios, perdida de informacion o cualquier otro 
// inconveniente. 
// 
// Bajo ninguna circunstancia DeArIn/AFIP podra ser indicada como responsable por consecuencias y/o 
// incidentes ya sean directos o indirectos que puedan surgir de la utilizacion del codigo. 
// 
// DeSoTe/AFIP no da ninguna garantia, expresa o implicita, de la utilidad del codigo, si el mismo es 
// correcto, o si cumple con los requerimientos de algun proposito en particular. 
// 
// DeArIn/AFIP puede realizar cambios en cualquier momento en el codigo sin previo aviso. 
// 
// El codigo debera ser evaluado, verificado, corregido y/o adaptado por personal tecnico calificado 
// de las entidades que lo utilicen. 
// 
// EL SIGUIENTE CODIGO ES DISTRIBUIDO PARA EVALUACION, CON TODOS SUS ERRORES Y OMISIONES. LA 
// RESPONSABILIDAD DEL CORRECTO FUNCIONAMIENTO DEL MISMO YA SEA POR SI SOLO O COMO PARTE DE 
// OTRA APLICACION, QUEDA A CARGO DE LAS ENTIDADES QUE LO UTILICEN. LA UTILIZACION DEL CODIGO 
// SIGNIFICA LA ACEPTACION DE TODOS LOS TERMINOS Y CONDICIONES MENCIONADAS ANTERIORMENTE. 
// 
// DeArIn-AFIP 
//</disclaimer> 

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Microsoft.VisualBasic;
using ClienteLoginCms_CS;

/// <summary> 
/// Clase para crear objetos Login Tickets 
/// </summary> 
/// <remarks> 
/// Ver documentacion: 
/// Especificacion Tecnica del Webservice de Autenticacion y Autorizacion 
/// Version 1.0 
/// Departamento de Seguridad Informatica - AFIP 
/// </remarks> 
class LoginTicket
{
    // Entero de 32 bits sin signo que identifica el requerimiento 
    public UInt32 UniqueId;
    // Momento en que fue generado el requerimiento 
    public DateTime GenerationTime;
    // Momento en el que exoira la solicitud 
    public DateTime ExpirationTime;
    // Identificacion del WSN para el cual se solicita el TA 
    public string Service;
    // Firma de seguridad recibida en la respuesta 
    public string Sign;
    // Token de seguridad recibido en la respuesta 
    public string Token;

    public XmlDocument XmlLoginTicketRequest = null;
    public XmlDocument XmlLoginTicketResponse = null;
    public string RutaDelCertificadoFirmante;
    public string XmlStrLoginTicketRequestTemplate = "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";

    private bool _verboseMode = true;

    // OJO! NO ES THREAD-SAFE 
    private static UInt32 _globalUniqueID = 0;

    /// <summary> 
    /// Construye un Login Ticket obtenido del WSAA 
    /// </summary> 
    /// <param name="argServicio">Servicio al que se desea acceder</param> 
    /// <param name="argUrlWsaa">URL del WSAA</param> 
    /// <param name="argRutaCertX509Firmante">Ruta del certificado X509 (con clave privada) usado para firmar</param> 
    /// <param name="argVerbose">Nivel detallado de descripcion? true/false</param> 
    /// <remarks></remarks> 
    public string ObtenerLoginTicketResponse(string argServicio, string argUrlWsaa, string argRutaCertX509Firmante, bool argVerbose)
    {

        this.RutaDelCertificadoFirmante = argRutaCertX509Firmante;
        this._verboseMode = argVerbose;
        CertificadosX509Lib.VerboseMode = argVerbose;

        string cmsFirmadoBase64;
        string loginTicketResponse;

        XmlNode xmlNodoUniqueId;
        XmlNode xmlNodoGenerationTime;
        XmlNode xmlNodoExpirationTime;
        XmlNode xmlNodoService;

        // PASO 1: Genero el Login Ticket Request 
        try
        {
            XmlLoginTicketRequest = new XmlDocument();
            XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

            xmlNodoUniqueId = XmlLoginTicketRequest.SelectSingleNode("//uniqueId");
            xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime");
            xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime");
            xmlNodoService = XmlLoginTicketRequest.SelectSingleNode("//service");

            xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s");
            xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+10).ToString("s");
            xmlNodoUniqueId.InnerText = Convert.ToString(_globalUniqueID);
            xmlNodoService.InnerText = argServicio;
            this.Service = argServicio;

            _globalUniqueID += 1;

            if (this._verboseMode)
            {
                Console.WriteLine(XmlLoginTicketRequest.OuterXml);
            }
        }

        catch (Exception excepcionAlGenerarLoginTicketRequest)
        {
            throw new Exception("***Error GENERANDO el LoginTicketRequest : " + excepcionAlGenerarLoginTicketRequest.Message);
        }

        // PASO 2: Firmo el Login Ticket Request 
        try
        {
            if (this._verboseMode)
            {
                Console.WriteLine("***Leyendo certificado: {0}", RutaDelCertificadoFirmante);
            }

            X509Certificate2 certFirmante = CertificadosX509Lib.ObtieneCertificadoDesdeArchivo(RutaDelCertificadoFirmante);

            if (this._verboseMode)
            {
                Console.WriteLine("***Firmando: ");
                Console.WriteLine(XmlLoginTicketRequest.OuterXml);
            }

            // Convierto el login ticket request a bytes, para firmar 
            Encoding EncodedMsg = Encoding.UTF8;
            byte[] msgBytes = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml);

            // Firmo el msg y paso a Base64 
            byte[] encodedSignedCms = CertificadosX509Lib.FirmaBytesMensaje(msgBytes, certFirmante);
            cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);
        }

        catch (Exception excepcionAlFirmar)
        {
            throw new Exception("***Error FIRMANDO el LoginTicketRequest : " + excepcionAlFirmar.Message);
        }

        // PASO 3: Invoco al WSAA para obtener el Login Ticket Response 
        try
        {
            if (this._verboseMode)
            {
                Console.WriteLine("***Llamando al WSAA en URL: {0}", argUrlWsaa);
                Console.WriteLine("***Argumento en el request:");
                Console.WriteLine(cmsFirmadoBase64);
            }
          
            ClienteLoginCms_CS.Wsaa.LoginCMSService servicioWsaa = new  ClienteLoginCms_CS.Wsaa.LoginCMSService();
            servicioWsaa.Url = argUrlWsaa;

            loginTicketResponse = servicioWsaa.loginCms(cmsFirmadoBase64);
            
            if (this._verboseMode)
            {
                Console.WriteLine("***LoguinTicketResponse: ");
                Console.WriteLine(loginTicketResponse);
            }
        }

        catch (Exception excepcionAlInvocarWsaa)
        {
            throw new Exception("***Error INVOCANDO al servicio WSAA : " + excepcionAlInvocarWsaa.Message);
        }


        // PASO 4: Analizo el Login Ticket Response recibido del WSAA 
        try
        {
            XmlLoginTicketResponse = new XmlDocument();
            XmlLoginTicketResponse.LoadXml(loginTicketResponse);

            this.UniqueId = UInt32.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId").InnerText);
            this.GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText);
            this.ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);
            this.Sign = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText;
            this.Token = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText;
        }
        catch (Exception excepcionAlAnalizarLoginTicketResponse)
        {
            throw new Exception("***Error ANALIZANDO el LoginTicketResponse : " + excepcionAlAnalizarLoginTicketResponse.Message);
        }

        return loginTicketResponse;

    }


}

/// <summary> 
/// Libreria de utilidades para manejo de certificados 
/// </summary> 
/// <remarks></remarks> 
class CertificadosX509Lib
{

    public static bool VerboseMode = false;

    /// <summary> 
    /// Firma mensaje 
    /// </summary> 
    /// <param name="argBytesMsg">Bytes del mensaje</param> 
    /// <param name="argCertFirmante">Certificado usado para firmar</param> 
    /// <returns>Bytes del mensaje firmado</returns> 
    /// <remarks></remarks> 
    public static byte[] FirmaBytesMensaje(byte[] argBytesMsg, X509Certificate2 argCertFirmante)
    {
        try
        {
            // Pongo el mensaje en un objeto ContentInfo (requerido para construir el obj SignedCms) 
            ContentInfo infoContenido = new ContentInfo(argBytesMsg);
            SignedCms cmsFirmado = new SignedCms(infoContenido);

            // Creo objeto CmsSigner que tiene las caracteristicas del firmante 
            CmsSigner cmsFirmante = new CmsSigner(argCertFirmante);
            cmsFirmante.IncludeOption = X509IncludeOption.EndCertOnly;

            if (VerboseMode)
            {
                Console.WriteLine("***Firmando bytes del mensaje...");
            }
            // Firmo el mensaje PKCS #7 
            cmsFirmado.ComputeSignature(cmsFirmante);

            if (VerboseMode)
            {
                Console.WriteLine("***OK mensaje firmado");
            }

            // Encodeo el mensaje PKCS #7. 
            return cmsFirmado.Encode();
        }
        catch (Exception excepcionAlFirmar)
        {
            throw new Exception("***Error al firmar: " + excepcionAlFirmar.Message);
        }
    }

    /// <summary> 
    /// Lee certificado de disco 
    /// </summary> 
    /// <param name="argArchivo">Ruta del certificado a leer.</param> 
    /// <returns>Un objeto certificado X509</returns> 
    /// <remarks></remarks> 
    public static X509Certificate2 ObtieneCertificadoDesdeArchivo(string argArchivo)
    {
        X509Certificate2 objCert = new X509Certificate2();

        try
        {
            objCert.Import(Microsoft.VisualBasic.FileIO.FileSystem.ReadAllBytes(argArchivo));
            return objCert;
        }
        catch (Exception excepcionAlImportarCertificado)
        {
            throw new Exception("argArchivo=" + argArchivo + " excepcion=" + excepcionAlImportarCertificado.Message + " " + excepcionAlImportarCertificado.StackTrace);

        }
    }

}

/// <summary> 
/// Clase principal 
/// </summary> 
/// <remarks></remarks> 
class ProgramaPrincipal
{

    // Valores por defecto, globales en esta clase 
    const string DEFAULT_URLWSAAWSDL = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL";
    const string DEFAULT_URLWSFEWSDL = "https://wswhomo.afip.gov.ar/wsfev1/service.asmx?WSDL";
    const string DEFAULT_SERVICIO = "wsfe";
    const string DEFAULT_CERTSIGNER = "f:\\wsaa_test\\MiCertificadoConClavePrivada.pfx";
    const long DEFAULT_CUIT = -1;
    const string DEFAULT_ARCHIVOFACTURA = "factura.xml";
    const string DEFAULT_ARCHIVOSALIDA = "salida.xml";
    const bool DEFAULT_VERBOSE = true;

    /// <summary> 
    /// Funcion Main (consola) 
    /// </summary> 
    /// <param name="args">Argumentos de linea de comandos</param> 
    /// <returns>0 si terminó bien, valores negativos si hubieron errores</returns> 
    /// <remarks></remarks> 
    public static int Main(string[] args)
    {

        MostrarVersion();

        string strUrlWsaaWsdl = DEFAULT_URLWSAAWSDL;
        string strUrlWsfeWsdl = DEFAULT_URLWSFEWSDL;
        string strIdServicioNegocio = DEFAULT_SERVICIO;
        string strRutaCertSigner = DEFAULT_CERTSIGNER;
        long   longCuit = DEFAULT_CUIT;
        string strFactura = DEFAULT_ARCHIVOFACTURA;
        string strSalida = DEFAULT_ARCHIVOSALIDA;
        bool blnVerboseMode = DEFAULT_VERBOSE;

        // Analizo argumentos de linea de comandos 
        if (args.Length == 0)
        {
            ExplicarUso();
            return -1;
        }
        for (int i = 0; i <= args.Length - 1; i++)
        {
            string argumento;
            argumento = args[i];

            if (String.Compare(argumento, "-w", true) == 0  )
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó la URL del WSDL del WSAA");
                    return -1;
                }
                else
                {
                    strUrlWsaaWsdl = args[i + 1];
                    i = i + 1;
                }
            }

            if (String.Compare(argumento, "-x", true) == 0)
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó la URL del WSDL del WSAA");
                    return -1;
                }
                else
                {
                    strUrlWsfeWsdl = args[i + 1];
                    i = i + 1;
                }
            }

            else if (String.Compare(argumento, "-s", true) == 0 )
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó el ID del servicio de negocio");
                    return -1;
                }
                else
                {
                    strIdServicioNegocio = args[i + 1];
                    i = i + 1;
                }
            }

            else if (String.Compare(argumento, "-c", true) == 0  )
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó ruta del certificado firmante");
                    return -1;
                }
                else
                {
                    strRutaCertSigner = args[i + 1];
                    i = i + 1;
                }
            }

            else if (String.Compare(argumento, "-t", true) == 0)
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó el CUIT");
                    return -1;
                }
                else
                {
                    longCuit = long.Parse(args[i + 1]);
                    i = i + 1;
                }
            }

            else if (String.Compare(argumento, "-f", true) == 0)
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó el archivo de entrada");
                    return -1;
                }
                else
                {
                    strFactura = args[i + 1];
                    i = i + 1;
                }
            }

            else if (String.Compare(argumento, "-o", true) == 0)
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó el archivo de salida");
                    return -1;
                }
                else
                {
                    strSalida = args[i + 1];
                    i = i + 1;
                }
            }

            else if (String.Compare(argumento, "-v", true) == 0  )
            {
                if (args.Length < (i + 2))
                {
                    Console.WriteLine("Error: no se especificó modo: on|off");
                    return -1;
                }
                else
                {
                    blnVerboseMode = ( String.Compare(args[i + 1], "on", true) == 0  ? true : false);
                    i = i + 1;
                }
            }

            else if (String.Compare(argumento, "-?", true) == 0 )
            {
                ExplicarUso();
                return 0;
            }
            else
            {

                Microsoft.VisualBasic.ApplicationServices.ApplicationBase MyApplication = new Microsoft.VisualBasic.ApplicationServices.ApplicationBase();

                Console.WriteLine("Error: argumento desconocido: {0}", argumento);
                Console.WriteLine("Para obtener ayuda: {0} -?", MyApplication.Info.AssemblyName);
                return -2;
            }
        }

        // Argumentos OK, entonces procesar normalmente... 

        LoginTicket objTicketRespuesta;
        string strTicketRespuesta;

        try
        {

            if (blnVerboseMode)
            {
                Console.WriteLine("***Servicio a acceder: {0}", strIdServicioNegocio);
                Console.WriteLine("***URL del WSAA: {0}", strUrlWsaaWsdl);
                Console.WriteLine("***URL del WSFE: {0}", strUrlWsfeWsdl);
                Console.WriteLine("***Ruta del certificado: {0}", strRutaCertSigner);
                Console.WriteLine("***Archivo de factura: {0}", strFactura);
                Console.WriteLine("***Archivo de salida: {0}", strSalida);
                Console.WriteLine("***Nro de CUIT en certificado: {0}", longCuit);
                Console.WriteLine("***Modo verbose: {0}", blnVerboseMode);

            }

            objTicketRespuesta = new LoginTicket();

            if (blnVerboseMode)
            {
                Console.WriteLine("***Accediendo a {0}", strUrlWsaaWsdl);
            }

            strTicketRespuesta = objTicketRespuesta.ObtenerLoginTicketResponse(strIdServicioNegocio, strUrlWsaaWsdl, strRutaCertSigner, blnVerboseMode);

            if (blnVerboseMode)
            {
                Console.WriteLine("***CONTENIDO DEL TICKET RESPUESTA:");
                Console.WriteLine(" Token: {0}", objTicketRespuesta.Token);
                Console.WriteLine(" Sign: {0}", objTicketRespuesta.Sign);
                Console.WriteLine(" GenerationTime: {0}", Convert.ToString(objTicketRespuesta.GenerationTime));
                Console.WriteLine(" ExpirationTime: {0}", Convert.ToString(objTicketRespuesta.ExpirationTime));
                Console.WriteLine(" Service: {0}", objTicketRespuesta.Service);
                Console.WriteLine(" UniqueID: {0}", Convert.ToString(objTicketRespuesta.UniqueId));
            }

        }
        catch (Exception excepcionAlObtenerTicket)
        {

            Console.WriteLine("***EXCEPCION AL OBTENER TICKET:");
            Console.WriteLine(excepcionAlObtenerTicket.Message);
            return -10;
        }

        WsfeSolicitud wsfeSolicitud = new WsfeSolicitud(strUrlWsfeWsdl, objTicketRespuesta.Token, objTicketRespuesta.Sign);
        wsfeSolicitud.verbose = blnVerboseMode;
        try
        {
            InputReader reader = new InputReader();
            Factura f = reader.leeFacturaEnXml(strFactura);

            wsfeSolicitud.send(longCuit, f, strSalida);
        }
        catch (Exception excepcionAlMandarFE)
        {

            Console.WriteLine("***EXCEPCION AL MANDAR EL PEDIDO DE FE:");
            Console.WriteLine(excepcionAlMandarFE.Message);
            Console.WriteLine(excepcionAlMandarFE.Source);

            OutputWriter output = new OutputWriter();
            output.verbose = blnVerboseMode;

            output.escribirRespuestaFacturaXml(strSalida, excepcionAlMandarFE);

            return -11;

        }

        return 0;
    }

    /// <summary> 
    /// Explica el uso del comando 
    /// </summary> 
    /// <remarks></remarks> 
    public static void ExplicarUso()
    {

        Microsoft.VisualBasic.ApplicationServices.ApplicationBase MyApplication = new Microsoft.VisualBasic.ApplicationServices.ApplicationBase();

        Console.WriteLine("");
        Console.WriteLine("Uso: {0} (opciones) ...", MyApplication.Info.AssemblyName);
        Console.WriteLine("");
        Console.WriteLine("opciones:");
        Console.WriteLine(" -s servicio ID del servicio de negocio");
        Console.WriteLine(" Valor por defecto: " + DEFAULT_SERVICIO);
        Console.WriteLine(" -c certificado Ruta del certificado (con clave privada)");
        Console.WriteLine(" Valor por defecto: " + DEFAULT_CERTSIGNER);
        Console.WriteLine(" -t CUIT");
        Console.WriteLine(" Valor por defecto: " + DEFAULT_CUIT);
        Console.WriteLine(" -w url URL del WSDL del WSAA");
        Console.WriteLine(" Valor por defecto: " + DEFAULT_URLWSAAWSDL);
        Console.WriteLine(" -x url URL del WSDL del WSFE");
        Console.WriteLine(" Valor por defecto: " + DEFAULT_URLWSFEWSDL);
        Console.WriteLine(" -f archivo_de_factura.xml");
        Console.WriteLine(" Valor por defecto: " + DEFAULT_ARCHIVOFACTURA);
        Console.WriteLine(" -o archivo_de_salida.xml");
        Console.WriteLine(" Valor por defecto: " + DEFAULT_ARCHIVOSALIDA);
        Console.WriteLine(" -v on|off Reportes detallados de la ejecución");
        Console.WriteLine(" -? Esta ayuda");
    }

    public static void MostrarVersion()
    {
        Microsoft.VisualBasic.ApplicationServices.ApplicationBase MyApplication = new Microsoft.VisualBasic.ApplicationServices.ApplicationBase();

        Console.WriteLine("Aplicacion : {0}", MyApplication.Info.AssemblyName);
        Console.WriteLine("Version : {0}", Convert.ToString(MyApplication.Info.Version));
        Console.WriteLine("Descripcion: {0}", MyApplication.Info.Description);
    }



}
