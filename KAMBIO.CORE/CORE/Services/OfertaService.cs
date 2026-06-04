using KAMBIO.CORE.CORE.Interfaces;
using KAMBIO.CORE.CORE.Services;

namespace KAMBIO.CORE.CORE.Services
{
    public class OfertaService : IOfertaService
    {
        private readonly IOfertaRepository _ofertaRepository;

        public OfertaService(IOfertaRepository ofertaRepository)
        {
            _ofertaRepository = ofertaRepository;
        }

        public async Task CancelarOfertaAsync(int idOferta, int idUsuario)
        {
            var oferta = await _ofertaRepository.ObtenerPorIdAsync(idOferta);
            if (oferta == null)
                throw new InvalidOperationException("Oferta no encontrada.");

            if (oferta.IdUsuario != idUsuario)
                throw new InvalidOperationException("No tienes permiso para cancelar esta oferta.");

            if (oferta.IdEstadoOferta != 1) // 1 = Activa
                throw new InvalidOperationException("Solo se pueden cancelar ofertas en estado Activa.");

            var tieneTransaccion = await _ofertaRepository.TieneTransaccionEnCursoAsync(idOferta);
            if (tieneTransaccion)
                throw new InvalidOperationException("No se puede cancelar una oferta con una transaccion en curso.");

            oferta.IdEstadoOferta = 2; // 2 = Cancelada
            oferta.FechaCancelacion = DateTime.UtcNow;

            await _ofertaRepository.ActualizarAsync(oferta);
        }
    }
}
