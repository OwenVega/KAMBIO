using System;
using System.Collections.Generic;
using System.Text;

namespace KAMBIO.CORE.Core.DTOs
{
    public class TransaccionHistorialDTO
    {
        public int IdTransaccion { get; set; }
        public string FechaOperacion { get; set; }
        public string ParDivisas { get; set; }
        public string Tipo { get; set; }
        public decimal MontoOrigen { get; set; }
        public decimal MontoDestino { get; set; }
        public string Estado { get; set; }
    }
}