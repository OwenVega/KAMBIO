using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Data;
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class FiltroOfertaRepository : IFiltroOfertaRepository
    {
        private readonly KambioDbContext _context;

        public FiltroOfertaRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<List<Oferta>> FiltrarOfertasAsync(FiltroOfertaRequestDto filtro)
        {
            // Iniciamos la consulta base filtrando SOLO ofertas activas (Criterio US-020)
            var query = _context.Oferta
                .Include(o => o.IdUsuarioNavigation)
                .Include(o => o.IdTipoOfertaNavigation)
                .Include(o => o.IdDivisaOrigenNavigation)
                .Include(o => o.IdDivisaDestinoNavigation)
                .Include(o => o.OfertaMetodoPago)
                    .ThenInclude(omp => omp.IdBancoNavigation)
                .Where(o => o.IdEstadoOferta == 1)
                .AsQueryable();

            // Construcción dinámica de la consulta mediante IQueryable
            if (filtro.IdTipoOferta.HasValue)
                query = query.Where(o => o.IdTipoOferta == filtro.IdTipoOferta.Value);

            if (filtro.IdDivisaOrigen.HasValue)
                query = query.Where(o => o.IdDivisaOrigen == filtro.IdDivisaOrigen.Value);

            if (filtro.IdDivisaDestino.HasValue)
                query = query.Where(o => o.IdDivisaDestino == filtro.IdDivisaDestino.Value);

            // Criterio: Si selecciona banco, buscar dentro de sus métodos de pago
            if (filtro.IdBanco.HasValue)
                query = query.Where(o => o.OfertaMetodoPago.Any(omp => omp.IdBanco == filtro.IdBanco.Value));

            // Criterio: Rango de monto (la oferta debe cubrir el monto requerido)
            if (filtro.MontoRequerido.HasValue)
                query = query.Where(o => filtro.MontoRequerido.Value >= o.MontoMinimo &&
                                         filtro.MontoRequerido.Value <= o.MontoMaximo);

            // Criterio: Rango de reputación
            if (filtro.ReputacionMinima.HasValue)
                query = query.Where(o => o.IdUsuarioNavigation.CalificacionPromedio >= filtro.ReputacionMinima.Value);

            if (filtro.ReputacionMaxima.HasValue)
                query = query.Where(o => o.IdUsuarioNavigation.CalificacionPromedio <= filtro.ReputacionMaxima.Value);

            // Se ordena por defecto mostrando las mejores reputaciones primero
            return await query.OrderByDescending(o => o.IdUsuarioNavigation.CalificacionPromedio).ToListAsync();
        }
    }
}