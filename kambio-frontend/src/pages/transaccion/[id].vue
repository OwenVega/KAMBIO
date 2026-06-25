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
              <div v-if="transaccion.estadoNombre === 'Completada'" class="q-mb-md">
                <q-separator class="q-mb-md" />
                <div v-if="!yaCalificado" class="text-center">
                  <q-icon name="check_circle" color="positive" size="48px" />
                  <div class="text-h6 text-weight-bold q-mt-sm">¡Transacción Finalizada!</div>
                  <div class="text-caption text-grey-7 q-mb-md">Has completado el intercambio con éxito.</div>

                  <div class="text-subtitle2 q-mb-sm">Califica tu experiencia</div>
                  <q-rating
                    v-model="estrellas"
                    size="2.5em"
                    color="amber"
                    icon="star_border"
                    icon-selected="star"
                    :max="5"
                  />

                  <q-input
                    v-model="comentarioCalificacion"
                    label="Comentario (opcional)"
                    outlined
                    dense
                    class="q-mt-md"
                    type="textarea"
                    rows="2"
                  />

                  <q-btn
                    label="Enviar Calificación"
                    color="dark"
                    unelevated
                    class="full-width q-mt-md"
                    :disable="estrellas === 0"
                    :loading="enviandoCalificacion"
                    @click="enviarCalificacion"
                  />
                </div>

                <q-banner v-else class="bg-green-1 text-green-9 rounded-borders text-center">
                  <q-icon name="star" color="amber" size="20px" />
                  Calificaste esta transacción con {{ estrellas }} estrella{{ estrellas > 1 ? 's' : '' }}.
                </q-banner>
              </div>
              <div v-if="!['Cancelada'].includes(transaccion.estadoNombre)" class="q-mb-md">
                <q-separator class="q-mb-md" />
                <q-expansion-item icon="chat" label="Chat de la Transacción" default-opened>
                  <q-card flat bordered class="q-mt-sm">
                    <div ref="contenedorMensajes" class="chat-mensajes q-pa-md">
                      <div
                        v-for="msg in mensajes"
                        :key="msg.idMensaje"
                        class="row q-mb-sm"
                        :class="esMio(msg) ? 'justify-end' : 'justify-start'"
                      >
                        <div class="chat-burbuja" :class="esMio(msg) ? 'chat-mia' : 'chat-otro'">
                          <div v-if="!esMio(msg)" class="text-caption text-weight-bold">{{ msg.nombreUsuarioEnvia }}</div>
                          <div>{{ msg.mensaje }}</div>
                          <div class="text-caption" :class="esMio(msg) ? 'text-grey-3' : 'text-grey-6'">
                            {{ formatearHora(msg.fechaEnvio) }}
                          </div>
                        </div>
                      </div>

                      <div v-if="mensajes.length === 0" class="text-center text-grey-6 q-py-md">
                        Todavía no hay mensajes. Escribe el primero.
                      </div>
                    </div>

                    <q-separator />

                    <div class="row q-pa-sm q-gutter-sm items-center">
                      <q-input
                        v-model="nuevoMensaje"
                        placeholder="Escribe un mensaje seguro..."
                        outlined
                        dense
                        class="col"
                        @keyup.enter="enviarMensaje"
                      />
                      <q-btn round color="primary" icon="send" :loading="enviandoMensaje" @click="enviarMensaje" />
                    </div>
                  </q-card>
                </q-expansion-item>
              </div>

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
                <q-btn
                  v-if="!['Completada', 'Cancelada'].includes(transaccion.estadoNombre)"
                  label="Reportar Problema"
                  flat
                  color="grey-7"
                  class="col"
                  icon="report_problem"
                  @click="mostrarDialogoDisputa = true"
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
        <q-dialog v-model="mostrarDialogoDisputa">
      <q-card style="min-width: 400px" class="q-pa-md">
        <q-card-section>
          <div class="text-h6 text-weight-bold">Reportar Problema</div>
          <div class="text-caption text-grey-7">
            Describe el problema que tuviste con esta transacción. Nuestro equipo lo revisará.
          </div>
        </q-card-section>

        <q-card-section>
          <q-input
            v-model="descripcionDisputa"
            type="textarea"
            rows="4"
            outlined
            label="Descripción del problema"
            placeholder="Ej: La transferencia se realizó hace 3 horas pero el destinatario dice que no ha recibido nada."
          />

          <q-banner v-if="errorDisputa" class="bg-red-1 text-red-9 rounded-borders q-mt-sm">
            {{ errorDisputa }}
          </q-banner>

          <div class="row q-gutter-sm justify-end q-mt-md">
            <q-btn label="Cancelar" flat v-close-popup @click="descripcionDisputa = ''" />
            <q-btn
              label="Enviar Reporte"
              color="negative"
              unelevated
              :loading="enviandoDisputa"
              :disable="!descripcionDisputa.trim()"
              @click="reportarProblema"
            />
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-layout>
</template>

<script setup>
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth-store'
import { transaccionService } from '../../services/transaccionService'
import { comprobanteService } from '../../services/comprobanteService'
import { useQuasar } from 'quasar'
import { calificacionService } from '../../services/calificacionService'
import { chatService } from '../../services/chatService'
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { disputaService } from '../../services/disputaService'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()
const estrellas = ref(0)
const comentarioCalificacion = ref('')
const enviandoCalificacion = ref(false)
const yaCalificado = ref(false)
const mensajes = ref([])
const nuevoMensaje = ref('')
const enviandoMensaje = ref(false)
const contenedorMensajes = ref(null)
let intervaloChat = null
const mostrarDialogoDisputa = ref(false)
const descripcionDisputa = ref('')
const enviandoDisputa = ref(false)
const errorDisputa = ref('')
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
async function enviarCalificacion () {
  enviandoCalificacion.value = true
  try {
    const idEvaluado = String(transaccion.value.idUsuarioComprador) === String(authStore.usuarioId)
      ? transaccion.value.idUsuarioVendedor
      : transaccion.value.idUsuarioComprador

    await calificacionService.calificar({
      idTransaccion: transaccion.value.idTransaccion,
      idUsuarioEvalua: authStore.usuarioId,
      idUsuarioEvaluado: idEvaluado,
      estrellas: estrellas.value,
      comentario: comentarioCalificacion.value
    })
    yaCalificado.value = true
  } catch (error) {
    const mensaje = error.response?.data?.error || 'No se pudo registrar la calificación.'
    alert(mensaje)
  } finally {
    enviandoCalificacion.value = false
  }
}
function esMio (msg) {
  return String(msg.idUsuarioEnvia) === String(authStore.usuarioId)
}

function formatearHora (fecha) {
  return new Date(fecha).toLocaleTimeString('es-PE', { hour: '2-digit', minute: '2-digit' })
}

async function cargarMensajes () {
  try {
    mensajes.value = await chatService.obtenerMensajes(transaccion.value.idTransaccion, authStore.usuarioId)
  } catch (error) {
    console.error('Error al cargar mensajes:', error)
  }
}
async function reportarProblema () {
  errorDisputa.value = ''
  enviandoDisputa.value = true
  try {
    await disputaService.crearDisputa(
      transaccion.value.idTransaccion,
      authStore.usuarioId,
      descripcionDisputa.value
    )
    mostrarDialogoDisputa.value = false
    descripcionDisputa.value = ''
    alert('Tu reporte fue enviado. Nuestro equipo lo revisará pronto.')
  } catch (error) {
    errorDisputa.value = error.response?.data?.mensaje || 'No se pudo enviar el reporte.'
  } finally {
    enviandoDisputa.value = false
  }
}

async function enviarMensaje () {
  if (!nuevoMensaje.value.trim()) return
  enviandoMensaje.value = true
  try {
    await chatService.enviarMensaje(transaccion.value.idTransaccion, nuevoMensaje.value, authStore.usuarioId)
    nuevoMensaje.value = ''
    await cargarMensajes()
  } catch (error) {
    console.error('Error al enviar mensaje:', error)
  } finally {
    enviandoMensaje.value = false
  }
}
function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarTransaccion()
  cargarMensajes()
  intervaloChat = setInterval(cargarMensajes, 5000)
})

onUnmounted(() => {
  if (intervaloChat) clearInterval(intervaloChat)
})
</script>


<style scoped>
.chat-mensajes {
  max-height: 300px;
  overflow-y: auto;
}

.chat-burbuja {
  max-width: 75%;
  padding: 8px 12px;
  border-radius: 12px;
}

.chat-mia {
  background: #1a1a1a;
  color: white;
}

.chat-otro {
  background: #f0f0f0;
  color: #1a1a1a;
}
</style>
