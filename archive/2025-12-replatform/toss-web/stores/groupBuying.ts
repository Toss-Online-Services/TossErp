import { defineStore } from 'pinia'

export const useGroupBuyingStore = defineStore('groupBuying', () => {
  const {
    createPool,
    getActivePools,
    getPoolById,
    joinPool,
    confirmPool,
    generateAggregatedPO,
    getMyParticipations,
    getNearbyOpportunities
  } = useGroupBuying()

  // State
  const activePools = ref<any[]>([])
  const myLeadPools = ref<any[]>([])
  const myParticipations = ref<any[]>([])
  const availablePools = ref<any[]>([])
  const currentPool = ref<any | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const totalSavings = ref(0)

  // Computed
  const leaderPools = computed(() => myLeadPools.value)
  const participantPools = computed(() => myParticipations.value)
  const myPools = computed(() => [...myLeadPools.value, ...myParticipations.value])
  const joinablePools = computed(() => availablePools.value.filter((p: any) => p.status === 'open'))
  const openPools = computed(() => activePools.value.filter((p: any) => p.status === 'open'))
  const pendingPools = computed(() => activePools.value.filter((p: any) => p.status === 'pending'))
  const confirmedPools = computed(() => activePools.value.filter((p: any) => p.status === 'confirmed'))

  // Actions
  const fetchActivePools = async (params?: any) => {
    loading.value = true
    error.value = null
    try {
      const response = await getActivePools(params)
      activePools.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch pools'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchPoolById = async (id: number) => {
    loading.value = true
    error.value = null
    try {
      const pool = await getPoolById(id)
      currentPool.value = pool
      
      // Update in active pools list if exists
      const index = activePools.value.findIndex((p: any) => p.id === id)
      if (index !== -1) {
        activePools.value[index] = pool
      } else {
        activePools.value.push(pool)
      }
      
      return pool
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const createNewPool = async (poolData: any) => {
    loading.value = true
    error.value = null
    try {
      const result = await createPool(poolData)
      await fetchActivePools()
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to create pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const joinExistingPool = async (poolId: number, shopId: number, quantity: number) => {
    loading.value = true
    error.value = null
    try {
      const result = await joinPool(poolId, { shopId, quantity })
      await fetchPoolById(poolId)
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to join pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const confirmExistingPool = async (poolId: number) => {
    loading.value = true
    error.value = null
    try {
      const result = await confirmPool(poolId)
      await fetchPoolById(poolId)
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to confirm pool'
      throw err
    } finally {
      loading.value = false
    }
  }

  const generatePO = async (poolId: number) => {
    loading.value = true
    error.value = null
    try {
      const result = await generateAggregatedPO(poolId)
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to generate PO'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchMyParticipations = async (shopId: number) => {
    loading.value = true
    error.value = null
    try {
      const response = await getMyParticipations(shopId)
      myParticipations.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch participations'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchNearbyOpportunities = async (shopId: number, maxDistanceKm?: number) => {
    loading.value = true
    error.value = null
    try {
      const response = await getNearbyOpportunities({ shopId, maxDistanceKm })
      availablePools.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch opportunities'
      throw err
    } finally {
      loading.value = false
    }
  }

  const clearError = () => {
    error.value = null
  }

  const clearState = () => {
    activePools.value = []
    myLeadPools.value = []
    myParticipations.value = []
    availablePools.value = []
    currentPool.value = null
    error.value = null
    totalSavings.value = 0
  }

  return {
    // State
    activePools,
    myLeadPools,
    myParticipations,
    availablePools,
    currentPool,
    loading,
    error,
    totalSavings,
    
    // Computed
    leaderPools,
    participantPools,
    myPools,
    joinablePools,
    openPools,
    pendingPools,
    confirmedPools,
    
    // Actions
    fetchActivePools,
    fetchPoolById,
    createNewPool,
    joinExistingPool,
    confirmExistingPool,
    generatePO,
    fetchMyParticipations,
    fetchNearbyOpportunities,
    clearError,
    clearState
  }
})
