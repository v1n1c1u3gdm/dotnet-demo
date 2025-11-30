const STORAGE_KEY = 'dotnet-demo-admin-session'

function getStorage() {
  if (typeof window === 'undefined' || !window.localStorage) return null
  return window.localStorage
}

export function saveSession({ token, expiresAt }) {
  const storage = getStorage()
  if (!storage) return

  const payload = {
    token,
    expiresAt: expiresAt ? new Date(expiresAt).toISOString() : null
  }

  storage.setItem(STORAGE_KEY, JSON.stringify(payload))
}

export function getSession() {
  const storage = getStorage()
  if (!storage) return null

  const raw = storage.getItem(STORAGE_KEY)
  if (!raw) return null

  try {
    const session = JSON.parse(raw)
    if (!session?.token) {
      clearSession()
      return null
    }

    if (session.expiresAt && new Date(session.expiresAt) <= new Date()) {
      clearSession()
      return null
    }

    return session
  } catch (error) {
    clearSession()
    return null
  }
}

export function clearSession() {
  const storage = getStorage()
  if (!storage) return
  storage.removeItem(STORAGE_KEY)
}

export function isAuthenticated() {
  return Boolean(getSession())
}

export function getAuthHeaders() {
  const session = getSession()
  if (!session?.token) {
    return {}
  }

  return {
    Authorization: `Bearer ${session.token}`
  }
}

