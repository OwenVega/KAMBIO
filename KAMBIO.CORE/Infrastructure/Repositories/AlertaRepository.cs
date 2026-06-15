// REPOSITORIO de Alertas (implementación)
// Aquí se escriben las consultas SQL reales usando Entity Framework Core.
// Esta capa es la única que sabe cómo guardar y traer datos de SQL Server.
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories;

public class AlertaRepository : IAlertaRepository
{
    // _context = la conexión a la base de datos (KambioDbContext)
    private readonly KambioDbContext _context;

    public AlertaRepository(KambioDbContext context)
    {
        _context = context;
    }

    // Guardar una alerta nueva en la tabla AlertaTipoCambio de SQL Server
    public async Task<AlertaTipoCambio> CreateAsync(AlertaTipoCambio alerta)
    {
        _context.AlertaTipoCambio.Add(alerta);          // Prepara el INSERT
        await _context.SaveChangesAsync();               // Ejecuta el INSERT en SQL

        // Vuelve a traer la alerta pero con los datos de las divisas (USD, PEN) para mostrar
        return await _context.AlertaTipoCambio
            .Include(a => a.IdDivisaOrigenNavigation)    // Trae el nombre de la divisa origen
            .Include(a => a.IdDivisaDestinoNavigation)   // Trae el nombre de la divisa destino
            .FirstAsync(a => a.IdAlerta == alerta.IdAlerta);
    }

    // Buscar una alerta por su ID en la BD
    public async Task<AlertaTipoCambio?> GetByIdAsync(int id)
    {
        return await _context.AlertaTipoCambio
            .Include(a => a.IdDivisaOrigenNavigation)
            .Include(a => a.IdDivisaDestinoNavigation)
            .FirstOrDefaultAsync(a => a.IdAlerta == id);  // Si no existe, devuelve null
    }

    // Traer TODAS las alertas de un usuario específico
    public async Task<IEnumerable<AlertaTipoCambio>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.AlertaTipoCambio
            .Include(a => a.IdDivisaOrigenNavigation)
            .Include(a => a.IdDivisaDestinoNavigation)
            .Where(a => a.IdUsuario == idUsuario)        // Filtra por usuario
            .ToListAsync();
    }

    // Actualizar una alerta (cambiar valor umbral, activar/desactivar)
    public async Task UpdateAsync(AlertaTipoCambio alerta)
    {
        _context.AlertaTipoCambio.Update(alerta);    // Prepara el UPDATE
        await _context.SaveChangesAsync();            // Ejecuta el UPDATE en SQL
    }

    // Eliminar una alerta de la BD
    public async Task DeleteAsync(AlertaTipoCambio alerta)
    {
        _context.AlertaTipoCambio.Remove(alerta);    // Prepara el DELETE
        await _context.SaveChangesAsync();            // Ejecuta el DELETE en SQL
    }
}
