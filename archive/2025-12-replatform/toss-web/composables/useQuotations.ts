/**
 * Quotations Composable
 * Manages quotation operations including CRUD, status updates, and conversion to sales orders
 */

interface QuotationItem {
  id?: string
  productId: string
  productName: string
  description?: string
  quantity: number
  rate: number
  uom: string
  discount?: number
  discountType?: 'percentage' | 'amount'
  taxRate?: number
  amount: number
}

interface Quotation {
  id?: string
  quotationNumber?: string
  customerId: string
  customerName: string
  date: string
  validUntil: string
  status: 'draft' | 'submitted' | 'approved' | 'rejected' | 'expired' | 'converted'
  items: QuotationItem[]
  subtotal: number
  taxAmount: number
  discountAmount: number
  total: number
  terms?: string
  notes?: string
  salesPerson?: string
  territory?: string
  currency: string
  conversionRate: number
  priceList?: string
  shippingAddress?: string
  billingAddress?: string
  createdAt?: string
  updatedAt?: string
  convertedToOrder?: string
  approvedBy?: string
  approvedAt?: string
}

export const useQuotations = () => {
  const { $api } = useNuxtApp()
  const { t } = useI18n()
  
  // State
  const quotations = ref<Quotation[]>([])
  const currentQuotation = ref<Quotation | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Fetch all quotations
  const fetchQuotations = async (filters?: {
    status?: string
    customerId?: string
    dateFrom?: string
    dateTo?: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api('/api/sales/quotations', {
        method: 'GET',
        params: filters
      })
      quotations.value = response.data || []
      return quotations.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch single quotation
  const fetchQuotation = async (id: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/quotations/${id}`)
      currentQuotation.value = response.data
      return currentQuotation.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create quotation
  const createQuotation = async (quotation: Quotation) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api('/api/sales/quotations', {
        method: 'POST',
        body: quotation
      })
      
      quotations.value.unshift(response.data)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.createFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update quotation
  const updateQuotation = async (id: string, updates: Partial<Quotation>) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/quotations/${id}`, {
        method: 'PUT',
        body: updates
      })
      
      const index = quotations.value.findIndex(q => q.id === id)
      if (index !== -1) {
        quotations.value[index] = response.data
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.updateFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete quotation
  const deleteQuotation = async (id: string) => {
    loading.value = true
    error.value = null
    
    try {
      await $api(`/api/sales/quotations/${id}`, {
        method: 'DELETE'
      })
      
      quotations.value = quotations.value.filter(q => q.id !== id)
    } catch (err: any) {
      error.value = err.message || t('errors.deleteFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Change quotation status
  const changeStatus = async (id: string, status: Quotation['status'], notes?: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/quotations/${id}/status`, {
        method: 'PATCH',
        body: { status, notes }
      })
      
      const index = quotations.value.findIndex(q => q.id === id)
      if (index !== -1) {
        quotations.value[index].status = status
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.statusChangeFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Convert quotation to sales order
  const convertToSalesOrder = async (quotationId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/quotations/${quotationId}/convert`, {
        method: 'POST'
      })
      
      const index = quotations.value.findIndex(q => q.id === quotationId)
      if (index !== -1) {
        quotations.value[index].status = 'converted'
        quotations.value[index].convertedToOrder = response.data.orderId
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.conversionFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Calculate quotation totals
  const calculateTotals = (items: QuotationItem[]) => {
    let subtotal = 0
    let taxAmount = 0
    let discountAmount = 0

    items.forEach(item => {
      let itemAmount = item.quantity * item.rate
      
      // Apply discount
      if (item.discount) {
        if (item.discountType === 'percentage') {
          const discount = (itemAmount * item.discount) / 100
          discountAmount += discount
          itemAmount -= discount
        } else {
          discountAmount += item.discount
          itemAmount -= item.discount
        }
      }
      
      // Calculate tax
      if (item.taxRate) {
        const tax = (itemAmount * item.taxRate) / 100
        taxAmount += tax
      }
      
      subtotal += item.quantity * item.rate
      item.amount = itemAmount
    })

    const total = subtotal - discountAmount + taxAmount

    return {
      subtotal,
      taxAmount,
      discountAmount,
      total
    }
  }

  // Generate PDF
  const generatePDF = async (quotationId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/quotations/${quotationId}/pdf`, {
        method: 'GET',
        responseType: 'blob'
      })
      
      return response
    } catch (err: any) {
      error.value = err.message || t('errors.pdfGenerationFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Send quotation via email
  const sendEmail = async (quotationId: string, emailData: {
    to: string
    subject: string
    message: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      await $api(`/api/sales/quotations/${quotationId}/email`, {
        method: 'POST',
        body: emailData
      })
    } catch (err: any) {
      error.value = err.message || t('errors.emailSendFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Duplicate quotation
  const duplicateQuotation = async (quotationId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/quotations/${quotationId}/duplicate`, {
        method: 'POST'
      })
      
      quotations.value.unshift(response.data)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.duplicateFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    // State
    quotations,
    currentQuotation,
    loading,
    error,
    
    // Methods
    fetchQuotations,
    fetchQuotation,
    createQuotation,
    updateQuotation,
    deleteQuotation,
    changeStatus,
    convertToSalesOrder,
    calculateTotals,
    generatePDF,
    sendEmail,
    duplicateQuotation
  }
}
