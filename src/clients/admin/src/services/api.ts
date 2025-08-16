import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'

// API Configuration
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:8080/api'
const API_TIMEOUT = 30000

// Base API configuration
export const api = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl: API_BASE_URL,
    timeout: API_TIMEOUT,
    prepareHeaders: (headers) => {
      // Add common headers
      headers.set('Content-Type', 'application/json')
      headers.set('User-Agent', 'TossErp-Admin/1.0.0')
      headers.set('X-Client-Type', 'admin')
      headers.set('X-Client-Version', '1.0.0')
      
      // Add auth token if available
      const token = localStorage.getItem('toss_admin_auth')
      if (token) {
        headers.set('Authorization', `Bearer ${token}`)
      }
      
      return headers
    },
  }),
  endpoints: () => ({}),
  tagTypes: ['StockItem', 'StockMovement', 'StockLevel', 'Warehouse', 'User', 'Report'],
})

// Stock Items API
export const stockItemsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getItems: builder.query({
      query: (params) => ({
        url: '/stock/items',
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          search: params?.search,
          category: params?.category,
          warehouse: params?.warehouse,
          stockLevel: params?.stockLevel,
        },
      }),
      providesTags: ['StockItem'],
    }),
    
    getItemById: builder.query({
      query: (id) => `/stock/items/${id}`,
      providesTags: (result, error, id) => [{ type: 'StockItem', id }],
    }),
    
    createItem: builder.mutation({
      query: (item) => ({
        url: '/stock/items',
        method: 'POST',
        body: item,
      }),
      invalidatesTags: ['StockItem'],
    }),
    
    updateItem: builder.mutation({
      query: ({ id, ...item }) => ({
        url: `/stock/items/${id}`,
        method: 'PUT',
        body: item,
      }),
      invalidatesTags: (result, error, { id }) => [{ type: 'StockItem', id }],
    }),
    
    deleteItem: builder.mutation({
      query: (id) => ({
        url: `/stock/items/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['StockItem'],
    }),
  }),
})

// Stock Movements API
export const stockMovementsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getMovements: builder.query({
      query: (params) => ({
        url: '/stock/movements',
        params: {
          page: params?.page || 1,
          pageSize: params?.pageSize || 10,
          itemId: params?.itemId,
          warehouse: params?.warehouse,
          type: params?.type,
          startDate: params?.startDate,
          endDate: params?.endDate,
        },
      }),
      providesTags: ['StockMovement'],
    }),
    
    createMovement: builder.mutation({
      query: (movement) => ({
        url: '/stock/movements',
        method: 'POST',
        body: movement,
      }),
      invalidatesTags: ['StockMovement', 'StockItem'],
    }),
  }),
})

// Stock Levels API
export const stockLevelsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getStockLevels: builder.query({
      query: (params) => ({
        url: '/stock/levels',
        params: {
          warehouse: params?.warehouse,
          lowStock: params?.lowStock,
        },
      }),
      providesTags: ['StockLevel'],
    }),
  }),
})

// Reports API
export const reportsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    getStockValuationReport: builder.query({
      query: (params) => ({
        url: '/stock/reports/valuation',
        params: {
          warehouse: params?.warehouse,
          category: params?.category,
          asOf: params?.asOf,
        },
      }),
      providesTags: ['Report'],
    }),
    
    getMovementSummary: builder.query({
      query: (params) => ({
        url: '/stock/reports/movements',
        params: {
          startDate: params.startDate,
          endDate: params.endDate,
          warehouse: params?.warehouse,
          type: params?.type,
        },
      }),
      providesTags: ['Report'],
    }),
  }),
})

// Bulk Operations API
export const bulkOperationsApi = api.injectEndpoints({
  endpoints: (builder) => ({
    bulkImport: builder.mutation({
      query: (file) => {
        const formData = new FormData()
        formData.append('file', file)
        return {
          url: '/stock/items/bulk-import',
          method: 'POST',
          body: formData,
        }
      },
      invalidatesTags: ['StockItem'],
    }),
    
    bulkExport: builder.query({
      query: (params) => ({
        url: '/stock/items/bulk-export',
        params: {
          format: params?.format || 'csv',
          category: params?.category,
          warehouse: params?.warehouse,
        },
        responseHandler: (response) => response.blob(),
      }),
    }),
  }),
})

// Health Check API
export const healthApi = api.injectEndpoints({
  endpoints: (builder) => ({
    checkHealth: builder.query({
      query: () => ({
        url: '/health',
        // Remove /api prefix for health check
        baseUrl: API_BASE_URL.replace('/api', ''),
      }),
    }),
  }),
})

// Export hooks
export const {
  useGetItemsQuery,
  useGetItemByIdQuery,
  useCreateItemMutation,
  useUpdateItemMutation,
  useDeleteItemMutation,
} = stockItemsApi

export const {
  useGetMovementsQuery,
  useCreateMovementMutation,
} = stockMovementsApi

export const {
  useGetStockLevelsQuery,
} = stockLevelsApi

export const {
  useGetStockValuationReportQuery,
  useGetMovementSummaryQuery,
} = reportsApi

export const {
  useBulkImportMutation,
  useBulkExportQuery,
} = bulkOperationsApi

export const {
  useCheckHealthQuery,
} = healthApi

// Utility functions
export const apiUtils = {
  // Get current API base URL
  getBaseUrl: () => API_BASE_URL,
  
  // Check if API is healthy
  async healthCheck(): Promise<boolean> {
    try {
      const response = await fetch(`${API_BASE_URL.replace('/api', '')}/health`)
      return response.ok
    } catch {
      return false
    }
  },
  
  // Set authentication token
  setAuthToken: (token: string) => {
    localStorage.setItem('toss_admin_auth', token)
  },
  
  // Clear authentication token
  clearAuthToken: () => {
    localStorage.removeItem('toss_admin_auth')
  },
  
  // Get authentication token
  getAuthToken: () => {
    return localStorage.getItem('toss_admin_auth')
  },
}
