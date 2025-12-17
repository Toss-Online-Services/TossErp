// Group Buying / Pooling Types for TOSS

export interface Pool {
  id: string
  supplierId: string
  supplierName: string
  productId: string
  productName: string
  unitPrice: number
  poolPrice: number
  minQuantity: number
  maxQuantity: number
  currentQuantity: number
  status: 'open' | 'confirmed' | 'closed' | 'cancelled'
  startsAt: string
  endsAt: string
  deliveryDate?: string
  deliveryLocation?: string
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
  totalAmount: number
  status: 'pending' | 'confirmed' | 'paid' | 'cancelled'
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
