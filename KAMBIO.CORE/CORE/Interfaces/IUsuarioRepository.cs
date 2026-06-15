using KAMBIO.CORE.Core.Entities;


namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IUsuarioRepository
    {
        Task agregarUsuario(Usuario usuario);
        Task<bool> ExisteCorreo(string correo);
        Task<Usuario> ObtenerPorCorreoAsync(string correo);
        Task<Usuario> ObtenerPorIdAsync(int id);
        Task ActualizarAsync(Usuario usuario);

    }
}