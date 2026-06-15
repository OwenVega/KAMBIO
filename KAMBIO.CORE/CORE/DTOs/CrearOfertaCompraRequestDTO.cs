using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs
{
    public class CrearOfertaCompraRequestDTO
    {
        [Required(ErrorMessage = "La divisa que desea adquirir es obligatoria.")]
        public int IdDivisaOrigen { get; set; }

        [Required(ErrorMessage = "La divisa con la que pagará es obligatoria.")]
        public int IdDivisaDestino { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal MontoDisponible { get; set; }

        [Required(ErrorMessage = "El monto mínimo es obligatorio.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El monto mínimo debe ser mayor a cero.")]
        public decimal MontoMinimo { get; set; }

        [Required(ErrorMessage = "El monto máximo es obligatorio.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El monto máximo debe ser mayor a cero.")]
        public decimal MontoMaximo { get; set; }

        [Required(ErrorMessage = "El tipo de cambio es obligatorio.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El tipo de cambio debe ser mayor a cero.")]
        public decimal TasaCambio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar al menos un método de pago.")]
        public List<int> MetodosPago { get; set; } = new List<int>();
    }
}
