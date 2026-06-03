// SERVICIO de Verificación (lógica de negocio)
// Reglas: No puedes pedir verificación si ya tienes una pendiente o ya estás verificado.
// El admin puede aprobar (marca EsVerificado=true) o rechazar.
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

    // USUARIO: Solicitar verificación de identidad
    public async Task<VerificacionRespuestaDto> SolicitarVerificacionAsync(SolicitarVerificacionDto dto, int idUsuario)
    {
        // Validar que no tenga ya una solicitud pendiente
        var existentes = await _verificacionRepository.GetByUsuarioAsync(idUsuario);
        if (existentes.Any(v => v.IdEstadoVerificacion == 1))
            throw new InvalidOperationException("Ya tienes una solicitud de verificación pendiente.");

        // Validar que no esté ya verificado
        if (existentes.Any(v => v.IdEstadoVerificacion == 2))
            throw new InvalidOperationException("Tu cuenta ya está verificada.");

        // Crear la solicitud con estado Pendiente (1)
        var verificacion = new VerificacionIdentidad
        {
            IdUsuario = idUsuario,
            IdEstadoVerificacion = 1,     // 1 = Pendiente
            RutaImagen = dto.RutaImagen,  // Foto del DNI
            FechaSolicitud = DateTime.Now
        };

        var creada = await _verificacionRepository.CreateAsync(verificacion);

        return MapToDto(creada);
    }

    // ADMIN: Revisar y aprobar/rechazar una solicitud
    public async Task<VerificacionRespuestaDto> RevisarVerificacionAsync(RevisarVerificacionDto dto, int idAdmin)
    {
        var verificacion = await _verificacionRepository.GetByIdAsync(dto.IdVerificacion);
        if (verificacion == null)
            throw new InvalidOperationException("La solicitud de verificación no existe.");

        // Actualizar datos de la revisión
        verificacion.IdEstadoVerificacion = dto.IdEstadoVerificacion; // 2 = Aprobado, 3 = Rechazado
        verificacion.IdAdminResolucion = idAdmin;                     // Quién lo revisó
        verificacion.FechaResolucion = DateTime.Now;                  // Cuándo lo revisó
        verificacion.ObservacionAdmin = dto.ObservacionAdmin;         // Comentario

        // Si aprueba, marcar al usuario como verificado (EsVerificado = true)
        if (dto.IdEstadoVerificacion == 2)
        {
            verificacion.IdUsuarioNavigation.EsVerificado = true;
        }

        await _verificacionRepository.UpdateAsync(verificacion);

        return MapToDto(verificacion);
    }

    // ADMIN: Obtener todas las solicitudes pendientes de revisión
    public async Task<IEnumerable<VerificacionRespuestaDto>> ObtenerPendientesAsync()
    {
        var pendientes = await _verificacionRepository.GetPendientesAsync();
        return pendientes.Select(MapToDto);
    }

    // Ver detalle de una solicitud específica
    public async Task<VerificacionRespuestaDto?> ObtenerPorIdAsync(int id)
    {
        var verificacion = await _verificacionRepository.GetByIdAsync(id);
        return verificacion == null ? null : MapToDto(verificacion);
    }

    // Convierte un objeto de BD a DTO para la respuesta JSON
    private static VerificacionRespuestaDto MapToDto(VerificacionIdentidad v)
    {
        return new VerificacionRespuestaDto
        {
            IdVerificacion = v.IdVerificacion,
            IdUsuario = v.IdUsuario,
            NombreUsuario = $"{v.IdUsuarioNavigation.Nombres} {v.IdUsuarioNavigation.Apellidos}",
            CorreoUsuario = v.IdUsuarioNavigation.Correo,
            Estado = v.IdEstadoVerificacionNavigation?.Nombre ?? "",  // "Pendiente", "Verificado", "Rechazado"
            RutaImagen = v.RutaImagen,
            FechaSolicitud = v.FechaSolicitud,
            FechaResolucion = v.FechaResolucion,
            ObservacionAdmin = v.ObservacionAdmin
        };
    }
}
