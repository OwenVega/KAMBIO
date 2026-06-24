<template>
  <q-layout view="lHh Lpr lFf" container style="height: 100vh" class="bg-grey-2">
    <q-header bordered class="bg-white text-dark">
      <q-toolbar class="q-px-lg">
        <q-btn flat round icon="arrow_back" to="/marketplace" />
        <q-toolbar-title class="text-weight-bold text-dark">
          Kambio
        </q-toolbar-title>
        <q-space />
        <q-btn flat round icon="account_circle" @click="cerrarSesion" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <q-page class="q-pa-lg flex flex-center" v-if="cargando">
        <q-spinner color="primary" size="3em" />
      </q-page>

      <q-page class="q-pa-lg" v-else-if="transaccion">
        <div class="row q-col-gutter-md justify-center">
          <div class="col-12 col-md-7">
            <q-card flat bordered class="q-pa-lg">
              <div class="row items-center justify-between q-mb-md">
                <div>
                  <div class="text-overline text-grey-7">Operación #{{ transaccion.idTransaccion }}</div>
                  <div class="text-h6 text-weight-bold">Estado de Transacción</div>
                </div>
                <q-chip :color="colorEstado" text-color="white" class="text-weight-bold">
                  {{ transaccion.estadoNombre }}
                </q-chip>
              </div>

              <q-banner v-if="transaccion.estadoNombre === 'Pendiente'" class="bg-blue-1 text-blue-9 rounded-borders q-mb-md">
                <template v-slot:avatar>
                  <q-icon name="info" color="blue-9" />
                </template>
                Por favor, realiza la transferencia a la cuenta indicada para completar el cambio de divisas.
              </q-banner>

              <q-separator class="q-mb-md" />

              <div class="row q-col-gutter-md">
                <div class="col-6">
                  <div class="text-caption text-grey-7">Tipo de Operación</div>
                  <div class="text-weight-medium">{{ tipoOperacionPersonal }}</div>
                </div>
                <div class="col-6 text-right">
                  <div class="text-caption text-grey-7">Recibes</div>
                  <div class="text-h6 text-weight-bold text-positive">
                    {{ formatearMoneda(transaccion.montoEquivalente) }}
                  </div>
                </div>
              </div>

              <q-separator class="q-my-md" />

              <div class="row q-col-gutter-md">
                <div class="col-6">
                  <div class="text-caption text-grey-7">Tipo de Cambio</div>
                  <div class="text-weight-medium">{{ transaccion.tasaCambioAplicada }}</div>
                </div>
                <div class="col-6">
                  <div class="text-caption text-grey-7">Tipo de Operación</div>
                  <div class="text-weight-medium">{{ tipoOperacionPersonal }}</div>
                </div>
              </div>
              <div v-if="transaccion.estadoNombre === 'En Proceso'" class="q-mb-md">
                  <q-separator class="q-mb-md" />
                  <div class="text-caption text-grey-7 q-mb-sm">Comprobante de Pago</div>
                  <q-list v-if="comprobanteSubido" bordered separator class="rounded-borders">
                    <q-item>
                      <q-item-section avatar>
                        <q-icon name="description" color="primary" />
                      </q-item-section>
                      <q-item-section>
                        <q-item-label class="text-weight-medium">{{ nombreArchivoSubido }}</q-item-label>
                        <q-item-label caption>Subido el {{ new Date().toLocaleString('es-PE', { dateStyle: 'medium', timeStyle: 'short' }) }}</q-item-label>
                      </q-item-section>
                      <q-item-section side>
                        <q-icon name="check_circle" color="positive" />
                      </q-item-section>
                    </q-item>
                  </q-list>
                  <div v-else>
                    <q-file
                      v-model="archivoComprobante"
                      label="Subir imagen del voucher (JPG, PNG)"
                      outlined
                      dense
                      accept="image/jpeg,image/png"
                      @update:model-value="subirComprobante"
                    >
                      <template v-slot:prepend>
                        <q-icon name="attach_file" />
                      </template>
                    </q-file>
                    <q-linear-progress v-if="subiendoComprobante" indeterminate color="primary" class="q-mt-sm" />
                  </div>
                </div>

              <q-separator class="q-my-md" />

              <div class="text-caption text-grey-7 q-mb-sm">Historial de Estados</div>
              <q-timeline color="primary" dense>
                <q-timeline-entry
                  title="Pendiente"
                  :subtitle="formatearFecha(transaccion.fechaInicio)"
                  icon="schedule"
                  :color="transaccion.estadoNombre === 'Pendiente' ? 'primary' : 'grey-5'"
                />
                <q-timeline-entry
                  v-if="transaccion.fechaConfirmacionPago"
                  title="Pago Realizado"
                  :subtitle="formatearFecha(transaccion.fechaConfirmacionPago)"
                  icon="payments"
                  color="primary"
                />
                <q-timeline-entry
                  v-if="transaccion.fechaCompletado"
                  title="Completada"
                  :subtitle="formatearFecha(transaccion.fechaCompletado)"
                  icon="check_circle"
                  color="positive"
                />
              </q-timeline>

              <q-separator class="q-my-md" />

              <div class="row q-gutter-sm">
                <q-btn
                  v-if="['Pendiente', 'En Proceso'].includes(transaccion.estadoNombre)"
                  :label="transaccion.estadoNombre === 'Pendiente' ? 'Marcar En Proceso' : 'Confirmar Pago Realizado'"
                  color="positive"
                  unelevated
                  class="col"
                  :loading="actualizando"
                  @click="confirmarPago"
                />
                <q-btn
                  v-if="transaccion.estadoNombre === 'Pago Realizado'"
                  label="Marcar como Completada"
                  color="primary"
                  unelevated
                  class="col"
                  :loading="actualizando"
                  @click="completarTransaccion"
                  />
                <q-btn
                  v-if="['Pendiente', 'En Proceso'].includes(transaccion.estadoNombre)"
                  label="Cancelar Transacción"
                  flat
                  color="negative"
                  class="col"
                  :loading="actualizando"
                  @click="cancelarTransaccion"
                />
              </div>
            </q-card>
          </div>
        </div>
      </q-page>

      <q-page class="q-pa-lg flex flex-center" v-else>
        <div class="text-center text-grey-7">
          <q-icon name="error_outline" size="48px" />
          <div class="q-mt-sm">No se pudo cargar la transacción.</div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth-store'
import { transaccionService } from '../../services/transaccionService'
import { comprobanteService } from '../../services/comprobanteService'
import { useQuasar } from 'quasar'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const transaccion = ref(null)
const cargando = ref(true)
const actualizando = ref(false)
const archivoComprobante = ref(null)
const subiendoComprobante = ref(false)
const nombreArchivoSubido = ref('')
const comprobanteSubido = ref(false)

const colorEstado = computed(() => {
  const mapa = {
    Pendiente: 'orange',
    'En Proceso': 'blue',
    'Pago Realizado': 'teal',
    Completada: 'positive',
    Cancelada: 'negative',
    'En Disputa': 'deep-orange'
  }
  return mapa[transaccion.value?.estadoNombre] || 'grey'
})
const tipoOperacionPersonal = computed(() => {
  if (!transaccion.value) return ''
  const esComprador = String(transaccion.value.idUsuarioComprador) === String(authStore.usuarioId)
  return esComprador ? 'Compra' : 'Venta'
})

function formatearMoneda (valor) {
  if (valor == null) return '0.00'
  return Number(valor).toLocaleString('es-PE', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

function formatearFecha (fecha) {
  if (!fecha) return ''
  return new Date(fecha).toLocaleString('es-PE', { dateStyle: 'medium', timeStyle: 'short' })
}

async function cargarTransaccion () {
  cargando.value = true
  try {
    const idTransaccion = route.params.id
    transaccion.value = await transaccionService.obtenerPorId(idTransaccion)
  } catch (error) {
    console.error('Error al cargar la transacción:', error)
    transaccion.value = null
  } finally {
    cargando.value = false
  }
}

async function confirmarPago () {
  actualizando.value = true
  try {
    const siguienteEstado = transaccion.value.estadoNombre === 'Pendiente' ? 2 : 3
    await transaccionService.cambiarEstado({
      idTransaccion: transaccion.value.idTransaccion,
      idEstadoTransaccion: siguienteEstado,
      idUsuarioCambio: authStore.usuarioId,
      observacion: 'Pago confirmado por el usuario.'
    })
    await cargarTransaccion()
  } catch (error) {
    const mensaje = error.response?.data?.error || 'No se pudo confirmar el pago.'
    alert(mensaje)
  } finally {
    actualizando.value = false
  }
}
async function subirComprobante (archivo) {
  if (!archivo) return
  subiendoComprobante.value = true
  try {
    await comprobanteService.subirComprobante(transaccion.value.idTransaccion, authStore.usuarioId, archivo)
    comprobanteSubido.value = true
    nombreArchivoSubido.value = archivo.name
    $q.notify({ type: 'positive', message: 'Comprobante subido correctamente.', icon: 'check_circle' })
  } catch (error) {
    const mensaje = error.response?.data?.error || 'No se pudo subir el comprobante.'
    alert(mensaje)
  } finally {
    subiendoComprobante.value = false
  }
}
async function completarTransaccion () {
  actualizando.value = true
  try {
    await transaccionService.cambiarEstado({
      idTransaccion: transaccion.value.idTransaccion,
      idEstadoTransaccion: 4, // Completada
      idUsuarioCambio: authStore.usuarioId,
      observacion: 'Transacción completada por el usuario.'
    })
    await cargarTransaccion()
  } catch (error) {
    const mensaje = error.response?.data?.error || 'No se pudo completar la transacción.'
    alert(mensaje)
  } finally {
    actualizando.value = false
  }
}

async function cancelarTransaccion () {
  actualizando.value = true
  try {
    await transaccionService.cambiarEstado({
      idTransaccion: transaccion.value.idTransaccion,
      idEstadoTransaccion: 5, // Cancelada
      idUsuarioCambio: authStore.usuarioId,
      observacion: 'Cancelada por el usuario.'
    })
    await cargarTransaccion()
  } catch (error) {
    const mensaje = error.response?.data?.error || 'No se pudo cancelar la transacción.'
    alert(mensaje)
  } finally {
    actualizando.value = false
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarTransaccion()
})
</script>
