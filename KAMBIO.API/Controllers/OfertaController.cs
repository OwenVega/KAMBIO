using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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
    }
}