using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly KambioDbContext _context;

        public NotificacionRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notificacion>> ObtenerPorUsuarioAsync(int idUsuario)
        {
            return await _context.Notificacion
                .Where(n => n.IdUsuario == idUsuario)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Notificacion?> ObtenerPorIdAsync(int idNotificacion)
        {
            return await _context.Notificacion.FindAsync(idNotificacion);
        }

        public async Task MarcarComoLeidaAsync(int idNotificacion)
        {
            var notificacion = await _context.Notificacion.FindAsync(idNotificacion);
            if (notificacion != null)
            {
                notificacion.Leida = true;
                notificacion.FechaLectura = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarcarTodasComoLeidasAsync(int idUsuario)
        {
            var notificaciones = await _context.Notificacion
                .Where(n => n.IdUsuario == idUsuario && !n.Leida)
                .ToListAsync();

            foreach (var n in notificaciones)
            {
                n.Leida = true;
                n.FechaLectura = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> ContarNoLeidasAsync(int idUsuario)
        {
            return await _context.Notificacion
                .CountAsync(n => n.IdUsuario == idUsuario && !n.Leida);
        }

        public async Task CrearNotificacionAsync(Notificacion notificacion)
        {
            _context.Notificacion.Add(notificacion);
            await _context.SaveChangesAsync();
        }
    }
}