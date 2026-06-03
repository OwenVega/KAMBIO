namespace KAMBIO.CORE.Core.DTOs;

public class VerificacionRespuestaDto
{
    public int IdVerificacion { get; set; }
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public string CorreoUsuario { get; set; } = null!;
    public string Estado { get; set; } = null!;
    public string RutaImagen { get; set; } = null!;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public string? ObservacionAdmin { get; set; }
}
