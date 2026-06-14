using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.CORE.Services
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

        public async Task CancelarOfertaAsync(int idOferta, int idUsuario)
        {
            var oferta = await _ofertaRepository.ObtenerPorIdAsync(idOferta);
            if (oferta == null)
                throw new InvalidOperationException("Oferta no encontrada.");
            if (oferta.IdUsuario != idUsuario)
                throw new InvalidOperationException("No tienes permiso para cancelar esta oferta.");
            if (oferta.IdEstadoOferta != 1)
                throw new InvalidOperationException("Solo se pueden cancelar ofertas en estado Activa.");

            var tieneTransaccion = await _ofertaRepository.TieneTransaccionEnCursoAsync(idOferta);
            if (tieneTransaccion)
                throw new InvalidOperationException("No se puede cancelar una oferta con una transaccion en curso.");

            oferta.IdEstadoOferta = 2;
            oferta.FechaCancelacion = DateTime.UtcNow;
            await _ofertaRepository.ActualizarAsync(oferta);
        }
    }
}