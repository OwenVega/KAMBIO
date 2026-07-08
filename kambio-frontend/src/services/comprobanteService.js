import api from './api'

export const comprobanteService = {
  async subirComprobante (idTransaccion, idUsuario, archivo) {
    const formData = new FormData()
    formData.append('idTransaccion', idTransaccion)
    formData.append('idUsuario', idUsuario)
    formData.append('archivo', archivo)

    const response = await api.post('/Comprobante/subir', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  },
  async obtenerPorTransaccion (idTransaccion) {
    const response = await api.get(`/Comprobante/${idTransaccion}`)
    return response.data
  }
}
