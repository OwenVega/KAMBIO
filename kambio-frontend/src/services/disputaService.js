import api from './api'

export const disputaService = {
  async crearDisputa (idTransaccion, idUsuarioReporta, descripcion) {
    const response = await api.post('/Disputa', {
      IdTransaccion: idTransaccion,
      IdUsuarioReporta: idUsuarioReporta,
      Descripcion: descripcion
    })
    return response.data
  },

  async obtenerDisputas () {
    const response = await api.get('/Disputa')
    return response.data
  },

  async obtenerDisputaPorId (id) {
    const response = await api.get(`/Disputa/${id}`)
    return response.data
  },

  async resolverDisputa (id, idAdminResolucion, resolucionDetalle) {
    const response = await api.put(`/Disputa/resolver/${id}`, {
      IdAdminResolucion: idAdminResolucion,
      ResolucionDetalle: resolucionDetalle
    })
    return response.data
  },

  async rechazarDisputa (id, idAdminResolucion, resolucionDetalle) {
    const response = await api.put(`/Disputa/rechazar/${id}`, {
      IdAdminResolucion: idAdminResolucion,
      ResolucionDetalle: resolucionDetalle
    })
    return response.data
  }
}
