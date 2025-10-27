import type { Store, CreateStoreRequest, UpdateStoreRequest } from '~/types/stores'

export const useStoresAPI = () => {
  const config = useRuntimeConfig()
  const baseURL = (config.public.apiBase || 'https://localhost:5001') + '/api'

  const getStores = async (params?: {
    searchTerm?: string
    activeOnly?: boolean
    pageNumber?: number
    pageSize?: number
  }) => {
    try {
      const queryParams = new URLSearchParams()
      if (params?.searchTerm) queryParams.append('searchTerm', params.searchTerm)
      if (params?.activeOnly !== undefined) queryParams.append('activeOnly', params.activeOnly.toString())
      if (params?.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const url = `${baseURL}/Stores?${queryParams.toString()}`
      const response = await $fetch(url)
      return response as Store[]
    } catch (error) {
      console.error('Failed to fetch stores:', error)
      throw error
    }
  }

  const getStoreById = async (id: number) => {
    try {
      const response = await $fetch(`${baseURL}/Stores/${id}`)
      return response as Store
    } catch (error) {
      console.error(`Failed to fetch store ${id}:`, error)
      throw error
    }
  }

  const createStore = async (data: CreateStoreRequest) => {
    try {
      const response = await $fetch(`${baseURL}/Stores`, {
        method: 'POST',
        body: data
      })
      return response as { id: number }
    } catch (error) {
      console.error('Failed to create store:', error)
      throw error
    }
  }

  const updateStore = async (id: number, data: UpdateStoreRequest) => {
    try {
      await $fetch(`${baseURL}/Stores/${id}`, {
        method: 'PUT',
        body: { ...data, id }
      })
      return true
    } catch (error) {
      console.error(`Failed to update store ${id}:`, error)
      throw error
    }
  }

  const deleteStore = async (id: number) => {
    try {
      await $fetch(`${baseURL}/Stores/${id}`, {
        method: 'DELETE'
      })
      return true
    } catch (error) {
      console.error(`Failed to delete store ${id}:`, error)
      throw error
    }
  }

  return {
    getStores,
    getStoreById,
    createStore,
    updateStore,
    deleteStore
  }
}

