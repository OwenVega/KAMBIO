using System;
using System.Text;

using System.Collections.Generic;

namespace KAMBIO.CORE.Core.DTOs
{
    public class HistorialPaginadoDTO
    {
        public ResumenHistorialDTO Resumen { get; set; }
        public List<TransaccionHistorialDTO> Transacciones { get; set; }
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
