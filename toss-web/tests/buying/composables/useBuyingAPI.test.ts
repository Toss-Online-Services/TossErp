import { describe, it, expect, beforeEach, vi } from 'vitest'
import { useBuyingAPI } from '../../../composables/useBuyingAPI'
import { useInMemoryDB } from '../../../composables/useInMemoryDB'

// Mock the in-memory database
vi.mock('../../../composables/useInMemoryDB', () => ({
  useInMemoryDB: vi.fn()
}))

describe('useBuyingAPI', () => {
  let api: ReturnType<typeof useBuyingAPI>
  let mockDB: any

  beforeEach(() => {
    // Setup mock database
    mockDB = {
      buyingOrders: {
        value: [
          {
            id: 'PO1',
            orderNumber: 'PO-2025-001',
            customer: 'Test Supplier',
            total: 5000,
            status: 'pending',
            type: 'buying'
          }
        ]
      },
      buyingInvoices: {
        value: [
          {
            id: 'BI1',
            invoiceNumber: 'PINV-2025-001',
            customer: 'Test Supplier',
            total: 5000,
            status: 'sent',
            type: 'buying'
          }
        ]
      },
      suppliers: {
        value: [
          {
            id: 'S1',
            name: 'Test Supplier',
            category: 'Beverages',
            rating: 4.5
          }
        ]
      },
      products: { value: [] },
      groupBuys: {
        value: [
          {
            id: 'GB1',
            productName: 'Test Product',
            targetQuantity: 100,
            currentQuantity: 0,
            currentParticipants: 0,
            participants: [],
            status: 'active'
          }
        ]
      },
      generateId: vi.fn((prefix: string) => `${prefix}-123`),
      generateOrderNumber: vi.fn(() => 'PO-2025-002'),
      generateInvoiceNumber: vi.fn(() => 'PINV-2025-002')
    }

    vi.mocked(useInMemoryDB).mockReturnValue(mockDB)
    api = useBuyingAPI()
  })

  describe('Orders', () => {
    it('gets all orders', async () => {
      const orders = await api.getOrders()
      expect(orders).toHaveLength(1)
      expect(orders[0].orderNumber).toBe('PO-2025-001')
    })

    it('gets single order', async () => {
      const order = await api.getOrder('PO1')
      expect(order).toBeDefined()
      expect(order?.orderNumber).toBe('PO-2025-001')
    })

    it('creates new order', async () => {
      const newOrder = await api.createOrder({
        customer: 'New Supplier',
        total: 3000,
        status: 'pending'
      })

      expect(newOrder.id).toBe('PO-123')
      expect(newOrder.orderNumber).toBe('PO-2025-002')
      expect(newOrder.customer).toBe('New Supplier')
      expect(mockDB.buyingOrders.value).toHaveLength(2)
    })

    it('updates order status', async () => {
      const updated = await api.updateOrderStatus('PO1', 'approved')
      expect(updated.status).toBe('approved')
    })

    it('approves order', async () => {
      const approved = await api.approveOrder('PO1')
      expect(approved.status).toBe('approved')
    })

    it('cancels order', async () => {
      const result = await api.cancelOrder('PO1')
      expect(result).toBe(true)
      expect(mockDB.buyingOrders.value[0].status).toBe('cancelled')
    })
  })

  describe('Invoices', () => {
    it('gets all invoices', async () => {
      const invoices = await api.getInvoices()
      expect(invoices).toHaveLength(1)
      expect(invoices[0].invoiceNumber).toBe('PINV-2025-001')
    })

    it('creates new invoice', async () => {
      const newInvoice = await api.createInvoice({
        customer: 'Test Supplier',
        total: 3000,
        status: 'draft'
      })

      expect(newInvoice.id).toBe('BI-123')
      expect(newInvoice.invoiceNumber).toBe('PINV-2025-002')
      expect(mockDB.buyingInvoices.value).toHaveLength(2)
    })

    it('updates invoice status', async () => {
      const updated = await api.updateInvoiceStatus('BI1', 'paid')
      expect(updated.status).toBe('paid')
    })
  })

  describe('Suppliers', () => {
    it('gets all suppliers', async () => {
      const suppliers = await api.getSuppliers()
      expect(suppliers).toHaveLength(1)
      expect(suppliers[0].name).toBe('Test Supplier')
    })

    it('adds new supplier', async () => {
      const newSupplier = await api.addSupplier({
        name: 'New Supplier',
        category: 'Groceries',
        contact: 'John Doe',
        phone: '+27 11 123 4567',
        email: 'john@newsupplier.com',
        address: '123 Street'
      })

      expect(newSupplier.id).toBe('SUP-123')
      expect(newSupplier.name).toBe('New Supplier')
      expect(newSupplier.rating).toBe(0)
      expect(mockDB.suppliers.value).toHaveLength(2)
    })
  })

  describe('Group Buys', () => {
    it('gets all group buys', async () => {
      const groupBuys = await api.getGroupBuys()
      expect(groupBuys).toHaveLength(1)
      expect(groupBuys[0].productName).toBe('Test Product')
    })

    it('creates new group buy', async () => {
      const newGroupBuy = await api.createGroupBuy({
        productName: 'New Product',
        targetQuantity: 200,
        supplier: 'Test Supplier'
      })

      expect(newGroupBuy.id).toBe('GB-123')
      expect(newGroupBuy.productName).toBe('New Product')
      expect(newGroupBuy.status).toBe('active')
      expect(mockDB.groupBuys.value).toHaveLength(2)
    })

    it('joins group buy', async () => {
      const groupBuy = await api.joinGroupBuy('GB1', 25, 'My Shop')
      
      expect(groupBuy.currentQuantity).toBe(25)
      expect(groupBuy.currentParticipants).toBe(1)
      expect(groupBuy.participants).toHaveLength(1)
    })

    it('updates status when target reached', async () => {
      const groupBuy = await api.joinGroupBuy('GB1', 100, 'Shop2')
      
      expect(groupBuy.currentQuantity).toBeGreaterThanOrEqual(100)
      expect(groupBuy.status).toBe('target_reached')
    })

    it('leaves group buy', async () => {
      // First join
      await api.joinGroupBuy('GB1', 30, 'LeaveShop')
      
      // Then leave
      const groupBuy = await api.leaveGroupBuy('GB1', 'LeaveShop')
      
      // After leaving, the shop should be removed from participants
      const leftShop = groupBuy.participants.find(p => p.shopName === 'LeaveShop')
      expect(leftShop).toBeUndefined()
    })
  })

  describe('Statistics', () => {
    it('calculates statistics correctly', async () => {
      const stats = await api.getStatistics()
      
      expect(stats.totalOrders).toBe(1)
      expect(stats.pendingOrders).toBe(1)
      expect(stats.totalSpent).toBe(5000)
      expect(stats.activeGroupBuys).toBe(1)
    })
  })
})

