using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IAlertaRepository
{
    Task<AlertaTipoCambio> CreateAsync(AlertaTipoCambio alerta);
    Task<AlertaTipoCambio?> GetByIdAsync(int id);
    Task<IEnumerable<AlertaTipoCambio>> GetByUsuarioAsync(int idUsuario);
    Task UpdateAsync(AlertaTipoCambio alerta);
    Task DeleteAsync(AlertaTipoCambio alerta);
}
