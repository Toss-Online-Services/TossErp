// GET /api/pools/:id/savings - Calculate savings for a participant
import { defineEventHandler, getRouterParam, getQuery, createError } from 'h3'
import type { PoolSavingsCalculation } from '~/types/group-buying'

export default defineEventHandler(async (event) => {
  const poolId = getRouterParam(event, 'id')
  const query = getQuery(event)
  const shopId = query.shopId as string
  
  if (!poolId) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Pool ID is required'
    })
  }
  
  if (!shopId) {
    throw createError({
      statusCode: 400,
      statusMessage: 'shopId query parameter is required'
    })
  }
  
  // TODO: Fetch pool and participant from database
  // const pool = await db.pools.findById(poolId)
  // const participant = pool.participants.find(p => p.shopId === shopId)
  
  // Mock data for calculation
  const currentPrice = 12.50 // Regular solo price per unit
  const poolPrice = 10.00 // Pool price per unit
  const quantityCommitted = 20 // Participant's committed quantity
  const deliverySavings = 15 // Savings from shared logistics
  
  const regularCost = quantityCommitted * currentPrice
  const poolCost = quantityCommitted * poolPrice
  const savings = regularCost - poolCost
  const savingsPercentage = (savings / regularCost) * 100
  const totalSavings = savings + deliverySavings
  
  const calculation: PoolSavingsCalculation = {
    poolId,
    regularCost,
    poolCost,
    savings,
    savingsPercentage: Number(savingsPercentage.toFixed(2)),
    deliverySavings,
    totalSavings
  }
  
  return calculation
})

