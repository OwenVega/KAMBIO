using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaRepository
    {
        Task<Oferta> CrearOfertaCompra(Oferta oferta, List<int> idBancos);
        Task<List<Oferta>> ObtenerOfertasActivas();
    }
}