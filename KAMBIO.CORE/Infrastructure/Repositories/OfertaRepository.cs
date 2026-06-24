using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class OfertaRepository : IOfertaRepository
    {
        private readonly KambioDbContext _context;

        public OfertaRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Oferta>> EncontrarOfertasCompatiblesAsync(Oferta ofertaActual)
        {
            var matches = await _context.Oferta
                .Include(o => o.IdUsuarioNavigation)
                .Where(o => o.IdEstadoOferta == 1
                         && o.IdOferta != ofertaActual.IdOferta
                         && o.IdUsuario != ofertaActual.IdUsuario
                         && o.IdDivisaOrigen == ofertaActual.IdDivisaDestino
                         && o.IdDivisaDestino == ofertaActual.IdDivisaOrigen
                         && o.MontoMinimo <= ofertaActual.MontoMaximo
                         && o.MontoMaximo >= ofertaActual.MontoMinimo)
                .OrderByDescending(o => o.IdUsuarioNavigation.CalificacionPromedio)
                .ThenByDescending(o => o.IdUsuarioNavigation.TotalOrdenes)
                .ToListAsync();

            return matches;
        }

        public async Task CrearMatchSugeridoAsync(MatchOferta nuevoMatch)
        {
            await _context.MatchOferta.AddAsync(nuevoMatch);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MatchOferta>> ObtenerMatchesSugeridosPorUsuarioIdAsync(int idUsuario)
        {
            return await _context.MatchOferta
                .Include(m => m.IdOfertaOrigenNavigation).ThenInclude(o => o.IdUsuarioNavigation)
                .Include(m => m.IdOfertaMatchNavigation).ThenInclude(o => o.IdUsuarioNavigation)
                .Where(m => (m.IdOfertaOrigenNavigation.IdUsuario == idUsuario ||
                             m.IdOfertaMatchNavigation.IdUsuario == idUsuario)
                         && m.Estado == "Pendiente")
                .ToListAsync();
        }

        public async Task ActualizarEstadoMatchAsync(int idMatch, int idUsuario, bool aceptado)
        {
            var match = await _context.MatchOferta.FirstOrDefaultAsync(m => m.IdMatch == idMatch);

            if (match != null)
            {
                if (aceptado)
                {
                    if (match.Estado == "Pendiente")
                    {
                        match.Estado = $"AceptadoPor_{idUsuario}";
                    }
                    else if (match.Estado.StartsWith("AceptadoPor_") && !match.Estado.EndsWith(idUsuario.ToString()))
                    {
                        match.Estado = "Aceptado";
                        match.FechaRespuesta = DateTime.Now;
                    }
                }
                else
                {
                    match.Estado = "Rechazado";
                    match.FechaRespuesta = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }
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

        // ===== Métodos para CrearOfertaAsync (US-022) =====

        public async Task<Oferta> CreateAsync(Oferta oferta)
        {
            _context.Oferta.Add(oferta);
            await _context.SaveChangesAsync();

            return await _context.Oferta
                .Include(o => o.IdUsuarioNavigation)
                .Include(o => o.IdDivisaOrigenNavigation)
                .Include(o => o.IdDivisaDestinoNavigation)
                .Include(o => o.IdTipoOfertaNavigation)
                .Include(o => o.IdEstadoOfertaNavigation)
                .Include(o => o.OfertaMetodoPago).ThenInclude(omp => omp.IdBancoNavigation)
                .FirstAsync(o => o.IdOferta == oferta.IdOferta);
        }

        public async Task<Banco?> GetBancoByIdAsync(int id)
        {
            return await _context.Banco.FindAsync(id);
        }
    }
}