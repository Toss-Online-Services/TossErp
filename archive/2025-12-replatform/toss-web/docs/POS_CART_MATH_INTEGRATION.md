// CART MATH INTEGRATION FOR POS
// This file contains code snippets to integrate cart math utilities into pages/sales/pos/index.vue

// ============================================================
// STEP 1: Add imports at the top of the script section
// ============================================================
import { useCartMath, applyStandardVAT, type CartLine, type CartTotals } from '~/composables/useCartMath'
import { useOfflineQueue, type QueueOperation } from '~/composables/useOfflineQueue'
import * as Sentry from '@sentry/nuxt'

// ============================================================
// STEP 2: Initialize composables after other refs
// ============================================================
const cartMath = useCartMath()
const offlineQueue = useOfflineQueue()

// Enable auto-sync when component mounts
onMounted(() => {
  // Existing onMounted code stays here...
  
  // Add auto-sync setup
  const syncFn = async (item: any) => {
    try {
      if (item.type === 'pos.invoice.create') {
        const response = await salesAPI.createSale(item.payload)
        Sentry.addBreadcrumb({
          category: 'pos',
          message: `Synced invoice ${item.payload.invoiceNumber} from offline queue`,
          level: 'info'
        })
        return response
      }
      // Handle other queue types...
    } catch (error) {
      Sentry.captureException(error, {
        tags: { component: 'POS', operation: 'offline_sync' }
      })
      throw error
    }
  }
  
  const cleanup = offlineQueue.enableAutoSync(syncFn)
  
  // Cleanup on unmount
  onUnmounted(() => {
    cleanup()
  })
})

// ============================================================
// STEP 3: Update cart item structure (replace existing cartItems definition)
// ============================================================
// OLD:
// const cartItems = ref<any[]>([])

// NEW - Enhanced with discount and tax fields:
interface PosCartItem {
  id: string | number
  name: string
  price: number // unit price
  quantity: number
  discountPercent?: number
  discountAmount?: number
  taxRate?: number // 15 for 15% VAT
  showDiscountInput?: boolean // UI state
}

const cartItems = ref<PosCartItem[]>([])

// ============================================================
// STEP 4: Convert cart items to CartLine format for calculations
// ============================================================
const cartLines = computed((): CartLine[] => {
  return cartItems.value.map(item => ({
    productId: String(item.id),
    name: item.name,
    quantity: item.quantity,
    unitPrice: item.price,
    discountPercent: item.discountPercent,
    discountAmount: item.discountAmount,
    taxRate: item.taxRate
  }))
})

// ============================================================
// STEP 5: Calculate totals using cartMath (replace existing cartTotal)
// ============================================================
// OLD:
// const cartTotal = computed(() => {
//   return cartItems.value.reduce((total: number, item: any) => total + (item.price * item.quantity), 0)
// })

// NEW - Use cartMath utility:
const cartTotals = computed((): CartTotals => {
  // Apply standard 15% VAT to items without explicit tax rate
  const linesWithVAT = cartMath.applyStandardVAT(cartLines.value)
  return cartMath.calculateCartTotals(linesWithVAT)
})

const cartTotal = computed(() => {
  return cartTotals.value.grandTotal
})

// ============================================================
// STEP 6: Update addToCart to include VAT (replace existing)
// ============================================================
const addToCart = (product: any) => {
  if (product.stock === 0) return

  const existingItem = cartItems.value.find((item: any) => item.id === product.id)
  if (existingItem) {
    existingItem.quantity += 1
  } else {
    cartItems.value.push({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: 1,
      taxRate: 15 // Default 15% VAT for South Africa
    })
  }
  
  // Add Sentry breadcrumb
  Sentry.addBreadcrumb({
    category: 'pos.cart',
    message: `Added ${product.name} to cart`,
    level: 'info',
    data: {
      productId: product.id,
      productName: product.name,
      quantity: existingItem ? existingItem.quantity : 1
    }
  })
}

// ============================================================
// STEP 7: Update quantity with Sentry tracking
// ============================================================
const updateQuantity = (productId: number | string, newQuantity: number) => {
  const item = cartItems.value.find((item: any) => item.id === productId)
  if (item) {
    const oldQty = item.quantity
    item.quantity = Math.max(1, newQuantity)
    
    Sentry.addBreadcrumb({
      category: 'pos.cart',
      message: `Updated quantity for ${item.name}`,
      level: 'info',
      data: {
        productId,
        oldQuantity: oldQty,
        newQuantity: item.quantity
      }
    })
  }
}

// ============================================================
// STEP 8: Update removeFromCart with Sentry tracking
// ============================================================
const removeFromCart = (productId: number | string) => {
  const item = cartItems.value.find((i: any) => i.id === productId)
  if (item) {
    Sentry.addBreadcrumb({
      category: 'pos.cart',
      message: `Removed ${item.name} from cart`,
      level: 'info',
      data: {
        productId,
        productName: item.name
      }
    })
  }
  cartItems.value = cartItems.value.filter((item: any) => item.id !== productId)
}

// ============================================================
// STEP 9: Update clearCart with Sentry tracking
// ============================================================
const clearCart = () => {
  const itemCount = cartItems.value.length
  cartItems.value = []
  
  Sentry.addBreadcrumb({
    category: 'pos.cart',
    message: 'Cleared cart',
    level: 'info',
    data: { itemCount }
  })
}

// ============================================================
// STEP 10: Integrate offline queue into processPayment
// ============================================================
const processPayment = async () => {
  if (cartItems.value.length === 0) {
    toast.add({ title: 'Error', description: 'Cart is empty', color: 'red' })
    return
  }

  try {
    isProcessing.value = true
    
    // Generate invoice data
    const invoiceData = {
      invoiceNumber: `INV-${Date.now()}`,
      shopId: 1, // Replace with actual shop ID
      customerId: selectedCustomer.value?.id || null,
      customerName: selectedCustomer.value?.name || 'Walk-in Customer',
      paymentMethod: selectedPaymentMethod.value,
      lines: cartLines.value.map(line => ({
        productId: line.productId,
        productName: line.name,
        quantity: line.quantity,
        unitPrice: line.unitPrice,
        discountPercent: line.discountPercent,
        discountAmount: line.discountAmount,
        taxRate: line.taxRate,
        lineTotal: cartMath.calculateLineTotal(line)
      })),
      subtotal: cartTotals.value.subtotal,
      discountTotal: cartTotals.value.discountTotal,
      taxTotal: cartTotals.value.taxTotal,
      grandTotal: cartTotals.value.grandTotal,
      createdAt: new Date().toISOString()
    }
    
    // Check if online
    if (navigator.onLine) {
      // Try direct API call
      try {
        const response = await salesAPI.createSale(invoiceData)
        
        toast.add({
          title: 'Success',
          description: `Sale completed: ${invoiceData.invoiceNumber}`,
          color: 'green'
        })
        
        Sentry.addBreadcrumb({
          category: 'pos.payment',
          message: `Payment processed: ${invoiceData.invoiceNumber}`,
          level: 'info',
          data: {
            amount: cartTotals.value.grandTotal,
            method: selectedPaymentMethod.value,
            online: true
          }
        })
        
        clearCart()
        selectedCustomer.value = null
        selectedPaymentMethod.value = 'cash'
        
      } catch (apiError) {
        // API failed but we're online - enqueue for retry
        console.warn('API failed but online - enqueueing for retry:', apiError)
        await offlineQueue.enqueue('pos.invoice.create', invoiceData)
        
        toast.add({
          title: 'Queued',
          description: 'Sale saved for sync. Will retry automatically.',
          color: 'orange'
        })
        
        Sentry.captureException(apiError, {
          tags: { component: 'POS', operation: 'payment_enqueued' }
        })
        
        clearCart()
      }
    } else {
      // Offline - enqueue
      await offlineQueue.enqueue('pos.invoice.create', invoiceData)
      
      toast.add({
        title: 'Offline Mode',
        description: 'Sale saved. Will sync when online.',
        color: 'yellow'
      })
      
      Sentry.addBreadcrumb({
        category: 'pos.payment',
        message: `Payment queued (offline): ${invoiceData.invoiceNumber}`,
        level: 'warning',
        data: {
          amount: cartTotals.value.grandTotal,
          method: selectedPaymentMethod.value
        }
      })
      
      clearCart()
      selectedCustomer.value = null
      selectedPaymentMethod.value = 'cash'
    }
    
  } catch (error) {
    console.error('Payment error:', error)
    toast.add({
      title: 'Error',
      description: 'Failed to process payment',
      color: 'red'
    })
    Sentry.captureException(error, {
      tags: { component: 'POS', operation: 'payment' }
    })
  } finally {
    isProcessing.value = false
  }
}

// ============================================================
// STEP 11: Add queue status indicator (add this computed)
// ============================================================
const queueStats = ref({ pending: 0, synced: 0, failed: 0, total: 0 })

// Update stats periodically
onMounted(() => {
  const updateStats = async () => {
    queueStats.value = await offlineQueue.getStats()
  }
  
  updateStats()
  const interval = setInterval(updateStats, 5000) // Every 5 seconds
  
  onUnmounted(() => {
    clearInterval(interval)
  })
})
