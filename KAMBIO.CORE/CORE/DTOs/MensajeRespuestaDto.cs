// DTO de respuesta - así se ve cada mensaje del chat cuando lo consultas
namespace KAMBIO.CORE.Core.DTOs;

public class MensajeRespuestaDto
{
    public int IdMensaje { get; set; }                // ID del mensaje
    public int IdTransaccion { get; set; }            // Transacción a la que pertenece
    public int IdUsuarioEnvia { get; set; }           // Quién lo escribió
    public string NombreUsuarioEnvia { get; set; } = null!;  // "Juan Pérez" (quién lo envió)
    public string Mensaje { get; set; } = null!;      // Contenido del mensaje
    public DateTime FechaEnvio { get; set; }          // Fecha y hora exacta
    public bool Leido { get; set; }                   // True = ya lo vieron, False = no leído
}
