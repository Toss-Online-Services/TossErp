<template>
  <div class="space-y-6">
    <PageHeader
      title="General Ledger"
      description="Complete transaction history by account"
    />

    <!-- Filters and Export -->
    <div class="flex flex-wrap gap-4 items-center justify-between">
      <div class="flex gap-4 items-center flex-wrap">
        <div>
          <label class="block text-sm font-medium mb-1 dark:text-gray-300">Account</label>
          <select
            v-model="selectedAccount"
            class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white dark:border-gray-600"
            @change="loadLedger"
          >
            <option value="">Select Account</option>
            <option value="1110">1110 - Cash and Bank</option>
            <option value="1120">1120 - Accounts Receivable</option>
            <option value="2110">2110 - Accounts Payable</option>
            <option value="4100">4100 - Sales Revenue</option>
            <option value="5100">5100 - Cost of Goods Sold</option>
          </select>
        </div>
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
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          @click="loadLedger"
        >
          Generate Report
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      <p class="mt-4 text-gray-600 dark:text-gray-400">Loading general ledger...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
      <p class="text-red-800 dark:text-red-200">{{ error }}</p>
    </div>

    <!-- No Account Selected -->
    <div v-else-if="!selectedAccount" class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-8 text-center">
      <svg class="w-16 h-16 mx-auto mb-4 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
      </svg>
      <p class="text-blue-800 dark:text-blue-200 font-semibold mb-2">Select an Account</p>
      <p class="text-blue-600 dark:text-blue-300 text-sm">Choose an account from the dropdown above to view its ledger entries</p>
    </div>

    <!-- General Ledger Content -->
    <div v-else-if="ledgerEntries && ledgerEntries.length > 0" class="space-y-6">
      <!-- Ledger Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-semibold dark:text-white">Account: {{ selectedAccount }}</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Period: {{ formatDate(startDate) }} to {{ formatDate(endDate) }}</p>
        </div>
        
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Date
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Reference
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Description
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Voucher
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Debit (R)
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Credit (R)
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Balance (R)
                </th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="entry in ledgerEntries" :key="entry.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap text-sm dark:text-gray-300">
                  {{ formatDate(entry.date) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap font-mono text-sm dark:text-gray-300">
                  {{ entry.reference }}
                </td>
                <td class="px-6 py-4 dark:text-gray-300">
                  {{ entry.description }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm dark:text-gray-300">
                  <div>
                    <span class="text-xs text-gray-500 dark:text-gray-400">{{ entry.voucherType }}</span>
                    <br>
                    <span class="font-mono">{{ entry.voucherNo }}</span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right font-medium text-green-600 dark:text-green-400">
                  {{ entry.debit > 0 ? formatCurrency(entry.debit) : '-' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right font-medium text-red-600 dark:text-red-400">
                  {{ entry.credit > 0 ? formatCurrency(entry.credit) : '-' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right font-bold dark:text-white">
                  {{ formatCurrency(entry.balance) }}
                </td>
              </tr>
              <!-- Final Balance Row -->
              <tr v-if="ledgerEntries.length > 0" class="bg-gray-100 dark:bg-gray-700 font-bold">
                <td colspan="4" class="px-6 py-4 dark:text-white">
                  Closing Balance
                </td>
                <td class="px-6 py-4 text-right text-green-600 dark:text-green-400">
                  {{ formatCurrency(totalDebits) }}
                </td>
                <td class="px-6 py-4 text-right text-red-600 dark:text-red-400">
                  {{ formatCurrency(totalCredits) }}
                </td>
                <td class="px-6 py-4 text-right text-blue-600 dark:text-blue-400">
                  {{ formatCurrency(closingBalance) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Summary Stats -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Debits</p>
          <p class="text-2xl font-bold text-green-600 dark:text-green-400">
            {{ formatCurrency(totalDebits) }}
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Credits</p>
          <p class="text-2xl font-bold text-red-600 dark:text-red-400">
            {{ formatCurrency(totalCredits) }}
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Closing Balance</p>
          <p class="text-2xl font-bold text-blue-600 dark:text-blue-400">
            {{ formatCurrency(closingBalance) }}
          </p>
        </div>
      </div>
    </div>

    <!-- No Entries -->
    <div v-else class="bg-gray-50 dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg p-8 text-center">
      <svg class="w-16 h-16 mx-auto mb-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
      </svg>
      <p class="text-gray-600 dark:text-gray-400 font-semibold mb-2">No Entries Found</p>
      <p class="text-gray-500 dark:text-gray-500 text-sm">No ledger entries found for the selected account and date range</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useFinancialReports } from '~/composables/useFinancialReports'
import type { LedgerEntry } from '~/composables/useFinancialReports'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'General Ledger - TOSS ERP',
})

const { getGeneralLedger, isLoading, error } = useFinancialReports()

const selectedAccount = ref('')
const startDate = ref(new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0])
const endDate = ref(new Date().toISOString().split('T')[0])
const ledgerEntries = ref<LedgerEntry[]>([])

const totalDebits = computed(() => {
  return ledgerEntries.value.reduce((sum, entry) => sum + entry.debit, 0)
})

const totalCredits = computed(() => {
  return ledgerEntries.value.reduce((sum, entry) => sum + entry.credit, 0)
})

const closingBalance = computed(() => {
  return ledgerEntries.value.length > 0 
    ? ledgerEntries.value[ledgerEntries.value.length - 1].balance 
    : 0
})

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const formatDate = (date: string | Date): string => {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const loadLedger = async () => {
  if (!selectedAccount.value) return
  
  const start = new Date(startDate.value)
  const end = new Date(endDate.value)
  ledgerEntries.value = await getGeneralLedger(selectedAccount.value, start, end)
}

const exportReport = async (format: 'csv') => {
  if (!ledgerEntries.value || ledgerEntries.value.length === 0) return

  const csvData = [
    ['General Ledger'],
    [`Account: ${selectedAccount.value}`],
    [`Period: ${formatDate(startDate.value)} to ${formatDate(endDate.value)}`],
    [''],
    ['Date', 'Reference', 'Description', 'Voucher Type', 'Voucher No', 'Debit (R)', 'Credit (R)', 'Balance (R)'],
    ...ledgerEntries.value.map(entry => [
      formatDate(entry.date),
      entry.reference,
      entry.description,
      entry.voucherType,
      entry.voucherNo,
      entry.debit,
      entry.credit,
      entry.balance
    ]),
    [''],
    ['Totals', '', '', '', '', totalDebits.value, totalCredits.value, closingBalance.value],
  ]

  const csv = csvData.map(row => row.join(',')).join('\n')
  const blob = new Blob([csv], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `general-ledger-${selectedAccount.value}-${startDate.value}.${format}`
  a.click()
  URL.revokeObjectURL(url)
}

onMounted(() => {
  // Don't load automatically - wait for account selection
})
</script>

