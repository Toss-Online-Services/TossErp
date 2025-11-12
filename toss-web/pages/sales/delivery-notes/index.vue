<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.deliveryNotes.list.title') }}
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('sales.deliveryNotes.list.description') }}
        </p>
      </div>
      <div class="flex items-center gap-2">
        <NuxtLink to="/sales/delivery-notes/create" class="btn btn-primary">
          <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
          {{ $t('sales.deliveryNotes.list.newDeliveryNote') }}
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
          :options="['All', 'Scheduled', 'In-Transit', 'Delivered', 'Failed']"
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

    <!-- Delivery Notes Table -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="table-header">{{ $t('sales.deliveryNotes.list.deliveryNote') }} #</th>
            <th class="table-header">{{ $t('sales.deliveryNotes.list.customer') }}</th>
            <th class="table-header">{{ $t('sales.deliveryNotes.list.order') }} #</th>
            <th class="table-header">{{ $t('sales.deliveryNotes.list.deliveryDate') }}</th>
            <th class="table-header">{{ $t('sales.deliveryNotes.list.driver') }}</th>
            <th class="table-header text-center">{{ $t('common.status') }}</th>
            <th class="table-header text-right">{{ $t('common.actions') }}</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="dn in filteredDeliveryNotes" :key="dn.id">
            <td class="table-cell font-medium">{{ dn.id }}</td>
            <td class="table-cell">{{ dn.customerName }}</td>
            <td class="table-cell">{{ dn.orderId }}</td>
            <td class="table-cell">{{ formatDate(dn.deliveryDate) }}</td>
            <td class="table-cell">{{ dn.driver }}</td>
            <td class="table-cell text-center">
              <span :class="statusClasses(dn.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
                {{ dn.status }}
              </span>
            </td>
            <td class="table-cell text-right">
              <NuxtLink :to="`/sales/delivery-notes/${dn.id}`" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
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
  title: t('sales.deliveryNotes.list.pageTitle'),
})

// Mock Data
const stats = ref([
  { name: t('sales.deliveryNotes.list.scheduled'), value: 8, icon: 'heroicons:calendar-days', bgColor: 'bg-blue-100', iconColor: 'text-blue-600' },
  { name: t('sales.deliveryNotes.list.inTransit'), value: 3, icon: 'heroicons:truck', bgColor: 'bg-yellow-100', iconColor: 'text-yellow-600' },
  { name: t('sales.deliveryNotes.list.delivered'), value: 45, icon: 'heroicons:check-circle', bgColor: 'bg-green-100', iconColor: 'text-green-600' },
  { name: t('sales.deliveryNotes.list.failed'), value: 1, icon: 'heroicons:x-circle', bgColor: 'bg-red-100', iconColor: 'text-red-600' },
])

const deliveryNotes = ref([
  { id: 'DN-001', customerName: 'Jabu\'s Spaza', orderId: 'SO-123', deliveryDate: '2025-11-12', driver: 'Themba', status: 'Delivered' },
  { id: 'DN-002', customerName: 'Sipho\'s Tavern', orderId: 'SO-124', deliveryDate: '2025-11-13', driver: 'Lefa', status: 'In-Transit' },
  { id: 'DN-003', customerName: 'The Gogo Shop', orderId: 'SO-125', deliveryDate: '2025-11-14', driver: 'Themba', status: 'Scheduled' },
  { id: 'DN-004', customerName: 'Jabu\'s Spaza', orderId: 'SO-120', deliveryDate: '2025-11-10', driver: 'Lefa', status: 'Failed' },
])

const filters = ref({
  search: '',
  status: 'All',
  startDate: '',
  endDate: '',
})

const filteredDeliveryNotes = computed(() => {
  return deliveryNotes.value.filter(dn => {
    const searchMatch = filters.value.search ? dn.customerName.toLowerCase().includes(filters.value.search.toLowerCase()) || dn.id.toLowerCase().includes(filters.value.search.toLowerCase()) : true
    const statusMatch = filters.value.status !== 'All' ? dn.status === filters.value.status : true
    const startDateMatch = filters.value.startDate ? new Date(dn.deliveryDate) >= new Date(filters.value.startDate) : true
    const endDateMatch = filters.value.endDate ? new Date(dn.deliveryDate) <= new Date(filters.value.endDate) : true
    return searchMatch && statusMatch && startDateMatch && endDateMatch
  })
})

// Helper functions
const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')

const statusClasses = (status: string) => {
  switch (status) {
    case 'Scheduled': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'In-Transit': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'Delivered': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'Failed': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
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
