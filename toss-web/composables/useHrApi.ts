import { useApi, useAuthApi } from './useApi'
import type { Employee, Attendance, PayrollRun, EmployeeRateType } from '~/stores/hr'

export function useHrApi() {
  const { getHeaders } = useAuthApi()
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase || 'http://localhost:5000/api'

  async function getEmployees() {
    const { data, error, execute } = useApi<Employee[]>('/hr/employees', {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getEmployeeById(id: number) {
    const { data, error, execute } = useApi<Employee>(`/hr/employees/${id}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createEmployee(employee: Omit<Employee, 'id' | 'createdAt' | 'updatedAt'>) {
    const { data, error, execute } = useApi<Employee>('/hr/employees', {
      method: 'POST',
      body: employee,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateEmployee(id: number, updates: Partial<Employee>) {
    const { data, error, execute } = useApi<Employee>(`/hr/employees/${id}`, {
      method: 'PUT',
      body: updates,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function deleteEmployee(id: number) {
    const { data, error, execute } = useApi(`/hr/employees/${id}`, {
      method: 'DELETE',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function recordClockAttendance(employeeId: number, clockIn: boolean) {
    const { data, error, execute } = useApi<Attendance>('/hr/attendance/clock', {
      method: 'POST',
      body: { employeeId, clockIn },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function recordDaysAttendance(employeeId: number, attendanceDate: Date, daysWorked: number, notes?: string) {
    const { data, error, execute } = useApi<Attendance>('/hr/attendance/days', {
      method: 'POST',
      body: { employeeId, attendanceDate, daysWorked, notes },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getAttendance(employeeId?: number, startDate?: Date, endDate?: Date) {
    const params = new URLSearchParams()
    if (employeeId) params.append('employeeId', employeeId.toString())
    if (startDate) params.append('startDate', startDate.toISOString())
    if (endDate) params.append('endDate', endDate.toISOString())
    
    const url = `/hr/attendance${params.toString() ? '?' + params.toString() : ''}`
    const { data, error, execute } = useApi<Attendance[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function runPayroll(employeeIds: number[], periodStart: Date, periodEnd: Date) {
    const { data, error, execute } = useApi<PayrollRun[]>('/hr/payroll/run', {
      method: 'POST',
      body: { employeeIds, periodStart, periodEnd },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getPayrollRuns(employeeId?: number) {
    const url = employeeId ? `/hr/payroll/runs?employeeId=${employeeId}` : '/hr/payroll/runs'
    const { data, error, execute } = useApi<PayrollRun[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  return {
    getEmployees,
    getEmployeeById,
    createEmployee,
    updateEmployee,
    deleteEmployee,
    recordClockAttendance,
    recordDaysAttendance,
    getAttendance,
    runPayroll,
    getPayrollRuns
  }
}



