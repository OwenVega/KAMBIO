import api from './api'

export const perfilService = {
  async obtenerPerfil (idUsuario) {
    const response = await api.get(`/Perfil/${idUsuario}`)
    return response.data
  },

  async actualizarPerfil (idUsuario, datos) {
    // datos: { nombres, apellidos, telefono }
    const response = await api.put(`/Perfil/${idUsuario}`, {
      Nombres: datos.nombres,
      Apellidos: datos.apellidos,
      Telefono: datos.telefono
    })
    return response.data
  },

  async actualizarFoto (idUsuario, archivo) {
    const formData = new FormData()
    formData.append('foto', archivo)

    const response = await api.put(`/Perfil/${idUsuario}/foto`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  }
}
