import api from './api'

export const reporteService = {
  async obtenerReporte (filtro) {
    const response = await api.get('/Reporte', {
      params: {
        FechaInicio: filtro.fechaInicio || undefined,
        FechaFin: filtro.fechaFin || undefined,
        IdDivisa: filtro.idDivisa || undefined,
        IdUsuario: filtro.idUsuario || undefined
      }
    })
    return response.data
  },

  async exportarExcel (filtro) {
    const response = await api.get('/Reporte/exportar-excel', {
      params: {
        FechaInicio: filtro.fechaInicio || undefined,
        FechaFin: filtro.fechaFin || undefined,
        IdDivisa: filtro.idDivisa || undefined,
        IdUsuario: filtro.idUsuario || undefined
      },
      responseType: 'blob'
    })
    return response.data
  },

  async exportarPdf (filtro) {
    const response = await api.get('/Reporte/exportar-pdf', {
      params: {
        FechaInicio: filtro.fechaInicio || undefined,
        FechaFin: filtro.fechaFin || undefined,
        IdDivisa: filtro.idDivisa || undefined,
        IdUsuario: filtro.idUsuario || undefined
      },
      responseType: 'blob'
    })
    return response.data
  }
}
