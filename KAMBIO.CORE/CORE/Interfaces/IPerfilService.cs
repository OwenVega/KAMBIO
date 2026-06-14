using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IPerfilService
    {
        Task<PerfilUsuarioDto> ObtenerPerfilAsync(int idUsuario);
        Task ActualizarPerfilAsync(int idUsuario, ActualizarPerfilDto dto);
        Task ActualizarFotoPerfilAsync(int idUsuario, string rutaFoto);
    }
}