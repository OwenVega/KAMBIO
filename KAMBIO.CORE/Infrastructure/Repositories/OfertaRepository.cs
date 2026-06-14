using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class OfertaRepository : IOfertaRepository
    {
        private readonly KambioDbContext _context;

        public OfertaRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<Oferta?> ObtenerPorIdAsync(int idOferta)
        {
            return await _context.Oferta.FindAsync(idOferta);
        }

        public async Task ActualizarAsync(Oferta oferta)
        {
            _context.Oferta.Update(oferta);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TieneTransaccionEnCursoAsync(int idOferta)
        {
            return await _context.Transaccion
                .AnyAsync(t => t.IdOferta == idOferta &&
                               t.IdEstadoTransaccion != 4 && // 4 = Completada
                               t.IdEstadoTransaccion != 5);  // 5 = Cancelada
        }
    }
}