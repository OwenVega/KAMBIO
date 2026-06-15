using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IDisputaRepository
    {
        Task<List<Disputa>> ObtenerDisputas();

        Task<Disputa?> ObtenerDisputaPorId(int id);

        Task ActualizarDisputa();
    }
}