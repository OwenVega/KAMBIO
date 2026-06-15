using System;
using System.Threading.Tasks;
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

        [HttpGet("{idUsuario}/matches")]
        public async Task<IActionResult> ObtenerMatchesSugeridos(int idUsuario)
        {
            try
            {
                var matches = await _ofertaService.ObtenerMatchesSugeridosAsync(idUsuario);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al obtener las coincidencias sugeridas.", detalle = ex.Message });
            }
        }

        [HttpPost("match/respuesta")]
        public async Task<IActionResult> ResponderMatch([FromBody] RespuestaMatchDto respuesta)
        {
            try
            {
                await _ofertaService.ProcesarRespuestaMatchAsync(respuesta);
                string accion = respuesta.Aceptado ? "aceptada" : "rechazada";
                return Ok(new { mensaje = $"La coincidencia fue {accion} correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al procesar la respuesta del match.", detalle = ex.Message });
            }
        }

        [HttpPost("compra")]
        public async Task<IActionResult> CrearOfertaCompra(
            [FromHeader(Name = "X-Usuario-Id")] int? idUsuario,
            [FromBody] CrearOfertaCompraRequestDTO dto)
        {
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

        [HttpPut("cancelar")]
        public async Task<IActionResult> CancelarOferta([FromBody] CancelacionOfertaDto dto)
        {
            try
            {
                await _ofertaService.CancelarOfertaAsync(dto.IdOferta, dto.IdUsuario);
                return Ok(new { mensaje = "Oferta cancelada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}