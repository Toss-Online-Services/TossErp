/**
 * Check for order aggregation opportunities
 * POST /api/buying/aggregation/check
 */

import type { AggregationOpportunity, PurchaseOrderItem } from '~/types/smart-buying'

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const { items } = body as { items: PurchaseOrderItem[] }

  try {
    // Mock implementation - In production, this would query your database
    // for pending orders from the last 24 hours
    const pendingOrders = await findPendingOrders(items)
    
    if (pendingOrders.length === 0) {
      return {
        canAggregate: false,
        opportunity: null
      }
    }

    // Calculate aggregation opportunity
    const opportunity = calculateAggregation(items, pendingOrders)
    
    return {
      canAggregate: opportunity.totalQuantity >= opportunity.metadata?.minQuantityForDiscount || 100,
      opportunity
    }
  } catch (error) {
    console.error('Error checking aggregation:', error)
    return {
      canAggregate: false,
      opportunity: null
    }
  }
})

/**
 * Find pending orders with matching items (last 24 hours)
 */
async function findPendingOrders(items: PurchaseOrderItem[]) {
  // Mock data - Replace with actual database query
  const mockPendingOrders = [
    {
      orderId: 'ORD-001',
      shopId: 'SHOP-B',
      shopName: 'Soweto Spaza',
      items: [
        { sku: 'MLK-FC-1L', name: 'Full Cream Milk', quantity: 30, unitPrice: 18.99 }
      ],
      createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000) // 2 hours ago
    },
    {
      orderId: 'ORD-002',
      shopId: 'SHOP-C',
      shopName: 'Kwa-Thema Traders',
      items: [
        { sku: 'MLK-FC-1L', name: 'Full Cream Milk', quantity: 25, unitPrice: 18.99 }
      ],
      createdAt: new Date(Date.now() - 5 * 60 * 60 * 1000) // 5 hours ago
    }
  ]

  // Filter orders that have matching SKUs
  const itemSkus = items.map(item => item.sku)
  return mockPendingOrders.filter(order =>
    order.items.some(item => itemSkus.includes(item.sku))
  )
}

/**
 * Calculate aggregation details
 */
function calculateAggregation(
  currentItems: PurchaseOrderItem[],
  pendingOrders: any[]
): AggregationOpportunity {
  // For simplicity, aggregate the first matching item
  const firstItem = currentItems[0]
  
  // Find all pending orders with this item
  const matchingOrders = pendingOrders.filter(order =>
    order.items.some((item: any) => item.sku === firstItem.sku)
  )

  // Calculate total quantity
  const currentQuantity = firstItem.quantity
  const pendingQuantity = matchingOrders.reduce((sum, order) => {
    const item = order.items.find((i: any) => i.sku === firstItem.sku)
    return sum + (item?.quantity || 0)
  }, 0)
  
  const totalQuantity = currentQuantity + pendingQuantity

  // Calculate savings (5% for 100+ units, 10% for 200+, 15% for 500+)
  let savingsPercentage = 0
  if (totalQuantity >= 500) savingsPercentage = 15
  else if (totalQuantity >= 200) savingsPercentage = 10
  else if (totalQuantity >= 100) savingsPercentage = 5

  const unitPrice = firstItem.unitPrice
  const totalValue = totalQuantity * unitPrice
  const estimatedSavings = (totalValue * savingsPercentage) / 100

  // Build aggregated order info
  const participatingOrders = matchingOrders.map(order => ({
    orderId: order.orderId,
    shopId: order.shopId,
    shopName: order.shopName,
    quantity: order.items.find((i: any) => i.sku === firstItem.sku)?.quantity || 0,
    createdAt: order.createdAt
  }))

  return {
    id: `AGG-${Date.now()}`,
    itemSku: firstItem.sku,
    itemName: firstItem.name,
    totalQuantity,
    participatingOrders,
    estimatedSavings: Math.round(estimatedSavings),
    savingsPercentage,
    expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000), // 24 hours
    status: 'pending',
    metadata: {
      currentQuantity,
      pendingQuantity,
      minQuantityForDiscount: 100
    }
  } as any
}


