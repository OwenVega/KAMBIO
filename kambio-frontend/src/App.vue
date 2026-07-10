<template>
  <router-view />
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from './stores/auth-store'
import { signalrService } from './services/signalrService'

const authStore = useAuthStore()

onMounted(async () => {
  if (authStore.estaLogueado) {
    try {
      await signalrService.conectar(authStore.usuarioId)
    } catch (error) {
      console.error('Error al conectar SignalR:', error)
    }
  }
})
</script>
