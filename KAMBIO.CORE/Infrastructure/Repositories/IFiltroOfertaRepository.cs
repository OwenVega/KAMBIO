using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public interface IFiltroOfertaRepository
    {
        Task<List<Oferta>> FiltrarOfertasAsync(FiltroOfertaRequestDto filtro);
    }
}