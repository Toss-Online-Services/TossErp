<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useHrStore, type Employee, type EmployeeRateType } from '~/stores/hr'
import EmployeeModal from '~/components/hr/EmployeeModal.vue'

useHead({ title: 'Employees - TOSS' })

const hrStore = useHrStore()
const showModal = ref(false)
const selectedEmployee = ref<Employee | null>(null)
const searchQuery = ref('')
const statusFilter = ref<'all' | 'active' | 'inactive'>('all')
const rateTypeFilter = ref<EmployeeRateType | 'all'>('all')

const filteredEmployees = computed(() => {
  let filtered = hrStore.employees

  // Filter by status
  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(e => 
      statusFilter.value === 'active' ? e.isActive : !e.isActive
    )
  }

  // Filter by rate type
  if (rateTypeFilter.value !== 'all') {
    filtered = filtered.filter(e => e.rateType === rateTypeFilter.value)
  }

  // Filter by search query
  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(e =>
      e.name.toLowerCase().includes(query) ||
      e.email?.toLowerCase().includes(query) ||
      e.phone?.includes(query) ||
      e.idNumber?.includes(query)
    )
  }

  return filtered.sort((a, b) => a.name.localeCompare(b.name))
})

function openCreateModal() {
  selectedEmployee.value = null
  showModal.value = true
}

function openEditModal(employee: Employee) {
  selectedEmployee.value = employee
  showModal.value = true
}

function handleSaved() {
  showModal.value = false
  selectedEmployee.value = null
  hrStore.fetchEmployees()
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function getRateTypeLabel(rateType: EmployeeRateType) {
  return rateType === 'Hourly' ? '/hr' : rateType === 'Daily' ? '/day' : '/month'
}

onMounted(() => {
  hrStore.fetchEmployees()
})
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Employees</h3>
        <p class="text-gray-600 text-sm">Manage employee records and information</p>
      </div>
      <button
        @click="openCreateModal"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center gap-2 transition-colors"
      >
        <i class="material-symbols-rounded text-xl">add</i>
        <span>Add Employee</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Employees</p>
            <p class="text-2xl font-bold text-gray-900">{{ hrStore.employees.length }}</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-blue-600 text-2xl">badge</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Active</p>
            <p class="text-2xl font-bold text-green-600">{{ hrStore.activeEmployees.length }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-green-600 text-2xl">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Hours</p>
            <p class="text-2xl font-bold text-gray-900">{{ hrStore.totalHoursWorked.toFixed(1) }}h</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-purple-600 text-2xl">schedule</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Payroll Cost</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatCurrency(hrStore.totalPayrollCost) }}</p>
          </div>
          <div class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-orange-600 text-2xl">payments</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-sm p-4 mb-6">
      <div class="flex flex-col sm:flex-row gap-4">
        <div class="flex-1">
          <div class="relative">
            <i class="material-symbols-rounded absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">search</i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search employees..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
        <div class="sm:w-48">
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="all">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>
        </div>
        <div class="sm:w-48">
          <select
            v-model="rateTypeFilter"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="all">All Rate Types</option>
            <option value="Hourly">Hourly</option>
            <option value="Daily">Daily</option>
            <option value="Monthly">Monthly</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Employees List -->
    <div v-if="hrStore.loading" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
      <p class="text-gray-600 mt-4">Loading employees...</p>
    </div>

    <div v-else-if="filteredEmployees.length === 0" class="bg-white rounded-xl shadow-sm p-12 text-center">
      <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">badge</i>
      <h3 class="text-xl font-semibold text-gray-900 mb-2">No Employees Found</h3>
      <p class="text-gray-600 mb-6">Add your first employee to get started</p>
      <button
        @click="openCreateModal"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Add Employee
      </button>
    </div>

    <div v-else class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Name</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Contact</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rate Type</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rate</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="employee in filteredEmployees"
              :key="employee.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                    <span class="text-blue-600 font-semibold">{{ employee.name.charAt(0) }}</span>
                  </div>
                  <div>
                    <div class="text-sm font-medium text-gray-900">{{ employee.name }}</div>
                    <div v-if="employee.idNumber" class="text-xs text-gray-500">ID: {{ employee.idNumber }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ employee.phone || '-' }}</div>
                <div class="text-xs text-gray-500">{{ employee.email || '-' }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="px-2 py-1 text-xs font-medium rounded-full bg-blue-100 text-blue-800">
                  {{ employee.rateType }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(employee.rate) }}{{ getRateTypeLabel(employee.rateType) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="[
                    'px-2 py-1 text-xs font-medium rounded-full',
                    employee.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                  ]"
                >
                  {{ employee.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
                <button
                  @click="openEditModal(employee)"
                  class="text-blue-600 hover:text-blue-800 px-2 py-1 rounded hover:bg-blue-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-base">edit</i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Employee Modal -->
    <EmployeeModal
      v-if="showModal"
      :show="showModal"
      :employee="selectedEmployee"
      @close="showModal = false"
      @saved="handleSaved"
    />
  </div>
</template>
