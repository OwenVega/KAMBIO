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
              <q-item v-if="authStore.esAdmin" clickable v-close-popup to="/admin/disputas">
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
        <div class="row items-center justify-between q-mb-md">
          <div>
            <div class="text-h6 text-weight-bold">Centro de Notificaciones</div>
            <div class="text-caption text-grey-7">
              Gestiona tus alertas y el estado de tus transacciones P2P.
            </div>
          </div>
          <q-btn
            v-if="notificaciones.some(n => !n.leida)"
            label="Marcar todas como leídas"
            flat
            no-caps
            color="primary"
            icon="done_all"
            @click="marcarTodasLeidas"
          />
        </div>

        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-8">
            <q-card flat bordered>
              <q-list separator>
                <q-item
                  v-for="notif in notificaciones"
                  :key="notif.idNotificacion"
                  :class="!notif.leida ? 'bg-blue-1' : ''"
                >
                  <q-item-section avatar>
                    <q-icon
                      :name="iconoTipo(notif.tipoReferencia)"
                      :color="colorTipo(notif.tipoReferencia)"
                      size="28px"
                    />
                  </q-item-section>

                  <q-item-section>
                    <q-item-label class="text-weight-medium">{{ notif.titulo }}</q-item-label>
                    <q-item-label caption lines="2">{{ notif.mensaje }}</q-item-label>
                    <q-item-label caption class="text-grey-6">{{ tiempoRelativo(notif.fechaCreacion) }}</q-item-label>
                  </q-item-section>

                  <q-item-section side top>
                    <div class="column items-end q-gutter-xs">
                      <q-badge v-if="!notif.leida" color="primary" rounded />
                      <q-btn
                        v-if="!notif.leida"
                        label="Marcar leída"
                        flat
                        dense
                        no-caps
                        size="sm"
                        color="primary"
                        @click="marcarLeida(notif)"
                      />
                    </div>
                  </q-item-section>
                </q-item>

                <q-item v-if="!cargando && notificaciones.length === 0">
                  <q-item-section class="text-center text-grey-7 q-py-xl">
                    <q-icon name="notifications_none" size="48px" class="q-mb-sm" />
                    No tienes notificaciones todavía.
                  </q-item-section>
                </q-item>
              </q-list>
            </q-card>
          </div>

          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-md q-mb-md">
              <div class="text-subtitle2 text-weight-bold q-mb-sm">Resumen de Actividad</div>
              <div class="row justify-between q-mb-xs">
                <span class="text-caption text-grey-7">Sin Leer</span>
                <q-chip color="red-1" text-color="red-9" dense>{{ noLeidas }}</q-chip>
              </div>
              <div class="row justify-between">
                <span class="text-caption text-grey-7">Total</span>
                <span class="text-weight-bold">{{ notificaciones.length }}</span>
              </div>
            </q-card>

            <q-banner class="bg-blue-1 text-blue-9 rounded-borders">
              <template v-slot:avatar>
                <q-icon name="security" color="blue-9" />
              </template>
              <div class="text-weight-bold">Seguridad Kambio</div>
              Nunca compartas tus credenciales o códigos de verificación. Kambio nunca te pedirá contraseñas por chat o correo.
            </q-banner>
          </div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth-store'
import { notificacionService } from '../services/notificacionService'

const router = useRouter()
const authStore = useAuthStore()

const notificaciones = ref([])
const noLeidas = ref(0)
const cargando = ref(false)

function iconoTipo (tipo) {
  const mapa = {
    Oferta: 'local_offer',
    Transaccion: 'sync_alt',
    Disputa: 'gavel',
    Sistema: 'info'
  }
  return mapa[tipo] || 'notifications'
}

function colorTipo (tipo) {
  const mapa = {
    Oferta: 'positive',
    Transaccion: 'primary',
    Disputa: 'negative',
    Sistema: 'grey-7'
  }
  return mapa[tipo] || 'grey-7'
}

function tiempoRelativo (fecha) {
  const ahora = new Date()
  const fechaNotif = new Date(fecha)
  const diffMs = ahora - fechaNotif
  const diffMin = Math.floor(diffMs / 60000)

  if (diffMin < 1) return 'Hace un momento'
  if (diffMin < 60) return `Hace ${diffMin} min`
  const diffHoras = Math.floor(diffMin / 60)
  if (diffHoras < 24) return `Hace ${diffHoras} hora${diffHoras > 1 ? 's' : ''}`
  const diffDias = Math.floor(diffHoras / 24)
  return `Hace ${diffDias} día${diffDias > 1 ? 's' : ''}`
}

async function cargarNotificaciones () {
  cargando.value = true
  try {
    notificaciones.value = await notificacionService.obtenerNotificaciones(authStore.usuarioId)
    noLeidas.value = await notificacionService.contarNoLeidas(authStore.usuarioId)
  } catch (error) {
    console.error('Error al cargar notificaciones:', error)
  } finally {
    cargando.value = false
  }
}

async function marcarLeida (notif) {
  try {
    await notificacionService.marcarComoLeida(notif.idNotificacion)
    await cargarNotificaciones()
  } catch (error) {
    console.error('Error al marcar como leída:', error)
  }
}

async function marcarTodasLeidas () {
  try {
    await notificacionService.marcarTodasComoLeidas(authStore.usuarioId)
    await cargarNotificaciones()
  } catch (error) {
    console.error('Error al marcar todas como leídas:', error)
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarNotificaciones()
})
</script>
