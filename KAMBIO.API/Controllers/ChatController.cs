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

    [HttpPost("enviar")]
    public async Task<IActionResult> EnviarMensaje([FromHeader(Name = "X-Usuario-Id")] int? idUsuario, [FromBody] EnviarMensajeDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (idUsuario == null || idUsuario <= 0)
            return Unauthorized(new { mensaje = "Debe iniciar sesión para enviar mensajes." });

        try
        {
            var resultado = await _mensajeChatService.EnviarMensajeAsync(dto, idUsuario.Value);
            return Ok(new { mensaje = "Mensaje enviado correctamente.", data = resultado });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("{idTransaccion}")]
    public async Task<IActionResult> ObtenerMensajes(int idTransaccion, [FromHeader(Name = "X-Usuario-Id")] int? idUsuario)
    {
        if (idUsuario == null || idUsuario <= 0)
            return Unauthorized(new { mensaje = "Debe iniciar sesión para ver los mensajes." });

        try
        {
            var mensajes = await _mensajeChatService.ObtenerMensajesAsync(idTransaccion, idUsuario.Value);
            return Ok(mensajes);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}