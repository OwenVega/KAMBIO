<template>
  <q-layout view="lHh Lpr lFf" container style="height: 100vh" class="bg-grey-2">
    <q-header bordered class="bg-dark text-white">
      <q-toolbar class="q-px-lg">
        <q-toolbar-title class="text-weight-bold">
          Kambio Admin
        </q-toolbar-title>
        <q-space />
        <q-btn flat round icon="account_circle">
          <q-menu anchor="bottom right" self="top right">
            <q-list style="min-width: 200px">
              <q-item clickable v-close-popup to="/marketplace">
                <q-item-section avatar>
                  <q-icon name="storefront" />
                </q-item-section>
                <q-item-section>Volver al Marketplace</q-item-section>
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
      <q-page class="row no-wrap">
        <div class="q-pa-md bg-dark text-white" style="width: 220px; min-height: 100%;">
          <q-list>
            <q-item clickable to="/admin/dashboard">
              <q-item-section avatar>
                <q-icon name="dashboard" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Dashboard</q-item-section>
            </q-item>
            <q-item clickable to="/admin/usuarios">
              <q-item-section avatar>
                <q-icon name="people" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Usuarios</q-item-section>
            </q-item>
            <q-item clickable to="/admin/disputas" class="bg-primary rounded-borders">
              <q-item-section avatar>
                <q-icon name="gavel" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Disputas</q-item-section>
            </q-item>
            <q-item clickable to="/admin/reportes">
              <q-item-section avatar>
                <q-icon name="bar_chart" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Reportes</q-item-section>
            </q-item>
          </q-list>
        </div>

        <div class="col q-pa-lg">
          <div class="text-h6 text-weight-bold q-mb-md">Gestión de Disputas</div>

          <q-card flat bordered>
            <q-table
              :rows="disputas"
              :columns="columnas"
              row-key="idDisputa"
              flat
              :loading="cargando"
              hide-pagination
              :rows-per-page-options="[0]"
            >
              <template v-slot:body-cell-estado="props">
                <q-td :props="props">
                  <q-chip dense size="sm" :color="colorEstado(props.row.estado)" text-color="white">
                    {{ props.row.estado }}
                  </q-chip>
                </q-td>
              </template>

              <template v-slot:body-cell-accion="props">
                <q-td :props="props">
                  <q-btn
                    label="Ver detalles"
                    flat
                    dense
                    no-caps
                    size="sm"
                    color="primary"
                    @click="verDetalle(props.row)"
                  />
                </q-td>
              </template>

              <template v-slot:no-data>
                <div class="full-width text-center q-pa-lg text-grey-7">
                  <q-icon name="gavel" size="48px" class="q-mb-sm" />
                  No hay disputas registradas.
                </div>
              </template>
            </q-table>
          </q-card>
        </div>
      </q-page>
    </q-page-container>

    <q-dialog v-model="mostrarDetalle">
      <q-card style="min-width: 600px; max-width: 700px" class="q-pa-md" v-if="disputaSeleccionada">
        <q-inner-loading :showing="cargandoDetalle">
          <q-spinner color="primary" size="3em" />
        </q-inner-loading>

        <template v-if="!cargandoDetalle">
          <q-card-section>
            <div class="text-h6 text-weight-bold">Disputa #{{ disputaSeleccionada.idDisputa }}</div>
            <div class="text-caption text-grey-7">Transacción #{{ disputaSeleccionada.idTransaccion }}</div>
          </q-card-section>

          <q-card-section>
            <!-- Comprador / Vendedor -->
            <div class="row q-col-gutter-md q-mb-md">
              <div class="col-6">
                <q-card flat bordered :class="disputaSeleccionada.ladoReportante === 'Comprador' ? 'bg-red-1' : ''" class="q-pa-sm">
                  <div class="text-caption text-grey-7">Comprador</div>
                  <div class="text-weight-medium">{{ disputaSeleccionada.comprador }}</div>
                  <q-chip v-if="disputaSeleccionada.ladoReportante === 'Comprador'" dense size="sm" color="negative" text-color="white" class="q-mt-xs">
                    Reportante
                  </q-chip>
                </q-card>
              </div>
              <div class="col-6">
                <q-card flat bordered :class="disputaSeleccionada.ladoReportante === 'Vendedor' ? 'bg-red-1' : ''" class="q-pa-sm">
                  <div class="text-caption text-grey-7">Vendedor</div>
                  <div class="text-weight-medium">{{ disputaSeleccionada.vendedor }}</div>
                  <q-chip v-if="disputaSeleccionada.ladoReportante === 'Vendedor'" dense size="sm" color="negative" text-color="white" class="q-mt-xs">
                    Reportante
                  </q-chip>
                </q-card>
              </div>
            </div>

            <div class="text-caption text-grey-7">Descripción del problema</div>
            <div class="q-mb-sm">{{ disputaSeleccionada.descripcion }}</div>

            <div class="text-caption text-grey-7">Estado actual</div>
            <q-chip dense size="sm" :color="colorEstado(disputaSeleccionada.estado)" text-color="white" class="q-mb-md">
              {{ disputaSeleccionada.estado }}
            </q-chip>

            <!-- Comprobante -->
            <q-separator class="q-my-md" />
            <div class="text-subtitle2 q-mb-sm">Comprobante de Pago</div>
            <div v-if="comprobantes.length === 0" class="text-caption text-grey-6">
              No se ha subido ningún comprobante.
            </div>
            <div v-else class="row q-gutter-sm">
              <q-img
                v-for="c in comprobantes"
                :key="c.idComprobante"
                :src="urlComprobante(c.rutaImagen)"
                style="width: 150px; height: 150px; border-radius: 8px; cursor: pointer"
                @click="imagenAmpliada = urlComprobante(c.rutaImagen)"
              />
            </div>

            <!-- Chat -->
            <q-separator class="q-my-md" />
            <div class="text-subtitle2 q-mb-sm">Chat de la Transacción</div>
            <q-card flat bordered class="q-pa-sm" style="max-height: 250px; overflow-y: auto">
              <div v-if="mensajesChat.length === 0" class="text-caption text-grey-6 text-center q-py-md">
                No hay mensajes en esta conversación.
              </div>
              <div
                v-for="msg in mensajesChat"
                :key="msg.idMensaje"
                class="q-mb-sm"
              >
                <div class="text-caption text-weight-bold">{{ msg.nombreUsuarioEnvia }}</div>
                <div class="text-body2">{{ msg.mensaje }}</div>
                <div class="text-caption text-grey-6">{{ formatearHora(msg.fechaEnvio) }}</div>
              </div>
            </q-card>

            <!-- Acciones -->
            <q-separator class="q-my-md" />
            <div v-if="disputaSeleccionada.estado === 'Abierta'">
              <q-input
                v-model="resolucionDetalle"
                type="textarea"
                rows="3"
                outlined
                label="Detalle de la resolución"
                placeholder="Explica cómo se resolvió o por qué se rechaza"
              />

              <q-banner v-if="errorAccion" class="bg-red-1 text-red-9 rounded-borders q-mt-sm">
                {{ errorAccion }}
              </q-banner>

              <div class="row q-gutter-sm justify-end q-mt-md">
                <q-btn label="Rechazar Disputa" color="negative" flat :loading="procesando" @click="accionar('rechazar')" />
                <q-btn label="Marcar Resuelta" color="positive" unelevated :loading="procesando" @click="accionar('resolver')" />
              </div>
            </div>

            <div v-else>
              <div class="text-caption text-grey-7">Detalle de la resolución</div>
              <div>{{ disputaSeleccionada.resolucionDetalle || 'Sin detalle' }}</div>
            </div>
          </q-card-section>
        </template>
      </q-card>
    </q-dialog>



    <!-- Diálogo de sanción post-resolución -->
    <q-dialog v-model="mostrarDialogoBloqueo" persistent>
      <q-card style="min-width: 450px" class="q-pa-md" v-if="disputaResuelta">
        <q-card-section>
          <div class="text-h6 text-weight-bold">¿Deseas sancionar a alguien?</div>
          <div class="text-caption text-grey-7">Transacción #{{ disputaResuelta.idTransaccion }} — elige a quién, si aplica</div>
        </q-card-section>

        <q-card-section>
          <div class="row q-col-gutter-md q-mb-md">
            <div class="col-6">
              <q-card
                flat bordered
                class="q-pa-sm cursor-pointer"
                :class="usuarioObjetivo?.lado === 'Comprador' ? 'bg-red-1' : ''"
                @click="elegirUsuarioObjetivo('Comprador')"
              >
                <div class="text-caption text-grey-7">Comprador</div>
                <div class="text-weight-medium">{{ disputaResuelta.comprador }}</div>
              </q-card>
            </div>
            <div class="col-6">
              <q-card
                flat bordered
                class="q-pa-sm cursor-pointer"
                :class="usuarioObjetivo?.lado === 'Vendedor' ? 'bg-red-1' : ''"
                @click="elegirUsuarioObjetivo('Vendedor')"
              >
                <div class="text-caption text-grey-7">Vendedor</div>
                <div class="text-weight-medium">{{ disputaResuelta.vendedor }}</div>
              </q-card>
            </div>
          </div>

          <template v-if="usuarioObjetivo">
            <q-option-group
              v-model="nuevoEstadoCuenta"
              :options="[
                { label: 'Suspender cuenta', value: 2 },
                { label: 'Bloquear cuenta', value: 3 }
              ]"
              color="negative"
              class="q-mb-md"
            />

            <q-input
              v-model="motivoBloqueo"
              type="textarea"
              rows="4"
              outlined
              label="Motivo (editable)"
            />

            <q-banner v-if="errorBloqueo" class="bg-red-1 text-red-9 rounded-borders q-mt-sm">
              {{ errorBloqueo }}
            </q-banner>
          </template>

          <div class="row q-gutter-sm justify-end q-mt-md">
            <q-btn label="No sancionar a nadie" flat color="grey-7" v-close-popup />
            <q-btn
              v-if="usuarioObjetivo"
              label="Confirmar sanción"
              color="negative"
              unelevated
              :loading="procesandoBloqueo"
              @click="confirmarBloqueo"
            />
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>



    <!-- Imagen ampliada -->
    <q-dialog v-model="mostrarImagenAmpliada">
      <q-img :src="imagenAmpliada" style="max-width: 90vw; max-height: 90vh" />
    </q-dialog>
  </q-layout>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth-store'
import { disputaService } from '../../services/disputaService'
import { comprobanteService } from '../../services/comprobanteService'
import { chatService } from '../../services/chatService'
import { adminUsuarioService } from '../../services/adminUsuarioService'

const router = useRouter()
const authStore = useAuthStore()

const disputas = ref([])
const cargando = ref(false)
const mostrarDetalle = ref(false)
const cargandoDetalle = ref(false)
const disputaSeleccionada = ref(null)
const resolucionDetalle = ref('')
const procesando = ref(false)
const errorAccion = ref('')

const comprobantes = ref([])
const mensajesChat = ref([])
const imagenAmpliada = ref('')

// --- Flujo de bloqueo post-resolución ---
const mostrarDialogoBloqueo = ref(false)

const disputaResuelta = ref(null)
const usuarioObjetivo = ref(null)
const motivoBloqueo = ref('')
const nuevoEstadoCuenta = ref(2)
const procesandoBloqueo = ref(false)
const errorBloqueo = ref('')

const usuarioObjetivo = ref(null) // { idUsuario, nombre, lado }
const motivoBloqueo = ref('')
const nuevoEstadoCuenta = ref(2) // 2=Suspendido, 3=Bloqueado
const procesandoBloqueo = ref(false)
const errorBloqueo = ref('')
const disputaResuelta = ref(null)


const mostrarImagenAmpliada = computed({
  get: () => !!imagenAmpliada.value,
  set: (val) => { if (!val) imagenAmpliada.value = '' }
})

const BASE_BACKEND = 'https://localhost:7126'

function urlComprobante (ruta) {
  return `${BASE_BACKEND}${ruta}`
}

const columnas = [
  { name: 'idDisputa', label: 'ID Disputa', field: 'idDisputa', align: 'left' },
  { name: 'reportante', label: 'Reportante', field: 'usuarioReportante', align: 'left' },
  { name: 'idTransaccion', label: 'ID Transacción', field: 'idTransaccion', align: 'left' },
  { name: 'estado', label: 'Estado', field: 'estado', align: 'left' },
  { name: 'accion', label: 'Acción', field: 'idDisputa', align: 'right' }
]

function colorEstado (estado) {
  const mapa = {
    Abierta: 'orange',
    Resuelta: 'positive',
    Rechazada: 'negative'
  }
  return mapa[estado] || 'grey'
}

function formatearHora (fecha) {
  return new Date(fecha).toLocaleString('es-PE', { dateStyle: 'medium', timeStyle: 'short' })
}

async function cargarDisputas () {
  cargando.value = true
  try {
    disputas.value = await disputaService.obtenerDisputas()
  } catch (error) {
    console.error('Error al cargar disputas:', error)
  } finally {
    cargando.value = false
  }
}

async function verDetalle (disputaFila) {
  mostrarDetalle.value = true
  cargandoDetalle.value = true
  resolucionDetalle.value = ''
  errorAccion.value = ''
  comprobantes.value = []
  mensajesChat.value = []

  try {
    const detalle = await disputaService.obtenerDisputaPorId(disputaFila.idDisputa)
    disputaSeleccionada.value = detalle

    const [comprobantesRes, mensajesRes] = await Promise.allSettled([
      comprobanteService.obtenerPorTransaccion(detalle.idTransaccion),
      chatService.obtenerMensajes(detalle.idTransaccion, authStore.usuarioId)
    ])

    if (comprobantesRes.status === 'fulfilled') comprobantes.value = comprobantesRes.value
    if (mensajesRes.status === 'fulfilled') mensajesChat.value = mensajesRes.value
  } catch (error) {
    console.error('Error al cargar detalle de disputa:', error)
    mostrarDetalle.value = false
  } finally {
    cargandoDetalle.value = false
  }
}

async function accionar (tipo) {
  errorAccion.value = ''
  if (!resolucionDetalle.value.trim()) {
    errorAccion.value = 'Debes escribir un detalle de la resolución.'
    return
  }
  procesando.value = true
  try {
    if (tipo === 'resolver') {
      await disputaService.resolverDisputa(disputaSeleccionada.value.idDisputa, authStore.usuarioId, resolucionDetalle.value)
    } else {
      await disputaService.rechazarDisputa(disputaSeleccionada.value.idDisputa, authStore.usuarioId, resolucionDetalle.value)
    }


    // Guardamos referencia antes de cerrar el modal principal

    const d = disputaSeleccionada.value
    mostrarDetalle.value = false
    await cargarDisputas()


    // Si se resolvió (no rechazó), preguntamos si se quiere sancionar al culpable

    if (tipo === 'resolver') {
      abrirDialogoBloqueo(d)
    }
  } catch {
    errorAccion.value = 'No se pudo procesar la acción.'
  } finally {
    procesando.value = false
  }
}

function abrirDialogoBloqueo (disputa) {
  disputaResuelta.value = disputa

  usuarioObjetivo.value = null

  // el admin elige, no se asume nadie por defecto

  motivoBloqueo.value = ''
  nuevoEstadoCuenta.value = 2
  errorBloqueo.value = ''
  mostrarDialogoBloqueo.value = true
}

function elegirUsuarioObjetivo (lado) {
  const d = disputaResuelta.value
  usuarioObjetivo.value = lado === 'Comprador'
    ? { idUsuario: d.idUsuarioComprador, nombre: d.comprador, lado: 'Comprador' }
    : { idUsuario: d.idUsuarioVendedor, nombre: d.vendedor, lado: 'Vendedor' }

  motivoBloqueo.value = `Transacción #${d.idTransaccion} — disputa resuelta. Motivo reportado: ${d.descripcion}. Detalle de resolución: ${resolucionDetalle.value}`
}

async function confirmarBloqueo () {
  errorBloqueo.value = ''
  if (!motivoBloqueo.value.trim() || motivoBloqueo.value.trim().length < 10) {
    errorBloqueo.value = 'El motivo debe tener al menos 10 caracteres.'
    return
  }
  procesandoBloqueo.value = true
  try {
    await adminUsuarioService.cambiarEstado(
      usuarioObjetivo.value.idUsuario,
      nuevoEstadoCuenta.value,
      motivoBloqueo.value,
      authStore.usuarioId
    )
    mostrarDialogoBloqueo.value = false
  } catch (error) {
    errorBloqueo.value = error.response?.data?.error || 'No se pudo actualizar el estado del usuario.'
  } finally {
    procesandoBloqueo.value = false
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  if (!authStore.esAdmin) {
    router.push('/marketplace')
    return
  }
  cargarDisputas()
})
</script>
