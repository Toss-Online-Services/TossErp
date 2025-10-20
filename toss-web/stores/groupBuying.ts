// Group Buying Pinia Store
// Manages pool state, participant actions, and savings tracking

import { defineStore } from 'pinia'
import type {
  Pool,
  PoolParticipant,
  CreatePoolDto,
  JoinPoolDto,
  PoolSavingsCalculation,
  PoolAnalytics,
  ShopPoolStats,
  PoolFilterDto
} from '~/types/group-buying'

interface GroupBuyingState {
  // Pools
  activePools: Pool[]
  myLeadPools: Pool[]
  myParticipations: Pool[]
  availablePools: Pool[]
  
  // Current pool details
  currentPool: Pool | null
  
  // Analytics
  analytics: PoolAnalytics | null
  myStats: ShopPoolStats | null
  totalSavings: number
  
  // UI state
  loading: boolean
  error: string | null
}

export const useGroupBuyingStore = defineStore('groupBuying', {
  state: (): GroupBuyingState => ({
    activePools: [],
    myLeadPools: [],
    myParticipations: [],
    availablePools: [],
    currentPool: null,
    analytics: null,
    myStats: null,
    totalSavings: 0,
    loading: false,
    error: null
  }),

  getters: {
    // Pools I'm leading
    leaderPools: (state) => state.myLeadPools,
    
    // Pools I'm participating in (but not leading)
    participantPools: (state) => state.myParticipations,
    
    // All my pools (leading + participating)
    myPools: (state) => [...state.myLeadPools, ...state.myParticipations],
    
    // Pools available to join in my area
    joinablePools: (state) => state.availablePools.filter(p => p.status === 'open'),
    
    // Pools by status
    openPools: (state) => state.activePools.filter(p => p.status === 'open'),
    pendingPools: (state) => state.activePools.filter(p => p.status === 'pending'),
    confirmedPools: (state) => state.activePools.filter(p => p.status === 'confirmed'),
    
    // Pool fill progress
    poolProgress: (state) => (poolId: string) => {
      const pool = state.activePools.find(p => p.id === poolId)
      if (!pool) return 0
      return (pool.currentCommitment / pool.targetQuantity) * 100
    },
    
    // Check if current user is pool leader
    isPoolLeader: (state) => (poolId: string, shopId: string) => {
      const pool = state.activePools.find(p => p.id === poolId)
      return pool?.leadShopId === shopId
    },
    
    // Get my participation in a pool
    myParticipation: (state) => (poolId: string, shopId: string) => {
      const pool = state.activePools.find(p => p.id === poolId)
      return pool?.participants.find(p => p.shopId === shopId)
    }
  },

  actions: {
    // Fetch all pools with optional filters
    async fetchPools(filters?: PoolFilterDto) {
      this.loading = true
      this.error = null
      
      try {
        const response = await $fetch<{ pools: Pool[]; totalCount: number }>('/api/pools', {
          method: 'GET',
          query: filters
        })
        
        this.activePools = response.pools
        return response
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch pools'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Fetch pools I'm leading
    async fetchMyLeadPools(shopId: string) {
      this.loading = true
      
      try {
        const response = await $fetch<{ pools: Pool[] }>('/api/pools', {
          method: 'GET',
          query: { leadShopId: shopId }
        })
        
        this.myLeadPools = response.pools
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch lead pools'
      } finally {
        this.loading = false
      }
    },

    // Fetch pools I'm participating in
    async fetchMyParticipations(shopId: string) {
      this.loading = true
      
      try {
        const response = await $fetch<{ pools: Pool[] }>('/api/pools', {
          method: 'GET',
          query: { participantShopId: shopId }
        })
        
        this.myParticipations = response.pools
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch participations'
      } finally {
        this.loading = false
      }
    },

    // Fetch available pools in my area
    async fetchAvailablePools(area: string, shopId: string) {
      this.loading = true
      
      try {
        const response = await $fetch<{ pools: Pool[] }>('/api/pools', {
          method: 'GET',
          query: { area, status: 'open' }
        })
        
        // Filter out pools I'm already in
        this.availablePools = response.pools.filter(
          pool => !pool.participants.some(p => p.shopId === shopId)
        )
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch available pools'
      } finally {
        this.loading = false
      }
    },

    // Get specific pool details
    async fetchPool(poolId: string) {
      this.loading = true
      
      try {
        const pool = await $fetch<Pool>(`/api/pools/${poolId}`)
        this.currentPool = pool
        
        // Update in active pools list if exists
        const index = this.activePools.findIndex(p => p.id === poolId)
        if (index !== -1) {
          this.activePools[index] = pool
        } else {
          this.activePools.push(pool)
        }
        
        return pool
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch pool'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Create new pool
    async createPool(poolData: CreatePoolDto) {
      this.loading = true
      this.error = null
      
      try {
        const pool = await $fetch<Pool>('/api/pools', {
          method: 'POST',
          body: poolData
        })
        
        this.myLeadPools.unshift(pool)
        this.activePools.unshift(pool)
        this.currentPool = pool
        
        return pool
      } catch (err: any) {
        this.error = err.message || 'Failed to create pool'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Join existing pool
    async joinPool(joinData: JoinPoolDto) {
      this.loading = true
      this.error = null
      
      try {
        const pool = await $fetch<Pool>(`/api/pools/${joinData.poolId}/join`, {
          method: 'PATCH',
          body: joinData
        })
        
        // Update pool in all relevant lists
        this.updatePoolInLists(pool)
        this.myParticipations.push(pool)
        
        return pool
      } catch (err: any) {
        this.error = err.message || 'Failed to join pool'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Leave pool (before confirmation)
    async leavePool(poolId: string, shopId: string) {
      this.loading = true
      
      try {
        const pool = await $fetch<Pool>(`/api/pools/${poolId}/leave`, {
          method: 'PATCH',
          body: { shopId }
        })
        
        // Remove from participations
        this.myParticipations = this.myParticipations.filter(p => p.id !== poolId)
        this.updatePoolInLists(pool)
        
        return pool
      } catch (err: any) {
        this.error = err.message || 'Failed to leave pool'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Confirm pool (moves to pending/confirmed status)
    async confirmPool(poolId: string) {
      this.loading = true
      
      try {
        const pool = await $fetch<Pool>(`/api/pools/${poolId}/confirm`, {
          method: 'PATCH',
          body: { createPurchaseOrder: true }
        })
        
        this.updatePoolInLists(pool)
        
        return pool
      } catch (err: any) {
        this.error = err.message || 'Failed to confirm pool'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Cancel pool (by leader)
    async cancelPool(poolId: string, reason?: string) {
      this.loading = true
      
      try {
        const pool = await $fetch<Pool>(`/api/pools/${poolId}/cancel`, {
          method: 'PATCH',
          body: { reason }
        })
        
        this.updatePoolInLists(pool)
        
        return pool
      } catch (err: any) {
        this.error = err.message || 'Failed to cancel pool'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Generate WhatsApp invite link
    async generateInvite(poolId: string, phoneNumbers: string[]) {
      this.loading = true
      
      try {
        const response = await $fetch<{ inviteLink: string; messagesSent: number }>(`/api/pools/${poolId}/invite`, {
          method: 'POST',
          body: { phoneNumbers }
        })
        
        return response
      } catch (err: any) {
        this.error = err.message || 'Failed to generate invite'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Calculate savings for a pool
    async calculateSavings(poolId: string, shopId: string): Promise<PoolSavingsCalculation> {
      try {
        const savings = await $fetch<PoolSavingsCalculation>(`/api/pools/${poolId}/savings`, {
          method: 'GET',
          query: { shopId }
        })
        
        return savings
      } catch (err: any) {
        this.error = err.message || 'Failed to calculate savings'
        throw err
      }
    },

    // Fetch analytics
    async fetchAnalytics() {
      try {
        const analytics = await $fetch<PoolAnalytics>('/api/pools/analytics')
        this.analytics = analytics
        return analytics
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch analytics'
      }
    },

    // Fetch my shop stats
    async fetchMyStats(shopId: string) {
      try {
        const stats = await $fetch<ShopPoolStats>(`/api/pools/stats/${shopId}`)
        this.myStats = stats
        this.totalSavings = stats.totalSavings
        return stats
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch stats'
      }
    },

    // Utility: Update pool in all relevant lists
    updatePoolInLists(pool: Pool) {
      const updateList = (list: Pool[]) => {
        const index = list.findIndex(p => p.id === pool.id)
        if (index !== -1) {
          list[index] = pool
        }
      }
      
      updateList(this.activePools)
      updateList(this.myLeadPools)
      updateList(this.myParticipations)
      updateList(this.availablePools)
      
      if (this.currentPool?.id === pool.id) {
        this.currentPool = pool
      }
    },

    // Clear all state
    clearState() {
      this.activePools = []
      this.myLeadPools = []
      this.myParticipations = []
      this.availablePools = []
      this.currentPool = null
      this.analytics = null
      this.myStats = null
      this.totalSavings = 0
      this.error = null
    }
  }
})

