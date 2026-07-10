namespace KAMBIO.CORE.Core.DTOs
{
    public class ReseñaDto
    {
        public int IdCalificacion { get; set; }
        public string UsuarioEvaluaNombre { get; set; } = null!;
        public int Estrellas { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCalificacion { get; set; }
    }
}

