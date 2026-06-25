import api from './api'

export const chatService = {
  async enviarMensaje (idTransaccion, mensaje, idUsuario) {
    const response = await api.post('/Chat/enviar', {
      IdTransaccion: idTransaccion,
      Mensaje: mensaje
    }, {
      headers: {
        'X-Usuario-Id': idUsuario
      }
    })
    return response.data
  },

  async obtenerMensajes (idTransaccion, idUsuario) {
    const response = await api.get(`/Chat/${idTransaccion}`, {
      headers: {
        'X-Usuario-Id': idUsuario
      }
    })
    return response.data
  }
}
