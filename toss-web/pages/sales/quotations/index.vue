<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Sales Quotations
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          Manage quotes and proposals for customers
        </p>
      </div>
      <div class="flex items-center gap-2">
        <NuxtLink to="/sales/quotations/create" class="btn btn-primary">
          <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
          New Quotation
        </NuxtLink>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <div v-for="stat in stats" :key="stat.name" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-4 flex items-start justify-between">
        <div>
          <p class="text-sm font-medium text-gray-500 dark:text-gray-400">{{ stat.name }}</p>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stat.value }}</p>
        </div>
        <div class="p-2 rounded-full" :class="stat.bgColor">
          <Icon :name="stat.icon" class="w-6 h-6" :class="stat.iconColor" />
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-4 mb-6">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <FormKit
          type="search"
          placeholder="Search..."
          v-model="filters.search"
        />
        <FormKit
          type="select"
          label="Status"
          v-model="filters.status"
          :options="['All', 'Draft', 'Sent', 'Accepted', 'Expired']"
        />
        <FormKit
          type="date"
          label="Start Date"
          v-model="filters.startDate"
        />
        <FormKit
          type="date"
          label="End Date"
          v-model="filters.endDate"
        />
      </div>
    </div>

    <!-- Quotations Table -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="table-header">Quotation #</th>
            <th class="table-header">Customer</th>
            <th class="table-header">Date</th>
            <th class="table-header text-right">Amount</th>
            <th class="table-header text-center">Status</th>
            <th class="table-header text-right">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="q in filteredQuotations" :key="q.id">
            <td class="table-cell font-medium">{{ q.id }}</td>
            <td class="table-cell">{{ q.customerName }}</td>
            <td class="table-cell">{{ formatDate(q.date) }}</td>
            <td class="table-cell text-right">{{ formatCurrency(q.amount) }}</td>
            <td class="table-cell text-center">
              <span :class="statusClasses(q.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
                {{ q.status }}
              </span>
            </td>
            <td class="table-cell text-right">
              <NuxtLink :to="`/sales/quotations/${q.id}`" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
                View
              </NuxtLink>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { FormKit } from '@formkit/vue'

useHead({
  title: 'Sales Quotations',
})

// Mock Data
const stats = ref([
  { name: 'Draft', value: 5, icon: 'heroicons:document', bgColor: 'bg-yellow-100', iconColor: 'text-yellow-600' },
  { name: 'Sent', value: 23, icon: 'heroicons:paper-airplane', bgColor: 'bg-blue-100', iconColor: 'text-blue-600' },
  { name: 'Accepted', value: 15, icon: 'heroicons:check-circle', bgColor: 'bg-green-100', iconColor: 'text-green-600' },
  { name: 'Expired', value: 2, icon: 'heroicons:exclamation-circle', bgColor: 'bg-red-100', iconColor: 'text-red-600' },
])

const quotations = ref([
  { id: 'Q-2025-001', customerName: 'Jabu\'s Spaza', date: '2025-11-10', amount: 1907.78, status: 'Accepted' },
  { id: 'Q-2025-002', customerName: 'Sipho\'s Tavern', date: '2025-11-11', amount: 5430.00, status: 'Sent' },
  { id: 'Q-2025-003', customerName: 'The Gogo Shop', date: '2025-11-12', amount: 850.50, status: 'Draft' },
  { id: 'Q-2025-004', customerName: 'Jabu\'s Spaza', date: '2025-10-01', amount: 1200.00, status: 'Expired' },
])

const filters = ref({
  search: '',
  status: 'All',
  startDate: '',
  endDate: '',
})

const filteredQuotations = computed(() => {
  return quotations.value.filter(q => {
    const searchMatch = filters.value.search ? q.customerName.toLowerCase().includes(filters.value.search.toLowerCase()) || q.id.toLowerCase().includes(filters.value.search.toLowerCase()) : true
    const statusMatch = filters.value.status !== 'All' ? q.status === filters.value.status : true
    const startDateMatch = filters.value.startDate ? new Date(q.date) >= new Date(filters.value.startDate) : true
    const endDateMatch = filters.value.endDate ? new Date(q.date) <= new Date(filters.value.endDate) : true
    return searchMatch && statusMatch && startDateMatch && endDateMatch
  })
})

// Helper functions
const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')
const formatCurrency = (amount: number) => new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount)

const statusClasses = (status: string) => {
  switch (status) {
    case 'Draft': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'Sent': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'Accepted': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'Expired': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}
</script>

<style scoped>
.table-header {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300;
}
.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm text-gray-600 dark:text-gray-300;
}
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-primary {
  @apply text-white bg-blue-600 hover:bg-blue-700 focus:ring-blue-500;
}
</style>
