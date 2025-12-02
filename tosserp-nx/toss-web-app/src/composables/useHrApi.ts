import { ref } from 'vue'
import type { PaginatedResponse } from '~/types/api'

export type EmployeeRateType = 'Hourly' | 'Daily' | 'Monthly'

export interface EmployeeDto {
  id: number
  name: string
  email?: string
  phone?: string
  idNumber?: string
  rateType: EmployeeRateType
  rate: number
  isActive: boolean
  notes?: string
  created: string
}

export interface AttendanceDto {
  id: number
  employeeId: number
  attendanceDate: string
  clockIn?: string
  clockOut?: string
  hoursWorked?: number
  daysWorked?: number
  notes?: string
  created: string
}

export interface PayrollRunDto {
  id: number
  employeeId: number
  employeeName: string
  periodStart: string
  periodEnd: string
  gross: number
  deductions: number
  net: number
  generatedAt: string
  notes?: string
}

export const useHrApi = () => {
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

  const getEmployees = async (params: {
    searchTerm?: string
    isActive?: boolean
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<EmployeeDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm)
      if (params.isActive !== undefined) queryParams.append('isActive', params.isActive.toString())
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<EmployeeDto>>(
        `${getApiBaseUrl()}/api/hr/employees?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch employees'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getEmployeeById = async (id: number): Promise<EmployeeDto> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<EmployeeDto>(
        `${getApiBaseUrl()}/api/hr/employees/${id}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch employee'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getAttendanceForPeriod = async (params: {
    employeeId: number
    fromDate: string
    toDate: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<AttendanceDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      queryParams.append('employeeId', params.employeeId.toString())
      queryParams.append('fromDate', params.fromDate)
      queryParams.append('toDate', params.toDate)
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<AttendanceDto>>(
        `${getApiBaseUrl()}/api/hr/attendance?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch attendance'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  const getPayrollRuns = async (params: {
    employeeId?: number
    fromDate?: string
    toDate?: string
    pageNumber?: number
    pageSize?: number
  }): Promise<PaginatedResponse<PayrollRunDto>> => {
    isLoading.value = true
    error.value = null

    try {
      const queryParams = new URLSearchParams()
      if (params.employeeId) queryParams.append('employeeId', params.employeeId.toString())
      if (params.fromDate) queryParams.append('fromDate', params.fromDate)
      if (params.toDate) queryParams.append('toDate', params.toDate)
      if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString())
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString())

      const response = await $fetch<PaginatedResponse<PayrollRunDto>>(
        `${getApiBaseUrl()}/api/hr/payroll/runs?${queryParams.toString()}`,
        {
          method: 'GET',
          headers: getAuthHeaders()
        }
      )

      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch payroll runs'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getEmployees,
    getEmployeeById,
    getAttendanceForPeriod,
    getPayrollRuns
  }
}

