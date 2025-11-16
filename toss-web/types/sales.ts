// Customer Group types for South African SMME context

export interface CustomerGroup {
  id: string
  name: string
  description: string
  defaultDiscount: number // Percentage discount for this group
  creditLimit: number // Maximum credit allowed (ZAR)
  paymentTerms: number // Payment terms in days
  priceList?: string // Special pricing list ID
  territory?: string // Primary territory for this group
  groupType: 'individual' | 'stokvel' | 'burial_society' | 'business' | 'wholesale'
  isActive: boolean
  createdAt: string
  updatedAt: string
  
  // South African SMME specific fields
  vatRegistered: boolean
  preferredPaymentMethod: 'cash' | 'mobile_money' | 'bank_transfer' | 'account'
  deliveryPreference: 'pickup' | 'delivery' | 'both'
  minimumOrderAmount?: number
  
  // Group buying features
  allowGroupOrders: boolean
  groupOrderDiscount: number // Additional discount for group orders
  minimumGroupSize?: number
  
  // Statistics
  totalCustomers?: number
  averageOrderValue?: number
  totalRevenue?: number
}

export interface CreateCustomerGroupRequest {
  name: string
  description: string
  defaultDiscount: number
  creditLimit: number
  paymentTerms: number
  priceList?: string
  territory?: string
  groupType: 'individual' | 'stokvel' | 'burial_society' | 'business' | 'wholesale'
  vatRegistered: boolean
  preferredPaymentMethod: 'cash' | 'mobile_money' | 'bank_transfer' | 'account'
  deliveryPreference: 'pickup' | 'delivery' | 'both'
  minimumOrderAmount?: number
  allowGroupOrders: boolean
  groupOrderDiscount: number
  minimumGroupSize?: number
}

export interface UpdateCustomerGroupRequest extends Partial<CreateCustomerGroupRequest> {
  id: string
}

// Sales Territory types for township/rural delivery
export interface SalesTerritory {
  id: string
  name: string
  description: string
  parentTerritory?: string
  manager?: string
  deliveryCharges: number
  taxRate: number // VAT rate for this territory
  isActive: boolean
  
  // Location details
  provinces: string[] // South African provinces
  municipalities: string[]
  townships: string[]
  postalCodes: string[]
  landmarks: string[] // Informal location markers
  
  // Delivery logistics
  deliveryDays: string[] // Days when delivery is available
  deliveryTimeSlots: Array<{
    startTime: string
    endTime: string
    additionalCharge: number
  }>
  minimumDeliveryAmount: number
  freeDeliveryThreshold: number
  
  // Driver assignments
  assignedDrivers: string[]
  
  // Statistics
  totalCustomers?: number
  averageDeliveryTime?: number
  totalOrders?: number
  
  createdAt: string
  updatedAt: string
}

export interface CreateSalesTerritoryRequest {
  name: string
  description: string
  parentTerritory?: string
  manager?: string
  deliveryCharges: number
  taxRate: number
  provinces: string[]
  municipalities: string[]
  townships: string[]
  postalCodes: string[]
  landmarks: string[]
  deliveryDays: string[]
  deliveryTimeSlots: Array<{
    startTime: string
    endTime: string
    additionalCharge: number
  }>
  minimumDeliveryAmount: number
  freeDeliveryThreshold: number
  assignedDrivers: string[]
}

// Credit Note types for returns and adjustments
export interface CreditNote {
  id: string
  creditNoteNumber: string
  returnedInvoice: string
  customer: {
    id: string
    name: string
    groupId: string
  }
  returnDate: Date
  processedBy: string
  
  returnedItems: Array<{
    id: string
    productId: string
    productName: string
    productSku: string
    quantityReturned: number
    unitPrice: number
    lineTotal: number
    reason: string
    condition: 'damaged' | 'expired' | 'wrong_item' | 'customer_return'
    restockable: boolean
  }>
  
  financials: {
    subtotal: number
    vatAmount: number
    totalCreditAmount: number
  }
  
  refundMethod: 'cash' | 'store_credit' | 'replacement' | 'bank_refund'
  refundStatus: 'pending' | 'processed' | 'completed'
  refundReference?: string
  
  notes?: string
  attachments?: string[]
  
  createdAt: string
  updatedAt: string
}

export interface CreateCreditNoteRequest {
  returnedInvoice: string
  customerId: string
  returnedItems: Array<{
    productId: string
    quantityReturned: number
    reason: string
    condition: 'damaged' | 'expired' | 'wrong_item' | 'customer_return'
    restockable: boolean
  }>
  refundMethod: 'cash' | 'store_credit' | 'replacement' | 'bank_refund'
  notes?: string
}

// Promotional Scheme types for SMME marketing
export interface PromotionalScheme {
  id: string
  name: string
  description: string
  type: 'percentage' | 'amount' | 'buy_x_get_y' | 'bulk_discount' | 'group_discount'
  
  validity: {
    validFrom: Date
    validTo: Date
    isActive: boolean
  }
  
  // Applicability rules
  applicableCustomerGroups: string[]
  applicableTerritories: string[]
  applicableProducts?: string[]
  applicableCategories?: string[]
  
  // Conditions
  conditions: {
    minimumQuantity?: number
    minimumAmount?: number
    minimumGroupSize?: number // For group buying
    dayOfWeek?: string[] // Weekend specials, etc.
    timeOfDay?: {
      startTime: string
      endTime: string
    }
    paymentMethod?: string[] // Mobile money discounts, etc.
  }
  
  // Discount structure
  discount: {
    percentage?: number
    fixedAmount?: number
    freeQuantity?: number
    freeProduct?: string
    tieredDiscounts?: Array<{
      threshold: number
      discount: number
    }>
  }
  
  // Usage limits
  limits: {
    maxUsesPerCustomer?: number
    maxUsesTotal?: number
    currentUses: number
  }
  
  // Statistics
  statistics: {
    totalSaved: number
    timesUsed: number
    averageOrderValue: number
  }
  
  createdAt: string
  updatedAt: string
}

export interface CreatePromotionalSchemeRequest {
  name: string
  description: string
  type: 'percentage' | 'amount' | 'buy_x_get_y' | 'bulk_discount' | 'group_discount'
  validFrom: Date
  validTo: Date
  applicableCustomerGroups: string[]
  applicableTerritories: string[]
  applicableProducts?: string[]
  applicableCategories?: string[]
  conditions: {
    minimumQuantity?: number
    minimumAmount?: number
    minimumGroupSize?: number
    dayOfWeek?: string[]
    timeOfDay?: {
      startTime: string
      endTime: string
    }
    paymentMethod?: string[]
  }
  discount: {
    percentage?: number
    fixedAmount?: number
    freeQuantity?: number
    freeProduct?: string
    tieredDiscounts?: Array<{
      threshold: number
      discount: number
    }>
  }
  limits: {
    maxUsesPerCustomer?: number
    maxUsesTotal?: number
  }
}

// Sales Partner types for TOSS network
export interface SalesPartner {
  id: string
  name: string
  partnerType: 'supplier' | 'driver' | 'shop' | 'aggregator' | 'financial_partner'
  
  contactInfo: {
    primaryContact: string
    email: string
    phone: string
    whatsappNumber?: string
    address: {
      street: string
      township: string
      municipality: string
      province: string
      postalCode: string
    }
  }
  
  businessInfo: {
    registrationNumber?: string
    vatNumber?: string
    taxClearanceCertificate?: string
    bbbeeCertificate?: string
  }
  
  // Commission and financials
  commission: {
    rate: number // Percentage
    type: 'percentage' | 'fixed_amount'
    paymentTerms: number // Days
    minimumCommission?: number
  }
  
  // Service area and capabilities
  serviceArea: {
    territories: string[]
    deliveryRadius?: number // kilometers
    serviceTypes: string[]
  }
  
  // Performance metrics
  performance: {
    rating: number
    totalTransactions: number
    totalValue: number
    onTimeDeliveryRate?: number
    customerSatisfactionScore?: number
  }
  
  // Compliance and verification
  verification: {
    isVerified: boolean
    verifiedBy?: string
    verificationDate?: Date
    documentsUploaded: string[]
  }
  
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateSalesPartnerRequest {
  name: string
  partnerType: 'supplier' | 'driver' | 'shop' | 'aggregator' | 'financial_partner'
  contactInfo: {
    primaryContact: string
    email: string
    phone: string
    whatsappNumber?: string
    address: {
      street: string
      township: string
      municipality: string
      province: string
      postalCode: string
    }
  }
  businessInfo: {
    registrationNumber?: string
    vatNumber?: string
  }
  commission: {
    rate: number
    type: 'percentage' | 'fixed_amount'
    paymentTerms: number
    minimumCommission?: number
  }
  serviceArea: {
    territories: string[]
    deliveryRadius?: number
    serviceTypes: string[]
  }
}

export type QuotationStatus = 'draft' | 'sent' | 'accepted' | 'rejected' | 'expired' | 'converted'

export interface SalesProductSummary {
  id: string
  sku: string
  name: string
  description: string
  unitPrice: number
  vatRate: number
  uom: string
  stockOnHand: number
}

export interface QuotationCustomer {
  id: string
  name: string
  email?: string
  phone?: string
  address?: string
  billingAddress?: string
  shippingAddress?: string
  territory?: string
  customerGroup?: string
  creditLimit?: number
  creditUsed?: number
  paymentTerms?: number
  primaryContact?: string
}

export interface QuotationItem {
  id: string
  productId: string
  sku?: string
  name: string
  description?: string
  quantity: number
  unitPrice: number
  uom?: string
  discountRate?: number
  discountAmount?: number
  vatRate?: number
  vatAmount?: number
  total: number
}

export interface Quotation {
  id: string
  number: string
  status: QuotationStatus
  date: string
  validUntil: string
  salesPerson?: string
  terms?: string
  notes?: string
  customer: QuotationCustomer
  items: QuotationItem[]
  subtotal: number
  discountRate?: number
  discountAmount: number
  vatRate: number
  vatAmount: number
  grandTotal: number
  createdAt: string
  updatedAt: string
  convertedToOrderId?: string
  lastSentAt?: string
  lastViewedAt?: string
  attachments?: string[]
}

export type SalesOrderStatus = 'draft' | 'pending' | 'confirmed' | 'processing' | 'completed' | 'cancelled' | 'on_hold'
export type PaymentStatus = 'unpaid' | 'partial' | 'paid'

export interface SalesOrderItem extends QuotationItem {
  deliveredQuantity: number
  invoicedQuantity: number
  warehouse?: string
}

export interface SalesOrderHistoryEntry {
  id: string
  timestamp: string
  action: string
  user: string
  notes?: string
}

export interface SalesOrderFulfillment {
  expectedDeliveryDate: string
  deliveryWindow?: {
    start: string
    end: string
  }
  shippingAddress: string
  instructions?: string
  assignedDriverId?: string
  assignedDriverName?: string
  status: 'pending' | 'scheduled' | 'in_transit' | 'delivered' | 'failed'
}

export interface SalesOrderPayment {
  id: string
  method: 'cash' | 'card' | 'mobile_money' | 'eft' | 'account'
  amount: number
  status: 'pending' | 'authorised' | 'completed' | 'failed'
  reference?: string
  paidAt?: string
}

export interface SalesOrder {
  id: string
  number: string
  quotationId?: string
  customer: QuotationCustomer
  status: SalesOrderStatus
  paymentStatus: PaymentStatus
  items: SalesOrderItem[]
  subtotal: number
  discountAmount: number
  vatAmount: number
  grandTotal: number
  outstandingBalance: number
  fulfillment: SalesOrderFulfillment
  payments: SalesOrderPayment[]
  notes?: string
  createdAt: string
  updatedAt: string
  history: SalesOrderHistoryEntry[]
}

export type DeliveryNoteStatus = 'draft' | 'scheduled' | 'in_transit' | 'completed' | 'cancelled' | 'returned'

export interface DeliveryNoteItem {
  id: string
  salesOrderItemId?: string
  productId: string
  name: string
  description?: string
  orderedQuantity: number
  deliveredQuantity: number
  uom?: string
  warehouse?: string
  serialNumbers?: string[]
  batchNumbers?: string[]
}

export interface ProofOfDelivery {
  signatureUrl?: string
  photoUrl?: string
  deliveredAt?: string
  receivedBy?: string
  notes?: string
  geoLocation?: {
    lat: number
    lng: number
    accuracy?: number
  }
}

export interface DeliveryNote {
  id: string
  number: string
  salesOrderId?: string
  customer: QuotationCustomer
  status: DeliveryNoteStatus
  scheduledDate: string
  shippedAt?: string
  deliveredAt?: string
  shippingAddress: string
  billingAddress?: string
  driverId?: string
  driverName?: string
  vehicleNumber?: string
  instructions?: string
  items: DeliveryNoteItem[]
  proofOfDelivery?: ProofOfDelivery
  createdAt: string
  updatedAt: string
}

export type SalesInvoiceStatus = 'draft' | 'sent' | 'paid' | 'overdue' | 'cancelled'

export interface SalesInvoicePayment extends SalesOrderPayment {
  dueDate?: string
}

export interface SalesInvoice {
  id: string
  number: string
  salesOrderId?: string
  customer: QuotationCustomer
  status: SalesInvoiceStatus
  issueDate: string
  dueDate: string
  items: SalesOrderItem[]
  subtotal: number
  discountAmount: number
  vatAmount: number
  total: number
  paidAmount: number
  balanceDue: number
  payments: SalesInvoicePayment[]
  notes?: string
  createdAt: string
  updatedAt: string
}

export type SalesReturnStatus = 'requested' | 'approved' | 'declined' | 'received' | 'completed'

export interface SalesReturnItem {
  id: string
  invoiceItemId: string
  productId: string
  productName: string
  quantity: number
  unitPrice: number
  reason: string
  condition: 'damaged' | 'expired' | 'wrong_item' | 'customer_return'
  restockable: boolean
  lineTotal: number
}

export interface SalesReturn {
  id: string
  number: string
  status: SalesReturnStatus
  invoiceId: string
  customer: QuotationCustomer
  requestedAt: string
  approvedAt?: string
  inspectedAt?: string
  completedAt?: string
  items: SalesReturnItem[]
  refundMethod: 'cash' | 'store_credit' | 'exchange' | 'bank'
  refundAmount: number
  warehouseAction: 'restock' | 'scrap' | 'supplier_return'
  notes?: string
  attachments?: string[]
  createdAt: string
  updatedAt: string
}

export interface PricingRule {
  id: string
  name: string
  description?: string
  type: 'percentage' | 'amount' | 'buy_x_get_y' | 'bundle'
  active: boolean
  startDate?: string
  endDate?: string
  conditions: {
    minimumQuantity?: number
    minimumAmount?: number
    customerGroups?: string[]
    territories?: string[]
    paymentMethods?: Array<'cash' | 'card' | 'mobile_money' | 'eft' | 'account'>
    daysOfWeek?: number[]
    timeRange?: {
      start: string
      end: string
    }
    eligibleProducts?: string[]
    eligibleCategories?: string[]
  }
  reward: {
    discountRate?: number
    discountAmount?: number
    freeProductId?: string
    freeQuantity?: number
  }
  createdAt: string
  updatedAt: string
}

export interface PosProfile {
  id: string
  name: string
  description?: string
  defaultWarehouse: string
  defaultPriceList: string
  allowedPaymentMethods: Array<'cash' | 'card' | 'mobile_money' | 'eft' | 'account'>
  receiptTemplate: string
  autoLogoutMinutes: number
  cashierRoles: string[]
  loyaltyProgramId?: string
  createdAt: string
  updatedAt: string
}

export interface PosSessionPaymentSummary {
  method: 'cash' | 'card' | 'mobile_money' | 'eft' | 'account'
  amount: number
}

export interface PosSessionCashDrop {
  id: string
  amount: number
  reason?: string
  recordedAt: string
}

export interface PosSessionExpense {
  id: string
  amount: number
  reason: string
  recordedAt: string
}

export interface PosSession {
  id: string
  profileId: string
  cashierId: string
  cashierName: string
  openedAt: string
  closedAt?: string
  openingFloat: number
  closingFloat?: number
  payments: PosSessionPaymentSummary[]
  cashDrops: PosSessionCashDrop[]
  expenses: PosSessionExpense[]
  status: 'open' | 'closed' | 'pending_approval'
  variance?: number
  approvals?: Array<{
    id: string
    approver: string
    approvedAt: string
    notes?: string
  }>
}

export interface PosSaleItem {
  productId: string
  productName: string
  sku?: string
  quantity: number
  rate: number
  discount?: number
  discountType?: 'percentage' | 'amount'
  taxRate: number
  total: number
}

export interface PosPaymentEntry {
  mode: 'cash' | 'card' | 'mobile' | 'credit' | 'other'
  amount: number
  reference?: string
  accountNumber?: string
}

export interface PosSale {
  id: string
  reference: string
  sessionId?: string
  profileId?: string
  cashierId?: string
  cashierName?: string
  customerId?: string
  customerName?: string
  items: PosSaleItem[]
  payments: PosPaymentEntry[]
  subtotal: number
  discount: number
  tax: number
  total: number
  notes?: string
  createdAt: string
  status: 'completed' | 'refunded'
}

export interface PosParkedSale {
  reference: string
  createdAt: string
  customerId?: string
  items: PosSaleItem[]
  payments: PosPaymentEntry[]
}

export interface LoyaltyTier {
  id: string
  name: string
  minimumPoints: number
  benefits: string[]
}

export interface LoyaltyProgram {
  id: string
  name: string
  description?: string
  earnRatio: number
  redeemRatio: number
  minimumRedemptionPoints: number
  tiers: LoyaltyTier[]
  expiresAfterDays?: number
  createdAt: string
  updatedAt: string
}

export interface SalesAnalyticsSnapshot {
  from: string
  to: string
  totalRevenue: number
  totalOrders: number
  averageOrderValue: number
  quotationConversionRate: number
  repeatCustomerRate: number
  returnsRate: number
  topProducts: Array<{
    productId: string
    productName: string
    revenue: number
    quantity: number
  }>
  salesByTerritory: Array<{
    territory: string
    revenue: number
    orders: number
  }>
  paymentMix: Array<{
    method: 'cash' | 'card' | 'mobile_money' | 'eft' | 'account'
    amount: number
  }>
  dailyTrend: Array<{
    date: string
    revenue: number
    orders: number
  }>
}