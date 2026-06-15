using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisputaController : ControllerBase
    {
        private readonly IDisputaService _service;

        public DisputaController(
            IDisputaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDisputas()
        {
            var resultado =
                await _service.ObtenerDisputas();

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerDisputa(int id)
        {
            var resultado =
                await _service.ObtenerDisputaPorId(id);

            if (resultado == null)
                return NotFound();

            return Ok(resultado);
        }

        [HttpPut("resolver/{id}")]
        public async Task<IActionResult> ResolverDisputa(
            int id,
            ResolverDisputaDTO dto)
        {
            var resultado =
                await _service.ResolverDisputa(id, dto);

            if (!resultado)
                return BadRequest();

            return Ok(
                "Disputa gestionada exitosamente.");
        }

        [HttpPut("rechazar/{id}")]
        public async Task<IActionResult> RechazarDisputa(
            int id,
            ResolverDisputaDTO dto)
        {
            var resultado =
                await _service.RechazarDisputa(id, dto);

            if (!resultado)
                return BadRequest();

            return Ok(
                "Disputa gestionada exitosamente.");
        }
    }
}