namespace KAMBIO.CORE.Core.DTOs;

public class MensajeRespuestaDto
{
    public int IdMensaje { get; set; }
    public int IdTransaccion { get; set; }
    public int IdUsuarioEnvia { get; set; }
    public string NombreUsuarioEnvia { get; set; } = null!;
    public string Mensaje { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
    public bool Leido { get; set; }
}
