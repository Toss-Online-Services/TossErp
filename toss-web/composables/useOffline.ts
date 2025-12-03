import { ref, onMounted, onUnmounted } from 'vue'
import { useOnline } from '@vueuse/core'

export interface QueuedOperation {
  id: string
  type: string
  data: any
  timestamp: number
  retries: number
}

export const useOffline = () => {
  const isOnline = useOnline()
  const queue = ref<QueuedOperation[]>([])
  const isSyncing = ref(false)

  // Load queue from localStorage
  const loadQueue = () => {
    try {
      const stored = localStorage.getItem('toss_offline_queue')
      if (stored) {
        queue.value = JSON.parse(stored)
      }
    } catch (error) {
      console.error('Failed to load offline queue:', error)
    }
  }

  // Save queue to localStorage
  const saveQueue = () => {
    try {
      localStorage.setItem('toss_offline_queue', JSON.stringify(queue.value))
    } catch (error) {
      console.error('Failed to save offline queue:', error)
    }
  }

  // Add operation to queue
  const addToQueue = (type: string, data: any): string => {
    const id = `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
    const operation: QueuedOperation = {
      id,
      type,
      data,
      timestamp: Date.now(),
      retries: 0
    }
    
    queue.value.push(operation)
    saveQueue()
    
    // Try to sync immediately if online
    if (isOnline.value) {
      syncQueue()
    }
    
    return id
  }

  // Remove operation from queue
  const removeFromQueue = (id: string) => {
    queue.value = queue.value.filter(op => op.id !== id)
    saveQueue()
  }

  // Sync queue with server
  const syncQueue = async () => {
    if (!isOnline.value || isSyncing.value || queue.value.length === 0) {
      return
    }

    isSyncing.value = true

    try {
      // Process queue items one by one
      for (const operation of [...queue.value]) {
        try {
          // Send to API based on operation type
          const response = await $fetch(`/api/${operation.type}`, {
            method: 'POST',
            body: operation.data
          })

          // Remove from queue on success
          removeFromQueue(operation.id)
        } catch (error) {
          console.error(`Failed to sync operation ${operation.id}:`, error)
          
          // Increment retry count
          const index = queue.value.findIndex(op => op.id === operation.id)
          if (index !== -1) {
            queue.value[index].retries++
            
            // Remove if too many retries
            if (queue.value[index].retries > 3) {
              console.error(`Removing operation ${operation.id} after 3 failed retries`)
              removeFromQueue(operation.id)
            } else {
              saveQueue()
            }
          }
        }
      }
    } finally {
      isSyncing.value = false
    }
  }

  // Handle online/offline events
  const handleOnline = () => {
    console.log('Connection restored, syncing queue...')
    syncQueue()
  }

  const handleOffline = () => {
    console.log('Connection lost, operations will be queued')
  }

  // Setup event listeners
  onMounted(() => {
    loadQueue()
    
    if (isOnline.value && queue.value.length > 0) {
      syncQueue()
    }
  })

  // Watch for online status changes
  watch(isOnline, (online) => {
    if (online) {
      handleOnline()
    } else {
      handleOffline()
    }
  })

  return {
    isOnline,
    queue,
    isSyncing,
    addToQueue,
    removeFromQueue,
    syncQueue
  }
}

