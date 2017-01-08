using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;

/// <summary> 
/// Libreria de utilidades para manejo de certificados 
/// </summary> 
/// <remarks></remarks> 
public class CertificadosX509Lib
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
