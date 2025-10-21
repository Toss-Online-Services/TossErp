/**
 * Smart Purchasing Composable
 * Handles order aggregation and group buying logic
 */

import type {
  SmartPurchaseAnalysis,
  AggregationOpportunity,
  GroupBuyOpportunity,
  PurchaseOrderItem
} from '~/types/smart-purchasing'

export const useSmartPurchasing = () => {
  const toast = useToast()

  /**
   * Analyze cart items for smart purchasing opportunities
   */
  const analyzeCartForOpportunities = async (items: PurchaseOrderItem[]): Promise<SmartPurchaseAnalysis> => {
    try {
      // Check for aggregation opportunities
      const aggregationCheck = await checkAggregationOpportunities(items)
      
      // Check for active group buys
      const groupBuyCheck = await checkActiveGroupBuys(items)
      
      // Calculate group buy potential
      const groupBuyPotential = calculateGroupBuyPotential(items)
      
      // Determine best option
      const bestOption = determineBestOption(aggregationCheck, groupBuyCheck, groupBuyPotential)
      
      // Calculate total savings
      const estimatedTotalSavings = calculateTotalSavings(aggregationCheck, groupBuyCheck, groupBuyPotential)
      
      return {
        canAggregate: aggregationCheck.canAggregate,
        aggregationOpportunity: aggregationCheck.opportunity,
        
        hasActiveGroupBuy: groupBuyCheck.hasActive,
        groupBuyOpportunity: groupBuyCheck.opportunity,
        
        shouldCreateGroupBuy: groupBuyPotential.shouldCreate,
        groupBuyPotential: groupBuyPotential.details,
        
        bestOption,
        estimatedTotalSavings
      }
    } catch (error) {
      console.error('Error analyzing smart purchase opportunities:', error)
      return getDefaultAnalysis()
    }
  }

  /**
   * Check if items can be aggregated with pending orders
   */
  const checkAggregationOpportunities = async (items: PurchaseOrderItem[]) => {
    try {
      // Get pending orders from last 24 hours
      const response = await $fetch('/api/purchasing/aggregation/check', {
        method: 'POST',
        body: { items }
      })
      
      return {
        canAggregate: response.canAggregate,
        opportunity: response.opportunity as AggregationOpportunity | undefined
      }
    } catch (error) {
      console.error('Error checking aggregation:', error)
      return { canAggregate: false, opportunity: undefined }
    }
  }

  /**
   * Check if there are active group buys for items
   */
  const checkActiveGroupBuys = async (items: PurchaseOrderItem[]) => {
    try {
      const skus = items.map(item => item.sku)
      const response = await $fetch('/api/purchasing/group-buys/active', {
        method: 'POST',
        body: { skus }
      })
      
      return {
        hasActive: response.hasActive,
        opportunity: response.opportunity as GroupBuyOpportunity | undefined
      }
    } catch (error) {
      console.error('Error checking group buys:', error)
      return { hasActive: false, opportunity: undefined }
    }
  }

  /**
   * Calculate if creating a group buy would be beneficial
   */
  const calculateGroupBuyPotential = (items: PurchaseOrderItem[]) => {
    // Calculate total value
    const totalValue = items.reduce((sum, item) => sum + item.totalPrice, 0)
    
    // Check if any item has large quantity
    const largeQuantityItem = items.find(item => item.quantity >= 50)
    
    // Estimated savings if others join (15-25% typical)
    const estimatedSavingsPercentage = largeQuantityItem ? 20 : 15
    const estimatedSavings = (totalValue * estimatedSavingsPercentage) / 100
    
    // Should create if potential savings > R500
    const shouldCreate = estimatedSavings > 500
    
    return {
      shouldCreate,
      details: shouldCreate ? {
        estimatedSavings,
        savingsPercentage: estimatedSavingsPercentage,
        suggestedMinQuantity: largeQuantityItem ? largeQuantityItem.quantity : 100,
        suggestedDeadline: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000) // 7 days
      } : undefined
    }
  }

  /**
   * Determine the best purchasing option
   */
  const determineBestOption = (
    aggregation: any,
    groupBuy: any,
    potential: any
  ): 'aggregation' | 'group-buy' | 'create-group-buy' | 'individual' => {
    // Priority 1: Active group buy with good savings
    if (groupBuy.hasActive && groupBuy.opportunity?.savingsPercentage >= 15) {
      return 'group-buy'
    }
    
    // Priority 2: Aggregation opportunity
    if (aggregation.canAggregate && aggregation.opportunity?.savingsPercentage >= 10) {
      return 'aggregation'
    }
    
    // Priority 3: Create group buy if high potential
    if (potential.shouldCreate && potential.details?.savingsPercentage >= 15) {
      return 'create-group-buy'
    }
    
    return 'individual'
  }

  /**
   * Calculate total estimated savings
   */
  const calculateTotalSavings = (aggregation: any, groupBuy: any, potential: any): number => {
    if (groupBuy.hasActive) {
      return groupBuy.opportunity?.estimatedSavings || 0
    }
    
    if (aggregation.canAggregate) {
      return aggregation.opportunity?.estimatedSavings || 0
    }
    
    if (potential.shouldCreate) {
      return potential.details?.estimatedSavings || 0
    }
    
    return 0
  }

  /**
   * Join an active group buy
   */
  const joinGroupBuy = async (groupBuyId: string, items: PurchaseOrderItem[]) => {
    try {
      // Calculate total quantity from items
      const totalQuantity = items.reduce((sum, item) => sum + item.quantity, 0)
      
      // Get shop info (in production, this would come from auth/session)
      const shopId = 'SHOP-' + Date.now()
      const shopName = 'My Shop' // Would come from session/auth
      
      const response = await $fetch(`/api/pools/${groupBuyId}/join`, {
        method: 'PATCH',
        body: {
          shopId,
          shopName,
          quantityCommitted: totalQuantity
        }
      })
      
      // Calculate savings
      const savings = response.savingsPercentage 
        ? Math.round((items[0].totalPrice * response.savingsPercentage) / 100)
        : 0
      
      toast.success(`Joined group buy! Estimated savings: R${savings}`)
      return { ...response, savings }
    } catch (error) {
      console.error('Error joining group buy:', error)
      toast.error('Failed to join group buy. Please try again.')
      throw error
    }
  }

  /**
   * Create order with auto-aggregation
   */
  const createAggregatedOrder = async (orderData: any, aggregationGroupId: string) => {
    try {
      const response = await $fetch('/api/purchasing/orders/aggregated', {
        method: 'POST',
        body: {
          ...orderData,
          aggregationGroupId,
          purchaseType: 'aggregated'
        }
      })
      
      toast.success(`Order created! Saved R${response.savings} through auto-aggregation!`)
      return response
    } catch (error) {
      toast.error('Failed to create aggregated order')
      throw error
    }
  }

  /**
   * Create a new group buy from order
   */
  const createGroupBuyFromOrder = async (orderData: any, groupBuyDetails: any) => {
    try {
      const response = await $fetch('/api/pools', {
        method: 'POST',
        body: {
          title: groupBuyDetails.title,
          description: groupBuyDetails.description,
          items: orderData.items,
          minQuantity: groupBuyDetails.minQuantity,
          targetPrice: groupBuyDetails.targetPrice,
          deadline: groupBuyDetails.deadline,
          category: groupBuyDetails.category
        }
      })
      
      toast.success('Group buy created! Invite others to unlock savings!')
      return response
    } catch (error) {
      toast.error('Failed to create group buy')
      throw error
    }
  }

  /**
   * Get default analysis when checks fail
   */
  const getDefaultAnalysis = (): SmartPurchaseAnalysis => ({
    canAggregate: false,
    hasActiveGroupBuy: false,
    shouldCreateGroupBuy: false,
    bestOption: 'individual',
    estimatedTotalSavings: 0
  })

  return {
    analyzeCartForOpportunities,
    joinGroupBuy,
    createAggregatedOrder,
    createGroupBuyFromOrder
  }
}

