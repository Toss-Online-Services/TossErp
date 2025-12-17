/**
 * Delivery Notes Composable
 * Manages delivery note operations including creation, tracking, and proof of delivery
 */

interface DeliveryNoteItem {
  id?: string
  salesOrderItemId?: string
  productId: string
  productName: string
  description?: string
  orderedQty: number
  deliveredQty: number
  uom: string
  serialNumbers?: string[]
  batchNumbers?: string[]
  warehouse?: string
}

interface DeliveryNote {
  id?: string
  deliveryNoteNumber?: string
  salesOrderId?: string
  salesOrderNumber?: string
  customerId: string
  customerName: string
  date: string
  deliveryDate?: string
  status: 'draft' | 'submitted' | 'completed' | 'cancelled' | 'returned'
  items: DeliveryNoteItem[]
  shippingAddress: string
  billingAddress?: string
  carrier?: string
  trackingNumber?: string
  driverId?: string
  driverName?: string
  vehicleNumber?: string
  instructions?: string
  notes?: string
  proofOfDelivery?: {
    signatureUrl?: string
    photoUrl?: string
    deliveredAt?: string
    receivedBy?: string
    notes?: string
  }
  createdAt?: string
  updatedAt?: string
}

export const useDeliveryNotes = () => {
  const { $api } = useApi()
  const { t } = useI18n()
  
  // State
  const deliveryNotes = ref<DeliveryNote[]>([])
  const currentDeliveryNote = ref<DeliveryNote | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Fetch all delivery notes
  const fetchDeliveryNotes = async (filters?: {
    status?: string
    customerId?: string
    dateFrom?: string
    dateTo?: string
    driverId?: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api('/api/sales/delivery-notes', {
        method: 'GET',
        params: filters
      })
      deliveryNotes.value = response.data || []
      return deliveryNotes.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Fetch single delivery note
  const fetchDeliveryNote = async (id: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/delivery-notes/${id}`)
      currentDeliveryNote.value = response.data
      return currentDeliveryNote.value
    } catch (err: any) {
      error.value = err.message || t('errors.fetchFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create delivery note
  const createDeliveryNote = async (deliveryNote: DeliveryNote) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api('/api/sales/delivery-notes', {
        method: 'POST',
        body: deliveryNote
      })
      
      deliveryNotes.value.unshift(response.data)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.createFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Create delivery note from sales order
  const createFromSalesOrder = async (salesOrderId: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/sales-orders/${salesOrderId}/delivery-note`, {
        method: 'POST'
      })
      
      deliveryNotes.value.unshift(response.data)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.createFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update delivery note
  const updateDeliveryNote = async (id: string, updates: Partial<DeliveryNote>) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/delivery-notes/${id}`, {
        method: 'PUT',
        body: updates
      })
      
      const index = deliveryNotes.value.findIndex(dn => dn.id === id)
      if (index !== -1) {
        deliveryNotes.value[index] = response.data
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.updateFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Delete delivery note
  const deleteDeliveryNote = async (id: string) => {
    loading.value = true
    error.value = null
    
    try {
      await $api(`/api/sales/delivery-notes/${id}`, {
        method: 'DELETE'
      })
      
      deliveryNotes.value = deliveryNotes.value.filter(dn => dn.id !== id)
    } catch (err: any) {
      error.value = err.message || t('errors.deleteFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Update delivery status
  const updateDeliveryStatus = async (id: string, status: DeliveryNote['status']) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/delivery-notes/${id}/status`, {
        method: 'PATCH',
        body: { status }
      })
      
      const index = deliveryNotes.value.findIndex(dn => dn.id === id)
      if (index !== -1) {
        deliveryNotes.value[index].status = status
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.statusChangeFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Submit proof of delivery
  const submitProofOfDelivery = async (id: string, proof: {
    signatureUrl?: string
    photoUrl?: string
    receivedBy: string
    notes?: string
  }) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/delivery-notes/${id}/proof`, {
        method: 'POST',
        body: {
          ...proof,
          deliveredAt: new Date().toISOString()
        }
      })
      
      const index = deliveryNotes.value.findIndex(dn => dn.id === id)
      if (index !== -1) {
        deliveryNotes.value[index].proofOfDelivery = response.data.proofOfDelivery
        deliveryNotes.value[index].status = 'completed'
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.proofSubmissionFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Assign driver
  const assignDriver = async (id: string, driverId: string, vehicleNumber?: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/delivery-notes/${id}/assign-driver`, {
        method: 'PATCH',
        body: { driverId, vehicleNumber }
      })
      
      const index = deliveryNotes.value.findIndex(dn => dn.id === id)
      if (index !== -1) {
        deliveryNotes.value[index].driverId = driverId
        deliveryNotes.value[index].vehicleNumber = vehicleNumber
      }
      
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.assignmentFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  // Generate packing slip PDF
  const generatePackingSlip = async (id: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/delivery-notes/${id}/packing-slip`, {
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

  // Track delivery
  const trackDelivery = async (trackingNumber: string) => {
    loading.value = true
    error.value = null
    
    try {
      const response = await $api(`/api/sales/delivery-notes/track/${trackingNumber}`)
      return response.data
    } catch (err: any) {
      error.value = err.message || t('errors.trackingFailed')
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    // State
    deliveryNotes,
    currentDeliveryNote,
    loading,
    error,
    
    // Methods
    fetchDeliveryNotes,
    fetchDeliveryNote,
    createDeliveryNote,
    createFromSalesOrder,
    updateDeliveryNote,
    deleteDeliveryNote,
    updateDeliveryStatus,
    submitProofOfDelivery,
    assignDriver,
    generatePackingSlip,
    trackDelivery
  }
}
