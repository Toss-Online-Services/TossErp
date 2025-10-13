<template>
  <div class="space-y-6">
    <PageHeader
      title="Trial Balance"
      description="Verify that total debits equal total credits"
    />

    <!-- Date Selection and Export -->
    <div class="flex flex-wrap gap-4 items-center justify-between">
      <div class="flex gap-4 items-center">
        <div>
          <label class="block text-sm font-medium mb-1 dark:text-gray-300">As of Date</label>
          <input
            v-model="selectedDate"
            type="date"
            class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white dark:border-gray-600"
            @change="loadTrialBalance"
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
          @click="exportReport('excel')"
          class="px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700"
        >
          Export Excel
        </button>
        <button
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          @click="loadTrialBalance"
        >
          Refresh
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      <p class="mt-4 text-gray-600 dark:text-gray-400">Loading trial balance...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
      <p class="text-red-800 dark:text-red-200">{{ error }}</p>
    </div>

    <!-- Trial Balance Content -->
    <div v-else-if="trialBalance" class="space-y-6">
      <!-- Balance Status Card -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold dark:text-white">Trial Balance Status</h3>
            <p class="text-sm text-gray-600 dark:text-gray-400">As of {{ formatDate(trialBalance.asOfDate) }}</p>
          </div>
          <div v-if="trialBalance.isBalanced" class="flex items-center gap-2 px-4 py-2 bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-200 rounded-lg">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
            </svg>
            <span class="font-semibold">Balanced</span>
          </div>
          <div v-else class="flex items-center gap-2 px-4 py-2 bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-200 rounded-lg">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
            </svg>
            <span class="font-semibold">Out of Balance</span>
          </div>
        </div>
      </div>

      <!-- Trial Balance Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Account Code
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Account Name
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Debit (R)
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Credit (R)
                </th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="account in trialBalance.accounts" :key="account.accountCode" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap font-mono text-sm dark:text-gray-300">
                  {{ account.accountCode }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap dark:text-gray-300">
                  {{ account.accountName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right font-medium text-gray-900 dark:text-white">
                  {{ account.debit > 0 ? formatCurrency(account.debit) : '-' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right font-medium text-gray-900 dark:text-white">
                  {{ account.credit > 0 ? formatCurrency(account.credit) : '-' }}
                </td>
              </tr>
              <!-- Totals Row -->
              <tr class="bg-gray-100 dark:bg-gray-700 font-bold">
                <td colspan="2" class="px-6 py-4 dark:text-white">
                  Total
                </td>
                <td class="px-6 py-4 text-right text-green-600 dark:text-green-400">
                  {{ formatCurrency(trialBalance.totalDebits) }}
                </td>
                <td class="px-6 py-4 text-right text-red-600 dark:text-red-400">
                  {{ formatCurrency(trialBalance.totalCredits) }}
                </td>
              </tr>
              <!-- Difference Row (if out of balance) -->
              <tr v-if="!trialBalance.isBalanced" class="bg-red-50 dark:bg-red-900/20 font-semibold">
                <td colspan="2" class="px-6 py-4 text-red-800 dark:text-red-200">
                  Difference (Out of Balance)
                </td>
                <td colspan="2" class="px-6 py-4 text-right text-red-800 dark:text-red-200">
                  {{ formatCurrency(Math.abs(trialBalance.totalDebits - trialBalance.totalCredits)) }}
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
            {{ formatCurrency(trialBalance.totalDebits) }}
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Credits</p>
          <p class="text-2xl font-bold text-red-600 dark:text-red-400">
            {{ formatCurrency(trialBalance.totalCredits) }}
          </p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Difference</p>
          <p class="text-2xl font-bold" :class="trialBalance.isBalanced ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
            {{ formatCurrency(Math.abs(trialBalance.totalDebits - trialBalance.totalCredits)) }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useFinancialReports } from '~/composables/useFinancialReports'
import type { TrialBalance } from '~/composables/useFinancialReports'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Trial Balance - TOSS ERP',
})

const { getTrialBalance, isLoading, error } = useFinancialReports()

const selectedDate = ref(new Date().toISOString().split('T')[0])
const trialBalance = ref<TrialBalance | null>(null)

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const formatDate = (date: Date): string => {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const loadTrialBalance = async () => {
  const date = new Date(selectedDate.value)
  trialBalance.value = await getTrialBalance(date)
}

const exportReport = async (format: 'csv' | 'excel') => {
  if (!trialBalance.value) return

  const csvData = [
    ['Trial Balance'],
    [`As of: ${formatDate(trialBalance.value.asOfDate)}`],
    [''],
    ['Account Code', 'Account Name', 'Debit (R)', 'Credit (R)'],
    ...trialBalance.value.accounts.map(account => [
      account.accountCode,
      account.accountName,
      account.debit,
      account.credit
    ]),
    [''],
    ['Total', '', trialBalance.value.totalDebits, trialBalance.value.totalCredits],
    ['Status', trialBalance.value.isBalanced ? 'Balanced' : 'Out of Balance', '', ''],
  ]

  const csv = csvData.map(row => row.join(',')).join('\n')
  const blob = new Blob([csv], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `trial-balance-${selectedDate.value}.${format}`
  a.click()
  URL.revokeObjectURL(url)
}

onMounted(() => {
  loadTrialBalance()
})
</script>

