namespace KAMBIO.CORE.Core.DTOs
{
    public class NotificacionDto
    {
        public int IdNotificacion { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? IdReferencia { get; set; }
        public string TipoReferencia { get; set; }
    }
}
