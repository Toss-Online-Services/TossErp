// Group Buying Type Definitions for TOSS
// Core entities for collective procurement and pool management

export type PoolStatus = 'open' | 'pending' | 'confirmed' | 'fulfilled' | 'cancelled'
export type SplitRule = 'flat' | 'by-units'
export type ParticipantStatus = 'joined' | 'confirmed' | 'paid' | 'received'
export type PaymentStatus = 'pending' | 'paid' | 'failed'

export interface Pool {
  id: string
  tenantId: string
  sku: string // Single SKU for MVP
  itemId: string // Link to items table
  title: string
  description: string
  
  // Pool Settings
  targetQuantity: number
  minParticipants: number
  maxParticipants: number
  deadline: Date
  area: string // Geographic zone (e.g., "soweto-north", "alexandra")
  
  // Pricing
  currentPrice: number // Regular solo price per unit
  poolPrice: number // Discounted pool price per unit
  savingsPercentage: number // Calculated: ((currentPrice - poolPrice) / currentPrice) * 100
  priceBreaks?: PriceTier[] // Future: tiered pricing based on volume
  
  // State Management
  status: PoolStatus
  currentCommitment: number // Total units committed by all participants
  participantCount: number
  
  // Split Rules
  splitRule: SplitRule
  
  // Collaboration
  leadShopId: string
  leadShopName: string
  participants: PoolParticipant[]
  
  // Integration
  supplierId: string
  supplierName: string
  purchaseOrderId?: string // Created when pool confirms
  
  // Notifications
  inviteLink?: string // WhatsApp shareable link
  
  // Metadata
  createdAt: Date
  updatedAt: Date
  confirmedAt?: Date
  fulfilledAt?: Date
  cancelledAt?: Date
  cancellationReason?: string
  
  // Extension tracking
  extendedOnce: boolean // Pool can only be extended once
  originalDeadline?: Date
}

export interface PoolParticipant {
  id: string
  poolId: string
  shopId: string
  shopName: string // Or alias for privacy
  quantityCommitted: number
  costShare: number // Calculated based on splitRule
  deliveryFeeShare: number // Share of delivery cost if using shared logistics
  status: ParticipantStatus
  joinedAt: Date
  confirmedAt?: Date
  paidAt?: Date
  receivedAt?: Date
  
  // Payment
  paymentLinkId?: string
  paymentLinkUrl?: string
  paymentStatus: PaymentStatus
  paymentReference?: string
  
  // Delivery
  deliveryStopId?: string // Link to shared logistics delivery stop
  
  // Notes
  notes?: string
}

export interface PriceTier {
  minQuantity: number
  pricePerUnit: number
  savingsPercentage: number
  description?: string // e.g., "Bronze tier", "Volume discount"
}

// DTOs for API requests/responses

export interface CreatePoolDto {
  sku: string
  itemId: string
  title: string
  description: string
  targetQuantity: number
  minParticipants: number
  maxParticipants: number
  deadline: Date
  area: string
  currentPrice: number
  poolPrice: number
  splitRule: SplitRule
  supplierId: string
  supplierName: string
  priceBreaks?: PriceTier[]
}

export interface JoinPoolDto {
  poolId: string
  shopId: string
  shopName: string
  quantityCommitted: number
}

export interface UpdatePoolDto {
  title?: string
  description?: string
  deadline?: Date
  targetQuantity?: number
  maxParticipants?: number
  poolPrice?: number
}

export interface ConfirmPoolDto {
  poolId: string
  createPurchaseOrder: boolean
}

export interface PoolSavingsCalculation {
  poolId: string
  regularCost: number // What participant would pay solo
  poolCost: number // What participant pays in pool
  savings: number // Difference
  savingsPercentage: number
  deliverySavings?: number // Additional savings from shared logistics
  totalSavings: number
}

export interface PoolInviteDto {
  poolId: string
  phoneNumbers: string[] // WhatsApp numbers to invite
  customMessage?: string
}

export interface PoolFilterDto {
  status?: PoolStatus
  area?: string
  sku?: string
  supplierId?: string
  leadShopId?: string
  participantShopId?: string
  minSavingsPercentage?: number
  deadlineBefore?: Date
  deadlineAfter?: Date
}

// Analytics and reporting

export interface PoolAnalytics {
  totalPools: number
  activePools: number
  confirmedPools: number
  fulfilledPools: number
  cancelledPools: number
  poolFillRate: number // Percentage of pools that reached target
  avgParticipantsPerPool: number
  avgSavingsPercentage: number
  totalSavings: number
  popularSkus: { sku: string; poolCount: number }[]
  topAreas: { area: string; poolCount: number }[]
}

export interface ShopPoolStats {
  shopId: string
  poolsJoined: number
  poolsLed: number
  totalSavings: number
  avgSavingsPercentage: number
  participationRate: number // Percentage of available pools joined
  reliabilityScore: number // Based on payment and pickup timeliness
}

