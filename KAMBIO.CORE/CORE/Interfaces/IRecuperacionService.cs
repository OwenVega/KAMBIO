namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IRecuperacionService
    {
        Task SolicitarRecuperacionAsync(string correo);
        Task RestablecerContrasenaAsync(string token, string nuevaContrasena, string confirmarContrasena);
    }
}
