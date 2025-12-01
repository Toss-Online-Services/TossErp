import { ref } from 'vue'

export interface CustomerDto {
  id: number
  firstName: string
  lastName: string
  email: string
  phoneNumber?: string
  totalPurchases: number
  lastPurchaseDate?: string
  storeId?: number
  storeName: string
  tags?: string
  creditLimit: number
}

export interface CustomerProfileDto {
  id: number
  firstName: string
  lastName: string
  email: string
  phoneNumber?: string
  totalPurchases: number
  lastPurchaseDate?: string
  creditLimit: number
  outstandingBalance: number
  interactionCount: number
}

export interface CustomerInteractionDto {
  id: number
  customerId: number
  interactionType: string
  notes: string
  createdDate: string
  createdBy?: string
}

export interface PaginatedResponse<T> {
  items: T[]
  pageNumber: number
  totalPages: number
  totalCount: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export const useCrmApi = () => {
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

  const getCustomers = async (params: {
    storeId?: number
    searchTerm?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<CustomerDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.storeId) queryParams.append('storeId', params.storeId.toString())
      if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm)
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<CustomerDto>>(
        `${getApiBaseUrl()}/api/crm/customers?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch customers'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const searchCustomers = async (searchTerm: string, storeId?: number): Promise<CustomerDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      queryParams.append('searchTerm', searchTerm)
      if (storeId) queryParams.append('storeId', storeId.toString())

      const response = await $fetch<CustomerDto[]>(
        `${getApiBaseUrl()}/api/crm/customers/search?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to search customers'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getCustomerProfile = async (id: number): Promise<CustomerProfileDto> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<CustomerProfileDto>(
        `${getApiBaseUrl()}/api/crm/customers/${id}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch customer profile'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getCustomerInteractions = async (params: {
    customerId: number
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<CustomerInteractionDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<CustomerInteractionDto>>(
        `${getApiBaseUrl()}/api/crm/customers/${params.customerId}/interactions?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch customer interactions'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getCustomers,
    searchCustomers,
    getCustomerProfile,
    getCustomerInteractions
  }
}

