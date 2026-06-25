import api from './api'

export const recuperacionService = {
  async solicitarRecuperacion (correo) {
    const response = await api.post('/Recuperacion/solicitar', {
      Correo: correo
    })
    return response.data
  },

  async restablecerContrasena (datos) {
    // datos: { token, nuevaContrasena, confirmarContrasena }
    const response = await api.post('/Recuperacion/restablecer', {
      Token: datos.token,
      NuevaContrasena: datos.nuevaContrasena,
      ConfirmarContrasena: datos.confirmarContrasena
    })
    return response.data
  }
}
