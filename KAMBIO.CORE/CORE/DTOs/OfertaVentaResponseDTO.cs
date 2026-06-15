namespace KAMBIO.CORE.Core.DTOs.OfertaVenta
{
    public class OfertaVentaResponseDTO
    {
        public int IdOferta { get; set; }

        public string DivisaOrigen { get; set; } = string.Empty;

        public string DivisaDestino { get; set; } = string.Empty;

        public decimal MontoDisponible { get; set; }

        public decimal TasaCambio { get; set; }

        public string Estado { get; set; } = string.Empty;

        public DateTime FechaPublicacion { get; set; }

        public List<string> MetodosPago { get; set; } = new();
    }
}