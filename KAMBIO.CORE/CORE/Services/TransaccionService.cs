using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Infrastructure.Repositories;

namespace KAMBIO.CORE.Core.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly ITransaccionRepository _transaccionRepository;
        public TransaccionService(ITransaccionRepository transaccionRepository)
        {
            _transaccionRepository = transaccionRepository;
        }

        public async Task<HistorialPaginadoDTO> ObtenerHistorialUsuarioAsync(int idUsuario, FiltroHistorialDTO filtro)
        {
            var resultadoRepo = await _transaccionRepository.ObtenerHistorialPaginadoAsync(
                idUsuario,
                filtro.BusquedaDivisas,
                filtro.FechaInicio,
                filtro.FechaFin,
                filtro.TipoOperacion,
                filtro.IdEstado,
                filtro.Pagina,
                filtro.CantidadPorPagina
            );

            var fechaActual = DateTime.Now;
            var transaccionesMes = await _transaccionRepository.ObtenerTransaccionesCompletadasDelMesAsync(
                idUsuario,
                fechaActual.Month,
                fechaActual.Year
            );

            decimal volumenUsd = 0;
            double tiempoTotalMinutos = 0;
            int exitosas = transaccionesMes.Count;

            foreach (var t in transaccionesMes)
            {
                if (t.IdDivisaOrigen == 1)
                {
                    volumenUsd += t.Monto;
                }
                else if (t.IdDivisaDestino == 1)
                {
                    volumenUsd += t.MontoEquivalente;
                }

                if (t.FechaCompletado.HasValue)
                {
                    tiempoTotalMinutos += (t.FechaCompletado.Value - t.FechaInicio).TotalMinutes;
                }
            }

            double tiempoPromedio = exitosas > 0 ? tiempoTotalMinutos / exitosas : 0;

            var transaccionesDto = resultadoRepo.Transacciones.Select(t => new TransaccionHistorialDTO
            {
                IdTransaccion = t.IdTransaccion,
                FechaOperacion = t.FechaInicio.ToString("dd MMM, yyyy HH:mm:ss", new CultureInfo("es-ES")),
                ParDivisas = $"{t.IdDivisaOrigenNavigation.Codigo}/{t.IdDivisaDestinoNavigation.Codigo}",
                Tipo = t.TipoOperacion,
                MontoOrigen = Math.Round(t.Monto, 2),
                MontoDestino = Math.Round(t.MontoEquivalente, 2),
                Estado = t.IdEstadoTransaccionNavigation.Nombre
            }).ToList();

            int totalPaginas = (int)Math.Ceiling((double)resultadoRepo.TotalRegistros / filtro.CantidadPorPagina);

            return new HistorialPaginadoDTO
            {
                Resumen = new ResumenHistorialDTO
                {
                    VolumenMensualUSD = Math.Round(volumenUsd, 2),
                    OperacionesExitosas = exitosas,
                    TiempoPromedioMinutos = Math.Round(tiempoPromedio, 2)
                },
                Transacciones = transaccionesDto,
                TotalRegistros = resultadoRepo.TotalRegistros,
                PaginaActual = filtro.Pagina,
                TotalPaginas = totalPaginas
            };
        }
    }
}