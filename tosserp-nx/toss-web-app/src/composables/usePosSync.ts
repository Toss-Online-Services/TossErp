import { ref, computed, watch } from 'vue'
import { useIndexedDB } from './useIndexedDB'
import { useNetworkStatus } from './useNetworkStatus'
import { useToast } from './useToast'

export const usePosSync = () => {
  const { isOnline } = useNetworkStatus()
  const { success, error: showError } = useToast()
  const {
    queueSale,
    getQueuedSales,
    markSaleSynced,
    removeQueuedSale
  } = useIndexedDB()

  const isSyncing = ref(false)
  const syncError = ref<string | null>(null)
  const lastSyncAt = ref<Date | null>(null)

  const generateIdempotencyKey = () => {
    return `pos-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
  }

  const syncQueuedSales = async (apiBaseUrl: string, showNotifications = true) => {
    if (!isOnline.value || isSyncing.value) return

    isSyncing.value = true
    syncError.value = null

    try {
      const unsyncedSales = await getQueuedSales(false)
      
      if (unsyncedSales.length === 0) {
        isSyncing.value = false
        return
      }

      let successCount = 0
      let failureCount = 0

      for (const sale of unsyncedSales) {
        try {
          const token = localStorage.getItem('auth_token')
          const response = await fetch(`${apiBaseUrl}/api/sales/pos/checkout`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              ...(token ? { Authorization: `Bearer ${token}` } : {})
            },
            body: JSON.stringify({
              shopId: sale.shopId,
              customerId: sale.customerId,
              paymentMethod: sale.paymentMethod,
              paymentReference: sale.paymentReference || sale.id,
              notes: sale.notes,
              items: sale.items,
              idempotencyKey: sale.id
            })
          })

          if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`)
          }

          const result = await response.json()
          
          // Mark as synced
          await markSaleSynced(sale.id, result.saleId)
          successCount++
        } catch (error) {
          console.error(`Failed to sync sale ${sale.id}:`, error)
          failureCount++
          // Don't remove on error - will retry later
        }
      }

      lastSyncAt.value = new Date()

      if (showNotifications) {
        if (successCount > 0) {
          success(`${successCount} sale${successCount !== 1 ? 's' : ''} synced successfully`)
        }
        if (failureCount > 0) {
          showError(`${failureCount} sale${failureCount !== 1 ? 's' : ''} failed to sync. Will retry later.`)
        }
      }
    } catch (error) {
      syncError.value = error instanceof Error ? error.message : 'Sync failed'
      console.error('Sync error:', error)
      if (showNotifications) {
        showError('Failed to sync queued sales. Will retry when online.')
      }
    } finally {
      isSyncing.value = false
    }
  }

  const queueSaleForSync = async (
    shopId: number,
    items: Array<{ productId: number; quantity: number; unitPrice: number }>,
    paymentMethod: 'Cash' | 'Card' | 'Mobile',
    customerId?: number,
    paymentReference?: string,
    notes?: string
  ) => {
    const idempotencyKey = generateIdempotencyKey()
    const total = items.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0)

    const saleId = await queueSale({
      id: idempotencyKey,
      shopId,
      customerId,
      paymentMethod,
      paymentReference: paymentReference || idempotencyKey,
      notes,
      items,
      total
    })

    // Try to sync immediately if online
    if (isOnline.value) {
      // Get API base URL from config or environment
      const apiBaseUrl = useRuntimeConfig().public.apiBase || 'http://localhost:5000'
      await syncQueuedSales(apiBaseUrl)
    }

    return saleId
  }

  const pendingSalesCount = ref(0)

  const updatePendingCount = async () => {
    const unsynced = await getQueuedSales(false)
    pendingSalesCount.value = unsynced.length
  }

  // Initialize pending count
  if (typeof window !== 'undefined') {
    updatePendingCount()
    
    // Watch for online status to trigger sync
    watch(isOnline, async (online) => {
      if (online && !isSyncing.value) {
        const config = useRuntimeConfig()
        const apiBaseUrl = config.public.apiBase || 'http://localhost:5000'
        await syncQueuedSales(apiBaseUrl, true)
        await updatePendingCount()
      }
    })

    // Auto-sync periodically when online
    setInterval(async () => {
      if (isOnline.value && !isSyncing.value) {
        const config = useRuntimeConfig()
        const apiBaseUrl = config.public.apiBase || 'http://localhost:5000'
        await syncQueuedSales(apiBaseUrl, false) // Silent sync
        await updatePendingCount()
      }
    }, 30000) // Every 30 seconds
  }

  return {
    isSyncing,
    syncError,
    lastSyncAt,
    pendingSalesCount,
    syncQueuedSales,
    queueSaleForSync,
    updatePendingCount,
    generateIdempotencyKey
  }
}

