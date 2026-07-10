import api from './api'

export const transaccionService = {
  async crearDesdeOferta (idOferta, idUsuario) {
    const response = await api.post(`/Transaccion/desde-oferta/${idOferta}`, null, {
      headers: {
        'X-Usuario-Id': idUsuario
      }
    })
    return response.data
  },
  async obtenerActivas (idUsuario) {
  const response = await api.get('/Transaccion/activas', {
    headers: {
      'X-Usuario-Id': idUsuario
    }
  })
  return response.data
  },
  async obtenerPorId (idTransaccion) {
    const response = await api.get(`/Transaccion/${idTransaccion}`)
    return response.data
  },

  async cambiarEstado (datos) {
    // datos: { idTransaccion, idEstadoTransaccion, idUsuarioCambio, observacion }
    const response = await api.put('/Transaccion/cambiar-estado', {
      IdTransaccion: datos.idTransaccion,
      IdEstadoTransaccion: datos.idEstadoTransaccion,
      IdUsuarioCambio: datos.idUsuarioCambio,
      Observacion: datos.observacion
    })
    return response.data
  },

  async obtenerHistorial (idUsuario, filtro = {}) {
    const response = await api.get('/Transaccion/historial', {
      params: {
        BusquedaDivisas: filtro.busquedaDivisas || undefined,
        FechaInicio: filtro.fechaInicio || undefined,
        FechaFin: filtro.fechaFin || undefined,
        TipoOperacion: filtro.tipoOperacion || undefined,
        IdEstado: filtro.idEstado || undefined,
        Pagina: filtro.pagina || 1,
        CantidadPorPagina: filtro.cantidadPorPagina || 10
      }
    })
    return response.data
  }

}
