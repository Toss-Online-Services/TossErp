<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Accounts</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage your financial transactions and accounting</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showCreateAccountModal = true" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                New Account
              </button>
              <button @click="showCreateJournalModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <DocumentIcon class="w-5 h-5 inline mr-2" />
                Journal Entry
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Assets</p>
              <p class="text-2xl font-bold text-green-600">R {{ formatCurrency(stats.totalAssets) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <TrendingUpIcon class="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Liabilities</p>
              <p class="text-2xl font-bold text-red-600">R {{ formatCurrency(stats.totalLiabilities) }}</p>
            </div>
            <div class="p-3 bg-red-100 dark:bg-red-900 rounded-full">
              <TrendingDownIcon class="w-6 h-6 text-red-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Monthly Revenue</p>
              <p class="text-2xl font-bold text-blue-600">R {{ formatCurrency(stats.monthlyRevenue) }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <CurrencyDollarIcon class="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Net Profit</p>
              <p class="text-2xl font-bold text-green-600">R {{ formatCurrency(stats.netProfit) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <ChartBarIcon class="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <!-- Chart of Accounts -->
        <div class="lg:col-span-2">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Chart of Accounts</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="account in accounts" :key="account.id" class="flex items-center justify-between p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
                  <div class="flex items-center space-x-3">
                    <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                      <component :is="getAccountIcon(account.type)" class="w-5 h-5 text-blue-600" />
                    </div>
                    <div>
                      <p class="font-medium text-gray-900 dark:text-white">{{ account.name }}</p>
                      <p class="text-sm text-gray-600 dark:text-gray-400">{{ account.type }} • {{ account.number }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="font-semibold text-gray-900 dark:text-white">R {{ formatCurrency(account.balance) }}</p>
                    <p class="text-sm" :class="account.balance >= 0 ? 'text-green-600' : 'text-red-600'">
                      {{ account.balance >= 0 ? '+' : '' }}{{ account.change }}%
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Transactions -->
        <div>
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Transactions</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="transaction in recentTransactions" :key="transaction.id" class="flex items-center justify-between">
                  <div class="flex items-center space-x-3">
                    <div class="w-8 h-8 bg-gray-100 dark:bg-gray-700 rounded-full flex items-center justify-center">
                      <component :is="getTransactionIcon(transaction.type)" class="w-4 h-4 text-gray-600" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ transaction.description }}</p>
                      <p class="text-xs text-gray-600 dark:text-gray-400">{{ formatDate(transaction.date) }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-semibold" :class="transaction.amount >= 0 ? 'text-green-600' : 'text-red-600'">
                      {{ transaction.amount >= 0 ? '+' : '-' }}R {{ formatCurrency(Math.abs(transaction.amount)) }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Financial Reports -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Profit & Loss -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="p-6 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Profit & Loss</h3>
              <select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                <option>This Month</option>
                <option>Last Month</option>
                <option>This Quarter</option>
                <option>This Year</option>
              </select>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Revenue</span>
                <span class="font-semibold text-green-600">R {{ formatCurrency(profitLoss.revenue) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Cost of Goods Sold</span>
                <span class="font-semibold text-red-600">R {{ formatCurrency(profitLoss.cogs) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Operating Expenses</span>
                <span class="font-semibold text-red-600">R {{ formatCurrency(profitLoss.expenses) }}</span>
              </div>
              <hr class="border-gray-200 dark:border-gray-700">
              <div class="flex justify-between">
                <span class="font-semibold text-gray-900 dark:text-white">Net Profit</span>
                <span class="font-bold text-green-600">R {{ formatCurrency(profitLoss.netProfit) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Balance Sheet Summary -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="p-6 border-b border-gray-200 dark:border-gray-700">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Balance Sheet Summary</h3>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div>
                <h4 class="font-medium text-gray-900 dark:text-white mb-2">Assets</h4>
                <div class="space-y-2 ml-4">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Current Assets</span>
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatCurrency(balanceSheet.currentAssets) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Fixed Assets</span>
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatCurrency(balanceSheet.fixedAssets) }}</span>
                  </div>
                </div>
              </div>
              <div>
                <h4 class="font-medium text-gray-900 dark:text-white mb-2">Liabilities</h4>
                <div class="space-y-2 ml-4">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Current Liabilities</span>
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatCurrency(balanceSheet.currentLiabilities) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Long-term Debt</span>
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatCurrency(balanceSheet.longTermDebt) }}</span>
                  </div>
                </div>
              </div>
              <hr class="border-gray-200 dark:border-gray-700">
              <div class="flex justify-between">
                <span class="font-semibold text-gray-900 dark:text-white">Equity</span>
                <span class="font-bold text-blue-600">R {{ formatCurrency(balanceSheet.equity) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// Icons (would normally import from a proper icon library)
const PlusIcon = 'svg'
const DocumentIcon = 'svg'
const TrendingUpIcon = 'svg'
const TrendingDownIcon = 'svg'
const CurrencyDollarIcon = 'svg'
const ChartBarIcon = 'svg'

// Sample data
const stats = ref({
  totalAssets: 116300,  // Cash + Bank + Credit + Stock + Equipment
  totalLiabilities: 26700,  // Supplier credits + Business loan
  monthlyRevenue: 133500,  // Daily sales + Community events
  netProfit: 18500
})

const accounts = ref([
  { id: 1, name: 'Spaza Shop Cash Till', type: 'Asset', number: '1001', balance: 8500, change: 12.3 },
  { id: 2, name: 'FNB Business Account', type: 'Asset', number: '1002', balance: 35000, change: 8.7 },
  { id: 3, name: 'Customer Store Credit', type: 'Asset', number: '1200', balance: 2800, change: -1.5 },
  { id: 4, name: 'Stock on Hand', type: 'Asset', number: '1300', balance: 45000, change: 15.2 },
  { id: 5, name: 'Shop Equipment', type: 'Asset', number: '1400', balance: 25000, change: 0 },
  { id: 6, name: 'Supplier Credit (Shoprite)', type: 'Liability', number: '2001', balance: -8500, change: 2.1 },
  { id: 7, name: 'Coca-Cola Credit Account', type: 'Liability', number: '2002', balance: -3200, change: -5.8 },
  { id: 8, name: 'Township Business Loan', type: 'Liability', number: '2100', balance: -15000, change: -8.3 },
  { id: 9, name: 'Daily Sales Revenue', type: 'Income', number: '4001', balance: 125000, change: 18.6 },
  { id: 10, name: 'Community Event Sales', type: 'Income', number: '4002', balance: 8500, change: 25.4 }
])

const recentTransactions = ref([
  { id: 1, description: 'Nomsa Community Kitchen - Grocery supplies', type: 'income', amount: 450, date: new Date() },
  { id: 2, description: 'Shoprite Wholesale - Stock replenishment', type: 'expense', amount: -2800, date: new Date(Date.now() - 3600000) },
  { id: 3, description: 'Coca-Cola delivery payment', type: 'expense', amount: -890, date: new Date(Date.now() - 7200000) },
  { id: 4, description: 'Cash sales - Friday afternoon', type: 'income', amount: 1250, date: new Date(Date.now() - 86400000) },
  { id: 5, description: 'Electricity payment - Municipality', type: 'expense', amount: -380, date: new Date(Date.now() - 172800000) },
  { id: 6, description: 'Mobile money commission', type: 'income', amount: 125, date: new Date(Date.now() - 259200000) }
])

const profitLoss = ref({
  revenue: 45000,
  cogs: 18000,
  expenses: 15000,
  netProfit: 12000
})

const balanceSheet = ref({
  currentAssets: 91300,  // Cash Till + Bank + Store Credit + Stock
  fixedAssets: 25000,    // Shop Equipment
  currentLiabilities: 11700,  // Supplier credits
  longTermDebt: 15000,   // Township Business Loan
  equity: 89600          // Assets - Liabilities
})

const showCreateAccountModal = ref(false)
const showCreateJournalModal = ref(false)

// Helper functions
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-ZA', {
    style: 'decimal',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(Math.abs(amount))
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

function getAccountIcon(type: string) {
  switch (type) {
    case 'Asset': return 'svg'
    case 'Liability': return 'svg'
    case 'Income': return 'svg'
    case 'Expense': return 'svg'
    default: return 'svg'
  }
}

function getTransactionIcon(type: string) {
  switch (type) {
    case 'income': return 'svg'
    case 'expense': return 'svg'
    case 'transfer': return 'svg'
    default: return 'svg'
  }
}

// Page metadata
useHead({
  title: 'Accounts - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage financial transactions and accounting in TOSS ERP' }
  ]
})
</script>
