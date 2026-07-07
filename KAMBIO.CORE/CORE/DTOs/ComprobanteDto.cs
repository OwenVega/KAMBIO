namespace KAMBIO.CORE.Core.DTOs
{
    public class ComprobanteDto
    {
        public int IdComprobante { get; set; }
        public string RutaImagen { get; set; } = null!;
        public DateTime FechaSubida { get; set; }
    }
}