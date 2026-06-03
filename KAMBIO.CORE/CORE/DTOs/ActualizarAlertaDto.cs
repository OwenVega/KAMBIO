using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class ActualizarAlertaDto
{
    [Required(ErrorMessage = "El valor umbral es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor umbral debe ser mayor a 0.")]
    public decimal ValorUmbral { get; set; }

    public bool Activa { get; set; }
}
