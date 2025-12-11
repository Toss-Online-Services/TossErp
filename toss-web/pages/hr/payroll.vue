<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useHrStore, type PayrollRun } from '~/stores/hr'

useHead({ title: 'Payroll - TOSS' })

const hrStore = useHrStore()
const selectedEmployeeId = ref<number | null>(null)
const periodStart = ref(new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0])
const periodEnd = ref(new Date().toISOString().split('T')[0])
const isRunningPayroll = ref(false)

const filteredRuns = computed(() => {
  let filtered = hrStore.payrollRuns

  if (selectedEmployeeId.value) {
    filtered = filtered.filter(r => r.employeeId === selectedEmployeeId.value)
  }

  return filtered.sort((a, b) => 
    new Date(b.generatedAt).getTime() - new Date(a.generatedAt).getTime()
  )
})

async function runPayroll() {
  if (!periodStart.value || !periodEnd.value) {
    alert('Please select both start and end dates')
    return
  }

  const employeesToProcess = selectedEmployeeId.value
    ? [selectedEmployeeId.value]
    : hrStore.activeEmployees.map(e => e.id)

  if (employeesToProcess.length === 0) {
    alert('No active employees to process')
    return
  }

  isRunningPayroll.value = true
  try {
    await hrStore.runPayroll(
      employeesToProcess,
      new Date(periodStart.value),
      new Date(periodEnd.value)
    )
    alert(`Payroll processed successfully for ${employeesToProcess.length} employee(s)`)
    await hrStore.fetchPayrollRuns()
  } catch (error) {
    console.error('Failed to run payroll:', error)
    alert('Failed to process payroll. Please try again.')
  } finally {
    isRunningPayroll.value = false
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(date))
}

onMounted(() => {
  hrStore.fetchEmployees()
  hrStore.fetchPayrollRuns()
})
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Payroll</h3>
        <p class="text-gray-600 text-sm">Process employee salaries and wages</p>
      </div>
      <button
        @click="runPayroll"
        :disabled="isRunningPayroll"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2 transition-colors"
      >
        <i class="material-symbols-rounded text-xl">{{ isRunningPayroll ? 'sync' : 'play_arrow' }}</i>
        <span>{{ isRunningPayroll ? 'Processing...' : 'Run Payroll' }}</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Payroll</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatCurrency(hrStore.totalPayrollCost) }}</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-blue-600 text-2xl">payments</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Payroll Runs</p>
            <p class="text-2xl font-bold text-gray-900">{{ hrStore.payrollRuns.length }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-green-600 text-2xl">receipt</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Active Employees</p>
            <p class="text-2xl font-bold text-gray-900">{{ hrStore.activeEmployees.length }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-purple-600 text-2xl">badge</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Payroll Period Selection -->
    <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
      <h4 class="text-lg font-semibold text-gray-900 mb-4">Payroll Period</h4>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Start Date</label>
          <input
            v-model="periodStart"
            type="date"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">End Date</label>
          <input
            v-model="periodEnd"
            type="date"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Employee (Optional)</label>
          <select
            v-model="selectedEmployeeId"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option :value="null">All Employees</option>
            <option
              v-for="employee in hrStore.activeEmployees"
              :key="employee.id"
              :value="employee.id"
            >
              {{ employee.name }}
            </option>
          </select>
        </div>
        <div class="flex items-end">
          <button
            @click="runPayroll"
            :disabled="isRunningPayroll"
            class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            {{ isRunningPayroll ? 'Processing...' : 'Generate' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Payroll Runs List -->
    <div v-if="hrStore.loading" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
      <p class="text-gray-600 mt-4">Loading payroll runs...</p>
    </div>

    <div v-else-if="filteredRuns.length === 0" class="bg-white rounded-xl shadow-sm p-12 text-center">
      <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">payments</i>
      <h3 class="text-xl font-semibold text-gray-900 mb-2">No Payroll Runs</h3>
      <p class="text-gray-600 mb-6">Generate your first payroll run to get started</p>
      <button
        @click="runPayroll"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Run Payroll
      </button>
    </div>

    <div v-else class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Period</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Employee</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Gross</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Deductions</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Net</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Generated</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="run in filteredRuns"
              :key="run.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(run.periodStart) }} - {{ formatDate(run.periodEnd) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ run.employeeName || hrStore.getEmployeeById(run.employeeId)?.name || `Employee #${run.employeeId}` }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(run.gross) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(run.deductions) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                {{ formatCurrency(run.net) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                {{ formatDate(run.generatedAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
                <button
                  class="text-blue-600 hover:text-blue-800 px-2 py-1 rounded hover:bg-blue-50 transition-colors"
                  @click="alert('Payslip export coming soon')"
                >
                  <i class="material-symbols-rounded text-base">download</i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
