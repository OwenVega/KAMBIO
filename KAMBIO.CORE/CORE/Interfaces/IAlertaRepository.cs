// INTERFAZ del Repositorio (contrato)
// Define las operaciones que se pueden hacer en la BD para las alertas.
// El "Repositorio" es la capa que habla directamente con SQL Server.
using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IAlertaRepository
{
    Task<AlertaTipoCambio> CreateAsync(AlertaTipoCambio alerta);        // Guardar una alerta nueva en la BD
    Task<AlertaTipoCambio?> GetByIdAsync(int id);                       // Buscar una alerta por su ID
    Task<IEnumerable<AlertaTipoCambio>> GetByUsuarioAsync(int idUsuario); // Traer todas las alertas de un usuario
    Task UpdateAsync(AlertaTipoCambio alerta);                          // Actualizar una alerta en la BD
    Task DeleteAsync(AlertaTipoCambio alerta);                          // Eliminar una alerta de la BD
}
