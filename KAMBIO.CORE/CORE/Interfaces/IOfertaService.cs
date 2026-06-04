using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaService
    {
        Task<OfertaResponseDTO> CrearOfertaCompra(int idUsuario, CrearOfertaCompraRequestDTO dto);
        Task<List<OfertaResponseDTO>> ObtenerOfertasActivas();
    }
}