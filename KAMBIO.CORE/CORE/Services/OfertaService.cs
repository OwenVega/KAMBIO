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

        public async Task<OfertaResponseDTO> CrearOfertaCompra(int idUsuario, CrearOfertaCompraRequestDTO dto)
        {
            // Paso 1: Validaciones de negocio
            if (dto.IdDivisaOrigen <= 0)
                throw new ArgumentException("Debe indicar la divisa que desea adquirir.");

            if (dto.IdDivisaDestino <= 0)
                throw new ArgumentException("Debe indicar la divisa con la que pagará.");

            if (dto.MontoDisponible <= 0)
                throw new ArgumentException("El monto debe ser un valor numérico mayor a cero.");

            if (dto.TasaCambio <= 0)
                throw new ArgumentException("El tipo de cambio debe ser un valor numérico mayor a cero.");

            if (dto.MontoMinimo <= 0 || dto.MontoMaximo <= 0)
                throw new ArgumentException("Los montos mínimo y máximo deben ser mayores a cero.");

            if (dto.MontoMinimo > dto.MontoMaximo)
                throw new ArgumentException("El monto mínimo no puede ser mayor al monto máximo.");

            if (dto.MetodosPago == null || dto.MetodosPago.Count == 0)
                throw new ArgumentException("Debe seleccionar al menos un método de pago.");

            // Paso 2: Mapeo - Convertir DTO en Entidad
            // IdTipoOferta   = 1 → Compra  (automático)
            // IdEstadoOferta = 1 → Activa  (automático)
            // FechaPublicacion              (automático)
            var oferta = new Oferta
            {
                IdUsuario = idUsuario,
                IdTipoOferta = 1,
                IdEstadoOferta = 1,
                IdDivisaOrigen = dto.IdDivisaOrigen,
                IdDivisaDestino = dto.IdDivisaDestino,
                MontoDisponible = dto.MontoDisponible,
                MontoMinimo = dto.MontoMinimo,
                MontoMaximo = dto.MontoMaximo,
                TasaCambio = dto.TasaCambio,
                FechaPublicacion = DateTime.Now
            };

            // Paso 3: Persistencia
            var creada = await _ofertaRepository.CrearOfertaCompra(oferta, dto.MetodosPago);

            // Paso 4: Mapear a Response DTO
            return new OfertaResponseDTO
            {
                Mensaje = "Oferta de compra publicada exitosamente.",
                IdOferta = creada.IdOferta,
                IdUsuario = creada.IdUsuario,
                TipoOferta = creada.IdTipoOfertaNavigation?.Nombre ?? "Compra",
                Estado = creada.IdEstadoOfertaNavigation?.Nombre ?? "Activa",
                DivisaOrigen = creada.IdDivisaOrigenNavigation?.Codigo ?? dto.IdDivisaOrigen.ToString(),
                DivisaDestino = creada.IdDivisaDestinoNavigation?.Codigo ?? dto.IdDivisaDestino.ToString(),
                MontoDisponible = creada.MontoDisponible,
                MontoMinimo = creada.MontoMinimo,
                MontoMaximo = creada.MontoMaximo,
                TasaCambio = creada.TasaCambio,
                MetodosPago = creada.OfertaMetodoPago
                                        .Select(m => m.IdBancoNavigation?.Nombre ?? m.IdBanco.ToString())
                                        .ToList(),
                FechaPublicacion = creada.FechaPublicacion
            };
        }

        public async Task<List<OfertaResponseDTO>> ObtenerOfertasActivas()
        {
            var lista = await _ofertaRepository.ObtenerOfertasActivas();

            return lista.Select(o => new OfertaResponseDTO
            {
                Mensaje = string.Empty,
                IdOferta = o.IdOferta,
                IdUsuario = o.IdUsuario,
                TipoOferta = o.IdTipoOfertaNavigation?.Nombre ?? "",
                Estado = o.IdEstadoOfertaNavigation?.Nombre ?? "",
                DivisaOrigen = o.IdDivisaOrigenNavigation?.Codigo ?? "",
                DivisaDestino = o.IdDivisaDestinoNavigation?.Codigo ?? "",
                MontoDisponible = o.MontoDisponible,
                MontoMinimo = o.MontoMinimo,
                MontoMaximo = o.MontoMaximo,
                TasaCambio = o.TasaCambio,
                MetodosPago = o.OfertaMetodoPago
                                        .Select(m => m.IdBancoNavigation?.Nombre ?? m.IdBanco.ToString())
                                        .ToList(),
                FechaPublicacion = o.FechaPublicacion
            }).ToList();
        }
    }
}