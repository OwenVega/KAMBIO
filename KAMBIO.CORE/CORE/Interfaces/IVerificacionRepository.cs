// INTERFAZ del Repositorio de Verificación
// Define los métodos para guardar y consultar solicitudes de verificación en la BD.
using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IVerificacionRepository
{
    Task<VerificacionIdentidad> CreateAsync(VerificacionIdentidad verificacion);     // Guardar nueva solicitud
    Task<VerificacionIdentidad?> GetByIdAsync(int id);                               // Buscar por ID
    Task<IEnumerable<VerificacionIdentidad>> GetByUsuarioAsync(int idUsuario);       // Solicitudes de un usuario
    Task<IEnumerable<VerificacionIdentidad>> GetPendientesAsync();                   // Solicitudes sin revisar (Pendientes)
    Task UpdateAsync(VerificacionIdentidad verificacion);                            // Actualizar (aprobar/rechazar)
}
