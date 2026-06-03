using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class EnviarMensajeDto
{
    [Required(ErrorMessage = "La transacción es obligatoria.")]
    public int IdTransaccion { get; set; }

    [Required(ErrorMessage = "El mensaje no puede estar vacío.")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "El mensaje debe tener entre 1 y 2000 caracteres.")]
    public string Mensaje { get; set; } = null!;
}
