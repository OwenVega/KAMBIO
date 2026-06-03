using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class SolicitarVerificacionDto
{
    [Required(ErrorMessage = "La ruta de la imagen es obligatoria.")]
    public string RutaImagen { get; set; } = null!;
}
