using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaService
    {
        Task EjecutarMatchingAutomaticoAsync(Oferta nuevaOferta);
        Task<IEnumerable<MatchSugeridoDto>> ObtenerMatchesSugeridosAsync(int idUsuario);
        Task ProcesarRespuestaMatchAsync(RespuestaMatchDto respuesta);
    }
}