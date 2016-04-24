'<summary>
'EJEMPLO - Consola cliente del webservice de autenticacion y autorizacion
'</summary>
'<description>
'   Consume el metodo LoginCms del WSAA
'   Muestra en stdout el login ticket response
'</description>
'<version>10.09.30.1800</version>
'<usage>
'   clientelogincms (opciones) ...
'   opciones:
'      -s servicio      ID del servicio de negocio
'      -w url           URL del WSDL del WSAA
'      -c certificado   Ruta del certificado (con clave privada)
'      -p pasword       Password del certificado (con clave privada)
'      -x proxy         IP:port del proxy
'      -y usuario proxy Usuario del proxy
'      -z pass proxy    Password del proxy
'      -v on|off        Salida detallada on/off
'      -?               Muestra ayuda de uso
'</usage>
'<platform>.NET Framework 2.0</platform>
'<disclaimer>
' El Departamento de Arquitectura Informatica de la AFIP (DeArIn/AFIP), pone a disposicion
' el siguiente codigo para su utilizacion con el WebService de Facturacion Electronica (WSFE)
' de la AFIP.
'
' El mismo no puede ser re-distribuido, publicado o descargado en forma total o parcial, ya sea
' en forma electronica, mecanica u optica, sin la autorizacion de DeArIn/AFIP. El uso no
' autorizado del mismo esta prohibido.
'
' DeArIn/AFIP no asume ninguna responsabilidad de los errores que pueda contener el codigo ni la
' obligacion de subsanar dichos errores o informar de la existencia de los mismos.
'
' DeArIn/AFIP no asume ninguna responsabilidad que surja de la utilizacion del codigo, ya sea por
' utilizacion ilegal de patentes, perdida de beneficios, perdida de informacion o cualquier otro
' inconveniente.
'
' Bajo ninguna circunstancia DeArIn/AFIP podra ser indicada como responsable por consecuencias y/o
' incidentes ya sean directos o indirectos que puedan surgir de la utilizacion del codigo.
'
' DeArIn/AFIP no da ninguna garantia, expresa o implicita, de la utilidad del codigo, si el mismo es
' correcto, o si cumple con los requerimientos de algun proposito en particular.
'
' DeArIn/AFIP puede realizar cambios en cualquier momento en el codigo sin previo aviso.
'
' El codigo debera ser evaluado, verificado, corregido y/o adaptado por personal tecnico calificado
' de las entidades que lo utilicen.
'
' EL SIGUIENTE CODIGO ES DISTRIBUIDO PARA EVALUACION, CON TODOS SUS ERRORES Y OMISIONES. LA
' RESPONSABILIDAD DEL CORRECTO FUNCIONAMIENTO DEL MISMO YA SEA POR SI SOLO O COMO PARTE DE
' OTRA APLICACION, QUEDA A CARGO DE LAS ENTIDADES QUE LO UTILICEN. LA UTILIZACION DEL CODIGO
' SIGNIFICA LA ACEPTACION DE TODOS LOS TERMINOS Y CONDICIONES MENCIONADAS ANTERIORMENTE.
'
' DeArIn-AFIP
'</disclaimer>

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Xml
Imports System.Net
Imports System.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.Pkcs
Imports System.Security.Cryptography.X509Certificates
Imports System.IO
Imports System.Runtime.InteropServices


''' <summary>
''' Clase para crear objetos Login Tickets
''' </summary>
''' <remarks>
''' Ver documentacion: 
'''    Especificacion Tecnica del Webservice de Autenticacion y Autorizacion
'''    Version 1.0
'''    Departamento de Seguridad Informatica - AFIP
''' </remarks>
Class LoginTicket

    Public UniqueId As UInt32 ' Entero de 32 bits sin signo que identifica el requerimiento
    Public GenerationTime As DateTime ' Momento en que fue generado el requerimiento
    Public ExpirationTime As DateTime ' Momento en el que exoira la solicitud
    Public Service As String ' Identificacion del WSN para el cual se solicita el TA
    Public Sign As String ' Firma de seguridad recibida en la respuesta
    Public Token As String ' Token de seguridad recibido en la respuesta

    Public XmlLoginTicketRequest As XmlDocument = Nothing
    Public XmlLoginTicketResponse As XmlDocument = Nothing
    Public RutaDelCertificadoFirmante As String
    Public XmlStrLoginTicketRequestTemplate As String = "<loginTicketRequest><header><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>"

    Private _verboseMode As Boolean = True

    Private Shared _globalUniqueID As UInt32 = 0 ' OJO! NO ES THREAD-SAFE

    ''' <summary>
    ''' Construye un Login Ticket obtenido del WSAA
    ''' </summary>
    ''' <param name="argServicio">Servicio al que se desea acceder</param>
    ''' <param name="argUrlWsaa">URL del WSAA</param>
    ''' <param name="argRutaCertX509Firmante">Ruta del certificado X509 (con clave privada) usado para firmar</param>
    ''' <param name="argPassword">Password del certificado X509 (con clave privada) usado para firmar</param>
    ''' <param name="argProxy">IP:port del proxy</param>
    ''' <param name="argProxyUser">Usuario del proxy</param>''' 
    ''' <param name="argProxyPassword">Password del proxy</param>
    ''' <param name="argVerbose">Nivel detallado de descripcion? true/false</param>
    ''' <remarks></remarks>
    Public Function ObtenerLoginTicketResponse( _
    ByVal argServicio As String, _
    ByVal argUrlWsaa As String, _
    ByVal argRutaCertX509Firmante As String, _
    ByVal argPassword As SecureString, _
    ByVal argProxy As String, _
    ByVal argProxyUser As String, _
    ByVal argProxyPassword As String, _
    ByVal argVerbose As Boolean _
    ) As String

        Me.RutaDelCertificadoFirmante = argRutaCertX509Firmante
        Me._verboseMode = argVerbose
        CertificadosX509Lib.VerboseMode = argVerbose

        Dim cmsFirmadoBase64 As String
        Dim loginTicketResponse As String
        Dim xmlNodoUniqueId As XmlNode
        Dim xmlNodoGenerationTime As XmlNode
        Dim xmlNodoExpirationTime As XmlNode
        Dim xmlNodoService As XmlNode

        ' PASO 1: Genero el Login Ticket Request
        Try
            _globalUniqueID += 1

            XmlLoginTicketRequest = New XmlDocument()
            XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate)

            xmlNodoUniqueId = XmlLoginTicketRequest.SelectSingleNode("//uniqueId")
            xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime")
            xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime")
            xmlNodoService = XmlLoginTicketRequest.SelectSingleNode("//service")

            xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s")
            xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+10).ToString("s")
            xmlNodoUniqueId.InnerText = CStr(_globalUniqueID)
            xmlNodoService.InnerText = argServicio
            Me.Service = argServicio

            If Me._verboseMode Then
                Console.WriteLine(XmlLoginTicketRequest.OuterXml)
            End If

        Catch excepcionAlGenerarLoginTicketRequest As Exception
            Throw New Exception("***Error GENERANDO el LoginTicketRequest : " + excepcionAlGenerarLoginTicketRequest.Message + excepcionAlGenerarLoginTicketRequest.StackTrace)
        End Try

        ' PASO 2: Firmo el Login Ticket Request
        Try
            If Me._verboseMode Then
                Console.WriteLine("***Leyendo certificado: {0}", RutaDelCertificadoFirmante)
            End If

            Dim certFirmante As X509Certificate2 = CertificadosX509Lib.ObtieneCertificadoDesdeArchivo(RutaDelCertificadoFirmante, argPassword)

            If Me._verboseMode Then
                Console.WriteLine("***Firmando: ")
                Console.WriteLine(XmlLoginTicketRequest.OuterXml)
            End If

            ' Convierto el login ticket request a bytes, para firmar
            Dim EncodedMsg As Encoding = Encoding.UTF8
            Dim msgBytes As Byte() = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml)

            ' Firmo el msg y paso a Base64
            Dim encodedSignedCms As Byte() = CertificadosX509Lib.FirmaBytesMensaje(msgBytes, certFirmante)
            cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms)

        Catch excepcionAlFirmar As Exception
            Throw New Exception("***Error FIRMANDO el LoginTicketRequest : " + excepcionAlFirmar.Message)
        End Try

        ' PASO 3: Invoco al WSAA para obtener el Login Ticket Response
        Try
            If Me._verboseMode Then
                Console.WriteLine("***Llamando al WSAA en URL: {0}", argUrlWsaa)
                Console.WriteLine("***Argumento en el request:")
                Console.WriteLine(cmsFirmadoBase64)
            End If

            Dim servicioWsaa As New Wsaa.LoginCMSService()
            servicioWsaa.Url = argUrlWsaa
            If argProxy IsNot Nothing Then
                servicioWsaa.Proxy = New WebProxy(argProxy, True)
                If argProxyUser IsNot Nothing Then
                    Dim Credentials As New NetworkCredential(argProxyUser, argProxyPassword)
                    servicioWsaa.Proxy.Credentials = Credentials
                End If
            End If
            loginTicketResponse = servicioWsaa.loginCms(cmsFirmadoBase64)

            If Me._verboseMode Then
                Console.WriteLine("***LoguinTicketResponse: ")
                Console.WriteLine(loginTicketResponse)
            End If

        Catch excepcionAlInvocarWsaa As Exception
            Throw New Exception("***Error INVOCANDO al servicio WSAA : " + excepcionAlInvocarWsaa.Message)
        End Try


        ' PASO 4: Analizo el Login Ticket Response recibido del WSAA
        Try
            XmlLoginTicketResponse = New XmlDocument()
            XmlLoginTicketResponse.LoadXml(loginTicketResponse)

            Me.UniqueId = UInt32.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId").InnerText)
            Me.GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText)
            Me.ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText)
            Me.Sign = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText
            Me.Token = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText
        Catch excepcionAlAnalizarLoginTicketResponse As Exception
            Throw New Exception("***Error ANALIZANDO el LoginTicketResponse : " + excepcionAlAnalizarLoginTicketResponse.Message)
        End Try

        Return loginTicketResponse

    End Function


End Class

''' <summary>
''' Libreria de utilidades para manejo de certificados
''' </summary>
''' <remarks></remarks>
Class CertificadosX509Lib

    Public Shared VerboseMode As Boolean = False

    ''' <summary>
    ''' Firma mensaje
    ''' </summary>
    ''' <param name="argBytesMsg">Bytes del mensaje</param>
    ''' <param name="argCertFirmante">Certificado usado para firmar</param>
    ''' <returns>Bytes del mensaje firmado</returns>
    ''' <remarks></remarks>
    Public Shared Function FirmaBytesMensaje( _
    ByVal argBytesMsg As Byte(), _
    ByVal argCertFirmante As X509Certificate2 _
    ) As Byte()
        Try
            ' Pongo el mensaje en un objeto ContentInfo (requerido para construir el obj SignedCms)
            Dim infoContenido As New ContentInfo(argBytesMsg)
            Dim cmsFirmado As New SignedCms(infoContenido)

            ' Creo objeto CmsSigner que tiene las caracteristicas del firmante
            Dim cmsFirmante As New CmsSigner(argCertFirmante)
            cmsFirmante.IncludeOption = X509IncludeOption.EndCertOnly

            If VerboseMode Then
                Console.WriteLine("***Firmando bytes del mensaje...")
            End If
            ' Firmo el mensaje PKCS #7
            cmsFirmado.ComputeSignature(cmsFirmante)

            If VerboseMode Then
                Console.WriteLine("***OK mensaje firmado")
            End If

            ' Encodeo el mensaje PKCS #7.
            Return cmsFirmado.Encode()
        Catch excepcionAlFirmar As Exception
            Throw New Exception("***Error al firmar: " & excepcionAlFirmar.Message)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Lee certificado de disco
    ''' </summary>
    ''' <param name="argArchivo">Ruta del certificado a leer.</param>
    ''' <returns>Un objeto certificado X509</returns>
    ''' <remarks></remarks>
    Public Shared Function ObtieneCertificadoDesdeArchivo( _
    ByVal argArchivo As String, _
    ByVal argPassword As SecureString _
    ) As X509Certificate2
        Dim objCert As New X509Certificate2
        Try
            If argPassword.IsReadOnly Then
                objCert.Import(My.Computer.FileSystem.ReadAllBytes(argArchivo), argPassword, X509KeyStorageFlags.PersistKeySet)
            Else
                objCert.Import(My.Computer.FileSystem.ReadAllBytes(argArchivo))
            End If
            Return objCert
        Catch excepcionAlImportarCertificado As Exception
            Throw New Exception(excepcionAlImportarCertificado.Message & " " & excepcionAlImportarCertificado.StackTrace)
            Return Nothing
        End Try
    End Function

End Class

''' <summary>
''' Clase principal
''' </summary>
''' <remarks></remarks>
Class ProgramaPrincipal

    ' Valores por defecto, globales en esta clase
    Const DEFAULT_URLWSAAWSDL As String = "https://wsaahomo.afip.gov.ar/ws/services/LoginCms?WSDL"
    Const DEFAULT_SERVICIO As String = "wsfe"
    Const DEFAULT_CERTSIGNER As String = "c:\MiCertificadoConClavePrivada.pfx"
    Const DEFAULT_PROXY As String = Nothing
    Const DEFAULT_PROXY_USER As String = Nothing
    Const DEFAULT_PROXY_PASSWORD As String = ""
    Const DEFAULT_VERBOSE As Boolean = True


    ''' <summary>
    ''' Funcion Main (consola)
    ''' </summary>
    ''' <param name="args">Argumentos de linea de comandos</param>
    ''' <returns>0 si terminó bien, valores negativos si hubieron errores</returns>
    ''' <remarks></remarks>
    Public Shared Function Main(ByVal args As String()) As Integer

        Call MostrarVersion()

        Dim strUrlWsaaWsdl As String = DEFAULT_URLWSAAWSDL
        Dim strIdServicioNegocio As String = DEFAULT_SERVICIO
        Dim strRutaCertSigner As String = DEFAULT_CERTSIGNER
        Dim strPasswordSecureString As New SecureString
        Dim strProxy As String = DEFAULT_PROXY
        Dim strProxyUser As String = DEFAULT_PROXY_USER
        Dim strProxyPassword As String = DEFAULT_PROXY_PASSWORD
        Dim blnVerboseMode As Boolean = DEFAULT_VERBOSE

        ' Analizo argumentos de linea de comandos

        If args.Length = 0 Then
            Call ExplicarUso()
            Return -1
        End If
        For i As Integer = 0 To args.Length - 1
            Dim argumento As String
            argumento = args(i)

            If argumento.ToLower = "-w" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó la URL del WSDL del WSAA")
                    Return -1
                Else
                    strUrlWsaaWsdl = args(i + 1)
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-s" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó el ID del servicio de negocio")
                    Return -1
                Else
                    strIdServicioNegocio = args(i + 1)
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-c" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó ruta del certificado firmante")
                    Return -1
                Else
                    strRutaCertSigner = args(i + 1)
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-p" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó password del certificado firmante")
                    Return -1
                Else
                    For Each character As Char In args(i + 1).ToCharArray
                        strPasswordSecureString.AppendChar(character)
                    Next
                    strPasswordSecureString.MakeReadOnly()
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-x" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó IP:port del proxy")
                    Return -1
                Else
                    strProxy = args(i + 1)
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-y" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó usuario del proxy")
                    Return -1
                Else
                    strProxyUser = args(i + 1)
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-z" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó password del proxy")
                    Return -1
                Else
                    strProxyPassword = args(i + 1)
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-v" Then
                If args.Length < (i + 2) Then
                    Console.WriteLine("Error: no se especificó modo: on|off")
                    Return -1
                Else
                    blnVerboseMode = IIf(args(i + 1).ToLower = "on", True, False)
                    i = i + 1
                End If

            ElseIf argumento.ToLower = "-?" Then
                Call ExplicarUso()
                Return 0
            Else
                Console.WriteLine("Error: argumento desconocido: {0}", argumento)
                Console.WriteLine("Para obtener ayuda: {0} -?", My.Application.Info.AssemblyName)
                Return -2
            End If
        Next

        ' Argumentos OK, entonces procesar normalmente...

        Dim objTicketRespuesta As LoginTicket
        Dim strTicketRespuesta As String

        Try

            If blnVerboseMode Then
                Console.WriteLine("***Servicio a acceder: {0}", strIdServicioNegocio)
                Console.WriteLine("***URL del WSAA: {0}", strUrlWsaaWsdl)
                Console.WriteLine("***Ruta del certificado: {0}", strRutaCertSigner)
                Console.WriteLine("***Modo verbose: {0}", blnVerboseMode)
            End If

            objTicketRespuesta = New LoginTicket

            If blnVerboseMode Then
                Console.WriteLine("***Accediendo a {0}", strUrlWsaaWsdl)
            End If

            strTicketRespuesta = objTicketRespuesta.ObtenerLoginTicketResponse(strIdServicioNegocio, strUrlWsaaWsdl, strRutaCertSigner, strPasswordSecureString, strProxy, strProxyUser, strProxyPassword, blnVerboseMode)

            If blnVerboseMode Then
                Console.WriteLine("***CONTENIDO DEL TICKET RESPUESTA:")
                Console.WriteLine("   Token: {0}", objTicketRespuesta.Token)
                Console.WriteLine("   Sign: {0}", objTicketRespuesta.Sign)
                Console.WriteLine("   GenerationTime: {0}", CStr(objTicketRespuesta.GenerationTime))
                Console.WriteLine("   ExpirationTime: {0}", CStr(objTicketRespuesta.ExpirationTime))
                Console.WriteLine("   Service: {0}", objTicketRespuesta.Service)
                Console.WriteLine("   UniqueID: {0}", CStr(objTicketRespuesta.UniqueId))
            End If

        Catch excepcionAlObtenerTicket As Exception

            Console.WriteLine("***EXCEPCION AL OBTENER TICKET:")
            Console.WriteLine(excepcionAlObtenerTicket.Message)
            Return -10

        End Try
        Return 0
    End Function

    ''' <summary>
    ''' Explica el uso del comando
    ''' </summary>
    ''' <remarks></remarks>
    Shared Sub ExplicarUso()
        Console.WriteLine("")
        Console.WriteLine("Uso: {0} (opciones) ...", My.Application.Info.AssemblyName)
        Console.WriteLine("")
        Console.WriteLine("opciones:")
        Console.WriteLine("")
        Console.WriteLine("  -s servicio      ID del servicio de negocio")
        Console.WriteLine("                   Valor por defecto: " & DEFAULT_SERVICIO)
        Console.WriteLine("")
        Console.WriteLine("  -c certificado   Ruta del certificado (con clave privada)")
        Console.WriteLine("                   Valor por defecto: " & DEFAULT_CERTSIGNER)
        Console.WriteLine("")
        Console.WriteLine("  -p password      Password del certificado (con clave privada)")
        Console.WriteLine("                   Valor por defecto: sin password")
        Console.WriteLine("")
        Console.WriteLine("  -x IP:port proxy IP:port del proxy")
        Console.WriteLine("                   Valor por defecto: sin proxy")
        Console.WriteLine("")
        Console.WriteLine("  -y usuario proxy Usuario del proxy")
        Console.WriteLine("                   Valor por defecto: sin usuario proxy")
        Console.WriteLine("")
        Console.WriteLine("  -z pass proxy    Password del proxy")
        Console.WriteLine("                   Valor por defecto: sin password proxy")
        Console.WriteLine("")
        Console.WriteLine("  -w url           URL del WSDL del WSAA")
        Console.WriteLine("                   Valor por defecto: " & DEFAULT_URLWSAAWSDL)
        Console.WriteLine("")
        Console.WriteLine("  -v on|off        Reportes detallados de la ejecución")
        Console.WriteLine("                   Valor por defecto: " & IIf(DEFAULT_VERBOSE, "on", "off"))
        Console.WriteLine("")
        Console.WriteLine("  -?               Esta ayuda")
    End Sub

    Shared Sub MostrarVersion()
        Console.WriteLine("Aplicacion : " & My.Application.Info.AssemblyName)
        Console.WriteLine("Version    : " & My.Application.Info.Version.ToString)
        Console.WriteLine("Descripcion: " & My.Application.Info.Description)
    End Sub
End Class