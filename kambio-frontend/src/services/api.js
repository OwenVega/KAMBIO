import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7126/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

// Interceptor: si en el futuro usas JWT, aquí se agrega el token automáticamente
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('kambio_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api
