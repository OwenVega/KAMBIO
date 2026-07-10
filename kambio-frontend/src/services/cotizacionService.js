// src/services/cotizacionService.js
const API_KEY = '0d09c2859c94141975dad009' // reemplaza con tu key
const BASE_URL = 'https://v6.exchangerate-api.com/v6'

export const cotizacionService = {
  async obtenerTasa (monedaOrigen, monedaDestino) {
    if (monedaOrigen === monedaDestino) {
      return { tasa: 1, fecha: null }
    }
    const response = await fetch(`${BASE_URL}/${API_KEY}/pair/${monedaOrigen}/${monedaDestino}`)
    if (!response.ok) throw new Error('No se pudo obtener la cotización')
    const data = await response.json()

    if (data.result !== 'success') {
      throw new Error(data['error-type'] || 'Error al obtener cotización')
    }

    return {
      tasa: data.conversion_rate,
      fecha: new Date(data.time_last_update_unix * 1000).toLocaleDateString('es-PE')
    }
  }
}
