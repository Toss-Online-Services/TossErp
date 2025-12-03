<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useAccountingStore } from '~/stores/accounting'
import BarChart from '~/components/charts/BarChart.vue'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Financial Reports - TOSS'
})

const accountingStore = useAccountingStore()
const selectedReport = ref<'profit-loss' | 'balance-sheet' | 'cash-flow'>('profit-loss')
const dateRange = ref<'month' | 'quarter' | 'year' | 'custom'>('month')
const startDate = ref(new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0])
const endDate = ref(new Date().toISOString().split('T')[0])

// Computed
const profitLossData = computed(() => {
  const income = accountingStore.totalIncome
  const expenses = accountingStore.totalExpenses
  const netIncome = accountingStore.netIncome
  
  return {
    income: {
      label: 'Income',
      value: income,
      accounts: accountingStore.accountsByType.income
    },
    expenses: {
      label: 'Expenses',
      value: expenses,
      accounts: accountingStore.accountsByType.expense
    },
    netIncome: {
      label: 'Net Income',
      value: netIncome,
      isNet: true
    }
  }
})

const balanceSheetData = computed(() => {
  return {
    assets: {
      label: 'Assets',
      value: accountingStore.totalAssets,
      accounts: accountingStore.accountsByType.asset
    },
    liabilities: {
      label: 'Liabilities',
      value: accountingStore.totalLiabilities,
      accounts: accountingStore.accountsByType.liability
    },
    equity: {
      label: 'Equity',
      value: accountingStore.totalEquity,
      accounts: accountingStore.accountsByType.equity
    },
    total: accountingStore.totalAssets,
    totalLiabilitiesAndEquity: accountingStore.totalLiabilities + accountingStore.totalEquity
  }
})

const chartLabels = computed(() => {
  if (selectedReport.value === 'profit-loss') {
    return ['Income', 'Expenses', 'Net Income']
  }
  return []
})

const chartData = computed(() => {
  if (selectedReport.value === 'profit-loss') {
    return [
      profitLossData.value.income.value,
      profitLossData.value.expenses.value,
      profitLossData.value.netIncome.value
    ]
  }
  return []
})

// Methods
onMounted(async () => {
  await accountingStore.fetchAccounts()
  await accountingStore.fetchJournalEntries()
})

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function handlePrint() {
  window.print()
}

function handleExport() {
  alert('Export functionality coming soon')
}
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">Financial Reports</h3>
          <p class="text-gray-600 text-sm">View profit & loss, balance sheet, and cash flow reports</p>
        </div>
        <div class="flex items-center gap-3">
          <button
            @click="handlePrint"
            class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
          >
            <i class="material-symbols-rounded text-lg">print</i>
            <span>Print</span>
          </button>
          <button
            @click="handleExport"
            class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
          >
            <i class="material-symbols-rounded text-lg">download</i>
            <span>Export</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Report Selector -->
    <div class="bg-white rounded-xl shadow-card p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 mb-2">Report Type</label>
          <div class="grid grid-cols-3 gap-2">
            <button
              @click="selectedReport = 'profit-loss'"
              :class="[
                'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
                selectedReport === 'profit-loss'
                  ? 'bg-gray-900 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              Profit & Loss
            </button>
            <button
              @click="selectedReport = 'balance-sheet'"
              :class="[
                'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
                selectedReport === 'balance-sheet'
                  ? 'bg-gray-900 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              Balance Sheet
            </button>
            <button
              @click="selectedReport = 'cash-flow'"
              :class="[
                'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
                selectedReport === 'cash-flow'
                  ? 'bg-gray-900 text-white'
                  : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
              ]"
            >
              Cash Flow
            </button>
          </div>
        </div>
        <div class="md:w-48">
          <label class="block text-sm font-medium text-gray-700 mb-2">Date Range</label>
          <select
            v-model="dateRange"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="month">This Month</option>
            <option value="quarter">This Quarter</option>
            <option value="year">This Year</option>
            <option value="custom">Custom</option>
          </select>
        </div>
        <div v-if="dateRange === 'custom'" class="grid grid-cols-2 gap-2 md:w-96">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Start Date</label>
            <input
              v-model="startDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">End Date</label>
            <input
              v-model="endDate"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Profit & Loss Report -->
    <div v-if="selectedReport === 'profit-loss'" class="space-y-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <h4 class="text-xl font-bold text-gray-900 mb-6">Profit & Loss Statement</h4>
        
        <!-- Income Section -->
        <div class="mb-6">
          <h5 class="text-lg font-semibold text-gray-900 mb-4">Income</h5>
          <div class="space-y-2">
            <div
              v-for="account in profitLossData.income.accounts"
              :key="account.id"
              class="flex items-center justify-between py-2 px-4 bg-gray-50 rounded-lg"
            >
              <div>
                <p class="text-sm font-medium text-gray-900">{{ account.name }}</p>
                <p class="text-xs text-gray-500">{{ account.code }}</p>
              </div>
              <p class="text-sm font-semibold text-gray-900">{{ formatCurrency(account.balance) }}</p>
            </div>
            <div class="flex items-center justify-between py-3 px-4 bg-green-50 border-t-2 border-green-200 rounded-lg mt-2">
              <p class="text-base font-bold text-gray-900">Total Income</p>
              <p class="text-base font-bold text-green-600">{{ formatCurrency(profitLossData.income.value) }}</p>
            </div>
          </div>
        </div>

        <!-- Expenses Section -->
        <div class="mb-6">
          <h5 class="text-lg font-semibold text-gray-900 mb-4">Expenses</h5>
          <div class="space-y-2">
            <div
              v-for="account in profitLossData.expenses.accounts"
              :key="account.id"
              class="flex items-center justify-between py-2 px-4 bg-gray-50 rounded-lg"
            >
              <div>
                <p class="text-sm font-medium text-gray-900">{{ account.name }}</p>
                <p class="text-xs text-gray-500">{{ account.code }}</p>
              </div>
              <p class="text-sm font-semibold text-gray-900">{{ formatCurrency(account.balance) }}</p>
            </div>
            <div class="flex items-center justify-between py-3 px-4 bg-red-50 border-t-2 border-red-200 rounded-lg mt-2">
              <p class="text-base font-bold text-gray-900">Total Expenses</p>
              <p class="text-base font-bold text-red-600">{{ formatCurrency(profitLossData.expenses.value) }}</p>
            </div>
          </div>
        </div>

        <!-- Net Income -->
        <div class="flex items-center justify-between py-4 px-6 bg-gray-900 rounded-lg">
          <p class="text-lg font-bold text-white">Net Income</p>
          <p
            :class="[
              'text-2xl font-bold',
              profitLossData.netIncome.value >= 0 ? 'text-green-400' : 'text-red-400'
            ]"
          >
            {{ formatCurrency(profitLossData.netIncome.value) }}
          </p>
        </div>
      </div>

      <!-- Chart -->
      <div v-if="chartLabels.length > 0" class="bg-white rounded-xl shadow-card p-6">
        <h4 class="text-xl font-bold text-gray-900 mb-4">Visualization</h4>
        <ClientOnly>
          <BarChart :labels="chartLabels" :data="chartData" />
        </ClientOnly>
      </div>
    </div>

    <!-- Balance Sheet Report -->
    <div v-if="selectedReport === 'balance-sheet'" class="space-y-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <h4 class="text-xl font-bold text-gray-900 mb-6">Balance Sheet</h4>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Assets -->
          <div>
            <h5 class="text-lg font-semibold text-gray-900 mb-4">Assets</h5>
            <div class="space-y-2">
              <div
                v-for="account in balanceSheetData.assets.accounts"
                :key="account.id"
                class="flex items-center justify-between py-2 px-4 bg-gray-50 rounded-lg"
              >
                <div>
                  <p class="text-sm font-medium text-gray-900">{{ account.name }}</p>
                  <p class="text-xs text-gray-500">{{ account.code }}</p>
                </div>
                <p class="text-sm font-semibold text-gray-900">{{ formatCurrency(account.balance) }}</p>
              </div>
              <div class="flex items-center justify-between py-3 px-4 bg-blue-50 border-t-2 border-blue-200 rounded-lg mt-2">
                <p class="text-base font-bold text-gray-900">Total Assets</p>
                <p class="text-base font-bold text-blue-600">{{ formatCurrency(balanceSheetData.assets.value) }}</p>
              </div>
            </div>
          </div>

          <!-- Liabilities & Equity -->
          <div>
            <h5 class="text-lg font-semibold text-gray-900 mb-4">Liabilities & Equity</h5>
            
            <!-- Liabilities -->
            <div class="mb-4">
              <h6 class="text-sm font-medium text-gray-700 mb-2">Liabilities</h6>
              <div class="space-y-2">
                <div
                  v-for="account in balanceSheetData.liabilities.accounts"
                  :key="account.id"
                  class="flex items-center justify-between py-2 px-4 bg-gray-50 rounded-lg"
                >
                  <div>
                    <p class="text-sm font-medium text-gray-900">{{ account.name }}</p>
                    <p class="text-xs text-gray-500">{{ account.code }}</p>
                  </div>
                  <p class="text-sm font-semibold text-gray-900">{{ formatCurrency(account.balance) }}</p>
                </div>
                <div class="flex items-center justify-between py-2 px-4 bg-red-50 border-t border-red-200 rounded-lg mt-2">
                  <p class="text-sm font-bold text-gray-900">Total Liabilities</p>
                  <p class="text-sm font-bold text-red-600">{{ formatCurrency(balanceSheetData.liabilities.value) }}</p>
                </div>
              </div>
            </div>

            <!-- Equity -->
            <div>
              <h6 class="text-sm font-medium text-gray-700 mb-2">Equity</h6>
              <div class="space-y-2">
                <div
                  v-for="account in balanceSheetData.equity.accounts"
                  :key="account.id"
                  class="flex items-center justify-between py-2 px-4 bg-gray-50 rounded-lg"
                >
                  <div>
                    <p class="text-sm font-medium text-gray-900">{{ account.name }}</p>
                    <p class="text-xs text-gray-500">{{ account.code }}</p>
                  </div>
                  <p class="text-sm font-semibold text-gray-900">{{ formatCurrency(account.balance) }}</p>
                </div>
                <div class="flex items-center justify-between py-2 px-4 bg-green-50 border-t border-green-200 rounded-lg mt-2">
                  <p class="text-sm font-bold text-gray-900">Total Equity</p>
                  <p class="text-sm font-bold text-green-600">{{ formatCurrency(balanceSheetData.equity.value) }}</p>
                </div>
              </div>
            </div>

            <!-- Total Liabilities & Equity -->
            <div class="flex items-center justify-between py-3 px-4 bg-gray-900 rounded-lg mt-4">
              <p class="text-base font-bold text-white">Total Liabilities & Equity</p>
              <p class="text-base font-bold text-white">{{ formatCurrency(balanceSheetData.totalLiabilitiesAndEquity) }}</p>
            </div>
          </div>
        </div>

        <!-- Balance Check -->
        <div class="mt-6 p-4 rounded-lg" :class="balanceSheetData.total === balanceSheetData.totalLiabilitiesAndEquity ? 'bg-green-50 border-2 border-green-200' : 'bg-red-50 border-2 border-red-200'">
          <div class="flex items-center justify-between">
            <p class="text-sm font-medium text-gray-900">Balance Check</p>
            <p
              :class="[
                'text-sm font-bold',
                balanceSheetData.total === balanceSheetData.totalLiabilitiesAndEquity ? 'text-green-600' : 'text-red-600'
              ]"
            >
              {{ balanceSheetData.total === balanceSheetData.totalLiabilitiesAndEquity ? 'Balanced ✓' : 'Not Balanced ✗' }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Cash Flow Report -->
    <div v-if="selectedReport === 'cash-flow'" class="bg-white rounded-xl shadow-card p-6">
      <h4 class="text-xl font-bold text-gray-900 mb-6">Cash Flow Statement</h4>
      <div class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">account_balance_wallet</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Cash Flow Report</h3>
        <p class="text-gray-600">Cash flow statement functionality coming soon</p>
      </div>
    </div>
  </div>
</template>
