using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class RevisarVerificacionDto
{
    [Required(ErrorMessage = "El ID de verificación es obligatorio.")]
    public int IdVerificacion { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public int IdEstadoVerificacion { get; set; }

    public string? ObservacionAdmin { get; set; }
}
