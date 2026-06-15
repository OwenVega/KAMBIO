using System.ComponentModel.DataAnnotations;

// DTO = "Data Transfer Object" - es un "molde" que define qué datos
// necesita la API para crear una alerta de tipo de cambio.
namespace KAMBIO.CORE.Core.DTOs;

public class CrearAlertaDto
{
    [Required(ErrorMessage = "La divisa de origen es obligatoria.")]
    public int IdDivisaOrigen { get; set; }  // ID de la moneda que tienes (ej: USD)

    [Required(ErrorMessage = "La divisa de destino es obligatoria.")]
    public int IdDivisaDestino { get; set; }  // ID de la moneda que quieres (ej: PEN)

    [Required(ErrorMessage = "El valor umbral es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El valor umbral debe ser mayor a 0.")]
    public decimal ValorUmbral { get; set; }  // ¿A qué valor del tipo de cambio te aviso? (ej: 4.50)
}
