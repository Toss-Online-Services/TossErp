<template>
  <div class="space-y-6">
    <PageHeader
      title="Accounting Periods"
      description="Manage accounting periods and financial year closing"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-2">
        <select v-model="selectedFiscalYear" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">Select Fiscal Year</option>
          <option v-for="year in fiscalYears" :key="year.id" :value="year.id">{{ year.name }}</option>
        </select>
      </div>
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Generate Periods
      </button>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div v-for="period in periods" :key="period.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex justify-between items-start mb-3">
          <div>
            <h3 class="font-semibold text-lg dark:text-white">{{ period.name }}</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ period.startDate }} - {{ period.endDate }}</p>
          </div>
          <span :class="getStatusClass(period.status)" class="px-2 py-1 text-xs rounded-full">
            {{ period.status }}
          </span>
        </div>

        <div class="space-y-2 mb-4">
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Transactions:</span>
            <span class="font-medium dark:text-white">{{ period.transactionCount }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Total Amount:</span>
            <span class="font-medium dark:text-white">{{ formatCurrency(period.totalAmount) }}</span>
          </div>
        </div>

        <div class="flex gap-2">
          <button v-if="period.status === 'Open'" class="flex-1 px-3 py-1 text-sm bg-yellow-100 text-yellow-800 rounded hover:bg-yellow-200">
            Close Period
          </button>
          <button v-if="period.status === 'Closed'" class="flex-1 px-3 py-1 text-sm bg-green-100 text-green-800 rounded hover:bg-green-200">
            Reopen
          </button>
          <button class="px-3 py-1 text-sm border rounded hover:bg-gray-50 dark:hover:bg-gray-700 dark:text-white">
            View
          </button>
        </div>
      </div>
    </div>

    <div v-if="periods.length === 0" class="bg-white dark:bg-gray-800 rounded-lg shadow p-12 text-center">
      <p class="text-gray-500 dark:text-gray-400 mb-4">No periods found</p>
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Generate Periods
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Accounting Periods - TOSS ERP',
})

const selectedFiscalYear = ref('')

const fiscalYears = ref([
  { id: 1, name: 'FY 2024', startDate: '2024-01-01', endDate: '2024-12-31' },
  { id: 2, name: 'FY 2023', startDate: '2023-01-01', endDate: '2023-12-31' },
])

const periods = ref([
  { id: 1, name: 'January 2024', startDate: '2024-01-01', endDate: '2024-01-31', status: 'Closed', transactionCount: 234, totalAmount: 456789 },
  { id: 2, name: 'February 2024', startDate: '2024-02-01', endDate: '2024-02-29', status: 'Closed', transactionCount: 198, totalAmount: 389012 },
  { id: 3, name: 'March 2024', startDate: '2024-03-01', endDate: '2024-03-31', status: 'Closed', transactionCount: 276, totalAmount: 567234 },
  { id: 4, name: 'April 2024', startDate: '2024-04-01', endDate: '2024-04-30', status: 'Open', transactionCount: 145, totalAmount: 234567 },
  { id: 5, name: 'May 2024', startDate: '2024-05-01', endDate: '2024-05-31', status: 'Open', transactionCount: 89, totalAmount: 156789 },
  { id: 6, name: 'June 2024', startDate: '2024-06-01', endDate: '2024-06-30', status: 'Open', transactionCount: 0, totalAmount: 0 },
])

const getStatusClass = (status: string) => {
  switch(status) {
    case 'Open': return 'bg-green-100 text-green-800'
    case 'Closed': return 'bg-gray-100 text-gray-800'
    default: return 'bg-blue-100 text-blue-800'
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount)
}
</script>
