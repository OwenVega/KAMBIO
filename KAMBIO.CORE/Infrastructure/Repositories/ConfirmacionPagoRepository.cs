using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;

using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class ConfirmacionPagoRepository : IConfirmacionPagoRepository
    {
        private readonly KambioDbContext _context;

        public ConfirmacionPagoRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<Transaccion?> ObtenerTransaccion(int idTransaccion)
        {
            return await _context.Transaccion
                .Include(t => t.IdEstadoTransaccionNavigation)
                .FirstOrDefaultAsync(t => t.IdTransaccion == idTransaccion);
        }

        public async Task ActualizarEstadoPago(Transaccion transaccion)
        {
            _context.Transaccion.Update(transaccion);
            await _context.SaveChangesAsync();
        }

        public async Task RegistrarHistorial(
            HistorialEstadoTransaccion historial)
        {
            await _context.HistorialEstadoTransaccion.AddAsync(historial);
            await _context.SaveChangesAsync();
        }
    }
}