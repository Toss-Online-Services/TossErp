import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Customer {
  id: string
  name: string
  email?: string
  phone: string
  address?: string
  creditLimit: number
  currentBalance: number
  status: 'active' | 'inactive'
  createdAt: Date
}

export interface QuotationItem {
  id: string
  itemId: string
  itemName: string
  quantity: number
  rate: number
  discount: number
  tax: number
  amount: number
}

export interface Quotation {
  id: string
  quotationNumber: string
  customerId: string
  customerName: string
  date: Date
  validUntil: Date
  items: QuotationItem[]
  subtotal: number
  discount: number
  tax: number
  total: number
  status: 'draft' | 'sent' | 'accepted' | 'rejected' | 'expired'
  notes?: string
  createdBy: string
  createdAt: Date
}

export interface SalesOrder {
  id: string
  orderNumber: string
  customerId: string
  customerName: string
  quotationId?: string
  orderDate: Date
  deliveryDate?: Date
  items: QuotationItem[]
  subtotal: number
  discount: number
  tax: number
  total: number
  status: 'draft' | 'confirmed' | 'partially_delivered' | 'delivered' | 'cancelled'
  paymentStatus: 'unpaid' | 'partially_paid' | 'paid'
  notes?: string
  createdBy: string
  createdAt: Date
}

export interface DeliveryNote {
  id: string
  deliveryNumber: string
  orderId: string
  orderNumber: string
  customerId: string
  customerName: string
  deliveryDate: Date
  items: QuotationItem[]
  status: 'pending' | 'in_transit' | 'delivered' | 'failed'
  driverId?: string
  driverName?: string
  notes?: string
  signature?: string
  createdAt: Date
}

export interface Invoice {
  id: string
  invoiceNumber: string
  customerId: string
  customerName: string
  orderId?: string
  invoiceDate: Date
  dueDate: Date
  items: QuotationItem[]
  subtotal: number
  discount: number
  tax: number
  total: number
  amountPaid: number
  amountDue: number
  status: 'draft' | 'sent' | 'paid' | 'partially_paid' | 'overdue' | 'cancelled'
  paymentTerms?: string
  notes?: string
  createdAt: Date
}

export const useSalesStore = defineStore('sales', () => {
  // State
  const quotations = ref<Quotation[]>([])
  const orders = ref<SalesOrder[]>([])
  const deliveries = ref<DeliveryNote[]>([])
  const invoices = ref<Invoice[]>([])
  const loading = ref(false)

  // Computed
  const pendingQuotations = computed(() => {
    return quotations.value.filter(q => q.status === 'sent')
  })

  const activeOrders = computed(() => {
    return orders.value.filter(o => 
      o.status !== 'cancelled' && o.status !== 'delivered'
    )
  })

  const overdueInvoices = computed(() => {
    const now = new Date()
    return invoices.value.filter(inv => 
      inv.status !== 'paid' && inv.status !== 'cancelled' && new Date(inv.dueDate) < now
    )
  })

  const totalReceivables = computed(() => {
    return invoices.value
      .filter(inv => inv.status !== 'paid' && inv.status !== 'cancelled')
      .reduce((sum, inv) => sum + inv.amountDue, 0)
  })

  const salesByMonth = computed(() => {
    // Group invoices by month
    const monthlyData: Record<string, number> = {}
    invoices.value.forEach(inv => {
      const month = new Date(inv.invoiceDate).toLocaleDateString('en-ZA', { 
        year: 'numeric', 
        month: 'short' 
      })
      monthlyData[month] = (monthlyData[month] || 0) + inv.total
    })
    return monthlyData
  })

  // Actions
  async function fetchQuotations() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      quotations.value = [
        {
          id: '1',
          quotationNumber: 'QT-2025-001',
          customerId: 'cust-1',
          customerName: 'Thabo Builders',
          date: new Date(),
          validUntil: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
          items: [
            {
              id: '1',
              itemId: 'item-1',
              itemName: 'Cement 50kg',
              quantity: 100,
              rate: 100,
              discount: 0,
              tax: 1500,
              amount: 11500
            }
          ],
          subtotal: 10000,
          discount: 0,
          tax: 1500,
          total: 11500,
          status: 'sent',
          createdBy: 'admin',
          createdAt: new Date()
        }
      ]
    } catch (error) {
      console.error('Failed to fetch quotations:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchOrders() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      orders.value = []
    } catch (error) {
      console.error('Failed to fetch orders:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchInvoices() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      invoices.value = []
    } catch (error) {
      console.error('Failed to fetch invoices:', error)
    } finally {
      loading.value = false
    }
  }

  async function createQuotation(data: Omit<Quotation, 'id' | 'quotationNumber' | 'createdAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const quotation: Quotation = {
        ...data,
        id: `quot_${Date.now()}`,
        quotationNumber: generateQuotationNumber(),
        createdAt: new Date()
      }
      
      quotations.value.unshift(quotation)
      return quotation
    } catch (error) {
      console.error('Failed to create quotation:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function convertQuotationToOrder(quotationId: string) {
    const quotation = quotations.value.find(q => q.id === quotationId)
    if (!quotation) throw new Error('Quotation not found')

    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const order: SalesOrder = {
        id: `order_${Date.now()}`,
        orderNumber: generateOrderNumber(),
        customerId: quotation.customerId,
        customerName: quotation.customerName,
        quotationId: quotation.id,
        orderDate: new Date(),
        items: quotation.items,
        subtotal: quotation.subtotal,
        discount: quotation.discount,
        tax: quotation.tax,
        total: quotation.total,
        status: 'confirmed',
        paymentStatus: 'unpaid',
        createdBy: quotation.createdBy,
        createdAt: new Date()
      }
      
      orders.value.unshift(order)
      quotation.status = 'accepted'
      
      return order
    } catch (error) {
      console.error('Failed to convert quotation:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function createInvoiceFromOrder(orderId: string) {
    const order = orders.value.find(o => o.id === orderId)
    if (!order) throw new Error('Order not found')

    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const invoice: Invoice = {
        id: `inv_${Date.now()}`,
        invoiceNumber: generateInvoiceNumber(),
        customerId: order.customerId,
        customerName: order.customerName,
        orderId: order.id,
        invoiceDate: new Date(),
        dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000), // 30 days
        items: order.items,
        subtotal: order.subtotal,
        discount: order.discount,
        tax: order.tax,
        total: order.total,
        amountPaid: 0,
        amountDue: order.total,
        status: 'sent',
        createdAt: new Date()
      }
      
      invoices.value.unshift(invoice)
      return invoice
    } catch (error) {
      console.error('Failed to create invoice:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  function generateQuotationNumber(): string {
    const date = new Date()
    const year = date.getFullYear()
    const sequence = (quotations.value.length + 1).toString().padStart(3, '0')
    return `QT-${year}-${sequence}`
  }

  function generateOrderNumber(): string {
    const date = new Date()
    const year = date.getFullYear()
    const sequence = (orders.value.length + 1).toString().padStart(3, '0')
    return `SO-${year}-${sequence}`
  }

  function generateInvoiceNumber(): string {
    const date = new Date()
    const year = date.getFullYear()
    const sequence = (invoices.value.length + 1).toString().padStart(3, '0')
    return `INV-${year}-${sequence}`
  }

  return {
    // State
    quotations,
    orders,
    deliveries,
    invoices,
    loading,
    // Computed
    pendingQuotations,
    activeOrders,
    overdueInvoices,
    totalReceivables,
    salesByMonth,
    // Actions
    fetchQuotations,
    fetchOrders,
    fetchInvoices,
    createQuotation,
    convertQuotationToOrder,
    createInvoiceFromOrder
  }
})

