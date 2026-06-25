using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces; 

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDto dto)
        {

            try
            {
                await _usuarioService.RegistrarUsuarioAsync(dto);

                return Ok(new { mensaje = "Usuario registrado correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno en el servidor.", detalle = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto dto)
        {
            try
            {
                var usuario = await _usuarioService.LoginAsync(dto);

                return Ok(new
                {
                    mensaje = "Inicio de sesión exitoso.",
                    usuarioId = usuario.IdUsuario,
                    nombres = usuario.Nombres,
                    correo = usuario.Correo,
                    idRol = usuario.IdRol
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
        }
    }
}