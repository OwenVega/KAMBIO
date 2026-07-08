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
            <q-item clickable to="/admin/disputas">
              <q-item-section avatar>
                <q-icon name="gavel" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Disputas</q-item-section>
            </q-item>
            <q-item clickable to="/admin/reportes" class="bg-primary rounded-borders">
              <q-item-section avatar>
                <q-icon name="bar_chart" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Reportes</q-item-section>
            </q-item>
          </q-list>
        </div>

        <div class="col q-pa-lg">
          <div class="row items-center justify-between q-mb-md">
            <div class="text-h6 text-weight-bold">Panel Administrativo - Reportes</div>
            <div class="row q-gutter-sm">
              <q-btn label="Exportar a PDF" icon="picture_as_pdf" outline color="dark" no-caps :loading="exportandoPdf" @click="exportarPdf" />
              <q-btn label="Exportar a Excel" icon="grid_on" color="dark" unelevated no-caps :loading="exportandoExcel" @click="exportarExcel" />
            </div>
          </div>

          <div class="row q-col-gutter-md q-mb-md">
            <div class="col-12 col-md-3">
              <q-card flat bordered class="q-pa-md">
                <div class="text-caption text-grey-7">Volumen Total</div>
                <div class="text-h6 text-weight-bold">${{ formatearMoneda(volumenTotal) }}</div>
              </q-card>
            </div>
            <div class="col-12 col-md-3">
              <q-card flat bordered class="q-pa-md">
                <div class="text-caption text-grey-7">Transacciones</div>
                <div class="text-h6 text-weight-bold">{{ transacciones.length }}</div>
              </q-card>
            </div>
            <div class="col-12 col-md-3">
              <q-card flat bordered class="q-pa-md">
                <div class="text-caption text-grey-7">Completadas</div>
                <div class="text-h6 text-weight-bold">{{ completadas }}</div>
              </q-card>
            </div>
            <div class="col-12 col-md-3">
              <q-card flat bordered class="q-pa-md">
                <div class="text-caption text-grey-7">Usuarios Únicos</div>
                <div class="text-h6 text-weight-bold">{{ usuariosUnicos }}</div>
              </q-card>
            </div>
          </div>

          <q-card flat bordered class="q-pa-md q-mb-md">
            <div class="text-subtitle2 text-weight-bold q-mb-sm">Filtros de Auditoría</div>
            <div class="row q-col-gutter-md items-end">
              <div class="col-12 col-md-3">
                <q-input v-model="filtro.fechaInicio" label="Fecha Inicio" type="date" outlined dense />
              </div>
              <div class="col-12 col-md-3">
                <q-input v-model="filtro.fechaFin" label="Fecha Fin" type="date" outlined dense />
              </div>
              <div class="col-12 col-md-3">
                <q-select
                  v-model="filtro.idDivisa"
                  :options="opcionesDivisa"
                  label="Divisa"
                  outlined
                  dense
                  emit-value
                  map-options
                  clearable
                />
              </div>
              <div class="col-12 col-md-3">
                <q-btn label="Aplicar Filtros" icon="filter_alt" color="dark" unelevated class="full-width" @click="cargarReporte" />
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
              <template v-slot:body-cell-estado="props">
                <q-td :props="props">
                  <q-chip dense size="sm" :color="colorEstado(props.row.estado)" text-color="white">
                    {{ props.row.estado }}
                  </q-chip>
                </q-td>
              </template>

              <template v-slot:no-data>
                <div class="full-width text-center q-pa-lg text-grey-7">
                  <q-icon name="bar_chart" size="48px" class="q-mb-sm" />
                  No hay transacciones con esos filtros.
                </div>
              </template>
            </q-table>
          </q-card>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth-store'
import { reporteService } from '../../services/reporteService'

const router = useRouter()
const authStore = useAuthStore()

const transacciones = ref([])
const cargando = ref(false)
const exportandoPdf = ref(false)
const exportandoExcel = ref(false)

const filtro = reactive({
  fechaInicio: '',
  fechaFin: '',
  idDivisa: null,
  idUsuario: null
})

const opcionesDivisa = [
  { label: 'USD', value: 1 },
  { label: 'PEN', value: 2 },
  { label: 'EUR', value: 3 },
  { label: 'GBP', value: 4 },
  { label: 'JPY', value: 5 },
  { label: 'CHF', value: 6 }
]

const columnas = [
  { name: 'idTransaccion', label: 'ID Transacción', field: 'idTransaccion', align: 'left' },
  { name: 'fecha', label: 'Fecha', field: row => new Date(row.fechaInicio).toLocaleDateString('es-PE'), align: 'left' },
  { name: 'comprador', label: 'Comprador', field: 'comprador', align: 'left' },
  { name: 'vendedor', label: 'Vendedor', field: 'vendedor', align: 'left' },
  { name: 'monto', label: 'Monto', field: row => `${row.monto} ${row.divisaOrigen}`, align: 'left' },
  { name: 'tasa', label: 'Tasa', field: 'tasaCambioAplicada', align: 'left' },
  { name: 'estado', label: 'Estado', field: 'estado', align: 'left' }
]

const volumenTotal = computed(() =>
  transacciones.value.reduce((sum, t) => sum + Number(t.monto), 0)
)

const completadas = computed(() =>
  transacciones.value.filter(t => t.estado === 'Completada').length
)

const usuariosUnicos = computed(() => {
  const set = new Set()
  transacciones.value.forEach(t => {
    set.add(t.comprador)
    set.add(t.vendedor)
  })
  return set.size
})

function formatearMoneda (valor) {
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

function descargarArchivo (blob, nombreArchivo) {
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.setAttribute('download', nombreArchivo)
  document.body.appendChild(link)
  link.click()
  link.remove()
}

async function cargarReporte () {
  cargando.value = true
  try {
    transacciones.value = await reporteService.obtenerReporte(filtro)
  } catch (error) {
    console.error('Error al cargar reporte:', error)
  } finally {
    cargando.value = false
  }
}

async function exportarExcel () {
  exportandoExcel.value = true
  try {
    const blob = await reporteService.exportarExcel(filtro)
    descargarArchivo(blob, `reporte_kambio_${Date.now()}.xlsx`)
  } catch (error) {
    console.error('Error al exportar Excel:', error)
  } finally {
    exportandoExcel.value = false
  }
}

async function exportarPdf () {
  exportandoPdf.value = true
  try {
    const blob = await reporteService.exportarPdf(filtro)
    descargarArchivo(blob, `reporte_kambio_${Date.now()}.pdf`)
  } catch (error) {
    console.error('Error al exportar PDF:', error)
  } finally {
    exportandoPdf.value = false
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
  cargarReporte()
})
</script>
