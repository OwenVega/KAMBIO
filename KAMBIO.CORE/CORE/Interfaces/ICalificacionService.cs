using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface ICalificacionService
    {
        Task CalificarAsync(CalificacionDto dto);
        Task<PromedioCalificacionDto> ObtenerPromedioAsync(int idUsuario);
        Task<List<ReseñaDto>> ObtenerReseñasAsync(int idUsuario);
    }
}