using KAMBIO.CORE.Core.DTOs;

namespace KAMBIO.CORE.Core.Interfaces;

public interface IAlertaService
{
    Task<AlertaRespuestaDto> CrearAlertaAsync(CrearAlertaDto dto, int idUsuario);
    Task<IEnumerable<AlertaRespuestaDto>> ObtenerPorUsuarioAsync(int idUsuario);
    Task<AlertaRespuestaDto> ActualizarAlertaAsync(int id, ActualizarAlertaDto dto, int idUsuario);
    Task EliminarAlertaAsync(int id, int idUsuario);
}
