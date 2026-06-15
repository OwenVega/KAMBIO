using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Core.Services
{
    public class CalificacionService : ICalificacionService
    {
        private readonly KambioDbContext _context;

        public CalificacionService(KambioDbContext context)
        {
            _context = context;
        }

        public async Task CalificarAsync(CalificacionDto dto)
        {
            var transaccion = await _context.Transaccion.FindAsync(dto.IdTransaccion)
                ?? throw new InvalidOperationException("Transacción no encontrada.");

            if (transaccion.IdEstadoTransaccion != 4) // 4 = Completada
                throw new InvalidOperationException(
                    "Solo puedes calificar transacciones completadas.");

            var tieneDisputa = await _context.Disputa
                .AnyAsync(d => d.IdTransaccion == dto.IdTransaccion && d.IdEstadoDisputa == 1);
            if (tieneDisputa)
                throw new InvalidOperationException(
                    "No puedes calificar una transacción con disputa abierta.");

            if (dto.Estrellas < 1 || dto.Estrellas > 5)
                throw new InvalidOperationException("Las estrellas deben estar entre 1 y 5.");

            var calificacion = new Calificacion
            {
                IdTransaccion = dto.IdTransaccion,
                IdUsuarioEvalua = dto.IdUsuarioEvalua,
                IdUsuarioEvaluado = dto.IdUsuarioEvaluado,
                Estrellas = dto.Estrellas,
                Comentario = dto.Comentario,
                FechaCalificacion = DateTime.Now
            };
            _context.Calificacion.Add(calificacion);
            await _context.SaveChangesAsync();

            // Recalcular promedio en tabla Usuario
            await RecalcularPromedioAsync(dto.IdUsuarioEvaluado);
        }

        public async Task<PromedioCalificacionDto> ObtenerPromedioAsync(int idUsuario)
        {
            var cals = await _context.Calificacion
                .Where(c => c.IdUsuarioEvaluado == idUsuario)
                .ToListAsync();

            return new PromedioCalificacionDto
            {
                IdUsuario = idUsuario,
                Promedio = cals.Any() ? Math.Round(cals.Average(c => c.Estrellas), 2) : 0,
                TotalCalificaciones = cals.Count
            };
        }

        private async Task RecalcularPromedioAsync(int idUsuario)
        {
            var promedio = await _context.Calificacion
                .Where(c => c.IdUsuarioEvaluado == idUsuario)
                .AverageAsync(c => (double?)c.Estrellas) ?? 0;

            var usuario = await _context.Usuario.FindAsync(idUsuario);
            if (usuario != null)
            {
                usuario.CalificacionPromedio = (decimal)Math.Round(promedio, 2);
                await _context.SaveChangesAsync();
            }
        }
    }
}