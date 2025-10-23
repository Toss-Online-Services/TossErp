// GET /api/pools/:id - Get pool details
import { defineEventHandler, getRouterParam, createError } from 'h3'
import type { Pool } from '~/types/group-buying'

export default defineEventHandler(async (event) => {
  const poolId = getRouterParam(event, 'id')
  
  if (!poolId) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Pool ID is required'
    })
  }
  
  // TODO: Fetch from database
  // const pool = await db.pools.findById(poolId)
  
  // Mock data for MVP
  const mockPool: Pool = {
    id: poolId,
    tenantId: 'tenant1',
    sku: 'BREAD-001',
    itemId: 'item-1',
    title: 'White Bread Bulk Order',
    description: 'Fresh white bread loaves for the week. Join to save 20% on your weekly bread supply!',
    targetQuantity: 100,
    minParticipants: 3,
    maxParticipants: 10,
    deadline: new Date(Date.now() + 48 * 60 * 60 * 1000),
    area: 'soweto-north',
    currentPrice: 12.50,
    poolPrice: 10.00,
    savingsPercentage: 20,
    status: 'open',
    currentCommitment: 45,
    participantCount: 3,
    splitRule: 'by-units',
    leadShopId: 'shop-1',
    leadShopName: 'Thabo\'s Spaza',
    participants: [
      {
        id: 'part-1',
        poolId,
        shopId: 'shop-1',
        shopName: 'Thabo\'s Spaza',
        quantityCommitted: 20,
        costShare: 200,
        deliveryFeeShare: 15,
        status: 'joined',
        joinedAt: new Date(Date.now() - 24 * 60 * 60 * 1000),
        paymentStatus: 'pending'
      },
      {
        id: 'part-2',
        poolId,
        shopId: 'shop-2',
        shopName: 'Nomsa\'s Store',
        quantityCommitted: 15,
        costShare: 150,
        deliveryFeeShare: 15,
        status: 'joined',
        joinedAt: new Date(Date.now() - 12 * 60 * 60 * 1000),
        paymentStatus: 'pending'
      },
      {
        id: 'part-3',
        poolId,
        shopId: 'shop-3',
        shopName: 'Lucky\'s Grocery',
        quantityCommitted: 10,
        costShare: 100,
        deliveryFeeShare: 15,
        status: 'joined',
        joinedAt: new Date(Date.now() - 6 * 60 * 60 * 1000),
        paymentStatus: 'pending'
      }
    ],
    supplierId: 'supplier-1',
    supplierName: 'Albany Bread Suppliers',
    inviteLink: `https://toss.app/pools/${poolId}/join`,
    extendedOnce: false,
    createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    updatedAt: new Date()
  }
  
  if (!mockPool) {
    throw createError({
      statusCode: 404,
      statusMessage: 'Pool not found'
    })
  }
  
  return mockPool
})

