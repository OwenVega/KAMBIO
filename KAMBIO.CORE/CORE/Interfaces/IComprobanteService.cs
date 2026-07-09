using Microsoft.AspNetCore.Http;
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IComprobanteService
    {
        Task SubirComprobanteAsync(int idTransaccion, int idUsuario, IFormFile archivo, string carpetaVouchers);
        Task<List<ComprobanteDto>> ObtenerPorTransaccionAsync(int idTransaccion);
    }
}