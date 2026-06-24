<template>
  <div class="register-page">
    <div class="register-panel-izq">
      <div class="register-brand">
        <h1 class="text-h3 text-weight-bold text-white">Kambio</h1>
        <p class="text-body1 text-grey-4 q-mt-md">
          Intercambio peer-to-peer de divisas. Seguridad, transparencia y agilidad para el ciudadano global.
        </p>

        <div class="q-mt-xl q-gutter-md">
          <div class="row items-center q-gutter-sm">
            <q-icon name="bolt" color="green-5" size="20px" />
            <span class="text-grey-4">Liquidación en Tiempo Real</span>
          </div>
          <div class="row items-center q-gutter-sm">
            <q-icon name="public" color="green-5" size="20px" />
            <span class="text-grey-4">Acceso a Mercados</span>
          </div>
        </div>
      </div>
    </div>

    <div class="register-panel-der flex flex-center">
      <q-card flat class="register-card q-pa-lg">
        <q-card-section>
          <div class="text-h6 text-weight-bold">Crear cuenta</div>
          <div class="text-caption text-grey-7">Complete sus datos para comenzar a operar.</div>
        </q-card-section>

        <q-card-section>
          <q-form @submit.prevent="onSubmit" class="q-gutter-md">
            <div class="row q-gutter-sm">
              <q-input
                v-model="nombres"
                label="Nombres"
                placeholder="Ej. Juan"
                outlined
                dense
                class="col"
                :rules="[val => !!val || 'Campo obligatorio']"
              />
              <q-input
                v-model="apellidos"
                label="Apellidos"
                placeholder="Ej. Pérez"
                outlined
                dense
                class="col"
                :rules="[val => !!val || 'Campo obligatorio']"
              />
            </div>

            <q-input
              v-model="correo"
              label="Correo electrónico"
              placeholder="nombre@ejemplo.com"
              type="email"
              outlined
              dense
              :rules="[val => !!val || 'El correo es obligatorio']"
            />

            <q-input
              v-model="contrasena"
              label="Contraseña"
              :type="mostrarPassword ? 'text' : 'password'"
              outlined
              dense
              hint="Mínimo 8 caracteres"
              :rules="[
                val => !!val || 'La contraseña es obligatoria',
                val => val.length >= 8 || 'Debe tener al menos 8 caracteres'
              ]"
            >
              <template v-slot:append>
                <q-icon
                  :name="mostrarPassword ? 'visibility_off' : 'visibility'"
                  class="cursor-pointer"
                  @click="mostrarPassword = !mostrarPassword"
                />
              </template>
            </q-input>

            <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders">
              {{ errorMensaje }}
            </q-banner>

            <q-banner v-if="exitoMensaje" class="bg-green-1 text-green-9 rounded-borders">
              {{ exitoMensaje }}
            </q-banner>

            <q-btn
              type="submit"
              label="Crear cuenta"
              color="dark"
              class="full-width"
              unelevated
              :loading="cargando"
            />
          </q-form>
        </q-card-section>

        <q-card-section class="text-center">
          <span class="text-caption">¿Ya tienes una cuenta?</span>
          <router-link to="/login" class="text-caption text-primary text-weight-bold q-ml-xs">
            Inicia sesión
          </router-link>
        </q-card-section>
      </q-card>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth-store'

const router = useRouter()
const authStore = useAuthStore()

const nombres = ref('')
const apellidos = ref('')
const correo = ref('')
const contrasena = ref('')
const mostrarPassword = ref(false)
const cargando = ref(false)
const errorMensaje = ref('')
const exitoMensaje = ref('')

async function onSubmit () {
  errorMensaje.value = ''
  exitoMensaje.value = ''
  cargando.value = true

  try {
    await authStore.registrar({
      nombres: nombres.value,
      apellidos: apellidos.value,
      correo: correo.value,
      contrasena: contrasena.value
    })

    exitoMensaje.value = 'Cuenta creada correctamente. Redirigiendo al login...'

    setTimeout(() => {
      router.push('/login')
    }, 1500)
  } catch (error) {
    if (error.response?.data?.error) {
      errorMensaje.value = error.response.data.error
    } else {
      errorMensaje.value = 'Ocurrió un error al crear la cuenta. Intenta de nuevo.'
    }
  } finally {
    cargando.value = false
  }
}
</script>

<style scoped>
.register-page {
  display: flex;
  min-height: 100vh;
}

.register-panel-izq {
  flex: 1;
  background: linear-gradient(135deg, #0a0e1a 0%, #1a2236 100%);
  display: flex;
  align-items: center;
  padding: 48px;
}

.register-panel-der {
  flex: 1;
  background: #f5f5f7;
}

.register-card {
  width: 100%;
  max-width: 420px;
}

@media (max-width: 768px) {
  .register-panel-izq {
    display: none;
  }
}
</style>
