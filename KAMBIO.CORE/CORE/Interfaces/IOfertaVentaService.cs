using KAMBIO.CORE.Core.DTOs.OfertaVenta;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaVentaService
    {
        Task<OfertaVentaResponseDTO> CrearOfertaVenta(CrearOfertaVentaRequestDTO request);
    }
}