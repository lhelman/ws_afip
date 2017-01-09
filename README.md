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
FECAESolicitar.exe -w https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL -x https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL -f  C:\Afip\Factura.xml -c C:\Afip\keyfile.pfx -v on -o C:\Afip\Salida.xml -t 1234567890
```

## Consultando por tipo/nro/punto de venta

```
FECompConsultar.exe -w https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL -x https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL -T 2 -N 123 -P 1 -c C:\Afip\keyfile.pfx -v on -o C:\Afip\Salida.xml -t 1234567890
```

## Consultando por tipo/nro/punto de venta

```
FECompUltimoAutorizado.exe -w https://wsaa.afip.gov.ar/ws/services/LoginCms?WSDL -x https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL -T 2 -P 1 -c C:\Afip\keyfile.pfx -v on -o C:\Afip\Salida.xml -t 1234567890
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


