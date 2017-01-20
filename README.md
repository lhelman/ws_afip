# ws_afip

## Dot Net windows :(

Esto esta basado en los ejemplos que hay en la [pagina](http://www.afip.gov.ar/ws/paso3.asp) de la AFIP

Use este por que necesito que corra en windows y se complico instalar las dependencias del python m2crypto y otras para hacer andar el pyafip que se ve mucho mas razonable.


Especificamente parti de este [ejemplo](http://www.afip.gob.ar/ws/WSAA/ejemplos/wsaa_cliente_dotnet2-10.09.30.zip)

# Ejecucion

Cada comando viene con su `-?` para pedirle ayuda

Algunos ejemplos:


## Mandando facturas al WS


```
FECAESolicitar.exe -w https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL -x https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL -f  C:\Afip\Factura.xml -c C:\Afip\keyfile.pfx -v on -o C:\Afip\Salida.xml -t NRO_CUIT
```



El formato de factura es:


```
<?xml version="1.0" encoding="utf-8"?>
<FECAERequest xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <FeCabReq>
    <CantReg>1</CantReg>
    <PtoVta>4</PtoVta>
    <CbteTipo>1</CbteTipo>
  </FeCabReq>
  <FeDetReq>
    <FECAEDetRequest>
      <Concepto>1</Concepto>
      <DocTipo>80</DocTipo>
      <DocNro>27123456780</DocNro>
      <CbteDesde>581</CbteDesde>
      <CbteHasta>581</CbteHasta>
      <CbteFch>20170120</CbteFch>
      <ImpTotal>17700.4</ImpTotal>
      <ImpTotConc>0</ImpTotConc>
      <ImpNeto>15456.57</ImpNeto>
      <ImpOpEx>0</ImpOpEx>
      <ImpTrib>0</ImpTrib>
      <ImpIVA>3245.88</ImpIVA>
      <MonId>PES</MonId>
      <MonCotiz>1</MonCotiz>
      <Iva>
        <AlicIva>
          <Id>5</Id>
          <BaseImp>15456.57</BaseImp>
          <Importe>3245.88</Importe>
        </AlicIva>
      </Iva>
    </FECAEDetRequest>
  </FeDetReq>
</FECAERequest>
```


## Consultando por tipo/nro/punto de venta


```
FECompConsultar.exe -w https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL -x https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL -T NRO_TIPO -N NRO_COMPROBANTE -P NRO_PUNTO_DE_VENTA -c C:\Afip\keyfile.pfx -v on -o C:\Afip\Salida.xml -t NRO_CUIT
```

## Consultando por tipo/nro/punto de venta

No funciona todavia:

```
FECompUltimoAutorizado.exe -w https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL -x https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL -T NRO_TIPO -P NRO_PUNTO_DE_VENTA -c C:\Afip\keyfile.pfx -v on -o C:\Afip\Salida.xml -t NRO_CUIT
```

# Instalando

Utilizar el [instalador](dotnet2/ProgramasAfipInstaller/Release/ProgramasAfipInstaller.msi?raw=true)

# Creando un entorno de desarrollo

El entorno windows de desarrollo me lo genere bajandome un windows de 
* https://developer.microsoft.com/en-us/microsoft-edge/tools/vms/windows/
* Instalandome un VisualStudio 2015
* Instalando la extension para generar instaladores
* https://visualstudiogallery.msdn.microsoft.com/f1cc3f3e-c300-40a7-8797-c509fb8933b9
* Apunte el VisualStudio a este proyecto en github


# Agregando una llamada nueva

* Parado en el solution explorer, apuntando a la solucion
* Add un proyecto nuevo del tipo: Console Application
* Copiar de alguna de las anteriores los .cs (Por ej de FECompConsultar)
* Add references
  * `System.Web.Services` 
  * `System.ServiceModel` 
  * `Microsoft.VisualBasic` 
  * `Microsoft.VisualBasic` 
  * `projects` - `WsAfipCommon`
* Si el xml que hay que mandar es complejo, se puede llamar al InputReader.cs que esta en WsAfipCommon


