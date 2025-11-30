<template>
  <SiteLayout>
    <template #main-left>
      <div class="admin-hero">
        <p class="admin-eyebrow">/admin</p>
        <h1 class="admin-hero__title">Área administrativa</h1>
      </div>
    </template>

    <template #main-right>
      <section class="admin-shell">
        <header class="admin-shell__toolbar">
          <div>
            <h2 class="admin-shell__title">{{ title }}</h2>
            <p class="admin-shell__subtitle">
              {{ subtitle }}
            </p>
          </div>
          <div class="admin-shell__actions">
            <slot name="toolbar" />
            <button
              v-if="showLogout"
              type="button"
              class="admin-button admin-button--ghost"
              @click="$emit('logout')"
            >
              Sair
            </button>
          </div>
        </header>

        <div class="admin-shell__content">
          <slot />
        </div>
      </section>
    </template>
  </SiteLayout>
</template>

<script>
import SiteLayout from '@/components/SiteLayout.vue'

export default {
  name: 'AdminLayout',
  components: {
    SiteLayout
  },
  props: {
    title: {
      type: String,
      default: 'Painel administrativo'
    },
    subtitle: {
      type: String,
      default: 'Acompanhe a publicação dos artigos e mantenha tudo atualizado.'
    },
    showLogout: {
      type: Boolean,
      default: true
    }
  }
}
</script>

<style scoped>
.admin-shell {
  background: var(--white);
  border-radius: 1.5rem;
  padding: 2rem;
  box-shadow: 0 30px 80px rgba(12, 19, 38, 0.08);
  border: 1px solid rgba(12, 19, 38, 0.04);
}

.admin-shell__toolbar {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 2rem;
}

.admin-eyebrow {
  text-transform: uppercase;
  letter-spacing: 0.25rem;
  font-size: 0.65rem;
  margin-bottom: 0.35rem;
  color: var(--gray-2);
}

.admin-shell__title {
  margin: 0;
  font-size: clamp(1.5rem, 3vw, 2.1rem);
  color: var(--dark);
}

.admin-shell__subtitle {
  margin: 0.35rem 0 0;
  color: var(--gray-2);
  max-width: 38rem;
}

.admin-shell__actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.admin-shell__content {
  min-height: 200px;
}

:deep(.admin-button) {
  border: none;
  padding: 0.65rem 1.5rem;
  border-radius: 999px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

:deep(.admin-button--ghost) {
  border: 1px solid var(--dark);
  background: transparent;
  color: var(--dark);
}

:deep(.admin-button--ghost:hover) {
  transform: translateY(-1px);
  box-shadow: 0 15px 20px rgba(12, 19, 38, 0.1);
}

.admin-hero {
  padding: 2rem;
}

.admin-hero__title {
  font-size: clamp(2rem, 5vw, 3rem);
  margin: 0 0 1rem;
  color: var(--white);
}

.admin-hero__subtitle {
  margin: 0 0 1.5rem;
  color: rgba(255, 255, 255, 0.85);
}

.admin-hero__summary {
  list-style: none;
  padding: 0;
  margin: 0;
  display: grid;
  gap: 0.5rem;
  color: rgba(255, 255, 255, 0.75);
}

.admin-hero__summary li::before {
  content: '•';
  margin-right: 0.35rem;
}

@media (max-width: 768px) {
  .admin-shell {
    padding: 1.5rem;
  }

  .admin-shell__toolbar {
    flex-direction: column;
  }

  .admin-shell__actions {
    width: 100%;
    justify-content: flex-start;
  }
}
</style>

