using ClosedXML.Excel;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
using KAMBIO.CORE.CORE.Interfaces;

using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace KAMBIO.CORE.Core.Services
{
    public class ReporteService : IReporteService
    {
        private readonly KambioDbContext _context;

        public ReporteService(KambioDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReporteTransaccionDto>> ObtenerTransaccionesAsync(FiltroReporteDto filtro)
        {
            var query = _context.Transaccion
                .Include(t => t.IdUsuarioCompradorNavigation)
                .Include(t => t.IdUsuarioVendedorNavigation)
                .Include(t => t.IdEstadoTransaccionNavigation)
                .Include(t => t.IdDivisaOrigenNavigation)
                .Include(t => t.IdDivisaDestinoNavigation)
                .AsQueryable();

            if (filtro.FechaInicio.HasValue)
                query = query.Where(t => t.FechaInicio >= filtro.FechaInicio.Value);

            if (filtro.FechaFin.HasValue)
                query = query.Where(t => t.FechaInicio <= filtro.FechaFin.Value);

            if (filtro.IdDivisa.HasValue)
                query = query.Where(t => t.IdDivisaOrigen == filtro.IdDivisa.Value
                                      || t.IdDivisaDestino == filtro.IdDivisa.Value);

            if (filtro.IdUsuario.HasValue)
                query = query.Where(t => t.IdUsuarioComprador == filtro.IdUsuario.Value
                                      || t.IdUsuarioVendedor == filtro.IdUsuario.Value);

            return await query.Select(t => new ReporteTransaccionDto
            {
                IdTransaccion = t.IdTransaccion,
                Comprador = t.IdUsuarioCompradorNavigation.Nombres + " " + t.IdUsuarioCompradorNavigation.Apellidos,
                Vendedor = t.IdUsuarioVendedorNavigation.Nombres + " " + t.IdUsuarioVendedorNavigation.Apellidos,
                Monto = t.Monto,
                MontoEquivalente = t.MontoEquivalente,
                TasaCambioAplicada = t.TasaCambioAplicada,
                TipoOperacion = t.TipoOperacion,
                Estado = t.IdEstadoTransaccionNavigation.Nombre,
                DivisaOrigen = t.IdDivisaOrigenNavigation.Codigo,
                DivisaDestino = t.IdDivisaDestinoNavigation.Codigo,
                FechaInicio = t.FechaInicio
            }).ToListAsync();
        }

        public byte[] ExportarExcel(List<ReporteTransaccionDto> datos)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Transacciones");

            // Encabezados
            var headers = new[] { "ID", "Comprador", "Vendedor", "Monto", "Equivalente", "Tasa", "Tipo", "Estado", "Origen", "Destino", "Fecha" };
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cell(1, i + 1).Value = headers[i];
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            // Datos
            for (int i = 0; i < datos.Count; i++)
            {
                var d = datos[i];
                var row = i + 2;
                sheet.Cell(row, 1).Value = d.IdTransaccion;
                sheet.Cell(row, 2).Value = d.Comprador;
                sheet.Cell(row, 3).Value = d.Vendedor;
                sheet.Cell(row, 4).Value = d.Monto;
                sheet.Cell(row, 5).Value = d.MontoEquivalente;
                sheet.Cell(row, 6).Value = d.TasaCambioAplicada;
                sheet.Cell(row, 7).Value = d.TipoOperacion;
                sheet.Cell(row, 8).Value = d.Estado;
                sheet.Cell(row, 9).Value = d.DivisaOrigen;
                sheet.Cell(row, 10).Value = d.DivisaDestino;
                sheet.Cell(row, 11).Value = d.FechaInicio.ToString("dd/MM/yyyy");
            }

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportarPdf(List<ReporteTransaccionDto> datos)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(30);
                            c.RelativeColumn();
                            c.RelativeColumn();
                            c.ConstantColumn(60);
                            c.ConstantColumn(60);
                            c.ConstantColumn(50);
                            c.ConstantColumn(50);
                            c.ConstantColumn(70);
                            c.ConstantColumn(40);
                            c.ConstantColumn(40);
                            c.ConstantColumn(70);
                        });

                        var headers = new[] { "ID", "Comprador", "Vendedor", "Monto", "Equiv.", "Tasa", "Tipo", "Estado", "Orig.", "Dest.", "Fecha" };

                        table.Header(header =>
                        {
                            foreach (var h in headers)
                                header.Cell().Background("#2c3e50").Padding(4)
                                    .Text(h).FontColor("#ffffff").Bold().FontSize(8);
                        });

                        foreach (var d in datos)
                        {
                            var values = new[]
                            {
                        d.IdTransaccion.ToString(), d.Comprador, d.Vendedor,
                        d.Monto.ToString("N2"), d.MontoEquivalente.ToString("N2"),
                        d.TasaCambioAplicada.ToString("N4"), d.TipoOperacion,
                        d.Estado, d.DivisaOrigen, d.DivisaDestino,
                        d.FechaInicio.ToString("dd/MM/yy")
                    };
                            foreach (var v in values)
                                table.Cell().Padding(3).Text(v).FontSize(7);
                        }
                    });
                });
            });

            return doc.GeneratePdf();
        }
    }
}