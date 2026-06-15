using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaVentaRepository
    {
        Task<Oferta> CrearOfertaVenta(Oferta oferta, List<int> idBancos);
        Task<List<Oferta>> ObtenerOfertasActivas();
    }
}