// DTO para que el ADMIN revise una solicitud de verificación
// El admin puede aprobar (IdEstadoVerificacion=2) o rechazar (IdEstadoVerificacion=3)
using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class RevisarVerificacionDto
{
    [Required(ErrorMessage = "El ID de verificación es obligatorio.")]
    public int IdVerificacion { get; set; }        // ID de la solicitud a revisar

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public int IdEstadoVerificacion { get; set; }  // 2=Aprobar, 3=Rechazar

    public string? ObservacionAdmin { get; set; }  // Mensaje opcional del admin
}
