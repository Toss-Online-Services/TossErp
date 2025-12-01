import { ref } from 'vue'
import { useIndexedDB } from './useIndexedDB'
import { useNetworkStatus } from './useNetworkStatus'
import { useToast } from './useToast'

export interface QueuedOperation<T = any> {
  id: string
  type: string
  endpoint: string
  method: 'POST' | 'PUT' | 'PATCH' | 'DELETE'
  payload: T
  createdAt: number
  synced: boolean
  retryCount: number
  maxRetries?: number
}

export const useOfflineStore = () => {
  const { isOnline } = useNetworkStatus()
  const { success, error: showError } = useToast()
  const { initDB } = useIndexedDB()

  const queuedOperations = ref<QueuedOperation[]>([])
  const isSyncing = ref(false)

  const init = async () => {
    await initDB()
    await loadQueuedOperations()
  }

  const loadQueuedOperations = async () => {
    const db = await initDB()
    if (!db.objectStoreNames.contains('operations')) {
      queuedOperations.value = []
      return
    }
    const tx = db.transaction('operations', 'readonly')
    const store = tx.objectStore('operations')
    const index = store.index('by-synced')
    const unsynced = await index.getAll(false)
    queuedOperations.value = unsynced
  }

  const queueOperation = async <T>(
    type: string,
    endpoint: string,
    method: 'POST' | 'PUT' | 'PATCH' | 'DELETE',
    payload: T,
    maxRetries = 3
  ): Promise<string> => {
    const id = `op-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
    const operation: QueuedOperation<T> = {
      id,
      type,
      endpoint,
      method,
      payload,
      createdAt: Date.now(),
      synced: false,
      retryCount: 0,
      maxRetries
    }

    const db = await initDB()
    if (!db.objectStoreNames.contains('operations')) {
      // Create store on the fly (should be in upgrade, but handle gracefully)
      console.warn('Operations store not found, operation will be lost')
      return id
    }

    await db.add('operations', operation)
    await loadQueuedOperations()

    // Try to sync immediately if online
    if (isOnline.value) {
      await syncQueuedOperations()
    }

    return id
  }

  const syncQueuedOperations = async (showNotifications = true) => {
    if (!isOnline.value || isSyncing.value) return

    isSyncing.value = true

    try {
      const unsynced = queuedOperations.value.filter(op => !op.synced && op.retryCount < (op.maxRetries || 3))
      
      if (unsynced.length === 0) {
        isSyncing.value = false
        return
      }

      const apiBaseUrl = useRuntimeConfig().public.apiBase || 'http://localhost:5000'
      const token = localStorage.getItem('auth_token')

      let successCount = 0
      let failureCount = 0

      for (const operation of unsynced) {
        try {
          const response = await fetch(`${apiBaseUrl}${operation.endpoint}`, {
            method: operation.method,
            headers: {
              'Content-Type': 'application/json',
              ...(token ? { Authorization: `Bearer ${token}` } : {})
            },
            body: operation.method !== 'DELETE' ? JSON.stringify(operation.payload) : undefined
          })

          if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`)
          }

          // Mark as synced
          const db = await initDB()
          const tx = db.transaction('operations', 'readwrite')
          const store = tx.objectStore('operations')
          const op = await store.get(operation.id)
          if (op) {
            op.synced = true
            await store.put(op)
          }
          await tx.done

          successCount++
        } catch (error) {
          console.error(`Failed to sync operation ${operation.id}:`, error)
          
          // Increment retry count
          const db = await initDB()
          const tx = db.transaction('operations', 'readwrite')
          const store = tx.objectStore('operations')
          const op = await store.get(operation.id)
          if (op) {
            op.retryCount = (op.retryCount || 0) + 1
            await store.put(op)
          }
          await tx.done

          failureCount++
        }
      }

      await loadQueuedOperations()

      if (showNotifications) {
        if (successCount > 0) {
          success(`${successCount} operation${successCount !== 1 ? 's' : ''} synced`)
        }
        if (failureCount > 0) {
          showError(`${failureCount} operation${failureCount !== 1 ? 's' : ''} failed to sync`)
        }
      }
    } catch (error) {
      console.error('Sync error:', error)
      if (showNotifications) {
        showError('Failed to sync operations')
      }
    } finally {
      isSyncing.value = false
    }
  }

  // Watch for online status
  if (typeof window !== 'undefined') {
    watch(isOnline, async (online) => {
      if (online && !isSyncing.value) {
        await syncQueuedOperations(true)
      }
    })
  }

  return {
    queuedOperations,
    isSyncing,
    init,
    queueOperation,
    syncQueuedOperations,
    loadQueuedOperations
  }
}

