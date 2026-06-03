using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories;

public class OfertaRepository : IOfertaRepository
{
    private readonly KambioDbContext _context;

    public OfertaRepository(KambioDbContext context)
    {
        _context = context;
    }

    public async Task<Oferta> CreateAsync(Oferta oferta)
    {
        _context.Oferta.Add(oferta);
        await _context.SaveChangesAsync();
        return oferta;
    }

    public async Task<Oferta?> GetByIdAsync(int id)
    {
        return await _context.Oferta
            .Include(o => o.IdUsuarioNavigation)
            .Include(o => o.IdDivisaOrigenNavigation)
            .Include(o => o.IdDivisaDestinoNavigation)
            .Include(o => o.IdTipoOfertaNavigation)
            .Include(o => o.IdEstadoOfertaNavigation)
            .Include(o => o.OfertaMetodoPago).ThenInclude(omp => omp.IdBancoNavigation)
            .FirstOrDefaultAsync(o => o.IdOferta == id);
    }

    public async Task<IEnumerable<Oferta>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.Oferta
            .Include(o => o.IdDivisaOrigenNavigation)
            .Include(o => o.IdDivisaDestinoNavigation)
            .Include(o => o.IdTipoOfertaNavigation)
            .Include(o => o.IdEstadoOfertaNavigation)
            .Include(o => o.OfertaMetodoPago).ThenInclude(omp => omp.IdBancoNavigation)
            .Where(o => o.IdUsuario == idUsuario)
            .ToListAsync();
    }

    public async Task<IEnumerable<Oferta>> GetActivasAsync()
    {
        return await _context.Oferta
            .Include(o => o.IdUsuarioNavigation)
            .Include(o => o.IdDivisaOrigenNavigation)
            .Include(o => o.IdDivisaDestinoNavigation)
            .Include(o => o.IdTipoOfertaNavigation)
            .Include(o => o.IdEstadoOfertaNavigation)
            .Include(o => o.OfertaMetodoPago).ThenInclude(omp => omp.IdBancoNavigation)
            .Where(o => o.IdEstadoOferta == 1)
            .ToListAsync();
    }

    public async Task<Banco?> GetBancoByIdAsync(int id)
    {
        return await _context.Banco.FindAsync(id);
    }
}
