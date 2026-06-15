using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Core.DTOs.OfertaVenta;

namespace KAMBIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfertaVentaController : ControllerBase
    {
        private readonly IOfertaVentaService _ofertaVentaService;

        public OfertaVentaController(IOfertaVentaService ofertaVentaService)
        {
            _ofertaVentaService = ofertaVentaService;
        }

        [HttpPost]
        public async Task<IActionResult> CrearOfertaVenta(
            [FromBody] CrearOfertaVentaRequestDTO request)
        {
            try
            {
                var response =
                    await _ofertaVentaService.CrearOfertaVenta(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }
    }
}