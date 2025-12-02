import { ref } from 'vue'
import type { PaginatedResponse } from '~/types/api'

export type TicketType = 'Bug' | 'Feature' | 'Question' | 'Other'
export type TicketStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed'

export interface TicketDto {
  id: number
  type: TicketType
  status: TicketStatus
  title: string
  description?: string
  linkedEntityType?: string
  linkedEntityId?: number
  createdById: string
  assignedToId?: string
  closedAt?: string
  priority: number
  noteCount: number
  created: string
}

export const useSupportApi = () => {
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

  const getTickets = async (params: {
    status?: TicketStatus
    type?: TicketType
    searchTerm?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<TicketDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.status) queryParams.append('status', params.status)
      if (params.type) queryParams.append('type', params.type)
      if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm)
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<TicketDto>>(
        `${getApiBaseUrl()}/api/support/tickets?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch tickets'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getTicketById = async (id: number): Promise<TicketDto> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<TicketDto>(
        `${getApiBaseUrl()}/api/support/tickets/${id}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch ticket'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getTickets,
    getTicketById
  }
}

