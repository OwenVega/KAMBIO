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
      <q-page class="q-pa-lg flex flex-center">
        <q-card flat bordered class="verificacion-card q-pa-lg">
          <div v-if="!enviado">
            <div class="text-h6 text-weight-bold">Verificación de Documento</div>
            <div class="text-caption text-grey-7 q-mb-md">
              Sube una foto clara de tu Documento Nacional de Identidad (DNI) por ambos lados.
            </div>

            <div class="row q-col-gutter-md q-mb-md">
              <div class="col-6">
                <div class="text-caption text-weight-medium text-grey-8 q-mb-xs">PARTE FRONTAL</div>
                <q-card
                  flat
                  bordered
                  class="zona-subida flex flex-center column cursor-pointer"
                  @click="$refs.inputFrontal.click()"
                >
                  <q-img v-if="previewFrontal" :src="previewFrontal" style="height: 120px" fit="cover" />
                  <template v-else>
                    <q-icon name="photo_camera" size="28px" color="grey-6" />
                    <div class="text-caption text-grey-6 q-mt-xs">Click para subir frontal</div>
                    <div class="text-caption text-grey-5">JPG, PNG hasta 5MB</div>
                  </template>
                </q-card>
                <input ref="inputFrontal" type="file" accept="image/jpeg,image/png" style="display:none" @change="e => onSeleccionar(e, 'frontal')" />
              </div>

              <div class="col-6">
                <div class="text-caption text-weight-medium text-grey-8 q-mb-xs">PARTE POSTERIOR</div>
                <q-card
                  flat
                  bordered
                  class="zona-subida flex flex-center column cursor-pointer"
                  @click="$refs.inputPosterior.click()"
                >
                  <q-img v-if="previewPosterior" :src="previewPosterior" style="height: 120px" fit="cover" />
                  <template v-else>
                    <q-icon name="upload" size="28px" color="grey-6" />
                    <div class="text-caption text-grey-6 q-mt-xs">Click para subir posterior</div>
                    <div class="text-caption text-grey-5">JPG, PNG hasta 5MB</div>
                  </template>
                </q-card>
                <input ref="inputPosterior" type="file" accept="image/jpeg,image/png" style="display:none" @change="e => onSeleccionar(e, 'posterior')" />
              </div>
            </div>

            <q-banner class="bg-green-1 text-green-9 rounded-borders q-mb-md">
              <template v-slot:avatar>
                <q-icon name="shield" color="green-9" />
              </template>
              Privacidad Garantizada — Tus datos están encriptados y se usan solo para validación.
            </q-banner>

            <div class="row q-col-gutter-sm q-mb-md text-center">
              <div class="col-4">
                <q-icon name="wb_sunny" color="grey-7" />
                <div class="text-caption text-grey-7">Buena iluminación</div>
              </div>
              <div class="col-4">
                <q-icon name="flash_off" color="grey-7" />
                <div class="text-caption text-grey-7">Sin reflejos ni borroso</div>
              </div>
              <div class="col-4">
                <q-icon name="crop_free" color="grey-7" />
                <div class="text-caption text-grey-7">Bordes legibles</div>
              </div>
            </div>

            <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders q-mb-md">
              {{ errorMensaje }}
            </q-banner>

            <q-btn
              label="Enviar para Revisión"
              color="positive"
              unelevated
              class="full-width"
              :disable="!archivoFrontal || !archivoPosterior"
              :loading="enviando"
              @click="enviarSolicitud"
            />
          </div>

          <div v-else class="text-center">
            <q-icon name="hourglass_top" size="48px" color="primary" />
            <div class="text-h6 text-weight-bold q-mt-sm">Solicitud Enviada</div>
            <div class="text-caption text-grey-7 q-mb-md">
              Tu documento está siendo revisado por nuestro equipo. Te notificaremos cuando se complete la verificación.
            </div>
            <q-btn label="Volver al Marketplace" color="dark" unelevated to="/marketplace" />
          </div>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth-store'
import { verificacionService } from '../services/verificacionService'
import { notificacionService } from '../services/notificacionService'

const router = useRouter()
const authStore = useAuthStore()

const archivoFrontal = ref(null)
const archivoPosterior = ref(null)
const previewFrontal = ref(null)
const previewPosterior = ref(null)
const enviando = ref(false)
const enviado = ref(false)
const errorMensaje = ref('')
const noLeidas = ref(0)

function onSeleccionar (evento, lado) {
  const archivo = evento.target.files[0]
  if (!archivo) return

  const url = URL.createObjectURL(archivo)

  if (lado === 'frontal') {
    archivoFrontal.value = archivo
    previewFrontal.value = url
  } else {
    archivoPosterior.value = archivo
    previewPosterior.value = url
  }
}

async function enviarSolicitud () {
  errorMensaje.value = ''
  enviando.value = true
  try {
    // Subimos la imagen frontal como la imagen principal de verificación
    const rutaFrontal = await verificacionService.subirImagen(archivoFrontal.value)
    await verificacionService.subirImagen(archivoPosterior.value)

    await verificacionService.solicitarVerificacion(rutaFrontal)
    enviado.value = true
  } catch (error) {
    errorMensaje.value = error.response?.data?.mensaje || 'No se pudo enviar la solicitud.'
  } finally {
    enviando.value = false
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  notificacionService.contarNoLeidas(authStore.usuarioId).then(n => { noLeidas.value = n })
})
</script>

<style scoped>
.verificacion-card {
  width: 100%;
  max-width: 600px;
}

.zona-subida {
  min-height: 140px;
  border-style: dashed;
  overflow: hidden;
}
</style>
