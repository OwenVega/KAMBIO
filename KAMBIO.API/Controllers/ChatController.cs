// CONTROLADOR de Chat (API)
// Endpoints para enviar mensajes y ver la conversación de una transacción.
using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Interfaces;

namespace KAMBIO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IMensajeChatService _mensajeChatService;

    public ChatController(IMensajeChatService mensajeChatService)
    {
        _mensajeChatService = mensajeChatService;
    }

    // POST /api/chat/enviar - Enviar un mensaje en el chat de una transacción
    // Body: { "idTransaccion": 1, "mensaje": "Hola, ya hice la transferencia" }
    [HttpPost("enviar")]
    public async Task<IActionResult> EnviarMensaje([FromBody] EnviarMensajeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var idUsuario = 1;  // Temporal: será el usuario logueado
            var resultado = await _mensajeChatService.EnviarMensajeAsync(dto, idUsuario);
            return Ok(new { mensaje = "Mensaje enviado correctamente.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // GET /api/chat/{idTransaccion} - Ver todos los mensajes de una transacción
    // Ejemplo: GET /api/chat/1
    [HttpGet("{idTransaccion}")]
    public async Task<IActionResult> ObtenerMensajes(int idTransaccion)
    {
        try
        {
            var idUsuario = 1;
            var mensajes = await _mensajeChatService.ObtenerMensajesAsync(idTransaccion, idUsuario);
            return Ok(mensajes);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
