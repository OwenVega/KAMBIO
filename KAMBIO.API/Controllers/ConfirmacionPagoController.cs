using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.Core.DTOs.ConfirmacionPago;

namespace KAMBIO.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmacionPagoController : ControllerBase
    {
        private readonly IConfirmacionPagoService _confirmacionPagoService;

        public ConfirmacionPagoController(
            IConfirmacionPagoService confirmacionPagoService)
        {
            _confirmacionPagoService = confirmacionPagoService;
        }
        [HttpGet("{idTransaccion}")]
        public async Task<IActionResult> ObtenerTransaccion(int idTransaccion)
        {
            return Ok($"Consulta de la transacción {idTransaccion}");
        }
        [HttpPut("{idTransaccion}")]
        public async Task<IActionResult> ConfirmarPago(
            int idTransaccion,
            [FromBody] ConfirmarPagoRequestDTO request)
        {
            try
            {
                var response =
                    await _confirmacionPagoService.ConfirmarPago(
                        idTransaccion,
                        request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }
    }
}