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
      <q-page class="q-pa-lg flex flex-center" v-if="cargando">
        <q-spinner color="primary" size="3em" />
      </q-page>

      <q-page class="q-pa-lg" v-else>
        <div class="row q-col-gutter-md justify-center">
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-lg text-center">
              <div class="q-mb-md" style="position: relative; display: inline-block;">
                <q-avatar size="100px" color="primary" text-color="white" class="q-mb-sm">
                  <img v-if="perfil.fotoPerfil" :src="urlFoto" />
                  <span v-else class="text-h4">{{ iniciales }}</span>
                </q-avatar>
                <q-btn
                  round
                  size="sm"
                  color="dark"
                  icon="photo_camera"
                  style="position: absolute; bottom: 0; right: -8px;"
                  @click="$refs.inputFoto.click()"
                />
                <input
                  ref="inputFoto"
                  type="file"
                  accept="image/jpeg,image/png"
                  style="display: none"
                  @change="onFotoSeleccionada"
                />
              </div>

              <div class="text-h6 text-weight-bold">{{ perfil.nombres }} {{ perfil.apellidos }}</div>
              <div class="text-caption text-grey-7 q-mb-sm">Miembro desde 2026</div>

              <div class="row items-center justify-center q-gutter-xs q-mb-md">
                <q-icon name="star" color="amber" />
                <span class="text-weight-bold">{{ perfil.calificacionPromedio || '0.0' }}</span>
                <span class="text-grey-7">/ 5</span>
              </div>

              <q-separator class="q-mb-md" />

              <div class="text-left q-gutter-sm">
                <div class="row items-center q-gutter-sm">
                  <q-icon name="verified" color="green" size="20px" />
                  <span class="text-caption">Estado de Identidad: Verificado</span>
                </div>
                <div class="row items-center q-gutter-sm">
                  <q-icon name="email" color="grey-7" size="20px" />
                  <span class="text-caption">{{ perfil.correo }}</span>
                </div>
              </div>
            </q-card>
          </div>

          <div class="col-12 col-md-6">
            <q-card flat bordered class="q-pa-lg">
              <div class="text-h6 text-weight-bold q-mb-xs">Información Personal</div>
              <div class="text-caption text-grey-7 q-mb-md">
                Gestiona tu información personal y verifica tu reputación institucional.
              </div>

              <q-form @submit.prevent="guardarCambios" class="q-gutter-md">
                <div class="row q-col-gutter-md">
                  <div class="col-6">
                    <q-input v-model="form.nombres" label="Nombres" outlined dense />
                  </div>
                  <div class="col-6">
                    <q-input v-model="form.apellidos" label="Apellidos" outlined dense />
                  </div>
                </div>

                <q-input v-model="form.telefono" label="Teléfono" outlined dense />

                <q-input :model-value="perfil.correo" label="Correo Electrónico" outlined dense disable>
                  <template v-slot:append>
                    <q-icon name="lock" size="16px" color="grey-6" />
                  </template>
                </q-input>

                <q-banner v-if="exitoMensaje" class="bg-green-1 text-green-9 rounded-borders">
                  {{ exitoMensaje }}
                </q-banner>
                <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders">
                  {{ errorMensaje }}
                </q-banner>

                <div class="row q-gutter-sm">
                  <q-btn label="Guardar Cambios" type="submit" color="positive" unelevated :loading="guardando" />
                  <q-btn label="Cancelar" flat @click="restaurarForm" />
                </div>
              </q-form>
            </q-card>
          </div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useAuthStore } from '../stores/auth-store'
import { perfilService } from '../services/perfilService'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()

const router = useRouter()
const cargando = ref(true)
const guardando = ref(false)
const errorMensaje = ref('')
const exitoMensaje = ref('')


const perfil = reactive({
  idUsuario: null,
  nombres: '',
  apellidos: '',
  correo: '',
  telefono: '',
  fotoPerfil: null,
  calificacionPromedio: 0
})

const form = reactive({
  nombres: '',
  apellidos: '',
  telefono: ''
})

const iniciales = computed(() => {
  const n = perfil.nombres?.charAt(0) || ''
  const a = perfil.apellidos?.charAt(0) || ''
  return (n + a).toUpperCase() || '?'
})

const urlFoto = computed(() => {
  if (!perfil.fotoPerfil) return null
  return `https://localhost:7126${perfil.fotoPerfil}`
})

function restaurarForm () {
  form.nombres = perfil.nombres
  form.apellidos = perfil.apellidos
  form.telefono = perfil.telefono
  errorMensaje.value = ''
  exitoMensaje.value = ''
}

async function cargarPerfil () {
  cargando.value = true
  try {
    const datos = await perfilService.obtenerPerfil(authStore.usuarioId)
    Object.assign(perfil, datos)
    restaurarForm()
  } catch (error) {
    console.error('Error al cargar el perfil:', error)
  } finally {
    cargando.value = false
  }
}

async function guardarCambios () {
  guardando.value = true
  errorMensaje.value = ''
  exitoMensaje.value = ''
  try {
    await perfilService.actualizarPerfil(authStore.usuarioId, form)
    exitoMensaje.value = 'Perfil actualizado correctamente.'
    await cargarPerfil()
  } catch (error) {
    errorMensaje.value = error.response?.data?.mensaje || 'No se pudo actualizar el perfil.'
  } finally {
    guardando.value = false
  }
}

async function onFotoSeleccionada (evento) {
  const archivo = evento.target.files[0]
  if (!archivo) return

  try {
    await perfilService.actualizarFoto(authStore.usuarioId, archivo)
    await cargarPerfil()
  } catch (error) {
    errorMensaje.value = error.response?.data?.mensaje || 'No se pudo actualizar la foto.'
  }
}
function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarPerfil()
})
</script>
