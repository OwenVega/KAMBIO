using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class CrearOfertaDto
{
    [Required(ErrorMessage = "La divisa de origen es obligatoria.")]
    public int IdDivisaOrigen { get; set; }

    [Required(ErrorMessage = "La divisa de destino es obligatoria.")]
    public int IdDivisaDestino { get; set; }

    [Required(ErrorMessage = "El monto disponible es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto disponible debe ser mayor a 0.")]
    public decimal MontoDisponible { get; set; }

    [Required(ErrorMessage = "El monto mínimo es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto mínimo debe ser mayor a 0.")]
    public decimal MontoMinimo { get; set; }

    [Required(ErrorMessage = "El monto máximo es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto máximo debe ser mayor a 0.")]
    public decimal MontoMaximo { get; set; }

    [Required(ErrorMessage = "La tasa de cambio es obligatoria.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "La tasa de cambio debe ser mayor a 0.")]
    public decimal TasaCambio { get; set; }

    [Required(ErrorMessage = "El tipo de oferta es obligatorio.")]
    public int IdTipoOferta { get; set; }

    [Required(ErrorMessage = "Debe seleccionar al menos un método de pago.")]
    [MinLength(1, ErrorMessage = "Debe seleccionar al menos un banco.")]
    public List<int> IdsBancos { get; set; } = new();
}
