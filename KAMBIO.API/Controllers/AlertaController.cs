using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertaController : ControllerBase
{
    private readonly IAlertaService _alertaService;

    public AlertaController(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearAlerta([FromBody] CrearAlertaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idUsuario = 1;
            var resultado = await _alertaService.CrearAlertaAsync(dto, idUsuario);
            return Ok(new { mensaje = "Alerta creada correctamente.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerAlertas()
    {
        var idUsuario = 1;
        var alertas = await _alertaService.ObtenerPorUsuarioAsync(idUsuario);
        return Ok(alertas);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarAlerta(int id, [FromBody] ActualizarAlertaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idUsuario = 1;
            var resultado = await _alertaService.ActualizarAlertaAsync(id, dto, idUsuario);
            return Ok(new { mensaje = "Alerta actualizada correctamente.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAlerta(int id)
    {
        try
        {
            var idUsuario = 1;
            await _alertaService.EliminarAlertaAsync(id, idUsuario);
            return Ok(new { mensaje = "Alerta eliminada correctamente." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
