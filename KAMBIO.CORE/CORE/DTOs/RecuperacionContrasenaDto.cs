namespace KAMBIO.CORE.Core.DTOs
{
    public class SolicitarRecuperacionDto
    {
        public string Correo { get; set; }
    }

    public class RestablecerContrasenaDto
    {
        public string Token { get; set; }
        public string NuevaContrasena { get; set; }
        public string ConfirmarContrasena { get; set; }
    }
}
