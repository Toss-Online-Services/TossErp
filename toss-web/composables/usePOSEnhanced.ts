/**
 * Enhanced POS Composable
 * Manages Point of Sale operations including profiles, sessions, loyalty, and multi-payment modes
 */

interface POSProfile {
  id: string
  name: string
  warehouse: string
  priceList: string
  currency: string
  allowDiscount: boolean
  maxDiscount: number
  allowCreditSales: boolean
  printReceiptAutomatically: boolean
  enableLoyalty: boolean
  paymentMethods: string[]
}

interface POSSession {
  id?: string
  profileId: string
  userId: string
  userName: string
  openingDate: string
  closingDate?: string
  status: 'open' | 'closed'
  openingCash: number
  closingCash?: number
  expectedCash?: number
  difference?: number
  totalSales: number
  totalReturns: number
  cashPayments: number
  cardPayments: number
  mobilePayments: number
  otherPayments: number
  notes?: string
}

interface PaymentEntry {
  mode: 'cash' | 'card' | 'mobile' | 'credit' | 'other'
  amount: number
  reference?: string
  accountNumber?: string
}

interface POSCustomer {
  id: string
  name: string
  phone?: string
  email?: string
  loyaltyPoints?: number
  creditLimit?: number
  outstandingBalance?: number
}

interface POSCartItem {
  productId: string
  productName: string
  barcode?: string
  quantity: number
  rate: number
  discount?: number
  discountType?: 'percentage' | 'amount'
  taxRate: number
  total: number
  availableQty?: number
}

export const usePOS = () => {
  const { api } = useApi()
  const { user } = useAuth()
  
  // Simple translation function
  const t = (key: string) => {
    const translations: Record<string, string> = {
      'errors.fetchFailed': 'Failed to fetch data',
      'errors.createFailed': 'Failed to create item',
      'errors.updateFailed': 'Failed to update item',
      'errors.deleteFailed': 'Failed to delete item',
      'errors.sessionNotFound': 'Session not found',
      'errors.invalidPayment': 'Invalid payment details',
      'errors.insufficientStock': 'Insufficient stock',
      'errors.sessionClosed': 'Session is closed',
      'errors.invalidDiscount': 'Invalid discount amount'
    }
    return translations[key] || key
  }
  
  // State
  const profiles = ref<POSProfile[]>([])
  const activeProfile = ref<POSProfile | null>(null)
  const currentSession = ref<POSSession | null>(null)
  const cartItems = ref<POSCartItem[]>([])
  const selectedCustomer = ref<POSCustomer | null>(null)
  const payments = ref<PaymentEntry[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed
  const cartSubtotal = computed(() => {
    return cartItems.value.reduce((sum, item) => sum + (item.quantity * item.rate), 0)
  })

  const cartDiscount = computed(() => {
    return cartItems.value.reduce((sum, item) => {
      if (!item.discount) return sum
      if (item.discountType === 'percentage') {
        return sum + ((item.quantity * item.rate * item.discount) / 100)
      }
      return sum + item.discount
    }, 0)
  })

  const cartTax = computed(() => {
    return cartItems.value.reduce((sum, item) => {
      const itemTotal = (item.quantity * item.rate) - (item.discount || 0)
      return sum + ((itemTotal * item.taxRate) / 100)
    }, 0)
  })

  const cartTotal = computed(() => {
    return cartSubtotal.value - cartDiscount.value + cartTax.value
  })

  const totalPayments = computed(() => {
    return payments.value.reduce((sum, p) => sum + p.amount, 0)
  })

  const balanceDue = computed(() => {
    return cartTotal.value - totalPayments.value
  })

  // Fetch POS profiles
  const fetchProfiles = async () => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/pos/profiles')
      profiles.value = response.data || []
      
      // Set active profile from localStorage or first profile
      const savedProfileId = localStorage.getItem('activePOSProfile')
      if (savedProfileId) {
        activeProfile.value = profiles.value.find(p => p.id === savedProfileId) || profiles.value[0]
      } else {
        activeProfile.value = profiles.value[0]
      }
      
      return profiles.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Set active profile
  const setActiveProfile = (profileId: string) => {
    const profile = profiles.value.find(p => p.id === profileId)
    if (profile) {
      activeProfile.value = profile
      localStorage.setItem('activePOSProfile', profileId)
    }
  }

  // Open POS session
  const openSession = async (openingCash: number, notes?: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/api/pos/sessions/open', {
        profileId: activeProfile.value?.id,
        userId: user.value?.id,
        userName: user.value?.name,
        openingCash,
        notes
      })
      
      currentSession.value = response.data
      localStorage.setItem('currentPOSSession', JSON.stringify(response.data))
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('pos.errors.sessionOpenFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Close POS session
  const closeSession = async (closingCash: number, notes?: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post(`/api/pos/sessions/${currentSession.value?.id}/close`, {
        closingCash,
        notes
      })
      
      currentSession.value = null
      localStorage.removeItem('currentPOSSession')
      clearCart()
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('pos.errors.sessionCloseFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Load session from storage
  const loadSession = () => {
    const saved = localStorage.getItem('currentPOSSession')
    if (saved) {
      currentSession.value = JSON.parse(saved)
    }
  }

  // Add item to cart
  const addToCart = (item: POSCartItem) => {
    const existingIndex = cartItems.value.findIndex(i => i.productId === item.productId)
    
    if (existingIndex !== -1) {
      cartItems.value[existingIndex].quantity += item.quantity
      updateCartItemTotal(existingIndex)
    } else {
      cartItems.value.push({
        ...item,
        total: calculateItemTotal(item)
      })
    }
    
    saveCartToStorage()
  }

  // Remove item from cart
  const removeFromCart = (index: number) => {
    cartItems.value.splice(index, 1)
    saveCartToStorage()
  }

  // Update cart item quantity
  const updateQuantity = (index: number, quantity: number) => {
    if (quantity <= 0) {
      removeFromCart(index)
      return
    }
    
    cartItems.value[index].quantity = quantity
    updateCartItemTotal(index)
    saveCartToStorage()
  }

  // Apply discount to item
  const applyDiscount = (index: number, discount: number, type: 'percentage' | 'amount') => {
    const item = cartItems.value[index]
    const maxDiscount = activeProfile.value?.maxDiscount || 100
    
    if (type === 'percentage' && discount > maxDiscount) {
      throw new Error(t('pos.errors.discountExceedsLimit', { max: maxDiscount }))
    }
    
    item.discount = discount
    item.discountType = type
    updateCartItemTotal(index)
    saveCartToStorage()
  }

  // Calculate item total
  const calculateItemTotal = (item: POSCartItem): number => {
    let total = item.quantity * item.rate
    
    if (item.discount) {
      if (item.discountType === 'percentage') {
        total -= (total * item.discount) / 100
      } else {
        total -= item.discount
      }
    }
    
    total += (total * item.taxRate) / 100
    
    return total
  }

  // Update cart item total
  const updateCartItemTotal = (index: number) => {
    const item = cartItems.value[index]
    item.total = calculateItemTotal(item)
  }

  // Add payment
  const addPayment = (payment: PaymentEntry) => {
    if (totalPayments.value + payment.amount > cartTotal.value) {
      throw new Error(t('pos.errors.paymentExceedsTotal'))
    }
    
    payments.value.push(payment)
  }

  // Remove payment
  const removePayment = (index: number) => {
    payments.value.splice(index, 1)
  }

  // Clear cart
  const clearCart = () => {
    cartItems.value = []
    payments.value = []
    selectedCustomer.value = null
    localStorage.removeItem('posCart')
    localStorage.removeItem('posPayments')
  }

  // Save cart to storage
  const saveCartToStorage = () => {
    localStorage.setItem('posCart', JSON.stringify(cartItems.value))
  }

  // Load cart from storage
  const loadCartFromStorage = () => {
    const saved = localStorage.getItem('posCart')
    if (saved) {
      cartItems.value = JSON.parse(saved)
    }
    
    const savedPayments = localStorage.getItem('posPayments')
    if (savedPayments) {
      payments.value = JSON.parse(savedPayments)
    }
  }

  // Complete sale
  const completeSale = async (customerId?: string, notes?: string) => {
    if (balanceDue.value > 0.01) {
      throw new Error(t('pos.errors.incompletePayment'))
    }
    
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/api/pos/sales', {
        sessionId: currentSession.value?.id,
        customerId: customerId || selectedCustomer.value?.id,
        items: cartItems.value,
        payments: payments.value,
        subtotal: cartSubtotal.value,
        discount: cartDiscount.value,
        tax: cartTax.value,
        total: cartTotal.value,
        notes
      })
      
      // Print receipt if enabled
      if (activeProfile.value?.printReceiptAutomatically) {
        await printReceipt(response.data.id)
      }
      
      // Update loyalty points if enabled
      if (activeProfile.value?.enableLoyalty && selectedCustomer.value) {
        await updateLoyaltyPoints(selectedCustomer.value.id, cartTotal.value)
      }
      
      clearCart()
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('pos.errors.saleFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Print receipt
  const printReceipt = async (saleId: string) => {
    try {
      const response = await api.get(`/api/pos/sales/${saleId}/receipt`, {
        responseType: 'blob'
      })
      
      // Create blob URL and open in new window for printing
      const blob = new Blob([response], { type: 'application/pdf' })
      const url = window.URL.createObjectURL(blob)
      window.open(url, '_blank')
      
      return response
    } catch (err: any) {
      console.error('Receipt print failed:', err)
      throw err
    }
  }

  // Park sale (save for later)
  const parkSale = async (reference: string) => {
    try {
      await api.post('/api/pos/sales/park', {
        reference,
        items: cartItems.value,
        payments: payments.value,
        customerId: selectedCustomer.value?.id
      })
      
      clearCart()
    } catch (err: any) {
      error.value = err.message || t('pos.errors.parkFailed')
      throw err
    }
  }

  // Retrieve parked sale
  const retrieveParkedSale = async (reference: string) => {
    loading.value = true
    
    try {
      const response = await api.get(`/api/pos/sales/parked/${reference}`)
      
      cartItems.value = response.data.items
      payments.value = response.data.payments
      if (response.data.customerId) {
        // Fetch customer details
        const customerResponse = await api.get(`/api/customers/${response.data.customerId}`)
        selectedCustomer.value = customerResponse.data
      }
      
      saveCartToStorage()
    } catch (err: any) {
      error.value = err.message || t('pos.errors.retrieveFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update loyalty points
  const updateLoyaltyPoints = async (customerId: string, saleAmount: number) => {
    try {
      const points = Math.floor(saleAmount / 10) // 1 point per R10 spent
      await api.post(`/api/customers/${customerId}/loyalty`, {
        points,
        transactionType: 'earn',
        amount: saleAmount
      })
    } catch (err: any) {
      console.error('Loyalty update failed:', err)
    }
  }

  // Redeem loyalty points
  const redeemLoyaltyPoints = async (customerId: string, points: number) => {
    try {
      const response = await api.post(`/api/customers/${customerId}/loyalty/redeem`, {
        points
      })
      
      // Add as payment
      const discount = points / 10 // R1 per 10 points
      addPayment({
        mode: 'other',
        amount: discount,
        reference: `Loyalty Points: ${points}`
      })
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('pos.errors.redeemFailed')
      throw err
    }
  }

  // Search product by barcode
  const searchByBarcode = async (barcode: string) => {
    loading.value = true
    
    try {
      const response = await api.get(`/api/products/barcode/${barcode}`)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('pos.errors.productNotFound')
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    // State
    profiles,
    activeProfile,
    currentSession,
    cartItems,
    selectedCustomer,
    payments,
    loading,
    error,
    
    // Computed
    cartSubtotal,
    cartDiscount,
    cartTax,
    cartTotal,
    totalPayments,
    balanceDue,
    
    // Methods
    fetchProfiles,
    setActiveProfile,
    openSession,
    closeSession,
    loadSession,
    addToCart,
    removeFromCart,
    updateQuantity,
    applyDiscount,
    addPayment,
    removePayment,
    clearCart,
    loadCartFromStorage,
    completeSale,
    printReceipt,
    parkSale,
    retrieveParkedSale,
    redeemLoyaltyPoints,
    searchByBarcode
  }
}
