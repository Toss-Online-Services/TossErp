<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.orders.list.title') }}
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('sales.orders.list.description') }}
        </p>
      </div>
      <div class="flex items-center gap-2">
        <NuxtLink to="/sales/orders/create" class="btn btn-primary">
          <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
          {{ $t('sales.orders.list.newOrder') }}
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
          :options="['All', 'Pending', 'Confirmed', 'Processing', 'Shipped', 'Completed', 'Cancelled']"
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

    <!-- Sales Orders Table -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="table-header">{{ $t('sales.orders.list.order') }} #</th>
            <th class="table-header">{{ $t('sales.orders.list.customer') }}</th>
            <th class="table-header">{{ $t('sales.orders.list.date') }}</th>
            <th class="table-header text-right">{{ $t('common.amount') }}</th>
            <th class="table-header text-center">{{ $t('common.status') }}</th>
            <th class="table-header text-right">{{ $t('common.actions') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="order in filteredOrders" :key="order.id">
            <td class="table-cell font-medium">{{ order.id }}</td>
            <td class="table-cell">{{ order.customerName }}</td>
            <td class="table-cell">{{ formatDate(order.date) }}</td>
            <td class="table-cell text-right">{{ formatCurrency(order.amount) }}</td>
            <td class="table-cell text-center">
              <span :class="statusClasses(order.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
                {{ order.status }}
              </span>
            </td>
            <td class="table-cell text-right">
              <NuxtLink :to="`/sales/orders/${order.id}`" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
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
  title: t('sales.orders.list.pageTitle'),
})

// Mock Data
const stats = ref([
  { name: t('sales.orders.list.pending'), value: 5, icon: 'heroicons:clock', bgColor: 'bg-yellow-100', iconColor: 'text-yellow-600' },
  { name: t('sales.orders.list.processing'), value: 8, icon: 'heroicons:cog', bgColor: 'bg-blue-100', iconColor: 'text-blue-600' },
  { name: t('sales.orders.list.shipped'), value: 12, icon: 'heroicons:truck', bgColor: 'bg-purple-100', iconColor: 'text-purple-600' },
  { name: t('sales.orders.list.revenueMonth'), value: 'R 88,250.00', icon: 'heroicons:chart-bar', bgColor: 'bg-green-100', iconColor: 'text-green-600' },
])

const orders = ref([
  { id: 'SO-123', customerName: 'Jabu\'s Spaza', date: '2025-11-10', amount: 5250.00, status: 'Completed' },
  { id: 'SO-124', customerName: 'Sipho\'s Tavern', date: '2025-11-12', amount: 8700.50, status: 'Shipped' },
  { id: 'SO-125', customerName: 'The Gogo Shop', date: '2025-11-14', amount: 3200.00, status: 'Processing' },
  { id: 'SO-126', customerName: 'Jabu\'s Spaza', date: '2025-11-15', amount: 4500.00, status: 'Pending' },
  { id: 'SO-127', customerName: 'Sipho\'s Tavern', date: '2025-11-16', amount: 1250.00, status: 'Cancelled' },
])

const filters = ref({
  search: '',
  status: 'All',
  startDate: '',
  endDate: '',
})

const filteredOrders = computed(() => {
  return orders.value.filter(order => {
    const searchMatch = filters.value.search ? order.customerName.toLowerCase().includes(filters.value.search.toLowerCase()) || order.id.toLowerCase().includes(filters.value.search.toLowerCase()) : true
    const statusMatch = filters.value.status !== 'All' ? order.status === filters.value.status : true
    const startDateMatch = filters.value.startDate ? new Date(order.date) >= new Date(filters.value.startDate) : true
    const endDateMatch = filters.value.endDate ? new Date(order.date) <= new Date(filters.value.endDate) : true
    return searchMatch && statusMatch && startDateMatch && endDateMatch
  })
})

// Helper functions
const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')
const formatCurrency = (value: number) => new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value)

const statusClasses = (status: string) => {
  switch (status) {
    case 'Pending': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'Confirmed': return 'bg-sky-100 text-sky-800 dark:bg-sky-900 dark:text-sky-300'
    case 'Processing': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'Shipped': return 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300'
    case 'Completed': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'Cancelled': return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
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
