using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<Oferta> CrearOfertaCompra(Oferta oferta, List<int> idBancos)
        {
            await _context.Oferta.AddAsync(oferta);
            await _context.SaveChangesAsync();

            foreach (var idBanco in idBancos)
            {
                var metodo = new OfertaMetodoPago
                {
                    IdOferta = oferta.IdOferta,
                    IdBanco = idBanco
                };
                await _context.OfertaMetodoPago.AddAsync(metodo);
            }
            await _context.SaveChangesAsync();

            return await _context.Oferta
                .Include(o => o.IdDivisaOrigenNavigation)
                .Include(o => o.IdDivisaDestinoNavigation)
                .Include(o => o.IdEstadoOfertaNavigation)
                .Include(o => o.IdTipoOfertaNavigation)
                .Include(o => o.OfertaMetodoPago)
                    .ThenInclude(m => m.IdBancoNavigation)
                .FirstAsync(o => o.IdOferta == oferta.IdOferta);
        }

        public async Task<List<Oferta>> ObtenerOfertasActivas()
        {
            return await _context.Oferta
                .Where(o => o.IdEstadoOferta == 1)
                .Include(o => o.IdDivisaOrigenNavigation)
                .Include(o => o.IdDivisaDestinoNavigation)
                .Include(o => o.IdEstadoOfertaNavigation)
                .Include(o => o.IdTipoOfertaNavigation)
                .Include(o => o.OfertaMetodoPago)
                    .ThenInclude(m => m.IdBancoNavigation)
                .OrderByDescending(o => o.FechaPublicacion)
                .ToListAsync();
        }

        public async Task<List<Oferta>> ObtenerOfertasFiltradasAsync(int idTipoOferta, int idDivisaOrigen, int idDivisaDestino, decimal? monto, int? idBanco)
        {
            var query = _context.Oferta
                .Include(o => o.IdUsuarioNavigation)
                .Include(o => o.OfertaMetodoPago)
                    .ThenInclude(om => om.IdBancoNavigation)
                .Where(o => o.IdEstadoOferta == 1
                         && o.IdTipoOferta == idTipoOferta
                         && o.IdDivisaOrigen == idDivisaOrigen
                         && o.IdDivisaDestino == idDivisaDestino)
                .AsQueryable();

            if (monto.HasValue && monto.Value > 0)
                query = query.Where(o => monto.Value >= o.MontoMinimo && monto.Value <= o.MontoMaximo);

            if (idBanco.HasValue && idBanco.Value > 0)
                query = query.Where(o => o.OfertaMetodoPago.Any(om => om.IdBanco == idBanco.Value));

            return await query.AsNoTracking().ToListAsync();
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
                               t.IdEstadoTransaccion != 4 &&
                               t.IdEstadoTransaccion != 5);
        }
    }
}