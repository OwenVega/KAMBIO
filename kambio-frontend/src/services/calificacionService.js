import api from './api'

export const calificacionService = {
  async calificar (datos) {
    // datos: { idTransaccion, idUsuarioEvalua, idUsuarioEvaluado, estrellas, comentario }
    const response = await api.post('/Calificacion', {
      IdTransaccion: datos.idTransaccion,
      IdUsuarioEvalua: datos.idUsuarioEvalua,
      IdUsuarioEvaluado: datos.idUsuarioEvaluado,
      Estrellas: datos.estrellas,
      Comentario: datos.comentario || null
    })
    return response.data
  },

  async obtenerPromedio (idUsuario) {
    const response = await api.get(`/Calificacion/usuario/${idUsuario}`)
    return response.data
  }
}
