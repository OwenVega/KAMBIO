using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class CrearAlertaDto
{
    [Required(ErrorMessage = "La divisa de origen es obligatoria.")]
    public int IdDivisaOrigen { get; set; }

    [Required(ErrorMessage = "La divisa de destino es obligatoria.")]
    public int IdDivisaDestino { get; set; }

    [Required(ErrorMessage = "El valor umbral es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor umbral debe ser mayor a 0.")]
    public decimal ValorUmbral { get; set; }
}
