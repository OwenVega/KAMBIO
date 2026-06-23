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
  },

  async crearOfertaCompra (idUsuario, datos) {
    // datos: { idDivisaOrigen, idDivisaDestino, montoDisponible, montoMinimo, montoMaximo, tasaCambio, metodosPago }
    const response = await api.post('/Oferta/compra', {
      IdDivisaOrigen: datos.idDivisaOrigen,
      IdDivisaDestino: datos.idDivisaDestino,
      MontoDisponible: datos.montoDisponible,
      MontoMinimo: datos.montoMinimo,
      MontoMaximo: datos.montoMaximo,
      TasaCambio: datos.tasaCambio,
      MetodosPago: datos.metodosPago
    }, {
      headers: {
        'X-Usuario-Id': idUsuario
      }
    })
    return response.data
  },

  async crearOfertaVenta (idUsuario, datos) {
    // datos: { idDivisaOrigen, idDivisaDestino, montoDisponible, montoMinimo, montoMaximo, tasaCambio, idBancos }
    const response = await api.post('/OfertaVenta', {
      IdUsuario: idUsuario,
      IdDivisaOrigen: datos.idDivisaOrigen,
      IdDivisaDestino: datos.idDivisaDestino,
      MontoDisponible: datos.montoDisponible,
      MontoMinimo: datos.montoMinimo,
      MontoMaximo: datos.montoMaximo,
      TasaCambio: datos.tasaCambio,
      IdBancos: datos.idBancos
    })
    return response.data
  }
}
