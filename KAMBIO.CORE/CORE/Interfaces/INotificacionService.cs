using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface INotificacionService
    {
        Task<IEnumerable<NotificacionDto>> ObtenerNotificacionesAsync(int idUsuario);
        Task MarcarComoLeidaAsync(int idNotificacion);
        Task MarcarTodasComoLeidasAsync(int idUsuario);
        Task<int> ContarNoLeidasAsync(int idUsuario);
        Task CrearNotificacionAsync(int idUsuario, string titulo, string mensaje, int? idReferencia, string tipoReferencia);
    }
}