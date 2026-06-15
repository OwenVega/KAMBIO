using KAMBIO.CORE.Core.Entities;

namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface ITransaccionRepository
    {
        Task<(List<Transaccion> Transacciones, int TotalRegistros)> ObtenerHistorialPaginadoAsync(int idUsuario, string busquedaDivisas, DateTime? fechaInicio, DateTime? fechaFin, string tipoOperacion, int? idEstado, int pagina, int cantidadPorPagina);
        Task<List<Transaccion>> ObtenerTransaccionesCompletadasDelMesAsync(int idUsuario, int mes, int anio);
    }
}