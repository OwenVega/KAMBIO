// INTERFAZ del Servicio (contrato)
// Define las operaciones de NEGOCIO que se pueden hacer con las alertas.
// El "Servicio" contiene las reglas de negocio (validaciones, lógica).
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IAlertaService
{
    Task<AlertaRespuestaDto> CrearAlertaAsync(CrearAlertaDto dto, int idUsuario);       // Crear una alerta (con validaciones)
    Task<IEnumerable<AlertaRespuestaDto>> ObtenerPorUsuarioAsync(int idUsuario);        // Ver mis alertas
    Task<AlertaRespuestaDto> ActualizarAlertaAsync(int id, ActualizarAlertaDto dto, int idUsuario); // Editar alerta
    Task EliminarAlertaAsync(int id, int idUsuario);                                    // Eliminar alerta
}
