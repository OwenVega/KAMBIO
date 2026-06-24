using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaRepository
    {
        Task ActualizarEstadoMatchAsync(int idMatch, int idUsuario, bool aceptado);
        Task CrearMatchSugeridoAsync(MatchOferta nuevoMatch);
        Task<IEnumerable<Oferta>> EncontrarOfertasCompatiblesAsync(Oferta ofertaActual);
        Task<IEnumerable<MatchOferta>> ObtenerMatchesSugeridosPorUsuarioIdAsync(int idUsuario);
        Task<Oferta> CrearOfertaCompra(Oferta oferta, List<int> idBancos);
        Task<List<Oferta>> ObtenerOfertasActivas();
        Task<List<Oferta>> ObtenerOfertasFiltradasAsync(int idTipoOferta, int idDivisaOrigen, int idDivisaDestino, decimal? monto, int? idBanco);
        Task<Oferta?> ObtenerPorIdAsync(int idOferta);
        Task ActualizarAsync(Oferta oferta);
        Task<bool> TieneTransaccionEnCursoAsync(int idOferta);

        // Métodos para CrearOfertaAsync (US-022)
        Task<Oferta> CreateAsync(Oferta oferta);
        Task<Banco?> GetBancoByIdAsync(int id);
    }
}