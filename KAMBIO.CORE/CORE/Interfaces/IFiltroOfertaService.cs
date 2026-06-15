using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IFiltroOfertaService
    {
        Task<FiltroOfertaResponseDto> ObtenerOfertasFiltradasAsync(FiltroOfertaRequestDto filtro);
    }
}