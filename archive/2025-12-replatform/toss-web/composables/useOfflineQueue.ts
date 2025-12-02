/**
 * Offline Queue Composable
 * Manages queueing of sales transactions for offline-first operation
 * Uses IndexedDB for persistent storage with fallback to localStorage
 */

import { v4 as uuidv4 } from 'uuid'

export type QueueOperation = 
  | 'pos.invoice.create'
  | 'pos.sale.hold'
  | 'pos.sale.void'
  | 'pos.sale.return'
  | 'order.create'
  | 'order.update'
  | 'payment.process'

export interface QueueItem {
  id: string
  type: QueueOperation
  payload: any
  createdAt: number
  attempts: number
  lastAttempt?: number
  syncedAt?: number
  error?: string
  status: 'pending' | 'syncing' | 'synced' | 'failed'
}

export interface QueueStats {
  pending: number
  synced: number
  failed: number
  total: number
}

const DB_NAME = 'toss_offline_queue'
const DB_VERSION = 1
const STORE_NAME = 'queue_items'
const MAX_RETRIES = 3
const RETRY_DELAY_MS = 2000

export const useOfflineQueue = () => {
  let db: IDBDatabase | null = null

  /**
   * Initialize IndexedDB
   */
  const initDB = (): Promise<IDBDatabase> => {
    return new Promise((resolve, reject) => {
      if (db) {
        resolve(db)
        return
      }

      const request = indexedDB.open(DB_NAME, DB_VERSION)

      request.onerror = () => reject(request.error)
      
      request.onsuccess = () => {
        db = request.result
        resolve(db)
      }

      request.onupgradeneeded = (event) => {
        const database = (event.target as IDBOpenDBRequest).result
        
        if (!database.objectStoreNames.contains(STORE_NAME)) {
          const objectStore = database.createObjectStore(STORE_NAME, { keyPath: 'id' })
          objectStore.createIndex('status', 'status', { unique: false })
          objectStore.createIndex('type', 'type', { unique: false })
          objectStore.createIndex('createdAt', 'createdAt', { unique: false })
        }
      }
    })
  }

  /**
   * Add item to queue
   */
  const enqueue = async (type: QueueOperation, payload: any): Promise<QueueItem> => {
    const database = await initDB()
    
    const item: QueueItem = {
      id: uuidv4(),
      type,
      payload,
      createdAt: Date.now(),
      attempts: 0,
      status: 'pending'
    }

    return new Promise((resolve, reject) => {
      const transaction = database.transaction([STORE_NAME], 'readwrite')
      const store = transaction.objectStore(STORE_NAME)
      const request = store.add(item)

      request.onsuccess = () => resolve(item)
      request.onerror = () => reject(request.error)
    })
  }

  /**
   * Get all pending items
   */
  const getPending = async (): Promise<QueueItem[]> => {
    const database = await initDB()
    
    return new Promise((resolve, reject) => {
      const transaction = database.transaction([STORE_NAME], 'readonly')
      const store = transaction.objectStore(STORE_NAME)
      const index = store.index('status')
      const request = index.getAll('pending')

      request.onsuccess = () => resolve(request.result || [])
      request.onerror = () => reject(request.error)
    })
  }

  /**
   * Get all items
   */
  const getAll = async (): Promise<QueueItem[]> => {
    const database = await initDB()
    
    return new Promise((resolve, reject) => {
      const transaction = database.transaction([STORE_NAME], 'readonly')
      const store = transaction.objectStore(STORE_NAME)
      const request = store.getAll()

      request.onsuccess = () => resolve(request.result || [])
      request.onerror = () => reject(request.error)
    })
  }

  /**
   * Get single item by ID
   */
  const getItem = async (id: string): Promise<QueueItem | undefined> => {
    const database = await initDB()
    
    return new Promise((resolve, reject) => {
      const transaction = database.transaction([STORE_NAME], 'readonly')
      const store = transaction.objectStore(STORE_NAME)
      const request = store.get(id)

      request.onsuccess = () => resolve(request.result)
      request.onerror = () => reject(request.error)
    })
  }

  /**
   * Update item status
   */
  const updateItem = async (item: QueueItem): Promise<void> => {
    const database = await initDB()
    
    return new Promise((resolve, reject) => {
      const transaction = database.transaction([STORE_NAME], 'readwrite')
      const store = transaction.objectStore(STORE_NAME)
      const request = store.put(item)

      request.onsuccess = () => resolve()
      request.onerror = () => reject(request.error)
    })
  }

  /**
   * Mark item as synced
   */
  const markSynced = async (id: string): Promise<void> => {
    const item = await getItem(id)
    if (!item) return

    item.status = 'synced'
    item.syncedAt = Date.now()
    await updateItem(item)
  }

  /**
   * Mark item as failed
   */
  const markFailed = async (id: string, error: string): Promise<void> => {
    const item = await getItem(id)
    if (!item) return

    item.status = 'failed'
    item.error = error
    item.lastAttempt = Date.now()
    item.attempts += 1
    await updateItem(item)
  }

  /**
   * Delete item from queue
   */
  const deleteItem = async (id: string): Promise<void> => {
    const database = await initDB()
    
    return new Promise((resolve, reject) => {
      const transaction = database.transaction([STORE_NAME], 'readwrite')
      const store = transaction.objectStore(STORE_NAME)
      const request = store.delete(id)

      request.onsuccess = () => resolve()
      request.onerror = () => reject(request.error)
    })
  }

  /**
   * Clear all synced items
   */
  const clearSynced = async (): Promise<number> => {
    const items = await getAll()
    const synced = items.filter(item => item.status === 'synced')
    
    for (const item of synced) {
      await deleteItem(item.id)
    }
    
    return synced.length
  }

  /**
   * Get queue statistics
   */
  const getStats = async (): Promise<QueueStats> => {
    const items = await getAll()
    
    return {
      pending: items.filter(item => item.status === 'pending').length,
      synced: items.filter(item => item.status === 'synced').length,
      failed: items.filter(item => item.status === 'failed').length,
      total: items.length
    }
  }

  /**
   * Process queue by syncing with backend
   */
  const processQueue = async (
    syncFn: (item: QueueItem) => Promise<any>,
    onProgress?: (current: number, total: number) => void
  ): Promise<{ succeeded: number; failed: number }> => {
    const pending = await getPending()
    let succeeded = 0
    let failed = 0

    for (let i = 0; i < pending.length; i++) {
      const item = pending[i]
      
      onProgress?.(i + 1, pending.length)

      try {
        // Update to syncing status
        item.status = 'syncing'
        item.lastAttempt = Date.now()
        await updateItem(item)

        // Execute sync function
        await syncFn(item)

        // Mark as synced
        await markSynced(item.id)
        succeeded++
      } catch (error: any) {
        // Mark as failed
        await markFailed(item.id, error.message || 'Unknown error')
        failed++

        // Retry logic
        if (item.attempts < MAX_RETRIES) {
          // Reset to pending for retry
          item.status = 'pending'
          await updateItem(item)
          
          // Wait before next attempt
          await new Promise(resolve => setTimeout(resolve, RETRY_DELAY_MS))
        }
      }
    }

    return { succeeded, failed }
  }

  /**
   * Check if online
   */
  const isOnline = (): boolean => {
    return navigator.onLine
  }

  /**
   * Auto-sync when coming online
   */
  const enableAutoSync = (syncFn: (item: QueueItem) => Promise<any>) => {
    const handleOnline = async () => {
      console.log('🌐 Connection restored - processing queue...')
      try {
        const result = await processQueue(syncFn)
        console.log(`✅ Queue processed: ${result.succeeded} succeeded, ${result.failed} failed`)
      } catch (error) {
        console.error('❌ Queue processing failed:', error)
      }
    }

    window.addEventListener('online', handleOnline)
    
    // Return cleanup function
    return () => window.removeEventListener('online', handleOnline)
  }

  return {
    enqueue,
    getPending,
    getAll,
    getItem,
    updateItem,
    markSynced,
    markFailed,
    deleteItem,
    clearSynced,
    getStats,
    processQueue,
    isOnline,
    enableAutoSync
  }
}
