/**
 * Mock Sales Data Service
 * Simulates sales orders, quotations, invoices, and POS transactions
 */

export interface SalesOrder {
  id: number
  orderNumber: string
  customer: string
  customerEmail: string
  date: string
  itemCount: number
  totalAmount: number
  status: 'Draft' | 'Pending' | 'Confirmed' | 'Delivered' | 'Cancelled'
  paymentStatus: 'Unpaid' | 'Partial' | 'Paid'
}

export interface Quotation {
  id: number
  quoteNumber: string
  customer: string
  date: string
  validUntil: string
  itemCount: number
  totalAmount: number
  status: 'Draft' | 'Sent' | 'Accepted' | 'Rejected' | 'Expired'
}

export interface SalesInvoice {
  id: number
  invoiceNumber: string
  customer: string
  orderReference: string
  date: string
  dueDate: string
  amount: number
  paid: number
  status: 'Draft' | 'Sent' | 'Paid' | 'Overdue'
}

export interface POSTransaction {
  id: number
  transactionId: string
  timestamp: Date
  items: Array<{ name: string; quantity: number; price: number }>
  subtotal: number
  tax: number
  total: number
  paymentMethod: 'Cash' | 'Card' | 'Mobile' | 'EFT'
  cashier: string
}

export interface Product {
  id: number
  name: string
  sku: string
  price: number
  stock: number
  category: string
  barcode?: string
}

const mockSalesOrders: SalesOrder[] = [
  { id: 1, orderNumber: 'SO-2024-001', customer: 'TechCorp Ltd', customerEmail: 'orders@techcorp.com', date: '2024-01-15', itemCount: 5, totalAmount: 45000, status: 'Delivered', paymentStatus: 'Paid' },
  { id: 2, orderNumber: 'SO-2024-002', customer: 'Retail Solutions', customerEmail: 'info@retail.com', date: '2024-01-20', itemCount: 8, totalAmount: 67800, status: 'Confirmed', paymentStatus: 'Partial' },
  { id: 3, orderNumber: 'SO-2024-003', customer: 'Metro Traders', customerEmail: 'sales@metro.com', date: '2024-02-01', itemCount: 3, totalAmount: 23400, status: 'Pending', paymentStatus: 'Unpaid' }
]

const mockQuotations: Quotation[] = [
  { id: 1, quoteNumber: 'QT-2024-001', customer: 'BigCorp Industries', date: '2024-01-10', validUntil: '2024-02-10', itemCount: 12, totalAmount: 125000, status: 'Sent' },
  { id: 2, quoteNumber: 'QT-2024-002', customer: 'StartUp Inc', date: '2024-01-18', validUntil: '2024-02-18', itemCount: 5, totalAmount: 34500, status: 'Accepted' },
  { id: 3, quoteNumber: 'QT-2024-003', customer: 'Local Business', date: '2024-02-02', validUntil: '2024-03-02', itemCount: 8, totalAmount: 56700, status: 'Draft' }
]

const mockSalesInvoices: SalesInvoice[] = [
  { id: 1, invoiceNumber: 'INV-2024-001', customer: 'TechCorp Ltd', orderReference: 'SO-2024-001', date: '2024-01-15', dueDate: '2024-02-14', amount: 45000, paid: 45000, status: 'Paid' },
  { id: 2, invoiceNumber: 'INV-2024-002', customer: 'Retail Solutions', orderReference: 'SO-2024-002', date: '2024-01-22', dueDate: '2024-02-21', amount: 67800, paid: 30000, status: 'Sent' },
  { id: 3, invoiceNumber: 'INV-2024-003', customer: 'Metro Traders', orderReference: 'SO-2024-003', date: '2024-02-01', dueDate: '2024-01-25', amount: 23400, paid: 0, status: 'Overdue' }
]

const mockPOSProducts: Product[] = [
  { id: 1, name: 'Coca Cola 2L', sku: 'BEV-001', price: 35.00, stock: 24, category: 'Beverages', barcode: '6001234567890' },
  { id: 2, name: 'White Bread 700g', sku: 'GRO-001', price: 18.00, stock: 15, category: 'Groceries', barcode: '6002345678901' },
  { id: 3, name: 'Milk 1L', sku: 'GRO-002', price: 22.00, stock: 12, category: 'Groceries', barcode: '6003456789012' },
  { id: 4, name: 'Simba Chips 125g', sku: 'SNK-001', price: 12.00, stock: 30, category: 'Snacks', barcode: '6004567890123' },
  { id: 5, name: 'Sunlight Soap 250g', sku: 'HOU-001', price: 15.00, stock: 20, category: 'Household', barcode: '6005678901234' }
]

const mockPOSTransactions: POSTransaction[] = [
  {
    id: 1,
    transactionId: 'TXN-001',
    timestamp: new Date(Date.now() - 3600000),
    items: [
      { name: 'Coca Cola 2L', quantity: 2, price: 35.00 },
      { name: 'Simba Chips 125g', quantity: 3, price: 12.00 }
    ],
    subtotal: 106.00,
    tax: 15.90,
    total: 121.90,
    paymentMethod: 'Cash',
    cashier: 'John Doe'
  },
  {
    id: 2,
    transactionId: 'TXN-002',
    timestamp: new Date(Date.now() - 7200000),
    items: [
      { name: 'White Bread 700g', quantity: 5, price: 18.00 },
      { name: 'Milk 1L', quantity: 3, price: 22.00 }
    ],
    subtotal: 156.00,
    tax: 23.40,
    total: 179.40,
    paymentMethod: 'Card',
    cashier: 'Sarah Smith'
  }
]

export class MockSalesService {
  static getOrders(): SalesOrder[] {
    return mockSalesOrders
  }

  static getQuotations(): Quotation[] {
    return mockQuotations
  }

  static getInvoices(): SalesInvoice[] {
    return mockSalesInvoices
  }

  static getPOSProducts(): Product[] {
    return mockPOSProducts
  }

  static getPOSTransactions(): POSTransaction[] {
    return mockPOSTransactions
  }

  static createSalesOrder(order: Omit<SalesOrder, 'id' | 'orderNumber'>): SalesOrder {
    const newOrder: SalesOrder = {
      ...order,
      id: mockSalesOrders.length + 1,
      orderNumber: `SO-2024-${String(mockSalesOrders.length + 1).padStart(3, '0')}`
    }
    mockSalesOrders.unshift(newOrder)
    return newOrder
  }

  static createPOSTransaction(transaction: Omit<POSTransaction, 'id' | 'transactionId' | 'timestamp'>): POSTransaction {
    const newTransaction: POSTransaction = {
      ...transaction,
      id: mockPOSTransactions.length + 1,
      transactionId: `TXN-${String(mockPOSTransactions.length + 1).padStart(3, '0')}`,
      timestamp: new Date()
    }
    mockPOSTransactions.unshift(newTransaction)
    return newTransaction
  }

  static getSalesMetrics() {
    return {
      totalOrders: mockSalesOrders.length,
      totalRevenue: mockSalesOrders.reduce((sum, order) => sum + order.totalAmount, 0),
      pendingOrders: mockSalesOrders.filter(o => o.status === 'Pending').length,
      completedOrders: mockSalesOrders.filter(o => o.status === 'Delivered').length
    }
  }

  static getPOSMetrics() {
    return {
      todayTransactions: mockPOSTransactions.length,
      todayRevenue: mockPOSTransactions.reduce((sum, txn) => sum + txn.total, 0),
      avgTransactionValue: mockPOSTransactions.reduce((sum, txn) => sum + txn.total, 0) / mockPOSTransactions.length,
      cashPayments: mockPOSTransactions.filter(t => t.paymentMethod === 'Cash').length
    }
  }
}

