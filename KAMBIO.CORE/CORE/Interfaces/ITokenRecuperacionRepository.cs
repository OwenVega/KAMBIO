using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface ITokenRecuperacionRepository
    {
        Task CrearTokenAsync(TokenRecuperacion token);
        Task<TokenRecuperacion?> ObtenerTokenValidoAsync(string token);
        Task MarcarTokenUsadoAsync(int idToken);
    }
}
