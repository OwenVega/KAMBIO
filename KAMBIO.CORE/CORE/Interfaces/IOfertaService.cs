using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaService
    {
        Task<List<OfertaP2PDTO>> ObtenerOfertasMercadoAsync(FiltroOfertaDTO filtro);
    }
}