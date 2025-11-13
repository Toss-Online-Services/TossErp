/**
 * HR & Payroll Management Composable
 * Designed for South African SMMEs (spaza shops, chisa nyamas, small retailers)
 * Handles employee management, payroll, attendance, and SA-specific compliance
 */

import { ref, computed, reactive } from 'vue'
import type { 
  Employee, 
  EmployeeCreate, 
  EmployeeUpdate,
  PayrollEntry,
  AttendanceRecord,
  LeaveRequest,
  SalaryStructure,
  TaxCalculation,
  PayslipData
} from '~/types/hr'

export const useHR = () => {
  // State management
  const employees = ref<Employee[]>([])
  const currentEmployee = ref<Employee | null>(null)
  const payrollEntries = ref<PayrollEntry[]>([])
  const attendanceRecords = ref<AttendanceRecord[]>([])
  const leaveRequests = ref<LeaveRequest[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // API configuration
  const { $fetch } = useNuxtApp()
  const { apiBaseUrl } = useRuntimeConfig().public

  /**
   * South African Tax Year and Compliance Constants
   */
  const SA_TAX_CONSTANTS = {
    taxYear: '2024/2025',
    uifRate: 0.01, // 1% UIF (Unemployment Insurance Fund)
    uifMax: 177.12, // Max monthly UIF contribution (2024)
    sdlRate: 0.01, // 1% SDL (Skills Development Levy)
    sdlThreshold: 500000, // Annual threshold
    payeThresholds: [
      { min: 0, max: 95750, rate: 0.18, rebate: 17235 },
      { min: 95751, max: 148217, rate: 0.26, rebate: 24855 },
      { min: 148218, max: 163770, rate: 0.31, rebate: 32305 },
      { min: 163771, max: 361005, rate: 0.36, rebate: 40387 },
      { min: 361006, max: 761002, rate: 0.39, rebate: 51821 },
      { min: 761003, max: Infinity, rate: 0.45, rebate: 97821 }
    ]
  }

  /**
   * Employee Management Functions
   */

  // Fetch all employees
  const fetchEmployees = async () => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<Employee[]>(`${apiBaseUrl}/api/hr/employees`)
      employees.value = response
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch employees'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Get employee by ID
  const getEmployee = async (id: string): Promise<Employee> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<Employee>(`${apiBaseUrl}/api/hr/employees/${id}`)
      currentEmployee.value = response
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch employee'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Create new employee
  const createEmployee = async (employeeData: EmployeeCreate): Promise<Employee> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<Employee>(`${apiBaseUrl}/api/hr/employees`, {
        method: 'POST',
        body: employeeData
      })
      employees.value.push(response)
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to create employee'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Update employee
  const updateEmployee = async (id: string, updateData: EmployeeUpdate): Promise<Employee> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<Employee>(`${apiBaseUrl}/api/hr/employees/${id}`, {
        method: 'PATCH',
        body: updateData
      })
      
      const index = employees.value.findIndex(emp => emp.id === id)
      if (index !== -1) {
        employees.value[index] = response
      }
      
      if (currentEmployee.value?.id === id) {
        currentEmployee.value = response
      }
      
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to update employee'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Delete employee
  const deleteEmployee = async (id: string): Promise<void> => {
    isLoading.value = true
    error.value = null

    try {
      await $fetch(`${apiBaseUrl}/api/hr/employees/${id}`, {
        method: 'DELETE'
      })
      
      employees.value = employees.value.filter(emp => emp.id !== id)
      if (currentEmployee.value?.id === id) {
        currentEmployee.value = null
      }
    } catch (err: any) {
      error.value = err.message || 'Failed to delete employee'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Attendance Management Functions
   */

  // Clock in/out
  const clockInOut = async (employeeId: string, type: 'in' | 'out', location?: { lat: number; lng: number }): Promise<AttendanceRecord> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<AttendanceRecord>(`${apiBaseUrl}/api/hr/attendance/clock`, {
        method: 'POST',
        body: {
          employeeId,
          type,
          timestamp: new Date().toISOString(),
          location
        }
      })
      
      attendanceRecords.value.push(response)
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to record attendance'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Get attendance records
  const fetchAttendance = async (employeeId?: string, dateFrom?: string, dateTo?: string) => {
    isLoading.value = true
    error.value = null

    try {
      const params = new URLSearchParams()
      if (employeeId) params.append('employeeId', employeeId)
      if (dateFrom) params.append('dateFrom', dateFrom)
      if (dateTo) params.append('dateTo', dateTo)

      const response = await $fetch<AttendanceRecord[]>(`${apiBaseUrl}/api/hr/attendance?${params}`)
      attendanceRecords.value = response
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to fetch attendance records'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Leave Management Functions
   */

  // Submit leave request
  const submitLeaveRequest = async (leaveData: Omit<LeaveRequest, 'id' | 'status' | 'createdAt'>): Promise<LeaveRequest> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<LeaveRequest>(`${apiBaseUrl}/api/hr/leave-requests`, {
        method: 'POST',
        body: leaveData
      })
      
      leaveRequests.value.push(response)
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to submit leave request'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Approve/reject leave request
  const processLeaveRequest = async (id: string, status: 'approved' | 'rejected', reason?: string): Promise<LeaveRequest> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<LeaveRequest>(`${apiBaseUrl}/api/hr/leave-requests/${id}/process`, {
        method: 'PATCH',
        body: { status, reason }
      })
      
      const index = leaveRequests.value.findIndex(req => req.id === id)
      if (index !== -1) {
        leaveRequests.value[index] = response
      }
      
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to process leave request'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Payroll Management Functions
   */

  // Calculate South African taxes and deductions
  const calculateSATax = (grossSalary: number, payPeriod: 'monthly' | 'weekly' = 'monthly'): TaxCalculation => {
    const annualSalary = payPeriod === 'monthly' ? grossSalary * 12 : grossSalary * 52
    
    // Calculate PAYE
    let paye = 0
    for (const bracket of SA_TAX_CONSTANTS.payeThresholds) {
      if (annualSalary > bracket.min) {
        const taxableInBracket = Math.min(annualSalary, bracket.max) - bracket.min
        paye += taxableInBracket * bracket.rate
      }
    }
    
    // Apply rebates (primary rebate for most taxpayers)
    paye = Math.max(0, paye - 17235)
    
    // Convert to period amount
    const periodPAYE = payPeriod === 'monthly' ? paye / 12 : paye / 52
    
    // Calculate UIF
    const uifContribution = Math.min(grossSalary * SA_TAX_CONSTANTS.uifRate, SA_TAX_CONSTANTS.uifMax)
    
    // Calculate SDL (employer responsibility, but tracked for compliance)
    const sdlContribution = annualSalary > SA_TAX_CONSTANTS.sdlThreshold ? grossSalary * SA_TAX_CONSTANTS.sdlRate : 0
    
    const totalDeductions = periodPAYE + uifContribution
    const netSalary = grossSalary - totalDeductions
    
    return {
      grossSalary,
      paye: periodPAYE,
      uif: uifContribution,
      sdl: sdlContribution,
      totalDeductions,
      netSalary,
      payPeriod
    }
  }

  // Generate payslip
  const generatePayslip = async (employeeId: string, payPeriod: string): Promise<PayslipData> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<PayslipData>(`${apiBaseUrl}/api/hr/payroll/payslip`, {
        method: 'POST',
        body: { employeeId, payPeriod }
      })
      
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to generate payslip'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Process bulk payroll
  const processBulkPayroll = async (payPeriod: string, employeeIds?: string[]): Promise<PayrollEntry[]> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<PayrollEntry[]>(`${apiBaseUrl}/api/hr/payroll/process`, {
        method: 'POST',
        body: { payPeriod, employeeIds }
      })
      
      payrollEntries.value.push(...response)
      return response
    } catch (err: any) {
      error.value = err.message || 'Failed to process payroll'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Reporting Functions
   */

  // Generate UIF report for Department of Labour
  const generateUIFReport = async (dateFrom: string, dateTo: string) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch(`${apiBaseUrl}/api/hr/reports/uif`, {
        method: 'POST',
        body: { dateFrom, dateTo },
        responseType: 'blob'
      })
      
      // Download the file
      const blob = new Blob([response], { type: 'application/pdf' })
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `UIF_Report_${dateFrom}_to_${dateTo}.pdf`
      a.click()
      window.URL.revokeObjectURL(url)
    } catch (err: any) {
      error.value = err.message || 'Failed to generate UIF report'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Generate PAYE report for SARS
  const generatePAYEReport = async (taxYear: string) => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch(`${apiBaseUrl}/api/hr/reports/paye`, {
        method: 'POST',
        body: { taxYear },
        responseType: 'blob'
      })
      
      // Download the file
      const blob = new Blob([response], { type: 'application/excel' })
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `PAYE_Report_${taxYear}.xlsx`
      a.click()
      window.URL.revokeObjectURL(url)
    } catch (err: any) {
      error.value = err.message || 'Failed to generate PAYE report'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Computed Properties
   */

  // Active employees
  const activeEmployees = computed(() => 
    employees.value.filter(emp => emp.status === 'active')
  )

  // Pending leave requests
  const pendingLeaveRequests = computed(() => 
    leaveRequests.value.filter(req => req.status === 'pending')
  )

  // Today's attendance
  const todaysAttendance = computed(() => {
    const today = new Date().toISOString().split('T')[0]
    return attendanceRecords.value.filter(record => 
      record.date === today
    )
  })

  // Monthly payroll total
  const currentMonthPayrollTotal = computed(() => {
    const currentMonth = new Date().toISOString().substring(0, 7) // YYYY-MM
    return payrollEntries.value
      .filter(entry => entry.payPeriod.startsWith(currentMonth))
      .reduce((total, entry) => total + entry.netSalary, 0)
  })

  /**
   * Utility Functions
   */

  // Validate South African ID number
  const validateSAIDNumber = (idNumber: string): boolean => {
    if (!/^\d{13}$/.test(idNumber)) return false
    
    // Luhn algorithm for SA ID validation
    const digits = idNumber.split('').map(Number)
    let sum = 0
    
    for (let i = 0; i < 12; i++) {
      if (i % 2 === 0) {
        sum += digits[i]
      } else {
        const doubled = digits[i] * 2
        sum += doubled > 9 ? doubled - 9 : doubled
      }
    }
    
    const checkDigit = (10 - (sum % 10)) % 10
    return checkDigit === digits[12]
  }

  // Format currency for South African Rand
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    }).format(amount)
  }

  // Format date for SA locale
  const formatDate = (date: string | Date): string => {
    const dateObj = typeof date === 'string' ? new Date(date) : date
    return dateObj.toLocaleDateString('en-ZA')
  }

  return {
    // State
    employees: readonly(employees),
    currentEmployee: readonly(currentEmployee),
    payrollEntries: readonly(payrollEntries),
    attendanceRecords: readonly(attendanceRecords),
    leaveRequests: readonly(leaveRequests),
    isLoading: readonly(isLoading),
    error: readonly(error),

    // Employee Management
    fetchEmployees,
    getEmployee,
    createEmployee,
    updateEmployee,
    deleteEmployee,

    // Attendance Management
    clockInOut,
    fetchAttendance,

    // Leave Management
    submitLeaveRequest,
    processLeaveRequest,

    // Payroll Management
    calculateSATax,
    generatePayslip,
    processBulkPayroll,

    // Reporting
    generateUIFReport,
    generatePAYEReport,

    // Computed Properties
    activeEmployees,
    pendingLeaveRequests,
    todaysAttendance,
    currentMonthPayrollTotal,

    // Utilities
    validateSAIDNumber,
    formatCurrency,
    formatDate,

    // Constants
    SA_TAX_CONSTANTS
  }
}