using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IMetodoPagoService
    {
        Task EliminarMetodoPagoAsync(int idMetodoPago, int idUsuario);
        Task<IEnumerable<MetodoPagoListDto>> ObtenerMetodosPagoUsuarioAsync(int idUsuario);
        Task RegistrarMetodoPagoAsync(MetodoPagoCrearDto dto);
    }
}