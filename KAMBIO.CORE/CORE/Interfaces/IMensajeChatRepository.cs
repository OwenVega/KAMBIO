using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IMensajeChatRepository
{
    Task<MensajeChat> CreateAsync(MensajeChat mensaje);
    Task<IEnumerable<MensajeChat>> GetByTransaccionAsync(int idTransaccion);
    Task MarcarComoLeidosAsync(int idTransaccion, int idUsuario);
}
