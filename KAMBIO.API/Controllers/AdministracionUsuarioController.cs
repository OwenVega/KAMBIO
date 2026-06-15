using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/admin/usuarios")]
    public class AdministracionUsuarioController : ControllerBase
    {
        private readonly IAdministracionUsuarioService _adminService;

        public AdministracionUsuarioController(IAdministracionUsuarioService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerListadoUsuarios()
        {
            try
            {
                var usuarios = await _adminService.ObtenerListadoUsuariosAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al cargar el listado de usuarios.", detalle = ex.Message });
            }
        }

        [HttpPut("estado")]
        public async Task<IActionResult> CambiarEstadoUsuario([FromBody] CambiarEstadoUsuarioDto dto)
        {
            try
            {
                await _adminService.CambiarEstadoCuentaAsync(dto);
                return Ok(new { mensaje = "El estado del usuario ha sido actualizado y se han aplicado las restricciones correspondientes." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al actualizar el estado de la cuenta.", detalle = ex.Message });
            }
        }
    }
}