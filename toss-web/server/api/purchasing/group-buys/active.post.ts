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
      description: 'Bulk purchase for Full Cream Milk 1L from various shops.',
      itemSku: 'MLK-FC-1L',
      itemName: 'Full Cream Milk 1L',
      currentParticipants: 12,
      targetQuantity: 500,
      currentQuantity: 385,
      savingsPercentage: 18,
      deadline: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), // 2 days from now
    },
    {
      id: 'GB-002',
      title: 'Coca-Cola 2L Group Order',
      description: 'Bulk purchase for Coca-Cola 2L from various shops.',
      itemSku: 'BEV-COK-2L',
      itemName: 'Coca-Cola 2L',
      currentParticipants: 8,
      targetQuantity: 300,
      currentQuantity: 245,
      savingsPercentage: 15,
      deadline: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000), // 3 days from now
    },
    {
      id: 'GB-003',
      title: 'White Bread Bulk Order',
      description: 'Bulk purchase for White Bread from various shops.',
      itemSku: 'BREAD-WHITE',
      itemName: 'White Bread',
      currentParticipants: 15,
      targetQuantity: 400,
      currentQuantity: 320,
      savingsPercentage: 20,
      deadline: new Date(Date.now() + 1 * 24 * 60 * 60 * 1000), // 1 day from now
    },
    {
      id: 'GB-004',
      title: 'Milk 2L Group Order',
      description: 'Bulk purchase for 2L Full Cream Milk from various shops.',
      itemSku: 'MILK-FC-2L',
      itemName: 'Full Cream Milk 2L',
      currentParticipants: 10,
      targetQuantity: 350,
      currentQuantity: 280,
      savingsPercentage: 16,
      deadline: new Date(Date.now() + 4 * 24 * 60 * 60 * 1000), // 4 days from now
    }
  ]

  // Filter to only active group buys that match the SKUs
  return mockGroupBuys.filter(gb => 
    skus.includes(gb.itemSku) && 
    gb.deadline > new Date()
  )
}

