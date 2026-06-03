using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IUsuarioService
    {
        Task RegistrarUsuarioAsync(RegistroUsuarioDto dto);
    }
}