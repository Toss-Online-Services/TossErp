/**
 * Mock Sales Data Service
 * Simulates sales orders, quotations, invoices, and POS transactions
 */

type QuotationStatus = 'draft' | 'sent' | 'accepted' | 'rejected' | 'expired'

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

export interface QuotationSummary {
  id: number
  quotationNo: string
  customerId: number
  customerName: string
  date: string
  validUntil: string
  itemCount: number
  totalAmount: number
  status: QuotationStatus
}

export interface QuotationCustomer {
  businessName?: string
  address: string
  phone: string
  email?: string
}

export interface QuotationItem {
  id: number
  productName: string
  description: string
  quantity: number
  rate: number
  discountPercent: number
  amount: number
}

export interface QuotationActivity {
  id: number
  type: 'created' | 'sent' | 'accepted' | 'rejected' | 'converted'
  title: string
  description: string
  user: string
  timestamp: string
}

export interface QuotationDetail extends QuotationSummary {
  priceList: 'standard' | 'wholesale' | 'bulk'
  customer: QuotationCustomer
  items: QuotationItem[]
  subtotal: number
  discountAmount: number
  taxableAmount: number
  taxAmount: number
  grandTotal: number
  customerPoNumber?: string
  termsAndConditions?: string
  notes?: string
  createdAt: string
  activities: QuotationActivity[]
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

export interface SalesCustomer {
  id: number
  name: string
  businessName?: string
  email?: string
  phone: string
  creditLimit: number
  balance: number
  address: string
}

const clone = <T>(value: T): T => JSON.parse(JSON.stringify(value))

const mockSalesOrders: SalesOrder[] = [
  { id: 1, orderNumber: 'SO-2024-001', customer: 'TechCorp Ltd', customerEmail: 'orders@techcorp.com', date: '2024-01-15', itemCount: 5, totalAmount: 45000, status: 'Delivered', paymentStatus: 'Paid' },
  { id: 2, orderNumber: 'SO-2024-002', customer: 'Retail Solutions', customerEmail: 'info@retail.com', date: '2024-01-20', itemCount: 8, totalAmount: 67800, status: 'Confirmed', paymentStatus: 'Partial' },
  { id: 3, orderNumber: 'SO-2024-003', customer: 'Metro Traders', customerEmail: 'sales@metro.com', date: '2024-02-01', itemCount: 3, totalAmount: 23400, status: 'Pending', paymentStatus: 'Unpaid' }
]

const mockQuotations: QuotationSummary[] = []

const mockCustomers: SalesCustomer[] = [
  {
    id: 1,
    name: 'Mama Dlamini Spaza',
    businessName: 'Mama Dlamini Spaza Shop',
    email: 'mama@example.com',
    phone: '082 123 4567',
    creditLimit: 5000,
    balance: 1250,
    address: '45 Main Road, Soweto, Johannesburg 1800'
  },
  {
    id: 2,
    name: 'Sibusiso Butchery',
    businessName: 'Sibusiso Family Butchery',
    email: 'orders@sibusisobutchery.co.za',
    phone: '083 234 5678',
    creditLimit: 10000,
    balance: 0,
    address: '12 Market Street, Umlazi, Durban 4066'
  },
  {
    id: 3,
    name: 'Thandi Hair Salon',
    businessName: 'Thandi Glam Studio',
    email: 'thandi@glamstudio.co.za',
    phone: '084 345 6789',
    creditLimit: 3000,
    balance: 750,
    address: '27 Makhaya Street, Khayelitsha, Cape Town 7784'
  }
]

const syncQuotationSummary = (detail: QuotationDetail) => {
  const summary: QuotationSummary = {
    id: detail.id,
    quotationNo: detail.quotationNo,
    customerId: detail.customerId,
    customerName: detail.customerName,
    date: detail.date,
    validUntil: detail.validUntil,
    itemCount: detail.items.length,
    totalAmount: detail.grandTotal,
    status: detail.status
  }

  const index = mockQuotations.findIndex(q => q.id === detail.id)
  if (index === -1) {
    mockQuotations.push(summary)
  } else {
    mockQuotations[index] = summary
  }
}

const mockQuotationDetails: QuotationDetail[] = [
  {
    id: 1,
    quotationNo: 'QTN-2025-001',
    customerId: 1,
    customerName: 'Mama Dlamini Spaza',
    date: '2025-01-10',
    validUntil: '2025-01-31',
    status: 'sent',
    itemCount: 3,
    totalAmount: 2901.51,
    priceList: 'standard',
    customer: {
      businessName: 'Mama Dlamini Spaza Shop',
      address: '45 Main Road, Soweto, Johannesburg 1800',
      phone: '082 123 4567',
      email: 'mama@example.com'
    },
    items: [
      {
        id: 1,
        productName: 'White Bread',
        description: 'Albany White Bread 700g',
        quantity: 50,
        rate: 15.99,
        discountPercent: 5,
        amount: 759.53
      },
      {
        id: 2,
        productName: 'Milk 2L',
        description: 'Full Cream Milk 2L',
        quantity: 30,
        rate: 32.99,
        discountPercent: 0,
        amount: 989.7
      },
      {
        id: 3,
        productName: 'Sugar 2.5kg',
        description: 'White Sugar 2.5kg',
        quantity: 20,
        rate: 42.99,
        discountPercent: 10,
        amount: 773.82
      }
    ],
    subtotal: 2562.95,
    discountAmount: 39.9,
    taxableAmount: 2523.05,
    taxAmount: 378.46,
    grandTotal: 2901.51,
    termsAndConditions: 'Payment due within 30 days\nDelivery within 5 business days\nPrices valid for 21 days',
    notes: 'First order - special bulk discount applied',
    createdAt: '2025-01-10T09:30:00Z',
    activities: [
      {
        id: 1,
        type: 'created',
        title: 'Quotation Created',
        description: 'Quotation was created as draft',
        user: 'John Doe',
        timestamp: '2025-01-10T09:30:00Z'
      },
      {
        id: 2,
        type: 'sent',
        title: 'Quotation Sent',
        description: 'Quotation emailed to customer',
        user: 'John Doe',
        timestamp: '2025-01-10T10:15:00Z'
      }
    ]
  },
  {
    id: 2,
    quotationNo: 'QTN-2025-002',
    customerId: 2,
    customerName: 'Sibusiso Butchery',
    date: '2025-02-05',
    validUntil: '2025-02-28',
    status: 'accepted',
    itemCount: 4,
    totalAmount: 4950.46,
    priceList: 'wholesale',
    customer: {
      businessName: 'Sibusiso Family Butchery',
      address: '12 Market Street, Umlazi, Durban 4066',
      phone: '083 234 5678',
      email: 'orders@sibusisobutchery.co.za'
    },
    items: [
      {
        id: 1,
        productName: 'Beef Sausage 5kg',
        description: 'Bulk pack - vacuum sealed',
        quantity: 10,
        rate: 120.5,
        discountPercent: 0,
        amount: 1205
      },
      {
        id: 2,
        productName: 'Chicken Portions 10kg',
        description: 'Mixed portions, frozen',
        quantity: 8,
        rate: 145.75,
        discountPercent: 5,
        amount: 1108.7
      },
      {
        id: 3,
        productName: 'Spice Mix Bulk',
        description: 'House blend spice mix 2kg',
        quantity: 6,
        rate: 210.99,
        discountPercent: 0,
        amount: 1265.94
      },
      {
        id: 4,
        productName: 'Butcher Paper Roll',
        description: 'Grease proof paper 100m',
        quantity: 5,
        rate: 168.22,
        discountPercent: 2,
        amount: 823.61
      }
    ],
    subtotal: 4403.25,
    discountAmount: 98.5,
    taxableAmount: 4304.75,
    taxAmount: 645.71,
    grandTotal: 4950.46,
    termsAndConditions: 'Prices valid for 14 days. 50% deposit on acceptance. Delivery within 3 working days.',
    notes: 'Customer requested weekend delivery slot.',
    createdAt: '2025-02-05T14:15:00Z',
    activities: [
      {
        id: 1,
        type: 'created',
        title: 'Quotation Created',
        description: 'Quotation captured from supplier price list',
        user: 'Thandi Nkosi',
        timestamp: '2025-02-05T14:15:00Z'
      },
      {
        id: 2,
        type: 'accepted',
        title: 'Quotation Accepted',
        description: 'Customer accepted quotation via WhatsApp confirmation',
        user: 'Sibusiso Butchery',
        timestamp: '2025-02-06T09:20:00Z'
      }
    ]
  }
]

mockQuotationDetails.forEach(syncQuotationSummary)

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

const getQuotationDetailRef = (id: number) => mockQuotationDetails.find(q => q.id === id)

export class MockSalesService {
  static getOrders(): SalesOrder[] {
    return clone(mockSalesOrders)
  }

  static getQuotations(): QuotationSummary[] {
    return clone(mockQuotations)
  }

  static getCustomers(): SalesCustomer[] {
    return clone(mockCustomers)
  }

  static getQuotationDetail(id: number): QuotationDetail | null {
    const detail = getQuotationDetailRef(id)
    return detail ? clone(detail) : null
  }

  static updateQuotation(id: number, payload: Partial<Omit<QuotationDetail, 'id' | 'activities'>>): QuotationDetail | null {
    const detail = getQuotationDetailRef(id)
    if (!detail) {
      return null
    }

    if (payload.customerId && payload.customerId !== detail.customerId) {
      const customer = mockCustomers.find(c => c.id === payload.customerId)
      if (customer) {
        detail.customerId = customer.id
        detail.customerName = customer.name
        detail.customer = {
          businessName: customer.businessName,
          address: customer.address,
          phone: customer.phone,
          email: customer.email
        }
      }
    }

    Object.assign(detail, payload)

    if (payload.items) {
      detail.items = payload.items.map((item, index) => ({ ...item, id: index + 1 }))
      detail.itemCount = detail.items.length
    }

    detail.totalAmount = detail.grandTotal
    syncQuotationSummary(detail)
    return clone(detail)
  }

  static markQuotationSent(id: number, user = 'Sales Agent'): QuotationDetail | null {
    const detail = getQuotationDetailRef(id)
    if (!detail) {
      return null
    }

    detail.status = 'sent'
    detail.activities.push({
      id: detail.activities.length + 1,
      type: 'sent',
      title: 'Quotation Sent',
      description: 'Quotation emailed to customer',
      user,
      timestamp: new Date().toISOString()
    })
    syncQuotationSummary(detail)
    return clone(detail)
  }

  static convertQuotationToOrder(id: number, user = 'Sales Agent') {
    const detail = getQuotationDetailRef(id)
    if (!detail) {
      return null
    }

    detail.status = 'accepted'
    detail.activities.push({
      id: detail.activities.length + 1,
      type: 'converted',
      title: 'Converted to Sales Order',
      description: 'Quotation converted to a confirmed sales order',
      user,
      timestamp: new Date().toISOString()
    })
    syncQuotationSummary(detail)

    const order = this.createSalesOrder({
      customer: detail.customerName,
      customerEmail: detail.customer.email || 'orders@unknown.local',
      date: new Date().toISOString().split('T')[0],
      itemCount: detail.items.length,
      totalAmount: detail.grandTotal,
      status: 'Pending',
      paymentStatus: 'Unpaid'
    })

    return { quotation: clone(detail), order }
  }

  static createQuotation(payload: {
    customerId: number
    quotationDate: string
    validUntil: string
    priceList: 'standard' | 'wholesale' | 'bulk'
    items: QuotationItem[]
    termsAndConditions?: string
    notes?: string
    customerPoNumber?: string
    totals: {
      subtotal: number
      discountAmount: number
      taxableAmount: number
      taxAmount: number
      grandTotal: number
    }
    status: QuotationStatus
  }): QuotationDetail {
    const customer = mockCustomers.find(c => c.id === payload.customerId)
    if (!customer) {
      throw new Error('Customer not found')
    }

    const newId = mockQuotationDetails.length + 1
    const quotationNo = `QTN-${new Date(payload.quotationDate).getFullYear()}-${String(newId).padStart(3, '0')}`

    const detail: QuotationDetail = {
      id: newId,
      quotationNo,
      customerId: customer.id,
      customerName: customer.name,
      date: payload.quotationDate,
      validUntil: payload.validUntil,
      status: payload.status,
      itemCount: payload.items.length,
      totalAmount: payload.totals.grandTotal,
      priceList: payload.priceList,
      customer: {
        businessName: customer.businessName,
        address: customer.address,
        phone: customer.phone,
        email: customer.email
      },
      items: payload.items.map((item, index) => ({ ...item, id: index + 1 })),
      subtotal: payload.totals.subtotal,
      discountAmount: payload.totals.discountAmount,
      taxableAmount: payload.totals.taxableAmount,
      taxAmount: payload.totals.taxAmount,
      grandTotal: payload.totals.grandTotal,
      customerPoNumber: payload.customerPoNumber,
      termsAndConditions: payload.termsAndConditions,
      notes: payload.notes,
      createdAt: new Date().toISOString(),
      activities: [
        {
          id: 1,
          type: 'created',
          title: 'Quotation Created',
          description: 'Quotation captured from sales module',
          user: 'Sales Agent',
          timestamp: new Date().toISOString()
        }
      ]
    }

    if (payload.status === 'sent') {
      detail.activities.push({
        id: 2,
        type: 'sent',
        title: 'Quotation Sent',
        description: 'Quotation emailed to customer',
        user: 'Sales Agent',
        timestamp: new Date().toISOString()
      })
    }

    mockQuotationDetails.unshift(detail)
    syncQuotationSummary(detail)
    return clone(detail)
  }

  static getInvoices(): SalesInvoice[] {
    return clone(mockSalesInvoices)
  }

  static getPOSProducts(): Product[] {
    return clone(mockPOSProducts)
  }

  static getPOSTransactions(): POSTransaction[] {
    return clone(mockPOSTransactions)
  }

  static createSalesOrder(order: Omit<SalesOrder, 'id' | 'orderNumber'>): SalesOrder {
    const newOrder: SalesOrder = {
      ...order,
      id: mockSalesOrders.length + 1,
      orderNumber: `SO-2024-${String(mockSalesOrders.length + 1).padStart(3, '0')}`
    }
    mockSalesOrders.unshift(newOrder)
    return clone(newOrder)
  }

  static createPOSTransaction(transaction: Omit<POSTransaction, 'id' | 'transactionId' | 'timestamp'>): POSTransaction {
    const newTransaction: POSTransaction = {
      ...transaction,
      id: mockPOSTransactions.length + 1,
      transactionId: `TXN-${String(mockPOSTransactions.length + 1).padStart(3, '0')}`,
      timestamp: new Date()
    }
    mockPOSTransactions.unshift(newTransaction)
    return clone(newTransaction)
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
    const transactionCount = mockPOSTransactions.length || 1
    const totalRevenue = mockPOSTransactions.reduce((sum, txn) => sum + txn.total, 0)
    return {
      todayTransactions: mockPOSTransactions.length,
      todayRevenue: totalRevenue,
      avgTransactionValue: totalRevenue / transactionCount,
      cashPayments: mockPOSTransactions.filter(t => t.paymentMethod === 'Cash').length
    }
  }
}

