import { defineStore } from 'pinia'

export const useSharedLogisticsStore = defineStore('sharedLogistics', () => {
  const {
    createDeliveryRun,
    getDeliveryRuns,
    getDriverRunView,
    updateDeliveryStatus,
    assignDriver,
    captureProofOfDelivery,
    getDeliveryTracking
  } = useSharedDelivery()

  // State
  const activeRuns = ref<any[]>([])
  const myRuns = ref<any[]>([])
  const availableRuns = ref<any[]>([])
  const currentRun = ref<any | null>(null)
  const myDeliveries = ref<any[]>([])
  const driverLocation = ref<{ lat: number; lng: number } | null>(null)
  const trackingRunId = ref<string | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const totalSavings = ref(0)

  // Computed
  const scheduledRuns = computed(() => activeRuns.value.filter((r: any) => r.status === 'scheduled'))
  const inTransitRuns = computed(() => activeRuns.value.filter((r: any) => r.status === 'out-for-delivery'))
  const completedRuns = computed(() => activeRuns.value.filter((r: any) => r.status === 'completed'))
  const pendingDeliveries = computed(() => 
    myDeliveries.value.filter((d: any) => d.status === 'pending' || d.status === 'out-for-delivery')
  )
  const joinableRuns = computed(() => 
    availableRuns.value.filter((r: any) => r.status === 'scheduled' && r.availableSlots > 0)
  )

  // Actions
  const fetchDeliveryRuns = async (params?: any) => {
    loading.value = true
    error.value = null
    try {
      const response = await getDeliveryRuns(params)
      activeRuns.value = response.items || response || []
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch runs'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchRunById = async (runId: number) => {
    loading.value = true
    error.value = null
    try {
      const run = await getDeliveryTracking(runId)
      currentRun.value = run
      
      // Update in active runs list
      const index = activeRuns.value.findIndex((r: any) => r.id === runId)
      if (index !== -1) {
        activeRuns.value[index] = run
      } else {
        activeRuns.value.push(run)
      }
      
      return run
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch run'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchDriverRunView = async (runId: number, driverId: number) => {
    loading.value = true
    error.value = null
    try {
      const driverView = await getDriverRunView(runId, driverId)
      return driverView
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch driver view'
      throw err
    } finally {
      loading.value = false
    }
  }

  const createNewRun = async (runData: any) => {
    loading.value = true
    error.value = null
    try {
      const result = await createDeliveryRun(runData)
      await fetchDeliveryRuns()
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to create run'
      throw err
    } finally {
      loading.value = false
    }
  }

  const updateRunStatus = async (runId: number, statusData: any) => {
    loading.value = true
    error.value = null
    try {
      const result = await updateDeliveryStatus(runId, statusData)
      await fetchRunById(runId)
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to update status'
      throw err
    } finally {
      loading.value = false
    }
  }

  const assignDriverToRun = async (runId: number, driverId: number) => {
    loading.value = true
    error.value = null
    try {
      const result = await assignDriver(runId, { driverId })
      await fetchRunById(runId)
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to assign driver'
      throw err
    } finally {
      loading.value = false
    }
  }

  const capturePOD = async (stopId: number, podData: any) => {
    loading.value = true
    error.value = null
    try {
      const result = await captureProofOfDelivery(stopId, podData)
      return result
    } catch (err: any) {
      error.value = err.message || 'Failed to capture POD'
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchMyRuns = async (shopId: number) => {
    loading.value = true
    error.value = null
    try {
      const response = await getDeliveryRuns({ shopId })
      myRuns.value = response.items || response || []
      
      // Extract my delivery stops
      myDeliveries.value = myRuns.value.flatMap((run: any) =>
        run.dropList?.filter((stop: any) => stop.shopId === shopId) || []
      )
      
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch my runs'
      throw err
    } finally {
      loading.value = false
    }
  }

  const startTracking = (runId: string) => {
    trackingRunId.value = runId
    // TODO: Implement WebSocket or SSE for real-time tracking
  }

  const stopTracking = () => {
    driverLocation.value = null
    trackingRunId.value = null
  }

  const clearError = () => {
    error.value = null
  }

  const clearState = () => {
    activeRuns.value = []
    myRuns.value = []
    availableRuns.value = []
    currentRun.value = null
    myDeliveries.value = []
    driverLocation.value = null
    trackingRunId.value = null
    error.value = null
    totalSavings.value = 0
  }

  return {
    // State
    activeRuns,
    myRuns,
    availableRuns,
    currentRun,
    myDeliveries,
    driverLocation,
    trackingRunId,
    loading,
    error,
    totalSavings,
    
    // Computed
    scheduledRuns,
    inTransitRuns,
    completedRuns,
    pendingDeliveries,
    joinableRuns,
    
    // Actions
    fetchDeliveryRuns,
    fetchRunById,
    fetchDriverRunView,
    createNewRun,
    updateRunStatus,
    assignDriverToRun,
    capturePOD,
    fetchMyRuns,
    startTracking,
    stopTracking,
    clearError,
    clearState
  }
})
