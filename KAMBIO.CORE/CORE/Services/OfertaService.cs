// SERVICIO de Ofertas (lógica de negocio)
// Reglas: Monto mínimo ≤ Máximo, Monto disponible ≥ Máximo, divisas distintas, bancos válidos.
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Core.Services;

public class OfertaService : IOfertaService
{
    private readonly IOfertaRepository _ofertaRepository;

    public OfertaService(IOfertaRepository ofertaRepository)
    {
        _ofertaRepository = ofertaRepository;
    }

    // PUBLICAR una oferta (con todas las validaciones de la US-021)
    public async Task<OfertaRespuestaDto> CrearOfertaAsync(CrearOfertaDto dto, int idUsuario)
    {
        // VALIDACIÓN 1: El monto mínimo no puede ser mayor al máximo
        if (dto.MontoMinimo > dto.MontoMaximo)
            throw new InvalidOperationException("El monto mínimo no puede ser mayor al monto máximo.");

        // VALIDACIÓN 2: El disponible debe ser >= al máximo (no puedes ofrecer más de lo que tienes)
        if (dto.MontoDisponible < dto.MontoMaximo)
            throw new InvalidOperationException("El monto disponible debe ser mayor o igual al monto máximo.");

        // VALIDACIÓN 3: Las divisas deben ser diferentes (no puedes cambiar USD por USD)
        if (dto.IdDivisaOrigen == dto.IdDivisaDestino)
            throw new InvalidOperationException("La divisa de origen y destino deben ser diferentes.");

        // Creamos la oferta con Estado = 1 (Activa)
        var oferta = new Oferta
        {
            IdUsuario = idUsuario,
            IdTipoOferta = dto.IdTipoOferta,     // Compra o Venta
            IdEstadoOferta = 1,                   // Activa
            IdDivisaOrigen = dto.IdDivisaOrigen,  // USD
            IdDivisaDestino = dto.IdDivisaDestino, // PEN
            MontoDisponible = dto.MontoDisponible,
            MontoMinimo = dto.MontoMinimo,        // ← CAMPO NUEVO US-021
            MontoMaximo = dto.MontoMaximo,        // ← CAMPO NUEVO US-021
            TasaCambio = dto.TasaCambio,
            FechaPublicacion = DateTime.Now
        };

        // Validar que los bancos seleccionados existan y agregarlos a la oferta
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

        // Devolver la respuesta con todos los datos
        return new OfertaRespuestaDto
        {
            IdOferta = ofertaCreada.IdOferta,
            NombresAnunciante = ofertaCreada.IdUsuarioNavigation.Nombres,
            ApellidosAnunciante = ofertaCreada.IdUsuarioNavigation.Apellidos,
            DivisaOrigen = ofertaCreada.IdDivisaOrigenNavigation.Codigo,
            DivisaDestino = ofertaCreada.IdDivisaDestinoNavigation.Codigo,
            MontoDisponible = ofertaCreada.MontoDisponible,
            MontoMinimo = ofertaCreada.MontoMinimo,   // ← SE MUESTRA EL RANGO
            MontoMaximo = ofertaCreada.MontoMaximo,   // ← SE MUESTRA EL RANGO
            TasaCambio = ofertaCreada.TasaCambio,
            TipoOferta = ofertaCreada.IdTipoOfertaNavigation.Nombre,
            Estado = ofertaCreada.IdEstadoOfertaNavigation.Nombre,
            Bancos = ofertaCreada.OfertaMetodoPago.Select(omp => omp.IdBancoNavigation.Nombre).ToList(),
            FechaPublicacion = ofertaCreada.FechaPublicacion
        };
    }
}
