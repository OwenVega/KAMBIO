import api from './api'

export const ofertaService = {
  async obtenerOfertasMercado (filtro) {
    // filtro: { idTipoOferta, idDivisaOrigen, idDivisaDestino, monto, idBanco }
    const response = await api.get('/Mercado/ofertas', {
      params: {
        IdTipoOferta: filtro.idTipoOferta,
        IdDivisaOrigen: filtro.idDivisaOrigen,
        IdDivisaDestino: filtro.idDivisaDestino,
        Monto: filtro.monto || undefined,
        IdBanco: filtro.idBanco || undefined
      }
    })
    return response.data
  }
}
