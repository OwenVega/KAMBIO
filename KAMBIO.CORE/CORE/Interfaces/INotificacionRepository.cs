using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface INotificacionRepository
    {
        Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(int idUsuario);
        Task<Notificacion?> ObtenerPorIdAsync(int idNotificacion);
        Task MarcarComoLeidaAsync(int idNotificacion);
        Task MarcarTodasComoLeidasAsync(int idUsuario);
        Task<int> ContarNoLeidasAsync(int idUsuario);
        Task CrearNotificacionAsync(Notificacion notificacion);
    }
}