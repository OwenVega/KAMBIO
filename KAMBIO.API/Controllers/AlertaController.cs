// CONTROLADOR de Alertas (API)
// Esta es la capa que recibe las peticiones HTTP del frontend (navegador, app, Postman).
// Expone "endpoints" (URLs) que el frontend puede llamar.
using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.API.Controllers;

// [ApiController] = esta clase es un controlador de API
// [Route("api/[controller]")] = la URL base es /api/alerta
[ApiController]
[Route("api/[controller]")]
public class AlertaController : ControllerBase
{
    // Llamamos al servicio para la lógica de negocio
    private readonly IAlertaService _alertaService;

    public AlertaController(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    // POST /api/alerta - Crear una alerta nueva
    // Ejemplo de JSON a enviar:
    // { "idDivisaOrigen": 1, "idDivisaDestino": 2, "valorUmbral": 4.50 }
    [HttpPost]
    public async Task<IActionResult> CrearAlerta([FromBody] CrearAlertaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);  // Si los datos no son válidos, devuelve error 400

        try
        {
            var idUsuario = 1;  // Temporal: después se obtendrá del usuario logueado
            var resultado = await _alertaService.CrearAlertaAsync(dto, idUsuario);
            return Ok(new { mensaje = "Alerta creada correctamente.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // GET /api/alerta - Listar todas las alertas del usuario logueado
    [HttpGet]
    public async Task<IActionResult> ObtenerAlertas()
    {
        var idUsuario = 1;  // Temporal
        var alertas = await _alertaService.ObtenerPorUsuarioAsync(idUsuario);
        return Ok(alertas);  // Devuelve la lista en formato JSON
    }

    // PUT /api/alerta/{id} - Actualizar una alerta
    // Ejemplo: PUT /api/alerta/1
    // PUT /api/alerta/{id} - Actualizar una alerta
    // Ejemplo: PUT /api/alerta/1
    // Body: { "valorUmbral": 4.80, "activa": false }
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

    // DELETE /api/alerta/{id} - Eliminar una alerta
    // Ejemplo: DELETE /api/alerta/1
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
