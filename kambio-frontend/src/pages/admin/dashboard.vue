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
            <q-item clickable to="/admin/dashboard" class="bg-primary rounded-borders">
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
            <q-item clickable to="/admin/reportes">
              <q-item-section avatar>
                <q-icon name="bar_chart" color="white" />
              </q-item-section>
              <q-item-section class="text-white">Reportes</q-item-section>
            </q-item>
          </q-list>
        </div>

        <div class="col q-pa-lg">
          <q-inner-loading :showing="cargando">
            <q-spinner color="primary" size="3em" />
          </q-inner-loading>

          <template v-if="!cargando">
            <!-- Alerta de disputas pendientes -->
            <q-banner
              v-if="disputasPendientes > 0"
              class="bg-orange-1 text-orange-9 rounded-borders q-mb-md"
              dense
            >
              <template v-slot:avatar>
                <q-icon name="warning" color="orange-9" />
              </template>
              Tienes <strong>{{ disputasPendientes }}</strong> disputa{{ disputasPendientes > 1 ? 's' : '' }} pendiente{{ disputasPendientes > 1 ? 's' : '' }} de resolver.
              <template v-slot:action>
                <q-btn flat dense no-caps label="Revisar ahora" color="orange-9" to="/admin/disputas" />
              </template>
            </q-banner>

            <!-- KPIs principales -->
            <div class="row q-col-gutter-md q-mb-md">
              <div class="col-12 col-sm-6 col-md-3">
                <q-card flat bordered class="kpi-card bg-gradient-blue text-white">
                  <q-card-section>
                    <div class="row items-center justify-between">
                      <q-icon name="groups" size="32px" />
                      <q-chip dense size="sm" color="white" text-color="primary">{{ usuariosActivos }} activos</q-chip>
                    </div>
                    <div class="text-h4 text-weight-bold q-mt-sm">{{ totalUsuarios }}</div>
                    <div class="text-caption">Usuarios totales</div>
                  </q-card-section>
                </q-card>
              </div>

              <div class="col-12 col-sm-6 col-md-3">
                <q-card flat bordered class="kpi-card bg-gradient-orange text-white">
                  <q-card-section>
                    <div class="row items-center justify-between">
                      <q-icon name="gavel" size="32px" />
                      <q-chip dense size="sm" color="white" text-color="orange-9">{{ disputasResueltas }} resueltas</q-chip>
                    </div>
                    <div class="text-h4 text-weight-bold q-mt-sm">{{ disputasPendientes }}</div>
                    <div class="text-caption">Disputas pendientes</div>
                  </q-card-section>
                </q-card>
              </div>

              <div class="col-12 col-sm-6 col-md-3">
                <q-card flat bordered class="kpi-card bg-gradient-green text-white">
                  <q-card-section>
                    <div class="row items-center justify-between">
                      <q-icon name="payments" size="32px" />
                      <q-chip dense size="sm" color="white" text-color="positive">{{ totalTransacciones }} operaciones</q-chip>
                    </div>
                    <div class="text-h5 text-weight-bold q-mt-sm">{{ formatoMonto(volumenTotal) }}</div>
                    <div class="text-caption">Volumen total</div>
                  </q-card-section>
                </q-card>
              </div>

              <div class="col-12 col-sm-6 col-md-3">
                <q-card flat bordered class="kpi-card bg-gradient-purple text-white">
                  <q-card-section>
                    <div class="row items-center justify-between">
                      <q-icon name="check_circle" size="32px" />
                      <q-chip dense size="sm" color="white" text-color="purple">
                        {{ totalTransacciones ? Math.round((transaccionesCompletadas / totalTransacciones) * 100) : 0 }}%
                      </q-chip>
                    </div>
                    <div class="text-h4 text-weight-bold q-mt-sm">{{ transaccionesCompletadas }}</div>
                    <div class="text-caption">Transacciones completadas</div>
                  </q-card-section>
                </q-card>
              </div>
            </div>

            <div class="row q-col-gutter-md q-mb-md">
              <!-- Gráfico: Transacciones por estado -->
              <div class="col-12 col-md-6">
                <q-card flat bordered class="q-pa-md full-height">
                  <div class="text-subtitle1 text-weight-bold q-mb-md">
                    <q-icon name="donut_large" class="q-mr-xs" />
                    Transacciones por Estado
                  </div>
                  <div v-for="item in barraTransacciones" :key="item.label" class="q-mb-md">
                    <div class="row justify-between text-caption q-mb-xs">
                      <span class="text-weight-medium">{{ item.label }}</span>
                      <span>{{ item.valor }} ({{ item.pct ? Math.round(item.pct * 100) : 0 }}%)</span>
                    </div>
                    <q-linear-progress :value="item.pct" :color="item.color" size="10px" rounded />
                  </div>
                </q-card>
              </div>

              <!-- Gráfico: Usuarios por estado -->
              <div class="col-12 col-md-6">
                <q-card flat bordered class="q-pa-md full-height">
                  <div class="text-subtitle1 text-weight-bold q-mb-md">
                    <q-icon name="pie_chart" class="q-mr-xs" />
                    Usuarios por Estado
                  </div>
                  <div v-for="item in barraUsuarios" :key="item.label" class="q-mb-md">
                    <div class="row justify-between text-caption q-mb-xs">
                      <span class="text-weight-medium">{{ item.label }}</span>
                      <span>{{ item.valor }} ({{ item.pct ? Math.round(item.pct * 100) : 0 }}%)</span>
                    </div>
                    <q-linear-progress :value="item.pct" :color="item.color" size="10px" rounded />
                  </div>
                </q-card>
              </div>
            </div>

            <!-- Accesos directos -->
            <div class="text-subtitle1 text-weight-bold q-mb-sm">Accesos Rápidos</div>
            <div class="row q-col-gutter-md q-mb-md">
              <div class="col-12 col-sm-4">
                <q-card flat bordered class="q-pa-md cursor-pointer access-card" @click="router.push('/admin/usuarios')">
                  <div class="row items-center q-gutter-md">
                    <q-avatar color="blue-1" text-color="primary" icon="people" size="48px" />
                    <div>
                      <div class="text-weight-bold">Gestionar Usuarios</div>
                      <div class="text-caption text-grey-7">Activar, suspender o bloquear cuentas</div>
                    </div>
                  </div>
                </q-card>
              </div>

              <div class="col-12 col-sm-4">
                <q-card flat bordered class="q-pa-md cursor-pointer access-card" @click="router.push('/admin/disputas')">
                  <div class="row items-center q-gutter-md">
                    <q-avatar color="orange-1" text-color="orange-9" icon="gavel" size="48px" />
                    <div>
                      <div class="text-weight-bold">Resolver Disputas</div>
                      <div class="text-caption text-grey-7">Revisar reclamos entre usuarios</div>
                    </div>
                  </div>
                </q-card>
              </div>

              <div class="col-12 col-sm-4">
                <q-card flat bordered class="q-pa-md cursor-pointer access-card" @click="router.push('/admin/reportes')">
                  <div class="row items-center q-gutter-md">
                    <q-avatar color="green-1" text-color="positive" icon="bar_chart" size="48px" />
                    <div>
                      <div class="text-weight-bold">Ver Reportes</div>
                      <div class="text-caption text-grey-7">Auditoría y exportación de datos</div>
                    </div>
                  </div>
                </q-card>
              </div>
            </div>

            <!-- Últimas disputas -->
            <div class="text-subtitle1 text-weight-bold q-mb-sm">Últimas Disputas</div>
            <q-card flat bordered>
              <q-list separator>
                <q-item v-if="ultimasDisputas.length === 0">
                  <q-item-section class="text-grey-6 text-center q-py-md">
                    No hay disputas registradas.
                  </q-item-section>
                </q-item>
                <q-item v-for="d in ultimasDisputas" :key="d.idDisputa" clickable @click="router.push('/admin/disputas')">
                  <q-item-section avatar>
                    <q-icon name="gavel" :color="colorEstadoDisputa(d.estado)" />
                  </q-item-section>
                  <q-item-section>
                    <q-item-label class="text-weight-medium">{{ d.usuarioReportante }} — Transacción #{{ d.idTransaccion }}</q-item-label>
                    <q-item-label caption>{{ d.descripcion }}</q-item-label>
                  </q-item-section>
                  <q-item-section side>
                    <q-chip dense size="sm" :color="colorEstadoDisputa(d.estado)" text-color="white">{{ d.estado }}</q-chip>
                  </q-item-section>
                </q-item>
              </q-list>
            </q-card>
          </template>
        </div>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../../stores/auth-store'
import { adminUsuarioService } from '../../services/adminUsuarioService'
import { disputaService } from '../../services/disputaService'
import { reporteService } from '../../services/reporteService'

const router = useRouter()
const authStore = useAuthStore()

const cargando = ref(true)

const totalUsuarios = ref(0)
const usuariosActivos = ref(0)
const usuariosSuspendidos = ref(0)
const usuariosBloqueados = ref(0)

const disputasPendientes = ref(0)
const disputasResueltas = ref(0)
const disputasRechazadas = ref(0)
const ultimasDisputas = ref([])

const volumenTotal = ref(0)
const totalTransacciones = ref(0)
const transaccionesCompletadas = ref(0)

const barraUsuarios = ref([])
const barraTransacciones = ref([])

function formatoMonto (valor) {
  return new Intl.NumberFormat('es-PE', { style: 'currency', currency: 'PEN' }).format(valor || 0)
}

function colorEstadoDisputa (estado) {
  const mapa = { Abierta: 'orange', Resuelta: 'positive', Rechazada: 'negative' }
  return mapa[estado] || 'grey'
}

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(async () => {
  if (!authStore.esAdmin) {
    router.push('/marketplace')
    return
  }

  try {
    const [usuariosRes, disputasRes, reporteRes] = await Promise.all([
      adminUsuarioService.obtenerUsuarios(),
      disputaService.obtenerDisputas(),
      reporteService.obtenerReporte({})
    ])

    // --- Usuarios ---
    totalUsuarios.value = usuariosRes.length
    usuariosActivos.value = usuariosRes.filter(u => u.estadoCuenta === 'Activo').length
    usuariosSuspendidos.value = usuariosRes.filter(u => u.estadoCuenta === 'Suspendido').length
    usuariosBloqueados.value = usuariosRes.filter(u => u.estadoCuenta === 'Bloqueado').length

    barraUsuarios.value = [
      { label: 'Activos', valor: usuariosActivos.value, pct: usuariosActivos.value / (totalUsuarios.value || 1), color: 'positive' },
      { label: 'Suspendidos', valor: usuariosSuspendidos.value, pct: usuariosSuspendidos.value / (totalUsuarios.value || 1), color: 'warning' },
      { label: 'Bloqueados', valor: usuariosBloqueados.value, pct: usuariosBloqueados.value / (totalUsuarios.value || 1), color: 'negative' }
    ]

    // --- Disputas ---
    disputasPendientes.value = disputasRes.filter(d => d.estado === 'Abierta').length
    disputasResueltas.value = disputasRes.filter(d => d.estado === 'Resuelta').length
    disputasRechazadas.value = disputasRes.filter(d => d.estado === 'Rechazada').length
    ultimasDisputas.value = [...disputasRes]
      .sort((a, b) => new Date(b.fechaReporte) - new Date(a.fechaReporte))
      .slice(0, 5)

    // --- Transacciones / volumen ---
    totalTransacciones.value = reporteRes.length
    volumenTotal.value = reporteRes.reduce((acc, t) => acc + (t.monto || 0), 0)
    transaccionesCompletadas.value = reporteRes.filter(t => t.estado === 'Completada').length
    const canceladas = reporteRes.filter(t => t.estado === 'Cancelada').length
    const enDisputa = reporteRes.filter(t => t.estado === 'En Disputa').length
    const enProceso = reporteRes.filter(t => ['Pendiente', 'En Proceso', 'Pago Realizado'].includes(t.estado)).length

    barraTransacciones.value = [
      { label: 'Completadas', valor: transaccionesCompletadas.value, pct: transaccionesCompletadas.value / (totalTransacciones.value || 1), color: 'positive' },
      { label: 'En Proceso', valor: enProceso, pct: enProceso / (totalTransacciones.value || 1), color: 'blue' },
      { label: 'Canceladas', valor: canceladas, pct: canceladas / (totalTransacciones.value || 1), color: 'grey-6' },
      { label: 'En Disputa', valor: enDisputa, pct: enDisputa / (totalTransacciones.value || 1), color: 'deep-orange' }
    ]
  } catch (err) {
    console.error('Error cargando dashboard admin:', err)
  } finally {
    cargando.value = false
  }
})
</script>

<style scoped>
.kpi-card {
  border-radius: 12px;
  border: none;
}
.bg-gradient-blue {
  background: linear-gradient(135deg, #4f7cff 0%, #2c4bd6 100%);
}
.bg-gradient-orange {
  background: linear-gradient(135deg, #ff9d4f 0%, #d67a2c 100%);
}
.bg-gradient-green {
  background: linear-gradient(135deg, #4fdba0 0%, #1ea36f 100%);
}
.bg-gradient-purple {
  background: linear-gradient(135deg, #a05fff 0%, #6e2cd6 100%);
}
.access-card {
  border-radius: 12px;
  transition: transform 0.15s, box-shadow 0.15s;
}
.access-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}
.full-height {
  height: 100%;
}
</style>
