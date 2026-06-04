using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaRepository
    {
        Task ActualizarEstadoMatchAsync(int idMatch, int idUsuario, bool aceptado);
        Task CrearMatchSugeridoAsync(MatchOferta nuevoMatch);
        Task<IEnumerable<Oferta>> EncontrarOfertasCompatiblesAsync(Oferta ofertaActual);
        Task<IEnumerable<MatchOferta>> ObtenerMatchesSugeridosPorUsuarioIdAsync(int idUsuario);
    }
}