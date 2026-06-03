using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IVerificacionService
{
    Task<VerificacionRespuestaDto> SolicitarVerificacionAsync(SolicitarVerificacionDto dto, int idUsuario);
    Task<VerificacionRespuestaDto> RevisarVerificacionAsync(RevisarVerificacionDto dto, int idAdmin);
    Task<IEnumerable<VerificacionRespuestaDto>> ObtenerPendientesAsync();
    Task<VerificacionRespuestaDto?> ObtenerPorIdAsync(int id);
}
