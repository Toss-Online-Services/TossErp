// PATCH /api/pools/:id/join - Join a pool
import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'
import type { JoinPoolDto, Pool, PoolParticipant } from '~/types/group-buying'

export default defineEventHandler(async (event) => {
  const poolId = getRouterParam(event, 'id')
  const body = await readBody<Omit<JoinPoolDto, 'poolId'>>(event)
  
  if (!poolId) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Pool ID is required'
    })
  }
  
  if (!body.shopId || !body.shopName) {
    throw createError({
      statusCode: 400,
      statusMessage: 'shopId and shopName are required'
    })
  }
  
  if (!body.quantityCommitted || body.quantityCommitted <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'quantityCommitted must be greater than 0'
    })
  }
  
  // TODO: Fetch pool from database
  // const pool = await db.pools.findById(poolId)
  
  // For now, we'll create a mock updated pool
  // In production, this would fetch, validate, update, and save
  
  // Validation checks (would be against actual pool data):
  // 1. Pool must be in 'open' status
  // 2. Shop not already in pool
  // 3. Pool not at max participants
  // 4. Commitment doesn't exceed remaining quantity
  
  const existingParticipant = false // Check if shop already joined
  if (existingParticipant) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Shop has already joined this pool'
    })
  }
  
  // Calculate cost share based on split rule
  const poolPrice = 10.00 // Would come from pool.poolPrice
  const splitRule = 'by-units' // Would come from pool.splitRule
  
  let costShare = 0
  if (splitRule === 'by-units') {
    costShare = body.quantityCommitted * poolPrice
  } else if (splitRule === 'flat') {
    // Flat split would be calculated differently
    // totalCost / numberOfParticipants
    costShare = (100 * poolPrice) / 4 // Example calculation
  }
  
  const newParticipant: PoolParticipant = {
    id: `part-${Date.now()}`,
    poolId,
    shopId: body.shopId,
    shopName: body.shopName,
    quantityCommitted: body.quantityCommitted,
    costShare,
    deliveryFeeShare: 0, // Will be calculated when delivery is arranged
    status: 'joined',
    joinedAt: new Date(),
    paymentStatus: 'pending'
  }
  
  // TODO: Add participant to pool in database
  // pool.participants.push(newParticipant)
  // pool.currentCommitment += body.quantityCommitted
  // pool.participantCount += 1
  // await db.pools.update(poolId, pool)
  
  // Mock updated pool response
  const updatedPool: Pool = {
    id: poolId,
    tenantId: 'tenant1',
    sku: 'BREAD-001',
    itemId: 'item-1',
    title: 'White Bread Bulk Order',
    description: 'Fresh white bread loaves for the week',
    targetQuantity: 100,
    minParticipants: 3,
    maxParticipants: 10,
    deadline: new Date(Date.now() + 48 * 60 * 60 * 1000),
    area: 'soweto-north',
    currentPrice: 12.50,
    poolPrice: 10.00,
    savingsPercentage: 20,
    status: 'open',
    currentCommitment: 45 + body.quantityCommitted,
    participantCount: 4,
    splitRule: 'by-units',
    leadShopId: 'shop-1',
    leadShopName: 'Thabo\'s Spaza',
    participants: [
      // ... existing participants
      newParticipant
    ],
    supplierId: 'supplier-1',
    supplierName: 'Albany Bread Suppliers',
    extendedOnce: false,
    createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    updatedAt: new Date()
  }
  
  // TODO: Send notifications
  // - Notify lead shop of new participant
  // - Notify all participants of pool progress
  // - Check if pool reached target (move to 'pending' status)
  
  return updatedPool
})

