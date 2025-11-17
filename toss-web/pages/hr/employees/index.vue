<template>
  <div class="employees-list">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Employees
        </h1>
        <p class="text-gray-600 dark:text-gray-400 mt-1">
          Manage your employee records and information
        </p>
      </div>
      <div class="mt-4 sm:mt-0">
        <button
          @click="navigateTo('/hr/employees/new')"
          class="inline-flex items-center px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          <Icon name="mdi:plus" class="w-4 h-4 mr-2" />
          Add Employee
        </button>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- Search -->
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Search
          </label>
          <div class="relative">
            <input
              v-model="filters.search"
              type="text"
              placeholder="Search employees..."
              class="block w-full pl-10 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md placeholder-gray-400 focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
            >
            <Icon name="mdi:magnify" class="absolute left-3 top-2.5 w-5 h-5 text-gray-400" />
          </div>
        </div>

        <!-- Status Filter -->
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Status
          </label>
          <select
            v-model="filters.status"
            class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
          >
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
            <option value="terminated">Terminated</option>
            <option value="suspended">Suspended</option>
          </select>
        </div>

        <!-- Employment Type Filter -->
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Employment Type
          </label>
          <select
            v-model="filters.employmentType"
            class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
          >
            <option value="">All Types</option>
            <option value="permanent">Permanent</option>
            <option value="contract">Contract</option>
            <option value="casual">Casual</option>
            <option value="part-time">Part-time</option>
          </select>
        </div>

        <!-- Department Filter -->
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Department
          </label>
          <select
            v-model="filters.department"
            class="block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:text-white"
          >
            <option value="">All Departments</option>
            <option v-for="dept in departments" :key="dept" :value="dept">
              {{ dept }}
            </option>
          </select>
        </div>
      </div>

      <!-- Filter Actions -->
      <div class="flex justify-between items-center mt-4">
        <button
          @click="clearFilters"
          class="text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-200 text-sm"
        >
          Clear Filters
        </button>
        <div class="flex space-x-2">
          <button
            @click="exportEmployees"
            class="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            <Icon name="mdi:download" class="w-4 h-4 mr-2" />
            Export
          </button>
          <button
            @click="refreshData"
            class="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            <Icon name="mdi:refresh" class="w-4 h-4 mr-2" />
            Refresh
          </button>
        </div>
      </div>
    </div>

    <!-- Employee Cards (Mobile) -->
    <div class="block lg:hidden space-y-4">
      <div
        v-for="employee in filteredEmployees"
        :key="employee.id"
        class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow hover:shadow-md transition-shadow cursor-pointer"
        @click="navigateTo(`/hr/employees/${employee.id}`)"
      >
        <div class="flex items-start space-x-4">
          <!-- Avatar -->
          <div class="flex-shrink-0">
            <div class="w-12 h-12 bg-blue-500 rounded-full flex items-center justify-center">
              <span class="text-white font-medium">
                {{ employee.personalInfo.firstName.charAt(0) }}{{ employee.personalInfo.lastName.charAt(0) }}
              </span>
            </div>
          </div>
          
          <!-- Employee Info -->
          <div class="flex-1 min-w-0">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white truncate">
                {{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}
              </h3>
              <StatusBadge :status="employee.status" />
            </div>
            
            <p class="text-gray-600 dark:text-gray-400 text-sm mt-1">
              {{ employee.employment.position }}
            </p>
            
            <div class="flex items-center justify-between mt-2">
              <span class="text-sm text-gray-500 dark:text-gray-400">
                {{ employee.employeeNumber }}
              </span>
              <span class="text-sm text-gray-500 dark:text-gray-400">
                Started {{ formatDate(employee.employment.startDate) }}
              </span>
            </div>
            
            <div class="flex items-center mt-2 space-x-4">
              <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200">
                {{ employee.employment.employmentType }}
              </span>
              <span class="text-sm text-gray-600 dark:text-gray-400">
                {{ formatCurrency(employee.compensation.basicSalary) }}/month
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Employee Table (Desktop) -->
    <div class="hidden lg:block bg-white dark:bg-gray-800 shadow rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider cursor-pointer" @click="sortBy('personalInfo.firstName')">
              <div class="flex items-center space-x-1">
                <span>Employee</span>
                <Icon name="mdi:sort" class="w-4 h-4" />
              </div>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider cursor-pointer" @click="sortBy('employeeNumber')">
              <div class="flex items-center space-x-1">
                <span>Employee #</span>
                <Icon name="mdi:sort" class="w-4 h-4" />
              </div>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider cursor-pointer" @click="sortBy('employment.position')">
              <div class="flex items-center space-x-1">
                <span>Position</span>
                <Icon name="mdi:sort" class="w-4 h-4" />
              </div>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider cursor-pointer" @click="sortBy('employment.employmentType')">
              <div class="flex items-center space-x-1">
                <span>Type</span>
                <Icon name="mdi:sort" class="w-4 h-4" />
              </div>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider cursor-pointer" @click="sortBy('status')">
              <div class="flex items-center space-x-1">
                <span>Status</span>
                <Icon name="mdi:sort" class="w-4 h-4" />
              </div>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider cursor-pointer" @click="sortBy('compensation.basicSalary')">
              <div class="flex items-center space-x-1">
                <span>Salary</span>
                <Icon name="mdi:sort" class="w-4 h-4" />
              </div>
            </th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider cursor-pointer" @click="sortBy('employment.startDate')">
              <div class="flex items-center space-x-1">
                <span>Start Date</span>
                <Icon name="mdi:sort" class="w-4 h-4" />
              </div>
            </th>
            <th class="relative px-6 py-3">
              <span class="sr-only">Actions</span>
            </th>
          </tr>
        </thead>
        <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          <tr
            v-for="employee in paginatedEmployees"
            :key="employee.id"
            class="hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer"
            @click="navigateTo(`/hr/employees/${employee.id}`)"
          >
            <!-- Employee Info -->
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <div class="flex-shrink-0 h-10 w-10">
                  <div class="h-10 w-10 bg-blue-500 rounded-full flex items-center justify-center">
                    <span class="text-white font-medium text-sm">
                      {{ employee.personalInfo.firstName.charAt(0) }}{{ employee.personalInfo.lastName.charAt(0) }}
                    </span>
                  </div>
                </div>
                <div class="ml-4">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ employee.personalInfo.firstName }} {{ employee.personalInfo.lastName }}
                  </div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">
                    {{ employee.contactInfo.email || employee.contactInfo.phoneNumber }}
                  </div>
                </div>
              </div>
            </td>
            
            <!-- Employee Number -->
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
              {{ employee.employeeNumber }}
            </td>
            
            <!-- Position -->
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900 dark:text-white">{{ employee.employment.position }}</div>
              <div class="text-sm text-gray-500 dark:text-gray-400">{{ employee.employment.department || 'No Department' }}</div>
            </td>
            
            <!-- Employment Type -->
            <td class="px-6 py-4 whitespace-nowrap">
              <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200">
                {{ employee.employment.employmentType }}
              </span>
            </td>
            
            <!-- Status -->
            <td class="px-6 py-4 whitespace-nowrap">
              <StatusBadge :status="employee.status" />
            </td>
            
            <!-- Salary -->
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
              {{ formatCurrency(employee.compensation.basicSalary) }}
            </td>
            
            <!-- Start Date -->
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
              {{ formatDate(employee.employment.startDate) }}
            </td>
            
            <!-- Actions -->
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center space-x-2">
                <button
                  @click.stop="navigateTo(`/hr/employees/${employee.id}/edit`)"
                  class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300"
                  title="Edit"
                >
                  <Icon name="mdi:pencil" class="w-4 h-4" />
                </button>
                <button
                  @click.stop="viewEmployee(employee.id)"
                  class="text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-300"
                  title="View"
                >
                  <Icon name="mdi:eye" class="w-4 h-4" />
                </button>
                <button
                  @click.stop="deleteEmployeeConfirm(employee.id)"
                  class="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300"
                  title="Delete"
                >
                  <Icon name="mdi:delete" class="w-4 h-4" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      
      <!-- No employees state -->
      <div v-if="!filteredEmployees.length" class="text-center py-12">
        <Icon name="mdi:account-group" class="w-16 h-16 text-gray-400 mx-auto mb-4" />
        <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-2">No employees found</h3>
        <p class="text-gray-500 dark:text-gray-400 mb-4">
          {{ filters.search || filters.status || filters.employmentType || filters.department 
            ? 'Try adjusting your search criteria' 
            : 'Get started by adding your first employee' }}
        </p>
        <button
          v-if="!filters.search && !filters.status && !filters.employmentType && !filters.department"
          @click="navigateTo('/hr/employees/new')"
          class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
        >
          <Icon name="mdi:plus" class="w-4 h-4 mr-2" />
          Add Employee
        </button>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="filteredEmployees.length > pageSize" class="bg-white dark:bg-gray-800 px-4 py-3 flex items-center justify-between border-t border-gray-200 dark:border-gray-700 sm:px-6 mt-6 rounded-lg">
      <div class="flex-1 flex justify-between sm:hidden">
        <button
          @click="previousPage"
          :disabled="currentPage === 1"
          class="relative inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Previous
        </button>
        <button
          @click="nextPage"
          :disabled="currentPage === totalPages"
          class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Next
        </button>
      </div>
      <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
        <div>
          <p class="text-sm text-gray-700 dark:text-gray-300">
            Showing
            <span class="font-medium">{{ ((currentPage - 1) * pageSize) + 1 }}</span>
            to
            <span class="font-medium">{{ Math.min(currentPage * pageSize, filteredEmployees.length) }}</span>
            of
            <span class="font-medium">{{ filteredEmployees.length }}</span>
            results
          </p>
        </div>
        <div>
          <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
            <button
              @click="previousPage"
              :disabled="currentPage === 1"
              class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-sm font-medium text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span class="sr-only">Previous</span>
              <Icon name="mdi:chevron-left" class="h-5 w-5" />
            </button>
            
            <button
              v-for="page in visiblePages"
              :key="page"
              @click="goToPage(page)"
              :class="[
                'relative inline-flex items-center px-4 py-2 border text-sm font-medium',
                page === currentPage 
                  ? 'z-10 bg-blue-50 dark:bg-blue-900 border-blue-500 dark:border-blue-400 text-blue-600 dark:text-blue-300'
                  : 'bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600 text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-600'
              ]"
            >
              {{ page }}
            </button>
            
            <button
              @click="nextPage"
              :disabled="currentPage === totalPages"
              class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-sm font-medium text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <span class="sr-only">Next</span>
              <Icon name="mdi:chevron-right" class="h-5 w-5" />
            </button>
          </nav>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-lg">
        <div class="flex items-center space-x-3">
          <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
          <span class="text-gray-900 dark:text-white">Loading employees...</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import type { Employee, EmployeeFilters } from '~/types/hr'

// Meta
definePageMeta({
  title: 'Employees',
  layout: 'default'
})

// Composables
const {
  employees,
  isLoading,
  error,
  fetchEmployees,
  deleteEmployee,
  formatCurrency,
  formatDate
} = useHR()

// Local state
const filters = reactive<EmployeeFilters>({
  search: '',
  status: undefined,
  department: '',
  employmentType: undefined
})

const sortField = ref('')
const sortDirection = ref<'asc' | 'desc'>('asc')
const currentPage = ref(1)
const pageSize = ref(10)

// Computed properties
const departments = computed(() => {
  const depts = new Set(
    employees.value
      .map(emp => emp.employment.department)
      .filter(Boolean)
  )
  return Array.from(depts).sort()
})

const filteredEmployees = computed(() => {
  let filtered = employees.value

  // Apply filters
  if (filters.search) {
    const search = filters.search.toLowerCase()
    filtered = filtered.filter(emp => 
      emp.personalInfo.firstName.toLowerCase().includes(search) ||
      emp.personalInfo.lastName.toLowerCase().includes(search) ||
      emp.employeeNumber.toLowerCase().includes(search) ||
      emp.employment.position.toLowerCase().includes(search) ||
      emp.contactInfo.email?.toLowerCase().includes(search) ||
      emp.contactInfo.phoneNumber.includes(search)
    )
  }

  if (filters.status) {
    filtered = filtered.filter(emp => emp.status === filters.status)
  }

  if (filters.department) {
    filtered = filtered.filter(emp => emp.employment.department === filters.department)
  }

  if (filters.employmentType) {
    filtered = filtered.filter(emp => emp.employment.employmentType === filters.employmentType)
  }

  // Apply sorting
  if (sortField.value) {
    filtered.sort((a, b) => {
      const getValue = (obj: any, path: string) => {
        return path.split('.').reduce((value, key) => value?.[key], obj)
      }

      const aValue = getValue(a, sortField.value)
      const bValue = getValue(b, sortField.value)

      if (aValue === bValue) return 0
      
      const comparison = aValue < bValue ? -1 : 1
      return sortDirection.value === 'asc' ? comparison : -comparison
    })
  }

  return filtered
})

const totalPages = computed(() => Math.ceil(filteredEmployees.value.length / pageSize.value))

const paginatedEmployees = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredEmployees.value.slice(start, end)
})

const visiblePages = computed(() => {
  const pages = []
  const total = totalPages.value
  const current = currentPage.value
  
  // Always show first page
  if (total > 0) pages.push(1)
  
  // Show pages around current
  for (let i = Math.max(2, current - 1); i <= Math.min(total - 1, current + 1); i++) {
    pages.push(i)
  }
  
  // Always show last page if it's not already included
  if (total > 1 && !pages.includes(total)) {
    pages.push(total)
  }
  
  return pages
})

// Methods
const sortBy = (field: string) => {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortDirection.value = 'asc'
  }
}

const clearFilters = () => {
  Object.assign(filters, {
    search: '',
    status: undefined,
    department: '',
    employmentType: undefined
  })
  currentPage.value = 1
}

const refreshData = async () => {
  try {
    await fetchEmployees()
  } catch (err) {
    console.error('Failed to refresh employees:', err)
  }
}

const exportEmployees = () => {
  // TODO: Implement export functionality
  console.log('Export employees functionality to be implemented')
}

const viewEmployee = (id: string) => {
  navigateTo(`/hr/employees/${id}`)
}

const deleteEmployeeConfirm = async (id: string) => {
  const employee = employees.value.find(emp => emp.id === id)
  if (!employee) return

  const confirmed = confirm(
    `Are you sure you want to delete ${employee.personalInfo.firstName} ${employee.personalInfo.lastName}? This action cannot be undone.`
  )

  if (confirmed) {
    try {
      await deleteEmployee(id)
    } catch (err) {
      console.error('Failed to delete employee:', err)
    }
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const goToPage = (page: number) => {
  currentPage.value = page
}

// Watchers
watch(() => filteredEmployees.value.length, () => {
  // Reset to first page when filters change
  if (currentPage.value > totalPages.value) {
    currentPage.value = 1
  }
})

// Lifecycle
onMounted(async () => {
  try {
    await fetchEmployees()
  } catch (err) {
    console.error('Failed to load employees:', err)
  }
})
</script>

<script lang="ts">
// Status Badge Component
const StatusBadge = defineComponent({
  props: {
    status: {
      type: String as PropType<Employee['status']>,
      required: true
    }
  },
  setup(props) {
    const statusConfig = computed(() => {
      switch (props.status) {
        case 'active':
          return { class: 'bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200', label: 'Active' }
        case 'inactive':
          return { class: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200', label: 'Inactive' }
        case 'terminated':
          return { class: 'bg-red-100 dark:bg-red-900 text-red-800 dark:text-red-200', label: 'Terminated' }
        case 'suspended':
          return { class: 'bg-yellow-100 dark:bg-yellow-900 text-yellow-800 dark:text-yellow-200', label: 'Suspended' }
        default:
          return { class: 'bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-200', label: 'Unknown' }
      }
    })

    return () => h('span', {
      class: `inline-flex px-2 py-1 text-xs font-semibold rounded-full ${statusConfig.value.class}`
    }, statusConfig.value.label)
  }
})
</script>

<style scoped>
.employees-list {
  @apply p-6 max-w-7xl mx-auto;
}
</style>