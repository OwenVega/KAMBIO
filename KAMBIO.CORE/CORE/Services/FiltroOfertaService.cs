using System.Linq;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Repositories;

namespace KAMBIO.CORE.Core.Services
{
    public class FiltroOfertaService :IFiltroOfertaService
    {
        private readonly IFiltroOfertaRepository _filtroRepository;

        public FiltroOfertaService(IFiltroOfertaRepository filtroRepository)
        {
            _filtroRepository = filtroRepository;
        }

        public async Task<FiltroOfertaResponseDto> ObtenerOfertasFiltradasAsync(FiltroOfertaRequestDto filtro)
        {
            var ofertas = await _filtroRepository.FiltrarOfertasAsync(filtro);

            var response = new FiltroOfertaResponseDto
            {
                // Contador de resultados solicitados en la US-020
                TotalResultados = ofertas.Count,
                Ofertas = ofertas.Select(o => new OfertaFiltradaDto
                {
                    IdOferta = o.IdOferta,
                    TipoOperacion = o.IdTipoOfertaNavigation.Nombre,
                    Anunciante = $"{o.IdUsuarioNavigation.Nombres} {o.IdUsuarioNavigation.Apellidos}",
                    Reputacion = o.IdUsuarioNavigation.CalificacionPromedio,
                    MonedaOrigen = o.IdDivisaOrigenNavigation.Codigo,
                    MonedaDestino = o.IdDivisaDestinoNavigation.Codigo,
                    TasaCambio = o.TasaCambio,
                    MontoDisponible = o.MontoDisponible,
                    MontoMinimo = o.MontoMinimo,
                    MontoMaximo = o.MontoMaximo,
                    BancosAceptados = o.OfertaMetodoPago.Select(omp => omp.IdBancoNavigation.Nombre).ToList(),
                    FechaPublicacion = o.FechaPublicacion
                }).ToList()
            };

            return response;
        }
    }
}