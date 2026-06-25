<template>
  <div class="restablecer-page flex flex-center">
    <q-card flat bordered class="restablecer-card q-pa-lg">
      <q-card-section class="text-center">
        <q-icon name="vpn_key" size="48px" color="dark" />
        <div class="text-h6 text-weight-bold q-mt-sm">Restablecer Contraseña</div>
        <div class="text-caption text-grey-7">
          Elige una contraseña segura para proteger tus operaciones en Kambio.
        </div>
      </q-card-section>

      <q-card-section>
        <q-form v-if="!exitoso" @submit.prevent="onSubmit" class="q-gutter-md">
          <q-input
            v-model="token"
            label="Código / Token de recuperación"
            outlined
            dense
            :rules="[val => !!val || 'El token es obligatorio']"
            hint="Lo encuentras en la consola del servidor (modo desarrollo)"
          />

          <q-input
            v-model="nuevaContrasena"
            label="Nueva Contraseña"
            :type="mostrarPassword ? 'text' : 'password'"
            outlined
            dense
            hint="Mínimo 8 caracteres"
            :rules="[
              val => !!val || 'Obligatorio',
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

          <q-input
            v-model="confirmarContrasena"
            label="Confirmar Nueva Contraseña"
            :type="mostrarPassword ? 'text' : 'password'"
            outlined
            dense
            :rules="[
              val => !!val || 'Obligatorio',
              val => val === nuevaContrasena || 'Las contraseñas no coinciden'
            ]"
          />

          <q-banner v-if="errorMensaje" class="bg-red-1 text-red-9 rounded-borders">
            {{ errorMensaje }}
          </q-banner>

          <q-btn
            type="submit"
            label="Actualizar Contraseña"
            color="positive"
            unelevated
            class="full-width"
            :loading="cargando"
          />
        </q-form>

        <div v-else class="text-center">
          <q-icon name="check_circle" size="48px" color="positive" />
          <div class="text-weight-medium q-mt-sm">¡Contraseña actualizada!</div>
          <div class="text-caption text-grey-7 q-mb-md">
            Ya puedes iniciar sesión con tu nueva contraseña.
          </div>
          <q-btn label="Ir a Iniciar Sesión" color="dark" unelevated to="/login" />
        </div>
      </q-card-section>

      <q-card-section v-if="!exitoso" class="text-center">
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

const token = ref('')
const nuevaContrasena = ref('')
const confirmarContrasena = ref('')
const mostrarPassword = ref(false)
const cargando = ref(false)
const errorMensaje = ref('')
const exitoso = ref(false)

async function onSubmit () {
  errorMensaje.value = ''
  cargando.value = true
  try {
    await recuperacionService.restablecerContrasena({
      token: token.value,
      nuevaContrasena: nuevaContrasena.value,
      confirmarContrasena: confirmarContrasena.value
    })
    exitoso.value = true
  } catch (error) {
    errorMensaje.value = error.response?.data?.mensaje || error.response?.data?.error || 'Ocurrió un error. Intenta de nuevo.'
  } finally {
    cargando.value = false
  }
}
</script>

<style scoped>
.restablecer-page {
  min-height: 100vh;
  background: #f5f5f7;
}

.restablecer-card {
  width: 100%;
  max-width: 420px;
}
</style>
