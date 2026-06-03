// INTERFAZ del Repositorio de Ofertas
// Define métodos para guardar y consultar ofertas en la BD.
using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IOfertaRepository
{
    Task<Oferta> CreateAsync(Oferta oferta);                    // Publicar una oferta
    Task<Oferta?> GetByIdAsync(int id);                         // Buscar oferta por ID
    Task<IEnumerable<Oferta>> GetByUsuarioAsync(int idUsuario); // Ofertas de un usuario
    Task<IEnumerable<Oferta>> GetActivasAsync();                // Todas las ofertas activas
    Task<Banco?> GetBancoByIdAsync(int id);                     // Verificar si un banco existe
}
