// INTERFAZ del Servicio de Ofertas
// Define la operación de negocio: crear una oferta con validaciones.
using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IOfertaService
{
    Task<OfertaRespuestaDto> CrearOfertaAsync(CrearOfertaDto dto, int idUsuario); // Publicar una oferta
}
