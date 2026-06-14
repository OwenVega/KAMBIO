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