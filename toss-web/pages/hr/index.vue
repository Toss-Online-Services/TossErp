<template>
  <div class="hr-dashboard">
    <!-- Page Header -->
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        HR & Payroll Dashboard
      </h1>
      <p class="text-gray-600 dark:text-gray-400 mt-1">
        Manage your employees, attendance, and payroll
      </p>
    </div>

    <!-- Quick Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
            <Icon name="mdi:account-group" class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Employees</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ activeEmployees.length }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
            <Icon name="mdi:clock-check" class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Present Today</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ todaysAttendance.filter(a => a.status === 'present').length }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
        <div class="flex items-center">
          <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
            <Icon name="mdi:calendar-clock" class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending Leaves</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ pendingLeaveRequests.length }}
            </p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
            <Icon name="mdi:currency-usd" class="w-6 h-6 text-purple-600 dark:text-purple-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Monthly Payroll</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">
              {{ formatCurrency(currentMonthPayrollTotal) }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="mb-8">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Quick Actions
      </h2>
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <button
          @click="navigateTo('/hr/employees/new')"
          class="flex items-center justify-center p-4 bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg hover:bg-blue-100 dark:hover:bg-blue-900/30 transition-colors"
        >
          <Icon name="mdi:account-plus" class="w-5 h-5 text-blue-600 dark:text-blue-400 mr-2" />
          <span class="text-blue-700 dark:text-blue-300">Add Employee</span>
        </button>

        <button
          @click="navigateTo('/hr/attendance')"
          class="flex items-center justify-center p-4 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg hover:bg-green-100 dark:hover:bg-green-900/30 transition-colors"
        >
          <Icon name="mdi:clock-in" class="w-5 h-5 text-green-600 dark:text-green-400 mr-2" />
          <span class="text-green-700 dark:text-green-300">Mark Attendance</span>
        </button>

        <button
          @click="navigateTo('/hr/payroll')"
          class="flex items-center justify-center p-4 bg-purple-50 dark:bg-purple-900/20 border border-purple-200 dark:border-purple-800 rounded-lg hover:bg-purple-100 dark:hover:bg-purple-900/30 transition-colors"
        >
          <Icon name="mdi:cash-multiple" class="w-5 h-5 text-purple-600 dark:text-purple-400 mr-2" />
          <span class="text-purple-700 dark:text-purple-300">Run Payroll</span>
        </button>

        <button
          @click="navigateTo('/hr/reports')"
          class="flex items-center justify-center p-4 bg-orange-50 dark:bg-orange-900/20 border border-orange-200 dark:border-orange-800 rounded-lg hover:bg-orange-100 dark:hover:bg-orange-900/30 transition-colors"
        >
          <Icon name="mdi:chart-line" class="w-5 h-5 text-orange-600 dark:text-orange-400 mr-2" />
          <span class="text-orange-700 dark:text-orange-300">View Reports</span>
        </button>
      </div>
    </div>

    <!-- Recent Activity -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Recent Employees -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
            Recent Employees
          </h3>
          <NuxtLink
            to="/hr/employees"
            class="text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-300 text-sm"
          >
            View All
          </NuxtLink>
        </div>
        
        <div class="space-y-3">
          <div
            v-for="employee in recentEmployees"
            :key="employee.id"
            class="flex items-center p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
          >
            <div class="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center">
              <span class="text-white font-medium text-sm">
                {{ employee.personalInfo.firstName.charAt(0) }}{{ employee.personalInfo.lastName.charAt(0) }}
              </span>
            </div>
            <div class="ml-3 flex-1">
              <p class="text-sm font-medium text-gray-900 dark:text-white">
                {{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}
              </p>
              <p class="text-xs text-gray-600 dark:text-gray-400">
                {{ employee.employment.position }}
              </p>
            </div>
            <div class="text-xs text-gray-500 dark:text-gray-400">
              {{ formatDate(employee.employment.startDate) }}
            </div>
          </div>

          <div v-if="!recentEmployees.length" class="text-center py-4">
            <p class="text-gray-500 dark:text-gray-400">No employees yet</p>
            <button
              @click="navigateTo('/hr/employees/new')"
              class="text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-300 text-sm mt-1"
            >
              Add your first employee
            </button>
          </div>
        </div>
      </div>

      <!-- Pending Leave Requests -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
            Pending Leave Requests
          </h3>
          <NuxtLink
            to="/hr/leave-requests"
            class="text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-300 text-sm"
          >
            View All
          </NuxtLink>
        </div>
        
        <div class="space-y-3">
          <div
            v-for="request in pendingLeaveRequests.slice(0, 5)"
            :key="request.id"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg"
          >
            <div class="flex-1">
              <div class="flex items-center">
                <span class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ getEmployeeName(request.employeeId) }}
                </span>
                <span class="ml-2 px-2 py-1 bg-yellow-100 dark:bg-yellow-900 text-yellow-800 dark:text-yellow-200 text-xs rounded">
                  {{ request.leaveType }}
                </span>
              </div>
              <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">
                {{ formatDate(request.startDate) }} - {{ formatDate(request.endDate) }}
                ({{ request.totalDays }} days)
              </p>
            </div>
            <div class="flex space-x-2 ml-4">
              <button
                @click="processLeaveRequest(request.id, 'approved')"
                class="p-1 text-green-600 dark:text-green-400 hover:text-green-800 dark:hover:text-green-300"
                title="Approve"
              >
                <Icon name="mdi:check" class="w-4 h-4" />
              </button>
              <button
                @click="processLeaveRequest(request.id, 'rejected')"
                class="p-1 text-red-600 dark:text-red-400 hover:text-red-800 dark:hover:text-red-300"
                title="Reject"
              >
                <Icon name="mdi:close" class="w-4 h-4" />
              </button>
            </div>
          </div>

          <div v-if="!pendingLeaveRequests.length" class="text-center py-4">
            <p class="text-gray-500 dark:text-gray-400">No pending leave requests</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-lg">
        <div class="flex items-center space-x-3">
          <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
          <span class="text-gray-900 dark:text-white">Loading...</span>
        </div>
      </div>
    </div>

    <!-- Error Message -->
    <div v-if="error" class="fixed bottom-4 right-4 z-50">
      <div class="bg-red-500 text-white p-4 rounded-lg shadow-lg max-w-sm">
        <div class="flex items-center justify-between">
          <span>{{ error }}</span>
          <button @click="error = null" class="ml-2 text-white hover:text-gray-200">
            <Icon name="mdi:close" class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { Employee, LeaveRequest } from '~/types/hr'

// Meta information
definePageMeta({
  title: 'HR Dashboard',
  layout: 'default'
})

// Use HR composable
const {
  employees,
  leaveRequests,
  attendanceRecords,
  currentMonthPayrollTotal,
  activeEmployees,
  pendingLeaveRequests,
  todaysAttendance,
  isLoading,
  error,
  fetchEmployees,
  processLeaveRequest,
  fetchAttendance,
  formatCurrency,
  formatDate
} = useHR()

// Local state
const recentEmployees = computed(() => 
  employees.value
    .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
    .slice(0, 5)
)

// Helper function to get employee name
const getEmployeeName = (employeeId: string): string => {
  const employee = employees.value.find(emp => emp.id === employeeId)
  return employee 
    ? `${employee.personalInfo.firstName} ${employee.personalInfo.lastName}`
    : 'Unknown Employee'
}

// Load data on mount
onMounted(async () => {
  try {
    await Promise.all([
      fetchEmployees(),
      fetchAttendance(undefined, new Date().toISOString().split('T')[0])
    ])
  } catch (err) {
    console.error('Failed to load HR dashboard data:', err)
  }
})
</script>

<style scoped>
.hr-dashboard {
  @apply p-6 max-w-7xl mx-auto;
}

/* Add any custom styles here */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>