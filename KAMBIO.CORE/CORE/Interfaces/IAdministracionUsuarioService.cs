using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IAdministracionUsuarioService
    {
        Task CambiarEstadoCuentaAsync(CambiarEstadoUsuarioDto dto);
        Task<IEnumerable<UsuarioListadoAdminDto>> ObtenerListadoUsuariosAsync();
    }
}