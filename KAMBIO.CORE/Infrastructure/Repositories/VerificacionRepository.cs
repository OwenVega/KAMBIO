using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories;

public class VerificacionRepository : IVerificacionRepository
{
    private readonly KambioDbContext _context;

    public VerificacionRepository(KambioDbContext context)
    {
        _context = context;
    }

    public async Task<VerificacionIdentidad> CreateAsync(VerificacionIdentidad verificacion)
    {
        _context.VerificacionIdentidad.Add(verificacion);
        await _context.SaveChangesAsync();
        return verificacion;
    }

    public async Task<VerificacionIdentidad?> GetByIdAsync(int id)
    {
        return await _context.VerificacionIdentidad
            .Include(v => v.IdUsuarioNavigation)
            .Include(v => v.IdEstadoVerificacionNavigation)
            .Include(v => v.IdAdminResolucionNavigation)
            .FirstOrDefaultAsync(v => v.IdVerificacion == id);
    }

    public async Task<IEnumerable<VerificacionIdentidad>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.VerificacionIdentidad
            .Include(v => v.IdEstadoVerificacionNavigation)
            .Where(v => v.IdUsuario == idUsuario)
            .ToListAsync();
    }

    public async Task<IEnumerable<VerificacionIdentidad>> GetPendientesAsync()
    {
        return await _context.VerificacionIdentidad
            .Include(v => v.IdUsuarioNavigation)
            .Include(v => v.IdEstadoVerificacionNavigation)
            .Where(v => v.IdEstadoVerificacion == 1)
            .ToListAsync();
    }

    public async Task UpdateAsync(VerificacionIdentidad verificacion)
    {
        _context.VerificacionIdentidad.Update(verificacion);
        await _context.SaveChangesAsync();
    }
}
