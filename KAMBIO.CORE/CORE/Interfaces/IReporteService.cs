using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IReporteService
    {
        byte[] ExportarExcel(List<ReporteTransaccionDto> datos);
        byte[] ExportarPdf(List<ReporteTransaccionDto> datos);
        Task<List<ReporteTransaccionDto>> ObtenerTransaccionesAsync(FiltroReporteDto filtro);
    }
}