import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useOfflineSync } from '~/composables/useOfflineSync'

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

    const sale: Sale = {
      id: `sale_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
      invoiceNumber: generateInvoiceNumber(),
      customerId,
      customerName,
      items: [...cart.value],
      subtotal: cartSubtotal.value,
      discount: cartDiscount.value,
      tax: cartTax.value,
      total: cartTotal.value,
      payments,
      change,
      status: 'pending_sync',
      createdAt: new Date(),
      createdBy: 'current_user' // TODO: Get from auth
    }

    try {
      // Queue for offline sync
      queueOperation('sale', sale)

      // Add to recent sales
      recentSales.value.unshift(sale)

      // Clear cart
      clearCart()

      // Set as current sale for receipt
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
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data would be loaded here
      // recentSales.value = response.data
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

