// Shared Logistics Pinia Store
// Manages delivery runs, stops, and driver coordination

import { defineStore } from 'pinia'
import type {
  SharedRun,
  DeliveryStop,
  CreateSharedRunDto,
  CreateDeliveryStopDto,
  CapturePODDto,
  RunFilterDto,
  DeliveryCostBreakdown,
  DeliverySavingsCalculation,
  LogisticsAnalytics,
  DriverStats,
  RouteOptimizationRequest,
  RouteOptimizationResponse
} from '~/types/logistics'

interface SharedLogisticsState {
  // Runs
  activeRuns: SharedRun[]
  myRuns: SharedRun[] // Runs where my shop has a stop
  availableRuns: SharedRun[] // Runs with available capacity in my area
  
  // Current run details
  currentRun: SharedRun | null
  
  // My deliveries
  myDeliveries: DeliveryStop[]
  
  // Driver location tracking (for active deliveries)
  driverLocation: { lat: number; lng: number } | null
  trackingRunId: string | null
  
  // Analytics
  analytics: LogisticsAnalytics | null
  totalSavings: number
  
  // UI state
  loading: boolean
  error: string | null
}

export const useSharedLogisticsStore = defineStore('sharedLogistics', {
  state: (): SharedLogisticsState => ({
    activeRuns: [],
    myRuns: [],
    availableRuns: [],
    currentRun: null,
    myDeliveries: [],
    driverLocation: null,
    trackingRunId: null,
    analytics: null,
    totalSavings: 0,
    loading: false,
    error: null
  }),

  getters: {
    // Runs by status
    scheduledRuns: (state) => state.activeRuns.filter(r => r.status === 'scheduled'),
    inTransitRuns: (state) => state.activeRuns.filter(r => r.status === 'out-for-delivery'),
    completedRuns: (state) => state.activeRuns.filter(r => r.status === 'completed'),
    
    // My pending deliveries
    pendingDeliveries: (state) => state.myDeliveries.filter(d => d.status === 'pending' || d.status === 'out-for-delivery'),
    
    // Runs with available capacity
    joinableRuns: (state) => state.availableRuns.filter(r => r.status === 'scheduled' && r.availableSlots > 0),
    
    // Get stops for a run
    runStops: (state) => (runId: string) => {
      const run = state.activeRuns.find(r => r.id === runId)
      return run?.dropList || []
    },
    
    // Check if I'm in a run
    isInRun: (state) => (runId: string, shopId: string) => {
      const run = state.activeRuns.find(r => r.id === runId)
      return run?.dropList.some(stop => stop.shopId === shopId) || false
    }
  },

  actions: {
    // Fetch runs with optional filters
    async fetchRuns(filters?: RunFilterDto) {
      this.loading = true
      this.error = null
      
      try {
        const response = await $fetch<{ runs: SharedRun[]; totalCount: number }>('/api/runs', {
          method: 'GET',
          query: filters
        })
        
        this.activeRuns = response.runs
        return response
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch runs'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Fetch runs where I have deliveries
    async fetchMyRuns(shopId: string) {
      this.loading = true
      
      try {
        const response = await $fetch<{ runs: SharedRun[] }>('/api/runs/my-runs', {
          method: 'GET',
          query: { shopId }
        })
        
        this.myRuns = response.runs
        
        // Extract my delivery stops
        this.myDeliveries = response.runs.flatMap(run =>
          run.dropList.filter(stop => stop.shopId === shopId)
        )
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch my runs'
      } finally {
        this.loading = false
      }
    },

    // Fetch available runs in my area
    async fetchAvailableRuns(zone: string, date?: Date) {
      this.loading = true
      
      try {
        const response = await $fetch<{ runs: SharedRun[] }>('/api/runs', {
          method: 'GET',
          query: {
            zone,
            scheduledDateFrom: date,
            hasAvailableCapacity: true,
            status: 'scheduled'
          }
        })
        
        this.availableRuns = response.runs
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch available runs'
      } finally {
        this.loading = false
      }
    },

    // Get specific run details
    async fetchRun(runId: string) {
      this.loading = true
      
      try {
        const run = await $fetch<SharedRun>(`/api/runs/${runId}`)
        this.currentRun = run
        
        // Update in active runs list
        const index = this.activeRuns.findIndex(r => r.id === runId)
        if (index !== -1) {
          this.activeRuns[index] = run
        } else {
          this.activeRuns.push(run)
        }
        
        return run
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch run'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Create new shared run
    async createRun(runData: CreateSharedRunDto) {
      this.loading = true
      this.error = null
      
      try {
        const run = await $fetch<SharedRun>('/api/runs', {
          method: 'POST',
          body: runData
        })
        
        this.activeRuns.unshift(run)
        this.currentRun = run
        
        return run
      } catch (err: any) {
        this.error = err.message || 'Failed to create run'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Add stop to existing run
    async addStopToRun(runId: string, stop: CreateDeliveryStopDto) {
      this.loading = true
      
      try {
        const run = await $fetch<SharedRun>(`/api/runs/${runId}/stops`, {
          method: 'POST',
          body: stop
        })
        
        this.updateRunInLists(run)
        return run
      } catch (err: any) {
        this.error = err.message || 'Failed to add stop'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Remove stop from run
    async removeStopFromRun(runId: string, stopId: string, reason?: string) {
      this.loading = true
      
      try {
        const run = await $fetch<SharedRun>(`/api/runs/${runId}/stops/${stopId}`, {
          method: 'DELETE',
          body: { reason }
        })
        
        this.updateRunInLists(run)
        return run
      } catch (err: any) {
        this.error = err.message || 'Failed to remove stop'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Start delivery run (driver action)
    async startRun(runId: string) {
      this.loading = true
      
      try {
        const run = await $fetch<SharedRun>(`/api/runs/${runId}/start`, {
          method: 'PATCH'
        })
        
        this.updateRunInLists(run)
        return run
      } catch (err: any) {
        this.error = err.message || 'Failed to start run'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Complete delivery run
    async completeRun(runId: string) {
      this.loading = true
      
      try {
        const run = await $fetch<SharedRun>(`/api/runs/${runId}/complete`, {
          method: 'PATCH'
        })
        
        this.updateRunInLists(run)
        return run
      } catch (err: any) {
        this.error = err.message || 'Failed to complete run'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Capture proof of delivery
    async capturePOD(podData: CapturePODDto) {
      this.loading = true
      
      try {
        const stop = await $fetch<DeliveryStop>(`/api/runs/stops/${podData.stopId}/pod`, {
          method: 'POST',
          body: podData
        })
        
        // Update stop in my deliveries
        const index = this.myDeliveries.findIndex(d => d.id === podData.stopId)
        if (index !== -1) {
          this.myDeliveries[index] = stop
        }
        
        // Update run
        if (this.currentRun) {
          const stopIndex = this.currentRun.dropList.findIndex(d => d.id === podData.stopId)
          if (stopIndex !== -1) {
            this.currentRun.dropList[stopIndex] = stop
          }
        }
        
        return stop
      } catch (err: any) {
        this.error = err.message || 'Failed to capture POD'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Optimize route for a run
    async optimizeRoute(request: RouteOptimizationRequest): Promise<RouteOptimizationResponse> {
      this.loading = true
      
      try {
        const response = await $fetch<RouteOptimizationResponse>('/api/runs/optimize-route', {
          method: 'POST',
          body: request
        })
        
        return response
      } catch (err: any) {
        this.error = err.message || 'Failed to optimize route'
        throw err
      } finally {
        this.loading = false
      }
    },

    // Calculate delivery cost breakdown
    async calculateCostBreakdown(runId: string): Promise<DeliveryCostBreakdown> {
      try {
        const breakdown = await $fetch<DeliveryCostBreakdown>(`/api/runs/${runId}/cost-breakdown`)
        return breakdown
      } catch (err: any) {
        this.error = err.message || 'Failed to calculate cost breakdown'
        throw err
      }
    },

    // Calculate delivery savings
    async calculateSavings(orderId: string, shopId: string): Promise<DeliverySavingsCalculation> {
      try {
        const savings = await $fetch<DeliverySavingsCalculation>('/api/runs/savings', {
          method: 'GET',
          query: { orderId, shopId }
        })
        
        return savings
      } catch (err: any) {
        this.error = err.message || 'Failed to calculate savings'
        throw err
      }
    },

    // Track driver location
    async startTracking(runId: string) {
      this.trackingRunId = runId
      
      // Use WebSocket or SSE for real-time updates
      const eventSource = new EventSource(`/api/runs/${runId}/track`)
      
      eventSource.onmessage = (event) => {
        const data = JSON.parse(event.data)
        this.driverLocation = data.location
        
        // Update run status if changed
        if (data.run) {
          this.updateRunInLists(data.run)
        }
      }
      
      eventSource.onerror = () => {
        eventSource.close()
        this.stopTracking()
      }
      
      return eventSource
    },

    stopTracking() {
      this.driverLocation = null
      this.trackingRunId = null
    },

    // Fetch analytics
    async fetchAnalytics() {
      try {
        const analytics = await $fetch<LogisticsAnalytics>('/api/runs/analytics')
        this.analytics = analytics
        return analytics
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch analytics'
      }
    },

    // Fetch driver stats
    async fetchDriverStats(driverId: string) {
      try {
        const stats = await $fetch<DriverStats>(`/api/runs/driver-stats/${driverId}`)
        return stats
      } catch (err: any) {
        this.error = err.message || 'Failed to fetch driver stats'
        throw err
      }
    },

    // Utility: Update run in all relevant lists
    updateRunInLists(run: SharedRun) {
      const updateList = (list: SharedRun[]) => {
        const index = list.findIndex(r => r.id === run.id)
        if (index !== -1) {
          list[index] = run
        }
      }
      
      updateList(this.activeRuns)
      updateList(this.myRuns)
      updateList(this.availableRuns)
      
      if (this.currentRun?.id === run.id) {
        this.currentRun = run
      }
    },

    // Clear all state
    clearState() {
      this.activeRuns = []
      this.myRuns = []
      this.availableRuns = []
      this.currentRun = null
      this.myDeliveries = []
      this.driverLocation = null
      this.trackingRunId = null
      this.analytics = null
      this.totalSavings = 0
      this.error = null
    }
  }
})

