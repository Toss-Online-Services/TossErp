<template>
  <div class="space-y-6">
    <PageHeader
      title="Cash Flow Statement"
      description="Track cash inflows and outflows from operating, investing, and financing activities"
    />

    <!-- Date Range Selection and Export -->
    <div class="flex flex-wrap gap-4 items-center justify-between">
      <div class="flex gap-4 items-center">
        <div>
          <label class="block text-sm font-medium mb-1 dark:text-gray-300">Start Date</label>
          <input
            v-model="startDate"
            type="date"
            class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white dark:border-gray-600"
          />
        </div>
        <div>
          <label class="block text-sm font-medium mb-1 dark:text-gray-300">End Date</label>
          <input
            v-model="endDate"
            type="date"
            class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white dark:border-gray-600"
          />
        </div>
      </div>

      <div class="flex gap-2">
        <button
          @click="exportReport('csv')"
          class="px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700"
        >
          Export CSV
        </button>
        <button
          @click="exportReport('pdf')"
          class="px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700"
        >
          Export PDF
        </button>
        <button
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          @click="loadCashFlow"
        >
          Generate Report
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      <p class="mt-4 text-gray-600 dark:text-gray-400">Generating cash flow statement...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
      <p class="text-red-800 dark:text-red-200">{{ error }}</p>
    </div>

    <!-- Cash Flow Content -->
    <div v-else-if="cashFlow" class="space-y-6">
      <!-- Summary Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Operating Activities</p>
          <p class="text-2xl font-bold" :class="cashFlow.operating.netCash >= 0 ? 'text-green-600' : 'text-red-600'">
            {{ formatCurrency(cashFlow.operating.netCash) }}
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Investing Activities</p>
          <p class="text-2xl font-bold" :class="cashFlow.investing.netCash >= 0 ? 'text-green-600' : 'text-red-600'">
            {{ formatCurrency(cashFlow.investing.netCash) }}
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Financing Activities</p>
          <p class="text-2xl font-bold" :class="cashFlow.financing.netCash >= 0 ? 'text-green-600' : 'text-red-600'">
            {{ formatCurrency(cashFlow.financing.netCash) }}
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Net Cash Flow</p>
          <p class="text-2xl font-bold" :class="cashFlow.netCashFlow >= 0 ? 'text-green-600' : 'text-red-600'">
            {{ formatCurrency(cashFlow.netCashFlow) }}
          </p>
        </div>
      </div>

      <!-- Detailed Cash Flow -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <!-- Operating Activities -->
        <div class="mb-8">
          <h2 class="text-xl font-bold mb-4 dark:text-white">Operating Activities</h2>
          <table class="w-full">
            <tbody>
              <tr
                v-for="item in cashFlow.operating.items"
                :key="item.accountId"
                class="border-b dark:border-gray-700"
              >
                <td class="py-2 dark:text-gray-300">{{ item.accountName }}</td>
                <td class="text-right py-2" :class="item.balance >= 0 ? 'text-green-600' : 'text-red-600'">
                  {{ formatCurrency(item.balance) }}
                </td>
              </tr>
              <tr class="font-semibold border-t-2 dark:border-gray-600">
                <td class="py-3 dark:text-white">Net Cash from Operating Activities</td>
                <td class="text-right py-3" :class="cashFlow.operating.netCash >= 0 ? 'text-green-600' : 'text-red-600'">
                  {{ formatCurrency(cashFlow.operating.netCash) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Investing Activities -->
        <div class="mb-8">
          <h2 class="text-xl font-bold mb-4 dark:text-white">Investing Activities</h2>
          <table class="w-full">
            <tbody>
              <tr
                v-for="item in cashFlow.investing.items"
                :key="item.accountId"
                class="border-b dark:border-gray-700"
              >
                <td class="py-2 dark:text-gray-300">{{ item.accountName }}</td>
                <td class="text-right py-2" :class="item.balance >= 0 ? 'text-green-600' : 'text-red-600'">
                  {{ formatCurrency(item.balance) }}
                </td>
              </tr>
              <tr class="font-semibold border-t-2 dark:border-gray-600">
                <td class="py-3 dark:text-white">Net Cash from Investing Activities</td>
                <td class="text-right py-3" :class="cashFlow.investing.netCash >= 0 ? 'text-green-600' : 'text-red-600'">
                  {{ formatCurrency(cashFlow.investing.netCash) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Financing Activities -->
        <div class="mb-8">
          <h2 class="text-xl font-bold mb-4 dark:text-white">Financing Activities</h2>
          <table class="w-full">
            <tbody>
              <tr
                v-for="item in cashFlow.financing.items"
                :key="item.accountId"
                class="border-b dark:border-gray-700"
              >
                <td class="py-2 dark:text-gray-300">{{ item.accountName }}</td>
                <td class="text-right py-2" :class="item.balance >= 0 ? 'text-green-600' : 'text-red-600'">
                  {{ formatCurrency(item.balance) }}
                </td>
              </tr>
              <tr class="font-semibold border-t-2 dark:border-gray-600">
                <td class="py-3 dark:text-white">Net Cash from Financing Activities</td>
                <td class="text-right py-3" :class="cashFlow.financing.netCash >= 0 ? 'text-green-600' : 'text-red-600'">
                  {{ formatCurrency(cashFlow.financing.netCash) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Net Change in Cash -->
        <div class="border-t-4 border-gray-300 dark:border-gray-600 pt-6">
          <div class="space-y-3">
            <div class="flex justify-between text-lg dark:text-gray-300">
              <span>Opening Cash Balance</span>
              <span>{{ formatCurrency(cashFlow.openingBalance) }}</span>
            </div>
            <div class="flex justify-between text-lg font-semibold dark:text-white">
              <span>Net Increase/(Decrease) in Cash</span>
              <span :class="cashFlow.netCashFlow >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ formatCurrency(cashFlow.netCashFlow) }}
              </span>
            </div>
            <div class="flex justify-between text-2xl font-bold dark:text-white">
              <span>Closing Cash Balance</span>
              <span :class="cashFlow.closingBalance >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ formatCurrency(cashFlow.closingBalance) }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useFinancialReports } from '~/composables/useFinancialReports'
import type { CashFlow } from '~/composables/useFinancialReports'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Cash Flow Statement - TOSS ERP',
})

const { getCashFlow, isLoading, error } = useFinancialReports()

const startDate = ref(new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0])
const endDate = ref(new Date().toISOString().split('T')[0])
const cashFlow = ref<CashFlow | null>(null)

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const loadCashFlow = async () => {
  const start = new Date(startDate.value)
  const end = new Date(endDate.value)
  cashFlow.value = await getCashFlow(start, end)
}

const exportReport = async (format: 'csv' | 'pdf') => {
  if (!cashFlow.value) return

  if (format === 'csv') {
    const csvData = [
      ['Cash Flow Statement'],
      [`Period: ${startDate.value} to ${endDate.value}`],
      [''],
      ['Operating Activities', ''],
      ...cashFlow.value.operating.items.map(item => [item.accountName, item.balance]),
      ['Net Cash from Operating', cashFlow.value.operating.netCash],
      [''],
      ['Investing Activities', ''],
      ...cashFlow.value.investing.items.map(item => [item.accountName, item.balance]),
      ['Net Cash from Investing', cashFlow.value.investing.netCash],
      [''],
      ['Financing Activities', ''],
      ...cashFlow.value.financing.items.map(item => [item.accountName, item.balance]),
      ['Net Cash from Financing', cashFlow.value.financing.netCash],
      [''],
      ['Net Cash Flow', cashFlow.value.netCashFlow],
      ['Opening Balance', cashFlow.value.openingBalance],
      ['Closing Balance', cashFlow.value.closingBalance],
    ]

    const csv = csvData.map(row => row.join(',')).join('\n')
    const blob = new Blob([csv], { type: 'text/csv' })
    const url = URL.createObjectURL(blob)
    const a = document.createElement('a')
    a.href = url
    a.download = `cash-flow-${startDate.value}-to-${endDate.value}.csv`
    a.click()
    URL.revokeObjectURL(url)
  } else {
    alert('PDF export functionality coming soon!')
  }
}

onMounted(() => {
  loadCashFlow()
})
</script>

