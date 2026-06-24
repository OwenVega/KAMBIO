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
        </q-tabs>

        <q-space />

        <q-btn flat round icon="add" to="/publicar-oferta">
          <q-tooltip>Publicar nueva oferta</q-tooltip>
        </q-btn>
        <q-btn flat round icon="account_circle" @click="cerrarSesion" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <q-page class="q-pa-lg">
        <div class="text-h6 text-weight-bold q-mb-md">Mis Ofertas</div>

        <q-card flat bordered>
          <q-table
            :rows="misOfertas"
            :columns="columnas"
            row-key="idOferta"
            flat
            :loading="cargando"
            hide-pagination
            :rows-per-page-options="[0]"
          >
            <template v-slot:body-cell-tipo="props">
              <q-td :props="props">
                <q-chip
                  dense
                  size="sm"
                  :color="props.row.tipoOferta === 'Compra' ? 'blue-1' : 'orange-1'"
                  :text-color="props.row.tipoOferta === 'Compra' ? 'blue-9' : 'orange-9'"
                >
                  {{ props.row.tipoOferta }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-divisas="props">
              <q-td :props="props">
                {{ props.row.divisaOrigen }} / {{ props.row.divisaDestino }}
              </q-td>
            </template>

            <template v-slot:body-cell-monto="props">
              <q-td :props="props">
                {{ formatearMoneda(props.row.montoDisponible) }} {{ props.row.divisaOrigen }}
              </q-td>
            </template>

            <template v-slot:body-cell-tasa="props">
              <q-td :props="props">
                <span class="text-positive text-weight-medium">{{ props.row.tasaCambio }}</span>
                <div class="text-caption text-grey-7">{{ props.row.divisaDestino }}/{{ props.row.divisaOrigen }}</div>
              </q-td>
            </template>

            <template v-slot:body-cell-estado="props">
              <q-td :props="props">
                <q-chip
                  dense
                  size="sm"
                  :color="props.row.estado === 'Activa' ? 'green-1' : 'grey-3'"
                  :text-color="props.row.estado === 'Activa' ? 'green-9' : 'grey-8'"
                >
                  {{ props.row.estado }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-acciones="props">
              <q-td :props="props">
                <q-btn
                  v-if="props.row.estado === 'Activa'"
                  label="Cancelar"
                  flat
                  dense
                  no-caps
                  size="sm"
                  color="negative"
                  :loading="cancelandoId === props.row.idOferta"
                  @click="confirmarCancelacion(props.row)"
                />
              </q-td>
            </template>

            <template v-slot:no-data>
              <div class="full-width text-center q-pa-lg text-grey-7">
                <q-icon name="inbox" size="48px" class="q-mb-sm" />
                <div>Todavía no has publicado ninguna oferta.</div>
                <q-btn
                  label="Publicar mi primera oferta"
                  color="dark"
                  unelevated
                  no-caps
                  class="q-mt-sm"
                  to="/publicar-oferta"
                />
              </div>
            </template>
          </q-table>
        </q-card>

        <div class="row q-col-gutter-md q-mt-md">
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-md text-center">
              <div class="text-caption text-grey-7">Ofertas Activas</div>
              <div class="text-h5 text-weight-bold">{{ totalActivas }}</div>
            </q-card>
          </div>
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-md text-center">
              <div class="text-caption text-grey-7">Total Publicadas</div>
              <div class="text-h5 text-weight-bold">{{ misOfertas.length }}</div>
            </q-card>
          </div>
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-md text-center">
              <div class="text-caption text-grey-7">Volumen Activo (USD)</div>
              <div class="text-h5 text-weight-bold">${{ formatearMoneda(volumenActivoUsd) }}</div>
            </q-card>
          </div>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth-store'
import { ofertaService } from '../services/ofertaService'
import { useQuasar } from 'quasar'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const todasLasOfertas = ref([])
const cargando = ref(false)
const cancelandoId = ref(null)

const columnas = [
  { name: 'tipo', label: 'Tipo', field: 'tipoOferta', align: 'left' },
  { name: 'divisas', label: 'Divisas', field: 'divisaOrigen', align: 'left' },
  { name: 'monto', label: 'Monto Original', field: 'montoDisponible', align: 'left' },
  { name: 'tasa', label: 'Tasa de Cambio', field: 'tasaCambio', align: 'left' },
  { name: 'estado', label: 'Estado', field: 'estado', align: 'left' },
  { name: 'acciones', label: 'Acciones', field: 'idOferta', align: 'right' }
]

const misOfertas = computed(() =>
  todasLasOfertas.value.filter(o => o.idUsuario === authStore.usuarioId)
)

const totalActivas = computed(() =>
  misOfertas.value.filter(o => o.estado === 'Activa').length
)

const volumenActivoUsd = computed(() =>
  misOfertas.value
    .filter(o => o.estado === 'Activa' && o.divisaOrigen === 'USD')
    .reduce((sum, o) => sum + Number(o.montoDisponible), 0)
)

function formatearMoneda (valor) {
  if (valor == null) return '0.00'
  return Number(valor).toLocaleString('es-PE', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

async function cargarOfertas () {
  cargando.value = true
  try {
    todasLasOfertas.value = await ofertaService.obtenerOfertasActivas()
  } catch (error) {
    console.error('Error al cargar ofertas:', error)
  } finally {
    cargando.value = false
  }
}

function confirmarCancelacion (oferta) {
  $q.dialog({
    title: 'Cancelar oferta',
    message: `¿Seguro que quieres cancelar esta oferta de ${oferta.divisaOrigen}/${oferta.divisaDestino}?`,
    cancel: true,
    persistent: true
  }).onOk(() => {
    cancelarOferta(oferta.idOferta)
  })
}

async function cancelarOferta (idOferta) {
  cancelandoId.value = idOferta
  try {
    await ofertaService.cancelarOferta(idOferta, authStore.usuarioId)
    await cargarOfertas()
  } catch (error) {
    const mensaje = error.response?.data?.mensaje || 'No se pudo cancelar la oferta.'
    $q.notify({ type: 'negative', message: mensaje })
  } finally {
    cancelandoId.value = null
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarOfertas()
})
</script>
