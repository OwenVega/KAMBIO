import api from './api'

export const authService = {
  async registrar (datos) {
    // datos: { nombres, apellidos, correo, contrasena }
    const response = await api.post('/Auth/registro', {
      Nombres: datos.nombres,
      Apellidos: datos.apellidos,
      Correo: datos.correo,
      Contrasena: datos.contrasena
    })
    return response.data
  },

  async login (datos) {
    // datos: { correo, contrasena }
    const response = await api.post('/Auth/login', {
      Correo: datos.correo,
      Contrasena: datos.contrasena
    })
    return response.data
  }
}
