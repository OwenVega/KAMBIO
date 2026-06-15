using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IAdministracionUsuarioRepository
    {
        Task ActualizarUsuarioAsync(Usuario usuario);
        Task CancelarOfertasActivasAsync(int idUsuario);
        Task MarcarTransaccionesParaRevisionAsync(int idUsuario);
        Task<Usuario> ObtenerUsuarioPorIdAsync(int idUsuario);
        Task<IEnumerable<Usuario>> ObtenerUsuariosParaAdminAsync();
    }
}