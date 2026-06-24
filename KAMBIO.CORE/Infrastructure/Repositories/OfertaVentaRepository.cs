using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;


namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class OfertaVentaRepository : IOfertaVentaRepository
    {
        private readonly KambioDbContext _context;

        public OfertaVentaRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<Oferta> CrearOfertaVenta(Oferta oferta, List<int> idBancos)
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
    }
}