<template>
  <div class="space-y-6">
    <PageHeader
      title="Financial Statements"
      description="View and generate comprehensive financial statements"
    />

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- Balance Sheet Card -->
      <NuxtLink to="/accounting/reports/balance-sheet" class="bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
        <div class="flex items-center justify-between mb-4">
          <Icon name="mdi:scale-balance" class="w-12 h-12" />
          <Icon name="mdi:chevron-right" class="w-6 h-6" />
        </div>
        <h3 class="text-xl font-semibold mb-2">Balance Sheet</h3>
        <p class="text-blue-100">View assets, liabilities, and equity</p>
      </NuxtLink>

      <!-- Profit & Loss Card -->
      <NuxtLink to="/accounting/reports/profit-loss" class="bg-gradient-to-br from-green-500 to-green-600 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
        <div class="flex items-center justify-between mb-4">
          <Icon name="mdi:chart-line" class="w-12 h-12" />
          <Icon name="mdi:chevron-right" class="w-6 h-6" />
        </div>
        <h3 class="text-xl font-semibold mb-2">Profit & Loss</h3>
        <p class="text-green-100">Income statement and profitability</p>
      </NuxtLink>

      <!-- Cash Flow Card -->
      <div class="bg-gradient-to-br from-purple-500 to-purple-600 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow cursor-pointer">
        <div class="flex items-center justify-between mb-4">
          <Icon name="mdi:cash-multiple" class="w-12 h-12" />
          <Icon name="mdi:chevron-right" class="w-6 h-6" />
        </div>
        <h3 class="text-xl font-semibold mb-2">Cash Flow</h3>
        <p class="text-purple-100">Track cash inflows and outflows</p>
      </div>

      <!-- VAT Report Card -->
      <NuxtLink to="/accounting/vat-report" class="bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
        <div class="flex items-center justify-between mb-4">
          <Icon name="mdi:file-document-outline" class="w-12 h-12" />
          <Icon name="mdi:chevron-right" class="w-6 h-6" />
        </div>
        <h3 class="text-xl font-semibold mb-2">VAT Report</h3>
        <p class="text-yellow-100">South African VAT compliance</p>
      </NuxtLink>

      <!-- Trial Balance Card -->
      <div class="bg-gradient-to-br from-red-500 to-red-600 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow cursor-pointer">
        <div class="flex items-center justify-between mb-4">
          <Icon name="mdi:format-list-bulleted" class="w-12 h-12" />
          <Icon name="mdi:chevron-right" class="w-6 h-6" />
        </div>
        <h3 class="text-xl font-semibold mb-2">Trial Balance</h3>
        <p class="text-red-100">Verify ledger accuracy</p>
      </div>

      <!-- General Ledger Card -->
      <div class="bg-gradient-to-br from-indigo-500 to-indigo-600 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow cursor-pointer">
        <div class="flex items-center justify-between mb-4">
          <Icon name="mdi:book-open-variant" class="w-12 h-12" />
          <Icon name="mdi:chevron-right" class="w-6 h-6" />
        </div>
        <h3 class="text-xl font-semibold mb-2">General Ledger</h3>
        <p class="text-indigo-100">Complete transaction history</p>
      </div>
    </div>

    <!-- Quick Stats -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
      <h3 class="text-lg font-semibold mb-4 dark:text-white">Financial Overview</h3>
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="text-center">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Assets</p>
          <p class="text-2xl font-bold text-blue-600 dark:text-blue-400">R 925,000</p>
          <p class="text-xs text-green-600">+12.5% from last month</p>
        </div>
        <div class="text-center">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Liabilities</p>
          <p class="text-2xl font-bold text-red-600 dark:text-red-400">R 340,000</p>
          <p class="text-xs text-red-600">+3.2% from last month</p>
        </div>
        <div class="text-center">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Net Equity</p>
          <p class="text-2xl font-bold text-purple-600 dark:text-purple-400">R 585,000</p>
          <p class="text-xs text-green-600">+18.7% from last month</p>
        </div>
        <div class="text-center">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Net Profit (YTD)</p>
          <p class="text-2xl font-bold text-green-600 dark:text-green-400">R 185,500</p>
          <p class="text-xs text-green-600">+25.3% from last year</p>
        </div>
      </div>
    </div>

    <!-- Recent Reports -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-semibold dark:text-white">Recent Reports</h3>
      </div>
      <div class="divide-y divide-gray-200 dark:divide-gray-700">
        <div v-for="report in recentReports" :key="report.id" class="px-6 py-4 hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer">
          <div class="flex justify-between items-center">
            <div>
              <p class="font-medium dark:text-white">{{ report.name }}</p>
              <p class="text-sm text-gray-500 dark:text-gray-400">Generated {{ report.date }}</p>
            </div>
            <div class="flex gap-2">
              <button class="px-3 py-1 text-sm bg-blue-100 text-blue-800 rounded hover:bg-blue-200">
                Download
              </button>
              <button class="px-3 py-1 text-sm border rounded hover:bg-gray-100 dark:hover:bg-gray-600 dark:text-white">
                View
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Financial Statements - TOSS ERP',
})

const recentReports = ref([
  { id: 1, name: 'Balance Sheet - February 2024', date: '2024-02-29' },
  { id: 2, name: 'Profit & Loss Statement - Q1 2024', date: '2024-01-31' },
  { id: 3, name: 'VAT Return - January 2024', date: '2024-01-15' },
  { id: 4, name: 'Cash Flow Statement - December 2023', date: '2023-12-31' },
])
</script>
