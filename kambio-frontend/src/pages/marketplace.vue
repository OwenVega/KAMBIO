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
          <q-route-tab name="mensajes" label="Mensajes" no-caps to="/mensajes" />
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
        <q-card flat bordered class="q-pa-md q-mb-md" :class="cargandoCotizacion ? 'bg-grey-2' : 'bg-blue-1'">
          <div class="row items-center justify-between">
            <div class="row items-center q-gutter-sm">
              <q-icon name="show_chart" color="blue-9" size="24px" />
              <div>
                <div class="text-caption text-blue-9">Cotización de mercado (referencial)</div>
                <div v-if="cotizacionMercado" class="text-weight-bold text-blue-9">
                  1 {{ divisaOrigenCodigo }} = {{ formatearMoneda(cotizacionMercado.tasa) }} {{ divisaDestinoCodigo }}
                </div>
                <div v-else-if="cargandoCotizacion" class="text-caption text-grey-6">
                  Cargando cotización...
                </div>
                <div v-else class="text-caption text-grey-6">
                  Cotización no disponible
                </div>
              </div>
            </div>
            <div v-if="cotizacionMercado" class="text-caption text-grey-7">
              Fuente: ExchangeRate-API — {{ cotizacionMercado.fecha }}
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
                <div class="row items-center q-gutter-sm cursor-pointer" @click="verPerfilAnunciante(props.row)">
                  <q-avatar size="32px" color="primary" text-color="white">
                    {{ obtenerIniciales(props.row.anuncianteNombre) }}
                  </q-avatar>
                  <div>
                    <div class="row items-center q-gutter-xs">
                      <span class="text-weight-medium text-primary">{{ props.row.anuncianteNombre }}</span>
                      <q-chip
                        v-if="obtenerBadge(props.row.porcentajeReputacion, props.row.ordenesCompletadas)"
                        dense
                        size="sm"
                        :color="obtenerBadge(props.row.porcentajeReputacion, props.row.ordenesCompletadas).color"
                        text-color="white"
                        :icon="obtenerBadge(props.row.porcentajeReputacion, props.row.ordenesCompletadas).icon"
                      >
                        {{ obtenerBadge(props.row.porcentajeReputacion, props.row.ordenesCompletadas).label }}
                      </q-chip>
                    </div>
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
        <q-dialog v-model="mostrarPerfil">
      <q-card style="min-width: 400px; max-width: 500px" class="q-pa-md" v-if="anuncianteSeleccionado">
        <q-card-section class="text-center">
          <q-avatar size="64px" color="primary" text-color="white" class="q-mb-sm">
            {{ obtenerIniciales(anuncianteSeleccionado.anuncianteNombre) }}
          </q-avatar>
          <div class="text-h6 text-weight-bold">{{ anuncianteSeleccionado.anuncianteNombre }}</div>

          <q-chip
            v-if="obtenerBadge(anuncianteSeleccionado.porcentajeReputacion, anuncianteSeleccionado.ordenesCompletadas)"
            dense
            :color="obtenerBadge(anuncianteSeleccionado.porcentajeReputacion, anuncianteSeleccionado.ordenesCompletadas).color"
            text-color="white"
            :icon="obtenerBadge(anuncianteSeleccionado.porcentajeReputacion, anuncianteSeleccionado.ordenesCompletadas).icon"
            class="q-mt-xs"
          >
            {{ obtenerBadge(anuncianteSeleccionado.porcentajeReputacion, anuncianteSeleccionado.ordenesCompletadas).label }}
          </q-chip>

          <div class="row justify-center q-gutter-md q-mt-md">
            <div class="text-center">
              <div class="text-h6 text-weight-bold text-amber-9">
                <q-icon name="star" color="amber" /> {{ promedioAnunciante?.promedio ?? '—' }}
              </div>
              <div class="text-caption text-grey-7">{{ promedioAnunciante?.totalCalificaciones ?? 0 }} calificaciones</div>
            </div>
            <div class="text-center">
              <div class="text-h6 text-weight-bold">{{ anuncianteSeleccionado.ordenesCompletadas }}</div>
              <div class="text-caption text-grey-7">Órdenes completadas</div>
            </div>
          </div>
        </q-card-section>

        <q-separator />

        <q-card-section>
          <div class="text-subtitle2 text-weight-bold q-mb-sm">Reseñas recientes</div>

          <q-inner-loading :showing="cargandoResenas">
            <q-spinner color="primary" size="2em" />
          </q-inner-loading>

          <div v-if="!cargandoResenas && resenas.length === 0" class="text-caption text-grey-6 text-center q-py-md">
            Este usuario todavía no tiene reseñas.
          </div>

          <q-list v-else separator style="max-height: 300px; overflow-y: auto">
            <q-item v-for="r in resenas" :key="r.idCalificacion">
              <q-item-section>
                <div class="row items-center justify-between">
                  <span class="text-weight-medium">{{ r.usuarioEvaluaNombre }}</span>
                  <div>
                    <q-icon
                      v-for="n in 5"
                      :key="n"
                      name="star"
                      :color="n <= r.estrellas ? 'amber' : 'grey-4'"
                      size="16px"
                    />
                  </div>
                </div>
                <q-item-label caption v-if="r.comentario">{{ r.comentario }}</q-item-label>
                <q-item-label caption class="text-grey-5">{{ formatearFechaResena(r.fechaCalificacion) }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </q-card-section>
      </q-card>
    </q-dialog>
  </q-layout>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useQuasar } from 'quasar'
import { useAuthStore } from '../stores/auth-store'
import { ofertaService } from '../services/ofertaService'
import { transaccionService } from '../services/transaccionService'
import { cotizacionService } from '../services/cotizacionService'
import { calificacionService } from '../services/calificacionService'
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const cotizacionMercado = ref(null)
const cargandoCotizacion = ref(false)
const tipoOferta = ref(1) // 1 = Compra, 2 = Venta
const divisaOrigen = ref(1) // USD
const divisaDestino = ref(2) // PEN
const monto = ref(null)
const cargando = ref(false)
const ofertas = ref([])
const mostrarPerfil = ref(false)
const anuncianteSeleccionado = ref(null)
const promedioAnunciante = ref(null)
const resenas = ref([])
const cargandoResenas = ref(false)

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
function formatearFechaResena (fecha) {
  return new Date(fecha).toLocaleDateString('es-PE', { year: 'numeric', month: 'short', day: 'numeric' })
}

function obtenerBadge (reputacion, ordenes) {
  if (reputacion >= 95 && ordenes > 10) {
    return { label: 'Top Vendedor', color: 'amber-8', icon: 'workspace_premium' }
  }
  if (reputacion >= 80) {
    return { label: 'Confiable', color: 'blue-6', icon: 'verified' }
  }
  if (ordenes < 3) {
    return { label: 'Nuevo', color: 'grey-6', icon: 'fiber_new' }
  }
  return null
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
async function verPerfilAnunciante (oferta) {
  anuncianteSeleccionado.value = oferta
  mostrarPerfil.value = true
  cargandoResenas.value = true
  promedioAnunciante.value = null
  resenas.value = []

  // Nota: usamos oferta.idUsuario si tu OfertaP2PDTO lo trae; si no, revisa el nombre exacto del campo
  const idAnunciante = oferta.idAnunciante

  try {
    const [promedioRes, resenasRes] = await Promise.allSettled([
      calificacionService.obtenerPromedio(idAnunciante),
      calificacionService.obtenerResenas(idAnunciante)
    ])
    if (promedioRes.status === 'fulfilled') promedioAnunciante.value = promedioRes.value
    if (resenasRes.status === 'fulfilled') resenas.value = resenasRes.value
  } catch (error) {
    console.error('Error al cargar perfil del anunciante:', error)
  } finally {
    cargandoResenas.value = false
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

    if (mensaje.includes('tomada por otro usuario') || mensaje.includes('ya no está disponible')) {
      buscarOfertas()
    }
  }
}

async function cargarCotizacion () {
  cargandoCotizacion.value = true
  cotizacionMercado.value = null
  try {
    const resultado = await cotizacionService.obtenerTasa(divisaOrigenCodigo.value, divisaDestinoCodigo.value)
    cotizacionMercado.value = resultado
  } catch (error) {
    console.error('Error al obtener cotización:', error)
  } finally {
    cargandoCotizacion.value = false
  }
}

watch([divisaOrigen, divisaDestino], () => {
  cargarCotizacion()
})

function cerrarSesion () {
  authStore.cerrarSesion()
  router.push('/login')
}

onMounted(() => {
  buscarOfertas()
  cargarCotizacion()
})
</script>
