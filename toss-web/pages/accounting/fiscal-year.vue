<template>
  <div class="space-y-6">
    <PageHeader
      title="Fiscal Year Management"
      description="Configure fiscal years and accounting periods"
    />

    <div class="flex justify-end">
      <button
        @click="showModal = true"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Create Fiscal Year
      </button>
    </div>

    <div class="grid grid-cols-1 gap-4">
      <div v-for="year in fiscalYears" :key="year.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white">Fiscal Year {{ year.year }}</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">{{ year.startDate }} to {{ year.endDate }}</p>
          </div>
          <div class="flex items-center gap-2">
            <span :class="year.isCurrent ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200'" class="px-3 py-1 text-sm rounded-full">
              {{ year.isCurrent ? 'Current' : year.isClosed ? 'Closed' : 'Future' }}
            </span>
          </div>
        </div>

        <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-4">
          <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded">
            <p class="text-sm text-gray-600 dark:text-gray-400">Periods</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">{{ year.periods }}</p>
          </div>
          <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded">
            <p class="text-sm text-gray-600 dark:text-gray-400">Open Periods</p>
            <p class="text-lg font-semibold text-green-600 dark:text-green-400">{{ year.openPeriods }}</p>
          </div>
          <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded">
            <p class="text-sm text-gray-600 dark:text-gray-400">Transactions</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">{{ year.transactionCount.toLocaleString() }}</p>
          </div>
          <div class="text-center p-3 bg-gray-50 dark:bg-gray-700 rounded">
            <p class="text-sm text-gray-600 dark:text-gray-400">Revenue</p>
            <p class="text-lg font-semibold text-blue-600 dark:text-blue-400">R {{ year.totalRevenue.toLocaleString() }}</p>
          </div>
        </div>

        <div class="flex gap-2">
          <button class="px-4 py-2 text-sm bg-blue-50 dark:bg-blue-900 text-blue-600 dark:text-blue-300 rounded hover:bg-blue-100 dark:hover:bg-blue-800">
            View Periods
          </button>
          <button class="px-4 py-2 text-sm bg-gray-50 dark:bg-gray-700 text-gray-600 dark:text-gray-300 rounded hover:bg-gray-100 dark:hover:bg-gray-600">
            Reports
          </button>
          <button v-if="!year.isClosed && !year.isCurrent" class="px-4 py-2 text-sm bg-gray-50 dark:bg-gray-700 text-gray-600 dark:text-gray-300 rounded hover:bg-gray-100 dark:hover:bg-gray-600">
            Edit
          </button>
          <button v-if="year.isCurrent && year.canClose" class="ml-auto px-4 py-2 text-sm bg-red-50 dark:bg-red-900 text-red-600 dark:text-red-300 rounded hover:bg-red-100 dark:hover:bg-red-800">
            Close Year
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Fiscal Year - TOSS ERP',
})

const showModal = ref(false)

const fiscalYears = ref([
  {
    id: 1,
    year: '2024',
    startDate: '2024-01-01',
    endDate: '2024-12-31',
    isCurrent: true,
    isClosed: false,
    periods: 12,
    openPeriods: 10,
    transactionCount: 4523,
    totalRevenue: 2450000,
    canClose: false
  },
  {
    id: 2,
    year: '2023',
    startDate: '2023-01-01',
    endDate: '2023-12-31',
    isCurrent: false,
    isClosed: true,
    periods: 12,
    openPeriods: 0,
    transactionCount: 8934,
    totalRevenue: 1980000,
    canClose: false
  },
  {
    id: 3,
    year: '2025',
    startDate: '2025-01-01',
    endDate: '2025-12-31',
    isCurrent: false,
    isClosed: false,
    periods: 12,
    openPeriods: 12,
    transactionCount: 0,
    totalRevenue: 0,
    canClose: false
  }
])
</script>
