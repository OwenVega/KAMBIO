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
            <q-item clickable to="/admin/usuarios" class="bg-primary rounded-borders">
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
            <q-item clickable to="/admin/reportes">
              <q-item-section avatar>
                <q-icon name="bar_chart" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Reportes</q-item-section>
            </q-item>
          </q-list>
        </div>

        <div class="col q-pa-lg">
          <div class="row items-center justify-between q-mb-md">
            <div>
              <div class="text-h6 text-weight-bold">Gestión de Usuarios</div>
              <div class="text-caption text-grey-7">
                Administra y monitorea la actividad de los usuarios en la plataforma.
              </div>
            </div>
            <q-input
              v-model="busqueda"
              placeholder="Buscar por nombre, email o ID..."
              outlined
              dense
              style="width: 300px"
            >
              <template v-slot:prepend>
                <q-icon name="search" />
              </template>
            </q-input>
          </div>

          <q-card flat bordered>
            <q-table
              :rows="usuariosFiltrados"
              :columns="columnas"
              row-key="idUsuario"
              flat
              :loading="cargando"
              hide-pagination
              :rows-per-page-options="[0]"
            >
              <template v-slot:body-cell-usuario="props">
                <q-td :props="props">
                  <div class="row items-center q-gutter-sm">
                    <q-avatar size="32px" color="primary" text-color="white">
                      {{ obtenerIniciales(props.row.nombres, props.row.apellidos) }}
                    </q-avatar>
                    <div>
                      <div class="text-weight-medium">{{ props.row.nombres }} {{ props.row.apellidos }}</div>
                      <div class="text-caption text-grey-7">ID: #USR-{{ props.row.idUsuario }}</div>
                    </div>
                  </div>
                </q-td>
              </template>

              <template v-slot:body-cell-correo="props">
                <q-td :props="props">{{ props.row.correo }}</q-td>
              </template>

              <template v-slot:body-cell-estado="props">
                <q-td :props="props">
                  <q-chip dense size="sm" :color="colorEstado(props.row.estadoCuenta)" text-color="white">
                    {{ props.row.estadoCuenta }}
                  </q-chip>
                </q-td>
              </template>

              <template v-slot:body-cell-reputacion="props">
                <q-td :props="props">
                  <q-icon name="star" color="amber" size="16px" />
                  {{ props.row.calificacionPromedio || '0.0' }} ({{ props.row.totalOrdenes }} órdenes)
                </q-td>
              </template>

              <template v-slot:body-cell-acciones="props">
                <q-td :props="props">
                  <q-btn flat round dense icon="more_vert">
                    <q-menu>
                      <q-list>
                        <q-item
                          v-if="props.row.estadoCuenta !== 'Activo'"
                          clickable
                          v-close-popup
                          @click="confirmarCambioEstado(props.row, 1)"
                        >
                          <q-item-section class="text-positive">Activar</q-item-section>
                        </q-item>
                        <q-item
                          v-if="props.row.estadoCuenta !== 'Suspendido'"
                          clickable
                          v-close-popup
                          @click="confirmarCambioEstado(props.row, 2)"
                        >
                          <q-item-section class="text-orange">Suspender</q-item-section>
                        </q-item>
                        <q-item
                          v-if="props.row.estadoCuenta !== 'Bloqueado'"
                          clickable
                          v-close-popup
                          @click="confirmarCambioEstado(props.row, 3)"
                        >
                          <q-item-section class="text-negative">Bloquear</q-item-section>
                        </q-item>
                      </q-list>
                    </q-menu>
                  </q-btn>
                </q-td>
              </template>

              <template v-slot:no-data>
                <div class="full-width text-center q-pa-lg text-grey-7">
                  <q-icon name="people_outline" size="48px" class="q-mb-sm" />
                  No hay usuarios registrados.
                </div>
              </template>
            </q-table>
          </q-card>
        </div>
      </q-page>
    </q-page-container>

    <q-dialog v-model="mostrarMotivo">
      <q-card style="min-width: 400px" class="q-pa-md">
        <q-card-section>
          <div class="text-h6 text-weight-bold">{{ tituloAccion }}</div>
          <div class="text-caption text-grey-7">
            {{ usuarioSeleccionado?.nombres }} {{ usuarioSeleccionado?.apellidos }}
          </div>
        </q-card-section>

        <q-card-section>
          <q-input
            v-model="motivo"
            type="textarea"
            rows="3"
            outlined
            label="Motivo"
            placeholder="Explica la razón de esta acción"
          />

          <q-banner v-if="errorAccion" class="bg-red-1 text-red-9 rounded-borders q-mt-sm">
            {{ errorAccion }}
          </q-banner>

          <div class="row q-gutter-sm justify-end q-mt-md">
            <q-btn label="Cancelar" flat v-close-popup @click="motivo = ''" />
            <q-btn label="Confirmar" color="dark" unelevated :loading="procesando" @click="ejecutarCambioEstado" />
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-layout>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth-store'
import { adminUsuarioService } from '../../services/adminUsuarioService'

const router = useRouter()
const authStore = useAuthStore()

const usuarios = ref([])
const cargando = ref(false)
const busqueda = ref('')
const mostrarMotivo = ref(false)
const usuarioSeleccionado = ref(null)
const nuevoEstadoPendiente = ref(null)
const motivo = ref('')
const procesando = ref(false)
const errorAccion = ref('')

const columnas = [
  { name: 'usuario', label: 'Usuario', field: 'nombres', align: 'left' },
  { name: 'correo', label: 'Email / Contacto', field: 'correo', align: 'left' },
  { name: 'estado', label: 'Estado', field: 'estadoCuenta', align: 'left' },
  { name: 'reputacion', label: 'Reputación', field: 'calificacionPromedio', align: 'left' },
  { name: 'acciones', label: 'Acciones', field: 'idUsuario', align: 'right' }
]

const usuariosFiltrados = computed(() => {
  if (!busqueda.value.trim()) return usuarios.value
  const termino = busqueda.value.toLowerCase()
  return usuarios.value.filter(u =>
    `${u.nombres} ${u.apellidos}`.toLowerCase().includes(termino) ||
    u.correo.toLowerCase().includes(termino) ||
    String(u.idUsuario).includes(termino)
  )
})

const tituloAccion = computed(() => {
  const mapa = { 1: 'Activar Usuario', 2: 'Suspender Usuario', 3: 'Bloquear Usuario' }
  return mapa[nuevoEstadoPendiente.value] || 'Cambiar Estado'
})

function obtenerIniciales (nombres, apellidos) {
  return `${(nombres || '?').charAt(0)}${(apellidos || '').charAt(0)}`.toUpperCase()
}

function colorEstado (estado) {
  const mapa = { Activo: 'positive', Suspendido: 'orange', Bloqueado: 'negative' }
  return mapa[estado] || 'grey'
}

async function cargarUsuarios () {
  cargando.value = true
  try {
    usuarios.value = await adminUsuarioService.obtenerUsuarios()
  } catch (error) {
    console.error('Error al cargar usuarios:', error)
  } finally {
    cargando.value = false
  }
}

function confirmarCambioEstado (usuario, nuevoEstado) {
  usuarioSeleccionado.value = usuario
  nuevoEstadoPendiente.value = nuevoEstado
  motivo.value = ''
  errorAccion.value = ''
  mostrarMotivo.value = true
}

async function ejecutarCambioEstado () {
  errorAccion.value = ''
  if (!motivo.value.trim()) {
    errorAccion.value = 'Debes escribir un motivo.'
    return
  }
  procesando.value = true
  try {
    await adminUsuarioService.cambiarEstado(
      usuarioSeleccionado.value.idUsuario,
      nuevoEstadoPendiente.value,
      motivo.value,
      authStore.usuarioId
    )
    mostrarMotivo.value = false
    await cargarUsuarios()
  } catch (error) {
    errorAccion.value = error.response?.data?.error || 'No se pudo cambiar el estado.'
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
  cargarUsuarios()
})
</script>
