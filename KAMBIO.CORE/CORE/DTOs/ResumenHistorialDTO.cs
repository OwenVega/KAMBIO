using System;
using System.Collections.Generic;
using System.Text;

namespace KAMBIO.CORE.Core.DTOs
{
    public class ResumenHistorialDTO
    {
        public decimal VolumenMensualUSD { get; set; }
        public int OperacionesExitosas { get; set; }
        public double TiempoPromedioMinutos { get; set; }
    }
}
