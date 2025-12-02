import { ref } from 'vue'
import type { PaginatedResponse } from '~/types/api'

export interface QualityChecklistDto {
  id: number
  name: string
  description?: string
  isActive: boolean
  itemCount: number
  created: string
}

export const useQualityApi = () => {
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

  const getChecklists = async (params: {
    searchTerm?: string
    isActive?: boolean
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<QualityChecklistDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm)
      if (params.isActive !== undefined) queryParams.append('isActive', params.isActive.toString())
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<QualityChecklistDto>>(
        `${getApiBaseUrl()}/api/quality/checklists?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch checklists'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getChecklists
  }
}

