<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Payroll Management</h1>
              <p class="text-gray-600 dark:text-gray-400">Process salaries, manage deductions and track payments</p>
            </div>
            <div class="flex space-x-3">
              <button @click="processPayroll" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <CurrencyDollarIcon class="w-5 h-5 inline mr-2" />
                Process Payroll
              </button>
              <button @click="generatePayslips" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <DocumentIcon class="w-5 h-5 inline mr-2" />
                Generate Payslips
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <CurrencyDollarIcon class="w-8 h-8 text-green-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Payroll</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ payrollStats.totalPayroll.toLocaleString() }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <UsersIcon class="w-8 h-8 text-blue-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Employees Paid</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ payrollStats.employeesPaid }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <ExclamationTriangleIcon class="w-8 h-8 text-yellow-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Deductions</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ payrollStats.totalDeductions.toLocaleString() }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <CheckCircleIcon class="w-8 h-8 text-purple-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Net Pay</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ payrollStats.netPay.toLocaleString() }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Payroll Period Selector -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Payroll Period</label>
            <select v-model="selectedPeriod" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              <option value="2024-01">January 2024</option>
              <option value="2024-02">February 2024</option>
              <option value="2024-03">March 2024</option>
              <option value="2024-04">April 2024</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Department</label>
            <select v-model="selectedDepartment" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              <option value="">All Departments</option>
              <option value="management">Management</option>
              <option value="sales">Sales</option>
              <option value="operations">Operations</option>
              <option value="finance">Finance</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              <option value="">All Status</option>
              <option value="processed">Processed</option>
              <option value="pending">Pending</option>
              <option value="approved">Approved</option>
            </select>
          </div>
          <div class="flex items-end">
            <button @click="refreshPayroll" class="w-full bg-gray-500 text-white px-4 py-2 rounded-lg hover:bg-gray-600 transition-colors">
              Refresh
            </button>
          </div>
        </div>
      </div>

      <!-- Payroll Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Employee</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Base Salary</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Overtime</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Deductions</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Net Pay</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="payroll in filteredPayroll" :key="payroll.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <div class="h-10 w-10 rounded-full bg-blue-500 flex items-center justify-center">
                        <span class="text-white font-medium">{{ payroll.employeeName.split(' ').map(n => n[0]).join('') }}</span>
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ payroll.employeeName }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ payroll.position }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">R{{ payroll.baseSalary.toLocaleString() }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">R{{ payroll.overtime.toLocaleString() }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-red-600">R{{ payroll.deductions.toLocaleString() }}</td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-green-600">R{{ payroll.netPay.toLocaleString() }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getStatusColor(payroll.status)" class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full">
                    {{ payroll.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <button @click="viewPayslip(payroll)" class="text-blue-600 hover:text-blue-900 mr-3">View</button>
                  <button @click="downloadPayslip(payroll)" class="text-green-600 hover:text-green-900">Download</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Payroll Summary -->
      <div class="mt-6 bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Payroll Summary</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="border-l-4 border-blue-500 pl-4">
            <p class="text-sm text-gray-600 dark:text-gray-400">Gross Pay</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ payrollSummary.grossPay.toLocaleString() }}</p>
          </div>
          <div class="border-l-4 border-red-500 pl-4">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Deductions</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ payrollSummary.totalDeductions.toLocaleString() }}</p>
          </div>
          <div class="border-l-4 border-green-500 pl-4">
            <p class="text-sm text-gray-600 dark:text-gray-400">Net Payable</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ payrollSummary.netPayable.toLocaleString() }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

definePageMeta({
  title: 'Payroll Management - TOSS ERP',
  description: 'Process payroll, manage salaries and generate payslips'
})

// Icons
const CurrencyDollarIcon = 'svg'
const DocumentIcon = 'svg'
const UsersIcon = 'svg'
const ExclamationTriangleIcon = 'svg'
const CheckCircleIcon = 'svg'

// Reactive data
const selectedPeriod = ref('2024-04')
const selectedDepartment = ref('')
const selectedStatus = ref('')

const payrollStats = ref({
  totalPayroll: 140000,
  employeesPaid: 8,
  totalDeductions: 32000,
  netPay: 108000
})

const payrollData = ref([
  {
    id: 1,
    employeeName: 'Thabo Mthembu',
    position: 'Store Owner/Manager',
    department: 'management',
    baseSalary: 35000,
    overtime: 2500,
    deductions: 8750,
    netPay: 28750,
    status: 'processed'
  },
  {
    id: 2,
    employeeName: 'Nomsa Khumalo',
    position: 'Assistant Manager',
    department: 'management',
    baseSalary: 22000,
    overtime: 1200,
    deductions: 5550,
    netPay: 17650,
    status: 'processed'
  },
  {
    id: 3,
    employeeName: 'Sipho Dube',
    position: 'Sales Clerk',
    department: 'sales',
    baseSalary: 15000,
    overtime: 800,
    deductions: 3700,
    netPay: 12100,
    status: 'processed'
  },
  {
    id: 4,
    employeeName: 'Lerato Masango',
    position: 'Cashier',
    department: 'sales',
    baseSalary: 12000,
    overtime: 600,
    deductions: 2800,
    netPay: 9800,
    status: 'approved'
  },
  {
    id: 5,
    employeeName: 'Mandla Ndaba',
    position: 'Stock Controller',
    department: 'operations',
    baseSalary: 18000,
    overtime: 1000,
    deductions: 4200,
    netPay: 14800,
    status: 'processed'
  },
  {
    id: 6,
    employeeName: 'Grace Mthethwa',
    position: 'Cleaner',
    department: 'operations',
    baseSalary: 8000,
    overtime: 300,
    deductions: 1800,
    netPay: 6500,
    status: 'pending'
  },
  {
    id: 7,
    employeeName: 'Bongani Sithole',
    position: 'Security Guard',
    department: 'operations',
    baseSalary: 10000,
    overtime: 1500,
    deductions: 2300,
    netPay: 9200,
    status: 'processed'
  },
  {
    id: 8,
    employeeName: 'Zinhle Mpofu',
    position: 'Bookkeeper',
    department: 'finance',
    baseSalary: 20000,
    overtime: 1100,
    deductions: 4900,
    netPay: 16200,
    status: 'approved'
  }
])

const payrollSummary = ref({
  grossPay: 148300,
  totalDeductions: 34000,
  netPayable: 114300
})

// Computed properties
const filteredPayroll = computed(() => {
  return payrollData.value.filter(payroll => {
    const matchesDepartment = !selectedDepartment.value || payroll.department === selectedDepartment.value
    const matchesStatus = !selectedStatus.value || payroll.status === selectedStatus.value
    
    return matchesDepartment && matchesStatus
  })
})

// Methods
const getStatusColor = (status) => {
  switch (status) {
    case 'processed':
      return 'bg-green-100 text-green-800'
    case 'approved':
      return 'bg-blue-100 text-blue-800'
    case 'pending':
      return 'bg-yellow-100 text-yellow-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const processPayroll = () => {
  console.log('Processing payroll for period:', selectedPeriod.value)
  // Implementation for processing payroll
}

const generatePayslips = () => {
  console.log('Generating payslips...')
  // Implementation for generating payslips
}

const viewPayslip = (payroll) => {
  console.log('View payslip for:', payroll.employeeName)
  // Implementation for viewing payslip
}

const downloadPayslip = (payroll) => {
  console.log('Download payslip for:', payroll.employeeName)
  // Implementation for downloading payslip
}

const refreshPayroll = () => {
  console.log('Refreshing payroll data...')
  // Implementation for refreshing data
}

useHead({
  title: 'Payroll Management - TOSS ERP',
  meta: [
    { name: 'description', content: 'Payroll processing and salary management in TOSS ERP' }
  ]
})
</script>
