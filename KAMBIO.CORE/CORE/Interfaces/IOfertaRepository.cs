using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IOfertaRepository
{
    Task<Oferta> CreateAsync(Oferta oferta);
    Task<Oferta?> GetByIdAsync(int id);
    Task<IEnumerable<Oferta>> GetByUsuarioAsync(int idUsuario);
    Task<IEnumerable<Oferta>> GetActivasAsync();
    Task<Banco?> GetBancoByIdAsync(int id);
}
