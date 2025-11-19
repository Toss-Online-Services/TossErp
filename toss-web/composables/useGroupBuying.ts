/**
 * Group Buying API Composable
 * Connects to TOSS backend /api/group-buying endpoints
 */
export const useGroupBuying = () => {
  const { get, post } = useApi()

  /**
   * Create a new group buying pool
   */
  const createPool = async (poolData: {
    productId: number
    supplierId: number
    initiatorShopId: number
    targetQuantity: number
    productPrice: number
    targetDate: string
    description?: string
  }) => {
    return await post<{ id: number }>('/api/group-buying/pools', poolData)
  }

  /**
   * Get active (open) group buying pools
   */
  const getActivePools = async (params?: {
    productId?: number
    supplierId?: number
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/group-buying/pools/active', params)
  }

  /**
   * Get pool by ID with full details
   */
  const getPoolById = async (id: number) => {
    return await get<any>(`/api/group-buying/pools/${id}`)
  }

  /**
   * Join an existing group buying pool
   */
  const joinPool = async (poolId: number, participationData: {
    shopId: number
    quantity: number
  }) => {
    return await post<{ id: number }>(`/api/group-buying/pools/${poolId}/join`, participationData)
  }

  /**
   * Confirm a pool (close it and generate aggregated PO)
   */
  const confirmPool = async (poolId: number) => {
    return await post<any>(`/api/group-buying/pools/${poolId}/confirm`, {})
  }

  /**
   * Generate aggregated purchase order from confirmed pool
   */
  const generateAggregatedPO = async (poolId: number) => {
    return await post<{ id: number }>(`/api/group-buying/pools/${poolId}/generate-po`, {})
  }

  /**
   * Get my participations in group buying pools
   */
  const getMyParticipations = async (shopId: number) => {
    return await get<any>('/api/group-buying/participations', { shopId })
  }

  /**
   * Get nearby pool opportunities
   */
  const getNearbyOpportunities = async (params: {
    shopId: number
    maxDistanceKm?: number
  }) => {
    return await get<any>('/api/group-buying/opportunities', params)
  }

  return {
    createPool,
    getActivePools,
    getPoolById,
    joinPool,
    confirmPool,
    generateAggregatedPO,
    getMyParticipations,
    getNearbyOpportunities
  }
}
