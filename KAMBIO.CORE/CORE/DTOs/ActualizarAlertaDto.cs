// DTO para actualizar una alerta existente.
// Puedes cambiar el valor umbral y si está activa o no.
using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class ActualizarAlertaDto
{
    [Required(ErrorMessage = "El valor umbral es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor umbral debe ser mayor a 0.")]
    public decimal ValorUmbral { get; set; }  // Nuevo valor para el tipo de cambio

    public bool Activa { get; set; }          // true = activa, false = desactivada
}
