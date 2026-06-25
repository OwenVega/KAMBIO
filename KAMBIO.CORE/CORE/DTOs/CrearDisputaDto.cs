namespace KAMBIO.CORE.Core.DTOs
{
    public class CrearDisputaDto
    {
        public int IdTransaccion { get; set; }
        public int IdUsuarioReporta { get; set; }
        public string Descripcion { get; set; } = null!;
    }
}
