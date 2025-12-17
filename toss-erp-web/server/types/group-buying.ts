// Group Buying / Pooling Types for TOSS

export interface Pool {
  id: string
  tenantId?: string
  sku?: string
  itemId?: string
  title?: string
  description?: string
  supplierId: string
  supplierName: string
  productId: string
  productName: string
  unitPrice: number
  poolPrice: number
  currentPrice?: number
  minQuantity: number
  maxQuantity: number
  currentQuantity: number
  targetQuantity?: number
  minParticipants?: number
  maxParticipants?: number
  currentCommitment?: number
  participantCount?: number
  savingsPercentage?: number
  status: 'open' | 'confirmed' | 'closed' | 'cancelled' | 'pending'
  startsAt: string
  endsAt: string
  deadline?: Date | string
  deliveryDate?: string
  deliveryLocation?: string
  area?: string
  splitRule?: string
  leadShopId?: string
  leadShopName?: string
  extendedOnce?: boolean
  participants: PoolParticipant[]
  createdAt: string
  updatedAt: string
}

export interface PoolParticipant {
  id: string
  poolId: string
  shopId: string
  shopName: string
  shopPhone?: string
  quantity: number
  quantityCommitted?: number
  totalAmount: number
  costShare?: number
  deliveryFeeShare?: number
  status: 'pending' | 'confirmed' | 'paid' | 'cancelled' | 'joined'
  paymentStatus?: string
  paymentLinkId?: string
  paymentLinkUrl?: string
  joinedAt: string
  confirmedAt?: string
  paidAt?: string
}

export interface PoolInvite {
  id: string
  poolId: string
  shopId: string
  shopName: string
  sentAt: string
  respondedAt?: string
  response?: 'interested' | 'not-interested' | 'joined'
}

export interface PoolSavings {
  poolId: string
  shopId: string
  regularPrice: number
  poolPrice: number
  quantity: number
  totalSavings: number
  savingsPercentage: number
}

export interface PoolFilterDto {
  status?: string
  supplierId?: string
  productId?: string
  participantShopId?: string
}

export interface CreatePoolDto {
  supplierId: string
  supplierName: string
  productId: string
  productName: string
  unitPrice: number
  poolPrice: number
  minQuantity: number
  maxQuantity: number
  startsAt: string
  endsAt: string
  deliveryDate?: string
  deliveryLocation?: string
}

export interface JoinPoolDto {
  shopId: string
  shopName: string
  shopPhone?: string
  quantity: number
}

export interface ConfirmPoolDto {
  deliveryDate: string
  deliveryLocation: string
}

export interface PoolSavingsCalculation {
  poolId: string
  shopId: string
  regularPrice: number
  poolPrice: number
  quantity: number
  totalSavings: number
  savingsPercentage: number
}
