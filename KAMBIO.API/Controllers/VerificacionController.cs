// CONTROLADOR de Verificación (API)
// Expone los endpoints para que el usuario solicite verificación
// y el admin pueda revisar las solicitudes pendientes.
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

    // POST /api/verificacion/solicitar - El usuario envía su foto DNI para verificación
    // Body: { "rutaImagen": "fotos/dni_123.jpg" }
    [HttpPost("solicitar")]
    public async Task<IActionResult> Solicitar([FromBody] SolicitarVerificacionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idUsuario = 1;  // Temporal: después será el usuario logueado
            var resultado = await _verificacionService.SolicitarVerificacionAsync(dto, idUsuario);
            return Ok(new { mensaje = "Solicitud de verificación enviada.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT /api/verificacion/revisar - El admin aprueba o rechaza
    // Body: { "idVerificacion": 1, "idEstadoVerificacion": 2, "observacionAdmin": "DNI válido" }
    [HttpPut("revisar")]
    public async Task<IActionResult> Revisar([FromBody] RevisarVerificacionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idAdmin = 1;  // Temporal: después será el admin logueado
            var resultado = await _verificacionService.RevisarVerificacionAsync(dto, idAdmin);
            return Ok(new { mensaje = "Solicitud revisada correctamente.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // GET /api/verificacion/pendientes - Admin ve todas las solicitudes sin revisar
    [HttpGet("pendientes")]
    public async Task<IActionResult> ObtenerPendientes()
    {
        var pendientes = await _verificacionService.ObtenerPendientesAsync();
        return Ok(pendientes);
    }

    // GET /api/verificacion/{id} - Ver detalle de una solicitud específica
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var resultado = await _verificacionService.ObtenerPorIdAsync(id);
        if (resultado == null)
            return NotFound(new { mensaje = "Solicitud no encontrada." });
        return Ok(resultado);
    }
}
