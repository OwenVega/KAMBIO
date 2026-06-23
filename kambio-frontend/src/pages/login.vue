<template>
  <div class="login-page">
    <div class="login-panel-izq">
      <div class="login-brand">
        <h1 class="text-h3 text-weight-bold text-white">Kambio</h1>
        <p class="text-subtitle1 text-grey-4 q-mt-md">
          Intercambio peer-to-peer de divisas.
        </p>
        <p class="text-body2 text-grey-5">
          Seguridad, transparencia y agilidad en cada transacción.
        </p>

        <div class="row q-gutter-md q-mt-lg">
          <q-icon name="verified" color="green-5" size="24px" />
          <q-icon name="public" color="green-5" size="24px" />
        </div>
      </div>
    </div>

    <div class="login-panel-der flex flex-center">
      <q-card flat class="login-card q-pa-lg">
        <q-card-section>
          <div class="text-h6 text-weight-bold">Iniciar sesión</div>
          <div class="text-caption text-grey-7">Ingresa tus datos para acceder</div>
        </q-card-section>

        <q-card-section>
          <q-form @submit.prevent="onSubmit" class="q-gutter-md">
            <q-input
              v-model="correo"
              label="Correo electrónico"
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
              :rules="[val => !!val || 'La contraseña es obligatoria']"
            >
              <template v-slot:append>
                <q-icon
                  :name="mostrarPassword ? 'visibility_off' : 'visibility'"
                  class="cursor-pointer"
                  @click="mostrarPassword = !mostrarPassword"
                />
              </template>
            </q-input>

            <div class="text-right">
              <router-link to="/recuperar-contrasena" class="text-caption text-primary">
                ¿Olvidaste tu contraseña?
              </router-link>
            </div>

            <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders">
              {{ errorMensaje }}
            </q-banner>

            <q-btn
              type="submit"
              label="Login"
              color="dark"
              class="full-width"
              unelevated
              :loading="cargando"
            />
          </q-form>
        </q-card-section>

        <q-card-section class="text-center">
          <span class="text-caption">¿No tienes cuenta?</span>
          <router-link to="/registro" class="text-caption text-primary text-weight-bold q-ml-xs">
            Regístrate
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

const correo = ref('')
const contrasena = ref('')
const mostrarPassword = ref(false)
const cargando = ref(false)
const errorMensaje = ref('')

async function onSubmit () {
  errorMensaje.value = ''
  cargando.value = true

  try {
    await authStore.login({
      correo: correo.value,
      contrasena: contrasena.value
    })
    router.push('/marketplace')
  } catch (error) {
    if (error.response?.data?.error) {
      errorMensaje.value = error.response.data.error
    } else {
      errorMensaje.value = 'Ocurrió un error al iniciar sesión. Intenta de nuevo.'
    }
  } finally {
    cargando.value = false
  }
}
</script>

<style scoped>
.login-page {
  display: flex;
  min-height: 100vh;
}

.login-panel-izq {
  flex: 1;
  background: linear-gradient(135deg, #0a0e1a 0%, #1a2236 100%);
  display: flex;
  align-items: center;
  padding: 48px;
}

.login-panel-der {
  flex: 1;
  background: #f5f5f7;
}

.login-card {
  width: 100%;
  max-width: 380px;
}

@media (max-width: 768px) {
  .login-panel-izq {
    display: none;
  }
}
</style>
