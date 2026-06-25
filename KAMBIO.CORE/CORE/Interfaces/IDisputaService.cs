using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IDisputaService
    {
        Task<DisputaDTO> CrearDisputaAsync(CrearDisputaDto dto);
        Task<List<DisputaDTO>> ObtenerDisputas();

        Task<DetalleDisputaDTO?> ObtenerDisputaPorId(int id);

        Task<bool> ResolverDisputa(
            int id,
            ResolverDisputaDTO dto);

        Task<bool> RechazarDisputa(
            int id,
            ResolverDisputaDTO dto);
        
    }
}