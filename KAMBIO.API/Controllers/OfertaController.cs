using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfertaController : ControllerBase
{
    private readonly IOfertaService _ofertaService;

    public OfertaController(IOfertaService ofertaService)
    {
        _ofertaService = ofertaService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearOferta([FromBody] CrearOfertaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idUsuario = 1;
            var resultado = await _ofertaService.CrearOfertaAsync(dto, idUsuario);
            return Ok(new { mensaje = "Oferta publicada correctamente.", oferta = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
