using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces; // Asegúrate de que este using apunte a donde está tu IUsuarioService

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        // Inyección de dependencias del servicio
        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDto dto)
        {
            // NOTA: Gracias al atributo [ApiController] arriba, .NET evalúa automáticamente 
            // los Data Annotations de tu DTO (como [Required] o [MinLength(8)]). 
            // Si algo falla, .NET corta la ejecución aquí mismo y devuelve un error HTTP 400 al frontend.

            try
            {
                // Pasamos el DTO al servicio para que aplique las reglas de negocio y guarde
                await _usuarioService.RegistrarUsuarioAsync(dto);

                // Si todo sale bien, devolvemos un HTTP 200 OK con un JSON de éxito
                return Ok(new { mensaje = "Usuario registrado correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                // Aquí atrapamos tu regla de negocio (ej. "Este correo ya está registrado.")
                // y le mandamos un HTTP 400 (Bad Request) al frontend para que lo muestre en pantalla.
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Si la base de datos se cae o hay un error de código crítico (HTTP 500)
                return StatusCode(500, new { error = "Ocurrió un error interno en el servidor.", detalle = ex.Message });
            }
        }
    }
}