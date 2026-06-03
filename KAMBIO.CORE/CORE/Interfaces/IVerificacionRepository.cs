using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IVerificacionRepository
{
    Task<VerificacionIdentidad> CreateAsync(VerificacionIdentidad verificacion);
    Task<VerificacionIdentidad?> GetByIdAsync(int id);
    Task<IEnumerable<VerificacionIdentidad>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<VerificacionIdentidad>> GetPendientesAsync();
    Task UpdateAsync(VerificacionIdentidad verificacion);
}
