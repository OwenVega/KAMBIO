using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetodoPagoController : ControllerBase
    {
        private readonly IMetodoPagoService _metodoPagoService;

        public MetodoPagoController(IMetodoPagoService metodoPagoService)
        {
            _metodoPagoService = metodoPagoService;
        }

        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> ObtenerCuentas(int idUsuario)
        {
            try
            {
                var cuentas = await _metodoPagoService.ObtenerMetodosPagoUsuarioAsync(idUsuario);
                return Ok(cuentas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al obtener los métodos de pago.", detalle = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarCuenta([FromBody] MetodoPagoCrearDto dto)
        {
            
            try
            {
                await _metodoPagoService.RegistrarMetodoPagoAsync(dto);
                return Ok(new { mensaje = "Cuenta bancaria registrada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al registrar la cuenta bancaria.", detalle = ex.Message });
            }
        }

        [HttpDelete("{idMetodoPago}/usuario/{idUsuario}")]
        public async Task<IActionResult> EliminarCuenta(int idMetodoPago, int idUsuario)
        {
            try
            {
                await _metodoPagoService.EliminarMetodoPagoAsync(idMetodoPago, idUsuario);
                return Ok(new { mensaje = "Cuenta bancaria eliminada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al intentar eliminar la cuenta.", detalle = ex.Message });
            }
        }
    }
}