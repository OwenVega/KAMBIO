
﻿using KAMBIO.CORE.Core.Entities;
namespace KAMBIO.CORE.CORE.Interfaces
{
    public interface IOfertaRepository
    {

        Task<Oferta> CrearOfertaCompra(Oferta oferta, List<int> idBancos);
        Task<List<Oferta>> ObtenerOfertasActivas();
        Task<List<Oferta>> ObtenerOfertasFiltradasAsync(int idTipoOferta, int idDivisaOrigen, int idDivisaDestino, decimal? monto, int? idBanco);
        Task<Oferta?> ObtenerPorIdAsync(int idOferta);
        Task ActualizarAsync(Oferta oferta);
        Task<bool> TieneTransaccionEnCursoAsync(int idOferta);
    }
}

       
   

