export interface Store {
  id: number
  name: string
  description?: string
  ownerId: string
  url: string
  sslEnabled: boolean
  hosts: string
  displayOrder: number
  companyName?: string
  companyAddress?: string
  companyPhoneNumber?: string
  companyVat?: string
  contactPhone?: string
  email?: string
  currency: string
  taxRate: number
  language: string
  timezone: string
  areaGroup?: string
  openingTime?: string
  closingTime?: string
  whatsAppAlertsEnabled: boolean
  groupBuyingEnabled: boolean
  aiAssistantEnabled: boolean
  isActive: boolean
  latitude?: number
  longitude?: number
  addressId?: number
  addressStreet?: string
  addressCity?: string
  addressProvince?: string
  addressPostalCode?: string
  addressCountry?: string
  customerCount?: number
  productCount?: number
  salesCount?: number
  totalRevenue?: number
}

export interface CreateStoreRequest {
  name: string
  description?: string
  ownerId: string
  url?: string
  sslEnabled?: boolean
  hosts?: string
  displayOrder?: number
  companyName?: string
  companyAddress?: string
  companyPhoneNumber?: string
  companyVat?: string
  contactPhone?: string
  email?: string
  currency?: string
  taxRate?: number
  language?: string
  timezone?: string
  areaGroup?: string
  openingTime?: string
  closingTime?: string
  whatsAppAlertsEnabled?: boolean
  groupBuyingEnabled?: boolean
  aiAssistantEnabled?: boolean
  latitude?: number
  longitude?: number
  addressStreet?: string
  addressCity?: string
  addressProvince?: string
  addressPostalCode?: string
  addressCountry?: string
}

export interface UpdateStoreRequest extends CreateStoreRequest {
  id: number
  isActive?: boolean
}

