using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClienteLoginCms_CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ClienteLoginCms_CS.Tests
{
    [TestClass()]
    public class InputReaderTests
    {
        [TestMethod()]
        public void leerFacturaCorrectaTest()
        {
            InputReader r = new InputReader();
            r.verbose = true;
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                        "<factura>\n" +
                        "<CantReg>1</CantReg>\n" +
                        "<CbteTipo>1</CbteTipo>\n" +
                        "<PtoVta>4</PtoVta>\n" +
                        "<DocNro>12345678901</DocNro>\n" +
                        "<DocTipo>80</DocTipo>\n" +
                        "<CbteDesde>1</CbteDesde>\n" +
                        "<CbteHasta>1</CbteHasta>\n" +
                        "<Concepto>1</Concepto>\n" +
                        "<ImpTotal>1234.49</ImpTotal>\n" +
                        "<ImpNeto>2345.25</ImpNeto>\n" +
                        "<ImpIVA>1222.24</ImpIVA>\n" +
                        "<MonId>PES</MonId>\n" +
                        "<MonCotiz>1</MonCotiz>\n" +
                        "<tributoIva_BaseImp>1234.25</tributoIva_BaseImp>\n" +
                        "<tributoIva_Importe>1123.24</tributoIva_Importe>\n" +
                        "</factura>\n";


            Factura f = r.leeFacturaEnXml(new StringReader(xml));
            Assert.IsNotNull(f);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException),
            "Un formato incorrecto fue permitido")]
        public void leerFacturaIncompletaTest()
        {
            string xml = "<x>1</x>";

            InputReader r = new InputReader();
            Factura f = r.leeFacturaEnXml(new StringReader(xml));
            //Assert.Fail();
        }

    }
}