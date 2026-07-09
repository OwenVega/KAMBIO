using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace KAMBIO.CORE.Core.Services
{
    public class TransaccionService : ITransaccionService
    {
        private readonly KambioDbContext _context;
        private readonly ITransaccionRepository _transaccionRepository;
        private readonly INotificacionService _notificacionService;

        public TransaccionService(KambioDbContext context, ITransaccionRepository transaccionRepository, INotificacionService notificacionService)
        {
            _context = context;
            _transaccionRepository = transaccionRepository;
            _notificacionService = notificacionService;
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
                IdUsuarioComprador = t.IdUsuarioComprador,
                IdUsuarioVendedor = t.IdUsuarioVendedor,
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

            var transicionesValidas = new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 2, 5 } },
                { 2, new List<int> { 3, 5 } },
                { 3, new List<int> { 4, 6 } },
            };

            if (transicionesValidas.ContainsKey(t.IdEstadoTransaccion) &&
                !transicionesValidas[t.IdEstadoTransaccion].Contains(dto.IdEstadoTransaccion))
                throw new InvalidOperationException("Transición de estado no permitida.");

            if (dto.IdEstadoTransaccion == 4)
            {
                t.FechaCompletado = DateTime.Now;

                // Si se completa la transacción, la oferta también se marca como Completada
                var ofertaCompletada = await _context.Oferta.FindAsync(t.IdOferta);
                if (ofertaCompletada != null)
                {
                    ofertaCompletada.IdEstadoOferta = 3; // Completada
                    _context.Oferta.Update(ofertaCompletada);
                }
            }
            else if (dto.IdEstadoTransaccion == 5)
            {
                t.FechaCancelacion = DateTime.Now;

                // Si se cancela la transacción, la oferta vuelve a estar Activa
                var oferta = await _context.Oferta.FindAsync(t.IdOferta);
                if (oferta != null)
                {
                    oferta.IdEstadoOferta = 1; // Activa
                    _context.Oferta.Update(oferta);
                }
            }

            t.IdEstadoTransaccion = dto.IdEstadoTransaccion;

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

            // Notificar a la otra parte sobre el cambio de estado
            var idOtraParteNotificar = dto.IdUsuarioCambio == t.IdUsuarioComprador ? t.IdUsuarioVendedor : t.IdUsuarioComprador;
            var nombreEstado = dto.IdEstadoTransaccion switch
            {
                2 => "En Proceso",
                3 => "Pago Realizado",
                4 => "Completada",
                5 => "Cancelada",
                6 => "En Disputa",
                _ => "Actualizada"
            };
            await _notificacionService.CrearNotificacionAsync(
                idOtraParteNotificar,
                "Transacción actualizada",
                $"La transacción #{t.IdTransaccion} ha cambiado de estado a: {nombreEstado}.",
                t.IdTransaccion,
                "Transaccion"
            );
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
                    volumenUsd += t.Monto;
                else if (t.IdDivisaDestino == 1)
                    volumenUsd += t.MontoEquivalente;

                if (t.FechaCompletado.HasValue)
                    tiempoTotalMinutos += (t.FechaCompletado.Value - t.FechaInicio).TotalMinutes;
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
        public async Task<TransaccionDetalleDto> CrearTransaccionDesdeOfertaAsync(int idOferta, int idUsuarioComprador)
        {
            var oferta = await _context.Oferta
                .Include(o => o.IdDivisaOrigenNavigation)
                .Include(o => o.IdDivisaDestinoNavigation)
                .FirstOrDefaultAsync(o => o.IdOferta == idOferta)
                ?? throw new InvalidOperationException("La oferta no existe.");

            if (oferta.IdEstadoOferta != 1)
                throw new InvalidOperationException("Esta oferta ya no está disponible.");

            if (oferta.IdUsuario == idUsuarioComprador)
                throw new InvalidOperationException("No puedes iniciar una transacción con tu propia oferta.");

            int idUsuarioComprara;
            int idUsuarioVende;

            if (oferta.IdTipoOferta == 1)
            {
                idUsuarioComprara = oferta.IdUsuario;
                idUsuarioVende = idUsuarioComprador;
            }
            else
            {
                idUsuarioComprara = idUsuarioComprador;
                idUsuarioVende = oferta.IdUsuario;
            }

            var monto = oferta.MontoMinimo;
            var montoEquivalente = Math.Round(monto * oferta.TasaCambio, 2);

            var nuevaTransaccion = new Transaccion
            {
                IdOferta = oferta.IdOferta,
                IdUsuarioComprador = idUsuarioComprara,
                IdUsuarioVendedor = idUsuarioVende,
                IdEstadoTransaccion = 1,
                IdDivisaOrigen = oferta.IdDivisaOrigen,
                IdDivisaDestino = oferta.IdDivisaDestino,
                Monto = monto,
                MontoEquivalente = montoEquivalente,
                TasaCambioAplicada = oferta.TasaCambio,
                TipoOperacion = oferta.IdTipoOferta == 1 ? "Compra" : "Venta",
                FechaInicio = DateTime.Now,
                ConfirmadoPorComprador = false,
                ConfirmadoPorVendedor = false
            };

            oferta.IdEstadoOferta = 4; // Emparejada
            _context.Oferta.Update(oferta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Esta oferta acaba de ser tomada por otro usuario. Por favor, elige otra oferta.");
            }

            var creada = await _transaccionRepository.CrearAsync(nuevaTransaccion);

            var idDuenoOferta = oferta.IdUsuario;
            var idOtraParte = idDuenoOferta == idUsuarioComprara ? idUsuarioVende : idUsuarioComprara;
            await _notificacionService.CrearNotificacionAsync(
                idDuenoOferta,
                "¡Oferta aceptada!",
                $"Un usuario ha aceptado tu oferta de {monto} {oferta.IdDivisaOrigenNavigation?.Codigo}. Procede con la transacción.",
                creada.IdTransaccion,
                "Transaccion"
            );

            return new TransaccionDetalleDto
            {
                IdTransaccion = creada.IdTransaccion,
                IdOferta = creada.IdOferta,
                IdUsuarioComprador = creada.IdUsuarioComprador,
                IdUsuarioVendedor = creada.IdUsuarioVendedor,
                Monto = creada.Monto,
                MontoEquivalente = creada.MontoEquivalente,
                TasaCambioAplicada = creada.TasaCambioAplicada,
                TipoOperacion = creada.TipoOperacion,
                EstadoNombre = "Pendiente",
                FechaInicio = creada.FechaInicio,
                FechaConfirmacionPago = creada.FechaConfirmacionPago,
                FechaCompletado = creada.FechaCompletado,
                ConfirmadoPorComprador = creada.ConfirmadoPorComprador,
                ConfirmadoPorVendedor = creada.ConfirmadoPorVendedor
            };
        }
    }
}