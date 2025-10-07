<template>
  <div>
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900">Finance Dashboard</h1>
      <p class="mt-2 text-sm text-gray-600">Monitor your financial health and cash flow</p>
    </div>

    <!-- Financial KPIs -->
    <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4 mb-8">
      <div class="bg-white overflow-hidden shadow-sm rounded-lg">
        <div class="p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0 bg-green-500 rounded-md p-3">
              <svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">Total Assets</dt>
                <dd class="text-2xl font-semibold text-gray-900">R{{ formatCurrency(totalAssets) }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow-sm rounded-lg">
        <div class="p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0 bg-red-500 rounded-md p-3">
              <svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 17h8m0 0V9m0 8l-8-8-4 4-6-6" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">Total Liabilities</dt>
                <dd class="text-2xl font-semibold text-gray-900">R{{ formatCurrency(totalLiabilities) }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow-sm rounded-lg">
        <div class="p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0 bg-blue-500 rounded-md p-3">
              <svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">Equity</dt>
                <dd class="text-2xl font-semibold text-gray-900">R{{ formatCurrency(totalEquity) }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>

      <div class="bg-white overflow-hidden shadow-sm rounded-lg">
        <div class="p-6">
          <div class="flex items-center">
            <div class="flex-shrink-0 bg-purple-500 rounded-md p-3">
              <svg class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 12l3-3 3 3 4-4M8 21l4-4 4 4M3 4h18M4 4h16v12a1 1 0 01-1 1H5a1 1 0 01-1-1V4z" />
              </svg>
            </div>
            <div class="ml-5 w-0 flex-1">
              <dl>
                <dt class="text-sm font-medium text-gray-500 truncate">Net Profit (MTD)</dt>
                <dd class="text-2xl font-semibold text-gray-900">R{{ formatCurrency(netProfit) }}</dd>
              </dl>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2 mb-8">
      <!-- Cash Flow -->
      <div class="bg-white shadow-sm rounded-lg p-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-4">Cash Flow (Last 30 Days)</h2>
        <div class="h-64 flex items-center justify-center border-2 border-dashed border-gray-300 rounded-lg">
          <p class="text-gray-500">Cash flow chart - integrate Chart.js</p>
        </div>
      </div>

      <!-- Profit & Loss Trend -->
      <div class="bg-white shadow-sm rounded-lg p-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-4">Profit & Loss Trend</h2>
        <div class="h-64 flex items-center justify-center border-2 border-dashed border-gray-300 rounded-lg">
          <p class="text-gray-500">P&L trend chart - integrate Chart.js</p>
        </div>
      </div>
    </div>

    <!-- Accounts Summary -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- Accounts Receivable -->
      <div class="bg-white shadow-sm rounded-lg p-6">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Accounts Receivable</h3>
        <div class="space-y-2">
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">Current (0-30 days)</span>
            <span class="text-sm font-medium">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">30-60 days</span>
            <span class="text-sm font-medium">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">60-90 days</span>
            <span class="text-sm font-medium text-yellow-600">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">90+ days</span>
            <span class="text-sm font-medium text-red-600">R0.00</span>
          </div>
          <div class="border-t pt-2 flex justify-between">
            <span class="text-sm font-semibold">Total</span>
            <span class="text-sm font-semibold">R0.00</span>
          </div>
        </div>
      </div>

      <!-- Accounts Payable -->
      <div class="bg-white shadow-sm rounded-lg p-6">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Accounts Payable</h3>
        <div class="space-y-2">
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">Current (0-30 days)</span>
            <span class="text-sm font-medium">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">30-60 days</span>
            <span class="text-sm font-medium">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">60-90 days</span>
            <span class="text-sm font-medium text-yellow-600">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">90+ days</span>
            <span class="text-sm font-medium text-red-600">R0.00</span>
          </div>
          <div class="border-t pt-2 flex justify-between">
            <span class="text-sm font-semibold">Total</span>
            <span class="text-sm font-semibold">R0.00</span>
          </div>
        </div>
      </div>

      <!-- Tax Liability -->
      <div class="bg-white shadow-sm rounded-lg p-6">
        <h3 class="text-lg font-semibold text-gray-900 mb-4">Tax Liability</h3>
        <div class="space-y-2">
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">VAT Collected</span>
            <span class="text-sm font-medium">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">VAT Paid</span>
            <span class="text-sm font-medium">R0.00</span>
          </div>
          <div class="flex justify-between">
            <span class="text-sm text-gray-500">PAYE</span>
            <span class="text-sm font-medium">R0.00</span>
          </div>
          <div class="border-t pt-2 flex justify-between">
            <span class="text-sm font-semibold">Total Due</span>
            <span class="text-sm font-semibold">R0.00</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'dashboard',
  middleware: ['auth'],
})

const totalAssets = ref(0)
const totalLiabilities = ref(0)
const totalEquity = ref(0)
const netProfit = ref(0)

const formatCurrency = (amount: number): string => {
  return (amount / 100).toFixed(2)
}
</script>

