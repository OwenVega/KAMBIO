using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IConfirmacionPagoRepository
    {
        Task<Transaccion?> ObtenerTransaccion(int idTransaccion);

        Task ActualizarEstadoPago(Transaccion transaccion);

        Task RegistrarHistorial(
            HistorialEstadoTransaccion historial);
    }
}