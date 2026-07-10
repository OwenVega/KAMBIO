<template>
  <q-layout view="lHh Lpr lFf" container style="height: 100vh" class="bg-grey-2">
    <q-header bordered class="bg-white text-dark">
      <q-toolbar class="q-px-lg">
        <q-toolbar-title class="text-weight-bold text-dark">
          Kambio
        </q-toolbar-title>

        <q-tabs class="text-grey-7" active-color="primary" indicator-color="primary">
          <q-route-tab name="intercambio" label="Intercambio" no-caps to="/marketplace" />
          <q-route-tab name="ofertas" label="Mis Ofertas" no-caps to="/mis-ofertas" />
          <q-route-tab name="historial" label="Historial" no-caps to="/historial" />
          <q-route-tab name="mensajes" label="Mensajes" no-caps to="/mensajes" />
          <q-route-tab name="billeteras" label="Billeteras" no-caps to="/billeteras" />
          <q-route-tab name="alertas" label="Alertas" no-caps to="/alertas" />
        </q-tabs>

        <q-space />
        <q-btn flat round icon="notifications" to="/notificaciones">
          <q-badge v-if="noLeidas > 0" color="red" floating>{{ noLeidas }}</q-badge>
        </q-btn>
        <q-btn flat round icon="account_circle">
          <q-menu anchor="bottom right" self="top right">
            <q-list style="min-width: 200px">
              <q-item-label header class="text-caption text-grey-7">
                {{ authStore.correo }}
              </q-item-label>
              <q-separator />
              <q-item clickable v-close-popup to="/perfil">
                <q-item-section avatar>
                  <q-icon name="person" />
                </q-item-section>
                <q-item-section>Mi Perfil</q-item-section>
              </q-item>
              <q-item v-if="authStore.esAdmin" clickable v-close-popup to="/admin/dashboard">
                <q-item-section avatar>
                  <q-icon name="gavel" color="orange" />
                </q-item-section>
                <q-item-section>Panel Admin</q-item-section>
              </q-item>
              <q-separator />
              <q-item clickable v-close-popup @click="cerrarSesion">
                <q-item-section avatar>
                  <q-icon name="logout" color="negative" />
                </q-item-section>
                <q-item-section class="text-negative">Cerrar Sesión</q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </q-btn>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <q-page class="q-pa-lg">
        <div class="text-h6 text-weight-bold q-mb-md">Mensajes</div>
        <div class="text-caption text-grey-7 q-mb-md">
          Conversaciones activas de tus transacciones en curso.
        </div>

        <q-inner-loading :showing="cargando">
          <q-spinner color="primary" size="3em" />
        </q-inner-loading>

        <q-card v-if="!cargando" flat bordered>
          <q-list separator>
            <q-item v-if="transacciones.length === 0">
              <q-item-section class="text-center text-grey-6 q-py-lg">
                <q-icon name="chat_bubble_outline" size="48px" class="q-mb-sm" />
                No tienes transacciones activas en este momento.
              </q-item-section>
            </q-item>

            <q-item
              v-for="t in transacciones"
              :key="t.idTransaccion"
              clickable
              @click="router.push(`/transaccion/${t.idTransaccion}`)"
            >
              <q-item-section avatar>
                <q-avatar color="primary" text-color="white">
                  {{ obtenerIniciales(t.otraParteNombre) }}
                </q-avatar>
              </q-item-section>

              <q-item-section>
                <q-item-label class="text-weight-medium">
                  {{ t.otraParteNombre }}
                  <span class="text-caption text-grey-6">— Transacción #{{ t.idTransaccion }}</span>
                </q-item-label>
                <q-item-label caption lines="1">
                  {{ t.ultimoMensaje || 'Sin mensajes todavía' }}
                </q-item-label>
              </q-item-section>

              <q-item-section side top>
                <q-chip dense size="sm" :color="colorEstado(t.estadoNombre)" text-color="white">
                  {{ t.estadoNombre }}
                </q-chip>
                <q-badge v-if="t.mensajesNoLeidos > 0" color="red" rounded class="q-mt-xs">
                  {{ t.mensajesNoLeidos }}
                </q-badge>
              </q-item-section>
            </q-item>
          </q-list>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth-store'
import { transaccionService } from '../services/transaccionService'

const router = useRouter()
const authStore = useAuthStore()

const transacciones = ref([])
const cargando = ref(false)
const noLeidas = ref(0) // si ya tienes lógica de notificaciones no leídas, reemplaza esto

function obtenerIniciales (nombreCompleto) {
  if (!nombreCompleto) return '?'
  const partes = nombreCompleto.trim().split(' ')
  if (partes.length === 1) return partes[0].charAt(0).toUpperCase()
  return (partes[0].charAt(0) + partes[1].charAt(0)).toUpperCase()
}

function colorEstado (estado) {
  const mapa = {
    Pendiente: 'orange',
    'En Proceso': 'blue',
    'Pago Realizado': 'teal',
    'En Disputa': 'deep-orange'
  }
  return mapa[estado] || 'grey'
}

async function cargarTransacciones () {
  cargando.value = true
  try {
    transacciones.value = await transaccionService.obtenerActivas(authStore.usuarioId)
  } catch (error) {
    console.error('Error al cargar transacciones activas:', error)
  } finally {
    cargando.value = false
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarTransacciones()
})
</script>
