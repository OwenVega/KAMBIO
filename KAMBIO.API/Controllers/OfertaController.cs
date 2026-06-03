// CONTROLADOR de Ofertas (API)
// Endpoint para publicar una oferta de compra o venta de divisas.
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

    // POST /api/oferta - Publicar una oferta con montos mínimo y máximo
    // Body: {
    //   "idDivisaOrigen": 1, "idDivisaDestino": 2,
    //   "montoDisponible": 1000, "montoMinimo": 50, "montoMaximo": 500,
    //   "tasaCambio": 3.75, "idTipoOferta": 1, "idsBancos": [1, 2]
    // }
    [HttpPost]
    public async Task<IActionResult> CrearOferta([FromBody] CrearOfertaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idUsuario = 1;  // Temporal: después será el usuario logueado
            var resultado = await _ofertaService.CrearOfertaAsync(dto, idUsuario);
            return Ok(new { mensaje = "Oferta publicada correctamente.", oferta = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
