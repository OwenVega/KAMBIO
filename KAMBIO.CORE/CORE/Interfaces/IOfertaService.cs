
﻿using KAMBIO.CORE.Core.DTOs;
using KAMBIO.CORE.Core.Entities;
namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaService
    {

        Task EjecutarMatchingAutomaticoAsync(Oferta nuevaOferta);
        Task<IEnumerable<MatchSugeridoDto>> ObtenerMatchesSugeridosAsync(int idUsuario);
        Task ProcesarRespuestaMatchAsync(RespuestaMatchDto respuesta);
        Task<OfertaResponseDTO> CrearOfertaCompra(int idUsuario, CrearOfertaCompraRequestDTO dto);
        Task<List<OfertaResponseDTO>> ObtenerOfertasActivas();
        Task CancelarOfertaAsync(int idOferta, int idUsuario);
    }
}

        
   
        
 
 

