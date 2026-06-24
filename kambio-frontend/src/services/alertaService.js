import api from './api'

export const alertaService = {
  async obtenerAlertas () {
    const response = await api.get('/Alerta')
    return response.data
  },

  async crearAlerta (datos) {
    // datos: { idDivisaOrigen, idDivisaDestino, valorUmbral }
    const response = await api.post('/Alerta', {
      IdDivisaOrigen: datos.idDivisaOrigen,
      IdDivisaDestino: datos.idDivisaDestino,
      ValorUmbral: datos.valorUmbral
    })
    return response.data
  },

  async actualizarAlerta (id, datos) {
    // datos: { valorUmbral, activa }
    const response = await api.put(`/Alerta/${id}`, {
      ValorUmbral: datos.valorUmbral,
      Activa: datos.activa
    })
    return response.data
  },

  async eliminarAlerta (id) {
    const response = await api.delete(`/Alerta/${id}`)
    return response.data
  }
}
