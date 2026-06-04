using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaRepository
    {
        Task<List<Oferta>> ObtenerOfertasFiltradasAsync(int idTipoOferta, int idDivisaOrigen, int idDivisaDestino, decimal? monto, int? idBanco);
    }
}