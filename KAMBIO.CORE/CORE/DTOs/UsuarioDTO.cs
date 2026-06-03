using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs
{
    public class RegistroUsuarioDto
    {
        [Required(ErrorMessage = "El campo nombres es obligatorio.")]
        public string Nombres { get; set; }

        [Required(ErrorMessage = "El campo apellidos es obligatorio.")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El campo correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo incorrecto.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El campo contraseña es obligatorio.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Contrasena { get; set; }
    }
}