export default defineNuxtPlugin(() => {
  // Service Worker registration for offline support
  if (process.client && 'serviceWorker' in navigator) {
    window.addEventListener('load', () => {
      navigator.serviceWorker
        .register('/sw.js')
        .then(registration => {
          console.log('Service Worker registered:', registration)
        })
        .catch(error => {
          console.error('Service Worker registration failed:', error)
        })
    })
  }

  // Online/Offline detection
  const isOnline = ref(true)
  const syncQueue = ref<any[]>([])

  if (process.client) {
    isOnline.value = navigator.onLine

    window.addEventListener('online', () => {
      isOnline.value = true
      console.log('Connection restored - syncing queued operations...')
      syncPendingOperations()
    })

    window.addEventListener('offline', () => {
      isOnline.value = false
      console.log('Connection lost - offline mode activated')
    })
  }

  // Sync pending operations when back online
  async function syncPendingOperations() {
    if (syncQueue.value.length === 0) return

    const { $api } = useNuxtApp()
    let successCount = 0
    let failCount = 0

    for (const operation of syncQueue.value) {
      try {
        await $api.post('/sync/operation', operation)
        successCount++
        
        // Remove from queue
        syncQueue.value = syncQueue.value.filter(op => op.id !== operation.id)
        
        // Remove from IndexedDB
        if ('indexedDB' in window) {
          const db = await openDB()
          await db.delete('sync_queue', operation.id)
        }
      } catch (error) {
        console.error('Failed to sync operation:', operation, error)
        failCount++
      }
    }

    console.log(`Sync complete: ${successCount} successful, ${failCount} failed`)
  }

  // Queue an operation for later sync
  function queueOperation(operation: any) {
    const op = {
      ...operation,
      id: `${Date.now()}-${Math.random()}`,
      queued_at: new Date().toISOString()
    }

    syncQueue.value.push(op)

    // Persist to IndexedDB
    if ('indexedDB' in window) {
      openDB().then(db => {
        db.add('sync_queue', op)
      })
    }

    return op
  }

  // Open IndexedDB for offline storage
  async function openDB(): Promise<any> {
    return new Promise((resolve, reject) => {
      const request = indexedDB.open('TossOfflineDB', 1)

      request.onerror = () => reject(request.error)
      request.onsuccess = () => resolve(request.result)

      request.onupgradeneeded = (event) => {
        const db = (event.target as any).result
        
        if (!db.objectStoreNames.contains('sync_queue')) {
          const store = db.createObjectStore('sync_queue', { keyPath: 'id' })
          store.createIndex('queued_at', 'queued_at', { unique: false })
        }

        if (!db.objectStoreNames.contains('cached_data')) {
          db.createObjectStore('cached_data', { keyPath: 'key' })
        }
      }
    })
  }

  // Provide offline functionality globally
  return {
    provide: {
      offline: {
        isOnline,
        syncQueue,
        queueOperation,
        syncPendingOperations
      }
    }
  }
})

