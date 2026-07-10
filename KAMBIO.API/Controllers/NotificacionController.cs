using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.CORE.Hubs;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;
        private readonly IHubContext<NotificacionHub> _hubContext;

        public NotificacionController(
            INotificacionService notificacionService,
            IHubContext<NotificacionHub> hubContext)
        {
            _notificacionService = notificacionService;
            _hubContext = hubContext;
        }

        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> ObtenerNotificaciones(int idUsuario)
        {
            var notificaciones = await _notificacionService.ObtenerNotificacionesAsync(idUsuario);
            return Ok(notificaciones);
        }

        [HttpGet("{idUsuario}/contador")]
        public async Task<IActionResult> ContarNoLeidas(int idUsuario)
        {
            var count = await _notificacionService.ContarNoLeidasAsync(idUsuario);
            return Ok(new { noLeidas = count });
        }

        [HttpPut("{idNotificacion}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int idNotificacion)
        {
            try
            {
                await _notificacionService.MarcarComoLeidaAsync(idNotificacion);
                return Ok(new { mensaje = "Notificacion marcada como leida." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{idUsuario}/leer-todas")]
        public async Task<IActionResult> MarcarTodasComoLeidas(int idUsuario)
        {
            await _notificacionService.MarcarTodasComoLeidasAsync(idUsuario);

            // Notificar via SignalR que se actualizó el contador
            await _hubContext.Clients.Group($"usuario_{idUsuario}")
                .SendAsync("ActualizarContador", 0);

            return Ok(new { mensaje = "Todas las notificaciones marcadas como leidas." });
        }
    }
}
