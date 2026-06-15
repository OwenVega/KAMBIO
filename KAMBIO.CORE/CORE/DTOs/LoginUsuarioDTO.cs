using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs
{
    public class LoginUsuarioDto
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo incorrecto.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Contrasena { get; set; }
    }
}