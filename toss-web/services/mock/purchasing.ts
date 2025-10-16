/**
 * Mock Purchasing Data Service
 * Simulates purchasing, suppliers, group buying, and procurement
 */

export interface Supplier {
  id: number
  name: string
  email: string
  phone: string
  category: string
  rating: number
  onTimeDelivery: number
  totalOrders: number
  isActive: boolean
}

export interface PurchaseOrder {
  id: number
  poNumber: string
  supplier: string
  supplierContact: string
  date: string
  itemCount: number
  amount: number
  status: 'Draft' | 'Submitted' | 'Confirmed' | 'Received' | 'Cancelled'
}

export interface PurchaseInvoice {
  id: number
  invoiceNumber: string
  supplier: string
  supplierEmail: string
  poReference: string
  invoiceDate: string
  dueDate: string
  amount: number
  outstanding: number
  status: 'Draft' | 'Submitted' | 'Paid' | 'Overdue'
}

export interface GroupBuyOpportunity {
  id: number
  item: string
  currentPrice: number
  groupPrice: number
  savings: number
  participants: number
  deadline: Date
  minParticipants: number
  category: string
}

export const mockSuppliers: Supplier[] = [
  { id: 1, name: 'ABC Suppliers Ltd', email: 'billing@abc.com', phone: '+27 11 123 4567', category: 'General Goods', rating: 4.5, onTimeDelivery: 94, totalOrders: 156, isActive: true },
  { id: 2, name: 'XYZ Manufacturing', email: 'accounts@xyz.com', phone: '+27 11 234 5678', category: 'Manufacturing', rating: 4.2, onTimeDelivery: 88, totalOrders: 89, isActive: true },
  { id: 3, name: 'Global Trade Co', email: 'finance@global.com', phone: '+27 11 345 6789', category: 'Import/Export', rating: 4.8, onTimeDelivery: 96, totalOrders: 234, isActive: true },
  { id: 4, name: 'Tech Supplies Inc', email: 'billing@tech.com', phone: '+27 11 456 7890', category: 'Technology', rating: 4.6, onTimeDelivery: 92, totalOrders: 67, isActive: true }
]

export const mockPurchaseOrders: PurchaseOrder[] = [
  { id: 1, poNumber: 'PO-2024-001', supplier: 'ABC Suppliers Ltd', supplierContact: 'john@abc.com', date: '2024-01-15', itemCount: 5, amount: 45000, status: 'Received' },
  { id: 2, poNumber: 'PO-2024-002', supplier: 'XYZ Manufacturing', supplierContact: 'sarah@xyz.com', date: '2024-01-20', itemCount: 3, amount: 32500, status: 'Confirmed' },
  { id: 3, poNumber: 'PO-2024-003', supplier: 'Global Trade Co', supplierContact: 'mike@global.com', date: '2024-02-01', itemCount: 8, amount: 67800, status: 'Submitted' },
  { id: 4, poNumber: 'PO-2024-004', supplier: 'Tech Supplies Inc', supplierContact: 'info@tech.com', date: '2024-02-05', itemCount: 12, amount: 125000, status: 'Draft' }
]

export const mockPurchaseInvoices: PurchaseInvoice[] = [
  { id: 1, invoiceNumber: 'PINV-2024-001', supplier: 'ABC Suppliers Ltd', supplierEmail: 'billing@abc.com', poReference: 'PO-2024-001', invoiceDate: '2024-01-20', dueDate: '2024-02-20', amount: 45000, outstanding: 0, status: 'Paid' },
  { id: 2, invoiceNumber: 'PINV-2024-002', supplier: 'XYZ Manufacturing', supplierEmail: 'accounts@xyz.com', poReference: 'PO-2024-002', invoiceDate: '2024-01-25', dueDate: '2024-02-25', amount: 32500, outstanding: 32500, status: 'Submitted' },
  { id: 3, invoiceNumber: 'PINV-2024-003', supplier: 'Global Trade Co', supplierEmail: 'finance@global.com', poReference: 'PO-2024-003', invoiceDate: '2024-02-05', dueDate: '2024-02-20', amount: 67800, outstanding: 67800, status: 'Overdue' }
]

export const mockGroupBuyOpportunities: GroupBuyOpportunity[] = [
  { id: 1, item: 'Coca Cola 2L (24-pack case)', currentPrice: 35.00, groupPrice: 29.50, savings: 16, participants: 8, deadline: new Date(Date.now() + 604800000), minParticipants: 10, category: 'Beverages' },
  { id: 2, item: 'Bread Bundle (20 loaves)', currentPrice: 18.00, groupPrice: 15.50, savings: 14, participants: 12, deadline: new Date(Date.now() + 432000000), minParticipants: 15, category: 'Groceries' },
  { id: 3, item: 'Household Supplies Bundle', currentPrice: 89.00, groupPrice: 75.00, savings: 16, participants: 6, deadline: new Date(Date.now() + 259200000), minParticipants: 8, category: 'Household' },
  { id: 4, item: 'Snack Variety Pack (48 units)', currentPrice: 12.00, groupPrice: 10.20, savings: 15, participants: 9, deadline: new Date(Date.now() + 518400000), minParticipants: 12, category: 'Snacks' }
]

export class MockPurchasingService {
  static getSuppliers(): Supplier[] {
    return mockSuppliers
  }

  static getSupplierById(id: number): Supplier | undefined {
    return mockSuppliers.find(s => s.id === id)
  }

  static getPurchaseOrders(): PurchaseOrder[] {
    return mockPurchaseOrders
  }

  static getPurchaseInvoices(): PurchaseInvoice[] {
    return mockPurchaseInvoices
  }

  static getGroupBuyOpportunities(): GroupBuyOpportunity[] {
    return mockGroupBuyOpportunities
  }

  static createPurchaseOrder(order: Omit<PurchaseOrder, 'id' | 'poNumber'>): PurchaseOrder {
    const newOrder: PurchaseOrder = {
      ...order,
      id: mockPurchaseOrders.length + 1,
      poNumber: `PO-2024-${String(mockPurchaseOrders.length + 1).padStart(3, '0')}`
    }
    mockPurchaseOrders.unshift(newOrder)
    return newOrder
  }

  static joinGroupBuy(opportunityId: number): boolean {
    const opportunity = mockGroupBuyOpportunities.find(o => o.id === opportunityId)
    if (opportunity) {
      opportunity.participants++
      return true
    }
    return false
  }

  static getSupplierPerformance() {
    return {
      onTimeDelivery: 94,
      qualityRate: 96,
      priceCompetitiveness: 87,
      responseTime: 92
    }
  }

  static getProcurementMetrics() {
    return {
      totalSpend: 2.45,
      costSavings: 245,
      savingsPercent: 18,
      activePOs: 124,
      avgPOValue: 19.8,
      avgSupplierScore: 4.2
    }
  }
}

