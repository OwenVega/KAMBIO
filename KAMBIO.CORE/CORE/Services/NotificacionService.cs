using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.CORE.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _notificacionRepository;

        public NotificacionService(INotificacionRepository notificacionRepository)
        {
            _notificacionRepository = notificacionRepository;
        }

        public async Task<IEnumerable<NotificacionDto>> ObtenerNotificacionesAsync(int idUsuario)
        {
            var notificaciones = await _notificacionRepository.ObtenerPorUsuarioAsync(idUsuario);
            return notificaciones.Select(n => new NotificacionDto
            {
                IdNotificacion = n.IdNotificacion,
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                Leida = n.Leida,
                FechaCreacion = n.FechaCreacion,
                IdReferencia = n.IdReferencia,
                TipoReferencia = n.TipoReferencia
            });
        }

        public async Task MarcarComoLeidaAsync(int idNotificacion)
        {
            var notificacion = await _notificacionRepository.ObtenerPorIdAsync(idNotificacion);
            if (notificacion == null)
                throw new InvalidOperationException("Notificacion no encontrada.");
            await _notificacionRepository.MarcarComoLeidaAsync(idNotificacion);
        }

        public async Task MarcarTodasComoLeidasAsync(int idUsuario)
        {
            await _notificacionRepository.MarcarTodasComoLeidasAsync(idUsuario);
        }

        public async Task<int> ContarNoLeidasAsync(int idUsuario)
        {
            return await _notificacionRepository.ContarNoLeidasAsync(idUsuario);
        }
    }
}
