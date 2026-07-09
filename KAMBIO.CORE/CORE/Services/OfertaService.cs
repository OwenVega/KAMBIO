using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
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

        public async Task<OfertaResponseDTO> CrearOfertaCompra(int idUsuario, CrearOfertaCompraRequestDTO dto)
        {
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

            var creada = await _ofertaRepository.CrearOfertaCompra(oferta, dto.MetodosPago);
            await EjecutarMatchingAutomaticoAsync(creada);
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

        public async Task<List<OfertaP2PDTO>> ObtenerOfertasMercadoAsync(FiltroOfertaDTO filtro)
        {
            var ofertas = await _ofertaRepository.ObtenerOfertasFiltradasAsync(
                filtro.IdTipoOferta,
                filtro.IdDivisaOrigen,
                filtro.IdDivisaDestino,
                filtro.Monto,
                filtro.IdBanco
            );

            return ofertas.Select(o => new OfertaP2PDTO
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

        public async Task<OfertaRespuestaDto> CrearOfertaAsync(CrearOfertaDto dto, int idUsuario)
        {
            if (dto.MontoMinimo > dto.MontoMaximo)
                throw new InvalidOperationException("El monto mínimo no puede ser mayor al monto máximo.");

            if (dto.MontoDisponible < dto.MontoMaximo)
                throw new InvalidOperationException("El monto disponible debe ser mayor o igual al monto máximo.");

            if (dto.IdDivisaOrigen == dto.IdDivisaDestino)
                throw new InvalidOperationException("La divisa de origen y destino deben ser diferentes.");

            var oferta = new Oferta
            {
                IdUsuario = idUsuario,
                IdTipoOferta = dto.IdTipoOferta,
                IdEstadoOferta = 1,
                IdDivisaOrigen = dto.IdDivisaOrigen,
                IdDivisaDestino = dto.IdDivisaDestino,
                MontoDisponible = dto.MontoDisponible,
                MontoMinimo = dto.MontoMinimo,
                MontoMaximo = dto.MontoMaximo,
                TasaCambio = dto.TasaCambio,
                FechaPublicacion = DateTime.Now
            };

            foreach (var idBanco in dto.IdsBancos)
            {
                var banco = await _ofertaRepository.GetBancoByIdAsync(idBanco);
                if (banco == null)
                    throw new InvalidOperationException($"El banco con ID {idBanco} no existe.");

                oferta.OfertaMetodoPago.Add(new OfertaMetodoPago
                {
                    IdBanco = idBanco,
                    IdOfertaNavigation = oferta
                });
            }

            var ofertaCreada = await _ofertaRepository.CreateAsync(oferta);

            return new OfertaRespuestaDto
            {
                IdOferta = ofertaCreada.IdOferta,
                NombresAnunciante = ofertaCreada.IdUsuarioNavigation.Nombres,
                ApellidosAnunciante = ofertaCreada.IdUsuarioNavigation.Apellidos,
                DivisaOrigen = ofertaCreada.IdDivisaOrigenNavigation.Codigo,
                DivisaDestino = ofertaCreada.IdDivisaDestinoNavigation.Codigo,
                MontoDisponible = ofertaCreada.MontoDisponible,
                MontoMinimo = ofertaCreada.MontoMinimo,
                MontoMaximo = ofertaCreada.MontoMaximo,
                TasaCambio = ofertaCreada.TasaCambio,
                TipoOferta = ofertaCreada.IdTipoOfertaNavigation.Nombre,
                Estado = ofertaCreada.IdEstadoOfertaNavigation.Nombre,
                Bancos = ofertaCreada.OfertaMetodoPago.Select(omp => omp.IdBancoNavigation.Nombre).ToList(),
                FechaPublicacion = ofertaCreada.FechaPublicacion
            };
        }
    }
}