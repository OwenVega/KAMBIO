using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface ITransaccionService
    {
        Task CambiarEstadoAsync(CambiarEstadoDto dto);
        Task<TransaccionDetalleDto> ObtenerPorIdAsync(int idTransaccion);
    }
}