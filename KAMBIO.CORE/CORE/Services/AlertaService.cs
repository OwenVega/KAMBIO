// SERVICIO de Alertas (lógica de negocio)
// Aquí están las reglas de negocio: validaciones, cálculos, etc.
// El servicio recibe los DTOs del controlador, valida, y llama al repositorio para guardar.
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Core.Services;

public class AlertaService : IAlertaService
{
    // Llamamos al repositorio para guardar/traer datos de la BD
    private readonly IAlertaRepository _alertaRepository;

    public AlertaService(IAlertaRepository alertaRepository)
    {
        _alertaRepository = alertaRepository;
    }

    // Crear una alerta: validamos que las divisas sean diferentes y guardamos
    public async Task<AlertaRespuestaDto> CrearAlertaAsync(CrearAlertaDto dto, int idUsuario)
    {
        if (dto.IdDivisaOrigen == dto.IdDivisaDestino)
            throw new InvalidOperationException("La divisa de origen y destino deben ser diferentes.");

        // Creamos el objeto "AlertaTipoCambio" que se va a guardar en la BD
        var alerta = new AlertaTipoCambio
        {
            IdUsuario = idUsuario,              // Dueño de la alerta
            IdDivisaOrigen = dto.IdDivisaOrigen, // ej: USD
            IdDivisaDestino = dto.IdDivisaDestino, // ej: PEN
            ValorUmbral = dto.ValorUmbral,      // ej: 4.50
            Activa = true,                      // Por defecto activa
            FechaCreacion = DateTime.Now        // Fecha actual
        };

        var creada = await _alertaRepository.CreateAsync(alerta);

        // Convertimos el resultado de BD a DTO (para enviar al frontend)
        return MapToDto(creada);
    }

    // Traer todas las alertas de un usuario
    public async Task<IEnumerable<AlertaRespuestaDto>> ObtenerPorUsuarioAsync(int idUsuario)
    {
        var alertas = await _alertaRepository.GetByUsuarioAsync(idUsuario);
        return alertas.Select(MapToDto);
    }

    // Actualizar alerta: verificamos que exista y que sea del usuario dueño
    public async Task<AlertaRespuestaDto> ActualizarAlertaAsync(int id, ActualizarAlertaDto dto, int idUsuario)
    {
        var alerta = await _alertaRepository.GetByIdAsync(id);
        if (alerta == null)
            throw new InvalidOperationException("La alerta no existe.");

        // Solo el dueño puede modificar su alerta
        if (alerta.IdUsuario != idUsuario)
            throw new InvalidOperationException("No puedes modificar una alerta que no te pertenece.");

        alerta.ValorUmbral = dto.ValorUmbral;
        alerta.Activa = dto.Activa;

        await _alertaRepository.UpdateAsync(alerta);

        return MapToDto(alerta);
    }

    // Eliminar alerta: verificamos que exista y que sea del usuario dueño
    public async Task EliminarAlertaAsync(int id, int idUsuario)
    {
        var alerta = await _alertaRepository.GetByIdAsync(id);
        if (alerta == null)
            throw new InvalidOperationException("La alerta no existe.");

        if (alerta.IdUsuario != idUsuario)
            throw new InvalidOperationException("No puedes eliminar una alerta que no te pertenece.");

        await _alertaRepository.DeleteAsync(alerta);
    }

    // MÉTODO AUXILIAR: convierte un objeto de BD (AlertaTipoCambio) a DTO (AlertaRespuestaDto)
    private static AlertaRespuestaDto MapToDto(AlertaTipoCambio a)
    {
        return new AlertaRespuestaDto
        {
            IdAlerta = a.IdAlerta,
            DivisaOrigen = a.IdDivisaOrigenNavigation?.Codigo ?? "",  // "USD"
            DivisaDestino = a.IdDivisaDestinoNavigation?.Codigo ?? "", // "PEN"
            ValorUmbral = a.ValorUmbral,
            Activa = a.Activa,
            FechaCreacion = a.FechaCreacion,
            FechaDisparo = a.FechaDisparo
        };
    }
}
