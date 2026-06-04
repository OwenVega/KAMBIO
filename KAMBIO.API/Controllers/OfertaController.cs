using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OfertaController : ControllerBase
    {
        private readonly IOfertaService _ofertaService;

        public OfertaController(IOfertaService ofertaService)
        {
            _ofertaService = ofertaService;
        }

        // POST api/oferta/compra
        // Header requerido: X-Usuario-Id (simula sesión autenticada)
        [HttpPost("compra")]
        public async Task<IActionResult> CrearOfertaCompra(
            [FromHeader(Name = "X-Usuario-Id")] int? idUsuario,
            [FromBody] CrearOfertaCompraRequestDTO dto)
        {
            // Validar sesión activa — si no hay header, acceso denegado
            if (idUsuario == null || idUsuario <= 0)
                return Unauthorized(new { mensaje = "Acceso denegado. Debe iniciar sesión para publicar una oferta." });

            try
            {
                var resultado = await _ofertaService.CrearOfertaCompra(idUsuario.Value, dto);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno en el servidor.", detalle = ex.Message });
            }
        }

        // GET api/oferta/activas
        [HttpGet("activas")]
        public async Task<IActionResult> ObtenerOfertasActivas()
        {
            try
            {
                var ofertas = await _ofertaService.ObtenerOfertasActivas();
                return Ok(ofertas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno en el servidor.", detalle = ex.Message });
            }
        }
    }
}