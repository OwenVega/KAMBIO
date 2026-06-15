using System.ComponentModel.DataAnnotations;

namespace KAMBIO.CORE.Core.DTOs
{
    public class UsuarioListadoAdminDto
    {
        public int IdUsuario { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public decimal CalificacionPromedio { get; set; }
        public int TotalOrdenes { get; set; }
        public string EstadoCuenta { get; set; } = null!;
    }

    public class CambiarEstadoUsuarioDto
    {
        [Required(ErrorMessage = "El ID del usuario a modificar es obligatorio.")]
        public int IdUsuarioObjetivo { get; set; }

        [Required(ErrorMessage = "El ID del administrador es obligatorio.")]
        public int IdAdmin { get; set; }

        [Required(ErrorMessage = "Debe especificar el nuevo estado de la cuenta (Ej: 1=Activo, 2=Suspendido, 3=Bloqueado).")]
        public int NuevoIdEstadoCuenta { get; set; }

        [Required(ErrorMessage = "El motivo de la acción es obligatorio.")]
        [MinLength(10, ErrorMessage = "Debe proporcionar un motivo detallado (mínimo 10 caracteres).")]
        public string Motivo { get; set; } = null!;
    }
}