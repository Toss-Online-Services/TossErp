<template>
  <div class="space-y-6">
    <PageHeader
      title="Currency Management"
      description="Manage currencies and exchange rates"
    />

    <div class="flex justify-between items-center">
      <div>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search currencies..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
      </div>
      <button
        @click="showModal = true"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Add Currency
      </button>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Code</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Name</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Symbol</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Exchange Rate</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Last Updated</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="currency in filteredCurrencies" :key="currency.id" class="dark:text-white">
            <td class="px-6 py-4 font-medium">{{ currency.code }}</td>
            <td class="px-6 py-4">{{ currency.name }}</td>
            <td class="px-6 py-4">{{ currency.symbol }}</td>
            <td class="px-6 py-4">{{ currency.exchangeRate.toFixed(4) }}</td>
            <td class="px-6 py-4">{{ formatDate(currency.lastUpdated) }}</td>
            <td class="px-6 py-4">
              <span :class="currency.isActive ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'" class="px-2 py-1 text-xs rounded-full">
                {{ currency.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button class="text-blue-600 dark:text-blue-400 hover:text-blue-800 mr-3">Edit</button>
              <button class="text-green-600 dark:text-green-400 hover:text-green-800 mr-3">Update Rate</button>
              <button class="text-red-600 dark:text-red-400 hover:text-red-800">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Currency Management - TOSS ERP',
})

const searchQuery = ref('')
const showModal = ref(false)

const currencies = ref([
  { id: 1, code: 'ZAR', name: 'South African Rand', symbol: 'R', exchangeRate: 1.0000, lastUpdated: new Date(), isActive: true },
  { id: 2, code: 'USD', name: 'US Dollar', symbol: '$', exchangeRate: 0.0535, lastUpdated: new Date(), isActive: true },
  { id: 3, code: 'EUR', name: 'Euro', symbol: '€', exchangeRate: 0.0490, lastUpdated: new Date(), isActive: true },
  { id: 4, code: 'GBP', name: 'British Pound', symbol: '£', exchangeRate: 0.0420, lastUpdated: new Date(), isActive: true },
  { id: 5, code: 'JPY', name: 'Japanese Yen', symbol: '¥', exchangeRate: 7.8500, lastUpdated: new Date(), isActive: true },
])

const filteredCurrencies = computed(() => {
  if (!searchQuery.value) return currencies.value
  return currencies.value.filter(c =>
    c.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    c.code.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}
</script>
