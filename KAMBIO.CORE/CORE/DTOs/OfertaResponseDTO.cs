namespace KAMBIO.CORE.Core.DTOs
{
    public class OfertaResponseDTO
    {
        public string Mensaje { get; set; } = string.Empty;
        public int IdOferta { get; set; }
        public int IdUsuario { get; set; }
        public string TipoOferta { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string DivisaOrigen { get; set; } = string.Empty;
        public string DivisaDestino { get; set; } = string.Empty;
        public decimal MontoDisponible { get; set; }
        public decimal MontoMinimo { get; set; }
        public decimal MontoMaximo { get; set; }
        public decimal TasaCambio { get; set; }
        public List<string> MetodosPago { get; set; } = new List<string>();
        public DateTime FechaPublicacion { get; set; }
    }
}