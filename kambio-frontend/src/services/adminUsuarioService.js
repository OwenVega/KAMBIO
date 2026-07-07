import api from './api'

export const adminUsuarioService = {
  async obtenerUsuarios () {
    const response = await api.get('/admin/usuarios')
    return response.data
  },

  async cambiarEstado (idUsuarioObjetivo, nuevoIdEstadoCuenta, motivo, idAdmin) {
    const response = await api.put('/admin/usuarios/estado', {
      IdUsuarioObjetivo: idUsuarioObjetivo,
      NuevoIdEstadoCuenta: nuevoIdEstadoCuenta,
      Motivo: motivo,
      IdAdmin: idAdmin
    })
    return response.data
  }
}
