using KAMBIO.CORE.Core.DTOs.ConfirmacionPago;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IConfirmacionPagoService
    {
        Task<ConfirmarPagoResponseDTO> ConfirmarPago(
            int idTransaccion,
            ConfirmarPagoRequestDTO request);
    }
}