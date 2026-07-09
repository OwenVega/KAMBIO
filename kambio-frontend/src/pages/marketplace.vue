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
              <q-item v-if="authStore.esAdmin" clickable v-close-popup to="/admin/disputas">
                <q-item-section avatar>
                  <q-icon name="gavel" color="orange" />
                </q-item-section>
                <q-item-section>Panel Admin</q-item-section>
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
            <div class="text-h6 text-weight-bold">Mercado P2P</div>
            <div class="text-caption text-grey-7">
              Intercambia divisas directamente con otros usuarios de forma segura.
            </div>
          </div>

          <q-btn-toggle
            v-model="tipoOferta"
            no-caps
            rounded
            unelevated
            toggle-color="dark"
            color="white"
            text-color="dark"
            :options="[
              { label: 'Comprar', value: 1 },
              { label: 'Vender', value: 2 }
            ]"
          />
        </div>

        <q-card flat bordered class="q-pa-md q-mb-md">
          <div class="row q-col-gutter-md items-end">
            <div class="col-12 col-md-3">
              <q-select
                v-model="divisaOrigen"
                :options="opcionesDivisa"
                label="Tengo"
                outlined
                dense
                emit-value
                map-options
              />
            </div>
            <div class="col-12 col-md-3">
              <q-select
                v-model="divisaDestino"
                :options="opcionesDivisa"
                label="Quiero recibir"
                outlined
                dense
                emit-value
                map-options
              />
            </div>
            <div class="col-12 col-md-2">
              <q-input
                v-model.number="monto"
                label="Monto (opcional)"
                placeholder="Ej: 1,000"
                outlined
                dense
                type="number"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-btn
                label="Buscar Ofertas"
                icon="search"
                color="dark"
                unelevated
                class="full-width"
                :loading="cargando"
                @click="buscarOfertas"
              />
            </div>
          </div>
        </q-card>

        <q-card flat bordered>
          <q-table
            :rows="ofertas"
            :columns="columnas"
            row-key="idOferta"
            flat
            :loading="cargando"
            hide-pagination
            :rows-per-page-options="[0]"
          >
            <template v-slot:body-cell-anunciante="props">
              <q-td :props="props">
                <div class="row items-center q-gutter-sm">
                  <q-avatar size="32px" color="primary" text-color="white">
                    {{ obtenerIniciales(props.row.anuncianteNombre) }}
                  </q-avatar>
                  <div>
                    <div class="text-weight-medium">{{ props.row.anuncianteNombre }}</div>
                    <div class="text-caption text-grey-7">
                      <q-icon name="circle" size="8px" color="green" class="q-mr-xs" />
                      {{ props.row.porcentajeReputacion }}% ({{ props.row.ordenesCompletadas }} órdenes)
                    </div>
                  </div>
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-precio="props">
              <q-td :props="props">
                <div class="text-weight-bold">{{ formatearMoneda(props.row.tasaCambio) }} {{ divisaDestinoCodigo }}</div>
                <div class="text-caption text-grey-7">Tasa de cambio</div>
              </q-td>
            </template>

            <template v-slot:body-cell-disponible="props">
              <q-td :props="props">
                <div>{{ formatearMoneda(props.row.montoDisponible) }} {{ divisaOrigenCodigo }}</div>
                <div class="text-caption text-grey-7">
                  Límite: {{ formatearMoneda(props.row.limiteMinimo) }} - {{ formatearMoneda(props.row.limiteMaximo) }}
                </div>
              </q-td>
            </template>

            <template v-slot:body-cell-metodo="props">
              <q-td :props="props">
                <q-chip
                  v-for="(metodo, idx) in props.row.metodosPago"
                  :key="idx"
                  dense
                  size="sm"
                  color="grey-3"
                  text-color="dark"
                >
                  {{ metodo }}
                </q-chip>
              </q-td>
            </template>

            <template v-slot:body-cell-accion="props">
              <q-td :props="props">
                <q-btn
                  :label="tipoOferta === 1 ? `Comprar ${divisaOrigenCodigo}` : `Vender ${divisaOrigenCodigo}`"
                  :color="tipoOferta === 1 ? 'positive' : 'negative'"
                  unelevated
                  dense
                  no-caps
                  size="sm"
                  @click="seleccionarOferta(props.row)"
                />
              </q-td>
            </template>

            <template v-slot:no-data>
              <div class="full-width text-center q-pa-lg text-grey-7">
                <q-icon name="search_off" size="48px" class="q-mb-sm" />
                <div>No se encontraron ofertas con esos filtros.</div>
                <div class="text-caption">Intenta cambiar el monto o el par de divisas.</div>
              </div>
            </template>
          </q-table>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '../stores/auth-store'
import { ofertaService } from '../services/ofertaService'
import { transaccionService } from '../services/transaccionService'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()


const tipoOferta = ref(1) // 1 = Compra, 2 = Venta
const divisaOrigen = ref(1) // USD
const divisaDestino = ref(2) // PEN
const monto = ref(null)
const cargando = ref(false)
const ofertas = ref([])

const opcionesDivisa = [
  { label: 'USD - Dólar Estadounidense', value: 1 },
  { label: 'PEN - Sol Peruano', value: 2 },
  { label: 'EUR - Euro', value: 3 },
  { label: 'GBP - Libra Esterlina', value: 4 },
  { label: 'JPY - Yen Japonés', value: 5 },
  { label: 'CHF - Franco Suizo', value: 6 }
]

const codigosDivisa = {
  1: 'USD',
  2: 'PEN',
  3: 'EUR',
  4: 'GBP',
  5: 'JPY',
  6: 'CHF'
}

const divisaOrigenCodigo = computed(() => codigosDivisa[divisaOrigen.value] || '')
const divisaDestinoCodigo = computed(() => codigosDivisa[divisaDestino.value] || '')

const columnas = [
  { name: 'anunciante', label: 'Anunciante', field: 'anuncianteNombre', align: 'left' },
  { name: 'precio', label: 'Precio', field: 'tasaCambio', align: 'left' },
  { name: 'disponible', label: 'Disponible / Límites', field: 'montoDisponible', align: 'left' },
  { name: 'metodo', label: 'Método', field: 'metodosPago', align: 'left' },
  { name: 'accion', label: 'Acción', field: 'idOferta', align: 'right' }
]

function obtenerIniciales (nombreCompleto) {
  if (!nombreCompleto) return '?'
  const partes = nombreCompleto.trim().split(' ')
  if (partes.length === 1) return partes[0].charAt(0).toUpperCase()
  return (partes[0].charAt(0) + partes[1].charAt(0)).toUpperCase()
}

function formatearMoneda (valor) {
  if (valor == null) return '0.00'
  return Number(valor).toLocaleString('es-PE', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

async function buscarOfertas () {
  cargando.value = true
  try {
    const resultado = await ofertaService.obtenerOfertasMercado({
      idTipoOferta: tipoOferta.value,
      idDivisaOrigen: divisaOrigen.value,
      idDivisaDestino: divisaDestino.value,
      monto: monto.value
    })
    ofertas.value = resultado
  } catch (error) {
    console.error('Error al buscar ofertas:', error)
    ofertas.value = []
  } finally {
    cargando.value = false
  }
}

async function seleccionarOferta (oferta) {
  try {
    const resultado = await transaccionService.crearDesdeOferta(oferta.idOferta, authStore.usuarioId)
    router.push(`/transaccion/${resultado.transaccion.idTransaccion}`)
  } catch (error) {
    const mensaje = error.response?.data?.error || 'No se pudo iniciar la transacción.'

    $q.notify({
      type: 'negative',
      message: mensaje,
      icon: 'report_problem',
      position: 'top',
      timeout: 4000
    })

    // Si la oferta ya no está disponible, refrescamos la lista para que desaparezca
    if (mensaje.includes('tomada por otro usuario') || mensaje.includes('ya no está disponible')) {
      buscarOfertas()
    }
  }
}
onMounted(() => {
  buscarOfertas()
})
</script>
