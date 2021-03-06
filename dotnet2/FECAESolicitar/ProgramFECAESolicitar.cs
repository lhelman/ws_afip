﻿using System;

namespace FECAESolicitar
{

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
            string strPassword = null;
            long longCuit = DEFAULT_CUIT;
            string strFactura = DEFAULT_ARCHIVOFACTURA;
            string strSalida = DEFAULT_ARCHIVOSALIDA;
            bool blnVerboseMode = DEFAULT_VERBOSE;

            // Analizo argumentos de linea de comandos 
            if (args.Length == 0)
            {
                explicarUso();
                return -1;
            }
            for (int i = 0; i <= args.Length - 1; i++)
            {
                string argumento;
                argumento = args[i];

                if (String.Compare(argumento, "-w", true) == 0)
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

                else if (String.Compare(argumento, "-x", true) == 0)
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

                else if (String.Compare(argumento, "-s", true) == 0)
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

                else if (String.Compare(argumento, "-c", true) == 0)
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

                else if (String.Compare(argumento, "-p", true) == 0)
                {
                    if (args.Length < (i + 2))
                    {
                        Console.WriteLine("Error: no se especificó el password");
                        return -1;
                    }
                    else
                    {
                        strPassword = args[i + 1];
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

                else if (String.Compare(argumento, "-v", true) == 0)
                {
                    if (args.Length < (i + 2))
                    {
                        Console.WriteLine("Error: no se especificó modo: on|off");
                        return -1;
                    }
                    else
                    {
                        blnVerboseMode = (String.Compare(args[i + 1], "on", true) == 0 ? true : false);
                        i = i + 1;
                    }
                }

                else if (String.Compare(argumento, "-?", true) == 0)
                {
                    explicarUso();
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
            if (isMissingParameter(longCuit))
            {
                return -3;
            }

            //var x = new WsfeSolicitud("xxx", "token", "bla");
            //x.pruebas(12345678, "lalala.xml", "lalala.xml");
           // return 1;



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
                    Console.WriteLine("***Password del certificado: {0}", (strPassword == null ? "-NO PASSWD-" : "*****"));
                    Console.WriteLine("***Archivo de factura: {0}", strFactura);
                    Console.WriteLine("***Archivo de salida: {0}", strSalida);
                    Console.WriteLine("***Nro de CUIT en certificado: {0}", longCuit);
                    Console.WriteLine("***Modo verbose: {0}", blnVerboseMode);
                    Console.WriteLine(" CWD: {0}", AppDomain.CurrentDomain.BaseDirectory);

                }


                objTicketRespuesta = new LoginTicket();

                if (blnVerboseMode)
                {
                    Console.WriteLine("***Accediendo a {0}", strUrlWsaaWsdl);
                }

                strTicketRespuesta = objTicketRespuesta.ObtenerLoginTicketResponse(strIdServicioNegocio, strUrlWsaaWsdl, strRutaCertSigner, strPassword, blnVerboseMode);

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
            return wsfeSolicitud.send(longCuit, strFactura, strSalida);
        }

        public static bool isMissingParameter(long longCuit)
        {
            if (longCuit == DEFAULT_CUIT)
            {
                Console.WriteLine("Debe especificar un CUIT");
                explicarUso();
                return true;
            }
            return false;
        }

        /// <summary> 
        /// Explica el uso del comando 
        /// </summary> 
        /// <remarks></remarks> 
        public static void explicarUso()
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
            Console.WriteLine(" -p password del certificado");
            Console.WriteLine(" -t CUIT");
            Console.WriteLine(" Valor por defecto: " + DEFAULT_CUIT);
            Console.WriteLine(" -w url URL del WSDL del WSAA");
            Console.WriteLine(" Valor por defecto: " + DEFAULT_URLWSAAWSDL);
            Console.WriteLine(" -x url URL del WSDL del WSFE");
            Console.WriteLine(" Valor por defecto: " + DEFAULT_URLWSFEWSDL);
            Console.WriteLine(" -f factura.xml");
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
}
