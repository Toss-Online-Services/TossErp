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