<template>
  <AdminLayout :show-logout="false" title="Login" subtitle="Informe suas credenciais para acessar o painel.">
    <div class="admin-login-card">

      <b-alert v-if="errorMessage" variant="danger" show class="mb-3">
        {{ errorMessage }}
      </b-alert>

      <b-form @submit.prevent="handleSubmit">
        <b-form-group label="Usuário" label-for="username-input">
          <b-form-input
            id="username-input"
            v-model.trim="form.username"
            type="text"
            required
            autocomplete="username"
            placeholder="usuario"
          />
        </b-form-group>

        <b-form-group label="Senha" label-for="password-input" class="mt-3">
          <b-form-input
            id="password-input"
            v-model="form.password"
            type="password"
            required
            autocomplete="current-password"
            placeholder="••••••••"
          />
        </b-form-group>

        <div class="d-flex align-items-center justify-content-between mt-4">
          <small class="text-muted">Somente para uso interno.</small>
          <b-button type="submit" variant="dark" :disabled="isSubmitting">
            <b-spinner small v-if="isSubmitting" class="me-2" />
            Entrar
          </b-button>
        </div>
      </b-form>
    </div>
  </AdminLayout>
</template>

<script>
import AdminLayout from '@/components/AdminLayout.vue'
import { login, logout, isAuthenticated } from '@/services/authService'

export default {
  name: 'AdminLoginView',
  components: {
    AdminLayout
  },
  data() {
    return {
      form: {
        username: '',
        password: ''
      },
      isSubmitting: false,
      errorMessage: null
    }
  },
  created() {
    if (isAuthenticated()) {
      this.redirectAfterLogin()
    }
  },
  methods: {
    async handleSubmit() {
      this.errorMessage = null
      this.isSubmitting = true
      try {
        await login(this.form)
        this.redirectAfterLogin()
      } catch (error) {
        logout()
        this.errorMessage = error.message || 'Não foi possível autenticar.'
      } finally {
        this.isSubmitting = false
      }
    },
    redirectAfterLogin() {
      const redirect = this.$route.query.redirect || { name: 'admin-dashboard' }
      this.$router.replace(redirect)
    }
  }
}
</script>

<style scoped>
.admin-login-card {
  background: #fff;
  border-radius: 1rem;
  padding: 2rem;
  border: 1px solid rgba(12, 19, 38, 0.08);
  box-shadow: 0 20px 60px rgba(12, 19, 38, 0.08);
}

.admin-login-card__header h3 {
  margin: 0 0 0.25rem;
  font-size: 1.5rem;
}

.admin-login-card__header p {
  margin: 0;
  color: var(--gray-2);
}
</style>

