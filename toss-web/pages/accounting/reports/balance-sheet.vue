<template>
  <div class="space-y-6">
    <PageHeader
      title="Balance Sheet"
      description="Statement of Financial Position"
    />

    <!-- Date Selection and Export -->
    <div class="flex flex-wrap gap-4 items-center justify-between">
      <div class="flex gap-4 items-center">
        <div>
          <label class="block text-sm font-medium mb-1">As of Date</label>
          <input
            v-model="selectedDate"
            type="date"
            class="px-4 py-2 border rounded-lg"
            @change="loadBalanceSheet"
          />
        </div>
      </div>

      <div class="flex gap-2">
        <ExportButton
          :data="balanceSheet"
          filename="balance-sheet"
          :formats="['pdf', 'excel', 'csv']"
        />
        <button
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          @click="loadBalanceSheet"
        >
          Refresh
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="text-center py-12">
      <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
      <p class="mt-4 text-gray-600">Loading balance sheet...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="bg-red-50 border border-red-200 rounded-lg p-4">
      <p class="text-red-800">{{ error }}</p>
    </div>

    <!-- Balance Sheet Content -->
    <div v-else-if="balanceSheet" class="space-y-6">
      <!-- Assets Section -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-bold mb-4">Assets</h2>
        
        <!-- Current Assets -->
        <div class="mb-6">
          <h3 class="text-lg font-semibold mb-3 text-gray-700">Current Assets</h3>
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left py-2">Account</th>
                <th class="text-right py-2">Amount</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="account in balanceSheet.assets.currentAssets"
                :key="account.accountId"
                class="border-b"
              >
                <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
              </tr>
              <tr class="font-semibold">
                <td class="py-2">Total Current Assets</td>
                <td class="text-right py-2">
                  {{ formatCurrency(totalCurrentAssets) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Fixed Assets -->
        <div class="mb-6">
          <h3 class="text-lg font-semibold mb-3 text-gray-700">Fixed Assets</h3>
          <table class="w-full">
            <tbody>
              <tr
                v-for="account in balanceSheet.assets.fixedAssets"
                :key="account.accountId"
                class="border-b"
              >
                <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
              </tr>
              <tr class="font-semibold">
                <td class="py-2">Total Fixed Assets</td>
                <td class="text-right py-2">
                  {{ formatCurrency(totalFixedAssets) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="border-t-2 pt-4">
          <div class="flex justify-between text-xl font-bold">
            <span>Total Assets</span>
            <span>{{ formatCurrency(balanceSheet.assets.totalAssets) }}</span>
          </div>
        </div>
      </div>

      <!-- Liabilities & Equity Section -->
      <div class="bg-white rounded-lg shadow p-6">
        <h2 class="text-xl font-bold mb-4">Liabilities & Equity</h2>
        
        <!-- Current Liabilities -->
        <div class="mb-6">
          <h3 class="text-lg font-semibold mb-3 text-gray-700">Current Liabilities</h3>
          <table class="w-full">
            <tbody>
              <tr
                v-for="account in balanceSheet.liabilities.currentLiabilities"
                :key="account.accountId"
                class="border-b"
              >
                <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
              </tr>
              <tr class="font-semibold">
                <td class="py-2">Total Current Liabilities</td>
                <td class="text-right py-2">
                  {{ formatCurrency(totalCurrentLiabilities) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Long-term Liabilities -->
        <div class="mb-6">
          <h3 class="text-lg font-semibold mb-3 text-gray-700">Long-term Liabilities</h3>
          <table class="w-full">
            <tbody>
              <tr
                v-for="account in balanceSheet.liabilities.longTermLiabilities"
                :key="account.accountId"
                class="border-b"
              >
                <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
              </tr>
              <tr class="font-semibold">
                <td class="py-2">Total Long-term Liabilities</td>
                <td class="text-right py-2">
                  {{ formatCurrency(totalLongTermLiabilities) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Equity -->
        <div class="mb-6">
          <h3 class="text-lg font-semibold mb-3 text-gray-700">Equity</h3>
          <table class="w-full">
            <tbody>
              <tr
                v-for="account in balanceSheet.equity.accounts"
                :key="account.accountId"
                class="border-b"
              >
                <td class="py-2">{{ account.accountCode }} - {{ account.accountName }}</td>
                <td class="text-right py-2">{{ formatCurrency(account.balance) }}</td>
              </tr>
              <tr class="font-semibold">
                <td class="py-2">Total Equity</td>
                <td class="text-right py-2">
                  {{ formatCurrency(balanceSheet.equity.totalEquity) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="border-t-2 pt-4">
          <div class="flex justify-between text-xl font-bold">
            <span>Total Liabilities & Equity</span>
            <span>{{ formatCurrency(balanceSheet.totalLiabilitiesAndEquity) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { BalanceSheet } from '~/types/accounting'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Balance Sheet - TOSS ERP',
})

const { getBalanceSheet, isLoading, error } = useFinancialReports()

const selectedDate = ref(new Date().toISOString().split('T')[0])
const balanceSheet = ref<BalanceSheet | null>(null)

const totalCurrentAssets = computed(() => {
  return balanceSheet.value?.assets.currentAssets.reduce((sum, acc) => sum + acc.balance, 0) || 0
})

const totalFixedAssets = computed(() => {
  return balanceSheet.value?.assets.fixedAssets.reduce((sum, acc) => sum + acc.balance, 0) || 0
})

const totalCurrentLiabilities = computed(() => {
  return balanceSheet.value?.liabilities.currentLiabilities.reduce((sum, acc) => sum + acc.balance, 0) || 0
})

const totalLongTermLiabilities = computed(() => {
  return balanceSheet.value?.liabilities.longTermLiabilities.reduce((sum, acc) => sum + acc.balance, 0) || 0
})

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const loadBalanceSheet = async () => {
  const date = new Date(selectedDate.value)
  balanceSheet.value = await getBalanceSheet(date)
}

onMounted(() => {
  loadBalanceSheet()
})
</script>

