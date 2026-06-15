using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IMetodoPagoRepository
    {
        Task ActualizarAsync(MetodoPago metodoPago);
        Task AgregarAsync(MetodoPago metodoPago);
        Task<MetodoPago> ObtenerPorIdAsync(int idMetodoPago);
        Task<IEnumerable<MetodoPago>> ObtenerPorUsuarioIdAsync(int idUsuario);
        Task<bool> TieneTransaccionesActivasAsync(int idUsuario);
    }
}