using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Core.Services;

public class AlertaService : IAlertaService
{
    private readonly IAlertaRepository _alertaRepository;

    public AlertaService(IAlertaRepository alertaRepository)
    {
        _alertaRepository = alertaRepository;
    }

    public async Task<AlertaRespuestaDto> CrearAlertaAsync(CrearAlertaDto dto, int idUsuario)
    {
        if (dto.IdDivisaOrigen == dto.IdDivisaDestino)
            throw new InvalidOperationException("La divisa de origen y destino deben ser diferentes.");

        var alerta = new AlertaTipoCambio
        {
            IdUsuario = idUsuario,
            IdDivisaOrigen = dto.IdDivisaOrigen,
            IdDivisaDestino = dto.IdDivisaDestino,
            ValorUmbral = dto.ValorUmbral,
            Activa = true,
            FechaCreacion = DateTime.Now
        };

        var creada = await _alertaRepository.CreateAsync(alerta);

        return MapToDto(creada);
    }

    public async Task<IEnumerable<AlertaRespuestaDto>> ObtenerPorUsuarioAsync(int idUsuario)
    {
        var alertas = await _alertaRepository.GetByUsuarioAsync(idUsuario);
        return alertas.Select(MapToDto);
    }

    public async Task<AlertaRespuestaDto> ActualizarAlertaAsync(int id, ActualizarAlertaDto dto, int idUsuario)
    {
        var alerta = await _alertaRepository.GetByIdAsync(id);
        if (alerta == null)
            throw new InvalidOperationException("La alerta no existe.");

        if (alerta.IdUsuario != idUsuario)
            throw new InvalidOperationException("No puedes modificar una alerta que no te pertenece.");

        alerta.ValorUmbral = dto.ValorUmbral;
        alerta.Activa = dto.Activa;

        await _alertaRepository.UpdateAsync(alerta);

        return MapToDto(alerta);
    }

    public async Task EliminarAlertaAsync(int id, int idUsuario)
    {
        var alerta = await _alertaRepository.GetByIdAsync(id);
        if (alerta == null)
            throw new InvalidOperationException("La alerta no existe.");

        if (alerta.IdUsuario != idUsuario)
            throw new InvalidOperationException("No puedes eliminar una alerta que no te pertenece.");

        await _alertaRepository.DeleteAsync(alerta);
    }

    private static AlertaRespuestaDto MapToDto(AlertaTipoCambio a)
    {
        return new AlertaRespuestaDto
        {
            IdAlerta = a.IdAlerta,
            DivisaOrigen = a.IdDivisaOrigenNavigation?.Codigo ?? "",
            DivisaDestino = a.IdDivisaDestinoNavigation?.Codigo ?? "",
            ValorUmbral = a.ValorUmbral,
            Activa = a.Activa,
            FechaCreacion = a.FechaCreacion,
            FechaDisparo = a.FechaDisparo
        };
    }
}
