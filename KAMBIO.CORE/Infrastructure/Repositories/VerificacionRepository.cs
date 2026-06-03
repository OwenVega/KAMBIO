// REPOSITORIO de Verificación (implementación)
// Aquí están las consultas a SQL Server para las solicitudes de verificación.
using Microsoft.EntityFrameworkCore;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.CORE.Infrastructure.Repositories;

public class VerificacionRepository : IVerificacionRepository
{
    private readonly KambioDbContext _context;  // Conexión a la BD

    public VerificacionRepository(KambioDbContext context)
    {
        _context = context;
    }

    // Guardar una nueva solicitud de verificación en la BD
    public async Task<VerificacionIdentidad> CreateAsync(VerificacionIdentidad verificacion)
    {
        _context.VerificacionIdentidad.Add(verificacion);
        await _context.SaveChangesAsync();
        return verificacion;
    }

    // Buscar una solicitud por su ID (con datos del usuario, estado y admin)
    public async Task<VerificacionIdentidad?> GetByIdAsync(int id)
    {
        return await _context.VerificacionIdentidad
            .Include(v => v.IdUsuarioNavigation)              // Datos del usuario
            .Include(v => v.IdEstadoVerificacionNavigation)   // Estado (Pendiente/Verificado/Rechazado)
            .Include(v => v.IdAdminResolucionNavigation)      // Admin que revisó
            .FirstOrDefaultAsync(v => v.IdVerificacion == id);
    }

    // Traer todas las solicitudes de un usuario específico
    public async Task<IEnumerable<VerificacionIdentidad>> GetByUsuarioAsync(int idUsuario)
    {
        return await _context.VerificacionIdentidad
            .Include(v => v.IdEstadoVerificacionNavigation)
            .Where(v => v.IdUsuario == idUsuario)
            .ToListAsync();
    }

    // Traer SOLO las solicitudes PENDIENTES (Estado = 1) para que el admin las revise
    public async Task<IEnumerable<VerificacionIdentidad>> GetPendientesAsync()
    {
        return await _context.VerificacionIdentidad
            .Include(v => v.IdUsuarioNavigation)              // Quién pidió la verificación
            .Include(v => v.IdEstadoVerificacionNavigation)
            .Where(v => v.IdEstadoVerificacion == 1)          // 1 = Pendiente
            .ToListAsync();
    }

    // Actualizar una solicitud (aprobar o rechazar)
    public async Task UpdateAsync(VerificacionIdentidad verificacion)
    {
        _context.VerificacionIdentidad.Update(verificacion);
        await _context.SaveChangesAsync();
    }
}
