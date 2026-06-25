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
      <q-page class="q-pa-lg">
        <div class="text-h6 text-weight-bold q-mb-md">Historial de Transacciones</div>

        <div class="row q-col-gutter-md q-mb-md">
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-md">
              <div class="text-caption text-grey-7">Volumen Mensual (USD)</div>
              <div class="text-h5 text-weight-bold">${{ formatearMoneda(resumen.volumenMensualUSD) }}</div>
            </q-card>
          </div>
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-md">
              <div class="text-caption text-grey-7">Operaciones Exitosas</div>
              <div class="text-h5 text-weight-bold">{{ resumen.operacionesExitosas }}</div>
            </q-card>
          </div>
          <div class="col-12 col-md-4">
            <q-card flat bordered class="q-pa-md">
              <div class="text-caption text-grey-7">Tiempo Promedio</div>
              <div class="text-h5 text-weight-bold">{{ resumen.tiempoPromedioMinutos }} min</div>
            </q-card>
          </div>
        </div>

        <q-card flat bordered class="q-pa-md q-mb-md">
          <div class="row q-col-gutter-md items-end">
            <div class="col-12 col-md-3">
              <q-input
                v-model="filtro.busquedaDivisas"
                label="Buscar por divisa (ej: USD)"
                outlined
                dense
                clearable
                @keyup.enter="buscar"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtro.tipoOperacion"
                :options="opcionesTipo"
                label="Tipo"
                outlined
                dense
                emit-value
                map-options
                clearable
              />
            </div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtro.idEstado"
                :options="opcionesEstado"
                label="Estado"
                outlined
                dense
                emit-value
                map-options
                clearable
              />
            </div>
            <div class="col-12 col-md-2">
              <q-btn label="Buscar" icon="search" color="dark" unelevated class="full-width" @click="buscar" />
            </div>
          </div>
        </q-card>

        <q-card flat bordered>
          <q-table
            :rows="transacciones"
            :columns="columnas"
            row-key="idTransaccion"
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
                  :color="props.row.tipo === 'Compra' ? 'blue-1' : 'orange-1'"
                  :text-color="props.row.tipo === 'Compra' ? 'blue-9' : 'orange-9'"
                >
                  {{ props.row.tipo }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-estado="props">
              <q-td :props="props">
                <q-chip dense size="sm" :color="colorEstado(props.row.estado)" text-color="white">
                  {{ props.row.estado }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-montos="props">
              <q-td :props="props">
                <div>{{ formatearMoneda(props.row.montoOrigen) }} → {{ formatearMoneda(props.row.montoDestino) }}</div>
                <div class="text-caption text-grey-7">{{ props.row.parDivisas }}</div>
              </q-td>
            </template>

            <template v-slot:body-cell-accion="props">
              <q-td :props="props">
                <q-btn
                  label="Ver detalle"
                  flat
                  dense
                  no-caps
                  size="sm"
                  color="primary"
                  :to="`/transaccion/${props.row.idTransaccion}`"
                />
              </q-td>
            </template>

            <template v-slot:no-data>
              <div class="full-width text-center q-pa-lg text-grey-7">
                <q-icon name="receipt_long" size="48px" class="q-mb-sm" />
                <div>Todavía no tienes transacciones.</div>
              </div>
            </template>
          </q-table>

          <div class="row justify-center q-py-md" v-if="totalPaginas > 1">
            <q-pagination
              v-model="filtro.pagina"
              :max="totalPaginas"
              direction-links
              @update:model-value="buscar"
            />
          </div>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth-store'
import { transaccionService } from '../services/transaccionService'

const router = useRouter()
const authStore = useAuthStore()

const cargando = ref(false)
const transacciones = ref([])
const totalPaginas = ref(1)
const resumen = reactive({
  volumenMensualUSD: 0,
  operacionesExitosas: 0,
  tiempoPromedioMinutos: 0
})

const filtro = reactive({
  busquedaDivisas: '',
  tipoOperacion: null,
  idEstado: null,
  pagina: 1,
  cantidadPorPagina: 10
})

const opcionesTipo = [
  { label: 'Compra', value: 'Compra' },
  { label: 'Venta', value: 'Venta' }
]

const opcionesEstado = [
  { label: 'Pendiente', value: 1 },
  { label: 'En Proceso', value: 2 },
  { label: 'Pago Realizado', value: 3 },
  { label: 'Completada', value: 4 },
  { label: 'Cancelada', value: 5 },
  { label: 'En Disputa', value: 6 }
]

const columnas = [
  { name: 'fecha', label: 'Fecha', field: 'fechaOperacion', align: 'left' },
  { name: 'tipo', label: 'Tipo', field: 'tipo', align: 'left' },
  { name: 'montos', label: 'Monto', field: 'montoOrigen', align: 'left' },
  { name: 'estado', label: 'Estado', field: 'estado', align: 'left' },
  { name: 'accion', label: '', field: 'idTransaccion', align: 'right' }
]

function formatearMoneda (valor) {
  if (valor == null) return '0.00'
  return Number(valor).toLocaleString('es-PE', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

function colorEstado (estado) {
  const mapa = {
    Pendiente: 'orange',
    'En Proceso': 'blue',
    'Pago Realizado': 'teal',
    Completada: 'positive',
    Cancelada: 'negative',
    'En Disputa': 'deep-orange'
  }
  return mapa[estado] || 'grey'
}

async function buscar () {
  cargando.value = true
  try {
    const resultado = await transaccionService.obtenerHistorial(authStore.usuarioId, filtro)
    transacciones.value = resultado.transacciones
    totalPaginas.value = resultado.totalPaginas
    resumen.volumenMensualUSD = resultado.resumen.volumenMensualUSD
    resumen.operacionesExitosas = resultado.resumen.operacionesExitosas
    resumen.tiempoPromedioMinutos = resultado.resumen.tiempoPromedioMinutos
  } catch (error) {
    console.error('Error al cargar historial:', error)
  } finally {
    cargando.value = false
  }
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  buscar()
})
</script>
