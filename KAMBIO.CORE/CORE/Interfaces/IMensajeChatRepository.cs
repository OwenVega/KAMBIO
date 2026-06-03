// INTERFAZ del Repositorio de Chat
// Define cómo guardar y traer mensajes de la BD.
using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IMensajeChatRepository
{
    Task<MensajeChat> CreateAsync(MensajeChat mensaje);                    // Guardar un mensaje nuevo
    Task<IEnumerable<MensajeChat>> GetByTransaccionAsync(int idTransaccion); // Traer todos los mensajes de una transacción
    Task MarcarComoLeidosAsync(int idTransaccion, int idUsuario);          // Marcar como leídos los que no son míos
}
