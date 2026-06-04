using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Repositories;

namespace KAMBIO.CORE.Core.Services
{
    public class OfertaService : IOfertaService
    {
        private readonly IOfertaRepository _ofertaRepository;

        public OfertaService(IOfertaRepository ofertaRepository)
        {
            _ofertaRepository = ofertaRepository;
        }

        public async Task<List<OfertaP2PDTO>> ObtenerOfertasMercadoAsync(FiltroOfertaDTO filtro)
        {
            var ofertas = await _ofertaRepository.ObtenerOfertasFiltradasAsync(
                filtro.IdTipoOferta,
                filtro.IdDivisaOrigen,
                filtro.IdDivisaDestino,
                filtro.Monto,
                filtro.IdBanco
            );

            var resultado = ofertas.Select(o => new OfertaP2PDTO
            {
                IdOferta = o.IdOferta,
                AnuncianteNombre = $"{o.IdUsuarioNavigation.Nombres} {o.IdUsuarioNavigation.Apellidos}",
                PorcentajeReputacion = 100m,
                OrdenesCompletadas = 15,
                TasaCambio = o.TasaCambio,
                MontoDisponible = o.MontoDisponible,
                LimiteMinimo = o.MontoMinimo,
                LimiteMaximo = o.MontoMaximo,
                MetodosPago = o.OfertaMetodoPago
                    .Select(om => om.IdBancoNavigation.Nombre)
                    .ToList()
            }).ToList();

            return resultado;
        }
    }
}