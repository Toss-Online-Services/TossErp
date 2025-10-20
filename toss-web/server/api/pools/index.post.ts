// POST /api/pools - Create a new pool
import { defineEventHandler, readBody, createError } from 'h3'
import type { CreatePoolDto, Pool, PoolParticipant } from '~/types/group-buying'

export default defineEventHandler(async (event) => {
  const body = await readBody<CreatePoolDto>(event)
  
  // Validation
  if (!body.sku || !body.itemId || !body.title || !body.description) {
    throw createError({
      statusCode: 400,
      statusMessage: 'sku, itemId, title, and description are required'
    })
  }
  
  if (!body.targetQuantity || body.targetQuantity <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'targetQuantity must be greater than 0'
    })
  }
  
  if (!body.deadline || new Date(body.deadline) <= new Date()) {
    throw createError({
      statusCode: 400,
      statusMessage: 'deadline must be in the future'
    })
  }
  
  if (!body.area || !body.supplierId) {
    throw createError({
      statusCode: 400,
      statusMessage: 'area and supplierId are required'
    })
  }
  
  if (body.currentPrice <= 0 || body.poolPrice <= 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'currentPrice and poolPrice must be greater than 0'
    })
  }
  
  if (body.poolPrice >= body.currentPrice) {
    throw createError({
      statusCode: 400,
      statusMessage: 'poolPrice must be less than currentPrice to provide savings'
    })
  }
  
  // Calculate savings percentage
  const savingsPercentage = ((body.currentPrice - body.poolPrice) / body.currentPrice) * 100
  
  // Get user info (in production, from auth session)
  const leadShopId = event.context.user?.shopId || 'shop-demo'
  const leadShopName = event.context.user?.shopName || 'Demo Shop'
  
  // Create pool with lead as first participant
  const poolId = `pool-${Date.now()}`
  const leadParticipant: PoolParticipant = {
    id: `part-${Date.now()}`,
    poolId,
    shopId: leadShopId,
    shopName: leadShopName,
    quantityCommitted: 0, // Lead hasn't committed yet
    costShare: 0,
    deliveryFeeShare: 0,
    status: 'joined',
    joinedAt: new Date(),
    paymentStatus: 'pending'
  }
  
  const newPool: Pool = {
    id: poolId,
    tenantId: event.context.user?.tenantId || 'tenant1',
    sku: body.sku,
    itemId: body.itemId,
    title: body.title,
    description: body.description,
    targetQuantity: body.targetQuantity,
    minParticipants: body.minParticipants || 2,
    maxParticipants: body.maxParticipants || 10,
    deadline: new Date(body.deadline),
    area: body.area,
    currentPrice: body.currentPrice,
    poolPrice: body.poolPrice,
    savingsPercentage: Number(savingsPercentage.toFixed(2)),
    priceBreaks: body.priceBreaks,
    status: 'open',
    currentCommitment: 0,
    participantCount: 1,
    splitRule: body.splitRule || 'by-units',
    leadShopId,
    leadShopName,
    participants: [leadParticipant],
    supplierId: body.supplierId,
    supplierName: body.supplierName,
    extendedOnce: false,
    createdAt: new Date(),
    updatedAt: new Date()
  }
  
  // TODO: Save to database
  // await db.pools.create(newPool)
  
  // TODO: Send notifications to shops in the area
  // await notifyNearbyShops(newPool)
  
  return newPool
})

