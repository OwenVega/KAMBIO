using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario> LoginAsync(LoginUsuarioDto dto);
        Task RegistrarUsuarioAsync(RegistroUsuarioDto dto);
    }
}