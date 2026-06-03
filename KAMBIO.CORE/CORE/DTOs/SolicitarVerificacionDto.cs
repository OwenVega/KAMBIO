// DTO para SOLICITAR verificación de identidad
// El usuario manda la ruta/foto de su DNI para que el admin lo revise.
using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class SolicitarVerificacionDto
{
    [Required(ErrorMessage = "La ruta de la imagen es obligatoria.")]
    public string RutaImagen { get; set; } = null!;  // Ruta donde se guardó la foto del DNI
}
