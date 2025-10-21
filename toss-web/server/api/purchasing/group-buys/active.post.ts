/**
 * Check for active group buys matching items
 * POST /api/purchasing/group-buys/active
 */

import type { GroupBuyOpportunity } from '~/types/smart-purchasing'

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const { skus } = body as { skus: string[] }

  try {
    // Mock implementation - In production, query database for active group buys
    const activeGroupBuys = await findActiveGroupBuys(skus)
    
    if (activeGroupBuys.length === 0) {
      return {
        hasActive: false,
        opportunity: null
      }
    }

    // Return the best matching group buy
    const bestOpportunity = activeGroupBuys[0]
    
    return {
      hasActive: true,
      opportunity: bestOpportunity
    }
  } catch (error) {
    console.error('Error checking active group buys:', error)
    return {
      hasActive: false,
      opportunity: null
    }
  }
})

/**
 * Find active group buys for given SKUs
 */
async function findActiveGroupBuys(skus: string[]): Promise<GroupBuyOpportunity[]> {
  // Mock data - Replace with actual database query
  const mockGroupBuys: GroupBuyOpportunity[] = [
    {
      id: 'GB-001',
      title: 'Full Cream Milk Bulk Buy',
      itemSku: 'MLK-FC-1L',
      itemName: 'Full Cream Milk 1L',
      currentParticipants: 12,
      targetQuantity: 500,
      currentQuantity: 385,
      savingsPercentage: 18,
      deadline: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
      status: 'active'
    },
    {
      id: 'GB-002',
      title: 'Coca-Cola 2L Group Order',
      itemSku: 'BEV-COK-2L',
      itemName: 'Coca-Cola 2L',
      currentParticipants: 8,
      targetQuantity: 300,
      currentQuantity: 245,
      savingsPercentage: 15,
      deadline: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000), // 3 days from now
      status: 'active'
    }
  ]

  // Filter to only active group buys that match the SKUs
  return mockGroupBuys.filter(gb => 
    skus.includes(gb.itemSku) && 
    gb.status === 'active' &&
    gb.deadline > new Date()
  )
}

