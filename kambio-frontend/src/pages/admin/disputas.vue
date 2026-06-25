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
      <q-card style="min-width: 500px" class="q-pa-md" v-if="disputaSeleccionada">
        <q-card-section>
          <div class="text-h6 text-weight-bold">Disputa #{{ disputaSeleccionada.idDisputa }}</div>
          <div class="text-caption text-grey-7">Transacción #{{ disputaSeleccionada.idTransaccion }}</div>
        </q-card-section>

        <q-card-section>
          <div class="text-caption text-grey-7">Reportado por</div>
          <div class="text-weight-medium q-mb-sm">{{ disputaSeleccionada.usuarioReportante }}</div>

          <div class="text-caption text-grey-7">Descripción del problema</div>
          <div class="q-mb-sm">{{ disputaSeleccionada.descripcion }}</div>

          <div class="text-caption text-grey-7">Estado actual</div>
          <q-chip dense size="sm" :color="colorEstado(disputaSeleccionada.estado)" text-color="white" class="q-mb-md">
            {{ disputaSeleccionada.estado }}
          </q-chip>

          <div v-if="disputaSeleccionada.estado === 'Pendiente'">
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
      </q-card>
    </q-dialog>
  </q-layout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth-store'
import { disputaService } from '../../services/disputaService'

const router = useRouter()
const authStore = useAuthStore()


const disputas = ref([])
const cargando = ref(false)
const mostrarDetalle = ref(false)
const disputaSeleccionada = ref(null)
const resolucionDetalle = ref('')
const procesando = ref(false)
const errorAccion = ref('')

const columnas = [
  { name: 'idDisputa', label: 'ID Disputa', field: 'idDisputa', align: 'left' },
  { name: 'reportante', label: 'Reportante', field: 'usuarioReportante', align: 'left' },
  { name: 'idTransaccion', label: 'ID Transacción', field: 'idTransaccion', align: 'left' },
  { name: 'estado', label: 'Estado', field: 'estado', align: 'left' },
  { name: 'accion', label: 'Acción', field: 'idDisputa', align: 'right' }
]

function colorEstado (estado) {
  const mapa = {
    Pendiente: 'orange',
    Resuelta: 'positive',
    Rechazada: 'negative'
  }
  return mapa[estado] || 'grey'
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

function verDetalle (disputa) {
  disputaSeleccionada.value = disputa
  resolucionDetalle.value = ''
  errorAccion.value = ''
  mostrarDetalle.value = true
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
    mostrarDetalle.value = false
    await cargarDisputas()
  } catch {
    errorAccion.value = 'No se pudo procesar la acción.'
  } finally {
    procesando.value = false
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
