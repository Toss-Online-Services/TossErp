// GET /api/pools - List all pools with filters
import { defineEventHandler, getQuery } from 'h3'
import type { Pool, PoolFilterDto } from '~/types/group-buying'

export default defineEventHandler(async (event) => {
  const query = getQuery(event) as Partial<PoolFilterDto>
  
  // Mock data for MVP - replace with actual database queries
  const allPools: Pool[] = [
    {
      id: 'pool-1',
      tenantId: 'tenant1',
      sku: 'BREAD-001',
      itemId: 'item-1',
      title: 'White Bread Bulk Order',
      description: 'Fresh white bread loaves for the week',
      targetQuantity: 100,
      minParticipants: 3,
      maxParticipants: 10,
      deadline: new Date(Date.now() + 48 * 60 * 60 * 1000), // 2 days from now
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
          poolId: 'pool-1',
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
          poolId: 'pool-1',
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
          poolId: 'pool-1',
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
      extendedOnce: false,
      createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
      updatedAt: new Date()
    },
    {
      id: 'pool-2',
      tenantId: 'tenant1',
      sku: 'MILK-001',
      itemId: 'item-2',
      title: 'Fresh Milk 1L - Weekly Supply',
      description: 'Full cream fresh milk 1L bottles',
      targetQuantity: 60,
      minParticipants: 4,
      maxParticipants: 8,
      deadline: new Date(Date.now() + 24 * 60 * 60 * 1000), // 1 day from now
      area: 'soweto-north',
      currentPrice: 18.00,
      poolPrice: 15.50,
      savingsPercentage: 13.89,
      status: 'open',
      currentCommitment: 40,
      participantCount: 4,
      splitRule: 'flat',
      leadShopId: 'shop-2',
      leadShopName: 'Nomsa\'s Store',
      participants: [
        {
          id: 'part-4',
          poolId: 'pool-2',
          shopId: 'shop-2',
          shopName: 'Nomsa\'s Store',
          quantityCommitted: 15,
          costShare: 232.50,
          deliveryFeeShare: 12.50,
          status: 'joined',
          joinedAt: new Date(Date.now() - 36 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        },
        {
          id: 'part-5',
          poolId: 'pool-2',
          shopId: 'shop-4',
          shopName: 'Sbu\'s Corner Shop',
          quantityCommitted: 10,
          costShare: 155,
          deliveryFeeShare: 12.50,
          status: 'joined',
          joinedAt: new Date(Date.now() - 24 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        },
        {
          id: 'part-6',
          poolId: 'pool-2',
          shopId: 'shop-5',
          shopName: 'Zanele\'s Mini Market',
          quantityCommitted: 10,
          costShare: 155,
          deliveryFeeShare: 12.50,
          status: 'joined',
          joinedAt: new Date(Date.now() - 18 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        },
        {
          id: 'part-7',
          poolId: 'pool-2',
          shopId: 'shop-6',
          shopName: 'Mama Zulu\'s Tuck Shop',
          quantityCommitted: 5,
          costShare: 77.50,
          deliveryFeeShare: 12.50,
          status: 'joined',
          joinedAt: new Date(Date.now() - 6 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        }
      ],
      supplierId: 'supplier-2',
      supplierName: 'Clover Dairies',
      extendedOnce: false,
      createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
      updatedAt: new Date()
    },
    {
      id: 'pool-3',
      tenantId: 'tenant1',
      sku: 'SOAP-001',
      itemId: 'item-4',
      title: 'Washing Powder 1kg',
      description: 'OMO washing powder 1kg packets',
      targetQuantity: 50,
      minParticipants: 5,
      maxParticipants: 10,
      deadline: new Date(Date.now() + 72 * 60 * 60 * 1000), // 3 days from now
      area: 'alexandra',
      currentPrice: 45.00,
      poolPrice: 38.00,
      savingsPercentage: 15.56,
      status: 'pending',
      currentCommitment: 50,
      participantCount: 5,
      splitRule: 'by-units',
      leadShopId: 'shop-7',
      leadShopName: 'Alex Trading Store',
      participants: [
        {
          id: 'part-8',
          poolId: 'pool-3',
          shopId: 'shop-7',
          shopName: 'Alex Trading Store',
          quantityCommitted: 15,
          costShare: 570,
          deliveryFeeShare: 20,
          status: 'confirmed',
          joinedAt: new Date(Date.now() - 48 * 60 * 60 * 1000),
          confirmedAt: new Date(Date.now() - 1 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        },
        {
          id: 'part-9',
          poolId: 'pool-3',
          shopId: 'shop-8',
          shopName: 'Kasi Convenience',
          quantityCommitted: 10,
          costShare: 380,
          deliveryFeeShare: 20,
          status: 'confirmed',
          joinedAt: new Date(Date.now() - 36 * 60 * 60 * 1000),
          confirmedAt: new Date(Date.now() - 2 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        },
        {
          id: 'part-10',
          poolId: 'pool-3',
          shopId: 'shop-9',
          shopName: 'Gogo\'s Supplies',
          quantityCommitted: 10,
          costShare: 380,
          deliveryFeeShare: 20,
          status: 'joined',
          joinedAt: new Date(Date.now() - 24 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        },
        {
          id: 'part-11',
          poolId: 'pool-3',
          shopId: 'shop-10',
          shopName: 'Pan African Traders',
          quantityCommitted: 8,
          costShare: 304,
          deliveryFeeShare: 20,
          status: 'joined',
          joinedAt: new Date(Date.now() - 12 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        },
        {
          id: 'part-12',
          poolId: 'pool-3',
          shopId: 'shop-11',
          shopName: 'Lebo\'s Mini Shop',
          quantityCommitted: 7,
          costShare: 266,
          deliveryFeeShare: 20,
          status: 'joined',
          joinedAt: new Date(Date.now() - 6 * 60 * 60 * 1000),
          paymentStatus: 'pending'
        }
      ],
      supplierId: 'supplier-3',
      supplierName: 'Unilever SA',
      extendedOnce: false,
      createdAt: new Date(Date.now() - 4 * 24 * 60 * 60 * 1000),
      updatedAt: new Date()
    }
  ]
  
  // Apply filters
  let filteredPools = allPools
  
  if (query.status) {
    filteredPools = filteredPools.filter(p => p.status === query.status)
  }
  
  if (query.area) {
    filteredPools = filteredPools.filter(p => p.area === query.area)
  }
  
  if (query.sku) {
    filteredPools = filteredPools.filter(p => p.sku === query.sku)
  }
  
  if (query.supplierId) {
    filteredPools = filteredPools.filter(p => p.supplierId === query.supplierId)
  }
  
  if (query.leadShopId) {
    filteredPools = filteredPools.filter(p => p.leadShopId === query.leadShopId)
  }
  
  if (query.participantShopId) {
    filteredPools = filteredPools.filter(p =>
      p.participants.some(part => part.shopId === query.participantShopId)
    )
  }
  
  if (query.minSavingsPercentage) {
    filteredPools = filteredPools.filter(p => p.savingsPercentage >= (query.minSavingsPercentage || 0))
  }
  
  if (query.deadlineBefore) {
    const beforeDate = new Date(query.deadlineBefore)
    filteredPools = filteredPools.filter(p => new Date(p.deadline) <= beforeDate)
  }
  
  if (query.deadlineAfter) {
    const afterDate = new Date(query.deadlineAfter)
    filteredPools = filteredPools.filter(p => new Date(p.deadline) >= afterDate)
  }
  
  return {
    pools: filteredPools,
    totalCount: filteredPools.length
  }
})

