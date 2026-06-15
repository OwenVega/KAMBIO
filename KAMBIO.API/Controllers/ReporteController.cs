using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        // GET: api/Reporte?fechaInicio=2024-01-01&fechaFin=2024-12-31&idDivisa=1&idUsuario=1
        [HttpGet]
        public async Task<IActionResult> ObtenerReporte([FromQuery] FiltroReporteDto filtro)
        {
            try
            {
                // Solo administradores (IdRol = 2) — validación básica por query param
                var datos = await _reporteService.ObtenerTransaccionesAsync(filtro);
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }

        // GET: api/Reporte/exportar-excel
        [HttpGet("exportar-excel")]
        public async Task<IActionResult> ExportarExcel([FromQuery] FiltroReporteDto filtro)
        {
            try
            {
                var datos = await _reporteService.ObtenerTransaccionesAsync(filtro);
                var archivo = _reporteService.ExportarExcel(datos);
                return File(archivo,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"reporte_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }

        // GET: api/Reporte/exportar-pdf
        [HttpGet("exportar-pdf")]
        public async Task<IActionResult> ExportarPdf([FromQuery] FiltroReporteDto filtro)
        {
            try
            {
                var datos = await _reporteService.ObtenerTransaccionesAsync(filtro);
                var archivo = _reporteService.ExportarPdf(datos);
                return File(archivo, "application/pdf",
                    $"reporte_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }
    }
}