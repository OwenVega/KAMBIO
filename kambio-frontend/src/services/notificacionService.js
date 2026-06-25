import api from './api'

export const notificacionService = {
  async obtenerNotificaciones (idUsuario) {
    const response = await api.get(`/Notificacion/${idUsuario}`)
    return response.data
  },

  async contarNoLeidas (idUsuario) {
    const response = await api.get(`/Notificacion/${idUsuario}/contador`)
    return response.data.noLeidas
  },

  async marcarComoLeida (idNotificacion) {
    const response = await api.put(`/Notificacion/${idNotificacion}/leer`)
    return response.data
  },

  async marcarTodasComoLeidas (idUsuario) {
    const response = await api.put(`/Notificacion/${idUsuario}/leer-todas`)
    return response.data
  }
}
