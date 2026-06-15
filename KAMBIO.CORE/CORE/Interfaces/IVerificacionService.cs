// INTERFAZ del Servicio de Verificación
// Define las reglas de negocio para verificar la identidad de un usuario.
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IVerificacionService
{
    Task<VerificacionRespuestaDto> SolicitarVerificacionAsync(SolicitarVerificacionDto dto, int idUsuario);  // Usuario: pedir verificación
    Task<VerificacionRespuestaDto> RevisarVerificacionAsync(RevisarVerificacionDto dto, int idAdmin);         // Admin: aprobar/rechazar
    Task<IEnumerable<VerificacionRespuestaDto>> ObtenerPendientesAsync();                                       // Admin: ver solicitudes pendientes
    Task<VerificacionRespuestaDto?> ObtenerPorIdAsync(int id);                                                  // Ver detalle de una solicitud
}
