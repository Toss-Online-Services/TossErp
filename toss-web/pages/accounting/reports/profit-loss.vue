<template>
  <div class="space-y-6">
    <PageHeader
      title="Profit & Loss Statement"
      description="Income Statement for the selected period"
    />

    <!-- Date Range Selection and Export -->
    <div class="flex flex-wrap gap-4 items-center justify-between">
      <div class="flex gap-4 items-center">
        <div>
          <label class="block text-sm font-medium mb-1">Start Date</label>
          <input
            v-model="startDate"
            type="date"
            class="px-4 py-2 border rounded-lg"
          />
        </div>
        <div>
          <label class="block text-sm font-medium mb-1">End Date</label>
          <input
            v-model="endDate"
            type="date"
            class="px-4 py-2 border rounded-lg"
          />
        </div>
      </div>

      <div class="flex gap-2">
        <ExportButton
          :data="profitLoss"
          filename="profit-loss"
          :formats="['pdf', 'excel', 'csv']"
        />
        <button
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          @click="loadProfitLoss"
        >
          Generate Report
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      <p class="mt-4 text-gray-600">Generating profit & loss statement...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-red-800">{{ error }}</p>
    </div>

    <!-- P&L Content -->
    <div v-else-if="profitLoss" class="space-y-6">
      <!-- Summary Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Total Revenue</p>
          <p class="text-2xl font-bold text-green-600">
            {{ formatCurrency(profitLoss.revenue.totalRevenue) }}
          </p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Total Expenses</p>
          <p class="text-2xl font-bold text-red-600">
            {{ formatCurrency(profitLoss.expenses.totalExpenses) }}
          </p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Net Profit</p>
          <p class="text-2xl font-bold" :class="profitLoss.netProfit >= 0 ? 'text-green-600' : 'text-red-600'">
            {{ formatCurrency(profitLoss.netProfit) }}
          </p>
        </div>
        <div class="bg-white rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 mb-1">Profit Margin</p>
          <p class="text-2xl font-bold">
            {{ profitLoss.profitMargin.toFixed(2) }}%
          </p>
        </div>
      </div>

      <!-- Detailed P&L -->
      <div class="bg-white rounded-lg shadow p-6">
        <!-- Revenue Section -->
        <div class="mb-8">
          <h2 class="text-xl font-bold mb-4">Revenue</h2>
          
          <!-- Operating Revenue -->
          <div class="mb-4">
            <h3 class="text-lg font-semibold mb-3 text-gray-700">Operating Revenue</h3>
            <table class="w-full">
              <tbody>
                <tr
                  v-for="account in profitLoss.revenue.operating"
                  :key="account.accountId"
                  class="border-b"
                >
                  <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                  <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Non-Operating Revenue -->
          <div v-if="profitLoss.revenue.nonOperating.length > 0" class="mb-4">
            <h3 class="text-lg font-semibold mb-3 text-gray-700">Non-Operating Revenue</h3>
            <table class="w-full">
              <tbody>
                <tr
                  v-for="account in profitLoss.revenue.nonOperating"
                  :key="account.accountId"
                  class="border-b"
                >
                  <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                  <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="border-t-2 pt-3 mt-3">
            <div class="flex justify-between font-bold text-lg">
              <span>Total Revenue</span>
              <span class="text-green-600">{{ formatCurrency(profitLoss.revenue.totalRevenue) }}</span>
            </div>
          </div>
        </div>

        <!-- Expenses Section -->
        <div class="mb-8">
          <h2 class="text-xl font-bold mb-4">Expenses</h2>
          
          <!-- Operating Expenses -->
          <div class="mb-4">
            <h3 class="text-lg font-semibold mb-3 text-gray-700">Operating Expenses</h3>
            <table class="w-full">
              <tbody>
                <tr
                  v-for="account in profitLoss.expenses.operating"
                  :key="account.accountId"
                  class="border-b"
                >
                  <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                  <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Non-Operating Expenses -->
          <div v-if="profitLoss.expenses.nonOperating.length > 0" class="mb-4">
            <h3 class="text-lg font-semibold mb-3 text-gray-700">Non-Operating Expenses</h3>
            <table class="w-full">
              <tbody>
                <tr
                  v-for="account in profitLoss.expenses.nonOperating"
                  :key="account.accountId"
                  class="border-b"
                >
                  <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                  <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="border-t-2 pt-3 mt-3">
            <div class="flex justify-between font-bold text-lg">
              <span>Total Expenses</span>
              <span class="text-red-600">{{ formatCurrency(profitLoss.expenses.totalExpenses) }}</span>
            </div>
          </div>
        </div>

        <!-- Net Profit -->
        <div class="border-t-4 border-gray-300 pt-6">
          <div class="space-y-3">
            <div class="flex justify-between text-lg">
              <span class="font-semibold">Gross Profit</span>
              <span :class="profitLoss.grossProfit >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ formatCurrency(profitLoss.grossProfit) }}
              </span>
            </div>
            <div class="flex justify-between text-lg">
              <span class="font-semibold">Operating Profit</span>
              <span :class="profitLoss.operatingProfit >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ formatCurrency(profitLoss.operatingProfit) }}
              </span>
            </div>
            <div class="flex justify-between text-2xl font-bold">
              <span>Net Profit</span>
              <span :class="profitLoss.netProfit >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ formatCurrency(profitLoss.netProfit) }}
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
import type { ProfitAndLoss } from '~/types/accounting'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Profit & Loss - TOSS ERP',
})

const { getProfitAndLoss, isLoading, error } = useFinancialReports()

const startDate = ref(new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0])
const endDate = ref(new Date().toISOString().split('T')[0])
const profitLoss = ref<ProfitAndLoss | null>(null)

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const loadProfitLoss = async () => {
  const start = new Date(startDate.value)
  const end = new Date(endDate.value)
  profitLoss.value = await getProfitAndLoss(start, end)
}

onMounted(() => {
  loadProfitLoss()
})
</script>

