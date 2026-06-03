// REPOSITORIO de Ofertas (implementación)
// Aquí se guardan y consultan las ofertas en SQL Server.
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

    // Guardar una nueva oferta en la tabla Oferta (con sus métodos de pago)
    public async Task<Oferta> CreateAsync(Oferta oferta)
    {
        _context.Oferta.Add(oferta);
        await _context.SaveChangesAsync();

        // Vuelve a traer la oferta con todos sus datos relacionados para mostrar
        return await _context.Oferta
            .Include(o => o.IdUsuarioNavigation)              // Datos del anunciante
            .Include(o => o.IdDivisaOrigenNavigation)         // Moneda origen (USD)
            .Include(o => o.IdDivisaDestinoNavigation)        // Moneda destino (PEN)
            .Include(o => o.IdTipoOfertaNavigation)            // Compra o Venta
            .Include(o => o.IdEstadoOfertaNavigation)          // Activa/Cancelada
            .Include(o => o.OfertaMetodoPago).ThenInclude(omp => omp.IdBancoNavigation) // Bancos
            .FirstAsync(o => o.IdOferta == oferta.IdOferta);
    }

    // Buscar una oferta por ID (con todas sus relaciones)
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

    // Traer las ofertas de un usuario específico
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

    // Traer SOLO las ofertas activas (Estado = 1) para mostrar en el mercado P2P
    public async Task<IEnumerable<Oferta>> GetActivasAsync()
    {
        return await _context.Oferta
            .Include(o => o.IdUsuarioNavigation)
            .Include(o => o.IdDivisaOrigenNavigation)
            .Include(o => o.IdDivisaDestinoNavigation)
            .Include(o => o.IdTipoOfertaNavigation)
            .Include(o => o.IdEstadoOfertaNavigation)
            .Include(o => o.OfertaMetodoPago).ThenInclude(omp => omp.IdBancoNavigation)
            .Where(o => o.IdEstadoOferta == 1)  // Solo activas
            .ToListAsync();
    }

    // Verificar si un banco existe en la BD (para validar al crear oferta)
    public async Task<Banco?> GetBancoByIdAsync(int id)
    {
        return await _context.Banco.FindAsync(id);
    }
}
