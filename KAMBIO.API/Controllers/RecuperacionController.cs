using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecuperacionController : ControllerBase
    {
        private readonly IRecuperacionService _recuperacionService;

        public RecuperacionController(IRecuperacionService recuperacionService)
        {
            _recuperacionService = recuperacionService;
        }

        [HttpPost("solicitar")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitarRecuperacionDto dto)
        {
            try
            {
                await _recuperacionService.SolicitarRecuperacionAsync(dto.Correo);
                return Ok(new { mensaje = "Se ha enviado un enlace de recuperacion a tu correo." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaDto dto)
        {
            try
            {
                await _recuperacionService.RestablecerContrasenaAsync(
                    dto.Token, dto.NuevaContrasena, dto.ConfirmarContrasena);
                return Ok(new { mensaje = "Contrasena actualizada correctamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}