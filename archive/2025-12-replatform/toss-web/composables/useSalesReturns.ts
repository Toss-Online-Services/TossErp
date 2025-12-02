/**
 * Sales Returns & Credit Notes Composable
 * Manages return authorizations, credit notes, and refund processing
 */

interface SalesReturnItem {
  id?: string
  salesInvoiceItemId: string
  productId: string
  productName: string
  soldQty: number
  returnQty: number
  rate: number
  amount: number
  returnReason: string
  condition: 'new' | 'used' | 'damaged' | 'defective'
  restockable: boolean
  serialNumbers?: string[]
  batchNumbers?: string[]
}

interface SalesReturn {
  id?: string
  returnNumber?: string
  salesInvoiceId: string
  salesInvoiceNumber: string
  customerId: string
  customerName: string
  returnDate: string
  status: 'draft' | 'submitted' | 'approved' | 'rejected' | 'completed'
  items: SalesReturnItem[]
  subtotal: number
  taxAmount: number
  total: number
  refundMethod: 'cash' | 'card' | 'credit_note' | 'original_payment'
  refundStatus: 'pending' | 'processing' | 'completed' | 'failed'
  refundAmount: number
  restockingFee?: number
  notes?: string
  approvedBy?: string
  approvedAt?: string
  creditNoteId?: string
  createdAt?: string
  updatedAt?: string
}

interface CreditNote {
  id?: string
  creditNoteNumber?: string
  salesReturnId?: string
  customerId: string
  customerName: string
  date: string
  expiryDate?: string
  status: 'active' | 'partially_used' | 'fully_used' | 'expired' | 'cancelled'
  originalAmount: number
  usedAmount: number
  balance: number
  currency: string
  notes?: string
  createdAt?: string
  updatedAt?: string
}

export const useSalesReturns = () => {
  const { api } = useApi()
  const { t } = useI18n()
  
  // State
  const salesReturns = ref<SalesReturn[]>([])
  const creditNotes = ref<CreditNote[]>([])
  const currentReturn = ref<SalesReturn | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Fetch all sales returns
  const fetchSalesReturns = async (filters?: {
    status?: string
    customerId?: string
    dateFrom?: string
    dateTo?: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/returns', { params: filters })
      salesReturns.value = response.data || []
      return salesReturns.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch single sales return
  const fetchSalesReturn = async (id: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/api/sales/returns/${id}`)
      currentReturn.value = response.data
      return currentReturn.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create sales return
  const createSalesReturn = async (salesReturn: SalesReturn) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post('/api/sales/returns', salesReturn)
      salesReturns.value.unshift(response.data)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.createFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create return from sales invoice
  const createReturnFromInvoice = async (invoiceId: string, items: Array<{
    itemId: string
    returnQty: number
    returnReason: string
    condition: string
  }>) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post(`/api/sales/invoices/${invoiceId}/return`, {
        items
      })
      salesReturns.value.unshift(response.data)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.createFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update sales return
  const updateSalesReturn = async (id: string, updates: Partial<SalesReturn>) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.put(`/api/sales/returns/${id}`, updates)
      
      const index = salesReturns.value.findIndex(r => r.id === id)
      if (index !== -1) {
        salesReturns.value[index] = response.data
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.updateFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Approve/Reject return
  const changeReturnStatus = async (id: string, status: 'approved' | 'rejected', notes?: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.patch(`/api/sales/returns/${id}/status`, {
        status,
        notes
      })
      
      const index = salesReturns.value.findIndex(r => r.id === id)
      if (index !== -1) {
        salesReturns.value[index].status = status
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.statusChangeFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Process refund
  const processRefund = async (id: string, refundData: {
    method: 'cash' | 'card' | 'credit_note' | 'original_payment'
    amount: number
    reference?: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post(`/api/sales/returns/${id}/refund`, refundData)
      
      const index = salesReturns.value.findIndex(r => r.id === id)
      if (index !== -1) {
        salesReturns.value[index].refundStatus = 'completed'
        salesReturns.value[index].refundAmount = refundData.amount
      }
      
      // If credit note was created, add to credit notes
      if (response.data.creditNote) {
        creditNotes.value.unshift(response.data.creditNote)
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('returns.errors.refundFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch credit notes
  const fetchCreditNotes = async (filters?: {
    status?: string
    customerId?: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get('/api/sales/credit-notes', { params: filters })
      creditNotes.value = response.data || []
      return creditNotes.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Apply credit note to invoice
  const applyCreditNote = async (creditNoteId: string, invoiceId: string, amount: number) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.post(`/api/sales/credit-notes/${creditNoteId}/apply`, {
        invoiceId,
        amount
      })
      
      // Update credit note balance
      const index = creditNotes.value.findIndex(cn => cn.id === creditNoteId)
      if (index !== -1) {
        creditNotes.value[index].usedAmount += amount
        creditNotes.value[index].balance -= amount
        
        if (creditNotes.value[index].balance === 0) {
          creditNotes.value[index].status = 'fully_used'
        } else {
          creditNotes.value[index].status = 'partially_used'
        }
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('creditNotes.errors.applyFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Cancel credit note
  const cancelCreditNote = async (id: string, reason: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.patch(`/api/sales/credit-notes/${id}/cancel`, {
        reason
      })
      
      const index = creditNotes.value.findIndex(cn => cn.id === id)
      if (index !== -1) {
        creditNotes.value[index].status = 'cancelled'
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.cancelFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Calculate return totals
  const calculateReturnTotals = (items: SalesReturnItem[]) => {
    let subtotal = 0
    let taxAmount = 0

    items.forEach(item => {
      const itemAmount = item.returnQty * item.rate
      subtotal += itemAmount
      // Assuming 15% VAT
      taxAmount += (itemAmount * 15) / 100
    })

    const total = subtotal + taxAmount

    return {
      subtotal,
      taxAmount,
      total
    }
  }

  // Get customer return history
  const getCustomerReturnHistory = async (customerId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await api.get(`/api/customers/${customerId}/returns`)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Get return reasons
  const getReturnReasons = () => {
    return [
      { value: 'defective', label: t('returns.reasons.defective') },
      { value: 'wrong_item', label: t('returns.reasons.wrongItem') },
      { value: 'damaged', label: t('returns.reasons.damaged') },
      { value: 'not_as_described', label: t('returns.reasons.notAsDescribed') },
      { value: 'customer_changed_mind', label: t('returns.reasons.changedMind') },
      { value: 'expired', label: t('returns.reasons.expired') },
      { value: 'duplicate_order', label: t('returns.reasons.duplicateOrder') },
      { value: 'other', label: t('returns.reasons.other') }
    ]
  }

  return {
    // State
    salesReturns,
    creditNotes,
    currentReturn,
    loading,
    error,
    
    // Methods
    fetchSalesReturns,
    fetchSalesReturn,
    createSalesReturn,
    createReturnFromInvoice,
    updateSalesReturn,
    changeReturnStatus,
    processRefund,
    fetchCreditNotes,
    applyCreditNote,
    cancelCreditNote,
    calculateReturnTotals,
    getCustomerReturnHistory,
    getReturnReasons
  }
}
