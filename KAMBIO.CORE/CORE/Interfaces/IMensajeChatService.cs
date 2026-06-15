// INTERFAZ del Servicio de Chat
// Define las reglas para enviar y recibir mensajes entre 2 usuarios en una transacción.
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IMensajeChatService
{
    Task<MensajeRespuestaDto> EnviarMensajeAsync(EnviarMensajeDto dto, int idUsuarioEnvia);      // Enviar mensaje
    Task<IEnumerable<MensajeRespuestaDto>> ObtenerMensajesAsync(int idTransaccion, int idUsuario); // Ver conversación
}
