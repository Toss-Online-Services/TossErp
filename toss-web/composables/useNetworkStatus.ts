import { ref, onMounted, onUnmounted } from 'vue'

export const useNetworkStatus = () => {
  // Default to true for SSR, will be updated on client mount
  const isOnline = ref(true)
  const wasOffline = ref(false)

  const updateOnlineStatus = () => {
    if (typeof navigator !== 'undefined') {
      isOnline.value = navigator.onLine
      if (!isOnline.value) {
        wasOffline.value = true
      }
    }
  }

  onMounted(() => {
    if (typeof window !== 'undefined') {
      window.addEventListener('online', updateOnlineStatus)
      window.addEventListener('offline', updateOnlineStatus)
      isOnline.value = navigator.onLine
    }
  })

  onUnmounted(() => {
    if (typeof window !== 'undefined') {
      window.removeEventListener('online', updateOnlineStatus)
      window.removeEventListener('offline', updateOnlineStatus)
    }
  })

  return {
    isOnline,
    wasOffline
  }
}


