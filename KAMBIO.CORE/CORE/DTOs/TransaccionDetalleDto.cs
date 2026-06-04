namespace KAMBIO.CORE.Core.DTOs
{
    public class TransaccionDetalleDto
    {
        public int IdTransaccion { get; set; }
        public int IdOferta { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoEquivalente { get; set; }
        public decimal TasaCambioAplicada { get; set; }
        public string TipoOperacion { get; set; } = null!;
        public string EstadoNombre { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaConfirmacionPago { get; set; }
        public DateTime? FechaCompletado { get; set; }
        public bool ConfirmadoPorComprador { get; set; }
        public bool ConfirmadoPorVendedor { get; set; }
    }

    public class CambiarEstadoDto
    {
        public int IdTransaccion { get; set; }
        public int IdEstadoTransaccion { get; set; }
        public int IdUsuarioCambio { get; set; }
        public string? Observacion { get; set; }
    }
}