<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.invoices.list.title') }}
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('sales.invoices.list.description') }}
        </p>
      </div>
      <div class="flex items-center gap-2">
        <NuxtLink to="/sales/invoices/create" class="btn btn-primary">
          <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
          {{ $t('sales.invoices.list.newInvoice') }}
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
          :placeholder="$t('common.search') + '...'"
          v-model="filters.search"
        />
        <FormKit
          type="select"
          :label="$t('common.status')"
          v-model="filters.status"
          :options="['All', 'Draft', 'Sent', 'Paid', 'Overdue']"
        />
        <FormKit
          type="date"
          :label="$t('common.startDate')"
          v-model="filters.startDate"
        />
        <FormKit
          type="date"
          :label="$t('common.endDate')"
          v-model="filters.endDate"
        />
      </div>
    </div>

    <!-- Invoices Table -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="table-header">{{ $t('sales.invoices.list.invoice') }} #</th>
            <th class="table-header">{{ $t('sales.invoices.list.customer') }}</th>
            <th class="table-header">{{ $t('sales.invoices.list.issueDate') }}</th>
            <th class="table-header">{{ $t('sales.invoices.list.dueDate') }}</th>
            <th class="table-header text-right">{{ $t('common.amount') }}</th>
            <th class="table-header text-center">{{ $t('common.status') }}</th>
            <th class="table-header text-right">{{ $t('common.actions') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="invoice in filteredInvoices" :key="invoice.id">
            <td class="table-cell font-medium">{{ invoice.id }}</td>
            <td class="table-cell">{{ invoice.customerName }}</td>
            <td class="table-cell">{{ formatDate(invoice.issueDate) }}</td>
            <td class="table-cell">{{ formatDate(invoice.dueDate) }}</td>
            <td class="table-cell text-right">{{ formatCurrency(invoice.amount) }}</td>
            <td class="table-cell text-center">
              <span :class="statusClasses(invoice.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
                {{ invoice.status }}
              </span>
            </td>
            <td class="table-cell text-right">
              <NuxtLink :to="`/sales/invoices/${invoice.id}`" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
                {{ $t('common.view') }}
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

const { t } = useI18n()
useHead({
  title: t('sales.invoices.list.pageTitle'),
})

// Mock Data
const stats = ref([
  { name: t('sales.invoices.list.unpaid'), value: 'R 12,500.00', icon: 'heroicons:banknotes', bgColor: 'bg-yellow-100', iconColor: 'text-yellow-600' },
  { name: t('sales.invoices.list.overdue'), value: 'R 3,200.00', icon: 'heroicons:exclamation-triangle', bgColor: 'bg-red-100', iconColor: 'text-red-600' },
  { name: t('sales.invoices.list.draft'), value: 2, icon: 'heroicons:pencil-square', bgColor: 'bg-gray-100', iconColor: 'text-gray-600' },
  { name: t('sales.invoices.list.paidLast30d'), value: 'R 45,800.00', icon: 'heroicons:check-circle', bgColor: 'bg-green-100', iconColor: 'text-green-600' },
])

const invoices = ref([
  { id: 'INV-001', customerName: 'Jabu\'s Spaza', issueDate: '2025-11-10', dueDate: '2025-12-10', amount: 5250.00, status: 'Paid' },
  { id: 'INV-002', customerName: 'Sipho\'s Tavern', issueDate: '2025-11-12', dueDate: '2025-12-12', amount: 8700.50, status: 'Sent' },
  { id: 'INV-003', customerName: 'The Gogo Shop', issueDate: '2025-10-05', dueDate: '2025-11-05', amount: 3200.00, status: 'Overdue' },
  { id: 'INV-004', customerName: 'Jabu\'s Spaza', issueDate: '2025-11-15', dueDate: '2025-12-15', amount: 4500.00, status: 'Draft' },
])

const filters = ref({
  search: '',
  status: 'All',
  startDate: '',
  endDate: '',
})

const filteredInvoices = computed(() => {
  return invoices.value.filter(inv => {
    const searchMatch = filters.value.search ? inv.customerName.toLowerCase().includes(filters.value.search.toLowerCase()) || inv.id.toLowerCase().includes(filters.value.search.toLowerCase()) : true
    const statusMatch = filters.value.status !== 'All' ? inv.status === filters.value.status : true
    const startDateMatch = filters.value.startDate ? new Date(inv.issueDate) >= new Date(filters.value.startDate) : true
    const endDateMatch = filters.value.endDate ? new Date(inv.issueDate) <= new Date(filters.value.endDate) : true
    return searchMatch && statusMatch && startDateMatch && endDateMatch
  })
})

// Helper functions
const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')
const formatCurrency = (value: number) => new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value)

const statusClasses = (status: string) => {
  switch (status) {
    case 'Paid': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'Sent': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'Overdue': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    case 'Draft': return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
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
