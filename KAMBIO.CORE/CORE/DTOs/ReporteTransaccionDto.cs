namespace KAMBIO.CORE.Core.DTOs
{
    public class FiltroReporteDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? IdDivisa { get; set; }
        public int? IdUsuario { get; set; }
    }

    public class ReporteTransaccionDto
    {
        public int IdTransaccion { get; set; }
        public string Comprador { get; set; } = null!;
        public string Vendedor { get; set; } = null!;
        public decimal Monto { get; set; }
        public decimal MontoEquivalente { get; set; }
        public decimal TasaCambioAplicada { get; set; }
        public string TipoOperacion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string DivisaOrigen { get; set; } = null!;
        public string DivisaDestino { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
    }
}