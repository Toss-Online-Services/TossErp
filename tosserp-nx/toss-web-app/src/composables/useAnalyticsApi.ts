import { ref } from 'vue'

export interface AnalyticsSummaryDto {
  totalLogins: number
  totalPosSales: number
  totalSalesAmount: number
  stockAlertsResolved: number
  stockOuts: number
  moduleUsage: Record<string, number>
  eventTypeCounts: Record<string, number>
}

export interface WeeklyAnalyticsDto {
  weekStart: string
  weekEnd: string
  logins: number
  posSales: number
  salesAmount: number
  stockAlertsResolved: number
  stockOuts: number
}

export interface ActivityEntryDto {
  id: number
  entityType: string
  entityId: number
  action: string
  userId?: string
  userName?: string
  changes?: string
  notes?: string
  created: string
}

export const useAnalyticsApi = () => {
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

  const getAnalyticsSummary = async (params: {
    fromDate?: string
    toDate?: string
  }): Promise<AnalyticsSummaryDto> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.fromDate) queryParams.append('fromDate', params.fromDate)
      if (params.toDate) queryParams.append('toDate', params.toDate)

      const response = await $fetch<AnalyticsSummaryDto>(
        `${getApiBaseUrl()}/api/analytics/summary?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch analytics summary'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getWeeklyAnalytics = async (params: {
    weeks?: number
  }): Promise<WeeklyAnalyticsDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.weeks) queryParams.append('weeks', params.weeks.toString())

      const response = await $fetch<WeeklyAnalyticsDto[]>(
        `${getApiBaseUrl()}/api/analytics/weekly?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch weekly analytics'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getActivityToday = async (params: {
    date?: string
  }): Promise<ActivityEntryDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.date) queryParams.append('date', params.date)

      const response = await $fetch<ActivityEntryDto[]>(
        `${getApiBaseUrl()}/api/activity/today?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch today\'s activity'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getAnalyticsSummary,
    getWeeklyAnalytics,
    getActivityToday
  }
}

