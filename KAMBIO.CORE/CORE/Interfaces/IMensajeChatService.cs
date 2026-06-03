using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IMensajeChatService
{
    Task<MensajeRespuestaDto> EnviarMensajeAsync(EnviarMensajeDto dto, int idUsuarioEnvia);
    Task<IEnumerable<MensajeRespuestaDto>> ObtenerMensajesAsync(int idTransaccion, int idUsuario);
}
