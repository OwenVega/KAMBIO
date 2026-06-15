using System;
using System.Collections.Generic;
using System.Text;

namespace KAMBIO.CORE.Core.DTOs
{
    public class FiltroOfertaDTO
    {
        public int IdTipoOferta { get; set; }

        public int IdDivisaOrigen { get; set; }

        public int IdDivisaDestino { get; set; }

        public decimal? Monto { get; set; }

        public int? IdBanco { get; set; }
    }
}
