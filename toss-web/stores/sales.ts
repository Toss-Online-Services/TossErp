import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useSalesApi } from '~/composables/useSalesApi'

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

// Alias to align with backend sale DTOs used by API composables
export type Sale = SalesOrder

export const useSalesStore = defineStore('sales', () => {
  const salesApi = useSalesApi()
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
  async function fetchQuotations(shopId?: number) {
    loading.value = true
    try {
      const { data, error } = await salesApi.getQuotes(shopId)
      if (error.value) {
        console.error('Failed to fetch quotations:', error.value)
        return
      }
      quotations.value = (data.value ?? []).map(mapQuoteFromApi)
    } finally {
      loading.value = false
    }
  }

  async function fetchOrders(shopId?: number, status?: string) {
    loading.value = true
    try {
      const { data, error } = await salesApi.getOrders(shopId)
      if (error.value) {
        console.error('Failed to fetch orders:', error.value)
        return
      }
      const filtered = status ? (data.value ?? []).filter(o => (o as any).status === status) : (data.value ?? [])
      orders.value = filtered.map(mapOrderFromApi)
    } finally {
      loading.value = false
    }
  }

  async function fetchInvoices(shopId?: number, status?: string) {
    loading.value = true
    try {
      const { data, error } = await salesApi.getInvoices(shopId, status)
      if (error.value) {
        console.error('Failed to fetch invoices:', error.value)
        return
      }
      const items = data.value?.items ?? data.value ?? []
      invoices.value = items.map(mapInvoiceFromApi)
    } finally {
      loading.value = false
    }
  }

  async function fetchDeliveries(shopId?: number) {
    loading.value = true
    try {
      const { data, error } = await salesApi.getDeliveryNotes(shopId)
      if (error.value) {
        console.error('Failed to fetch delivery notes:', error.value)
        return
      }
      deliveries.value = (data.value ?? []).map(mapDeliveryFromApi)
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

  function getQuotationById(id: string): Quotation | undefined {
    return quotations.value.find(q => q.id === id)
  }

  function getOrderById(id: string): SalesOrder | undefined {
    return orders.value.find(o => o.id === id)
  }

  function getInvoiceById(id: string): Invoice | undefined {
    return invoices.value.find(inv => inv.id === id)
  }

  function getDeliveryById(id: string): DeliveryNote | undefined {
    return deliveries.value.find(d => d.id === id)
  }

  function mapQuoteFromApi(quote: any): Quotation {
    return {
      id: String(quote.id ?? quote.saleId ?? crypto.randomUUID()),
      quotationNumber: quote.quotationNumber ?? quote.quoteNumber ?? quote.documentNumber ?? `QT-${Date.now()}`,
      customerId: String(quote.customerId ?? quote.customer?.id ?? ''),
      customerName: quote.customerName ?? quote.customer?.name ?? 'Customer',
      date: quote.date ? new Date(quote.date) : new Date(),
      validUntil: quote.validUntil ? new Date(quote.validUntil) : new Date(),
      items: (quote.items ?? quote.lines ?? []).map(mapLineItemFromApi),
      subtotal: quote.subtotal ?? quote.subTotal ?? quote.total ?? 0,
      discount: quote.discount ?? 0,
      tax: quote.tax ?? 0,
      total: quote.total ?? 0,
      status: quote.status ?? 'draft',
      notes: quote.notes,
      createdBy: quote.createdBy ?? 'system',
      createdAt: quote.createdAt ? new Date(quote.createdAt) : new Date()
    }
  }

  function mapOrderFromApi(order: any): SalesOrder {
    return {
      id: String(order.id ?? order.saleId ?? crypto.randomUUID()),
      orderNumber: order.orderNumber ?? order.saleNumber ?? order.documentNumber ?? `SO-${Date.now()}`,
      customerId: String(order.customerId ?? order.customer?.id ?? ''),
      customerName: order.customerName ?? order.customer?.name ?? 'Customer',
      quotationId: order.quotationId ? String(order.quotationId) : undefined,
      orderDate: order.orderDate ? new Date(order.orderDate) : new Date(),
      deliveryDate: order.deliveryDate ? new Date(order.deliveryDate) : undefined,
      items: (order.items ?? order.lines ?? []).map(mapLineItemFromApi),
      subtotal: order.subtotal ?? order.subTotal ?? order.total ?? 0,
      discount: order.discount ?? 0,
      tax: order.tax ?? 0,
      total: order.total ?? 0,
      status: order.status ?? 'confirmed',
      paymentStatus: order.paymentStatus ?? 'unpaid',
      notes: order.notes,
      createdBy: order.createdBy ?? 'system',
      createdAt: order.createdAt ? new Date(order.createdAt) : new Date()
    }
  }

  function mapInvoiceFromApi(invoice: any): Invoice {
    return {
      id: String(invoice.id ?? invoice.invoiceId ?? crypto.randomUUID()),
      invoiceNumber: invoice.invoiceNumber ?? invoice.documentNumber ?? `INV-${Date.now()}`,
      customerId: String(invoice.customerId ?? invoice.customer?.id ?? ''),
      customerName: invoice.customerName ?? invoice.customer?.name ?? 'Customer',
      orderId: invoice.orderId ? String(invoice.orderId) : undefined,
      invoiceDate: invoice.invoiceDate ? new Date(invoice.invoiceDate) : new Date(),
      dueDate: invoice.dueDate ? new Date(invoice.dueDate) : new Date(),
      items: (invoice.items ?? invoice.lines ?? []).map(mapLineItemFromApi),
      subtotal: invoice.subtotal ?? invoice.subTotal ?? invoice.total ?? 0,
      discount: invoice.discount ?? 0,
      tax: invoice.tax ?? 0,
      total: invoice.total ?? 0,
      amountPaid: invoice.amountPaid ?? invoice.paidAmount ?? 0,
      amountDue: invoice.amountDue ?? invoice.outstandingAmount ?? Math.max((invoice.total ?? 0) - (invoice.amountPaid ?? 0), 0),
      status: invoice.status ?? 'draft',
      paymentTerms: invoice.paymentTerms,
      notes: invoice.notes,
      createdAt: invoice.createdAt ? new Date(invoice.createdAt) : new Date()
    }
  }

  function mapDeliveryFromApi(delivery: any): DeliveryNote {
    return {
      id: String(delivery.id ?? delivery.deliveryNoteId ?? crypto.randomUUID()),
      deliveryNumber: delivery.deliveryNumber ?? delivery.documentNumber ?? `DN-${Date.now()}`,
      orderId: delivery.orderId ? String(delivery.orderId) : '',
      orderNumber: delivery.orderNumber ?? '',
      customerId: String(delivery.customerId ?? delivery.customer?.id ?? ''),
      customerName: delivery.customerName ?? delivery.customer?.name ?? 'Customer',
      deliveryDate: delivery.deliveryDate ? new Date(delivery.deliveryDate) : new Date(),
      items: (delivery.items ?? delivery.lines ?? []).map(mapLineItemFromApi),
      status: delivery.status ?? 'pending',
      driverId: delivery.driverId ? String(delivery.driverId) : undefined,
      driverName: delivery.driverName,
      notes: delivery.notes,
      signature: delivery.signature,
      createdAt: delivery.createdAt ? new Date(delivery.createdAt) : new Date()
    }
  }

  function mapLineItemFromApi(line: any): QuotationItem {
    return {
      id: String(line.id ?? crypto.randomUUID()),
      itemId: String(line.itemId ?? line.productId ?? ''),
      itemName: line.itemName ?? line.productName ?? 'Item',
      quantity: line.quantity ?? line.qty ?? 0,
      rate: line.rate ?? line.unitPrice ?? 0,
      discount: line.discount ?? 0,
      tax: line.tax ?? 0,
      amount: line.amount ?? line.total ?? 0
    }
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
    fetchDeliveries,
    createQuotation,
    convertQuotationToOrder,
    createInvoiceFromOrder,
    getQuotationById,
    getOrderById,
    getInvoiceById,
    getDeliveryById
  }
})

