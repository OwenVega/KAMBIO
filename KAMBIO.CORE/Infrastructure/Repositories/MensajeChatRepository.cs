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

    public async Task<MensajeChat> CreateAsync(MensajeChat mensaje)
    {
        _context.MensajeChat.Add(mensaje);
        await _context.SaveChangesAsync();
        return mensaje;
    }

    public async Task<IEnumerable<MensajeChat>> GetByTransaccionAsync(int idTransaccion)
    {
        return await _context.MensajeChat
            .Include(m => m.IdUsuarioEnviaNavigation)
            .Where(m => m.IdTransaccion == idTransaccion)
            .OrderBy(m => m.FechaEnvio)
            .ToListAsync();
    }

    public async Task MarcarComoLeidosAsync(int idTransaccion, int idUsuario)
    {
        var mensajes = await _context.MensajeChat
            .Where(m => m.IdTransaccion == idTransaccion && m.IdUsuarioEnvia != idUsuario && !m.Leido)
            .ToListAsync();

        foreach (var mensaje in mensajes)
            mensaje.Leido = true;

        await _context.SaveChangesAsync();
    }
}
