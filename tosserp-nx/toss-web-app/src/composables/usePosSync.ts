import { ref, computed } from 'vue'
import { useIndexedDB } from './useIndexedDB'
import { useNetworkStatus } from './useNetworkStatus'

export const usePosSync = () => {
  const { isOnline } = useNetworkStatus()
  const {
    queueSale,
    getQueuedSales,
    markSaleSynced,
    removeQueuedSale
  } = useIndexedDB()

  const isSyncing = ref(false)
  const syncError = ref<string | null>(null)

  const generateIdempotencyKey = () => {
    return `pos-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
  }

  const syncQueuedSales = async (apiBaseUrl: string) => {
    if (!isOnline.value || isSyncing.value) return

    isSyncing.value = true
    syncError.value = null

    try {
      const unsyncedSales = await getQueuedSales(false)

      for (const sale of unsyncedSales) {
        try {
          const response = await fetch(`${apiBaseUrl}/api/sales/pos/checkout`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              // Add auth header if needed
              // 'Authorization': `Bearer ${token}`
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
        } catch (error) {
          console.error(`Failed to sync sale ${sale.id}:`, error)
          // Don't remove on error - will retry later
        }
      }
    } catch (error) {
      syncError.value = error instanceof Error ? error.message : 'Sync failed'
      console.error('Sync error:', error)
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

  const pendingSalesCount = computed(async () => {
    const unsynced = await getQueuedSales(false)
    return unsynced.length
  })

  return {
    isSyncing,
    syncError,
    syncQueuedSales,
    queueSaleForSync,
    pendingSalesCount,
    generateIdempotencyKey
  }
}

