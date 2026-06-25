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
        <div class="text-h6 text-weight-bold">Configuración de Alertas</div>
        <div class="text-caption text-grey-7 q-mb-md">
          Configura umbrales personalizados para recibir notificaciones cuando el tipo de cambio alcance el valor deseado.
        </div>

        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-lg">
              <div class="text-subtitle1 text-weight-bold q-mb-md">Nueva Alerta</div>

              <q-form @submit.prevent="crearAlerta" class="q-gutter-md">
                <q-select
                  v-model="form.idDivisaOrigen"
                  :options="opcionesDivisa"
                  label="Divisa Origen"
                  outlined
                  dense
                  emit-value
                  map-options
                />

                <q-select
                  v-model="form.idDivisaDestino"
                  :options="opcionesDivisa"
                  label="Divisa Destino"
                  outlined
                  dense
                  emit-value
                  map-options
                />

                <q-input
                  v-model.number="form.valorUmbral"
                  label="Valor Umbral"
                  outlined
                  dense
                  type="number"
                  step="0.01"
                  :hint="`Te avisaremos cuando ${codigoDivisa(form.idDivisaOrigen)}/${codigoDivisa(form.idDivisaDestino)} alcance este valor`"
                  :rules="[val => !!val && val > 0 || 'Obligatorio']"
                />

                <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders">
                  {{ errorMensaje }}
                </q-banner>

                <q-btn
                  type="submit"
                  label="Crear Alerta de Cambio"
                  color="dark"
                  unelevated
                  class="full-width"
                  :loading="creando"
                />
              </q-form>

              <q-banner class="bg-blue-1 text-blue-9 rounded-borders q-mt-md">
                <template v-slot:avatar>
                  <q-icon name="info" color="blue-9" />
                </template>
                <div class="text-weight-bold">¿Cómo funcionan?</div>
                Te enviaremos una notificación cuando el mercado alcance el precio que has configurado.
              </q-banner>
            </q-card>
          </div>

          <div class="col-12 col-md-8">
            <q-card flat bordered>
              <q-card-section class="row items-center justify-between">
                <div class="text-subtitle1 text-weight-bold">Alertas Activas</div>
                <q-chip color="green-1" text-color="green-9" dense>{{ alertas.length }} alertas</q-chip>
              </q-card-section>

              <q-separator />

              <q-list separator>
                <q-item v-for="alerta in alertas" :key="alerta.idAlerta">
                  <q-item-section avatar>
                    <q-icon
                      :name="alerta.activa ? 'trending_up' : 'trending_flat'"
                      :color="alerta.activa ? 'positive' : 'grey-5'"
                    />
                  </q-item-section>

                  <q-item-section>
                    <q-item-label class="text-weight-medium">
                      {{ alerta.divisaOrigen }} / {{ alerta.divisaDestino }}
                    </q-item-label>
                    <q-item-label caption>
                      Avisar en {{ alerta.valorUmbral }}
                    </q-item-label>
                  </q-item-section>

                  <q-item-section side>
                    <div class="row items-center q-gutter-sm">
                      <q-toggle
                        :model-value="alerta.activa"
                        color="positive"
                        @update:model-value="val => cambiarActiva(alerta, val)"
                      />
                      <q-btn
                        flat
                        round
                        dense
                        icon="delete"
                        color="negative"
                        @click="confirmarEliminar(alerta)"
                      />
                    </div>
                  </q-item-section>
                </q-item>

                <q-item v-if="!cargando && alertas.length === 0">
                  <q-item-section class="text-center text-grey-7 q-py-lg">
                    <q-icon name="notifications_none" size="48px" class="q-mb-sm" />
                    Todavía no tienes alertas configuradas.
                  </q-item-section>
                </q-item>
              </q-list>
            </q-card>
          </div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '../stores/auth-store'
import { alertaService } from '../services/alertaService'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const alertas = ref([])
const cargando = ref(false)
const creando = ref(false)
const errorMensaje = ref('')

const form = reactive({
  idDivisaOrigen: 1,
  idDivisaDestino: 2,
  valorUmbral: null
})

const opcionesDivisa = [
  { label: 'USD - Dólar Estadounidense', value: 1 },
  { label: 'PEN - Sol Peruano', value: 2 },
  { label: 'EUR - Euro', value: 3 },
  { label: 'GBP - Libra Esterlina', value: 4 },
  { label: 'JPY - Yen Japonés', value: 5 },
  { label: 'CHF - Franco Suizo', value: 6 }
]

const codigosDivisa = { 1: 'USD', 2: 'PEN', 3: 'EUR', 4: 'GBP', 5: 'JPY', 6: 'CHF' }
function codigoDivisa (id) {
  return codigosDivisa[id] || ''
}

async function cargarAlertas () {
  cargando.value = true
  try {
    alertas.value = await alertaService.obtenerAlertas()
  } catch (error) {
    console.error('Error al cargar alertas:', error)
  } finally {
    cargando.value = false
  }
}

async function crearAlerta () {
  errorMensaje.value = ''
  creando.value = true
  try {
    await alertaService.crearAlerta(form)
    form.valorUmbral = null
    await cargarAlertas()
  } catch (error) {
    errorMensaje.value = error.response?.data?.mensaje || 'No se pudo crear la alerta.'
  } finally {
    creando.value = false
  }
}

async function cambiarActiva (alerta, nuevoValor) {
  try {
    await alertaService.actualizarAlerta(alerta.idAlerta, {
      valorUmbral: alerta.valorUmbral,
      activa: nuevoValor
    })
    await cargarAlertas()
  } catch {
    $q.notify({ type: 'negative', message: 'No se pudo actualizar la alerta.' })
  }
}

function confirmarEliminar (alerta) {
  $q.dialog({
    title: 'Eliminar alerta',
    message: `¿Eliminar la alerta de ${codigoDivisa(alerta.idDivisaOrigen)}/${codigoDivisa(alerta.idDivisaDestino)}?`,
    cancel: true,
    persistent: true
  }).onOk(async () => {
    try {
      await alertaService.eliminarAlerta(alerta.idAlerta)
      await cargarAlertas()
    } catch {
      $q.notify({ type: 'negative', message: 'No se pudo eliminar la alerta.' })
    }
  })
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarAlertas()
})
</script>
