
﻿using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using Microsoft.EntityFrameworkCore;

﻿using DocumentFormat.OpenXml.InkML;


namespace KAMBIO.CORE.Core.Services
{
    public class DisputaService : IDisputaService
    {
        private readonly IDisputaRepository _repository;
        private readonly KambioDbContext _context;

        public DisputaService(
            IDisputaRepository repository,

            KambioDbContext context) 

        {
            _repository = repository;
            _context = context;
        }
        public async Task<DisputaDTO> CrearDisputaAsync(CrearDisputaDto dto)
        {
            var nuevaDisputa = new Disputa
            {
                IdTransaccion = dto.IdTransaccion,
                IdUsuarioReporta = dto.IdUsuarioReporta,
                IdEstadoDisputa = 1, // Pendiente
                Descripcion = dto.Descripcion,
                FechaReporte = DateTime.Now
            };

            var creada = await _repository.CrearDisputaAsync(nuevaDisputa);

            return new DisputaDTO
            {
                IdDisputa = creada.IdDisputa,
                IdTransaccion = creada.IdTransaccion,
                UsuarioReportante = $"{creada.IdUsuarioReportaNavigation.Nombres} {creada.IdUsuarioReportaNavigation.Apellidos}",
                Estado = creada.IdEstadoDisputaNavigation.Nombre,
                Descripcion = creada.Descripcion,
                FechaReporte = creada.FechaReporte
            };
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

        public async Task<DetalleDisputaDTO?> ObtenerDisputaPorId(int id)
        {
            var disputa = await _repository.ObtenerDisputaPorId(id);
            if (disputa == null)
                return null;

            var transaccion = await _context.Transaccion
                .Include(t => t.IdUsuarioCompradorNavigation)
                .Include(t => t.IdUsuarioVendedorNavigation)
                .FirstOrDefaultAsync(t => t.IdTransaccion == disputa.IdTransaccion);

            string ladoReportante = "Desconocido";
            string comprador = "N/A";
            string vendedor = "N/A";
            int idComprador = 0;
            int idVendedor = 0;

            if (transaccion != null)
            {
                comprador = $"{transaccion.IdUsuarioCompradorNavigation.Nombres} {transaccion.IdUsuarioCompradorNavigation.Apellidos}";
                vendedor = $"{transaccion.IdUsuarioVendedorNavigation.Nombres} {transaccion.IdUsuarioVendedorNavigation.Apellidos}";
                idComprador = transaccion.IdUsuarioComprador;
                idVendedor = transaccion.IdUsuarioVendedor;

                if (disputa.IdUsuarioReporta == transaccion.IdUsuarioComprador)
                    ladoReportante = "Comprador";
                else if (disputa.IdUsuarioReporta == transaccion.IdUsuarioVendedor)
                    ladoReportante = "Vendedor";
            }

            return new DetalleDisputaDTO
            {
                IdDisputa = disputa.IdDisputa,
                IdTransaccion = disputa.IdTransaccion,
                IdUsuarioReporta = disputa.IdUsuarioReporta,
                UsuarioReportante = $"{disputa.IdUsuarioReportaNavigation.Nombres} {disputa.IdUsuarioReportaNavigation.Apellidos}",
                Estado = disputa.IdEstadoDisputaNavigation.Nombre,
                Descripcion = disputa.Descripcion,
                FechaReporte = disputa.FechaReporte,
                FechaResolucion = disputa.FechaResolucion,
                ResolucionDetalle = disputa.ResolucionDetalle,
                IdUsuarioComprador = idComprador,
                Comprador = comprador,
                IdUsuarioVendedor = idVendedor,
                Vendedor = vendedor,
                LadoReportante = ladoReportante
            };
        }

        public async Task<bool> ResolverDisputa(int id, ResolverDisputaDTO dto)
        {
            var disputa = await _repository.ObtenerDisputaPorId(id);
            if (disputa == null)
                return false;

            if (disputa.IdEstadoDisputa != 1)
                return false;

            disputa.IdEstadoDisputa = 2;
            disputa.FechaResolucion = DateTime.Now;
            disputa.IdAdminResolucion = dto.IdAdminResolucion;
            disputa.ResolucionDetalle = dto.ResolucionDetalle;

            await _repository.ActualizarDisputa();

            // Cancelar la transacción asociada y reabrir la oferta
            var transaccion = await _context.Transaccion.FindAsync(disputa.IdTransaccion);
            if (transaccion != null && transaccion.IdEstadoTransaccion != 4 && transaccion.IdEstadoTransaccion != 5)
            {
                transaccion.IdEstadoTransaccion = 5; // Cancelada
                transaccion.FechaCancelacion = DateTime.Now;

                var oferta = await _context.Oferta.FindAsync(transaccion.IdOferta);
                if (oferta != null)
                    oferta.IdEstadoOferta = 1; // Activa de nuevo

                await _context.SaveChangesAsync();
            }

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