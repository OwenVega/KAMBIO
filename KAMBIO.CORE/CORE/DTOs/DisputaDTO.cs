namespace KAMBIO.CORE.Core.DTOs
{
    public class DisputaDTO
    {
        public int IdDisputa { get; set; }

        public int IdTransaccion { get; set; }

        public string UsuarioReportante { get; set; }

        public string Estado { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaReporte { get; set; }
    }
}