using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface ITransaccionService
    {
        Task CambiarEstadoAsync(CambiarEstadoDto dto);
        Task<TransaccionDetalleDto> ObtenerPorIdAsync(int idTransaccion);
        Task<HistorialPaginadoDTO> ObtenerHistorialUsuarioAsync(int idUsuario, FiltroHistorialDTO filtro);
        Task<TransaccionDetalleDto> CrearTransaccionDesdeOfertaAsync(int idOferta, int idUsuarioComprador);
    }
}