/**
 * Create an aggregated order
 * POST /api/buying/orders/aggregated
 */

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const { items, aggregationGroupId, purchaseType } = body

  try {
    // Calculate savings from aggregation
    const savings = calculateAggregationSavings(items, aggregationGroupId)
    
    // Create the order with aggregation details
    const order = {
      id: `ORD-${Date.now()}`,
      orderNumber: `PO-${Date.now()}`,
      items,
      purchaseType: 'aggregated',
      aggregationGroupId,
      savingsAmount: savings,
      savingsMethod: 'auto-aggregation',
      status: 'pending',
      createdAt: new Date()
    }

    // In production: Save to database and notify aggregation participants
    console.log('Created aggregated order:', order)

    return {
      success: true,
      order,
      savings,
      message: `Order aggregated with ${getAggregationPartners(aggregationGroupId)} other shops`
    }
  } catch (error) {
    console.error('Error creating aggregated order:', error)
    throw createError({
      statusCode: 500,
      message: 'Failed to create aggregated order'
    })
  }
})

function calculateAggregationSavings(items: any[], aggregationGroupId: string) {
  // Mock calculation - 10% savings on aggregated orders
  const total = items.reduce((sum, item) => sum + (item.quantity * item.unitPrice), 0)
  return Math.round(total * 0.10)
}

function getAggregationPartners(aggregationGroupId: string) {
  // Mock - return number of partners
  return 2
}


