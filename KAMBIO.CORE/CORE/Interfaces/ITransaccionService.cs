using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface ITransaccionService
    {
        Task<HistorialPaginadoDTO> ObtenerHistorialUsuarioAsync(int idUsuario, FiltroHistorialDTO filtro);
    }
}