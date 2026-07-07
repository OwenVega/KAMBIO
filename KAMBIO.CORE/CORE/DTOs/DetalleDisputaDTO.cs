namespace KAMBIO.CORE.Core.DTOs
{
    public class DetalleDisputaDTO
    {
        public int IdDisputa { get; set; }
        public int IdTransaccion { get; set; }
        public int IdUsuarioReporta { get; set; }
        public string UsuarioReportante { get; set; }
        public string Estado { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaReporte { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? ResolucionDetalle { get; set; }

        public int IdUsuarioComprador { get; set; }
        public string Comprador { get; set; } = null!;
        public int IdUsuarioVendedor { get; set; }
        public string Vendedor { get; set; } = null!;
        public string LadoReportante { get; set; } = null!;
    }
}