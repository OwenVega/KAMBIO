namespace KAMBIO.CORE.Core.DTOs
{
    public class PerfilUsuarioDto
    {
        public int IdUsuario { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string FotoPerfil { get; set; }
        public decimal? CalificacionPromedio { get; set; }
    }

    public class ActualizarPerfilDto
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
    }
}
