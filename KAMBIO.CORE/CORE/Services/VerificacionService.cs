using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Core.Services;

public class VerificacionService : IVerificacionService
{
    private readonly IVerificacionRepository _verificacionRepository;

    public VerificacionService(IVerificacionRepository verificacionRepository)
    {
        _verificacionRepository = verificacionRepository;
    }

    public async Task<VerificacionRespuestaDto> SolicitarVerificacionAsync(SolicitarVerificacionDto dto, int idUsuario)
    {
        var existentes = await _verificacionRepository.GetByUsuarioAsync(idUsuario);
        if (existentes.Any(v => v.IdEstadoVerificacion == 1))
            throw new InvalidOperationException("Ya tienes una solicitud de verificación pendiente.");

        if (existentes.Any(v => v.IdEstadoVerificacion == 2))
            throw new InvalidOperationException("Tu cuenta ya está verificada.");

        var verificacion = new VerificacionIdentidad
        {
            IdUsuario = idUsuario,
            IdEstadoVerificacion = 1,
            RutaImagen = dto.RutaImagen,
            FechaSolicitud = DateTime.Now
        };

        var creada = await _verificacionRepository.CreateAsync(verificacion);

        return MapToDto(creada);
    }

    public async Task<VerificacionRespuestaDto> RevisarVerificacionAsync(RevisarVerificacionDto dto, int idAdmin)
    {
        var verificacion = await _verificacionRepository.GetByIdAsync(dto.IdVerificacion);
        if (verificacion == null)
            throw new InvalidOperationException("La solicitud de verificación no existe.");

        verificacion.IdEstadoVerificacion = dto.IdEstadoVerificacion;
        verificacion.IdAdminResolucion = idAdmin;
        verificacion.FechaResolucion = DateTime.Now;
        verificacion.ObservacionAdmin = dto.ObservacionAdmin;

        if (dto.IdEstadoVerificacion == 2)
        {
            verificacion.IdUsuarioNavigation.EsVerificado = true;
        }

        await _verificacionRepository.UpdateAsync(verificacion);

        return MapToDto(verificacion);
    }

    public async Task<IEnumerable<VerificacionRespuestaDto>> ObtenerPendientesAsync()
    {
        var pendientes = await _verificacionRepository.GetPendientesAsync();
        return pendientes.Select(MapToDto);
    }

    public async Task<VerificacionRespuestaDto?> ObtenerPorIdAsync(int id)
    {
        var verificacion = await _verificacionRepository.GetByIdAsync(id);
        return verificacion == null ? null : MapToDto(verificacion);
    }

    private static VerificacionRespuestaDto MapToDto(VerificacionIdentidad v)
    {
        return new VerificacionRespuestaDto
        {
            IdVerificacion = v.IdVerificacion,
            IdUsuario = v.IdUsuario,
            NombreUsuario = $"{v.IdUsuarioNavigation.Nombres} {v.IdUsuarioNavigation.Apellidos}",
            CorreoUsuario = v.IdUsuarioNavigation.Correo,
            Estado = v.IdEstadoVerificacionNavigation?.Nombre ?? "",
            RutaImagen = v.RutaImagen,
            FechaSolicitud = v.FechaSolicitud,
            FechaResolucion = v.FechaResolucion,
            ObservacionAdmin = v.ObservacionAdmin
        };
    }
}
