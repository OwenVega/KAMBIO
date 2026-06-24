using System;
using System.Collections.Generic;
using System.Text;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Infrastructure.Repositories
{
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly KambioDbContext _context;

        public TransaccionRepository(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Transaccion> Transacciones, int TotalRegistros)> ObtenerHistorialPaginadoAsync(
            int idUsuario,
            string busquedaDivisas,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string tipoOperacion,
            int? idEstado,
            int pagina,
            int cantidadPorPagina)
        {
            var query = _context.Transaccion
                .Include(t => t.IdDivisaOrigenNavigation)
                .Include(t => t.IdDivisaDestinoNavigation)
                .Include(t => t.IdEstadoTransaccionNavigation)
                .Where(t => t.IdUsuarioComprador == idUsuario || t.IdUsuarioVendedor == idUsuario)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busquedaDivisas))
            {
                query = query.Where(t => (t.IdDivisaOrigenNavigation.Codigo + "/" + t.IdDivisaDestinoNavigation.Codigo).Contains(busquedaDivisas));
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(t => t.FechaInicio >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(t => t.FechaInicio <= fechaFin.Value);
            }

            if (!string.IsNullOrWhiteSpace(tipoOperacion))
            {
                query = query.Where(t => t.TipoOperacion == tipoOperacion);
            }

            if (idEstado.HasValue && idEstado.Value > 0)
            {
                query = query.Where(t => t.IdEstadoTransaccion == idEstado.Value);
            }

            int totalRegistros = await query.CountAsync();

            var transacciones = await query
                .OrderByDescending(t => t.FechaInicio)
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .AsNoTracking()
                .ToListAsync();

            return (transacciones, totalRegistros);
        }

        public async Task<List<Transaccion>> ObtenerTransaccionesCompletadasDelMesAsync(int idUsuario, int mes, int anio)
        {
            return await _context.Transaccion
                .Include(t => t.IdDivisaOrigenNavigation)
                .Where(t => (t.IdUsuarioComprador == idUsuario || t.IdUsuarioVendedor == idUsuario)
                         && t.IdEstadoTransaccion == 4
                         && t.FechaCompletado.HasValue
                         && t.FechaCompletado.Value.Month == mes
                         && t.FechaCompletado.Value.Year == anio)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Transaccion> CrearAsync(Transaccion transaccion)
        {
            _context.Transaccion.Add(transaccion);
            await _context.SaveChangesAsync();

            return await _context.Transaccion
                .Include(t => t.IdDivisaOrigenNavigation)
                .Include(t => t.IdDivisaDestinoNavigation)
                .Include(t => t.IdEstadoTransaccionNavigation)
                .Include(t => t.IdUsuarioCompradorNavigation)
                .Include(t => t.IdUsuarioVendedorNavigation)
                .FirstAsync(t => t.IdTransaccion == transaccion.IdTransaccion);
        }
    }
}
