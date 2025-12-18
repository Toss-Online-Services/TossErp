// Sales and Quotation types
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

export interface QuotationFormItemInput {
  productId: string
  productName?: string
  description?: string
  quantity: number
  rate?: number
  discountPercent: number
  amount?: number
}

export interface QuotationTotals {
  subtotal: number
  discountAmount: number
  discount?: number // Alias for discountAmount
  taxableAmount: number
  taxAmount: number
  tax?: number // Alias for taxAmount
  grandTotal: number
}

