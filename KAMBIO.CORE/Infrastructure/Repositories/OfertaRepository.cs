using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}