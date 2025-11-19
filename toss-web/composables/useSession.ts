import { ref, computed } from 'vue'
import type { SessionInfo } from '~/types/audit'

export const useSession = () => {
  const { user, isAuthenticated, logout } = useAuth()
  const { logSecurityAlert } = useAudit()
  
  const sessionInfo = ref<SessionInfo | null>(null)
  const sessionCheckInterval = ref<NodeJS.Timeout | null>(null)
  const inactivityTimeout = ref<NodeJS.Timeout | null>(null)
  
  const INACTIVITY_TIMEOUT = 30 * 60 * 1000 // 30 minutes
  const SESSION_CHECK_INTERVAL = 60 * 1000 // 1 minute

  const isSessionActive = computed(() => {
    return sessionInfo.value?.isActive ?? false
  })

  const timeUntilExpiry = computed(() => {
    if (!sessionInfo.value) return 0
    return Math.max(0, sessionInfo.value.expiresAt.getTime() - Date.now())
  })

  const initializeSession = async () => {
    if (!isAuthenticated.value) return

    try {
      const response = await $fetch<SessionInfo>('/api/auth/session', {
        method: 'GET',
      })

      sessionInfo.value = {
        ...response,
        createdAt: new Date(response.createdAt),
        lastActivity: new Date(response.lastActivity),
        expiresAt: new Date(response.expiresAt),
      }

      startSessionMonitoring()
      startInactivityTimer()
    } catch (error) {
      console.error('Failed to initialize session:', error)
    }
  }

  const updateSessionActivity = async () => {
    if (!isAuthenticated.value || !sessionInfo.value) return

    try {
      await $fetch('/api/auth/session/activity', {
        method: 'POST',
      })

      if (sessionInfo.value) {
        sessionInfo.value.lastActivity = new Date()
      }

      resetInactivityTimer()
    } catch (error) {
      console.error('Failed to update session activity:', error)
    }
  }

  const startSessionMonitoring = () => {
    if (sessionCheckInterval.value) {
      clearInterval(sessionCheckInterval.value)
    }

    sessionCheckInterval.value = setInterval(async () => {
      await checkSessionValidity()
    }, SESSION_CHECK_INTERVAL)
  }

  const checkSessionValidity = async () => {
    if (!isAuthenticated.value) {
      stopSessionMonitoring()
      return
    }

    try {
      const response = await $fetch<{ valid: boolean }>('/api/auth/session/validate', {
        method: 'GET',
      })

      if (!response.valid) {
        await logSecurityAlert('Session validation failed - forcing logout')
        await handleSessionExpiry()
      }
    } catch (error) {
      console.error('Session validation failed:', error)
      await handleSessionExpiry()
    }
  }

  const startInactivityTimer = () => {
    resetInactivityTimer()

    // Listen for user activity
    if (process.client) {
      const events = ['mousedown', 'keydown', 'scroll', 'touchstart']
      events.forEach(event => {
        window.addEventListener(event, resetInactivityTimer)
      })
    }
  }

  const resetInactivityTimer = () => {
    if (inactivityTimeout.value) {
      clearTimeout(inactivityTimeout.value)
    }

    inactivityTimeout.value = setTimeout(async () => {
      await logSecurityAlert('Session expired due to inactivity')
      await handleSessionExpiry()
    }, INACTIVITY_TIMEOUT)
  }

  const handleSessionExpiry = async () => {
    stopSessionMonitoring()
    stopInactivityTimer()
    
    await logout()
    
    // Show notification
    const notificationStore = useNotificationStore()
    notificationStore.add({
      type: 'warning',
      title: 'Session Expired',
      message: 'Your session has expired. Please log in again.',
    })
    
    // Redirect to login
    await navigateTo('/auth/login')
  }

  const stopSessionMonitoring = () => {
    if (sessionCheckInterval.value) {
      clearInterval(sessionCheckInterval.value)
      sessionCheckInterval.value = null
    }
  }

  const stopInactivityTimer = () => {
    if (inactivityTimeout.value) {
      clearTimeout(inactivityTimeout.value)
      inactivityTimeout.value = null
    }

    // Remove event listeners
    if (process.client) {
      const events = ['mousedown', 'keydown', 'scroll', 'touchstart']
      events.forEach(event => {
        window.removeEventListener(event, resetInactivityTimer)
      })
    }
  }

  const terminateSession = async () => {
    try {
      await $fetch('/api/auth/session/terminate', {
        method: 'POST',
      })

      stopSessionMonitoring()
      stopInactivityTimer()
      sessionInfo.value = null
    } catch (error) {
      console.error('Failed to terminate session:', error)
    }
  }

  return {
    sessionInfo,
    isSessionActive,
    timeUntilExpiry,
    initializeSession,
    updateSessionActivity,
    checkSessionValidity,
    handleSessionExpiry,
    terminateSession,
  }
}

