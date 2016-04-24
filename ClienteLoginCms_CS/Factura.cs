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

        public override String ToString()
        {
            return String.Format("CantReg={0} " +
                                 "CbteTipo={1} " +
                                 "PtoVta={2} ",
                                 CantReg,
                                 CbteTipo,
                                 PtoVta);
        }
    }
}
