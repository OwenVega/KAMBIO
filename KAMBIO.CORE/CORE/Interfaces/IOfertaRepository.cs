using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaRepository
    {
        Task<Oferta?> ObtenerPorIdAsync(int idOferta);
        Task ActualizarAsync(Oferta oferta);
        Task<bool> TieneTransaccionEnCursoAsync(int idOferta);
    }
}
