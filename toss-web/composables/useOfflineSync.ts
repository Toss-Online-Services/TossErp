import { ref, onMounted, computed } from 'vue'

interface QueuedOperation {
  id: string
  type: 'sale' | 'stock_adjustment' | 'purchase_receipt' | 'payment'
  data: any
  timestamp: Date
  retries: number
  status: 'pending' | 'syncing' | 'failed' | 'completed'
}

export function useOfflineSync() {
  const isOnline = ref(typeof window !== 'undefined' ? navigator.onLine : true)
  const syncQueue = ref<QueuedOperation[]>([])
  const isSyncing = ref(false)

  // Update online status
  function updateOnlineStatus() {
    if (typeof window === 'undefined') return
    isOnline.value = navigator.onLine
    if (isOnline.value) {
      syncPendingOperations()
    }
  }

  // Add operation to queue
  function queueOperation(type: QueuedOperation['type'], data: any): string {
    const operation: QueuedOperation = {
      id: `${type}_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
      type,
      data,
      timestamp: new Date(),
      retries: 0,
      status: 'pending'
    }

    syncQueue.value.push(operation)
    saveQueueToStorage()

    // Try to sync immediately if online
    if (isOnline.value) {
      syncPendingOperations()
    }

    return operation.id
  }

  // Save queue to localStorage
  function saveQueueToStorage() {
    if (typeof window === 'undefined') return
    try {
      localStorage.setItem('toss_sync_queue', JSON.stringify(syncQueue.value))
    } catch (error) {
      console.error('Failed to save sync queue:', error)
    }
  }

  // Load queue from localStorage
  function loadQueueFromStorage() {
    if (typeof window === 'undefined') return
    try {
      const stored = localStorage.getItem('toss_sync_queue')
      if (stored) {
        syncQueue.value = JSON.parse(stored)
      }
    } catch (error) {
      console.error('Failed to load sync queue:', error)
    }
  }

  // Sync pending operations
  async function syncPendingOperations() {
    if (isSyncing.value || !isOnline.value) return

    isSyncing.value = true
    const pendingOps = syncQueue.value.filter(op => op.status === 'pending' || op.status === 'failed')

    for (const operation of pendingOps) {
      try {
        operation.status = 'syncing'
        await syncOperation(operation)
        operation.status = 'completed'
      } catch (error) {
        console.error(`Failed to sync operation ${operation.id}:`, error)
        operation.status = 'failed'
        operation.retries++
      }
    }

    // Remove completed operations
    syncQueue.value = syncQueue.value.filter(op => op.status !== 'completed')
    saveQueueToStorage()
    isSyncing.value = false
  }

  // Sync individual operation
  async function syncOperation(operation: QueuedOperation) {
    const endpoints: Record<QueuedOperation['type'], string> = {
      sale: '/pos/sales',
      stock_adjustment: '/stock/adjustments',
      purchase_receipt: '/procurement/receipts',
      payment: '/accounting/payments'
    }

    const endpoint = endpoints[operation.type]
    if (!endpoint) {
      throw new Error(`Unknown operation type: ${operation.type}`)
    }

    // TODO: Replace with actual API call
    await new Promise(resolve => setTimeout(resolve, 500))
    console.log(`Synced ${operation.type}:`, operation.data)
  }

  // Get pending count
  const pendingCount = computed(() => {
    return syncQueue.value.filter(op => op.status === 'pending' || op.status === 'failed').length
  })

  // Initialize
  onMounted(() => {
    if (typeof window === 'undefined') return
    loadQueueFromStorage()
    window.addEventListener('online', updateOnlineStatus)
    window.addEventListener('offline', updateOnlineStatus)

    // Try to sync on mount if online
    if (isOnline.value) {
      syncPendingOperations()
    }
  })

  return {
    isOnline,
    syncQueue,
    isSyncing,
    pendingCount,
    queueOperation,
    syncPendingOperations
  }
}

