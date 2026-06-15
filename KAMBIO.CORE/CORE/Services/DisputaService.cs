using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Services
{
    public class DisputaService : IDisputaService
    {
        private readonly IDisputaRepository _repository;

        public DisputaService(
            IDisputaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DisputaDTO>> ObtenerDisputas()
        {
            var lista =
                await _repository.ObtenerDisputas();

            return lista.Select(d => new DisputaDTO
            {
                IdDisputa = d.IdDisputa,

                IdTransaccion = d.IdTransaccion,

                UsuarioReportante =
                    d.IdUsuarioReportaNavigation.Nombres
                    + " "
                    + d.IdUsuarioReportaNavigation.Apellidos,

                Estado =
                    d.IdEstadoDisputaNavigation.Nombre,

                Descripcion =
                    d.Descripcion,

                FechaReporte =
                    d.FechaReporte

            }).ToList();
        }

        public async Task<DetalleDisputaDTO?>
            ObtenerDisputaPorId(int id)
        {
            var disputa =
                await _repository.ObtenerDisputaPorId(id);

            if (disputa == null)
                return null;

            return new DetalleDisputaDTO
            {
                IdDisputa = disputa.IdDisputa,

                IdTransaccion = disputa.IdTransaccion,

                UsuarioReportante =
                    disputa.IdUsuarioReportaNavigation.Nombres
                    + " "
                    + disputa.IdUsuarioReportaNavigation.Apellidos,

                Estado =
                    disputa.IdEstadoDisputaNavigation.Nombre,

                Descripcion =
                    disputa.Descripcion,

                FechaReporte =
                    disputa.FechaReporte,

                FechaResolucion =
                    disputa.FechaResolucion,

                ResolucionDetalle =
                    disputa.ResolucionDetalle
            };
        }

        public async Task<bool> ResolverDisputa(
            int id,
            ResolverDisputaDTO dto)
        {
            var disputa =
                await _repository.ObtenerDisputaPorId(id);

            if (disputa == null)
                return false;

            if (disputa.IdEstadoDisputa != 1)
                return false;

            disputa.IdEstadoDisputa = 2;

            disputa.FechaResolucion =
                DateTime.Now;

            disputa.IdAdminResolucion =
                dto.IdAdminResolucion;

            disputa.ResolucionDetalle =
                dto.ResolucionDetalle;

            await _repository.ActualizarDisputa();

            return true;
        }

        public async Task<bool> RechazarDisputa(
            int id,
            ResolverDisputaDTO dto)
        {
            var disputa =
                await _repository.ObtenerDisputaPorId(id);

            if (disputa == null)
                return false;

            if (disputa.IdEstadoDisputa != 1)
                return false;

            disputa.IdEstadoDisputa = 3;

            disputa.FechaResolucion =
                DateTime.Now;

            disputa.IdAdminResolucion =
                dto.IdAdminResolucion;

            disputa.ResolucionDetalle =
                dto.ResolucionDetalle;

            await _repository.ActualizarDisputa();

            return true;
        }
    }
}