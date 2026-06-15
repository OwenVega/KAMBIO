// DTO de respuesta - esto es lo que el API devuelve sobre una solicitud de verificación
namespace KAMBIO.CORE.Core.DTOs;

public class VerificacionRespuestaDto
{
    public int IdVerificacion { get; set; }       // ID de la solicitud
    public int IdUsuario { get; set; }            // ID del usuario que pide verificación
    public string NombreUsuario { get; set; } = null!;  // "Juan Pérez"
    public string CorreoUsuario { get; set; } = null!;  // correo@ejemplo.com
    public string Estado { get; set; } = null!;   // "Pendiente", "Verificado", "Rechazado"
    public string RutaImagen { get; set; } = null!;     // Ruta de la foto del DNI
    public DateTime FechaSolicitud { get; set; }        // Cuándo pidió la verificación
    public DateTime? FechaResolucion { get; set; }      // Cuándo el admin la revisó
    public string? ObservacionAdmin { get; set; }       // Comentario del admin (si rechazó, por qué)
}
