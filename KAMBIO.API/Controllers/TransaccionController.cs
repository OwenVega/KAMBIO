using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;

        public TransaccionController(ITransaccionService transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var resultado = await _transaccionService.ObtenerPorIdAsync(id);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }

        [HttpPut("cambiar-estado")]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoDto dto)
        {
            try
            {
                await _transaccionService.CambiarEstadoAsync(dto);
                return Ok(new { mensaje = "Estado actualizado correctamente." });
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
    }
}