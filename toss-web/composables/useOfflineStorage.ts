/**
 * Offline Storage Composable
 * 
 * Provides offline data persistence using IndexedDB
 * Automatically syncs data when connection is restored
 */

import { ref, computed } from 'vue'

interface OfflineQueueItem {
  id: string
  timestamp: number
  method: string
  url: string
  data: any
  retries: number
}

const DB_NAME = 'toss-erp-offline'
const DB_VERSION = 1
const QUEUE_STORE = 'sync-queue'
const CACHE_STORE = 'offline-cache'

export function useOfflineStorage() {
  const isOnline = ref(navigator.onLine)
  const syncQueue = ref<OfflineQueueItem[]>([])
  const isSyncing = ref(false)
  
  let db: IDBDatabase | null = null
  
  // Initialize IndexedDB
  async function initDB(): Promise<IDBDatabase> {
    if (db) return db
    
    return new Promise((resolve, reject) => {
      const request = indexedDB.open(DB_NAME, DB_VERSION)
      
      request.onerror = () => reject(request.error)
      request.onsuccess = () => {
        db = request.result
        resolve(db)
      }
      
      request.onupgradeneeded = (event: any) => {
        const database = event.target.result
        
        // Create queue store
        if (!database.objectStoreNames.contains(QUEUE_STORE)) {
          database.createObjectStore(QUEUE_STORE, { keyPath: 'id' })
        }
        
        // Create cache store
        if (!database.objectStoreNames.contains(CACHE_STORE)) {
          const cacheStore = database.createObjectStore(CACHE_STORE, { keyPath: 'key' })
          cacheStore.createIndex('timestamp', 'timestamp', { unique: false })
        }
      }
    })
  }
  
  // Add item to sync queue
  async function addToQueue(method: string, url: string, data: any) {
    const database = await initDB()
    const transaction = database.transaction([QUEUE_STORE], 'readwrite')
    const store = transaction.objectStore(QUEUE_STORE)
    
    const item: OfflineQueueItem = {
      id: `${Date.now()}-${Math.random()}`,
      timestamp: Date.now(),
      method,
      url,
      data,
      retries: 0
    }
    
    store.add(item)
    syncQueue.value.push(item)
    
    return new Promise((resolve, reject) => {
      transaction.oncomplete = () => resolve(item)
      transaction.onerror = () => reject(transaction.error)
    })
  }
  
  // Load sync queue from IndexedDB
  async function loadQueue() {
    const database = await initDB()
    const transaction = database.transaction([QUEUE_STORE], 'readonly')
    const store = transaction.objectStore(QUEUE_STORE)
    const request = store.getAll()
    
    return new Promise<OfflineQueueItem[]>((resolve, reject) => {
      request.onsuccess = () => {
        syncQueue.value = request.result
        resolve(request.result)
      }
      request.onerror = () => reject(request.error)
    })
  }
  
  // Sync queued items when online
  async function syncQueuedItems() {
    if (!isOnline.value || isSyncing.value || syncQueue.value.length === 0) {
      return
    }
    
    isSyncing.value = true
    const database = await initDB()
    
    for (const item of [...syncQueue.value]) {
      try {
        const response = await fetch(item.url, {
          method: item.method,
          headers: {
            'Content-Type': 'application/json',
          },
          body: item.data ? JSON.stringify(item.data) : undefined
        })
        
        if (response.ok) {
          // Remove from queue on success
          const transaction = database.transaction([QUEUE_STORE], 'readwrite')
          const store = transaction.objectStore(QUEUE_STORE)
          store.delete(item.id)
          
          syncQueue.value = syncQueue.value.filter(q => q.id !== item.id)
        } else {
          // Increment retries
          item.retries++
          if (item.retries > 3) {
            // Remove after 3 failed attempts
            const transaction = database.transaction([QUEUE_STORE], 'readwrite')
            const store = transaction.objectStore(QUEUE_STORE)
            store.delete(item.id)
            syncQueue.value = syncQueue.value.filter(q => q.id !== item.id)
          }
        }
      } catch (error) {
        console.error('Sync error:', error)
        item.retries++
      }
    }
    
    isSyncing.value = false
  }
  
  // Cache data for offline access
  async function cacheData(key: string, data: any, ttl: number = 3600000) {
    const database = await initDB()
    const transaction = database.transaction([CACHE_STORE], 'readwrite')
    const store = transaction.objectStore(CACHE_STORE)
    
    const item = {
      key,
      data,
      timestamp: Date.now(),
      ttl
    }
    
    store.put(item)
    
    return new Promise((resolve, reject) => {
      transaction.oncomplete = () => resolve(item)
      transaction.onerror = () => reject(transaction.error)
    })
  }
  
  // Get cached data
  async function getCachedData(key: string) {
    const database = await initDB()
    const transaction = database.transaction([CACHE_STORE], 'readonly')
    const store = transaction.objectStore(CACHE_STORE)
    const request = store.get(key)
    
    return new Promise<any>((resolve, reject) => {
      request.onsuccess = () => {
        const item = request.result
        if (!item) {
          resolve(null)
          return
        }
        
        // Check if expired
        if (Date.now() - item.timestamp > item.ttl) {
          resolve(null)
          return
        }
        
        resolve(item.data)
      }
      request.onerror = () => reject(request.error)
    })
  }
  
  // Clear expired cache
  async function clearExpiredCache() {
    const database = await initDB()
    const transaction = database.transaction([CACHE_STORE], 'readwrite')
    const store = transaction.objectStore(CACHE_STORE)
    const index = store.index('timestamp')
    const request = index.openCursor()
    
    request.onsuccess = (event: any) => {
      const cursor = event.target.result
      if (cursor) {
        const item = cursor.value
        if (Date.now() - item.timestamp > item.ttl) {
          cursor.delete()
        }
        cursor.continue()
      }
    }
  }
  
  // Listen for online/offline events
  if (process.client) {
    window.addEventListener('online', () => {
      isOnline.value = true
      syncQueuedItems()
    })
    
    window.addEventListener('offline', () => {
      isOnline.value = false
    })
    
    // Load queue on mount
    initDB().then(() => {
      loadQueue()
      clearExpiredCache()
      if (isOnline.value) {
        syncQueuedItems()
      }
    })
  }
  
  const hasPendingSync = computed(() => syncQueue.value.length > 0)
  
  return {
    isOnline,
    hasPendingSync,
    isSyncing,
    syncQueue,
    addToQueue,
    syncQueue,
    cacheData,
    getCachedData,
    clearExpiredCache
  }
}

