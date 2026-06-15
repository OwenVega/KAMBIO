// DTO para CREAR una oferta de compra/venta de divisas
// Define todos los datos necesarios para publicar una oferta en el mercado P2P.
using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs;

public class CrearOfertaDto
{
    [Required(ErrorMessage = "La divisa de origen es obligatoria.")]
    public int IdDivisaOrigen { get; set; }      // ¿Qué moneda tienes? (ej: 1 = USD)

    [Required(ErrorMessage = "La divisa de destino es obligatoria.")]
    public int IdDivisaDestino { get; set; }     // ¿Qué moneda quieres? (ej: 2 = PEN)

    [Required(ErrorMessage = "El monto disponible es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto disponible debe ser mayor a 0.")]
    public decimal MontoDisponible { get; set; } // ¿Cuánto tienes disponible para operar?

    [Required(ErrorMessage = "El monto mínimo es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto mínimo debe ser mayor a 0.")]
    public decimal MontoMinimo { get; set; }     // ¿Cuál es el monto mínimo que aceptas?

    [Required(ErrorMessage = "El monto máximo es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto máximo debe ser mayor a 0.")]
    public decimal MontoMaximo { get; set; }     // ¿Cuál es el monto máximo que aceptas?

    [Required(ErrorMessage = "La tasa de cambio es obligatoria.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "La tasa de cambio debe ser mayor a 0.")]
    public decimal TasaCambio { get; set; }      // ¿A qué tipo de cambio? (ej: 3.75)

    [Required(ErrorMessage = "El tipo de oferta es obligatorio.")]
    public int IdTipoOferta { get; set; }        // 1 = Compra, 2 = Venta

    [Required(ErrorMessage = "Debe seleccionar al menos un método de pago.")]
    [MinLength(1, ErrorMessage = "Debe seleccionar al menos un banco.")]
    public List<int> IdsBancos { get; set; } = new(); // ¿Qué bancos aceptas? [1, 2, 3]
}
