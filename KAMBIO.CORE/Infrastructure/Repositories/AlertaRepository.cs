using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories;

public class AlertaRepository : IAlertaRepository
{
    private readonly KambioDbContext _context;

    public AlertaRepository(KambioDbContext context)
    {
        _context = context;
    }

    public async Task<AlertaTipoCambio> CreateAsync(AlertaTipoCambio alerta)
    {
        _context.AlertaTipoCambio.Add(alerta);
        await _context.SaveChangesAsync();

        return await _context.AlertaTipoCambio
            .Include(a => a.IdDivisaOrigenNavigation)
            .Include(a => a.IdDivisaDestinoNavigation)
            .FirstAsync(a => a.IdAlerta == alerta.IdAlerta);
    }

    public async Task<AlertaTipoCambio?> GetByIdAsync(int id)
    {
        return await _context.AlertaTipoCambio
            .Include(a => a.IdDivisaOrigenNavigation)
            .Include(a => a.IdDivisaDestinoNavigation)
            .FirstOrDefaultAsync(a => a.IdAlerta == id);
    }

    public async Task<IEnumerable<AlertaTipoCambio>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.AlertaTipoCambio
            .Include(a => a.IdDivisaOrigenNavigation)
            .Include(a => a.IdDivisaDestinoNavigation)
            .Where(a => a.IdUsuario == idUsuario)
            .ToListAsync();
    }

    public async Task UpdateAsync(AlertaTipoCambio alerta)
    {
        _context.AlertaTipoCambio.Update(alerta);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(AlertaTipoCambio alerta)
    {
        _context.AlertaTipoCambio.Remove(alerta);
        await _context.SaveChangesAsync();
    }
}
