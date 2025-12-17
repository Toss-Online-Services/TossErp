/**
 * Smart Purchasing System Types
 * Unified types for Order Aggregation and Group Buying
 */

export type PurchaseType = 'individual' | 'aggregated' | 'group-buy'
export type SavingsMethod = 'auto-aggregation' | 'group-buy' | 'bulk-discount' | 'none'

export interface SmartPurchaseOption {
  type: 'aggregation' | 'group-buy' | 'create-group-buy'
  available: boolean
  estimatedSavings: number
  savingsPercentage: number
  details: string
  actionLabel: string
  metadata?: any
}

export interface AggregationOpportunity {
  id: string
  itemSku: string
  itemName: string
  totalQuantity: number
  participatingOrders: AggregatedOrderInfo[]
  estimatedSavings: number
  savingsPercentage: number
  expiresAt: Date
  status: 'pending' | 'confirmed' | 'expired'
}

export interface AggregatedOrderInfo {
  orderId: string
  shopId: string
  shopName: string
  quantity: number
  createdAt: Date
}

export interface GroupBuyOpportunity {
  id: string
  title: string
  itemSku: string
  itemName: string
  currentParticipants: number
  targetQuantity: number
  currentQuantity: number
  savingsPercentage: number
  deadline: Date
  status: 'active' | 'target-reached' | 'expired'
}

export interface EnhancedPurchaseOrder {
  id: string
  orderNumber: string
  shopId: string
  shopName: string
  items: PurchaseOrderItem[]
  subtotal: number
  deliveryFee: number
  total: number
  status: string
  createdAt: Date
  
  // Smart Purchasing Fields
  purchaseType: PurchaseType
  aggregationGroupId?: string
  groupBuyId?: string
  savingsAmount: number
  savingsMethod: SavingsMethod
  aggregatedWith?: string[] // Order IDs
  originalPrice?: number // Price before savings
}

export interface PurchaseOrderItem {
  id: string
  sku: string
  name: string
  quantity: number
  unitPrice: number
  totalPrice: number
  
  // Smart Purchasing Fields
  isAggregated?: boolean
  aggregationSavings?: number
  isGroupBuy?: boolean
  groupBuyId?: string
}

export interface SmartPurchaseAnalysis {
  canAggregate: boolean
  aggregationOpportunity?: AggregationOpportunity
  
  hasActiveGroupBuy: boolean
  groupBuyOpportunity?: GroupBuyOpportunity
  
  shouldCreateGroupBuy: boolean
  groupBuyPotential?: {
    estimatedSavings: number
    savingsPercentage: number
    suggestedMinQuantity: number
    suggestedDeadline: Date
  }
  
  bestOption: 'aggregation' | 'group-buy' | 'create-group-buy' | 'individual'
  estimatedTotalSavings: number
}


