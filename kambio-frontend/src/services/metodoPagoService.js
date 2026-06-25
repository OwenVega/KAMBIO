import api from './api'

export const metodoPagoService = {
  async obtenerCuentas (idUsuario) {
    const response = await api.get(`/MetodoPago/usuario/${idUsuario}`)
    return response.data
  },

  async agregarCuenta (datos) {
    // datos: { idUsuario, idBanco, tipoCuenta, numeroCuenta, cci }
    const response = await api.post('/MetodoPago', {
      IdUsuario: datos.idUsuario,
      IdBanco: datos.idBanco,
      TipoCuenta: datos.tipoCuenta,
      NumeroCuenta: datos.numeroCuenta,
      Cci: datos.cci
    })
    return response.data
  },

  async eliminarCuenta (idMetodoPago, idUsuario) {
    const response = await api.delete(`/MetodoPago/${idMetodoPago}/usuario/${idUsuario}`)
    return response.data
  }
}
