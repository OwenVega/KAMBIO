using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionController : ControllerBase
    {
        private readonly ICalificacionService _calificacionService;

        public CalificacionController(ICalificacionService calificacionService)
        {
            _calificacionService = calificacionService;
        }

        [HttpPost]
        public async Task<IActionResult> Calificar([FromBody] CalificacionDto dto)
        {
            try
            {
                await _calificacionService.CalificarAsync(dto);
                return Ok(new { mensaje = "Calificación registrada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> ObtenerPromedio(int idUsuario)
        {
            try
            {
                var resultado = await _calificacionService.ObtenerPromedioAsync(idUsuario);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }
    }
}