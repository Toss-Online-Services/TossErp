<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Accounting</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage your financial transactions, accounts, and reporting</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showAccountModal = true" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                New Account
              </button>
              <button @click="showJournalModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <DocumentTextIcon class="w-5 h-5 inline mr-2" />
                Journal Entry
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Assets</p>
              <p class="text-2xl font-bold text-green-600 dark:text-green-400">R {{ formatNumber(stats.totalAssets) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-full">
              <BuildingLibraryIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Liabilities</p>
              <p class="text-2xl font-bold text-red-600 dark:text-red-400">R {{ formatNumber(stats.totalLiabilities) }}</p>
            </div>
            <div class="p-3 bg-red-100 dark:bg-red-900/30 rounded-full">
              <ExclamationTriangleIcon class="w-6 h-6 text-red-600 dark:text-red-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Monthly Revenue</p>
              <p class="text-2xl font-bold text-blue-600 dark:text-blue-400">R {{ formatNumber(stats.monthlyRevenue) }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-full">
              <CurrencyDollarIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Net Profit</p>
              <p class="text-2xl font-bold text-green-600 dark:text-green-400">R {{ formatNumber(stats.netProfit) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-full">
              <ChartBarIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>
      </div>

      <!-- Core Modules -->
      <div class="mb-8">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Core Accounting</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <NuxtLink to="/accounting/chart-of-accounts" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-blue-100 dark:bg-blue-900/30">
                <ListBulletIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Chart of Accounts</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Manage account structure</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/journal-entries" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-purple-100 dark:bg-purple-900/30">
                <DocumentTextIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Journal Entries</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Record transactions</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/company" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-green-100 dark:bg-green-900/30">
                <BuildingOfficeIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Company Setup</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Configure companies</p>
              </div>
            </div>
          </NuxtLink>
        </div>
      </div>

      <!-- Financial Reports -->
      <div class="mb-8">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Financial Reports</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <NuxtLink to="/accounting/reports/balance-sheet" class="bg-gradient-to-br from-blue-500 to-blue-600 dark:from-blue-600 dark:to-blue-700 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
            <div class="flex items-center justify-between mb-4">
              <ScaleIcon class="w-10 h-10" />
              <ChevronRightIcon class="w-5 h-5" />
            </div>
            <h3 class="text-lg font-semibold mb-2">Balance Sheet</h3>
            <p class="text-blue-100 text-sm">Assets, liabilities & equity</p>
          </NuxtLink>

          <NuxtLink to="/accounting/reports/profit-loss" class="bg-gradient-to-br from-green-500 to-green-600 dark:from-green-600 dark:to-green-700 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
            <div class="flex items-center justify-between mb-4">
              <ChartBarIcon class="w-10 h-10" />
              <ChevronRightIcon class="w-5 h-5" />
            </div>
            <h3 class="text-lg font-semibold mb-2">Profit & Loss</h3>
            <p class="text-green-100 text-sm">Income statement</p>
          </NuxtLink>

          <NuxtLink to="/accounting/reports/cash-flow" class="bg-gradient-to-br from-purple-500 to-purple-600 dark:from-purple-600 dark:to-purple-700 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
            <div class="flex items-center justify-between mb-4">
              <BanknotesIcon class="w-10 h-10" />
              <ChevronRightIcon class="w-5 h-5" />
            </div>
            <h3 class="text-lg font-semibold mb-2">Cash Flow</h3>
            <p class="text-purple-100 text-sm">Cash movements</p>
          </NuxtLink>

          <NuxtLink to="/accounting/vat-report" class="bg-gradient-to-br from-yellow-500 to-yellow-600 dark:from-yellow-600 dark:to-yellow-700 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
            <div class="flex items-center justify-between mb-4">
              <DocumentChartBarIcon class="w-10 h-10" />
              <ChevronRightIcon class="w-5 h-5" />
            </div>
            <h3 class="text-lg font-semibold mb-2">VAT Report</h3>
            <p class="text-yellow-100 text-sm">SA VAT compliance</p>
          </NuxtLink>

          <NuxtLink to="/accounting/reports/trial-balance" class="bg-gradient-to-br from-red-500 to-red-600 dark:from-red-600 dark:to-red-700 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
            <div class="flex items-center justify-between mb-4">
              <CalculatorIcon class="w-10 h-10" />
              <ChevronRightIcon class="w-5 h-5" />
            </div>
            <h3 class="text-lg font-semibold mb-2">Trial Balance</h3>
            <p class="text-red-100 text-sm">Verify ledger accuracy</p>
          </NuxtLink>

          <NuxtLink to="/accounting/reports/general-ledger" class="bg-gradient-to-br from-indigo-500 to-indigo-600 dark:from-indigo-600 dark:to-indigo-700 rounded-lg shadow-lg p-6 text-white hover:shadow-xl transition-shadow">
            <div class="flex items-center justify-between mb-4">
              <BookOpenIcon class="w-10 h-10" />
              <ChevronRightIcon class="w-5 h-5" />
            </div>
            <h3 class="text-lg font-semibold mb-2">General Ledger</h3>
            <p class="text-indigo-100 text-sm">Transaction history</p>
          </NuxtLink>
        </div>
      </div>

      <!-- Configuration & Setup -->
      <div class="mb-8">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Configuration</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-4">
          <NuxtLink to="/accounting/fiscal-year" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-orange-100 dark:bg-orange-900/30">
                <CalendarIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Fiscal Year</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Financial periods</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/periods" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-teal-100 dark:bg-teal-900/30">
                <ClockIcon class="w-6 h-6 text-teal-600 dark:text-teal-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Periods</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Accounting periods</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/currency" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-pink-100 dark:bg-pink-900/30">
                <CurrencyDollarIcon class="w-6 h-6 text-pink-600 dark:text-pink-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Currency</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Exchange rates</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/country" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-cyan-100 dark:bg-cyan-900/30">
                <GlobeAltIcon class="w-6 h-6 text-cyan-600 dark:text-cyan-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Countries</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Regional settings</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/finance-book" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-indigo-100 dark:bg-indigo-900/30">
                <BookOpenIcon class="w-6 h-6 text-indigo-600 dark:text-indigo-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Finance Books</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Book management</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/payment-terms" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-amber-100 dark:bg-amber-900/30">
                <ClipboardDocumentCheckIcon class="w-6 h-6 text-amber-600 dark:text-amber-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Payment Terms</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Credit conditions</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/mode-of-payment" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-lime-100 dark:bg-lime-900/30">
                <CreditCardIcon class="w-6 h-6 text-lime-600 dark:text-lime-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Payment Methods</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Accepted methods</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/accounting/loyalty-program" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-rose-100 dark:bg-rose-900/30">
                <StarIcon class="w-6 h-6 text-rose-600 dark:text-rose-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Loyalty Programs</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Customer rewards</p>
              </div>
            </div>
          </NuxtLink>
        </div>
      </div>

      <!-- Dashboard Content -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <!-- Chart of Accounts Preview -->
        <div class="lg:col-span-2">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Accounts</h3>
              <NuxtLink to="/accounting/chart-of-accounts" class="text-blue-600 dark:text-blue-400 hover:text-blue-700 text-sm">
                View All →
              </NuxtLink>
            </div>
            <div class="p-6">
              <div class="space-y-3">
                <div v-for="account in recentAccounts" :key="account.id" class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors">
                  <div class="flex items-center space-x-3">
                    <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900/30 rounded-full flex items-center justify-center">
                      <span class="text-xs font-mono text-blue-600 dark:text-blue-400">{{ account.code }}</span>
                    </div>
                    <div>
                      <p class="font-medium text-gray-900 dark:text-white text-sm">{{ account.name }}</p>
                      <p class="text-xs text-gray-600 dark:text-gray-400">{{ account.type }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="font-semibold text-gray-900 dark:text-white text-sm">R {{ formatNumber(account.balance) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Transactions -->
        <div>
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Transactions</h3>
              <NuxtLink to="/accounting/journal-entries" class="text-blue-600 dark:text-blue-400 hover:text-blue-700 text-sm">
                View All →
              </NuxtLink>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="transaction in recentTransactions" :key="transaction.id" class="flex items-center justify-between">
                  <div class="flex items-center space-x-3">
                    <div class="w-8 h-8 bg-gray-100 dark:bg-gray-700 rounded-full flex items-center justify-center">
                      <ArrowsRightLeftIcon v-if="transaction.type === 'transfer'" class="w-4 h-4 text-gray-600 dark:text-gray-400" />
                      <ArrowTrendingUpIcon v-else-if="transaction.amount >= 0" class="w-4 h-4 text-green-600 dark:text-green-400" />
                      <ArrowTrendingDownIcon v-else class="w-4 h-4 text-red-600 dark:text-red-400" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ transaction.description }}</p>
                      <p class="text-xs text-gray-600 dark:text-gray-400">{{ formatDate(transaction.date) }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-semibold" :class="transaction.amount >= 0 ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                      {{ transaction.amount >= 0 ? '+' : '' }}R {{ formatNumber(Math.abs(transaction.amount)) }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Financial Summary -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Profit & Loss Summary -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="p-6 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Profit & Loss Summary</h3>
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
                <span class="font-semibold text-green-600 dark:text-green-400">R {{ formatNumber(profitLoss.revenue) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Cost of Goods Sold</span>
                <span class="font-semibold text-red-600 dark:text-red-400">R {{ formatNumber(profitLoss.cogs) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Operating Expenses</span>
                <span class="font-semibold text-red-600 dark:text-red-400">R {{ formatNumber(profitLoss.expenses) }}</span>
              </div>
              <hr class="border-gray-200 dark:border-gray-700">
              <div class="flex justify-between">
                <span class="font-semibold text-gray-900 dark:text-white">Gross Profit</span>
                <span class="font-bold text-green-600 dark:text-green-400">R {{ formatNumber(profitLoss.revenue - profitLoss.cogs) }}</span>
              </div>
              <div class="flex justify-between">
                <span class="font-semibold text-gray-900 dark:text-white">Net Profit</span>
                <span class="font-bold text-green-600 dark:text-green-400">R {{ formatNumber(profitLoss.netProfit) }}</span>
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
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatNumber(balanceSheet.currentAssets) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Fixed Assets</span>
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatNumber(balanceSheet.fixedAssets) }}</span>
                  </div>
                </div>
              </div>
              <div>
                <h4 class="font-medium text-gray-900 dark:text-white mb-2">Liabilities</h4>
                <div class="space-y-2 ml-4">
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Current Liabilities</span>
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatNumber(balanceSheet.currentLiabilities) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-gray-600 dark:text-gray-400">Long-term Debt</span>
                    <span class="font-semibold text-gray-900 dark:text-white">R {{ formatNumber(balanceSheet.longTermDebt) }}</span>
                  </div>
                </div>
              </div>
              <hr class="border-gray-200 dark:border-gray-700">
              <div class="flex justify-between">
                <span class="font-semibold text-gray-900 dark:text-white">Equity</span>
                <span class="font-bold text-blue-600 dark:text-blue-400">R {{ formatNumber(balanceSheet.equity) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Account Modal -->
    <AccountModal
      v-if="showAccountModal"
      :accounts="accountsList"
      @close="showAccountModal = false"
      @save="handleAccountSaved"
    />

    <!-- Journal Entry Modal -->
    <JournalEntryModal
      v-if="showJournalModal"
      :accounts="accountsList"
      @close="showJournalModal = false"
      @save="handleJournalSaved"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  PlusIcon,
  DocumentTextIcon,
  BuildingLibraryIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  ChartBarIcon,
  ListBulletIcon,
  BuildingOfficeIcon,
  ScaleIcon,
  ChevronRightIcon,
  BanknotesIcon,
  DocumentChartBarIcon,
  CalculatorIcon,
  BookOpenIcon,
  CalendarIcon,
  ClockIcon,
  GlobeAltIcon,
  ClipboardDocumentCheckIcon,
  CreditCardIcon,
  StarIcon,
  ArrowsRightLeftIcon,
  ArrowTrendingUpIcon,
  ArrowTrendingDownIcon
} from '@heroicons/vue/24/outline'
import { useAccounting } from '~/composables/useAccounting'
import type { Account } from '~/composables/useAccounting'
import AccountModal from '~/components/accounting/AccountModal.vue'
import JournalEntryModal from '~/components/accounting/JournalEntryModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Accounting - TOSS ERP',
  meta: [
    { name: 'description', content: 'Comprehensive accounting management for TOSS ERP' }
  ]
})

const { getAccounts } = useAccounting()

// Reactive data
const stats = ref({
  totalAssets: 1080000,
  totalLiabilities: 655000,
  monthlyRevenue: 350000,
  netProfit: 185000
})

const recentAccounts = ref([
  { id: 1, code: '1110', name: 'Cash and Bank', type: 'Asset', balance: 150000 },
  { id: 2, code: '1120', name: 'Accounts Receivable', type: 'Asset', balance: 75000 },
  { id: 3, code: '2110', name: 'Accounts Payable', type: 'Liability', balance: 60000 },
  { id: 4, code: '4100', name: 'Sales Revenue', type: 'Revenue', balance: 1200000 },
  { id: 5, code: '5100', name: 'Cost of Goods Sold', type: 'Expense', balance: 650000 },
])

const recentTransactions = ref([
  { id: 1, description: 'Sales Invoice Payment', type: 'income', amount: 11500, date: new Date() },
  { id: 2, description: 'Supplier Payment', type: 'expense', amount: -5000, date: new Date(Date.now() - 3600000) },
  { id: 3, description: 'Month-end Accruals', type: 'transfer', amount: 15000, date: new Date(Date.now() - 7200000) },
  { id: 4, description: 'Depreciation Entry', type: 'expense', amount: -3500, date: new Date(Date.now() - 86400000) },
  { id: 5, description: 'Cash Deposit', type: 'income', amount: 25000, date: new Date(Date.now() - 172800000) },
])

const profitLoss = ref({
  revenue: 1200000,
  cogs: 650000,
  expenses: 215000,
  netProfit: 335000
})

const balanceSheet = ref({
  currentAssets: 425000,
  fixedAssets: 655000,
  currentLiabilities: 105000,
  longTermDebt: 550000,
  equity: 425000
})

const showAccountModal = ref(false)
const showJournalModal = ref(false)
const accountsList = ref<Account[]>([])

// Helper functions
const formatNumber = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'decimal',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(Math.abs(amount))
}

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

const loadAccounts = async () => {
  try {
    accountsList.value = await getAccounts()
  } catch (error) {
    console.error('Error loading accounts:', error)
  }
}

const handleAccountSaved = async () => {
  showAccountModal.value = false
  await loadAccounts()
}

const handleJournalSaved = () => {
  showJournalModal.value = false
}

// Lifecycle
onMounted(() => {
  loadAccounts()
})
</script>

