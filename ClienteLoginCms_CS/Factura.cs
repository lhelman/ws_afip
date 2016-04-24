using System;
using System.Collections.Generic;
using System.Text;

namespace ClienteLoginCms_CS
{
    class Factura
    {

        // FECabRequest
        public int CantReg;
        public int CbteTipo;
        public int PtoVta;

        // FECAEDetRequest
        public long DocNro;

        public int DocTipo;
        public long CbteDesde;
        public long CbteHasta;
        public int Concepto;
        public double ImpTotal;
        public double ImpNeto;
        public double ImpIVA;
        public string MonId;
        public double MonCotiz;

        // tributos
        public short tributoIva_id = 5;
        public double tributoIva_BaseImp;
        public double tributoIva_Importe;



        public override String ToString()
        {
            return String.Format(
                "CantReg={0} " +
                "CbteTipo={1} " +
                "PtoVta={2} " +
                "DocNro={3} " +
                "DocTipo={4} " +
                "CbteDesde={5} " +
                "CbteHasta={6} " +
                "Concepto={7} " +
                "ImpTotal={8} " +
                "ImpNeto={9} " +
                "ImpIVA={10} " +
                "MonId={11} " +
                "MonCotiz={12} " +
                "tributoIva_BaseImp={13} " +
                "tributoIva_Importe={14} " +
                "",
                CantReg,
                CbteTipo,
                PtoVta,
                DocNro,
                DocTipo,
                CbteDesde,
                CbteHasta,
                Concepto,
                ImpTotal,
                ImpNeto,
                ImpIVA,
                MonId,
                MonCotiz,
                tributoIva_BaseImp,
                tributoIva_Importe);
            
        }
    }
}
