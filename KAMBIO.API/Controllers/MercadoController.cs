using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Services;
using KAMBIO.CORE.CORE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KAMBIO.CORE.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MercadoController : ControllerBase
    {
        private readonly IOfertaService _ofertaService;

        public MercadoController(IOfertaService ofertaService)
        {
            _ofertaService = ofertaService;
        }

        [HttpGet("ofertas")]
        public async Task<IActionResult> ObtenerOfertas([FromQuery] FiltroOfertaDTO filtro)
        {
            var ofertas = await _ofertaService.ObtenerOfertasMercadoAsync(filtro);
            return Ok(ofertas);
        }
    }
}