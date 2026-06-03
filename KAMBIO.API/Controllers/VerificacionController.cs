using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VerificacionController : ControllerBase
{
    private readonly IVerificacionService _verificacionService;

    public VerificacionController(IVerificacionService verificacionService)
    {
        _verificacionService = verificacionService;
    }

    [HttpPost("solicitar")]
    public async Task<IActionResult> Solicitar([FromBody] SolicitarVerificacionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idUsuario = 1;
            var resultado = await _verificacionService.SolicitarVerificacionAsync(dto, idUsuario);
            return Ok(new { mensaje = "Solicitud de verificación enviada.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("revisar")]
    public async Task<IActionResult> Revisar([FromBody] RevisarVerificacionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idAdmin = 1;
            var resultado = await _verificacionService.RevisarVerificacionAsync(dto, idAdmin);
            return Ok(new { mensaje = "Solicitud revisada correctamente.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("pendientes")]
    public async Task<IActionResult> ObtenerPendientes()
    {
        var pendientes = await _verificacionService.ObtenerPendientesAsync();
        return Ok(pendientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _verificacionService.ObtenerPorIdAsync(id);
        if (resultado == null)
            return NotFound(new { mensaje = "Solicitud no encontrada." });
        return Ok(resultado);
    }
}
