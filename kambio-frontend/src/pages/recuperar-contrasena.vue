<template>
  <div class="recuperar-page flex flex-center">
    <q-card flat bordered class="recuperar-card q-pa-lg">
      <q-card-section class="text-center">
        <q-icon name="lock_reset" size="48px" color="dark" />
        <div class="text-h6 text-weight-bold q-mt-sm">¿Olvidaste tu contraseña?</div>
        <div class="text-caption text-grey-7">
          No te preocupes. Introduce tu correo electrónico y te enviaremos un enlace para restablecerla de forma segura.
        </div>
      </q-card-section>

      <q-card-section>
        <q-form v-if="!enviado" @submit.prevent="onSubmit" class="q-gutter-md">
          <q-input
            v-model="correo"
            label="Correo Electrónico"
            placeholder="ejemplo@correo.com"
            type="email"
            outlined
            dense
            :rules="[val => !!val || 'El correo es obligatorio']"
          />

          <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders">
            {{ errorMensaje }}
          </q-banner>

          <q-btn
            type="submit"
            label="Enviar Enlace"
            icon-right="arrow_forward"
            color="dark"
            unelevated
            class="full-width"
            :loading="cargando"
          />
        </q-form>

        <div v-else class="text-center">
          <q-icon name="mark_email_read" size="48px" color="positive" />
          <div class="text-weight-medium q-mt-sm">Solicitud enviada</div>
          <div class="text-caption text-grey-7 q-mb-md">
            Si el correo existe en nuestro sistema, recibirás un enlace de recuperación en breve.
          </div>
          <q-btn
            label="Ya tengo mi enlace"
            flat
            color="primary"
            to="/restablecer-contrasena"
          />
        </div>
      </q-card-section>

      <q-card-section class="text-center">
        <router-link to="/login" class="text-caption text-grey-7">
          ← Volver a inicio de sesión
        </router-link>
      </q-card-section>
    </q-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { recuperacionService } from '../services/recuperacionService'

const correo = ref('')
const cargando = ref(false)
const errorMensaje = ref('')
const enviado = ref(false)

async function onSubmit () {
  errorMensaje.value = ''
  cargando.value = true
  try {
    await recuperacionService.solicitarRecuperacion(correo.value)
    enviado.value = true
  } catch (error) {
    errorMensaje.value = error.response?.data?.mensaje || error.response?.data?.error || 'Ocurrió un error. Intenta de nuevo.'
  } finally {
    cargando.value = false
  }
}
</script>

<style scoped>
.recuperar-page {
  min-height: 100vh;
  background: #f5f5f7;
}

.recuperar-card {
  width: 100%;
  max-width: 420px;
}
</style>
