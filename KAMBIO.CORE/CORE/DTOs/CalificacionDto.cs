namespace KAMBIO.CORE.Core.DTOs
{
    public class CalificacionDto
    {
        public int IdTransaccion { get; set; }
        public int IdUsuarioEvalua { get; set; }
        public int IdUsuarioEvaluado { get; set; }
        public byte Estrellas { get; set; }
        public string? Comentario { get; set; }
    }

    public class PromedioCalificacionDto
    {
        public int IdUsuario { get; set; }
        public double Promedio { get; set; }
        public int TotalCalificaciones { get; set; }
    }
}