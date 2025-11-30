import { getAuthHeaders } from './sessionStore'

const DEFAULT_API_BASE_URL = detectRuntimeApiBaseUrl()
const API_BASE_URL = normalizeBaseUrl(resolveConfiguredBaseUrl() || DEFAULT_API_BASE_URL)

function resolveConfiguredBaseUrl() {
  const candidates = [
    process.env.VUE_APP_API_BASE_URL,
    process.env.VUE_APP_API_BASE
  ].filter(Boolean)

  for (const candidate of candidates) {
    const parsed = parseUrl(candidate)
    if (!parsed) {
      continue
    }

    if (!isInternalDockerHostname(parsed.hostname)) {
      return candidate
    }
  }

  return null
}

function detectRuntimeApiBaseUrl() {
  if (typeof window === 'undefined') {
    return 'http://localhost:3000'
  }

  const protocol = window.location.protocol === 'https:' ? 'https:' : 'http:'
  const hostname = window.location.hostname || 'localhost'
  const port = determineApiPort(window.location.port)
  return `${protocol}//${hostname}:${port}`
}

function determineApiPort(currentPort) {
  if (!currentPort || currentPort === '8080' || currentPort === '80') {
    return '3000'
  }

  return currentPort
}

function isInternalDockerHostname(hostname) {
  return ['api', 'backend', 'web'].includes((hostname || '').toLowerCase())
}

function parseUrl(url) {
  try {
    return new URL(url)
  } catch (_) {
    return null
  }
}

function normalizeBaseUrl(url) {
  if (!url) return DEFAULT_API_BASE_URL
  return url.replace(/\/+$/, '')
}

function buildUrl(path) {
  return `${API_BASE_URL}${path}`
}

async function request(path, options = {}) {
  const headers = {
    Accept: 'application/json',
    ...(options.method && options.method !== 'GET' ? { 'Content-Type': 'application/json' } : {}),
    ...getAuthHeaders(),
    ...(options.headers || {})
  }

  const response = await fetch(buildUrl(path), {
    ...options,
    headers
  })

  if (!response.ok) {
    const detail = await extractError(response)
    throw new Error(detail)
  }

  if (response.status === 204) {
    return null
  }

  return response.json()
}

async function extractError(response) {
  try {
    const payload = await response.clone().json()
    if (payload?.errors) {
      return payload.errors.join(', ')
    }
    if (payload?.message) {
      return payload.message
    }
  } catch (_) {
    // ignora
  }

  return `Erro ${response.status}`
}

export function fetchAdminArticles() {
  return request('/articles')
}

export function updateAdminArticle(id, payload) {
  return request(`/articles/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(payload)
  })
}

export function deleteAdminArticle(id) {
  return request(`/articles/${id}`, {
    method: 'DELETE'
  })
}

