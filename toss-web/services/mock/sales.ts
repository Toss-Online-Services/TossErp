import type {
  DeliveryNote,
  DeliveryNoteStatus,
  LoyaltyProgram,
  PosProfile,
  PosSession,
  PosSessionCashDrop,
  PosSessionExpense,
  PosSessionPaymentSummary,
  PricingRule,
  ProofOfDelivery,
  Quotation,
  QuotationCustomer,
  QuotationItem,
  QuotationStatus,
  SalesAnalyticsSnapshot,
  SalesInvoice,
  SalesInvoicePayment,
  SalesInvoiceStatus,
  SalesOrder,
  SalesOrderHistoryEntry,
  SalesOrderItem,
  SalesOrderPayment,
  SalesOrderStatus,
  SalesReturn,
  SalesReturnItem,
  SalesReturnStatus,
  PaymentStatus,
  PosSale,
  PosParkedSale
} from '~/types/sales'

const VAT_RATE_DEFAULT = 15
const BASE_YEAR = 2025

const round = (value: number) => Math.round((value + Number.EPSILON) * 100) / 100
const clone = <T>(value: T): T => JSON.parse(JSON.stringify(value))
const normalise = (value?: string) => value?.toLowerCase().trim() ?? ''
const nextSequence = (sequence: number) => String(sequence).padStart(3, '0')
const generateDocumentNumber = (prefix: string, sequence: number, date?: string) => {
  const year = date ? new Date(date).getFullYear() : BASE_YEAR
  return `${prefix}-${year}-${nextSequence(sequence)}`
}
const now = () => new Date().toISOString()

type QuotationItemInput = Omit<QuotationItem, 'id' | 'total' | 'discountAmount' | 'vatAmount'> & { id?: string }
type QuotationSeed = {
  id: string
  number: string
  status: QuotationStatus
  date: string
  validUntil: string
  customer: QuotationCustomer
  items: QuotationItemInput[]
  discountRate?: number
  vatRate?: number
  terms?: string
  notes?: string
  salesPerson?: string
  createdAt?: string
  updatedAt?: string
  lastSentAt?: string
  lastViewedAt?: string
  attachments?: string[]
  convertedToOrderId?: string
}

const toQuotationItemInput = (item: QuotationItem): QuotationItemInput => ({
  id: item.id,
  productId: item.productId,
  sku: item.sku,
  name: item.name,
  description: item.description,
  quantity: item.quantity,
  unitPrice: item.unitPrice,
  uom: item.uom,
  discountRate: item.discountRate,
  vatRate: item.vatRate
})

const buildQuotation = (seed: QuotationSeed): Quotation => {
  const vatRate = seed.vatRate ?? VAT_RATE_DEFAULT
  let subtotal = 0
  let itemDiscountSum = 0
  let totalVatAmount = 0

  const items: QuotationItem[] = seed.items.map((item, index) => {
    const id = item.id ?? `${seed.id}-line-${index + 1}`
    const lineBase = round(item.quantity * item.unitPrice)
    const discountRate = item.discountRate ?? 0
    const discountAmount = round(lineBase * discountRate / 100)
    const net = round(lineBase - discountAmount)
    const appliedVatRate = item.vatRate ?? vatRate
    const lineVat = round(net * appliedVatRate / 100)

    subtotal = round(subtotal + lineBase)
    itemDiscountSum = round(itemDiscountSum + discountAmount)
    totalVatAmount = round(totalVatAmount + lineVat)

    return {
      ...item,
      id,
      discountRate: discountRate || undefined,
      discountAmount: discountAmount || undefined,
      vatRate: appliedVatRate,
      vatAmount: lineVat,
      total: net
    }
  })

  let discountAmount = itemDiscountSum
  if (seed.discountRate) {
    const remaining = subtotal - itemDiscountSum
    discountAmount = round(discountAmount + (remaining * seed.discountRate) / 100)
  }

  const netSubtotal = round(subtotal - discountAmount)
  const vatAmount = totalVatAmount || round(netSubtotal * vatRate / 100)
  const grandTotal = round(netSubtotal + vatAmount)
  const createdAt = seed.createdAt ?? new Date(seed.date).toISOString()
  const updatedAt = seed.updatedAt ?? createdAt

  return {
    id: seed.id,
    number: seed.number,
    status: seed.status,
    date: seed.date,
    validUntil: seed.validUntil,
    salesPerson: seed.salesPerson,
    terms: seed.terms,
    notes: seed.notes,
    customer: seed.customer,
    items,
    subtotal,
    discountRate: seed.discountRate,
    discountAmount,
    vatRate,
    vatAmount,
    grandTotal,
    createdAt,
    updatedAt,
    convertedToOrderId: seed.convertedToOrderId,
    lastSentAt: seed.lastSentAt,
    lastViewedAt: seed.lastViewedAt,
    attachments: seed.attachments
  }
}

const recalculateQuotationTotals = (quotation: Quotation) => {
  const recomputed = buildQuotation({
    id: quotation.id,
    number: quotation.number,
    status: quotation.status,
    date: quotation.date,
    validUntil: quotation.validUntil,
    customer: quotation.customer,
    items: quotation.items.map(toQuotationItemInput),
    discountRate: quotation.discountRate,
    vatRate: quotation.vatRate,
    terms: quotation.terms,
    notes: quotation.notes,
    salesPerson: quotation.salesPerson,
    createdAt: quotation.createdAt,
    updatedAt: now(),
    lastSentAt: quotation.lastSentAt,
    lastViewedAt: quotation.lastViewedAt,
    attachments: quotation.attachments,
    convertedToOrderId: quotation.convertedToOrderId
  })
  Object.assign(quotation, recomputed)
}

type SalesOrderItemInput = Omit<SalesOrderItem, 'id' | 'total' | 'discountAmount' | 'vatAmount'> & { id?: string }
type SalesOrderSeed = {
  id: string
  number: string
  status: SalesOrderStatus
  paymentStatus?: PaymentStatus
  quotationId?: string
  customer: QuotationCustomer
  items: SalesOrderItemInput[]
  expectedDeliveryDate: string
  shippingAddress: string
  instructions?: string
  assignedDriverId?: string
  assignedDriverName?: string
  payments?: SalesOrderPayment[]
  notes?: string
  history?: SalesOrderHistoryEntry[]
  createdAt: string
  updatedAt: string
}

const toSalesOrderItemInput = (item: SalesOrderItem): SalesOrderItemInput => ({
  id: item.id,
  productId: item.productId,
  sku: item.sku,
  name: item.name,
  description: item.description,
  quantity: item.quantity,
  unitPrice: item.unitPrice,
  uom: item.uom,
  discountRate: item.discountRate,
  vatRate: item.vatRate,
  deliveredQuantity: item.deliveredQuantity,
  invoicedQuantity: item.invoicedQuantity,
  warehouse: item.warehouse
})

const deriveFulfillmentStatus = (status: SalesOrderStatus): SalesOrder['fulfillment']['status'] => {
  switch (status) {
    case 'completed':
      return 'delivered'
    case 'processing':
      return 'in_transit'
    case 'confirmed':
      return 'scheduled'
    case 'cancelled':
      return 'failed'
    default:
      return 'pending'
  }
}

const buildSalesOrder = (seed: SalesOrderSeed): SalesOrder => {
  let subtotal = 0
  let discountAmount = 0
  let vatAmount = 0

  const items: SalesOrderItem[] = seed.items.map((item, index) => {
    const id = item.id ?? `${seed.id}-line-${index + 1}`
    const lineBase = round(item.quantity * item.unitPrice)
    const itemDiscountRate = item.discountRate ?? 0
    const lineDiscount = round(lineBase * itemDiscountRate / 100)
    const net = round(lineBase - lineDiscount)
    const appliedVatRate = item.vatRate ?? VAT_RATE_DEFAULT
    const lineVat = round(net * appliedVatRate / 100)

    subtotal = round(subtotal + lineBase)
    discountAmount = round(discountAmount + lineDiscount)
    vatAmount = round(vatAmount + lineVat)

    return {
      ...item,
      id,
      discountRate: item.discountRate,
      discountAmount: lineDiscount || undefined,
      vatRate: appliedVatRate,
      vatAmount: lineVat,
      total: net
    }
  })

  const grandTotal = round(subtotal - discountAmount + vatAmount)
  const payments = seed.payments ?? []
  const paidAmount = round(
    payments.filter(payment => payment.status === 'completed').reduce((sum, payment) => sum + payment.amount, 0)
  )
  const outstandingBalance = round(grandTotal - paidAmount)
  const paymentStatus: PaymentStatus =
    seed.paymentStatus ?? (outstandingBalance <= 0 ? 'paid' : paidAmount > 0 ? 'partial' : 'unpaid')

  const fulfillmentStatus = deriveFulfillmentStatus(seed.status)

  const fulfillment: SalesOrder['fulfillment'] = {
    expectedDeliveryDate: seed.expectedDeliveryDate,
    deliveryWindow: undefined,
    shippingAddress: seed.shippingAddress,
    instructions: seed.instructions,
    assignedDriverId: seed.assignedDriverId,
    assignedDriverName: seed.assignedDriverName,
    status: fulfillmentStatus
  }

  const history =
    seed.history ?? [
      {
        id: `${seed.id}-history-1`,
        timestamp: seed.createdAt,
        action: 'created',
        user: 'System',
        notes: 'Sales order captured'
      }
    ]

  return {
    id: seed.id,
    number: seed.number,
    quotationId: seed.quotationId,
    customer: seed.customer,
    status: seed.status,
    paymentStatus,
    items,
    subtotal,
    discountAmount,
    vatAmount,
    grandTotal,
    outstandingBalance,
    fulfillment,
    payments,
    notes: seed.notes,
    createdAt: seed.createdAt,
    updatedAt: seed.updatedAt,
    history
  }
}

const recalculateSalesOrderTotals = (order: SalesOrder) => {
  let subtotal = 0
  let discount = 0
  let vatAmount = 0
  let grandTotal = 0

  order.items = order.items.map((item, index) => {
    const lineBase = round(item.quantity * item.unitPrice)
    const itemDiscountRate = item.discountRate ?? 0
    const lineDiscount = round(lineBase * itemDiscountRate / 100)
    const net = round(lineBase - lineDiscount)
    const appliedVatRate = item.vatRate ?? VAT_RATE_DEFAULT
    const lineVat = round(net * appliedVatRate / 100)

    subtotal = round(subtotal + lineBase)
    discount = round(discount + lineDiscount)
    vatAmount = round(vatAmount + lineVat)
    grandTotal = round(grandTotal + net + lineVat)

    return {
      ...item,
      id: item.id ?? `${order.id}-line-${index + 1}`,
      discountAmount: lineDiscount || undefined,
      vatRate: appliedVatRate,
      vatAmount: lineVat,
      total: net
    }
  })

  const paidAmount = round(
    order.payments.filter(payment => payment.status === 'completed').reduce((sum, payment) => sum + payment.amount, 0)
  )
  const outstandingBalance = round(grandTotal - paidAmount)

  order.subtotal = subtotal
  order.discountAmount = discount
  order.vatAmount = vatAmount
  order.grandTotal = grandTotal
  order.outstandingBalance = outstandingBalance
  order.paymentStatus =
    outstandingBalance <= 0 ? 'paid' : paidAmount > 0 ? 'partial' : 'unpaid'
  order.fulfillment.status = deriveFulfillmentStatus(order.status)
  order.updatedAt = now()
}

type DeliveryNoteItemInput = Omit<DeliveryNoteItem, 'id'> & { id?: string }
type DeliveryNoteSeed = {
  id: string
  number: string
  status: DeliveryNoteStatus
  salesOrderId?: string
  customer: QuotationCustomer
  scheduledDate: string
  shippingAddress: string
  items: DeliveryNoteItemInput[]
  billingAddress?: string
  driverId?: string
  driverName?: string
  vehicleNumber?: string
  instructions?: string
  proofOfDelivery?: ProofOfDelivery
  shippedAt?: string
  deliveredAt?: string
  createdAt: string
  updatedAt: string
}

const buildDeliveryNote = (seed: DeliveryNoteSeed): DeliveryNote => ({
  id: seed.id,
  number: seed.number,
  salesOrderId: seed.salesOrderId,
  customer: seed.customer,
  status: seed.status,
  scheduledDate: seed.scheduledDate,
  shippedAt: seed.shippedAt,
  deliveredAt: seed.deliveredAt,
  shippingAddress: seed.shippingAddress,
  billingAddress: seed.billingAddress,
  driverId: seed.driverId,
  driverName: seed.driverName,
  vehicleNumber: seed.vehicleNumber,
  instructions: seed.instructions,
  items: seed.items.map((item, index) => ({
    ...item,
    id: item.id ?? `${seed.id}-item-${index + 1}`
  })),
  proofOfDelivery: seed.proofOfDelivery,
  createdAt: seed.createdAt,
  updatedAt: seed.updatedAt
})

type SalesInvoiceItemInput = Omit<SalesOrderItem, 'id' | 'total' | 'discountAmount' | 'vatAmount'> & { id?: string }
type SalesInvoiceSeed = {
  id: string
  number: string
  status: SalesInvoiceStatus
  salesOrderId?: string
  customer: QuotationCustomer
  issueDate: string
  dueDate: string
  items: SalesInvoiceItemInput[]
  payments?: SalesInvoicePayment[]
  notes?: string
  createdAt: string
  updatedAt: string
}

const buildSalesInvoice = (seed: SalesInvoiceSeed): SalesInvoice => {
  let subtotal = 0
  let discount = 0
  let vatAmount = 0
  let grandTotal = 0

  const items: SalesOrderItem[] = seed.items.map((item, index) => {
    const id = item.id ?? `${seed.id}-line-${index + 1}`
    const lineBase = round(item.quantity * item.unitPrice)
    const itemDiscountRate = item.discountRate ?? 0
    const lineDiscount = round(lineBase * itemDiscountRate / 100)
    const net = round(lineBase - lineDiscount)
    const appliedVatRate = item.vatRate ?? VAT_RATE_DEFAULT
    const lineVat = round(net * appliedVatRate / 100)

    subtotal = round(subtotal + lineBase)
    discount = round(discount + lineDiscount)
    vatAmount = round(vatAmount + lineVat)
    grandTotal = round(grandTotal + net + lineVat)

    return {
      ...item,
      id,
      discountAmount: lineDiscount || undefined,
      vatRate: appliedVatRate,
      vatAmount: lineVat,
      total: net,
      deliveredQuantity: item.deliveredQuantity ?? item.quantity,
      invoicedQuantity: item.invoicedQuantity ?? item.quantity,
      warehouse: item.warehouse
    }
  })

  const payments = seed.payments ?? []
  const paidAmount = round(
    payments.filter(payment => payment.status === 'completed').reduce((sum, payment) => sum + payment.amount, 0)
  )
  const balanceDue = round(grandTotal - paidAmount)

  return {
    id: seed.id,
    number: seed.number,
    salesOrderId: seed.salesOrderId,
    customer: seed.customer,
    status: seed.status,
    issueDate: seed.issueDate,
    dueDate: seed.dueDate,
    items,
    subtotal,
    discountAmount: discount,
    vatAmount,
    total: grandTotal,
    paidAmount,
    balanceDue,
    payments,
    notes: seed.notes,
    createdAt: seed.createdAt,
    updatedAt: seed.updatedAt
  }
}

const recalculateInvoiceTotals = (invoice: SalesInvoice) => {
  const recomputed = buildSalesInvoice({
    id: invoice.id,
    number: invoice.number,
    status: invoice.status,
    salesOrderId: invoice.salesOrderId,
    customer: invoice.customer,
    issueDate: invoice.issueDate,
    dueDate: invoice.dueDate,
    items: invoice.items.map(item => ({
      id: item.id,
      productId: item.productId,
      sku: item.sku,
      name: item.name,
      description: item.description,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      uom: item.uom,
      discountRate: item.discountRate,
      vatRate: item.vatRate,
      deliveredQuantity: item.deliveredQuantity,
      invoicedQuantity: item.invoicedQuantity,
      warehouse: item.warehouse
    })),
    payments: invoice.payments,
    notes: invoice.notes,
    createdAt: invoice.createdAt,
    updatedAt: now()
  })
  Object.assign(invoice, recomputed)
}

type SalesReturnItemInput = Omit<SalesReturnItem, 'id' | 'lineTotal'> & { id?: string }
type SalesReturnSeed = {
  id: string
  number: string
  status: SalesReturnStatus
  invoiceId: string
  customer: QuotationCustomer
  items: SalesReturnItemInput[]
  refundMethod: SalesReturn['refundMethod']
  notes?: string
  warehouseAction: SalesReturn['warehouseAction']
  requestedAt: string
  approvedAt?: string
  inspectedAt?: string
  completedAt?: string
  attachments?: string[]
  createdAt: string
  updatedAt: string
}

const buildSalesReturn = (seed: SalesReturnSeed): SalesReturn => {
  let refundAmount = 0
  const items: SalesReturnItem[] = seed.items.map((item, index) => {
    const id = item.id ?? `${seed.id}-item-${index + 1}`
    const lineTotal = round(item.quantity * item.unitPrice)
    refundAmount = round(refundAmount + lineTotal)
    return {
      ...item,
      id,
      lineTotal
    }
  })

  return {
    id: seed.id,
    number: seed.number,
    status: seed.status,
    invoiceId: seed.invoiceId,
    customer: seed.customer,
    requestedAt: seed.requestedAt,
    approvedAt: seed.approvedAt,
    inspectedAt: seed.inspectedAt,
    completedAt: seed.completedAt,
    items,
    refundMethod: seed.refundMethod,
    refundAmount,
    warehouseAction: seed.warehouseAction,
    notes: seed.notes,
    attachments: seed.attachments,
    createdAt: seed.createdAt,
    updatedAt: seed.updatedAt
  }
}

const customers: Record<string, QuotationCustomer> = {
  jabu: {
    id: 'cust-001',
    name: "Jabu's Spaza",
    email: 'jabu@spaza.co.za',
    phone: '+27 78 123 4567',
    address: '123 Main Road, Soweto, 1804',
    billingAddress: '123 Main Road, Soweto, 1804',
    shippingAddress: '123 Main Road, Soweto, 1804',
    territory: 'Gauteng / Soweto',
    customerGroup: 'stokvel',
    creditLimit: 25000,
    creditUsed: 6450,
    paymentTerms: 30,
    primaryContact: 'Jabu Dlamini'
  },
  sipho: {
    id: 'cust-002',
    name: "Sipho's Tavern",
    email: 'sipho@tavern.co.za',
    phone: '+27 73 555 9012',
    address: '45 Vilakazi Street, Orlando West, Soweto',
    billingAddress: '45 Vilakazi Street, Orlando West, Soweto',
    shippingAddress: '45 Vilakazi Street, Orlando West, Soweto',
    territory: 'Gauteng / Soweto',
    customerGroup: 'business',
    creditLimit: 15000,
    creditUsed: 2800,
    paymentTerms: 14,
    primaryContact: 'Sipho Mokoena'
  },
  gogo: {
    id: 'cust-003',
    name: 'The Gogo Shop',
    email: 'orders@gogoshop.co.za',
    phone: '+27 82 222 3344',
    address: '12 Tembisa Plaza, Tembisa',
    billingAddress: '12 Tembisa Plaza, Tembisa',
    shippingAddress: '12 Tembisa Plaza, Tembisa',
    territory: 'Gauteng / Ekurhuleni',
    customerGroup: 'individual',
    creditLimit: 8000,
    creditUsed: 1200,
    paymentTerms: 7,
    primaryContact: 'Gogo Ndlovu'
  },
  metro: {
    id: 'cust-004',
    name: 'Metro Traders',
    email: 'purchasing@metrotraders.co.za',
    phone: '+27 11 456 7890',
    address: '18 Commissioner Street, Johannesburg CBD',
    billingAddress: '18 Commissioner Street, Johannesburg CBD',
    shippingAddress: 'Plot 16, Midrand Industrial Park',
    territory: 'Gauteng / Johannesburg',
    customerGroup: 'wholesale',
    creditLimit: 120000,
    creditUsed: 48000,
    paymentTerms: 45,
    primaryContact: 'Naledi Khumalo'
  }
}

const mockQuotations: Quotation[] = [
  buildQuotation({
    id: 'qt-2025-001',
    number: 'QT-2025-001',
    status: 'sent',
    date: '2025-11-10',
    validUntil: '2025-11-24',
    customer: customers.jabu,
    items: [
      { productId: 'prod-bread-700', sku: 'BREAD-700', name: 'White Bread 700g', quantity: 20, unitPrice: 15.5, uom: 'unit' },
      { productId: 'prod-maize-5', sku: 'MAIZE-5KG', name: 'Maize Meal 5kg', quantity: 10, unitPrice: 45, uom: 'bag' },
      { productId: 'prod-oil-2l', sku: 'OIL-2L', name: 'Cooking Oil 2L', quantity: 15, unitPrice: 65.75, uom: 'bottle' }
    ],
    discountRate: 5,
    vatRate: 15,
    terms: 'Payment due within 30 days. Prices valid for 14 days.',
    notes: 'Customer requested early morning delivery.',
    salesPerson: 'Ayanda Nkosi',
    createdAt: '2025-11-10T09:12:00.000Z',
    updatedAt: '2025-11-11T08:42:00.000Z',
    lastSentAt: '2025-11-10T10:00:00.000Z'
  }),
  buildQuotation({
    id: 'qt-2025-002',
    number: 'QT-2025-002',
    status: 'accepted',
    date: '2025-11-08',
    validUntil: '2025-11-22',
    customer: customers.sipho,
    items: [
      { productId: 'prod-lager-500', sku: 'LAGER-500', name: 'Local Lager 500ml Case', quantity: 25, unitPrice: 235, uom: 'case', discountRate: 3 },
      { productId: 'prod-soda-330', sku: 'SODA-330', name: 'Assorted Soft Drinks 330ml Case', quantity: 18, unitPrice: 180, uom: 'case' }
    ],
    vatRate: 15,
    terms: 'Deposit of 50% required before delivery.',
    notes: 'Preferred Friday afternoon delivery.',
    salesPerson: 'Nokuthula Sithole',
    createdAt: '2025-11-08T13:30:00.000Z',
    updatedAt: '2025-11-09T09:18:00.000Z',
    convertedToOrderId: 'so-2025-001'
  }),
  buildQuotation({
    id: 'qt-2025-003',
    number: 'QT-2025-003',
    status: 'draft',
    date: '2025-11-12',
    validUntil: '2025-11-26',
    customer: customers.gogo,
    items: [
      { productId: 'prod-flour-12', sku: 'FLOUR-12.5', name: 'Cake Flour 12.5kg', quantity: 6, unitPrice: 128, uom: 'bag' },
      { productId: 'prod-sugar-10', sku: 'SUGAR-10', name: 'White Sugar 10kg', quantity: 5, unitPrice: 165, uom: 'bag' }
    ],
    vatRate: 15,
    salesPerson: 'Ayanda Nkosi',
    createdAt: '2025-11-12T07:20:00.000Z',
    updatedAt: '2025-11-12T07:20:00.000Z'
  })
]

const mockSalesOrders: SalesOrder[] = [
  buildSalesOrder({
    id: 'so-2025-001',
    number: 'SO-2025-001',
    status: 'completed',
    paymentStatus: 'paid',
    quotationId: 'qt-2025-002',
    customer: customers.sipho,
    items: [
      { productId: 'prod-lager-500', sku: 'LAGER-500', name: 'Local Lager 500ml Case', quantity: 25, unitPrice: 235, uom: 'case', discountRate: 3, vatRate: 15, deliveredQuantity: 25, invoicedQuantity: 25, warehouse: 'WH-JHB-01' },
      { productId: 'prod-soda-330', sku: 'SODA-330', name: 'Assorted Soft Drinks 330ml Case', quantity: 18, unitPrice: 180, uom: 'case', vatRate: 15, deliveredQuantity: 18, invoicedQuantity: 18, warehouse: 'WH-JHB-01' }
    ],
    expectedDeliveryDate: '2025-11-10',
    shippingAddress: customers.sipho.shippingAddress ?? customers.sipho.address ?? '',
    instructions: 'Deliver between 14:00 - 16:00. Offload at back entrance.',
    assignedDriverId: 'drv-001',
    assignedDriverName: 'Lerato M',
    payments: [
      { id: 'pay-so-2025-001-1', method: 'card', amount: 6500, status: 'completed', reference: 'POS-2025-1109', paidAt: '2025-11-09T16:45:00.000Z' }
    ],
    notes: 'Customer paid via card on delivery.',
    createdAt: '2025-11-08T13:35:00.000Z',
    updatedAt: '2025-11-10T18:12:00.000Z'
  }),
  buildSalesOrder({
    id: 'so-2025-002',
    number: 'SO-2025-002',
    status: 'processing',
    paymentStatus: 'partial',
    quotationId: 'qt-2025-001',
    customer: customers.jabu,
    items: mockQuotations[0].items.map(item => ({
      productId: item.productId,
      sku: item.sku,
      name: item.name,
      description: item.description,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      uom: item.uom,
      discountRate: item.discountRate,
      vatRate: item.vatRate,
      deliveredQuantity: item.quantity / 2,
      invoicedQuantity: 0,
      warehouse: 'WH-JHB-01'
    })),
    expectedDeliveryDate: '2025-11-13',
    shippingAddress: customers.jabu.shippingAddress ?? customers.jabu.address ?? '',
    instructions: 'Contact Jabu on arrival. Use side entrance.',
    assignedDriverId: 'drv-002',
    assignedDriverName: 'Thabo S',
    payments: [
      { id: 'pay-so-2025-002-1', method: 'cash', amount: 900, status: 'completed', reference: 'CASH-2025-1110', paidAt: '2025-11-10T12:00:00.000Z' }
    ],
    createdAt: '2025-11-10T09:20:00.000Z',
    updatedAt: '2025-11-12T06:45:00.000Z',
    notes: 'Balance due on delivery completion.'
  })
]

const mockDeliveryNotes: DeliveryNote[] = [
  buildDeliveryNote({
    id: 'dn-2025-001',
    number: 'DN-2025-001',
    status: 'completed',
    salesOrderId: 'so-2025-001',
    customer: customers.sipho,
    scheduledDate: '2025-11-10',
    shippingAddress: customers.sipho.shippingAddress ?? '',
    items: [
      { salesOrderItemId: 'so-2025-001-line-1', productId: 'prod-lager-500', name: 'Local Lager 500ml Case', orderedQuantity: 25, deliveredQuantity: 25, uom: 'case', warehouse: 'WH-JHB-01' },
      { salesOrderItemId: 'so-2025-001-line-2', productId: 'prod-soda-330', name: 'Assorted Soft Drinks 330ml Case', orderedQuantity: 18, deliveredQuantity: 18, uom: 'case', warehouse: 'WH-JHB-01' }
    ],
    driverId: 'drv-001',
    driverName: 'Lerato M',
    vehicleNumber: 'GP 45 LM GP',
    proofOfDelivery: {
      signatureUrl: '/proof/dn-2025-001-signature.png',
      photoUrl: '/proof/dn-2025-001-photo.jpg',
      deliveredAt: '2025-11-10T15:45:00.000Z',
      receivedBy: 'Sipho Mokoena'
    },
    shippedAt: '2025-11-10T13:00:00.000Z',
    deliveredAt: '2025-11-10T15:45:00.000Z',
    createdAt: '2025-11-10T12:45:00.000Z',
    updatedAt: '2025-11-10T16:00:00.000Z'
  }),
  buildDeliveryNote({
    id: 'dn-2025-002',
    number: 'DN-2025-002',
    status: 'in_transit',
    salesOrderId: 'so-2025-002',
    customer: customers.jabu,
    scheduledDate: '2025-11-13',
    shippingAddress: customers.jabu.shippingAddress ?? '',
    items: mockSalesOrders[1].items.map((item, index) => ({
      salesOrderItemId: item.id ?? `so-2025-002-line-${index + 1}`,
      productId: item.productId,
      name: item.name,
      orderedQuantity: item.quantity,
      deliveredQuantity: item.deliveredQuantity,
      uom: item.uom,
      warehouse: item.warehouse
    })),
    driverId: 'drv-002',
    driverName: 'Thabo S',
    vehicleNumber: 'GP 98 TS GP',
    instructions: 'Partial delivery completed, second drop pending.',
    shippedAt: '2025-11-12T08:15:00.000Z',
    createdAt: '2025-11-12T07:50:00.000Z',
    updatedAt: '2025-11-12T12:05:00.000Z'
  })
]

const mockSalesInvoices: SalesInvoice[] = [
  buildSalesInvoice({
    id: 'inv-2025-001',
    number: 'INV-2025-001',
    status: 'paid',
    salesOrderId: 'so-2025-001',
    customer: customers.sipho,
    issueDate: '2025-11-09',
    dueDate: '2025-12-09',
    items: mockSalesOrders[0].items.map(item => ({
      productId: item.productId,
      sku: item.sku,
      name: item.name,
      description: item.description,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      uom: item.uom,
      discountRate: item.discountRate,
      vatRate: item.vatRate,
      deliveredQuantity: item.deliveredQuantity,
      invoicedQuantity: item.invoicedQuantity,
      warehouse: item.warehouse
    })),
    payments: [
      { id: 'inv-2025-001-pay-1', method: 'card', amount: mockSalesOrders[0].grandTotal, status: 'completed', paidAt: '2025-11-09T16:45:00.000Z', reference: 'INV-2025-001-SETTLED' }
    ],
    createdAt: '2025-11-09T17:00:00.000Z',
    updatedAt: '2025-11-10T08:10:00.000Z'
  }),
  buildSalesInvoice({
    id: 'inv-2025-002',
    number: 'INV-2025-002',
    status: 'sent',
    salesOrderId: 'so-2025-002',
    customer: customers.jabu,
    issueDate: '2025-11-12',
    dueDate: '2025-12-12',
    items: mockSalesOrders[1].items.map(item => ({
      productId: item.productId,
      sku: item.sku,
      name: item.name,
      description: item.description,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      uom: item.uom,
      discountRate: item.discountRate,
      vatRate: item.vatRate,
      deliveredQuantity: item.deliveredQuantity,
      invoicedQuantity: item.invoicedQuantity,
      warehouse: item.warehouse
    })),
    payments: [
      { id: 'inv-2025-002-pay-1', method: 'cash', amount: 900, status: 'completed', paidAt: '2025-11-10T12:00:00.000Z', reference: 'CASH-2025-1110' }
    ],
    createdAt: '2025-11-12T06:45:00.000Z',
    updatedAt: '2025-11-12T06:45:00.000Z'
  })
]

const mockSalesReturns: SalesReturn[] = [
  buildSalesReturn({
    id: 'sr-2025-001',
    number: 'SR-2025-001',
    status: 'approved',
    invoiceId: 'inv-2025-001',
    customer: customers.sipho,
    items: [
      { invoiceItemId: 'inv-2025-001-line-1', productId: 'prod-lager-500', productName: 'Local Lager 500ml Case', quantity: 2, unitPrice: 235, reason: 'damaged', condition: 'damaged', restockable: false }
    ],
    refundMethod: 'store_credit',
    warehouseAction: 'supplier_return',
    notes: 'Damaged during offloading. Approved for credit.',
    requestedAt: '2025-11-11T08:10:00.000Z',
    approvedAt: '2025-11-11T10:45:00.000Z',
    inspectedAt: '2025-11-11T10:30:00.000Z',
    completedAt: '2025-11-12T09:15:00.000Z',
    attachments: ['/returns/sr-2025-001-photo.jpg'],
    createdAt: '2025-11-11T08:10:00.000Z',
    updatedAt: '2025-11-12T09:15:00.000Z'
  })
]

const mockPricingRules: PricingRule[] = [
  {
    id: 'pr-001',
    name: 'Spaza Starter Pack Discount',
    description: '5% discount on essential goods bundles for stokvel customers.',
    type: 'percentage',
    active: true,
    startDate: '2025-01-01',
    endDate: '2025-12-31',
    conditions: {
      minimumQuantity: 10,
      customerGroups: ['stokvel'],
      territories: ['Gauteng / Soweto'],
      eligibleCategories: ['Groceries', 'Beverages']
    },
    reward: {
      discountRate: 5
    },
    createdAt: '2025-01-05T07:00:00.000Z',
    updatedAt: '2025-06-01T08:00:00.000Z'
  },
  {
    id: 'pr-002',
    name: 'Friday Tavern Special',
    description: 'Bulk discount on beverage cases every Friday evening.',
    type: 'amount',
    active: true,
    conditions: {
      minimumAmount: 3000,
      customerGroups: ['business', 'wholesale'],
      daysOfWeek: [5],
      timeRange: { start: '16:00', end: '22:00' },
      eligibleCategories: ['Beverages']
    },
    reward: {
      discountAmount: 250
    },
    createdAt: '2025-02-10T12:00:00.000Z',
    updatedAt: '2025-09-01T06:30:00.000Z'
  }
]

const mockLoyaltyPrograms: LoyaltyProgram[] = [
  {
    id: 'lp-001',
    name: 'TOSS Loyal Spaza',
    description: 'Reward programme for township retailers.',
    earnRatio: 1,
    redeemRatio: 0.1,
    minimumRedemptionPoints: 500,
    tiers: [
      { id: 'lp-001-tier-1', name: 'Bronze', minimumPoints: 0, benefits: ['Birthday voucher', 'WhatsApp support'] },
      { id: 'lp-001-tier-2', name: 'Silver', minimumPoints: 5000, benefits: ['Free delivery (2 per month)', 'Priority restock alerts'] },
      { id: 'lp-001-tier-3', name: 'Gold', minimumPoints: 15000, benefits: ['Dedicated account manager', '3% rebate on monthly spend'] }
    ],
    expiresAfterDays: 365,
    createdAt: '2025-01-01T08:00:00.000Z',
    updatedAt: '2025-08-01T08:00:00.000Z'
  }
]

const mockPosProfiles: PosProfile[] = [
  {
    id: 'pos-prof-001',
    name: 'Main Store POS',
    description: 'Default POS profile for Soweto flagship store.',
    defaultWarehouse: 'WH-JHB-01',
    defaultPriceList: 'TOSS-SPAZA-2025',
    allowedPaymentMethods: ['cash', 'card', 'mobile_money', 'account'],
    receiptTemplate: 'default',
    autoLogoutMinutes: 15,
    cashierRoles: ['cashier', 'manager'],
    loyaltyProgramId: 'lp-001',
    createdAt: '2025-03-01T09:00:00.000Z',
    updatedAt: '2025-07-15T09:00:00.000Z'
  }
]

const mockPosSessions: PosSession[] = [
  {
    id: 'pos-session-001',
    profileId: 'pos-prof-001',
    cashierId: 'user-101',
    cashierName: 'Zanele Khosa',
    openedAt: '2025-11-12T07:55:00.000Z',
    closedAt: '2025-11-12T18:05:00.000Z',
    openingFloat: 1500,
    closingFloat: 4800,
    payments: [
      { method: 'cash', amount: 3800 },
      { method: 'card', amount: 6200 },
      { method: 'mobile_money', amount: 1850 }
    ],
    cashDrops: [
      { id: 'drop-001', amount: 2000, reason: 'Mid-morning safe drop', recordedAt: '2025-11-12T11:15:00.000Z' }
    ],
    expenses: [
      { id: 'exp-001', amount: 250, reason: 'Packaging material', recordedAt: '2025-11-12T13:45:00.000Z' }
    ],
    status: 'closed',
    variance: 50,
    approvals: [
      { id: 'appr-001', approver: 'Store Manager', approvedAt: '2025-11-12T18:10:00.000Z', notes: 'Variance within tolerance.' }
    ]
  },
  {
    id: 'pos-session-002',
    profileId: 'pos-prof-001',
    cashierId: 'user-102',
    cashierName: 'Bongani Dube',
    openedAt: '2025-11-13T07:45:00.000Z',
    openingFloat: 1200,
    payments: [],
    cashDrops: [],
    expenses: [],
    status: 'open'
  }
]

const mockPosSales: PosSale[] = [
  {
    id: 'pos-sale-001',
    reference: 'TOSS-POS-001',
    sessionId: 'pos-session-002',
    profileId: 'pos-prof-001',
    cashierId: 'user-102',
    cashierName: 'Bongani Dube',
    customerId: customers.jabu.id,
    customerName: customers.jabu.name,
    items: [
      {
        productId: 'prod-bread-700',
        productName: 'White Bread 700g',
        sku: 'BREAD-700',
        quantity: 4,
        rate: 15.5,
        discount: 0,
        discountType: 'amount',
        taxRate: VAT_RATE_DEFAULT,
        total: round(4 * 15.5 * (1 + VAT_RATE_DEFAULT / 100))
      },
      {
        productId: 'prod-maize-5',
        productName: 'Maize Meal 5kg',
        sku: 'MAIZE-5KG',
        quantity: 2,
        rate: 45,
        discount: 0,
        discountType: 'amount',
        taxRate: VAT_RATE_DEFAULT,
        total: round(2 * 45 * (1 + VAT_RATE_DEFAULT / 100))
      }
    ],
    payments: [
      { mode: 'cash', amount: 200 },
      { mode: 'mobile', amount: 50, reference: 'MOMO-REF-001' }
    ],
    subtotal: 4 * 15.5 + 2 * 45,
    discount: 0,
    tax: round((4 * 15.5 + 2 * 45) * VAT_RATE_DEFAULT / 100),
    total: round((4 * 15.5 + 2 * 45) * (1 + VAT_RATE_DEFAULT / 100)),
    notes: 'Morning top-up run for essentials.',
    createdAt: '2025-11-13T09:15:00.000Z',
    status: 'completed'
  }
]

const mockParkedPosSales: PosParkedSale[] = [
  {
    reference: 'TOSS-POS-PARK-001',
    createdAt: '2025-11-13T10:05:00.000Z',
    customerId: customers.gogo.id,
    items: [
      {
        productId: 'prod-oil-2l',
        productName: 'Cooking Oil 2L',
        sku: 'OIL-2L',
        quantity: 3,
        rate: 65.75,
        discount: 0,
        discountType: 'amount',
        taxRate: VAT_RATE_DEFAULT,
        total: round(3 * 65.75 * (1 + VAT_RATE_DEFAULT / 100))
      }
    ],
    payments: []
  }
]

const mockSalesAnalytics: SalesAnalyticsSnapshot = {
  from: '2025-11-01',
  to: '2025-11-13',
  totalRevenue: 452300,
  totalOrders: 128,
  averageOrderValue: 3533.59,
  quotationConversionRate: 0.67,
  repeatCustomerRate: 0.54,
  returnsRate: 0.03,
  topProducts: [
    { productId: 'prod-lager-500', productName: 'Local Lager 500ml Case', revenue: 120500, quantity: 180 },
    { productId: 'prod-oil-2l', productName: 'Cooking Oil 2L', revenue: 86500, quantity: 420 },
    { productId: 'prod-maize-5', productName: 'Maize Meal 5kg', revenue: 64500, quantity: 320 }
  ],
  salesByTerritory: [
    { territory: 'Gauteng / Soweto', revenue: 182000, orders: 55 },
    { territory: 'Gauteng / Johannesburg', revenue: 126500, orders: 38 },
    { territory: 'Gauteng / East Rand', revenue: 89600, orders: 25 }
  ],
  paymentMix: [
    { method: 'cash', amount: 142000 },
    { method: 'card', amount: 185000 },
    { method: 'mobile_money', amount: 64000 },
    { method: 'eft', amount: 61200 }
  ],
  dailyTrend: [
    { date: '2025-11-09', revenue: 42000, orders: 10 },
    { date: '2025-11-10', revenue: 86500, orders: 22 },
    { date: '2025-11-11', revenue: 73200, orders: 18 },
    { date: '2025-11-12', revenue: 90500, orders: 26 },
    { date: '2025-11-13', revenue: 84000, orders: 24 }
  ]
}

let quotationSequence = mockQuotations.length
let salesOrderSequence = mockSalesOrders.length
let deliveryNoteSequence = mockDeliveryNotes.length
let invoiceSequence = mockSalesInvoices.length
let returnSequence = mockSalesReturns.length
let posSessionSequence = mockPosSessions.length
let posSaleSequence = mockPosSales.length

const findQuotationIndex = (id: string) => mockQuotations.findIndex(quotation => quotation.id === id || quotation.number === id)
const ensureQuotationRef = (id: string) => {
  const index = findQuotationIndex(id)
  if (index === -1) {
    throw new Error('Quotation not found')
  }
  return { quotation: mockQuotations[index], index }
}

const findSalesOrderIndex = (id: string) => mockSalesOrders.findIndex(order => order.id === id || order.number === id)
const ensureSalesOrderRef = (id: string) => {
  const index = findSalesOrderIndex(id)
  if (index === -1) {
    throw new Error('Sales order not found')
  }
  return { order: mockSalesOrders[index], index }
}

const findDeliveryNoteIndex = (id: string) => mockDeliveryNotes.findIndex(note => note.id === id || note.number === id)
const ensureDeliveryNoteRef = (id: string) => {
  const index = findDeliveryNoteIndex(id)
  if (index === -1) {
    throw new Error('Delivery note not found')
  }
  return { deliveryNote: mockDeliveryNotes[index], index }
}

const findInvoiceIndex = (id: string) => mockSalesInvoices.findIndex(invoice => invoice.id === id || invoice.number === id)
const ensureInvoiceRef = (id: string) => {
  const index = findInvoiceIndex(id)
  if (index === -1) {
    throw new Error('Sales invoice not found')
  }
  return { invoice: mockSalesInvoices[index], index }
}

const findReturnIndex = (id: string) => mockSalesReturns.findIndex(ret => ret.id === id || ret.number === id)
const ensureReturnRef = (id: string) => {
  const index = findReturnIndex(id)
  if (index === -1) {
    throw new Error('Sales return not found')
  }
  return { salesReturn: mockSalesReturns[index], index }
}

const findPosSessionIndex = (id: string) => mockPosSessions.findIndex(session => session.id === id)
const ensurePosSessionRef = (id: string) => {
  const index = findPosSessionIndex(id)
  if (index === -1) {
    throw new Error('POS session not found')
  }
  return { session: mockPosSessions[index], index }
}

export class MockSalesService {
  static listQuotations(filters?: {
    status?: QuotationStatus | 'all'
    customerId?: string
    search?: string
    dateFrom?: string
    dateTo?: string
  }): Quotation[] {
    let results = [...mockQuotations]

    if (filters?.status && filters.status !== 'all') {
      results = results.filter(quotation => quotation.status === filters.status)
    }

    if (filters?.customerId) {
      results = results.filter(quotation => quotation.customer.id === filters.customerId)
    }

    if (filters?.search) {
      const token = normalise(filters.search)
      results = results.filter(quotation =>
        normalise(quotation.number).includes(token) || normalise(quotation.customer.name).includes(token)
      )
    }

    if (filters?.dateFrom) {
      const from = new Date(filters.dateFrom)
      results = results.filter(quotation => new Date(quotation.date) >= from)
    }

    if (filters?.dateTo) {
      const to = new Date(filters.dateTo)
      results = results.filter(quotation => new Date(quotation.date) <= to)
    }

    return results.map(clone)
  }

  static getQuotation(id: string): Quotation {
    const { quotation } = ensureQuotationRef(id)
    return clone(quotation)
  }

  static createQuotation(payload: {
    customer: QuotationCustomer
    items: QuotationItemInput[]
    discountRate?: number
    vatRate?: number
    terms?: string
    notes?: string
    salesPerson?: string
    date?: string
    validUntil?: string
    status?: QuotationStatus
    attachments?: string[]
  }): Quotation {
    const date = payload.date ?? new Date().toISOString().split('T')[0]
    quotationSequence += 1
    const id = `qt-${String(quotationSequence).padStart(4, '0')}`
    const number = generateDocumentNumber('QT', quotationSequence, date)
    const quotation = buildQuotation({
      id,
      number,
      status: payload.status ?? 'draft',
      date,
      validUntil:
        payload.validUntil ?? new Date(new Date(date).getTime() + 14 * 86400000).toISOString().split('T')[0],
      customer: payload.customer,
      items: payload.items,
      discountRate: payload.discountRate,
      vatRate: payload.vatRate,
      terms: payload.terms,
      notes: payload.notes,
      salesPerson: payload.salesPerson,
      createdAt: now(),
      updatedAt: now(),
      attachments: payload.attachments
    })

    mockQuotations.unshift(quotation)
    return clone(quotation)
  }

  static updateQuotation(
    id: string,
    updates: {
      customer?: QuotationCustomer
      status?: QuotationStatus
      date?: string
      validUntil?: string
      discountRate?: number
      vatRate?: number
      terms?: string
      notes?: string
      salesPerson?: string
      items?: QuotationItemInput[]
      attachments?: string[]
    }
  ): Quotation {
    const { quotation, index } = ensureQuotationRef(id)
    const recomputed = buildQuotation({
      id: quotation.id,
      number: quotation.number,
      status: updates.status ?? quotation.status,
      date: updates.date ?? quotation.date,
      validUntil: updates.validUntil ?? quotation.validUntil,
      customer: updates.customer ?? quotation.customer,
      items: updates.items ?? quotation.items.map(toQuotationItemInput),
      discountRate: updates.discountRate ?? quotation.discountRate,
      vatRate: updates.vatRate ?? quotation.vatRate,
      terms: updates.terms ?? quotation.terms,
      notes: updates.notes ?? quotation.notes,
      salesPerson: updates.salesPerson ?? quotation.salesPerson,
      createdAt: quotation.createdAt,
      updatedAt: now(),
      lastSentAt: quotation.lastSentAt,
      lastViewedAt: quotation.lastViewedAt,
      attachments: updates.attachments ?? quotation.attachments,
      convertedToOrderId: quotation.convertedToOrderId
    })

    mockQuotations[index] = recomputed
    return clone(recomputed)
  }

  static deleteQuotation(id: string): void {
    const index = findQuotationIndex(id)
    if (index === -1) {
      throw new Error('Quotation not found')
    }
    mockQuotations.splice(index, 1)
  }

  static changeQuotationStatus(id: string, status: QuotationStatus): Quotation {
    const { quotation } = ensureQuotationRef(id)
    quotation.status = status
    quotation.updatedAt = now()
    recalculateQuotationTotals(quotation)
    return clone(quotation)
  }

  static duplicateQuotation(id: string): Quotation {
    const { quotation } = ensureQuotationRef(id)
    quotationSequence += 1
    const date = new Date().toISOString().split('T')[0]
    const duplicate = buildQuotation({
      id: `qt-${String(quotationSequence).padStart(4, '0')}`,
      number: generateDocumentNumber('QT', quotationSequence, date),
      status: 'draft',
      date,
      validUntil: quotation.validUntil,
      customer: quotation.customer,
      items: quotation.items.map(toQuotationItemInput),
      discountRate: quotation.discountRate,
      vatRate: quotation.vatRate,
      terms: quotation.terms,
      notes: quotation.notes,
      salesPerson: quotation.salesPerson,
      attachments: quotation.attachments,
      createdAt: now(),
      updatedAt: now()
    })

    mockQuotations.unshift(duplicate)
    return clone(duplicate)
  }

  static convertQuotationToOrder(id: string): SalesOrder {
    const { quotation } = ensureQuotationRef(id)

    if (quotation.convertedToOrderId) {
      return this.getSalesOrder(quotation.convertedToOrderId)
    }

    salesOrderSequence += 1
    const orderId = `so-${String(salesOrderSequence).padStart(4, '0')}`
    const order = buildSalesOrder({
      id: orderId,
      number: generateDocumentNumber('SO', salesOrderSequence, quotation.date),
      status: 'confirmed',
      paymentStatus: 'unpaid',
      quotationId: quotation.id,
      customer: quotation.customer,
      items: quotation.items.map(item => ({
        productId: item.productId,
        sku: item.sku,
        name: item.name,
        description: item.description,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        uom: item.uom,
        discountRate: item.discountRate,
        vatRate: item.vatRate,
        deliveredQuantity: 0,
        invoicedQuantity: 0,
        warehouse: 'WH-JHB-01'
      })),
      expectedDeliveryDate: quotation.validUntil,
      shippingAddress: quotation.customer.shippingAddress ?? quotation.customer.address ?? '',
      instructions: quotation.notes,
      assignedDriverId: undefined,
      assignedDriverName: undefined,
      payments: [],
      notes: `Converted from quotation ${quotation.number}`,
      createdAt: now(),
      updatedAt: now()
    })

    mockSalesOrders.unshift(order)
    quotation.status = 'converted'
    quotation.convertedToOrderId = order.id
    quotation.updatedAt = now()
    recalculateQuotationTotals(quotation)

    return clone(order)
  }

  static sendQuotationEmail(
    id: string,
    email: { to: string; subject: string; message: string; cc?: string[]; bcc?: string[] }
  ): { sentAt: string } {
    const { quotation } = ensureQuotationRef(id)
    const sentAt = now()
    quotation.lastSentAt = sentAt
    quotation.updatedAt = sentAt
    return { sentAt }
  }

  static generateQuotationPdf(id: string): { filename: string; base64: string } {
    const { quotation } = ensureQuotationRef(id)
    const payload = `Quotation ${quotation.number} for ${quotation.customer.name}`
    const base64 = Buffer.from(payload).toString('base64')
    return {
      filename: `${quotation.number}.pdf`,
      base64
    }
  }

  static getQuotationStats(): Record<QuotationStatus, number> {
    const stats: Record<QuotationStatus, number> = {
      draft: 0,
      sent: 0,
      accepted: 0,
      rejected: 0,
      expired: 0,
      converted: 0
    }

    mockQuotations.forEach(quotation => {
      stats[quotation.status] = (stats[quotation.status] ?? 0) + 1
    })

    return stats
  }

  static listSalesOrders(filters?: {
    status?: SalesOrderStatus | 'all'
    customerId?: string
    paymentStatus?: PaymentStatus
    search?: string
  }): SalesOrder[] {
    let results = [...mockSalesOrders]

    if (filters?.status && filters.status !== 'all') {
      results = results.filter(order => order.status === filters.status)
    }

    if (filters?.customerId) {
      results = results.filter(order => order.customer.id === filters.customerId)
    }

    if (filters?.paymentStatus) {
      results = results.filter(order => order.paymentStatus === filters.paymentStatus)
    }

    if (filters?.search) {
      const token = normalise(filters.search)
      results = results.filter(order =>
        normalise(order.number).includes(token) || normalise(order.customer.name).includes(token)
      )
    }

    return results.map(clone)
  }

  static getSalesOrder(id: string): SalesOrder {
    const { order } = ensureSalesOrderRef(id)
    return clone(order)
  }

  static createSalesOrder(payload: {
    quotationId?: string
    customer?: QuotationCustomer
    items?: SalesOrderItemInput[]
    expectedDeliveryDate: string
    shippingAddress?: string
    instructions?: string
    payments?: SalesOrderPayment[]
    notes?: string
    status?: SalesOrderStatus
    paymentStatus?: PaymentStatus
  }): SalesOrder {
    const quotation = payload.quotationId ? this.getQuotation(payload.quotationId) : null
    const customer = payload.customer ?? quotation?.customer

    if (!customer) {
      throw new Error('Customer details are required to create a sales order')
    }

    const items = payload.items ?? quotation?.items.map(item => ({
      productId: item.productId,
      sku: item.sku,
      name: item.name,
      description: item.description,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      uom: item.uom,
      discountRate: item.discountRate,
      vatRate: item.vatRate,
      deliveredQuantity: 0,
      invoicedQuantity: 0,
      warehouse: 'WH-JHB-01'
    }))

    if (!items || !items.length) {
      throw new Error('Sales order requires at least one line item')
    }

    salesOrderSequence += 1
    const order = buildSalesOrder({
      id: `so-${String(salesOrderSequence).padStart(4, '0')}`,
      number: generateDocumentNumber('SO', salesOrderSequence, new Date().toISOString()),
      status: payload.status ?? 'pending',
      paymentStatus: payload.paymentStatus,
      quotationId: quotation?.id,
      customer,
      items,
      expectedDeliveryDate: payload.expectedDeliveryDate,
      shippingAddress: payload.shippingAddress ?? customer.shippingAddress ?? customer.address ?? '',
      instructions: payload.instructions,
      assignedDriverId: undefined,
      assignedDriverName: undefined,
      payments: payload.payments ?? [],
      notes: payload.notes,
      createdAt: now(),
      updatedAt: now()
    })

    mockSalesOrders.unshift(order)
    return clone(order)
  }

  static updateSalesOrder(
    id: string,
    updates: {
      customer?: QuotationCustomer
      status?: SalesOrderStatus
      paymentStatus?: PaymentStatus
      expectedDeliveryDate?: string
      shippingAddress?: string
      instructions?: string
      assignedDriverId?: string
      assignedDriverName?: string
      items?: SalesOrderItemInput[]
      payments?: SalesOrderPayment[]
      notes?: string
    }
  ): SalesOrder {
    const { order } = ensureSalesOrderRef(id)

    if (updates.customer) {
      order.customer = updates.customer
    }

    if (updates.status) {
      order.status = updates.status
    }

    if (updates.paymentStatus) {
      order.paymentStatus = updates.paymentStatus
    }

    if (updates.expectedDeliveryDate) {
      order.fulfillment.expectedDeliveryDate = updates.expectedDeliveryDate
    }

    if (updates.shippingAddress) {
      order.fulfillment.shippingAddress = updates.shippingAddress
    }

    if (typeof updates.instructions !== 'undefined') {
      order.fulfillment.instructions = updates.instructions
    }

    if (typeof updates.assignedDriverId !== 'undefined') {
      order.fulfillment.assignedDriverId = updates.assignedDriverId
    }

    if (typeof updates.assignedDriverName !== 'undefined') {
      order.fulfillment.assignedDriverName = updates.assignedDriverName
    }

    if (updates.items) {
      order.items = updates.items.map(toSalesOrderItemInput).map((item, index) => ({
        ...item,
        id: item.id ?? `${order.id}-line-${index + 1}`,
        discountAmount: undefined,
        vatAmount: undefined,
        total: 0
      })) as SalesOrderItem[]
    }

    if (updates.payments) {
      order.payments = updates.payments
    }

    if (typeof updates.notes !== 'undefined') {
      order.notes = updates.notes
    }

    recalculateSalesOrderTotals(order)
    return clone(order)
  }

  static updateSalesOrderStatus(id: string, status: SalesOrderStatus): SalesOrder {
    const { order } = ensureSalesOrderRef(id)
    order.status = status
    recalculateSalesOrderTotals(order)
    return clone(order)
  }

  static recordSalesOrderPayment(id: string, payment: SalesOrderPayment): SalesOrder {
    const { order } = ensureSalesOrderRef(id)
    order.payments.push({ ...payment, id: payment.id ?? `pay-${order.id}-${order.payments.length + 1}` })
    recalculateSalesOrderTotals(order)
    return clone(order)
  }

  static appendSalesOrderHistory(
    id: string,
    entry: { action: string; user: string; notes?: string; timestamp?: string }
  ): SalesOrder {
    const { order } = ensureSalesOrderRef(id)
    order.history.push({
      id: `${order.id}-history-${order.history.length + 1}`,
      action: entry.action,
      user: entry.user,
      notes: entry.notes,
      timestamp: entry.timestamp ?? now()
    })
    order.updatedAt = now()
    return clone(order)
  }

  static listDeliveryNotes(filters?: {
    status?: DeliveryNoteStatus | 'all'
    customerId?: string
    salesOrderId?: string
  }): DeliveryNote[] {
    let results = [...mockDeliveryNotes]

    if (filters?.status && filters.status !== 'all') {
      results = results.filter(note => note.status === filters.status)
    }

    if (filters?.customerId) {
      results = results.filter(note => note.customer.id === filters.customerId)
    }

    if (filters?.salesOrderId) {
      results = results.filter(note => note.salesOrderId === filters.salesOrderId)
    }

    return results.map(clone)
  }

  static getDeliveryNote(id: string): DeliveryNote {
    const { deliveryNote } = ensureDeliveryNoteRef(id)
    return clone(deliveryNote)
  }

  static createDeliveryNote(payload: {
    salesOrderId?: string
    customer?: QuotationCustomer
    items?: DeliveryNoteItemInput[]
    scheduledDate: string
    shippingAddress?: string
    billingAddress?: string
    driverId?: string
    driverName?: string
    vehicleNumber?: string
    instructions?: string
    status?: DeliveryNoteStatus
  }): DeliveryNote {
    let customer = payload.customer
    let items = payload.items

    if (payload.salesOrderId) {
      const order = this.getSalesOrder(payload.salesOrderId)
      customer = customer ?? order.customer
      items =
        items ??
        order.items.map((item, index) => ({
          salesOrderItemId: item.id ?? `${order.id}-line-${index + 1}`,
          productId: item.productId,
          name: item.name,
          orderedQuantity: item.quantity,
          deliveredQuantity: item.deliveredQuantity,
          uom: item.uom,
          warehouse: item.warehouse
        }))
    }

    if (!customer) {
      throw new Error('Customer information is required to create a delivery note')
    }

    if (!items || !items.length) {
      throw new Error('Delivery note requires at least one item')
    }

    deliveryNoteSequence += 1
    const deliveryNote = buildDeliveryNote({
      id: `dn-${String(deliveryNoteSequence).padStart(4, '0')}`,
      number: generateDocumentNumber('DN', deliveryNoteSequence, payload.scheduledDate),
      status: payload.status ?? 'scheduled',
      salesOrderId: payload.salesOrderId,
      customer,
      scheduledDate: payload.scheduledDate,
      shippingAddress: payload.shippingAddress ?? customer.shippingAddress ?? customer.address ?? '',
      billingAddress: payload.billingAddress,
      driverId: payload.driverId,
      driverName: payload.driverName,
      vehicleNumber: payload.vehicleNumber,
      instructions: payload.instructions,
      items,
      createdAt: now(),
      updatedAt: now()
    })

    mockDeliveryNotes.unshift(deliveryNote)
    return clone(deliveryNote)
  }

  static updateDeliveryNote(
    id: string,
    updates: {
      status?: DeliveryNoteStatus
      scheduledDate?: string
      shippingAddress?: string
      billingAddress?: string
      driverId?: string
      driverName?: string
      vehicleNumber?: string
      instructions?: string
      items?: DeliveryNoteItemInput[]
      proofOfDelivery?: ProofOfDelivery
    }
  ): DeliveryNote {
    const { deliveryNote } = ensureDeliveryNoteRef(id)

    if (updates.status) {
      deliveryNote.status = updates.status
    }

    if (updates.scheduledDate) {
      deliveryNote.scheduledDate = updates.scheduledDate
    }

    if (typeof updates.shippingAddress !== 'undefined') {
      deliveryNote.shippingAddress = updates.shippingAddress
    }

    if (typeof updates.billingAddress !== 'undefined') {
      deliveryNote.billingAddress = updates.billingAddress
    }

    if (typeof updates.driverId !== 'undefined') {
      deliveryNote.driverId = updates.driverId
    }

    if (typeof updates.driverName !== 'undefined') {
      deliveryNote.driverName = updates.driverName
    }

    if (typeof updates.vehicleNumber !== 'undefined') {
      deliveryNote.vehicleNumber = updates.vehicleNumber
    }

    if (typeof updates.instructions !== 'undefined') {
      deliveryNote.instructions = updates.instructions
    }

    if (updates.items) {
      deliveryNote.items = updates.items.map((item, index) => ({
        ...item,
        id: item.id ?? `${deliveryNote.id}-item-${index + 1}`
      }))
    }

    if (updates.proofOfDelivery) {
      deliveryNote.proofOfDelivery = updates.proofOfDelivery
    }

    deliveryNote.updatedAt = now()
    return clone(deliveryNote)
  }

  static updateDeliveryNoteStatus(id: string, status: DeliveryNoteStatus): DeliveryNote {
    const { deliveryNote } = ensureDeliveryNoteRef(id)
    deliveryNote.status = status
    if (status === 'completed') {
      deliveryNote.deliveredAt = deliveryNote.deliveredAt ?? now()
    }
    deliveryNote.updatedAt = now()
    return clone(deliveryNote)
  }

  static submitProofOfDelivery(id: string, proof: ProofOfDelivery): DeliveryNote {
    const { deliveryNote } = ensureDeliveryNoteRef(id)
    deliveryNote.proofOfDelivery = proof
    deliveryNote.status = 'completed'
    deliveryNote.deliveredAt = proof.deliveredAt ?? now()
    deliveryNote.updatedAt = now()
    return clone(deliveryNote)
  }

  static listSalesInvoices(filters?: {
    status?: SalesInvoiceStatus | 'all'
    customerId?: string
    salesOrderId?: string
  }): SalesInvoice[] {
    let results = [...mockSalesInvoices]

    if (filters?.status && filters.status !== 'all') {
      results = results.filter(invoice => invoice.status === filters.status)
    }

    if (filters?.customerId) {
      results = results.filter(invoice => invoice.customer.id === filters.customerId)
    }

    if (filters?.salesOrderId) {
      results = results.filter(invoice => invoice.salesOrderId === filters.salesOrderId)
    }

    return results.map(clone)
  }

  static getSalesInvoice(id: string): SalesInvoice {
    const { invoice } = ensureInvoiceRef(id)
    return clone(invoice)
  }

  static createSalesInvoice(payload: {
    salesOrderId?: string
    customer?: QuotationCustomer
    items?: SalesInvoiceItemInput[]
    issueDate?: string
    dueDate?: string
    payments?: SalesInvoicePayment[]
    notes?: string
    status?: SalesInvoiceStatus
  }): SalesInvoice {
    const order = payload.salesOrderId ? this.getSalesOrder(payload.salesOrderId) : null
    const customer = payload.customer ?? order?.customer
    const items = payload.items ?? order?.items.map(item => ({
      productId: item.productId,
      sku: item.sku,
      name: item.name,
      description: item.description,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      uom: item.uom,
      discountRate: item.discountRate,
      vatRate: item.vatRate,
      deliveredQuantity: item.deliveredQuantity,
      invoicedQuantity: item.invoicedQuantity,
      warehouse: item.warehouse
    }))

    if (!customer) {
      throw new Error('Customer information is required for invoice creation')
    }

    if (!items || !items.length) {
      throw new Error('Sales invoice requires at least one line item')
    }

    invoiceSequence += 1
    const invoice = buildSalesInvoice({
      id: `inv-${String(invoiceSequence).padStart(4, '0')}`,
      number: generateDocumentNumber('INV', invoiceSequence, payload.issueDate ?? now()),
      status: payload.status ?? 'draft',
      salesOrderId: payload.salesOrderId,
      customer,
      issueDate: payload.issueDate ?? new Date().toISOString().split('T')[0],
      dueDate:
        payload.dueDate ?? new Date(new Date().getTime() + 30 * 86400000).toISOString().split('T')[0],
      items,
      payments: payload.payments ?? [],
      notes: payload.notes,
      createdAt: now(),
      updatedAt: now()
    })

    mockSalesInvoices.unshift(invoice)
    return clone(invoice)
  }

  static updateSalesInvoice(
    id: string,
    updates: {
      status?: SalesInvoiceStatus
      issueDate?: string
      dueDate?: string
      items?: SalesInvoiceItemInput[]
      payments?: SalesInvoicePayment[]
      notes?: string
    }
  ): SalesInvoice {
    const { invoice } = ensureInvoiceRef(id)

    if (updates.status) {
      invoice.status = updates.status
    }

    if (updates.issueDate) {
      invoice.issueDate = updates.issueDate
    }

    if (updates.dueDate) {
      invoice.dueDate = updates.dueDate
    }

    if (updates.items) {
      invoice.items = updates.items.map((item, index) => ({
        ...item,
        id: item.id ?? `${invoice.id}-line-${index + 1}`,
        discountAmount: undefined,
        vatAmount: undefined,
        total: 0,
        deliveredQuantity: item.deliveredQuantity ?? item.quantity,
        invoicedQuantity: item.invoicedQuantity ?? item.quantity
      })) as SalesOrderItem[]
    }

    if (updates.payments) {
      invoice.payments = updates.payments
    }

    if (typeof updates.notes !== 'undefined') {
      invoice.notes = updates.notes
    }

    recalculateInvoiceTotals(invoice)
    return clone(invoice)
  }

  static recordInvoicePayment(id: string, payment: SalesInvoicePayment): SalesInvoice {
    const { invoice } = ensureInvoiceRef(id)
    invoice.payments.push({
      ...payment,
      id: payment.id ?? `${invoice.id}-pay-${invoice.payments.length + 1}`    })
    recalculateInvoiceTotals(invoice)
    return clone(invoice)
  }

  static listSalesReturns(filters?: {
    status?: SalesReturnStatus | 'all'
    customerId?: string
    invoiceId?: string
  }): SalesReturn[] {
    let results = [...mockSalesReturns]

    if (filters?.status && filters.status !== 'all') {
      results = results.filter(ret => ret.status === filters.status)
    }

    if (filters?.customerId) {
      results = results.filter(ret => ret.customer.id === filters.customerId)
    }

    if (filters?.invoiceId) {
      results = results.filter(ret => ret.invoiceId === filters.invoiceId)
    }

    return results.map(clone)
  }

  static getSalesReturn(id: string): SalesReturn {
    const { salesReturn } = ensureReturnRef(id)
    return clone(salesReturn)
  }

  static createSalesReturn(payload: {
    invoiceId: string
    customer?: QuotationCustomer
    items: SalesReturnItemInput[]
    refundMethod: SalesReturn['refundMethod']
    warehouseAction: SalesReturn['warehouseAction']
    notes?: string
  }): SalesReturn {
    const invoice = this.getSalesInvoice(payload.invoiceId)
    const customer = payload.customer ?? invoice.customer

    returnSequence += 1
    const salesReturn = buildSalesReturn({
      id: `sr-${String(returnSequence).padStart(4, '0')}`,
      number: generateDocumentNumber('SR', returnSequence, now()),
      status: 'requested',
      invoiceId: payload.invoiceId,
      customer,
      items: payload.items,
      refundMethod: payload.refundMethod,
      warehouseAction: payload.warehouseAction,
      notes: payload.notes,
      requestedAt: now(),
      createdAt: now(),
      updatedAt: now()
    })

    mockSalesReturns.unshift(salesReturn)
    return clone(salesReturn)
  }

  static updateSalesReturnStatus(id: string, status: SalesReturnStatus): SalesReturn {
    const { salesReturn } = ensureReturnRef(id)
    salesReturn.status = status
    if (status === 'approved') {
      salesReturn.approvedAt = now()
    }
    if (status === 'completed') {
      salesReturn.completedAt = now()
    }
    salesReturn.updatedAt = now()
    return clone(salesReturn)
  }

  static listPricingRules(): PricingRule[] {
    return mockPricingRules.map(clone)
  }

  static listLoyaltyPrograms(): LoyaltyProgram[] {
    return mockLoyaltyPrograms.map(clone)
  }

  static listPosProfiles(): PosProfile[] {
    return mockPosProfiles.map(clone)
  }

  static listPosSessions(filters?: {
    status?: PosSession['status'] | 'all'
    profileId?: string
    cashierId?: string
  }): PosSession[] {
    let results = [...mockPosSessions]

    if (filters?.status && filters.status !== 'all') {
      results = results.filter(session => session.status === filters.status)
    }

    if (filters?.profileId) {
      results = results.filter(session => session.profileId === filters.profileId)
    }

    if (filters?.cashierId) {
      results = results.filter(session => session.cashierId === filters.cashierId)
    }

    return results.map(clone)
  }

  static openPosSession(payload: {
    profileId: string
    cashierId: string
    cashierName: string
    openingFloat: number
    openedAt?: string
  }): PosSession {
    posSessionSequence += 1
    const session: PosSession = {
      id: `pos-session-${String(posSessionSequence).padStart(3, '0')}`,
      profileId: payload.profileId,
      cashierId: payload.cashierId,
      cashierName: payload.cashierName,
      openedAt: payload.openedAt ?? now(),
      openingFloat: payload.openingFloat,
      payments: [],
      cashDrops: [],
      expenses: [],
      status: 'open'
    }

    mockPosSessions.unshift(session)
    return clone(session)
  }

  static closePosSession(
    id: string,
    summary: {
      closingFloat: number
      payments: PosSessionPaymentSummary[]
      cashDrops?: PosSessionCashDrop[]
      expenses?: PosSessionExpense[]
      variance?: number
      closedAt?: string
      approvals?: PosSession['approvals']
    }
  ): PosSession {
    const { session } = ensurePosSessionRef(id)
    session.closingFloat = summary.closingFloat
    session.payments = summary.payments
    session.cashDrops = summary.cashDrops ?? session.cashDrops
    session.expenses = summary.expenses ?? session.expenses
    session.variance = summary.variance ?? session.variance
    session.closedAt = summary.closedAt ?? now()
    session.status = summary.variance && summary.variance > 100 ? 'pending_approval' : 'closed'
    session.approvals = summary.approvals ?? session.approvals
    return clone(session)
  }

  static getSalesAnalytics(): SalesAnalyticsSnapshot {
    return clone(mockSalesAnalytics)
  }

  // POS-specific helpers for the front-end POS module

  static listPosSales(filters?: { sessionId?: string; customerId?: string; limit?: number }): PosSale[] {
    let results = [...mockPosSales]

    if (filters?.sessionId) {
      results = results.filter(sale => sale.sessionId === filters.sessionId)
    }

    if (filters?.customerId) {
      results = results.filter(sale => sale.customerId === filters.customerId)
    }

    results.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())

    if (filters?.limit && filters.limit > 0) {
      results = results.slice(0, filters.limit)
    }

    return results.map(clone)
  }

  static getPosSale(idOrRef: string): PosSale | undefined {
    const match = mockPosSales.find(
      sale => sale.id === idOrRef || sale.reference === idOrRef
    )
    return match ? clone(match) : undefined
  }

  static createPosSale(payload: Omit<PosSale, 'id' | 'reference' | 'createdAt' | 'status'> & {
    referencePrefix?: string
  }): PosSale {
    posSaleSequence += 1
    const id = `pos-sale-${String(posSaleSequence).padStart(3, '0')}`
    const referencePrefix = payload.referencePrefix ?? 'TOSS-POS'
    const reference = `${referencePrefix}-${String(posSaleSequence).padStart(3, '0')}`
    const createdAt = now()

    const sale: PosSale = {
      id,
      reference,
      createdAt,
      status: 'completed',
      sessionId: payload.sessionId,
      profileId: payload.profileId,
      cashierId: payload.cashierId,
      cashierName: payload.cashierName,
      customerId: payload.customerId,
      customerName: payload.customerName,
      items: payload.items,
      payments: payload.payments,
      subtotal: payload.subtotal,
      discount: payload.discount,
      tax: payload.tax,
      total: payload.total,
      notes: payload.notes
    }

    mockPosSales.unshift(sale)
    return clone(sale)
  }

  static listPosParkedSales(filters?: { customerId?: string }): PosParkedSale[] {
    let results = [...mockParkedPosSales]

    if (filters?.customerId) {
      results = results.filter(sale => sale.customerId === filters.customerId)
    }

    results.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    return results.map(clone)
  }

  static parkPosSale(payload: Omit<PosParkedSale, 'reference' | 'createdAt'> & {
    referencePrefix?: string
  }): PosParkedSale {
    const referencePrefix = payload.referencePrefix ?? 'TOSS-POS-PARK'
    const reference = `${referencePrefix}-${String(mockParkedPosSales.length + 1).padStart(3, '0')}`
    const createdAt = now()

    const parked: PosParkedSale = {
      reference,
      createdAt,
      customerId: payload.customerId,
      items: payload.items,
      payments: payload.payments
    }

    mockParkedPosSales.unshift(parked)
    return clone(parked)
  }

  static removePosParkedSale(reference: string): void {
    const index = mockParkedPosSales.findIndex(sale => sale.reference === reference)
    if (index !== -1) {
      mockParkedPosSales.splice(index, 1)
    }
  }
}


