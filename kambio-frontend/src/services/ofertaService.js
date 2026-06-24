import api from './api'

export const ofertaService = {
  async obtenerOfertasMercado (filtro) {
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
  },

  async obtenerOfertasActivas () {
    const response = await api.get('/Oferta/activas')
    return response.data
  },

  async cancelarOferta (idOferta, idUsuario) {
    const response = await api.put('/Oferta/cancelar', {
      IdOferta: idOferta,
      IdUsuario: idUsuario
    })
    return response.data
  }
}
