using KAMBIO.CORE.Core.DTOs.ConfirmacionPago;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.CORE.Core.Services
{
    public class ConfirmacionPagoService : IConfirmacionPagoService
    {
        private readonly IConfirmacionPagoRepository _repository;

        public ConfirmacionPagoService(
            IConfirmacionPagoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConfirmarPagoResponseDTO> ConfirmarPago(
            int idTransaccion,
            ConfirmarPagoRequestDTO request)
        {
            var transaccion =
                await _repository.ObtenerTransaccion(idTransaccion);

            if (transaccion == null)
                throw new Exception("La transacción no existe.");

            // Solo se puede confirmar si está EN PROCESO
            // 1 = Pendiente
            // 2 = En Proceso
            // 3 = Pago Realizado
            // 4 = Completada
            // 5 = Cancelada
            // 6 = En Disputa
            if (transaccion.IdEstadoTransaccion != 2)
                throw new Exception(
                    "La transacción no permite confirmar el pago.");

            // Validar que el usuario participe en la transacción
            if (transaccion.IdUsuarioComprador != request.IdUsuario &&
                transaccion.IdUsuarioVendedor != request.IdUsuario)
            {
                throw new Exception(
                    "El usuario no pertenece a esta transacción.");
            }

            // Actualizar estado a PAGO_REALIZADO
            transaccion.IdEstadoTransaccion = 3;
            transaccion.FechaConfirmacionPago = DateTime.Now;

            await _repository.ActualizarEstadoPago(transaccion);

            // Registrar historial
            var historial = new HistorialEstadoTransaccion
            {
                IdTransaccion = transaccion.IdTransaccion,
                IdEstadoTransaccion = 3,
                FechaCambio = DateTime.Now,
                Observacion = "Pago confirmado por el comprador",
                IdUsuarioCambio = request.IdUsuario
            };

            await _repository.RegistrarHistorial(historial);

            return new ConfirmarPagoResponseDTO
            {
                IdTransaccion = transaccion.IdTransaccion,
                Estado = "PAGO_REALIZADO",
                FechaConfirmacion = transaccion.FechaConfirmacionPago.Value,
                Mensaje = "Pago confirmado exitosamente."
            };
        }
    }
}