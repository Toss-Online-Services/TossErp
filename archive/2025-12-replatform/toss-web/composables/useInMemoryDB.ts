import { ref } from 'vue'

// In-Memory Database - Simulates backend storage
// This will be replaced with real API calls later

interface Product {
  id: number
  name: string
  sku: string
  price: number
  stock: number
  category: string
  image?: string | null
}

interface OrderItem {
  id: number
  name: string
  sku: string
  quantity: number
  price: number
  stock: number
}

interface Order {
  id: string
  orderNumber: string
  customer: string
  customerPhone?: string
  total: number
  orderItems: OrderItem[]
  status: string
  priority?: string
  orderDate: Date
  expectedDelivery?: Date
  deliveryAddress?: string
  notes?: string
  paymentMethod?: string
  type: 'sales' | 'buying'
}

interface Invoice {
  id: string
  invoiceNumber: string
  customer: string
  orderNumber: string
  total: number
  status: string
  dueDate: Date
  invoiceDate: Date
  type: 'sales' | 'buying'
  invoiceItems?: OrderItem[]
}

interface Supplier {
  id: string
  name: string
  contact: string
  email: string
  phone: string
  address: string
  category: string
  rating: number
}

interface GroupBuy {
  id: string
  productName: string
  category: string
  supplier: string
  targetQuantity: number
  currentQuantity: number
  currentParticipants: number
  unitPrice: number
  bulkPrice: number
  savings: number
  endsAt: Date
  status: string
  participants: Array<{ shopName: string; quantity: number }>
}

// Shared products database
const products = ref<Product[]>([
  { id: 1, name: 'Coca Cola 2L', sku: 'CC2L001', price: 35, stock: 24, category: 'beverages', image: null },
  { id: 2, name: 'White Bread 700g', sku: 'WB700', price: 18, stock: 14, category: 'groceries', image: null },
  { id: 3, name: 'Milk 1L', sku: 'MLK1L', price: 22, stock: 11, category: 'groceries', image: null },
  { id: 4, name: 'Simba Chips 125g', sku: 'SC125', price: 12, stock: 30, category: 'snacks', image: null },
  { id: 5, name: 'Sunlight Soap 250g', sku: 'SS250', price: 15, stock: 8, category: 'household', image: null },
  { id: 6, name: 'Maggi 2-Minute Noodles', sku: 'MGN2M', price: 8, stock: 45, category: 'groceries', image: null },
  { id: 7, name: 'Castle Lager 440ml', sku: 'CL440', price: 25, stock: 0, category: 'beverages', image: null },
  { id: 8, name: 'Purity Baby Food', sku: 'PBF001', price: 45, stock: 12, category: 'groceries', image: null }
])

// Sales Orders
const salesOrders = ref<Order[]>([
  {
    id: '1',
    orderNumber: 'SO-2025-001',
    customer: 'John Doe',
    customerPhone: '+27 82 456 7890',
    total: 4850,
    orderItems: [
      { id: 1, name: 'Coca Cola 2L', sku: 'CC2L001', quantity: 50, price: 35.00, stock: 24 },
      { id: 2, name: 'White Bread 700g', sku: 'WB700', quantity: 100, price: 18.00, stock: 14 },
      { id: 3, name: 'Milk 1L', sku: 'MLK1L', quantity: 40, price: 22.00, stock: 11 }
    ],
    status: 'in-progress',
    priority: 'urgent',
    orderDate: new Date(),
    expectedDelivery: new Date(Date.now() + 2 * 60 * 60 * 1000),
    deliveryAddress: '123 Main Street, Soweto',
    notes: 'Needed for lunch service - please deliver before 11 AM',
    type: 'sales'
  },
  {
    id: '2',
    orderNumber: 'SO-2025-002', 
    customer: 'Sarah Smith',
    customerPhone: '+27 73 123 4567',
    total: 1250,
    orderItems: [
      { id: 4, name: 'Simba Chips 125g', sku: 'SC125', quantity: 30, price: 12.00, stock: 30 },
      { id: 5, name: 'Sunlight Soap 250g', sku: 'SS250', quantity: 20, price: 15.00, stock: 8 },
      { id: 6, name: 'Maggi 2-Minute Noodles', sku: 'MGN2M', quantity: 50, price: 8.00, stock: 45 }
    ],
    status: 'ready',
    priority: 'normal',
    orderDate: new Date(Date.now() - 2 * 60 * 60 * 1000),
    expectedDelivery: new Date(Date.now() + 1 * 60 * 60 * 1000),
    deliveryAddress: '456 Park Avenue, Alexandra',
    notes: 'Customer will collect in person',
    type: 'sales'
  }
])

// Buying Orders
const buyingOrders = ref<Order[]>([
  {
    id: 'B1',
    orderNumber: 'PO-2025-001',
    customer: 'Coca Cola Distributors',
    customerPhone: '+27 11 123 4567',
    total: 8500,
    orderItems: [
      { id: 1, name: 'Coca Cola 2L', sku: 'CC2L001', quantity: 200, price: 28.00, stock: 24 },
      { id: 7, name: 'Castle Lager 440ml', sku: 'CL440', quantity: 100, price: 20.00, stock: 0 }
    ],
    status: 'pending',
    priority: 'normal',
    orderDate: new Date(),
    expectedDelivery: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000),
    notes: 'Weekly stock replenishment',
    type: 'buying'
  },
  {
    id: 'B2',
    orderNumber: 'PO-2025-002',
    customer: 'Fresh Bakery Supplies',
    customerPhone: '+27 11 987 6543',
    total: 3600,
    orderItems: [
      { id: 2, name: 'White Bread 700g', sku: 'WB700', quantity: 200, price: 14.00, stock: 14 }
    ],
    status: 'approved',
    priority: 'normal',
    orderDate: new Date(Date.now() - 24 * 60 * 60 * 1000),
    expectedDelivery: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
    notes: 'Daily delivery',
    type: 'buying'
  }
])

// Invoices
const invoices = ref<Invoice[]>([
  {
    id: 'INV1',
    invoiceNumber: 'INV-2025-001',
    customer: 'John Doe',
    orderNumber: 'SO-2025-001',
    total: 4850,
    status: 'sent',
    invoiceDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    invoiceItems: [
      { id: 1, name: 'Coca Cola 2L', sku: 'CC2L001', quantity: 50, price: 35.00, stock: 24 },
      { id: 2, name: 'White Bread 700g', sku: 'WB700', quantity: 100, price: 18.00, stock: 14 },
      { id: 3, name: 'Milk 1L', sku: 'MLK1L', quantity: 40, price: 22.00, stock: 11 }
    ]
  },
  {
    id: 'INV2',
    invoiceNumber: 'INV-2025-002',
    customer: 'Sarah Smith',
    orderNumber: 'SO-2025-002',
    total: 1250,
    status: 'paid',
    invoiceDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
    dueDate: new Date(Date.now() + 25 * 24 * 60 * 60 * 1000),
    invoiceItems: [
      { id: 4, name: 'Simba Chips 125g', sku: 'SC125', quantity: 30, price: 12.00, stock: 30 },
      { id: 5, name: 'Sunlight Soap 250g', sku: 'SS250', quantity: 20, price: 15.00, stock: 8 }
    ]
  },
  {
    id: 'INV3',
    invoiceNumber: 'INV-2025-003',
    customer: 'Mike Johnson',
    orderNumber: '',
    total: 890,
    status: 'overdue',
    invoiceDate: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000),
    dueDate: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
    invoiceItems: [
      { id: 7, name: 'Castle Lager 440ml', sku: 'CL440', quantity: 24, price: 25.00, stock: 0 },
      { id: 8, name: 'Purity Baby Food', sku: 'PBF001', quantity: 10, price: 45.00, stock: 12 }
    ]
  },
  {
    id: 'INV4',
    invoiceNumber: 'INV-2025-004',
    customer: 'Emily Davis',
    orderNumber: '',
    total: 3200,
    status: 'draft',
    invoiceDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    invoiceItems: [
      { id: 3, name: 'Milk 1L', sku: 'MLK1L', quantity: 50, price: 22.00, stock: 11 },
      { id: 4, name: 'Simba Chips 125g', sku: 'SC125', quantity: 100, price: 12.00, stock: 30 }
    ]
  }
])

// Sales Invoices
const salesInvoices = ref<Invoice[]>([
  {
    id: 'SI1',
    invoiceNumber: 'INV-S-2025-001',
    customer: 'John Doe',
    orderNumber: 'SO-2025-001',
    total: 4850,
    status: 'sent',
    invoiceDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    type: 'sales',
    invoiceItems: [
      { id: 1, name: 'Coca Cola 2L', sku: 'CC2L001', quantity: 50, price: 35.00, stock: 24 },
      { id: 2, name: 'White Bread 700g', sku: 'WB700', quantity: 100, price: 18.00, stock: 14 },
      { id: 3, name: 'Milk 1L', sku: 'MLK1L', quantity: 40, price: 22.00, stock: 11 }
    ]
  },
  {
    id: 'SI2',
    invoiceNumber: 'INV-S-2025-002',
    customer: 'Sarah Smith',
    orderNumber: 'SO-2025-002',
    total: 1250,
    status: 'paid',
    invoiceDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
    dueDate: new Date(Date.now() + 25 * 24 * 60 * 60 * 1000),
    type: 'sales',
    invoiceItems: [
      { id: 4, name: 'Simba Chips 125g', sku: 'SC125', quantity: 30, price: 12.00, stock: 30 },
      { id: 5, name: 'Sunlight Soap 250g', sku: 'SS250', quantity: 20, price: 15.00, stock: 8 }
    ]
  },
  {
    id: 'SI3',
    invoiceNumber: 'INV-S-2025-003',
    customer: 'Mike Johnson',
    orderNumber: '',
    total: 890,
    status: 'overdue',
    invoiceDate: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000),
    dueDate: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
    type: 'sales',
    invoiceItems: [
      { id: 7, name: 'Castle Lager 440ml', sku: 'CL440', quantity: 24, price: 25.00, stock: 0 },
      { id: 8, name: 'Purity Baby Food', sku: 'PBF001', quantity: 10, price: 45.00, stock: 12 }
    ]
  },
  {
    id: 'SI4',
    invoiceNumber: 'INV-S-2025-004',
    customer: 'Emily Davis',
    orderNumber: '',
    total: 3200,
    status: 'draft',
    invoiceDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    type: 'sales',
    invoiceItems: [
      { id: 3, name: 'Milk 1L', sku: 'MLK1L', quantity: 50, price: 22.00, stock: 11 },
      { id: 4, name: 'Simba Chips 125g', sku: 'SC125', quantity: 100, price: 12.00, stock: 30 }
    ]
  }
])

// Buying Invoices
const buyingInvoices = ref<Invoice[]>([
  {
    id: 'BI1',
    invoiceNumber: 'INV-B-2025-001',
    customer: 'Coca Cola Distributors',
    orderNumber: 'PO-2025-001',
    total: 8500,
    status: 'pending',
    invoiceDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    type: 'buying'
  }
])

// Suppliers
const suppliers = ref<Supplier[]>([
  {
    id: 'SUP1',
    name: 'Coca Cola Distributors',
    contact: 'James Smith',
    email: 'james@cocacola.co.za',
    phone: '+27 11 123 4567',
    address: '123 Industrial Rd, Johannesburg',
    category: 'Beverages',
    rating: 4.8
  },
  {
    id: 'SUP2',
    name: 'Fresh Bakery Supplies',
    contact: 'Mary Johnson',
    email: 'mary@freshbakery.co.za',
    phone: '+27 11 987 6543',
    address: '456 Baker St, Pretoria',
    category: 'Groceries',
    rating: 4.5
  },
  {
    id: 'SUP3',
    name: 'Wholesale Foods SA',
    contact: 'Peter Williams',
    email: 'peter@wholesalefoods.co.za',
    phone: '+27 11 555 7890',
    address: '789 Trade Ave, Sandton',
    category: 'Groceries',
    rating: 4.6
  }
])

// Group Buys
const groupBuys = ref<GroupBuy[]>([
  {
    id: 'GB1',
    productName: 'Coca Cola 2L - Bulk Order',
    category: 'Beverages',
    supplier: 'Coca Cola Distributors',
    targetQuantity: 500,
    currentQuantity: 320,
    currentParticipants: 8,
    unitPrice: 35.00,
    bulkPrice: 28.00,
    savings: 7.00,
    endsAt: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
    status: 'active',
    participants: [
      { shopName: 'My Shop', quantity: 50 },
      { shopName: 'Corner Store', quantity: 100 },
      { shopName: 'Quick Mart', quantity: 80 }
    ]
  }
])

// Customers
const customers = ref([
  { id: 1, name: 'John Doe', phone: '+27 82 456 7890', email: 'john@example.com' },
  { id: 2, name: 'Sarah Smith', phone: '+27 73 123 4567', email: 'sarah@example.com' },
  { id: 3, name: 'Mike Johnson', phone: '+27 76 345 6789', email: 'mike@example.com' }
])

export const useInMemoryDB = () => {
  // Helper to generate IDs
  const generateId = (prefix: string = '') => {
    return `${prefix}${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
  }

  const generateOrderNumber = (type: 'sales' | 'buying') => {
    const prefix = type === 'sales' ? 'SO' : 'PO'
    const year = new Date().getFullYear()
    const orders = type === 'sales' ? salesOrders.value : buyingOrders.value
    const count = orders.length + 1
    return `${prefix}-${year}-${String(count).padStart(3, '0')}`
  }

  const generateInvoiceNumber = (type: 'sales' | 'buying') => {
    const prefix = type === 'sales' ? 'INV-S' : 'INV-B'
    const year = new Date().getFullYear()
    const invoices = type === 'sales' ? salesInvoices.value : buyingInvoices.value
    const count = invoices.length + 1
    return `${prefix}-${year}-${String(count).padStart(3, '0')}`
  }

  return {
    // Data stores
    products,
    salesOrders,
    buyingOrders,
    salesInvoices,
    buyingInvoices,
    suppliers,
    groupBuys,
    customers,

    // Helpers
    generateId,
    generateOrderNumber,
    generateInvoiceNumber
  }
}

