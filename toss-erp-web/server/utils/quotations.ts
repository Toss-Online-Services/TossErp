import { randomUUID } from 'uncrypto'

export type QuotationStatus =
  | 'draft'
  | 'sent'
  | 'accepted'
  | 'rejected'
  | 'expired'
  | 'converted'

export interface QuotationItem {
  id: string
  productId: string
  productName: string
  description?: string
  quantity: number
  rate: number
  discountPercent: number
  vatRate: number
  amount: number
}

export interface QuotationActivity {
  id: string
  type: 'created' | 'sent' | 'accepted' | 'rejected' | 'converted' | 'updated'
  title: string
  description: string
  user: string
  timestamp: string
}

export interface QuotationCustomer {
  id: string
  name: string
  businessName: string
  email: string
  phone: string
  address: string
  creditLimit: number
  balance: number
  vatNumber?: string
}

export interface QuotationProduct {
  id: string
  sku: string
  name: string
  description: string
  price: number
  unit: string
  vatRate: number
}

export interface QuotationRecord {
  id: string
  quotationNumber: string
  customerId: string
  customerName: string
  quotationDate: string
  validUntil: string
  status: QuotationStatus
  priceList: string
  currency: string
  subtotal: number
  discountAmount: number
  taxableAmount: number
  taxAmount: number
  grandTotal: number
  termsAndConditions?: string
  notes?: string
  createdAt: string
  updatedAt: string
  items: QuotationItem[]
  customer: QuotationCustomer
  activities: QuotationActivity[]
  convertedToOrder?: string
}

export interface QuotationMeta {
  customers: QuotationCustomer[]
  products: QuotationProduct[]
  priceLists: { value: string; label: string }[]
  salesPeople: { id: string; name: string }[]
}

interface QuotationStore {
  lastNumber: number
  records: QuotationRecord[]
}

const STORAGE_KEY = 'quotations:records'
const META_KEY = 'quotations:meta'

const storage = () => useStorage<QuotationStore>('data')
const metaStorage = () => useStorage<QuotationMeta>('data')

const buildQuotationNumber = (sequence: number) =>
  `QTN-${new Date().getFullYear()}-${sequence.toString().padStart(4, '0')}`

const seedCustomers = (): QuotationCustomer[] => [
  {
    id: 'cust-100',
    name: 'Mama Dlamini',
    businessName: 'Mama Dlamini Spaza Shop',
    email: 'mama.dlamini@example.com',
    phone: '+27 82 123 4567',
    address: '45 Main Road, Soweto, Johannesburg 1800',
    creditLimit: 7500,
    balance: 1250,
    vatNumber: '4450256679'
  },
  {
    id: 'cust-101',
    name: "Sibusiso Mkhize",
    businessName: "Sbu's Chisa Nyama",
    email: 'sbu@example.com',
    phone: '+27 84 987 6543',
    address: '12 Vilakazi Street, Orlando West, Soweto',
    creditLimit: 12000,
    balance: 0,
    vatNumber: '4450256680'
  },
  {
    id: 'cust-102',
    name: 'Thandi Mokoena',
    businessName: 'Thandi Hair Studio',
    email: 'thandi@example.com',
    phone: '+27 71 555 0102',
    address: '7 Bertha Street, Mamelodi, Pretoria',
    creditLimit: 5000,
    balance: 850,
    vatNumber: '4450256681'
  }
]

const seedProducts = (): QuotationProduct[] => [
  {
    id: 'prod-100',
    sku: 'BREAD-700',
    name: 'Albany White Bread 700g',
    description: 'Fresh daily white bread – Albany bakery',
    price: 15.99,
    unit: 'loaf',
    vatRate: 15
  },
  {
    id: 'prod-101',
    sku: 'OIL-2L',
    name: 'Sunflower Cooking Oil 2L',
    description: 'Golden Ray sunflower cooking oil 2L bottle',
    price: 54.99,
    unit: 'bottle',
    vatRate: 15
  },
  {
    id: 'prod-102',
    sku: 'MAIZE-5KG',
    name: 'Ace Super Maize Meal 5kg',
    description: 'Top-selling maize meal for pap lovers',
    price: 78.99,
    unit: 'bag',
    vatRate: 15
  },
  {
    id: 'prod-103',
    sku: 'SUGAR-2_5',
    name: 'Huletts White Sugar 2.5kg',
    description: 'Premium white sugar 2.5kg bag',
    price: 42.99,
    unit: 'bag',
    vatRate: 15
  }
]

const seedPriceLists = () => [
  { value: 'standard', label: 'Standard Pricing' },
  { value: 'wholesale', label: 'Wholesale Discount' },
  { value: 'bulk', label: 'Bulk Special' }
]

const seedSalesPeople = () => [
  { id: 'sales-100', name: 'John Dube' },
  { id: 'sales-101', name: 'Ayanda Nkosi' },
  { id: 'sales-102', name: 'Kabelo Phiri' }
]

const computeTotals = (items: QuotationItem[]) => {
  const subtotal = items.reduce((sum, item) => sum + item.quantity * item.rate, 0)
  const discountAmount = items.reduce(
    (sum, item) => sum + (item.quantity * item.rate * item.discountPercent) / 100,
    0
  )
  const taxableAmount = subtotal - discountAmount
  const taxAmount = items.reduce(
    (sum, item) => sum + ((item.quantity * item.rate - (item.quantity * item.rate * item.discountPercent) / 100) * item.vatRate) / 100,
    0
  )
  const grandTotal = taxableAmount + taxAmount

  return { subtotal, discountAmount, taxableAmount, taxAmount, grandTotal }
}

const buildItem = (product: QuotationProduct, quantity: number, discountPercent = 0): QuotationItem => {
  const rate = product.price
  const gross = quantity * rate
  const discountValue = (gross * discountPercent) / 100
  const net = gross - discountValue

  return {
    id: randomUUID(),
    productId: product.id,
    productName: product.name,
    description: product.description,
    quantity,
    rate,
    discountPercent,
    vatRate: product.vatRate,
    amount: Number(net.toFixed(2))
  }
}

const seedQuotations = async () => {
  const customers = seedCustomers()
  const products = seedProducts()

  if (!products[0] || !products[1] || !products[2] || !products[3]) {
    throw new Error('Failed to seed products')
  }

  const quotationAItems = [
    buildItem(products[0], 40, 5),
    buildItem(products[2], 10)
  ]
  const quotationBItems = [
    buildItem(products[1], 25),
    buildItem(products[3], 20, 10)
  ]

  const totalsA = computeTotals(quotationAItems)
  const totalsB = computeTotals(quotationBItems)

  if (!customers[0] || !customers[1]) {
    throw new Error('Failed to seed customers')
  }

  const now = new Date()
  const store: QuotationStore = {
    lastNumber: 2,
    records: [
      {
        id: 'qtn-1000',
        quotationNumber: buildQuotationNumber(1),
        customerId: customers[0].id,
        customerName: customers[0].businessName,
        quotationDate: new Date(now.getTime() - 1000 * 60 * 60 * 24 * 5).toISOString(),
        validUntil: new Date(now.getTime() + 1000 * 60 * 60 * 24 * 15).toISOString(),
        status: 'sent',
        priceList: 'standard',
        currency: 'ZAR',
        subtotal: Number(totalsA.subtotal.toFixed(2)),
        discountAmount: Number(totalsA.discountAmount.toFixed(2)),
        taxableAmount: Number(totalsA.taxableAmount.toFixed(2)),
        taxAmount: Number(totalsA.taxAmount.toFixed(2)),
        grandTotal: Number(totalsA.grandTotal.toFixed(2)),
        termsAndConditions: 'Payment within 14 days. Includes township delivery.',
        notes: 'Extended discount for first-time bulk order.',
        createdAt: now.toISOString(),
        updatedAt: now.toISOString(),
        items: quotationAItems,
        customer: customers[0],
        activities: [
          {
            id: randomUUID(),
            type: 'created',
            title: 'Quotation Created',
            description: 'Quotation drafted by sales rep John Dube.',
            user: 'John Dube',
            timestamp: now.toISOString()
          },
          {
            id: randomUUID(),
            type: 'sent',
            title: 'Quotation Sent',
            description: 'Sent to customer via email.',
            user: 'John Dube',
            timestamp: new Date(now.getTime() + 3600000).toISOString()
          }
        ]
      },
      {
        id: 'qtn-1001',
        quotationNumber: buildQuotationNumber(2),
        customerId: customers[1].id,
        customerName: customers[1].businessName,
        quotationDate: new Date(now.getTime() - 1000 * 60 * 60 * 24 * 2).toISOString(),
        validUntil: new Date(now.getTime() + 1000 * 60 * 60 * 24 * 20).toISOString(),
        status: 'draft',
        priceList: 'wholesale',
        currency: 'ZAR',
        subtotal: Number(totalsB.subtotal.toFixed(2)),
        discountAmount: Number(totalsB.discountAmount.toFixed(2)),
        taxableAmount: Number(totalsB.taxableAmount.toFixed(2)),
        taxAmount: Number(totalsB.taxAmount.toFixed(2)),
        grandTotal: Number(totalsB.grandTotal.toFixed(2)),
        termsAndConditions: 'Valid for 30 days. 50% deposit secures pricing.',
        notes: 'Awaiting customer confirmation after tasting.',
        createdAt: now.toISOString(),
        updatedAt: now.toISOString(),
        items: quotationBItems,
        customer: customers[1],
        activities: [
          {
            id: randomUUID(),
            type: 'created',
            title: 'Quotation Created',
            description: 'Draft prepared for catering supplies.',
            user: 'Ayanda Nkosi',
            timestamp: now.toISOString()
          }
        ]
      }
    ]
  }

  await storage().setItem(STORAGE_KEY, store)
  await metaStorage().setItem(META_KEY, {
    customers,
    products,
    priceLists: seedPriceLists(),
    salesPeople: seedSalesPeople()
  })
}

export const getQuotationStore = async (): Promise<QuotationStore> => {
  const existing = await storage().getItem(STORAGE_KEY)
  if (!existing) {
    await seedQuotations()
    return (await storage().getItem(STORAGE_KEY)) as QuotationStore
  }
  return existing
}

export const getQuotationMeta = async (): Promise<QuotationMeta> => {
  const existing = await metaStorage().getItem(META_KEY)
  if (!existing) {
    await seedQuotations()
    return (await metaStorage().getItem(META_KEY)) as QuotationMeta
  }
  return existing
}

export const persistQuotationStore = async (store: QuotationStore) => {
  await storage().setItem(STORAGE_KEY, store)
}

export const findQuotation = async (id: string) => {
  const store = await getQuotationStore()
  return store.records.find((record) => record.id === id)
}

export const listQuotations = async (filters?: {
  status?: string
  search?: string
  dateFrom?: string
  dateTo?: string
}) => {
  const store = await getQuotationStore()
  let records = [...store.records]

  if (filters?.status) {
    records = records.filter((record) => record.status === filters.status)
  }

  if (filters?.search) {
    const term = filters.search.toLowerCase()
    records = records.filter(
      (record) =>
        record.quotationNumber.toLowerCase().includes(term) ||
        record.customerName.toLowerCase().includes(term)
    )
  }

  if (filters?.dateFrom) {
    const from = new Date(filters.dateFrom)
    records = records.filter((record) => new Date(record.quotationDate) >= from)
  }

  if (filters?.dateTo) {
    const to = new Date(filters.dateTo)
    records = records.filter((record) => new Date(record.quotationDate) <= to)
  }

  return {
    records,
    stats: {
      draft: records.filter((r) => r.status === 'draft').length,
      sent: records.filter((r) => r.status === 'sent').length,
      accepted: records.filter((r) => r.status === 'accepted').length,
      expired: records.filter((r) => r.status === 'expired').length
    }
  }
}

export const createQuotationRecord = async (payload: {
  customerId: string
  quotationDate: string
  validUntil: string
  priceList: string
  currency: string
  termsAndConditions?: string
  notes?: string
  items: { productId: string; quantity: number; discountPercent?: number }[]
}) => {
  const store = await getQuotationStore()
  const meta = await getQuotationMeta()
  const customer = meta.customers.find((c) => c.id === payload.customerId)

  if (!customer) {
    throw createError({ statusCode: 400, statusMessage: 'Customer not found' })
  }

  const items = payload.items.map((item) => {
    const product = meta.products.find((p) => p.id === item.productId)
    if (!product) {
      throw createError({ statusCode: 400, statusMessage: `Product ${item.productId} not found` })
    }
    return buildItem(product, item.quantity, item.discountPercent ?? 0)
  })

  const totals = computeTotals(items)
  const nextNumber = store.lastNumber + 1
  const now = new Date().toISOString()

  const record: QuotationRecord = {
    id: randomUUID(),
    quotationNumber: buildQuotationNumber(nextNumber),
    customerId: customer.id,
    customerName: customer.businessName,
    quotationDate: payload.quotationDate,
    validUntil: payload.validUntil,
    status: 'draft',
    priceList: payload.priceList,
    currency: payload.currency,
    subtotal: Number(totals.subtotal.toFixed(2)),
    discountAmount: Number(totals.discountAmount.toFixed(2)),
    taxableAmount: Number(totals.taxableAmount.toFixed(2)),
    taxAmount: Number(totals.taxAmount.toFixed(2)),
    grandTotal: Number(totals.grandTotal.toFixed(2)),
    termsAndConditions: payload.termsAndConditions,
    notes: payload.notes,
    createdAt: now,
    updatedAt: now,
    items,
    customer,
    activities: [
      {
        id: randomUUID(),
        type: 'created',
        title: 'Quotation Created',
        description: 'Quotation drafted from web portal.',
        user: 'System',
        timestamp: now
      }
    ]
  }

  store.records.unshift(record)
  store.lastNumber = nextNumber
  await persistQuotationStore(store)
  return record
}

export const updateQuotationRecord = async (
  id: string,
  updates: Partial<Omit<QuotationRecord, 'id' | 'quotationNumber' | 'activities'>>
) => {
  const store = await getQuotationStore()
  const index = store.records.findIndex((record) => record.id === id)
  if (index === -1) {
    throw createError({ statusCode: 404, statusMessage: 'Quotation not found' })
  }

  const record = store.records[index]
  if (!record) {
    throw createError({ statusCode: 404, statusMessage: 'Quotation not found' })
  }

  let items = record.items
  if (updates.items && updates.items.length > 0) {
    const meta = await getQuotationMeta()
    items = updates.items.map((item) => {
      const product = meta.products.find((p) => p.id === item.productId)
      if (!product) {
        throw createError({ statusCode: 400, statusMessage: `Product ${item.productId} not found` })
      }
      return buildItem(product, item.quantity, item.discountPercent ?? 0)
    })
  }

  const totals = computeTotals(items)
  const updated: QuotationRecord = {
    ...record,
    ...updates,
    items,
    subtotal: Number(totals.subtotal.toFixed(2)),
    discountAmount: Number(totals.discountAmount.toFixed(2)),
    taxableAmount: Number(totals.taxableAmount.toFixed(2)),
    taxAmount: Number(totals.taxAmount.toFixed(2)),
    grandTotal: Number(totals.grandTotal.toFixed(2)),
    updatedAt: new Date().toISOString()
  }

  updated.activities = [
    ...record.activities,
    {
      id: randomUUID(),
      type: 'updated',
      title: 'Quotation Updated',
      description: 'Quotation details updated.',
      user: 'System',
      timestamp: updated.updatedAt
    }
  ]

  store.records[index] = updated
  await persistQuotationStore(store)
  return updated
}

export const deleteQuotationRecord = async (id: string) => {
  const store = await getQuotationStore()
  store.records = store.records.filter((record) => record.id !== id)
  await persistQuotationStore(store)
}

export const changeQuotationStatus = async (id: string, status: QuotationStatus) => {
  const store = await getQuotationStore()
  const record = store.records.find((item) => item.id === id)
  if (!record) {
    throw createError({ statusCode: 404, statusMessage: 'Quotation not found' })
  }

  record.status = status
  
  const activityType = status === 'converted' ? 'converted' : 
                      status === 'sent' ? 'sent' : 
                      status === 'accepted' ? 'accepted' : 
                      status === 'rejected' ? 'rejected' : 'updated'
  
  record.activities.push({
    id: randomUUID(),
    type: activityType,
    title: `Quotation ${status}`,
    description: `Status changed to ${status}.`,
    user: 'System',
    timestamp: new Date().toISOString()
  })
  record.updatedAt = new Date().toISOString()
  await persistQuotationStore(store)
  return record
}

export const convertQuotationToOrder = async (id: string) => {
  const record = await changeQuotationStatus(id, 'converted')
  const orderId = `SO-${record.quotationNumber.slice(-4)}`
  record.convertedToOrder = orderId

  const store = await getQuotationStore()
  const index = store.records.findIndex((item) => item.id === id)
  if (index !== -1) {
    store.records[index] = { ...record }
    await persistQuotationStore(store)
  }

  return orderId
}

export const duplicateQuotationRecord = async (id: string) => {
  const store = await getQuotationStore()
  const record = store.records.find((item) => item.id === id)
  if (!record) {
    throw createError({ statusCode: 404, statusMessage: 'Quotation not found' })
  }

  const now = new Date().toISOString()
  const nextNumber = store.lastNumber + 1
  const duplicate: QuotationRecord = {
    ...record,
    id: randomUUID(),
    quotationNumber: buildQuotationNumber(nextNumber),
    status: 'draft',
    createdAt: now,
    updatedAt: now,
    activities: [
      ...record.activities,
      {
        id: randomUUID(),
        type: 'created',
        title: 'Quotation Duplicated',
        description: `Duplicated from ${record.quotationNumber}.`,
        user: 'System',
        timestamp: now
      }
    ]
  }

  store.records.unshift(duplicate)
  store.lastNumber = nextNumber
  await persistQuotationStore(store)
  return duplicate
}
