// REPOSITORIO de Chat (implementación)
// Aquí se guardan y consultan los mensajes del chat en SQL Server.
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories;

public class MensajeChatRepository : IMensajeChatRepository
{
    private readonly KambioDbContext _context;

    public MensajeChatRepository(KambioDbContext context)
    {
        _context = context;
    }

    // Guardar un mensaje nuevo en la tabla MensajeChat
    public async Task<MensajeChat> CreateAsync(MensajeChat mensaje)
    {
        _context.MensajeChat.Add(mensaje);
        await _context.SaveChangesAsync();
        return mensaje;
    }

    // Traer todos los mensajes de una transacción, ordenados de más viejo a más nuevo
    public async Task<IEnumerable<MensajeChat>> GetByTransaccionAsync(int idTransaccion)
    {
        return await _context.MensajeChat
            .Include(m => m.IdUsuarioEnviaNavigation)  // Incluye nombre de quien escribió
            .Where(m => m.IdTransaccion == idTransaccion)
            .OrderBy(m => m.FechaEnvio)               // Orden cronológico
            .ToListAsync();
    }

    // Marcar como "Leído" los mensajes de la otra persona en una transacción
    public async Task MarcarComoLeidosAsync(int idTransaccion, int idUsuario)
    {
        // Busca mensajes NO míos que aún NO han sido leídos
        var mensajes = await _context.MensajeChat
            .Where(m => m.IdTransaccion == idTransaccion && m.IdUsuarioEnvia != idUsuario && !m.Leido)
            .ToListAsync();

        foreach (var mensaje in mensajes)
            mensaje.Leido = true;  // Los marca como leídos

        await _context.SaveChangesAsync();
    }
}
