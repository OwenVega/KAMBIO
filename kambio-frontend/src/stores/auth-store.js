import { defineStore } from 'pinia'
import { authService } from '../services/authService'

export const useAuthStore = defineStore('auth', {
  state: () => ({
  usuarioId: localStorage.getItem('kambio_usuarioId') || null,
  nombres: localStorage.getItem('kambio_nombres') || null,
  correo: localStorage.getItem('kambio_correo') || null,
  idRol: localStorage.getItem('kambio_idRol') || null
}),

  getters: {
  estaLogueado: (state) => !!state.usuarioId,
  esAdmin: (state) => Number(state.idRol) === 2
  },

  actions: {
    async login (datos) {
      const respuesta = await authService.login(datos)

      this.usuarioId = respuesta.usuarioId
      this.nombres = respuesta.nombres
      this.correo = respuesta.correo
      this.idRol = respuesta.idRol

      localStorage.setItem('kambio_usuarioId', respuesta.usuarioId)
      localStorage.setItem('kambio_nombres', respuesta.nombres)
      localStorage.setItem('kambio_correo', respuesta.correo)
      localStorage.setItem('kambio_idRol', respuesta.idRol)

      return respuesta
    },

    async registrar (datos) {
      return await authService.registrar(datos)
    },

    cerrarSesion () {
      this.usuarioId = null
      this.nombres = null
      this.correo = null
      this.idRol = null

      localStorage.removeItem('kambio_usuarioId')
      localStorage.removeItem('kambio_nombres')
      localStorage.removeItem('kambio_correo')
      localStorage.removeItem('kambio_idRol')
    }
  }
})
