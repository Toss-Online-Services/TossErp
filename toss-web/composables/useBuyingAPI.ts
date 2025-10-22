import { computed } from 'vue'
import { useInMemoryDB } from './useInMemoryDB'

export const useBuyingAPI = () => {
  const db = useInMemoryDB()

  // Get all buying orders
  const getOrders = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.buyingOrders.value
  }

  // Get single order
  const getOrder = async (id: string) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.buyingOrders.value.find(o => o.id === id)
  }

  // Create new order
  const createOrder = async (orderData: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const newOrder = {
      id: db.generateId('PO'),
      orderNumber: db.generateOrderNumber('buying'),
      ...orderData,
      orderDate: new Date(),
      type: 'buying' as const
    }
    
    db.buyingOrders.value.unshift(newOrder)
    return newOrder
  }

  // Update order
  const updateOrder = async (id: string, updates: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const index = db.buyingOrders.value.findIndex(o => o.id === id)
    if (index > -1) {
      db.buyingOrders.value[index] = {
        ...db.buyingOrders.value[index],
        ...updates
      }
      return db.buyingOrders.value[index]
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
    
    const index = db.buyingOrders.value.findIndex(o => o.id === id)
    if (index > -1) {
      db.buyingOrders.value[index].status = 'cancelled'
      return true
    }
    return false
  }

  // Approve order
  const approveOrder = async (id: string) => {
    return updateOrderStatus(id, 'approved')
  }

  // Get invoices
  const getInvoices = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.buyingInvoices.value
  }

  // Create invoice
  const createInvoice = async (invoiceData: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const newInvoice = {
      id: db.generateId('BI'),
      invoiceNumber: db.generateInvoiceNumber('buying'),
      ...invoiceData,
      invoiceDate: new Date(),
      type: 'buying' as const
    }
    
    db.buyingInvoices.value.unshift(newInvoice)
    return newInvoice
  }

  // Update invoice status
  const updateInvoiceStatus = async (id: string, status: string) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const index = db.buyingInvoices.value.findIndex(i => i.id === id)
    if (index > -1) {
      db.buyingInvoices.value[index].status = status
      return db.buyingInvoices.value[index]
    }
    throw new Error('Invoice not found')
  }

  // Get suppliers
  const getSuppliers = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.suppliers.value
  }

  // Add supplier
  const addSupplier = async (supplierData: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const newSupplier = {
      id: db.generateId('SUP'),
      ...supplierData,
      rating: 0
    }
    
    db.suppliers.value.push(newSupplier)
    return newSupplier
  }

  // Get products (same as sales, shared inventory)
  const getProducts = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.products.value
  }

  // Get group buys
  const getGroupBuys = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    return db.groupBuys.value
  }

  // Create group buy
  const createGroupBuy = async (groupBuyData: any) => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const newGroupBuy = {
      id: db.generateId('GB'),
      ...groupBuyData,
      currentQuantity: 0,
      currentParticipants: 0,
      participants: [],
      status: 'active'
    }
    
    db.groupBuys.value.unshift(newGroupBuy)
    return newGroupBuy
  }

  // Join group buy
  const joinGroupBuy = async (groupBuyId: string, quantity: number, shopName: string = 'My Shop') => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const groupBuy = db.groupBuys.value.find(gb => gb.id === groupBuyId)
    if (!groupBuy) throw new Error('Group buy not found')

    // Check if already participating
    const existingParticipant = groupBuy.participants.find(p => p.shopName === shopName)
    if (existingParticipant) {
      existingParticipant.quantity += quantity
    } else {
      groupBuy.participants.push({ shopName, quantity })
      groupBuy.currentParticipants++
    }

    groupBuy.currentQuantity += quantity

    // Check if target reached
    if (groupBuy.currentQuantity >= groupBuy.targetQuantity) {
      groupBuy.status = 'target_reached'
    }

    return groupBuy
  }

  // Leave group buy
  const leaveGroupBuy = async (groupBuyId: string, shopName: string = 'My Shop') => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const groupBuy = db.groupBuys.value.find(gb => gb.id === groupBuyId)
    if (!groupBuy) throw new Error('Group buy not found')

    const participantIndex = groupBuy.participants.findIndex(p => p.shopName === shopName)
    if (participantIndex > -1) {
      const participant = groupBuy.participants[participantIndex]
      groupBuy.currentQuantity -= participant.quantity
      groupBuy.participants.splice(participantIndex, 1)
      groupBuy.currentParticipants--
    }

    return groupBuy
  }

  // Statistics
  const getStatistics = async () => {
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const orders = db.buyingOrders.value
    return {
      totalOrders: orders.length,
      pendingOrders: orders.filter(o => o.status === 'pending').length,
      approvedOrders: orders.filter(o => o.status === 'approved').length,
      receivedOrders: orders.filter(o => o.status === 'received').length,
      totalSpent: orders.reduce((sum, o) => sum + o.total, 0),
      activeGroupBuys: db.groupBuys.value.filter(gb => gb.status === 'active').length,
      totalSavings: db.groupBuys.value
        .filter(gb => gb.participants.some(p => p.shopName === 'My Shop'))
        .reduce((sum, gb) => {
          const myQuantity = gb.participants.find(p => p.shopName === 'My Shop')?.quantity || 0
          return sum + (myQuantity * gb.savings)
        }, 0)
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
    approveOrder,

    // Invoices
    getInvoices,
    createInvoice,
    updateInvoiceStatus,

    // Suppliers
    getSuppliers,
    addSupplier,

    // Products
    getProducts,

    // Group Buys
    getGroupBuys,
    createGroupBuy,
    joinGroupBuy,
    leaveGroupBuy,

    // Statistics
    getStatistics
  }
}

