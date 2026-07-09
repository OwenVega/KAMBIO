using System.Security.Claims;
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

        [HttpGet("historial")]
        public async Task<IActionResult> ObtenerHistorial([FromQuery] FiltroHistorialDTO filtro)
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int idUsuario = string.IsNullOrEmpty(claimId) ? 1 : int.Parse(claimId);
            var historial = await _transaccionService.ObtenerHistorialUsuarioAsync(idUsuario, filtro);
            return Ok(historial);
        }
        [HttpPost("desde-oferta/{idOferta}")]
        public async Task<IActionResult> CrearDesdeOferta(int idOferta, [FromHeader(Name = "X-Usuario-Id")] int? idUsuario)
        {
            if (idUsuario == null || idUsuario <= 0)
                return Unauthorized(new { mensaje = "Debe iniciar sesión para realizar esta acción." });

            try
            {
                var resultado = await _transaccionService.CrearTransaccionDesdeOfertaAsync(idOferta, idUsuario.Value);
                return Ok(new { mensaje = "Transacción iniciada correctamente.", transaccion = resultado });
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
        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerActivas([FromHeader(Name = "X-Usuario-Id")] int? idUsuario)
        {
            if (idUsuario == null || idUsuario <= 0)
                return Unauthorized(new { mensaje = "Debe iniciar sesión para ver sus transacciones." });

            try
            {
                var resultado = await _transaccionService.ObtenerTransaccionesActivasAsync(idUsuario.Value);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }
    }
}