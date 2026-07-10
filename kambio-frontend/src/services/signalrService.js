import * as signalR from '@microsoft/signalr'

let connection = null

export const signalrService = {
  async conectar (idUsuario) {
    if (connection) return connection

    connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7126/hubs/notificaciones')
      .withAutomaticReconnect()
      .build()

    await connection.start()
    await connection.invoke('UnirseAGrupo', String(idUsuario))

    return connection
  },

  onNuevaNotificacion (callback) {
    if (!connection) return
    connection.on('NuevaNotificacion', callback)
  },

  async desconectar (idUsuario) {
    if (!connection) return
    try {
      await connection.invoke('SalirDeGrupo', String(idUsuario))
      await connection.stop()
    } catch (error) {
      console.error('Error al desconectar SignalR:', error)
    } finally {
      connection = null
    }
  }
}
