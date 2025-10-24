/**
 * Shared Logistics/Delivery API Composable
 * Connects to TOSS backend /api/logistics endpoints
 */
export const useSharedDelivery = () => {
  const { get, post } = useApi()

  /**
   * Create a new shared delivery run
   */
  const createDeliveryRun = async (runData: {
    driverId?: number
    vehicleId?: string
    plannedDate: string
    stops: Array<{
      shopId: number
      address: string
      latitude?: number
      longitude?: number
      estimatedArrival?: string
    }>
  }) => {
    return await post<{ id: number }>('/api/logistics/delivery-runs', runData)
  }

  /**
   * Get delivery runs with filters
   */
  const getDeliveryRuns = async (params?: {
    driverId?: number
    status?: string
    startDate?: string
    endDate?: string
    pageNumber?: number
    pageSize?: number
  }) => {
    return await get<any>('/api/logistics/delivery-runs', params)
  }

  /**
   * Get driver's view of a specific run
   */
  const getDriverRunView = async (runId: number, driverId: number) => {
    return await get<any>(`/api/logistics/delivery-runs/${runId}/driver-view`, { driverId })
  }

  /**
   * Update delivery run status
   */
  const updateDeliveryStatus = async (runId: number, statusData: {
    status: string
    notes?: string
  }) => {
    return await post<any>(`/api/logistics/delivery-runs/${runId}/status`, statusData)
  }

  /**
   * Assign driver to a delivery run
   */
  const assignDriver = async (runId: number, driverData: {
    driverId: number
  }) => {
    return await post<any>(`/api/logistics/delivery-runs/${runId}/assign-driver`, driverData)
  }

  /**
   * Capture proof of delivery for a stop
   */
  const captureProofOfDelivery = async (stopId: number, podData: {
    deliveredAt: string
    receivedBy?: string
    signature?: string
    photo?: string
    notes?: string
  }) => {
    return await post<{ id: number }>(`/api/logistics/delivery-stops/${stopId}/proof`, podData)
  }

  /**
   * Get delivery tracking information
   */
  const getDeliveryTracking = async (runId: number) => {
    return await get<any>(`/api/logistics/delivery-runs/${runId}/tracking`)
  }

  return {
    createDeliveryRun,
    getDeliveryRuns,
    getDriverRunView,
    updateDeliveryStatus,
    assignDriver,
    captureProofOfDelivery,
    getDeliveryTracking
  }
}
