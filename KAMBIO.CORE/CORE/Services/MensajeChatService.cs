// SERVICIO de Chat (lógica de negocio)
// Reglas: Solo puedes chatear si eres parte de la transacción y la transacción está activa.
// El chat se desactiva cuando la transacción se completa o se cancela.
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Core.Services;

public class MensajeChatService : IMensajeChatService
{
    private readonly IMensajeChatRepository _mensajeRepository;
    private readonly KambioDbContext _context;

    public MensajeChatService(IMensajeChatRepository mensajeRepository, KambioDbContext context)
    {
        _mensajeRepository = mensajeRepository;
        _context = context;
    }

    // ENVIAR mensaje: validamos que la transacción exista, que seas parte y que esté activa
    public async Task<MensajeRespuestaDto> EnviarMensajeAsync(EnviarMensajeDto dto, int idUsuarioEnvia)
    {
        var transaccion = await _context.Transaccion.FindAsync(dto.IdTransaccion);
        if (transaccion == null)
            throw new InvalidOperationException("La transacción no existe.");

        // Solo el comprador o vendedor pueden chatear
        if (transaccion.IdUsuarioComprador != idUsuarioEnvia && transaccion.IdUsuarioVendedor != idUsuarioEnvia)
            throw new InvalidOperationException("No eres parte de esta transacción.");

        // Si la transacción ya terminó o se canceló, no se puede chatear
        var estadoTransaccion = await _context.EstadoTransaccion.FindAsync(transaccion.IdEstadoTransaccion);
        if (estadoTransaccion == null || estadoTransaccion.Nombre == "Completado" || estadoTransaccion.Nombre == "Cancelado")
            throw new InvalidOperationException("El chat solo está disponible durante una transacción activa.");

        var mensaje = new MensajeChat
        {
            IdTransaccion = dto.IdTransaccion,
            IdUsuarioEnvia = idUsuarioEnvia,
            Mensaje = dto.Mensaje,
            FechaEnvio = DateTime.Now,
            Leido = false  // Aún no lo han leído
        };

        var mensajeCreado = await _mensajeRepository.CreateAsync(mensaje);

        return new MensajeRespuestaDto
        {
            IdMensaje = mensajeCreado.IdMensaje,
            IdTransaccion = mensajeCreado.IdTransaccion,
            IdUsuarioEnvia = mensajeCreado.IdUsuarioEnvia,
            NombreUsuarioEnvia = $"{mensajeCreado.IdUsuarioEnviaNavigation.Nombres} {mensajeCreado.IdUsuarioEnviaNavigation.Apellidos}",
            Mensaje = mensajeCreado.Mensaje,
            FechaEnvio = mensajeCreado.FechaEnvio,
            Leido = mensajeCreado.Leido
        };
    }

    // VER mensajes: trae la conversación y marca como leídos los mensajes de la otra persona
    public async Task<IEnumerable<MensajeRespuestaDto>> ObtenerMensajesAsync(int idTransaccion, int idUsuario)
    {
        var transaccion = await _context.Transaccion.FindAsync(idTransaccion);
        if (transaccion == null)
            throw new InvalidOperationException("La transacción no existe.");

        // Verificar que seas parte
        if (transaccion.IdUsuarioComprador != idUsuario && transaccion.IdUsuarioVendedor != idUsuario)
            throw new InvalidOperationException("No eres parte de esta transacción.");

        // Marcar los mensajes de la otra persona como leídos automáticamente
        await _mensajeRepository.MarcarComoLeidosAsync(idTransaccion, idUsuario);

        var mensajes = await _mensajeRepository.GetByTransaccionAsync(idTransaccion);

        return mensajes.Select(m => new MensajeRespuestaDto
        {
            IdMensaje = m.IdMensaje,
            IdTransaccion = m.IdTransaccion,
            IdUsuarioEnvia = m.IdUsuarioEnvia,
            NombreUsuarioEnvia = $"{m.IdUsuarioEnviaNavigation.Nombres} {m.IdUsuarioEnviaNavigation.Apellidos}",
            Mensaje = m.Mensaje,
            FechaEnvio = m.FechaEnvio,
            Leido = m.Leido
        });
    }
}
