using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs
{
    public class MetodoPagoListDto
    {
        public int IdMetodoPago { get; set; }
        public string Banco { get; set; } = null!;
        public string TipoCuenta { get; set; } = null!;
        public string NumeroCuentaEnmascarado { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class MetodoPagoCrearDto
    {
        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un banco.")]
        public int IdBanco { get; set; }

        [Required(ErrorMessage = "El tipo de cuenta es obligatorio.")]
        [StringLength(50, ErrorMessage = "El tipo de cuenta no puede exceder los 50 caracteres.")]
        public string TipoCuenta { get; set; } = null!;

        [Required(ErrorMessage = "El número de cuenta es obligatorio.")]
        [MinLength(10, ErrorMessage = "El número de cuenta debe tener al menos 10 dígitos.")]
        [MaxLength(30, ErrorMessage = "El número de cuenta no puede exceder los 30 dígitos.")]
        public string NumeroCuenta { get; set; } = null!;

        [Required(ErrorMessage = "El Código de Cuenta Interbancario (CCI) es obligatorio.")]
        [StringLength(20, MinimumLength = 20, ErrorMessage = "El CCI debe tener exactamente 20 dígitos.")]
        public string Cci { get; set; } = null!;
    }
}