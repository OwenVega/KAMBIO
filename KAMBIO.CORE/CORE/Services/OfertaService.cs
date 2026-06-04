using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Core.Services
{
    public class OfertaService : IOfertaService
    {
        private readonly IOfertaRepository _ofertaRepository;

        public OfertaService(IOfertaRepository ofertaRepository)
        {
            _ofertaRepository = ofertaRepository;
        }

        
        public async Task EjecutarMatchingAutomaticoAsync(Oferta nuevaOferta)
        {
            var ofertasCompatibles = await _ofertaRepository.EncontrarOfertasCompatiblesAsync(nuevaOferta);

            foreach (var contraparte in ofertasCompatibles)
            {
                var nuevoMatch = new MatchOferta
                {
                    IdOfertaOrigen = nuevaOferta.IdOferta,
                    IdOfertaMatch = contraparte.IdOferta,
                    Estado = "Pendiente",
                    FechaMatch = DateTime.Now
                };

                await _ofertaRepository.CrearMatchSugeridoAsync(nuevoMatch);

                
            }
        }

       
        public async Task<IEnumerable<MatchSugeridoDto>> ObtenerMatchesSugeridosAsync(int idUsuario)
        {
            var matchesEntidad = await _ofertaRepository.ObtenerMatchesSugeridosPorUsuarioIdAsync(idUsuario);
            var listaMatchesDto = new List<MatchSugeridoDto>();

            foreach (var match in matchesEntidad)
            {
                
                var ofertaContraparte = match.IdOfertaOrigenNavigation.IdUsuario != idUsuario
                    ? match.IdOfertaOrigenNavigation
                    : match.IdOfertaMatchNavigation;

                var usuarioContraparte = ofertaContraparte.IdUsuarioNavigation;

                var dto = new MatchSugeridoDto
                {
                    IdMatch = match.IdMatch,
                    IdOfertaContraparte = ofertaContraparte.IdOferta,
                    Anunciante = $"{usuarioContraparte.Nombres} {usuarioContraparte.Apellidos}",
                    Reputacion = usuarioContraparte.CalificacionPromedio,
                    TotalOperaciones = usuarioContraparte.TotalOrdenes,
                    TasaCambio = ofertaContraparte.TasaCambio,
                    MontoDisponible = ofertaContraparte.MontoDisponible,
                    FechaPublicacionOferta = ofertaContraparte.FechaPublicacion,

                    
                    MetodosPagoAceptados = ofertaContraparte.OfertaMetodoPago
                        .Select(omp => omp.IdBancoNavigation.Nombre)
                        .ToList()
                };

                listaMatchesDto.Add(dto);
            }

            return listaMatchesDto;
        }

       
        public async Task ProcesarRespuestaMatchAsync(RespuestaMatchDto respuesta)
        {
            await _ofertaRepository.ActualizarEstadoMatchAsync(respuesta.IdMatch, respuesta.IdUsuario, respuesta.Aceptado);

            
        }
    }
}