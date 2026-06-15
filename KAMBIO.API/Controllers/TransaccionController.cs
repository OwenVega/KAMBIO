using System.Security.Claims;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KAMBIO.CORE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransaccionController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;

        public TransaccionController(ITransaccionService transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpGet("historial")]
        public async Task<IActionResult> ObtenerHistorial([FromQuery] FiltroHistorialDTO filtro)
        {
            var claimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int idUsuario = string.IsNullOrEmpty(claimId) ? 1 : int.Parse(claimId);

            var historial = await _transaccionService.ObtenerHistorialUsuarioAsync(idUsuario, filtro);
            return Ok(historial);
        }
    }
}