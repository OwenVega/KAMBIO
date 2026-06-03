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
