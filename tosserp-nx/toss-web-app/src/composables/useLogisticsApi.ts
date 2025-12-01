import { ref } from 'vue'

export interface SharedRunDto {
  id: number
  runNumber: string
  scheduledDate: string
  status: string
  driverId?: number
  driverName?: string
  stopCount: number
  totalDistance: number
  totalCost: number
}

export interface DriverRunViewDto {
  runId: number
  runNumber: string
  scheduledDate: string
  status: string
  driverId?: number
  driverName?: string
  stops: StopDto[]
}

export interface StopDto {
  stopId: number
  sequence: number
  businessName: string
  address: string
  contactName?: string
  contactPhone?: string
  status: string
  proofOfDelivery?: string
}

export interface DeliveryTrackingDto {
  runId: number
  runNumber: string
  status: string
  stops: DeliveryStopTrackingDto[]
}

export interface DeliveryStopTrackingDto {
  stopId: number
  sequence: number
  businessName: string
  status: string
  completedAt?: string
}

export const useLogisticsApi = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const getApiBaseUrl = () => {
    if (typeof window !== 'undefined') {
      const config = useRuntimeConfig()
      return config.public.apiBase || 'http://localhost:5000'
    }
    return 'http://localhost:5000'
  }

  const getAuthHeaders = () => {
    const token = localStorage.getItem('auth_token')
    return {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    }
  }

  const getSharedRuns = async (params: {
    startDate?: string
    endDate?: string
    status?: string
  }): Promise<SharedRunDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.startDate) queryParams.append('startDate', params.startDate)
      if (params.endDate) queryParams.append('endDate', params.endDate)
      if (params.status) queryParams.append('status', params.status)

      const response = await $fetch<SharedRunDto[]>(
        `${getApiBaseUrl()}/api/logistics/delivery-runs?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch delivery runs'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getDriverRunView = async (runId: number): Promise<DriverRunViewDto> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<DriverRunViewDto>(
        `${getApiBaseUrl()}/api/logistics/delivery-runs/${runId}/driver-view`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch driver run view'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getDeliveryTracking = async (runId: number): Promise<DeliveryTrackingDto> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<DeliveryTrackingDto>(
        `${getApiBaseUrl()}/api/logistics/delivery-runs/${runId}/tracking`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch delivery tracking'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const updateDeliveryStatus = async (runId: number, status: string): Promise<boolean> => {
    isLoading.value = true
    error.value = null

    try {
      await $fetch(
        `${getApiBaseUrl()}/api/logistics/delivery-runs/${runId}/status`,
        {
          method: 'POST',
          headers: getAuthHeaders(),
          body: { status }
        }
      )

      return true
    } catch (err: any) {
      error.value = err.message || 'Failed to update delivery status'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getSharedRuns,
    getDriverRunView,
    getDeliveryTracking,
    updateDeliveryStatus
  }
}

