using Microsoft.AspNetCore.Mvc;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;


namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;

        public PerfilController(IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> ObtenerPerfil(int idUsuario)
        {
            try
            {
                var perfil = await _perfilService.ObtenerPerfilAsync(idUsuario);
                return Ok(perfil);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{idUsuario}")]
        public async Task<IActionResult> ActualizarPerfil(int idUsuario, [FromBody] ActualizarPerfilDto dto)
        {
            try
            {
                await _perfilService.ActualizarPerfilAsync(idUsuario, dto);
                return Ok(new { mensaje = "Perfil actualizado correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{idUsuario}/foto")]
        public async Task<IActionResult> ActualizarFoto(int idUsuario, IFormFile foto)
        {
            if (foto == null || foto.Length == 0)
                return BadRequest(new { mensaje = "No se proporcionó ninguna imagen." });

            var extensiones = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(foto.FileName).ToLower();
            if (!extensiones.Contains(extension))
                return BadRequest(new { mensaje = "Solo se permiten archivos JPG o PNG." });

            var carpeta = Path.Combine("wwwroot", "uploads", "perfiles");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"usuario_{idUsuario}_{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            var rutaRelativa = $"/uploads/perfiles/{nombreArchivo}";
            await _perfilService.ActualizarFotoPerfilAsync(idUsuario, rutaRelativa);

            return Ok(new { mensaje = "Foto de perfil actualizada.", ruta = rutaRelativa });
        }
    }
}
