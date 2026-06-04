using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Data;

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
            // 1. Guardar la oferta
            await _context.Oferta.AddAsync(oferta);
            await _context.SaveChangesAsync();

            // 2. Guardar métodos de pago asociados
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

            // 3. Retornar con navegaciones cargadas
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
                .Where(o => o.IdEstadoOferta == 1) // 1 = Activa
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