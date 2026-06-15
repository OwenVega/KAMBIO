namespace KAMBIO.CORE.Core.DTOs.OfertaVenta
{
    public class CrearOfertaVentaRequestDTO
    {
        public int IdUsuario { get; set; }

        public int IdDivisaOrigen { get; set; }

        public int IdDivisaDestino { get; set; }

        public decimal MontoDisponible { get; set; }

        public decimal MontoMinimo { get; set; }

        public decimal MontoMaximo { get; set; }

        public decimal TasaCambio { get; set; }

        public List<int> IdBancos { get; set; } = new();
    }
}