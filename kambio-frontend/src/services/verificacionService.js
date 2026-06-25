import api from './api'

export const verificacionService = {
  async subirImagen (archivo) {
    const formData = new FormData()
    formData.append('archivo', archivo)

    const response = await api.post('/Verificacion/subir-imagen', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data.ruta
  },

  async solicitarVerificacion (rutaImagen) {
    const response = await api.post('/Verificacion/solicitar', {
      RutaImagen: rutaImagen
    })
    return response.data
  },

  async obtenerPorId (id) {
    const response = await api.get(`/Verificacion/${id}`)
    return response.data
  }
}
