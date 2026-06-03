using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IOfertaService
{
    Task<OfertaRespuestaDto> CrearOfertaAsync(CrearOfertaDto dto, int idUsuario);
}
