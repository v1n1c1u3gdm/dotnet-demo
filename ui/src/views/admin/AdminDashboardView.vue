<template>
  <AdminLayout title="Painel de artigos" subtitle="Visualize e mantenha os artigos publicados no site." @logout="handleLogout">
    <template #toolbar>
      <button type="button" class="admin-button admin-button--primary" @click="openCreateModal">
        <i class="fas fa-plus me-1"></i>
        Novo
      </button>
    </template>

    <div class="admin-dashboard">
      <div class="admin-widgets">
        <div class="admin-widget">
          <p>Artigos publicados</p>
          <strong>{{ articles.length }}</strong>
        </div>
        <div class="admin-widget">
          <p>Autores disponíveis</p>
          <strong>{{ authors.length }}</strong>
        </div>
        <div class="admin-widget">
          <p>Última atualização</p>
          <strong>{{ lastUpdatedLabel }}</strong>
        </div>
      </div>

      <div class="admin-table-card">
        <div class="admin-table-card__header">
          <div>
            <h3>Lista de artigos</h3>
            <p>Datatable local com paginação, busca e ações rápidas.</p>
          </div>
        </div>

        <transition name="fade">
          <div v-if="successMessage" class="alert alert-success" role="status">
            {{ successMessage }}
          </div>
        </transition>

        <transition name="fade">
          <div v-if="errorMessage" class="alert alert-danger" role="alert">
            {{ errorMessage }}
          </div>
        </transition>

        <div class="position-relative">
          <div v-if="isLoading" class="admin-overlay">
            <div class="spinner-border text-dark" role="status"></div>
          </div>

          <div class="table-responsive">
            <table ref="articlesTable" class="table table-striped table-hover admin-table">
              <thead>
                <tr>
                  <th>Título</th>
                  <th>Slug</th>
                  <th>Publicado</th>
                  <th>Atualizado</th>
                  <th class="text-end">Ações</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="article in articles" :key="article.id">
                  <td>
                    <span class="admin-table__title">{{ article.title }}</span>
                  </td>
                  <td>
                    <code>{{ article.slug }}</code>
                  </td>
                  <td>{{ article.published_label || '—' }}</td>
                  <td>{{ formatDate(article.updated_at) }}</td>
                  <td class="text-end admin-table__actions">
                    <div class="admin-table__actions-inner">
                      <a
                        href="#"
                        class="admin-icon-link"
                        aria-label="Editar artigo"
                        title="Editar artigo"
                        @click.prevent="openEditModal(article)"
                      >
                        <i class="fas fa-pen"></i>
                      </a>
                      <a
                        href="#"
                        class="admin-icon-link admin-icon-link--danger"
                        aria-label="Remover artigo"
                        title="Remover artigo"
                        @click.prevent="openDeleteModal(article)"
                      >
                        <i class="fas fa-trash"></i>
                      </a>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <b-modal
      ref="editModal"
      v-model="isEditModalVisible"
      :title="modalTitle"
      hide-footer
      size="lg"
      content-class="admin-modal"
      body-class="admin-modal__body"
      @shown="onEditModalShown"
      @hide="onEditModalHide"
      @hidden="resetEditForm"
    >
      <template #modal-title>
        <div class="admin-modal__title">
          <span>{{ modalTitle }}</span>
          <div class="admin-modal__badges">
            <b-badge v-if="isDirty" variant="warning" class="text-dark">Alterações não salvas</b-badge>
            <b-badge v-else-if="isCreateMode" variant="success">Novo</b-badge>
            <b-badge v-else variant="info">Editando</b-badge>
          </div>
        </div>
      </template>

      <b-form @submit.prevent="saveArticle">
        <b-form-group label="Título" label-for="article-title">
          <b-form-input id="article-title" v-model="editForm.title" required @input="markDirty" />
        </b-form-group>

        <b-form-group label="Slug" label-for="article-slug" class="mt-3">
          <b-form-input
            id="article-slug"
            v-model="editForm.slug"
            placeholder="Deixe em branco para gerar automaticamente"
            @input="markDirty"
          />
        </b-form-group>

        <b-form-group label="Rótulo publicado" label-for="article-published" class="mt-3">
          <b-form-input id="article-published" v-model="editForm.publishedLabel" @input="markDirty" />
        </b-form-group>

        <b-form-group label="Autor" label-for="article-author" class="mt-3">
          <b-form-select
            id="article-author"
            v-model="editForm.authorId"
            :options="authorOptions"
            required
            @change="markDirty"
          />
        </b-form-group>

        <b-form-group label="Conteúdo" class="mt-3">
          <div class="admin-content-counters text-muted">
            <small>Parágrafos: {{ contentParagraphCount }}</small>
            <small>Caracteres: {{ contentCharCount }}</small>
          </div>
          <textarea ref="postEditor"></textarea>
        </b-form-group>

        <div class="d-flex justify-content-end mt-4 admin-modal__footer">
          <b-button type="button" variant="outline-secondary" class="me-2" @click="isEditModalVisible = false">
            Cancelar
          </b-button>
          <b-button type="submit" variant="dark" :disabled="isSaving">
            <b-spinner small v-if="isSaving" class="me-2" />
            Salvar alterações
          </b-button>
        </div>
      </b-form>
    </b-modal>

    <b-modal v-model="isDeleteModalVisible" title="Remover artigo" hide-footer @hidden="selectedArticle = null">
      <p>
        Tem certeza que deseja remover o artigo
        <strong>{{ selectedArticle?.title }}</strong>?
      </p>

      <div class="d-flex justify-content-end mt-4">
        <b-button variant="outline-secondary" class="me-2" @click="isDeleteModalVisible = false">
          Voltar
        </b-button>
        <b-button variant="danger" :disabled="isDeleting" @click="confirmDelete">
          <b-spinner small v-if="isDeleting" class="me-2" />
          Remover
        </b-button>
      </div>
    </b-modal>
  </AdminLayout>
</template>

<script>
import AdminLayout from '@/components/AdminLayout.vue'
import {
  fetchAdminArticles,
  updateAdminArticle,
  deleteAdminArticle,
  createAdminArticle
} from '@/services/adminArticlesService'
import { fetchAuthors } from '@/services/authorsService'
import { logout } from '@/services/authService'
import $ from 'jquery'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'datatables.net-bs4'
import 'datatables.net-bs4/css/dataTables.bootstrap4.min.css'
import 'summernote/dist/summernote-bs4.css'
import 'summernote/dist/summernote-bs4.js'

if (typeof window !== 'undefined' && !window.jQuery) {
  window.jQuery = $
  window.$ = $
}

export default {
  name: 'AdminDashboardView',
  components: {
    AdminLayout
  },
  data() {
    return {
      articles: [],
      authors: [],
      isLoading: true,
      isSaving: false,
      isDeleting: false,
      isEditModalVisible: false,
      isDeleteModalVisible: false,
      successMessage: null,
      errorMessage: null,
      selectedArticle: null,
      editorInitialContent: '',
      initialSnapshot: null,
      skipUnsavedGuard: false,
      isDirty: false,
      contentCharCount: 0,
      contentParagraphCount: 0,
      editForm: {
        title: '',
        slug: '',
        publishedLabel: '',
        authorId: null
      },
      dtInstance: null,
      successTimer: null,
      isCreateMode: false
    }
  },
  computed: {
    lastUpdatedLabel() {
      if (!this.articles.length) {
        return '—'
      }
      const sorted = [...this.articles].sort((a, b) => new Date(b.updated_at) - new Date(a.updated_at))
      return this.formatDate(sorted[0].updated_at)
    },
    authorOptions() {
      return this.authors.map(author => ({
        value: author.id,
        text: author.name
      }))
    },
    modalTitle() {
      return this.isCreateMode ? 'Novo artigo' : 'Editar artigo'
    }
  },
  async mounted() {
    await this.loadAuthors()
    await this.loadArticles()
  },
  beforeDestroy() {
    this.destroyDataTable()
    if (this.successTimer) {
      clearTimeout(this.successTimer)
    }
    this.destroyEditor()
  },
  methods: {
    async loadAuthors() {
      try {
        this.authors = (await fetchAuthors({ force: true })) || []
      } catch (_) {
        this.authors = []
      }
    },
    async loadArticles() {
      this.isLoading = true
      this.errorMessage = null
      try {
        const records = await fetchAdminArticles()
        this.articles = records || []
        this.$nextTick(() => {
          this.setupDataTable()
        })
      } catch (error) {
        this.errorMessage = error.message || 'Erro ao carregar artigos.'
      } finally {
        this.isLoading = false
      }
    },
    refreshArticles() {
      this.loadArticles()
    },
    setupDataTable() {
      const table = this.$refs.articlesTable
      if (!table) {
        return
      }
      this.destroyDataTable()
      this.dtInstance = $(table).DataTable({
        order: [[3, 'desc']],
        pageLength: 10,
        language: {
          search: 'Buscar:',
          lengthMenu: 'Mostrar _MENU_ registros',
          info: 'Mostrando _START_ a _END_ de _TOTAL_ artigos',
          infoEmpty: 'Nenhum artigo encontrado',
          paginate: {
            first: 'Primeiro',
            last: 'Último',
            next: 'Próximo',
            previous: 'Anterior'
          },
          zeroRecords: 'Nenhum artigo correspondente encontrado'
        }
      })
    },
    destroyDataTable() {
      if (this.dtInstance) {
        this.dtInstance.destroy()
        this.dtInstance = null
      }
    },
    openEditModal(article) {
      this.isCreateMode = false
      this.selectedArticle = { ...article }
      this.editForm = {
        title: article.title,
        slug: article.slug,
        publishedLabel: article.published_label,
        authorId: article.author_id
      }
      this.editorInitialContent = article.post_entry || ''
      this.isEditModalVisible = true
    },
    openCreateModal() {
      this.isCreateMode = true
      this.selectedArticle = null
      this.editForm = {
        title: '',
        slug: '',
        publishedLabel: '',
        authorId: this.authors[0]?.id || null
      }
      this.editorInitialContent = ''
      this.isEditModalVisible = true
    },
    resetEditForm() {
      this.destroyEditor()
      this.selectedArticle = null
      this.isCreateMode = false
      this.editorInitialContent = ''
      this.initialSnapshot = null
      this.skipUnsavedGuard = false
      this.isDirty = false
      this.contentCharCount = 0
      this.contentParagraphCount = 0
      this.editForm = {
        title: '',
        slug: '',
        publishedLabel: '',
        authorId: null
      }
    },
    async saveArticle() {
      this.isSaving = true
      this.errorMessage = null
      try {
        const postEntry = this.getEditorContent()
        const payload = {
          title: this.editForm.title,
          slug: this.editForm.slug,
          published_label: this.editForm.publishedLabel,
          post_entry: postEntry,
          tags: this.selectedArticle?.tags || [],
          author_id: this.editForm.authorId
        }

        if (this.isCreateMode) {
          const created = await createAdminArticle(payload)
          this.articles = [created, ...this.articles]
          this.notifySuccess('Artigo criado com sucesso.')
        } else if (this.selectedArticle) {
          const updated = await updateAdminArticle(this.selectedArticle.id, payload)
          this.articles = this.articles.map(article => (article.id === updated.id ? updated : article))
          this.notifySuccess('Artigo atualizado com sucesso.')
        }

        this.skipUnsavedGuard = true
        this.isEditModalVisible = false
        this.$nextTick(() => this.setupDataTable())
      } catch (error) {
        this.errorMessage = error.message || 'Não foi possível salvar as alterações.'
      } finally {
        this.isSaving = false
      }
    },
    openDeleteModal(article) {
      this.selectedArticle = { ...article }
      this.isDeleteModalVisible = true
    },
    async confirmDelete() {
      if (!this.selectedArticle) return
      this.isDeleting = true
      this.errorMessage = null
      try {
        await deleteAdminArticle(this.selectedArticle.id)
        this.articles = this.articles.filter(article => article.id !== this.selectedArticle.id)
        this.isDeleteModalVisible = false
        this.notifySuccess('Artigo removido.')
        this.$nextTick(() => this.setupDataTable())
      } catch (error) {
        this.errorMessage = error.message || 'Falha ao remover artigo.'
      } finally {
        this.isDeleting = false
      }
    },
    notifySuccess(message) {
      this.successMessage = message
      if (this.successTimer) {
        clearTimeout(this.successTimer)
      }
      this.successTimer = setTimeout(() => {
        this.successMessage = null
        this.successTimer = null
      }, 4000)
    },
    formatDate(value) {
      if (!value) return '—'
      try {
        return new Date(value).toLocaleString('pt-BR', {
          dateStyle: 'medium',
          timeStyle: 'short'
        })
      } catch (_) {
        return value
      }
    },
    handleLogout() {
      logout()
      this.$router.push({ name: 'admin-login' })
    },
    onEditModalShown() {
      // Initialize Summernote after the modal is fully visible to avoid layout glitches.
      this.$nextTick(() => this.initializeEditor(this.editorInitialContent))
      this.$nextTick(() => {
        this.initialSnapshot = this.createSnapshot(this.editorInitialContent || '')
        this.isDirty = false
        this.updateContentStats(this.editorInitialContent || '')
        this.focusTitle()
      })
    },
    onEditModalHide(bvEvent) {
      if (this.skipUnsavedGuard || !this.initialSnapshot) {
        return
      }
      if (this.hasUnsavedChanges()) {
        bvEvent.preventDefault()
        this.askDiscardChanges()
      }
    },
    initializeEditor(content) {
      const editor = this.$refs.postEditor
      if (!editor) return
      const $editor = $(editor)
      if ($editor.data('summernote')) {
        $editor.summernote('destroy')
      }
      const vm = this
      $editor.summernote({
        height: 280,
        placeholder: 'Conteúdo do artigo...',
        toolbar: [
          ['style', ['bold', 'italic', 'underline', 'clear']],
          ['para', ['ul', 'ol', 'paragraph']],
          ['insert', ['link', 'picture', 'video', 'table']],
          ['view', ['fullscreen', 'codeview']]
        ],
        callbacks: {
          onChange(contents) {
            vm.markDirty()
            vm.updateContentStats(contents)
          }
        }
      })
      $editor.summernote('code', content || '')
      this.updateContentStats(content || '')
    },
    destroyEditor() {
      const editor = this.$refs.postEditor
      if (!editor) return
      const $editor = $(editor)
      if ($editor.data('summernote')) {
        $editor.summernote('destroy')
      }
    },
    getEditorContent() {
      const editor = this.$refs.postEditor
      if (!editor) return ''
      const $editor = $(editor)
      if ($editor.data('summernote')) {
        return $editor.summernote('code')
      }
      return ''
    },
    createSnapshot(content) {
      return JSON.stringify({
        ...this.editForm,
        content: content || ''
      })
    },
    hasUnsavedChanges() {
      const currentContent = this.getEditorContent() || this.editorInitialContent || ''
      const currentSnapshot = this.createSnapshot(currentContent)
      return currentSnapshot !== this.initialSnapshot
    },
    async askDiscardChanges() {
      const confirmed = await this.$bvModal.msgBoxConfirm(
        'Existem alterações não salvas. Deseja descartá-las?',
        {
          title: 'Descartar alterações?',
          okTitle: 'Descartar',
          okVariant: 'danger',
          cancelTitle: 'Continuar editando',
          footerClass: 'justify-content-between',
          centered: true,
          size: 'md'
        }
      )
      if (confirmed) {
        this.skipUnsavedGuard = true
        this.isEditModalVisible = false
      }
    },
    markDirty() {
      if (this.skipUnsavedGuard) return
      this.isDirty = true
    },
    focusTitle() {
      const el = this.$el?.querySelector('#article-title')
      if (el) {
        el.focus()
      }
    },
    updateContentStats(content) {
      const stats = this.computeContentStats(content)
      this.contentCharCount = stats.chars
      this.contentParagraphCount = stats.paragraphs
    },
    computeContentStats(html) {
      if (typeof document === 'undefined') {
        return { chars: 0, paragraphs: 0 }
      }
      const wrapper = document.createElement('div')
      wrapper.innerHTML = html || ''
      const text = (wrapper.textContent || '').trim()
      const paragraphs = wrapper.querySelectorAll('p').length || (text ? 1 : 0)
      return {
        chars: text.length,
        paragraphs
      }
    }
  }
}
</script>

<style scoped>
.admin-dashboard {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.admin-widgets {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(210px, 1fr));
  gap: 1rem;
}

.admin-widget {
  background: #0c1326;
  color: #fff;
  padding: 1.5rem;
  border-radius: 1rem;
  box-shadow: inset 0 0 0 1px rgba(255, 255, 255, 0.1);
}

.admin-widget p {
  margin: 0;
  opacity: 0.8;
}

.admin-widget strong {
  display: block;
  margin-top: 0.5rem;
  font-size: 1.75rem;
}

.admin-table-card {
  background: #fff;
  border-radius: 1rem;
  padding: 1.5rem;
  border: 1px solid rgba(12, 19, 38, 0.08);
  box-shadow: 0 20px 60px rgba(12, 19, 38, 0.08);
}

.admin-table-card__header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.admin-button--primary {
  background: var(--dark);
  color: #fff;
  border: none;
}

.admin-table__title {
  font-weight: 600;
}

.admin-table__actions {
  min-width: 96px;
  white-space: nowrap;
}

.admin-table__actions-inner {
  display: inline-flex;
  gap: 0.35rem;
  align-items: center;
  justify-content: flex-end;
  width: 100%;
}

.admin-icon-link {
  width: 34px;
  height: 34px;
  border-radius: 8px;
  border: 1px solid rgba(12, 19, 38, 0.15);
  background: #fff;
  color: var(--dark);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  flex-shrink: 0;
  color: inherit;
  text-decoration: none;
}

.admin-icon-link:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 20px rgba(12, 19, 38, 0.1);
}

.admin-icon-link--danger {
  border-color: rgba(229, 57, 53, 0.3);
  color: #e53935;
}

.admin-icon-link--danger:hover {
  box-shadow: 0 10px 20px rgba(229, 57, 53, 0.25);
}

.admin-overlay {
  position: absolute;
  inset: 0;
  background: rgba(255, 255, 255, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 5;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter,
.fade-leave-to {
  opacity: 0;
}

.admin-modal {
  border-radius: 1rem;
}

.admin-modal .modal-body {
  padding: 1.75rem 1.75rem 0.75rem;
}

.admin-modal__footer {
  gap: 0.75rem;
  border-top: 1px solid rgba(12, 19, 38, 0.08);
  padding-top: 1rem;
}

.admin-modal__title {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  width: 100%;
}

.admin-modal__badges {
  display: inline-flex;
  gap: 0.5rem;
  align-items: center;
}

.admin-content-counters {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.35rem;
}
</style>
