using System;
using System.Collections.Generic;
using System.Text;

namespace KAMBIO.CORE.Core.DTOs
{
    public class FiltroHistorialDTO
    {
        public string? BusquedaDivisas { get; set; } 
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? TipoOperacion { get; set; } 
        public int? IdEstado { get; set; }
        public int Pagina { get; set; } = 1;
        public int CantidadPorPagina { get; set; } = 10;
    }
}