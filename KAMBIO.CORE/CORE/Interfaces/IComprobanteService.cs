using Microsoft.AspNetCore.Http;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IComprobanteService
    {
        Task SubirComprobanteAsync(int idTransaccion, int idUsuario, IFormFile archivo, string carpetaVouchers);
    }
}