import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useOfflineSync } from '~/composables/useOfflineSync'
import { useSalesApi } from '~/composables/useSalesApi'

export interface CartItem {
  id: string
  itemId: string
  name: string
  code: string
  price: number
  quantity: number
  discount: number
  tax: number
  total: number
}

export interface PaymentMethod {
  type: 'cash' | 'card' | 'mobile' | 'credit'
  amount: number
}

export interface Sale {
  id: string
  invoiceNumber: string
  customerId?: string
  customerName?: string
  items: CartItem[]
  subtotal: number
  discount: number
  tax: number
  total: number
  payments: PaymentMethod[]
  change: number
  status: 'draft' | 'completed' | 'pending_sync'
  createdAt: Date
  createdBy: string
}

export const usePosStore = defineStore('pos', () => {
  const { queueOperation } = useOfflineSync()
  const salesApi = useSalesApi()

  // State
  const cart = ref<CartItem[]>([])
  const currentSale = ref<Sale | null>(null)
  const recentSales = ref<Sale[]>([])
  const loading = ref(false)
  const cashDrawerOpen = ref(false)

  // Computed
  const cartSubtotal = computed(() => {
    return cart.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
  })

  const cartDiscount = computed(() => {
    return cart.value.reduce((sum, item) => sum + item.discount, 0)
  })

  const cartTax = computed(() => {
    const subtotalAfterDiscount = cartSubtotal.value - cartDiscount.value
    return subtotalAfterDiscount * 0.15 // 15% VAT
  })

  const cartTotal = computed(() => {
    return cartSubtotal.value - cartDiscount.value + cartTax.value
  })

  const cartItemCount = computed(() => {
    return cart.value.reduce((sum, item) => sum + item.quantity, 0)
  })

  // Actions
  function addToCart(item: any) {
    const existingItem = cart.value.find(cartItem => cartItem.itemId === item.id)

    if (existingItem) {
      existingItem.quantity++
      existingItem.total = existingItem.price * existingItem.quantity - existingItem.discount
    } else {
      const cartItem: CartItem = {
        id: `cart_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
        itemId: item.id,
        name: item.name,
        code: item.code,
        price: item.sellingPrice,
        quantity: 1,
        discount: 0,
        tax: item.sellingPrice * 0.15,
        total: item.sellingPrice
      }
      cart.value.push(cartItem)
    }
  }

  function removeFromCart(cartItemId: string) {
    const index = cart.value.findIndex(item => item.id === cartItemId)
    if (index !== -1) {
      cart.value.splice(index, 1)
    }
  }

  function updateQuantity(cartItemId: string, quantity: number) {
    const item = cart.value.find(cartItem => cartItem.id === cartItemId)
    if (item && quantity > 0) {
      item.quantity = quantity
      item.total = item.price * quantity - item.discount
    }
  }

  function updateDiscount(cartItemId: string, discount: number) {
    const item = cart.value.find(cartItem => cartItem.id === cartItemId)
    if (item) {
      item.discount = discount
      item.total = item.price * item.quantity - discount
    }
  }

  function clearCart() {
    cart.value = []
  }

  async function completeSale(payments: PaymentMethod[], customerId?: string, customerName?: string) {
    if (cart.value.length === 0) {
      throw new Error('Cart is empty')
    }

    const totalPaid = payments.reduce((sum, payment) => sum + payment.amount, 0)
    const change = totalPaid - cartTotal.value

    if (totalPaid < cartTotal.value) {
      throw new Error('Insufficient payment')
    }

    try {
      // Get ShopId - for now default to 1, in production this should come from store context
      const shopId = 1 // TODO: Get from store context or user's default shop
      
      // Determine primary payment method (use the first payment method with amount > 0)
      const primaryPayment = payments.find(p => p.amount > 0) ?? payments[0]
      const paymentType = primaryPayment?.type ?? 'cash'

      // Format items for backend (ProductId, Quantity, UnitPrice, DiscountAmount)
      // Note: Backend calculates tax automatically, so we don't send it
      const items = cart.value.map(i => ({
        productId: Number(i.itemId),
        quantity: i.quantity,
        unitPrice: i.price,
        discountAmount: i.discount
      }))

      // Generate idempotency key for offline sync support
      const idempotencyKey = `pos_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`

      // Send to backend POS checkout
      const { data, error } = await salesApi.posCheckout(
        shopId,
        items,
        paymentType,
        customerId ? Number(customerId) : undefined,
        idempotencyKey,
        `POS sale - ${paymentType} payment`
      )

      if (error.value) {
        console.error('Failed to complete sale:', error.value)
        throw error.value
      }

      // Map backend response to frontend Sale format
      const saleId = data.value?.saleId ? String(data.value.saleId) : `sale_${Date.now()}`
      const sale: Sale = {
        id: saleId,
        invoiceNumber: data.value?.saleNumber ?? generateInvoiceNumber(),
        customerId,
        customerName,
        items: [...cart.value],
        subtotal: cartSubtotal.value,
        discount: cartDiscount.value,
        tax: cartTax.value,
        total: data.value?.total ?? cartTotal.value,
        payments,
        change,
        status: 'completed',
        createdAt: new Date(),
        createdBy: 'current_user'
      }

      recentSales.value.unshift(sale)
      clearCart()
      currentSale.value = sale
      return sale
    } catch (error) {
      console.error('Failed to complete sale:', error)
      throw error
    }
  }

  function generateInvoiceNumber(): string {
    const date = new Date()
    const year = date.getFullYear().toString().substr(-2)
    const month = (date.getMonth() + 1).toString().padStart(2, '0')
    const day = date.getDate().toString().padStart(2, '0')
    const sequence = (recentSales.value.length + 1).toString().padStart(4, '0')
    return `INV-${year}${month}${day}-${sequence}`
  }

  async function fetchRecentSales(limit: number = 20) {
    loading.value = true
    try {
      const { data, error } = await salesApi.getSales(undefined, undefined, 1, limit)
      if (error.value) {
        console.error('Failed to fetch recent sales:', error.value)
        return
      }
      const items = data.value?.items ?? data.value ?? []
      recentSales.value = items.map(mapSaleFromApi)
    } catch (error) {
      console.error('Failed to fetch recent sales:', error)
    } finally {
      loading.value = false
    }
  }

  function openCashDrawer() {
    cashDrawerOpen.value = true
    // TODO: Send command to physical cash drawer if connected
    console.log('Cash drawer opened')
  }

  function closeCashDrawer() {
    cashDrawerOpen.value = false
  }

  function holdSale() {
    if (cart.value.length === 0) return

    const heldSale = {
      id: `held_${Date.now()}`,
      items: [...cart.value],
      timestamp: new Date()
    }

    // Save to localStorage
    const heldSales = JSON.parse(localStorage.getItem('held_sales') || '[]')
    heldSales.push(heldSale)
    localStorage.setItem('held_sales', JSON.stringify(heldSales))

    clearCart()
  }

  function retrieveHeldSale(saleId: string) {
    const heldSales = JSON.parse(localStorage.getItem('held_sales') || '[]')
    const sale = heldSales.find((s: any) => s.id === saleId)

    if (sale) {
      cart.value = sale.items
      // Remove from held sales
      const updated = heldSales.filter((s: any) => s.id !== saleId)
      localStorage.setItem('held_sales', JSON.stringify(updated))
    }
  }

  function getHeldSales() {
    return JSON.parse(localStorage.getItem('held_sales') || '[]')
  }

  function mapSaleFromApi(sale: any): Sale {
    const payments: PaymentMethod[] = sale.payments ?? [
      { type: 'cash', amount: sale.total ?? 0 }
    ]

    return {
      id: String(sale.id ?? sale.saleId ?? crypto.randomUUID()),
      invoiceNumber: sale.invoiceNumber ?? sale.saleNumber ?? `INV-${Date.now()}`,
      customerId: sale.customerId ? String(sale.customerId) : undefined,
      customerName: sale.customerName,
      items: (sale.items ?? sale.lines ?? []).map((line: any) => ({
        id: String(line.id ?? crypto.randomUUID()),
        itemId: String(line.itemId ?? line.productId ?? ''),
        name: line.itemName ?? line.productName ?? 'Item',
        code: line.itemCode ?? line.code ?? '',
        price: line.unitPrice ?? line.price ?? 0,
        quantity: line.quantity ?? line.qty ?? 0,
        discount: line.discount ?? 0,
        tax: line.tax ?? 0,
        total: line.total ?? 0
      })),
      subtotal: sale.subtotal ?? sale.subTotal ?? sale.total ?? 0,
      discount: sale.discount ?? 0,
      tax: sale.tax ?? 0,
      total: sale.total ?? 0,
      payments,
      change: sale.change ?? 0,
      status: sale.status ?? 'completed',
      createdAt: sale.createdAt ? new Date(sale.createdAt) : new Date(),
      createdBy: sale.createdBy ?? 'system'
    }
  }

  return {
    // State
    cart,
    currentSale,
    recentSales,
    loading,
    cashDrawerOpen,
    // Computed
    cartSubtotal,
    cartDiscount,
    cartTax,
    cartTotal,
    cartItemCount,
    // Actions
    addToCart,
    removeFromCart,
    updateQuantity,
    updateDiscount,
    clearCart,
    completeSale,
    fetchRecentSales,
    openCashDrawer,
    closeCashDrawer,
    holdSale,
    retrieveHeldSale,
    getHeldSales
  }
})

