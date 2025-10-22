import { computed } from 'vue'
import { useInMemoryDB } from './useInMemoryDB'

export const useSalesAPI = () => {
  const db = useInMemoryDB()

  // Get all sales orders
  const getOrders = async () => {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.salesOrders.value
  }

  // Get single order
  const getOrder = async (id: string) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.salesOrders.value.find(o => o.id === id)
  }

  // Create new order
  const createOrder = async (orderData: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const newOrder = {
      id: db.generateId('SO'),
      orderNumber: db.generateOrderNumber('sales'),
      ...orderData,
      orderDate: new Date(),
      type: 'sales' as const
    }
    
    db.salesOrders.value.unshift(newOrder)
    return newOrder
  }

  // Update order
  const updateOrder = async (id: string, updates: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const index = db.salesOrders.value.findIndex(o => o.id === id)
    if (index > -1) {
      db.salesOrders.value[index] = {
        ...db.salesOrders.value[index],
        ...updates
      }
      return db.salesOrders.value[index]
    }
    throw new Error('Order not found')
  }

  // Update order status
  const updateOrderStatus = async (id: string, status: string) => {
    return updateOrder(id, { status })
  }

  // Delete/Cancel order
  const cancelOrder = async (id: string) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const index = db.salesOrders.value.findIndex(o => o.id === id)
    if (index > -1) {
      db.salesOrders.value[index].status = 'cancelled'
      return true
    }
    return false
  }

  // Complete order (remove from queue)
  const completeOrder = async (id: string) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const index = db.salesOrders.value.findIndex(o => o.id === id)
    if (index > -1) {
      // Mark as completed instead of removing
      db.salesOrders.value[index].status = 'completed'
      return true
    }
    return false
  }

  // Get invoices
  const getInvoices = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.salesInvoices.value
  }

  // Create invoice
  const createInvoice = async (invoiceData: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const newInvoice = {
      id: db.generateId('SI'),
      invoiceNumber: db.generateInvoiceNumber('sales'),
      ...invoiceData,
      invoiceDate: new Date(),
      type: 'sales' as const
    }
    
    db.salesInvoices.value.unshift(newInvoice)
    return newInvoice
  }

  // Update invoice status
  const updateInvoiceStatus = async (id: string, status: string) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const index = db.salesInvoices.value.findIndex(i => i.id === id)
    if (index > -1) {
      db.salesInvoices.value[index].status = status
      return db.salesInvoices.value[index]
    }
    throw new Error('Invoice not found')
  }

  // Get products
  const getProducts = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.products.value
  }

  // Update product stock
  const updateProductStock = async (productId: number, quantity: number) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const product = db.products.value.find(p => p.id === productId)
    if (product) {
      product.stock += quantity
      return product
    }
    throw new Error('Product not found')
  }

  // Get customers
  const getCustomers = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.customers.value
  }

  // Statistics
  const getStatistics = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const orders = db.salesOrders.value
    return {
      totalOrders: orders.length,
      pendingOrders: orders.filter(o => o.status === 'pending').length,
      inProgressOrders: orders.filter(o => o.status === 'in-progress').length,
      readyOrders: orders.filter(o => o.status === 'ready').length,
      completedOrders: orders.filter(o => o.status === 'completed').length,
      totalRevenue: orders.reduce((sum, o) => sum + o.total, 0),
      todayOrders: orders.filter(o => {
        const today = new Date()
        const orderDate = new Date(o.orderDate)
        return orderDate.toDateString() === today.toDateString()
      }).length
    }
  }

  return {
    // Orders
    getOrders,
    getOrder,
    createOrder,
    updateOrder,
    updateOrderStatus,
    cancelOrder,
    completeOrder,

    // Invoices
    getInvoices,
    createInvoice,
    updateInvoiceStatus,

    // Products
    getProducts,
    updateProductStock,

    // Customers
    getCustomers,

    // Statistics
    getStatistics
  }
}

