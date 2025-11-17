/**
 * usePosSession - POS Session State Management
 * 
 * Manages the active POS session including cart, customer, payments,
 * discounts, and completion flow. Built on top of usePosMock.
 */

import type { CartItem, Customer } from './usePosMock.js'
import type { PosPaymentEntry, PosParkedSale } from '~/types/sales'

export const usePosSession = () => {
  const posMock = usePosMock()
  
  // Session state
  const sessionId = ref<string>('')
  const cart = ref<CartItem[]>([])
  const customer = ref<Customer | null>(null)
  const payments = ref<PosPaymentEntry[]>([])
  const notes = ref<string>('')
  const defaultTaxRate = ref<number>(15) // SA VAT rate

  // Initialize session
  const initSession = (id: string) => {
    sessionId.value = id
    clearCart()
  }

  // Cart operations
  const addToCart = (product: {
    id: number
    name: string
    sku: string
    barcode?: string
    basePrice: number
    isTaxable: boolean
  }, quantity: number = 1) => {
    const existing = cart.value.find((item: CartItem) => item.productId === product.id)
    
    if (existing) {
      existing.quantity += quantity
    } else {
      cart.value.push({
        productId: product.id,
        productName: product.name,
        sku: product.sku,
        barcode: product.barcode,
        quantity,
        unitPrice: product.basePrice,
        discount: 0,
        discountType: 'fixed',
        taxRate: product.isTaxable ? defaultTaxRate.value : 0,
        isTaxable: product.isTaxable
      })
    }
  }

  const updateQuantity = (productId: number, quantity: number) => {
    const item = cart.value.find((i: CartItem) => i.productId === productId)
    if (item) {
      if (quantity <= 0) {
        removeFromCart(productId)
      } else {
        item.quantity = quantity
      }
    }
  }

  const removeFromCart = (productId: number) => {
    cart.value = cart.value.filter((item: CartItem) => item.productId !== productId)
  }

  const applyDiscount = (productId: number, discount: number, type: 'fixed' | 'percentage' = 'fixed') => {
    const item = cart.value.find((i: CartItem) => i.productId === productId)
    if (item) {
      item.discount = discount
      item.discountType = type
    }
  }

  const clearCart = () => {
    cart.value = []
    customer.value = null
    payments.value = []
    notes.value = ''
  }

  // Customer operations
  const setCustomer = (cust: Customer | null) => {
    customer.value = cust
  }

  // Payment operations
  const addPayment = (payment: Omit<PosPaymentEntry, 'id'>) => {
    payments.value.push({
      id: payments.value.length + 1,
      ...payment
    })
  }

  const removePayment = (paymentId: number) => {
    payments.value = payments.value.filter((p: PosPaymentEntry) => p.id !== paymentId)
  }

  const clearPayments = () => {
    payments.value = []
  }

  // Calculations
  const subtotal = computed(() => {
    return cart.value.reduce((sum: number, item: CartItem) => {
      const lineSubtotal = item.quantity * item.unitPrice
      const discount = item.discountType === 'percentage'
        ? (lineSubtotal * item.discount) / 100
        : item.discount
      return sum + (lineSubtotal - discount)
    }, 0)
  })

  const totalTax = computed(() => {
    return cart.value.reduce((sum: number, item: CartItem) => {
      if (!item.isTaxable) return sum
      const lineSubtotal = item.quantity * item.unitPrice
      const discount = item.discountType === 'percentage'
        ? (lineSubtotal * item.discount) / 100
        : item.discount
      const afterDiscount = lineSubtotal - discount
      return sum + (afterDiscount * item.taxRate) / 100
    }, 0)
  })

  const totalDiscount = computed(() => {
    return cart.value.reduce((sum: number, item: CartItem) => {
      const lineSubtotal = item.quantity * item.unitPrice
      return sum + (item.discountType === 'percentage'
        ? (lineSubtotal * item.discount) / 100
        : item.discount)
    }, 0)
  })

  const total = computed(() => subtotal.value + totalTax.value)

  const totalPaid = computed(() => {
    return payments.value.reduce((sum: number, p: PosPaymentEntry) => sum + p.amount, 0)
  })

  const balance = computed(() => total.value - totalPaid.value)

  const isPaymentComplete = computed(() => totalPaid.value >= total.value)

  // Validation
  const canCompleteSale = computed(() => {
    return cart.value.length > 0 && isPaymentComplete.value
  })

  const canHoldSale = computed(() => {
    return cart.value.length > 0
  }
)

  const validateCreditLimit = (): { valid: boolean; message?: string } => {
    if (!customer.value || !customer.value.creditLimit) return { valid: true }
    
    const accountPayment = payments.value.find((p: PosPaymentEntry) => p.method === 'account')
    if (!accountPayment) return { valid: true }

    const newBalance = (customer.value.creditBalance || 0) + accountPayment.amount
    
    if (newBalance > customer.value.creditLimit) {
      return {
        valid: false,
        message: `Credit limit exceeded. Available: R${(customer.value.creditLimit - (customer.value.creditBalance || 0)).toFixed(2)}`
      }
    }

    return { valid: true }
  }

  // Actions
  const completeSale = async () => {
    if (!canCompleteSale.value) {
      throw new Error('Cannot complete sale: cart is empty or payment incomplete')
    }

    const creditValidation = validateCreditLimit()
    if (!creditValidation.valid) {
      throw new Error(creditValidation.message)
    }

    const sale = posMock.createSaleFromCart(
      sessionId.value,
      cart.value,
      payments.value,
      customer.value?.id,
      customer.value?.name,
      notes.value
    )

    // Clear session after successful sale
    clearCart()

    return sale
  }

  const holdSale = () => {
    if (!canHoldSale.value) {
      throw new Error('Cannot hold sale: cart is empty')
    }

    const parkedSale = posMock.holdSale(
      cart.value,
      customer.value?.id,
      notes.value
    )

    // Clear cart after parking
    clearCart()

    return parkedSale
  }

  const recallSale = (parkedSale: PosParkedSale) => {
    // Restore cart from parked sale
    clearCart()
    
    cart.value = parkedSale.items.map((item: any) => ({
      productId: item.productId,
      productName: item.productName,
      sku: item.sku,
      barcode: item.barcode,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      discount: item.discount,
      discountType: item.discountType,
      taxRate: item.taxRate,
      isTaxable: item.isTaxable
    }))

    if (parkedSale.customerId) {
      // In a real app, fetch customer details
      // For now, just set the ID
      customer.value = { id: parkedSale.customerId, name: 'Customer' }
    }

    notes.value = parkedSale.notes || ''

    // Remove from parked sales
    posMock.removeParkedSale(parkedSale.reference)
  }

  const voidSale = () => {
    clearCart()
  }

  // Persistence (optional - for browser refresh recovery)
  const saveToLocalStorage = () => {
    if (typeof window === 'undefined') return
    
    const state = {
      sessionId: sessionId.value,
      cart: cart.value,
      customer: customer.value,
      payments: payments.value,
      notes: notes.value
    }
    
    localStorage.setItem('pos-session', JSON.stringify(state))
  }

  const restoreFromLocalStorage = () => {
    if (typeof window === 'undefined') return false
    
    const saved = localStorage.getItem('pos-session')
    if (!saved) return false

    try {
      const state = JSON.parse(saved)
      sessionId.value = state.sessionId || ''
      cart.value = state.cart || []
      customer.value = state.customer || null
      payments.value = state.payments || []
      notes.value = state.notes || ''
      return true
    } catch (error) {
      console.error('Error restoring POS session:', error)
      return false
    }
  }

  const clearLocalStorage = () => {
    if (typeof window === 'undefined') return
    localStorage.removeItem('pos-session')
  }

  // Auto-save on changes
  watch([cart, customer, payments, notes], () => {
    saveToLocalStorage()
  }, { deep: true })

  return {
    // State
    sessionId,
    cart,
    customer,
    payments,
    notes,
    defaultTaxRate,
    
    // Computed
    subtotal,
    totalTax,
    totalDiscount,
    total,
    totalPaid,
    balance,
    isPaymentComplete,
    canCompleteSale,
    canHoldSale,
    
    // Cart operations
    initSession,
    addToCart,
    updateQuantity,
    removeFromCart,
    applyDiscount,
    clearCart,
    
    // Customer
    setCustomer,
    
    // Payments
    addPayment,
    removePayment,
    clearPayments,
    
    // Validation
    validateCreditLimit,
    
    // Actions
    completeSale,
    holdSale,
    recallSale,
    voidSale,
    
    // Persistence
    saveToLocalStorage,
    restoreFromLocalStorage,
    clearLocalStorage
  }
}
