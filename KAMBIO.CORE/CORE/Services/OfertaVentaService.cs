using KAMBIO.CORE.Core.DTOs.OfertaVenta;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Core.Services
{
    public class OfertaVentaService : IOfertaVentaService
    {
        private readonly IOfertaVentaRepository _repository;

        public OfertaVentaService(IOfertaVentaRepository repository)
        {
            _repository = repository;
        }

        public async Task<OfertaVentaResponseDTO> CrearOfertaVenta(
            CrearOfertaVentaRequestDTO request)
        {
            if (request.MontoDisponible <= 0)
                throw new Exception("El monto disponible debe ser mayor a cero.");

            if (request.TasaCambio <= 0)
                throw new Exception("El tipo de cambio debe ser mayor a cero.");

            if (request.IdBancos == null || !request.IdBancos.Any())
                throw new Exception("Debe seleccionar al menos un método de pago.");

            var oferta = new Oferta
            {
                IdUsuario = request.IdUsuario,
                IdTipoOferta = 2,
                IdEstadoOferta = 1,
                IdDivisaOrigen = request.IdDivisaOrigen,
                IdDivisaDestino = request.IdDivisaDestino,
                MontoDisponible = request.MontoDisponible,
                MontoMinimo = request.MontoDisponible,
                MontoMaximo = request.MontoDisponible,
                TasaCambio = request.TasaCambio,
                FechaPublicacion = DateTime.Now
            };

            var ofertaCreada = await _repository.CrearOfertaVenta(
                oferta,
                request.IdBancos);

            return new OfertaVentaResponseDTO
            {
                IdOferta = ofertaCreada.IdOferta,
                DivisaOrigen = ofertaCreada.IdDivisaOrigenNavigation.Nombre,
                DivisaDestino = ofertaCreada.IdDivisaDestinoNavigation.Nombre,
                MontoDisponible = ofertaCreada.MontoDisponible,
                TasaCambio = ofertaCreada.TasaCambio,
                Estado = ofertaCreada.IdEstadoOfertaNavigation.Nombre,
                FechaPublicacion = ofertaCreada.FechaPublicacion,
                MetodosPago = ofertaCreada.OfertaMetodoPago
                    .Select(x => x.IdBancoNavigation.Nombre)
                    .ToList()
            };
        }
    }
}