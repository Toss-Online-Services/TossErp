import { onMounted, onUnmounted, ref } from 'vue'

/**
 * Lightweight network status tracker that works in SSR-friendly contexts.
 * Returns a reactive `isOnline` flag and timestamp of the last change so the UI
 * can show "syncing" type helpers for low-connectivity environments.
 */
export const useNetworkStatus = () => {
  const hasWindow = typeof window !== 'undefined'
  const initialStatus =
    typeof navigator !== 'undefined' && typeof navigator.onLine === 'boolean'
      ? navigator.onLine
      : true

  const isOnline = ref(initialStatus)
  const lastChangedAt = ref<Date | null>(null)

  const updateStatus = () => {
    if (!hasWindow) {
      return
    }
    isOnline.value = navigator.onLine
    lastChangedAt.value = new Date()
  }

  onMounted(() => {
    if (!hasWindow) {
      return
    }
    window.addEventListener('online', updateStatus)
    window.addEventListener('offline', updateStatus)
  })

  onUnmounted(() => {
    if (!hasWindow) {
      return
    }
    window.removeEventListener('online', updateStatus)
    window.removeEventListener('offline', updateStatus)
  })

  return {
    isOnline,
    lastChangedAt
  }
}




