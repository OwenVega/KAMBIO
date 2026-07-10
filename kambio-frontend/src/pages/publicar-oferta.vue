<template>
  <q-layout view="lHh Lpr lFf" container style="height: 100vh" class="bg-grey-2">
    <q-header bordered class="bg-white text-dark">
      <q-toolbar class="q-px-lg">
        <q-toolbar-title class="text-weight-bold text-dark">
          Kambio
        </q-toolbar-title>

        <q-tabs class="text-grey-7" active-color="primary" indicator-color="primary">
          <q-tab name="intercambio" label="Intercambio" no-caps to="/marketplace" />
          <q-tab name="ofertas" label="Ofertas" no-caps />
          <q-tab name="historial" label="Historial" no-caps />
        </q-tabs>

        <q-space />

        <q-btn flat round icon="account_circle" @click="cerrarSesion">
          <q-tooltip>{{ authStore.nombres }} — Cerrar sesión</q-tooltip>
        </q-btn>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <q-page class="q-pa-lg flex flex-center">
        <q-card flat bordered class="publicar-card q-pa-lg">
          <q-tabs
            v-model="tipoPublicacion"
            class="q-mb-md"
            active-color="dark"
            indicator-color="dark"
            no-caps
          >
            <q-tab name="compra" label="Publicar Oferta de Compra" />
            <q-tab name="venta" label="Publicar Oferta de Venta" />
          </q-tabs>

          <q-card-section class="q-pt-none">
            <div class="text-caption text-grey-7 q-mb-md">
              {{ tipoPublicacion === 'compra'
                ? 'Crea una nueva propuesta de intercambio para la comunidad.'
                : 'Vende tus divisas de forma segura al tipo de cambio que tú decides.' }}
            </div>

            <q-form @submit.prevent="onSubmit">
              <div class="row q-col-gutter-md q-mb-md">
                <div class="col-6">
                  <q-select
                    v-model="form.idDivisaOrigen"
                    :options="opcionesDivisa"
                    :label="tipoPublicacion === 'compra' ? 'Tengo' : 'Tengo (a vender)'"
                    outlined
                    dense
                    emit-value
                    map-options
                  />
                </div>
                <div class="col-6">
                  <q-select
                    v-model="form.idDivisaDestino"
                    :options="opcionesDivisa"
                    label="Quiero Recibir"
                    outlined
                    dense
                    emit-value
                    map-options
                  />
                </div>
              </div>

              <q-input
                v-model.number="form.tasaCambio"
                label="Tipo de Cambio Propuesto"
                outlined
                dense
                type="number"
                step="0.001"
                :suffix="`${codigoDestino}/${codigoOrigen}`"
                lazy-rules
                class="q-mb-md"
                :rules="[val => !!val && val > 0 || 'El tipo de cambio debe ser mayor a 0']"
              />

              <div class="row q-col-gutter-md q-mb-md">
                <div class="col-6">
                  <q-input
                    v-model.number="form.montoMinimo"
                    label="Monto Mínimo"
                    outlined
                    dense
                    type="number"
                    lazy-rules
                    :rules="[val => !!val && val > 0 || 'Obligatorio']"
                  />
                </div>
                <div class="col-6">
                  <q-input
                    v-model.number="form.montoMaximo"
                    label="Monto Máximo"
                    outlined
                    dense
                    type="number"
                    lazy-rules
                    :rules="[val => !!val && val > 0 || 'Obligatorio']"
                  />
                </div>
              </div>

              <q-input
                v-model.number="form.montoDisponible"
                label="Monto Disponible Total"
                outlined
                dense
                type="number"
                lazy-rules
                class="q-mb-md"
                :rules="[val => !!val && val > 0 || 'Obligatorio']"
              />

              <q-select
                v-model="form.metodosPago"
                :options="opcionesBanco"
                label="Métodos de Pago Aceptados"
                outlined
                dense
                multiple
                emit-value
                map-options
                use-chips
                lazy-rules
                class="q-mb-md"
                :rules="[val => val && val.length > 0 || 'Selecciona al menos un método']"
              />

              <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders q-mb-md">
                {{ errorMensaje }}
              </q-banner>

              <q-banner v-if="exitoMensaje" class="bg-green-1 text-green-9 rounded-borders q-mb-md">
                {{ exitoMensaje }}
              </q-banner>

              <q-btn
                type="submit"
                :label="tipoPublicacion === 'compra' ? 'Publicar Oferta' : 'Publicar Oferta de Venta'"
                :color="tipoPublicacion === 'compra' ? 'dark' : 'positive'"
                unelevated
                class="full-width q-mb-sm"
                :loading="cargando"
              />
              <q-btn
                label="Cancelar"
                flat
                class="full-width"
                @click="resetForm"
              />
            </q-form>
          </q-card-section>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, reactive, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth-store'
import { ofertaService } from '../services/ofertaService'

const router = useRouter()
const authStore = useAuthStore()

const tipoPublicacion = ref('compra')
const cargando = ref(false)
const errorMensaje = ref('')
const exitoMensaje = ref('')

const form = reactive({
  idDivisaOrigen: 1,
  idDivisaDestino: 2,
  tasaCambio: null,
  montoMinimo: null,
  montoMaximo: null,
  montoDisponible: null,
  metodosPago: []
})

const opcionesDivisa = [
  { label: 'USD - Dólar Estadounidense', value: 1 },
  { label: 'PEN - Sol Peruano', value: 2 },
  { label: 'EUR - Euro', value: 3 },
  { label: 'GBP - Libra Esterlina', value: 4 },
  { label: 'JPY - Yen Japonés', value: 5 },
  { label: 'CHF - Franco Suizo', value: 6 }
]

const opcionesBanco = [
  { label: 'BCP', value: 1 },
  { label: 'Interbank', value: 2 },
  { label: 'BBVA', value: 3 },
  { label: 'Scotiabank', value: 4 },
  { label: 'BanBif', value: 5 },
  { label: 'Yape', value: 6 },
  { label: 'Plin', value: 7 }
]

const codigosDivisa = { 1: 'USD', 2: 'PEN', 3: 'EUR', 4: 'GBP', 5: 'JPY', 6: 'CHF' }
const codigoOrigen = computed(() => codigosDivisa[form.idDivisaOrigen] || '')
const codigoDestino = computed(() => codigosDivisa[form.idDivisaDestino] || '')

watch(tipoPublicacion, () => {
  resetForm()
})

function resetForm () {
  form.tasaCambio = null
  form.montoMinimo = null
  form.montoMaximo = null
  form.montoDisponible = null
  form.metodosPago = []
  errorMensaje.value = ''
  exitoMensaje.value = ''
}

async function onSubmit () {
  errorMensaje.value = ''
  exitoMensaje.value = ''
  cargando.value = true

  try {
    if (tipoPublicacion.value === 'compra') {
      await ofertaService.crearOfertaCompra(authStore.usuarioId, {
        idDivisaOrigen: form.idDivisaOrigen,
        idDivisaDestino: form.idDivisaDestino,
        montoDisponible: form.montoDisponible,
        montoMinimo: form.montoMinimo,
        montoMaximo: form.montoMaximo,
        tasaCambio: form.tasaCambio,
        metodosPago: form.metodosPago
      })
    } else {
      await ofertaService.crearOfertaVenta(authStore.usuarioId, {
        idDivisaOrigen: form.idDivisaOrigen,
        idDivisaDestino: form.idDivisaDestino,
        montoDisponible: form.montoDisponible,
        montoMinimo: form.montoMinimo,
        montoMaximo: form.montoMaximo,
        tasaCambio: form.tasaCambio,
        idBancos: form.metodosPago
      })
    }

    exitoMensaje.value = 'Oferta publicada correctamente. Redirigiendo al mercado...'

    setTimeout(() => {
      router.push('/marketplace')
    }, 1500)
  } catch (error) {
    if (error.response?.data?.error) {
      errorMensaje.value = error.response.data.error
    } else if (error.response?.data?.mensaje) {
      errorMensaje.value = error.response.data.mensaje
    } else {
      errorMensaje.value = 'Ocurrió un error al publicar la oferta. Intenta de nuevo.'
    }
  } finally {
    cargando.value = false
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}
</script>

<style scoped>
.publicar-card {
  width: 100%;
  max-width: 480px;
}
</style>
