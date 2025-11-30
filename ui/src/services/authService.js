import { saveSession, clearSession, getSession, isAuthenticated as isSessionValid } from './sessionStore'

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

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      Accept: 'application/json',
      'Content-Type': 'application/json',
      ...(options.headers || {})
    },
    ...options
  })

  if (!response.ok) {
    let message = 'Falha na autenticação'
    try {
      const payload = await response.clone().json()
      if (payload?.message) {
        message = payload.message
      }
    } catch (_) {
      // mantém mensagem padrão
    }
    throw new Error(message)
  }

  return response.json()
}

export async function login(credentials) {
  const payload = await request('/auth/login', {
    method: 'POST',
    body: JSON.stringify(credentials)
  })

  saveSession({
    token: payload.access_token,
    expiresAt: payload.expires_at
  })

  return payload
}

export function logout() {
  clearSession()
}

export function getCurrentSession() {
  return getSession()
}

export function isAuthenticated() {
  return isSessionValid()
}

