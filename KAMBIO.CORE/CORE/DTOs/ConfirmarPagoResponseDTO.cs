namespace KAMBIO.CORE.Core.DTOs.ConfirmacionPago
{
    public class ConfirmarPagoResponseDTO
    {
        public int IdTransaccion { get; set; }

        public string Estado { get; set; } = string.Empty;

        public DateTime FechaConfirmacion { get; set; }

        public string Mensaje { get; set; } = string.Empty;
    }
}