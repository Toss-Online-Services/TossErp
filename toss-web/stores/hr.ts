import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export type EmployeeRateType = 'Hourly' | 'Daily' | 'Monthly'

export interface Employee {
  id: number
  name: string
  email?: string
  phone?: string
  idNumber?: string
  rateType: EmployeeRateType
  rate: number
  isActive: boolean
  notes?: string
  createdAt: Date
  updatedAt: Date
}

export interface Attendance {
  id: number
  employeeId: number
  employeeName?: string
  attendanceDate: Date
  clockIn?: Date
  clockOut?: Date
  daysWorked?: number
  hoursWorked?: number
  notes?: string
  createdAt: Date
}

export interface PayrollRun {
  id: number
  employeeId: number
  employeeName?: string
  periodStart: Date
  periodEnd: Date
  gross: number
  deductions: number
  net: number
  generatedAt: Date
  notes?: string
}

export const useHrStore = defineStore('hr', () => {
  // State
  const employees = ref<Employee[]>([])
  const attendanceRecords = ref<Attendance[]>([])
  const payrollRuns = ref<PayrollRun[]>([])
  const loading = ref(false)

  // Computed
  const activeEmployees = computed(() => {
    return employees.value.filter(e => e.isActive)
  })

  const employeesByRateType = computed(() => {
    const grouped: Record<EmployeeRateType, Employee[]> = {
      Hourly: [],
      Daily: [],
      Monthly: []
    }
    employees.value.forEach(emp => {
      if (grouped[emp.rateType]) {
        grouped[emp.rateType].push(emp)
      }
    })
    return grouped
  })

  const totalPayrollCost = computed(() => {
    return payrollRuns.value.reduce((sum, run) => sum + run.net, 0)
  })

  const attendanceByEmployee = computed(() => {
    const grouped: Record<number, Attendance[]> = {}
    attendanceRecords.value.forEach(att => {
      if (!grouped[att.employeeId]) {
        grouped[att.employeeId] = []
      }
      grouped[att.employeeId].push(att)
    })
    return grouped
  })

  const totalHoursWorked = computed(() => {
    return attendanceRecords.value.reduce((sum, att) => {
      return sum + (att.hoursWorked || 0)
    }, 0)
  })

  // Actions
  async function fetchEmployees() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      employees.value = [
        {
          id: 1,
          name: 'Thabo Mthembu',
          email: 'thabo@example.com',
          phone: '+27 82 123 4567',
          idNumber: '850101 5800 08 5',
          rateType: 'Hourly',
          rate: 150,
          isActive: true,
          notes: 'Experienced carpenter',
          createdAt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000)
        },
        {
          id: 2,
          name: 'Sipho Zulu',
          email: 'sipho@example.com',
          phone: '+27 83 234 5678',
          rateType: 'Daily',
          rate: 800,
          isActive: true,
          notes: 'Tiler specialist',
          createdAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
        },
        {
          id: 3,
          name: 'Nomsa Dlamini',
          email: 'nomsa@example.com',
          phone: '+27 84 345 6789',
          rateType: 'Monthly',
          rate: 12000,
          isActive: true,
          notes: 'Office administrator',
          createdAt: new Date(Date.now() - 120 * 24 * 60 * 60 * 1000),
          updatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        }
      ]
    } catch (error) {
      console.error('Failed to fetch employees:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchAttendance(employeeId?: number, startDate?: Date, endDate?: Date) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      const mockAttendance: Attendance[] = [
        {
          id: 1,
          employeeId: 1,
          employeeName: 'Thabo Mthembu',
          attendanceDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
          clockIn: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000 + 8 * 60 * 60 * 1000),
          clockOut: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000 + 17 * 60 * 60 * 1000),
          hoursWorked: 8,
          createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
        },
        {
          id: 2,
          employeeId: 2,
          employeeName: 'Sipho Zulu',
          attendanceDate: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
          daysWorked: 1,
          createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        },
        {
          id: 3,
          employeeId: 1,
          employeeName: 'Thabo Mthembu',
          attendanceDate: new Date(),
          clockIn: new Date(Date.now() + 8 * 60 * 60 * 1000),
          hoursWorked: 0,
          createdAt: new Date()
        }
      ]

      if (employeeId) {
        attendanceRecords.value = mockAttendance.filter(a => a.employeeId === employeeId)
      } else {
        attendanceRecords.value = mockAttendance
      }
    } catch (error) {
      console.error('Failed to fetch attendance:', error)
    } finally {
      loading.value = false
    }
  }

  async function fetchPayrollRuns(employeeId?: number) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      const mockRuns: PayrollRun[] = [
        {
          id: 1,
          employeeId: 1,
          employeeName: 'Thabo Mthembu',
          periodStart: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
          periodEnd: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
          gross: 24000,
          deductions: 0,
          net: 24000,
          generatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        },
        {
          id: 2,
          employeeId: 2,
          employeeName: 'Sipho Zulu',
          periodStart: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000),
          periodEnd: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
          gross: 16000,
          deductions: 0,
          net: 16000,
          generatedAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
        }
      ]

      if (employeeId) {
        payrollRuns.value = mockRuns.filter(r => r.employeeId === employeeId)
      } else {
        payrollRuns.value = mockRuns
      }
    } catch (error) {
      console.error('Failed to fetch payroll runs:', error)
    } finally {
      loading.value = false
    }
  }

  async function createEmployee(employee: Omit<Employee, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const newEmployee: Employee = {
        ...employee,
        id: Date.now(),
        createdAt: new Date(),
        updatedAt: new Date()
      }
      
      employees.value.push(newEmployee)
      return newEmployee
    } catch (error) {
      console.error('Failed to create employee:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateEmployee(id: number, updates: Partial<Employee>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = employees.value.findIndex(e => e.id === id)
      if (index !== -1) {
        employees.value[index] = {
          ...employees.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update employee:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function recordAttendance(attendance: Omit<Attendance, 'id' | 'createdAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const employee = employees.value.find(e => e.id === attendance.employeeId)
      const newAttendance: Attendance = {
        ...attendance,
        employeeName: employee?.name,
        id: Date.now(),
        createdAt: new Date()
      }
      
      attendanceRecords.value.push(newAttendance)
      return newAttendance
    } catch (error) {
      console.error('Failed to record attendance:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function runPayroll(employeeIds: number[], periodStart: Date, periodEnd: Date) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      
      // Mock payroll generation
      const newRuns: PayrollRun[] = employeeIds.map(empId => {
        const employee = employees.value.find(e => e.id === empId)
        if (!employee) throw new Error('Employee not found')
        
        // Calculate based on rate type and attendance
        let gross = 0
        const attendance = attendanceRecords.value.filter(a => a.employeeId === empId)
        
        if (employee.rateType === 'Hourly') {
          const hours = attendance.reduce((sum, a) => sum + (a.hoursWorked || 0), 0)
          gross = hours * employee.rate
        } else if (employee.rateType === 'Daily') {
          const days = attendance.reduce((sum, a) => sum + (a.daysWorked || 0), 0)
          gross = days * employee.rate
        } else if (employee.rateType === 'Monthly') {
          gross = employee.rate
        }
        
        return {
          id: Date.now() + empId,
          employeeId: empId,
          employeeName: employee.name,
          periodStart,
          periodEnd,
          gross,
          deductions: 0,
          net: gross,
          generatedAt: new Date()
        }
      })
      
      payrollRuns.value.push(...newRuns)
      return newRuns
    } catch (error) {
      console.error('Failed to run payroll:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  function getEmployeeById(id: number): Employee | undefined {
    return employees.value.find(e => e.id === id)
  }

  return {
    // State
    employees,
    attendanceRecords,
    payrollRuns,
    loading,
    // Computed
    activeEmployees,
    employeesByRateType,
    totalPayrollCost,
    attendanceByEmployee,
    totalHoursWorked,
    // Actions
    fetchEmployees,
    fetchAttendance,
    fetchPayrollRuns,
    createEmployee,
    updateEmployee,
    recordAttendance,
    runPayroll,
    getEmployeeById
  }
})



