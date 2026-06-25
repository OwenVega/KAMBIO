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
        <div class="row items-center justify-between q-mb-md">
          <div>
            <div class="text-h6 text-weight-bold">Mis Billeteras y Cuentas</div>
            <div class="text-caption text-grey-7">
              Gestiona tus cuentas bancarias para recibir o enviar fondos en tus operaciones P2P.
            </div>
          </div>
          <q-btn label="Añadir método" icon="add" color="dark" unelevated no-caps @click="mostrarFormulario = true" />
        </div>

        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-4" v-for="cuenta in cuentas" :key="cuenta.idMetodoPago">
            <q-card flat bordered class="q-pa-md">
              <div class="row items-start justify-between">
                <div>
                  <div class="text-weight-bold">{{ cuenta.banco }}</div>
                  <div class="text-caption text-grey-7">{{ cuenta.tipoCuenta }}</div>
                </div>
                <q-btn flat round dense icon="more_vert">
                  <q-menu>
                    <q-list>
                      <q-item clickable v-close-popup @click="confirmarEliminar(cuenta)">
                        <q-item-section class="text-negative">Eliminar</q-item-section>
                      </q-item>
                    </q-list>
                  </q-menu>
                </q-btn>
              </div>

              <div class="text-caption text-grey-7 q-mt-md">Número de cuenta</div>
              <div class="text-weight-medium">{{ cuenta.numeroCuentaEnmascarado }}</div>

              <q-chip
                dense
                size="sm"
                class="q-mt-sm"
                :color="cuenta.activo ? 'green-1' : 'grey-3'"
                :text-color="cuenta.activo ? 'green-9' : 'grey-8'"
              >
                {{ cuenta.activo ? 'Verificada' : 'Inactiva' }}
              </q-chip>
            </q-card>
          </div>

          <div class="col-12 col-md-4">
            <q-card
              flat
              bordered
              class="q-pa-md flex flex-center column cursor-pointer"
              style="min-height: 140px; border-style: dashed;"
              @click="mostrarFormulario = true"
            >
              <q-icon name="add_circle_outline" size="32px" color="grey-6" />
              <div class="text-caption text-grey-6 q-mt-sm">Vincular Nueva Cuenta</div>
            </q-card>
          </div>
        </div>

        <div v-if="!cargando && cuentas.length === 0" class="text-center text-grey-7 q-py-xl">
          <q-icon name="account_balance" size="48px" class="q-mb-sm" />
          <div>Todavía no has registrado ninguna cuenta bancaria.</div>
        </div>
      </q-page>
    </q-page-container>

    <q-dialog v-model="mostrarFormulario">
      <q-card style="min-width: 400px" class="q-pa-md">
        <q-card-section>
          <div class="text-h6 text-weight-bold">Registrar Método de Pago</div>
          <div class="text-caption text-grey-7">
            Añade los detalles de tu cuenta bancaria para recibir fondos.
          </div>
        </q-card-section>

        <q-card-section>
          <q-banner class="bg-blue-1 text-blue-9 rounded-borders q-mb-md">
            <template v-slot:avatar>
              <q-icon name="lock" color="blue-9" />
            </template>
            Tus datos bancarios están encriptados y nunca se comparten con terceros.
          </q-banner>

          <q-form @submit.prevent="guardarCuenta" class="q-gutter-md">
            <q-select
              v-model="form.idBanco"
              :options="opcionesBanco"
              label="Banco"
              outlined
              dense
              emit-value
              map-options
            />

            <q-select
              v-model="form.tipoCuenta"
              :options="['Ahorros', 'Corriente']"
              label="Tipo de Cuenta"
              outlined
              dense
            />

            <q-input
              v-model="form.numeroCuenta"
              label="Número de Cuenta"
              placeholder="Ej. 1934872304012"
              outlined
              dense
              :rules="[val => !!val && val.length >= 10 || 'Mínimo 10 dígitos']"
            />

            <q-input
              v-model="form.cci"
              label="CCI (Código de Cuenta Interbancario)"
              placeholder="Ej. 00219300987234040121"
              outlined
              dense
              maxlength="20"
              :rules="[val => !!val && val.length === 20 || 'Debe tener exactamente 20 dígitos']"
            />

            <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders">
              {{ errorMensaje }}
            </q-banner>

            <div class="row q-gutter-sm justify-end">
              <q-btn label="Cancelar" flat v-close-popup @click="resetForm" />
              <q-btn label="Guardar Cuenta" type="submit" color="dark" unelevated :loading="guardando" />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-layout>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '../stores/auth-store'
import { metodoPagoService } from '../services/metodoPagoService'
import { notificacionService } from '../services/notificacionService'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const cuentas = ref([])
const cargando = ref(false)
const guardando = ref(false)
const mostrarFormulario = ref(false)
const errorMensaje = ref('')
const noLeidas = ref(0)

const form = reactive({
  idBanco: 1,
  tipoCuenta: 'Ahorros',
  numeroCuenta: '',
  cci: ''
})

const opcionesBanco = [
  { label: 'BCP', value: 1 },
  { label: 'Interbank', value: 2 },
  { label: 'BBVA', value: 3 },
  { label: 'Scotiabank', value: 4 },
  { label: 'BanBif', value: 5 },
  { label: 'Yape', value: 6 },
  { label: 'Plin', value: 7 }
]

function resetForm () {
  form.idBanco = 1
  form.tipoCuenta = 'Ahorros'
  form.numeroCuenta = ''
  form.cci = ''
  errorMensaje.value = ''
}

async function cargarCuentas () {
  cargando.value = true
  try {
    cuentas.value = await metodoPagoService.obtenerCuentas(authStore.usuarioId)
  } catch (error) {
    console.error('Error al cargar cuentas:', error)
  } finally {
    cargando.value = false
  }
}

async function guardarCuenta () {
  errorMensaje.value = ''
  guardando.value = true
  try {
    await metodoPagoService.agregarCuenta({
      idUsuario: authStore.usuarioId,
      idBanco: form.idBanco,
      tipoCuenta: form.tipoCuenta,
      numeroCuenta: form.numeroCuenta,
      cci: form.cci
    })
    mostrarFormulario.value = false
    resetForm()
    await cargarCuentas()
  } catch (error) {
    errorMensaje.value = error.response?.data?.error || 'No se pudo registrar la cuenta.'
  } finally {
    guardando.value = false
  }
}

function confirmarEliminar (cuenta) {
  $q.dialog({
    title: 'Eliminar cuenta',
    message: `¿Eliminar la cuenta de ${cuenta.banco} terminada en ${cuenta.numeroCuentaEnmascarado.slice(-4)}?`,
    cancel: true,
    persistent: true
  }).onOk(async () => {
    try {
      await metodoPagoService.eliminarCuenta(cuenta.idMetodoPago, authStore.usuarioId)
      await cargarCuentas()
    } catch (error) {
      $q.notify({ type: 'negative', message: error.response?.data?.error || 'No se pudo eliminar la cuenta.' })
    }
  })
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  cargarCuentas()
  notificacionService.contarNoLeidas(authStore.usuarioId).then(n => { noLeidas.value = n })
})
</script>
