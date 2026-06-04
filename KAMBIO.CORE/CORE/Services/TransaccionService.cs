using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Core.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly KambioDbContext _context;

        public TransaccionService(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<TransaccionDetalleDto> ObtenerPorIdAsync(int idTransaccion)
        {
            var t = await _context.Transaccion
                .Include(x => x.IdEstadoTransaccionNavigation)
                .FirstOrDefaultAsync(x => x.IdTransaccion == idTransaccion)
                ?? throw new InvalidOperationException("Transacción no encontrada.");

            return new TransaccionDetalleDto
            {
                IdTransaccion = t.IdTransaccion,
                IdOferta = t.IdOferta,
                Monto = t.Monto,
                MontoEquivalente = t.MontoEquivalente,
                TasaCambioAplicada = t.TasaCambioAplicada,
                TipoOperacion = t.TipoOperacion,
                EstadoNombre = t.IdEstadoTransaccionNavigation.Nombre,
                FechaInicio = t.FechaInicio,
                FechaConfirmacionPago = t.FechaConfirmacionPago,
                FechaCompletado = t.FechaCompletado,
                ConfirmadoPorComprador = t.ConfirmadoPorComprador,
                ConfirmadoPorVendedor = t.ConfirmadoPorVendedor
            };
        }

        public async Task CambiarEstadoAsync(CambiarEstadoDto dto)
        {
            var t = await _context.Transaccion.FindAsync(dto.IdTransaccion)
                ?? throw new InvalidOperationException("Transacción no encontrada.");

            // Transiciones válidas US-009
            // 1=Pendiente, 2=En Proceso, 3=Completada, 4=Cancelada
            var transicionesValidas = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 2, 5 } },   // Pendiente → En Proceso o Cancelada
                { 2, new List<int> { 3, 5 } },   // En Proceso → Pago Realizado o Cancelada
                { 3, new List<int> { 4, 6 } },   // Pago Realizado → Completada o En Disputa
            };

            if (transicionesValidas.ContainsKey(t.IdEstadoTransaccion) &&
                !transicionesValidas[t.IdEstadoTransaccion].Contains(dto.IdEstadoTransaccion))
                throw new InvalidOperationException(
                    "Transición de estado no permitida.");

            if (dto.IdEstadoTransaccion == 4) // Completada
                t.FechaCompletado = DateTime.Now;
            else if (dto.IdEstadoTransaccion == 5) // Cancelada
                t.FechaCancelacion = DateTime.Now;

            t.IdEstadoTransaccion = dto.IdEstadoTransaccion;

            // Registrar en historial para trazabilidad
            var historial = new HistorialEstadoTransaccion
            {
                IdTransaccion = dto.IdTransaccion,
                IdEstadoTransaccion = dto.IdEstadoTransaccion,
                IdUsuarioCambio = dto.IdUsuarioCambio,
                Observacion = dto.Observacion,
                FechaCambio = DateTime.Now
            };
            _context.HistorialEstadoTransaccion.Add(historial);

            await _context.SaveChangesAsync();
        }
    }
}