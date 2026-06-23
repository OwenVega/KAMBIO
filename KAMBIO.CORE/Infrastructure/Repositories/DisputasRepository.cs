using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;


namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class DisputaRepository : IDisputaRepository
    {
        private readonly KambioDbContext _context;

        public DisputaRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<List<Disputa>> ObtenerDisputas()
        {
            return await _context.Disputa
                .Include(d => d.IdEstadoDisputaNavigation)
                .Include(d => d.IdUsuarioReportaNavigation)
                .Include(d => d.IdTransaccionNavigation)
                .ToListAsync();
        }

        public async Task<Disputa?> ObtenerDisputaPorId(int id)
        {
            return await _context.Disputa
                .Include(d => d.IdEstadoDisputaNavigation)
                .Include(d => d.IdUsuarioReportaNavigation)
                .Include(d => d.IdTransaccionNavigation)
                .FirstOrDefaultAsync(d => d.IdDisputa == id);
        }

        public async Task ActualizarDisputa()
        {
            await _context.SaveChangesAsync();
        }
    }
}