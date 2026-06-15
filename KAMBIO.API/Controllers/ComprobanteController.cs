using KAMBIO.CORE.CORE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprobanteController : ControllerBase
    {
        private readonly IComprobanteService _comprobanteService;
        private readonly IWebHostEnvironment _env;

        public ComprobanteController(IComprobanteService comprobanteService, IWebHostEnvironment env)
        {
            _comprobanteService = comprobanteService;
            _env = env;
        }

        [HttpPost("subir")]
        public async Task<IActionResult> SubirVoucher(
        [FromForm] int idTransaccion,
        [FromForm] int idUsuario,
        IFormFile archivo)
            {
            try
            {
                // Usar ContentRootPath en lugar de WebRootPath
                var carpeta = Path.Combine(_env.ContentRootPath, "vouchers");
                await _comprobanteService.SubirComprobanteAsync(idTransaccion, idUsuario, archivo, carpeta);
                return Ok(new { mensaje = "Comprobante subido correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno.", detalle = ex.Message });
            }
        }
    }
}