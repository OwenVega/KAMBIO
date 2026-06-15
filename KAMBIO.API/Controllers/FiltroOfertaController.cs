using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.CORE.Interfaces;

namespace KAMBIO.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FiltroOfertaController : ControllerBase
    {
        private readonly IFiltroOfertaService _filtroService;

        public FiltroOfertaController(IFiltroOfertaService filtroService)
        {
            _filtroService = filtroService;
        }

        [HttpPost("buscar")]
        public async Task<IActionResult> BuscarOfertas([FromBody] FiltroOfertaRequestDto filtro)
        {
            try
            {
                // Si el frontend envía un body vacío {}, el DTO se instancia con todos sus campos nulos.
                // Esto actúa como el botón "Limpiar Filtros" devolviendo todas las ofertas activas.
                var request = filtro ?? new FiltroOfertaRequestDto();

                var resultado = await _filtroService.ObtenerOfertasFiltradasAsync(request);

                // Cumplimiento del mensaje especial de resultados vacíos
                if (resultado.TotalResultados == 0)
                {
                    return Ok(new
                    {
                        mensaje = "No se encontraron ofertas con los filtros seleccionados",
                        totalResultados = 0,
                        ofertas = resultado.Ofertas
                    });
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error al filtrar las ofertas.", detalle = ex.Message });
            }
        }
    }
}