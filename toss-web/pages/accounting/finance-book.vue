<template>
  <div class="space-y-6">
    <PageHeader
      title="Finance Books"
      description="Manage finance books and accounting periods"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search finance books..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <select v-model="filterStatus" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Status</option>
          <option value="open">Open</option>
          <option value="closed">Closed</option>
        </select>
      </div>
      <button
        @click="showModal = true"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Create Finance Book
      </button>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <div v-for="book in filteredBooks" :key="book.id" class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ book.name }}</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ book.company }}</p>
          </div>
          <span :class="book.status === 'open' ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200'" class="px-2 py-1 text-xs rounded-full capitalize">
            {{ book.status }}
          </span>
        </div>
        
        <div class="space-y-2 mb-4">
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Fiscal Year:</span>
            <span class="text-gray-900 dark:text-white font-medium">{{ book.fiscalYear }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Period:</span>
            <span class="text-gray-900 dark:text-white font-medium">{{ book.periodStart }} - {{ book.periodEnd }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600 dark:text-gray-400">Transactions:</span>
            <span class="text-gray-900 dark:text-white font-medium">{{ book.transactionCount.toLocaleString() }}</span>
          </div>
        </div>

        <div class="flex gap-2">
          <button class="flex-1 px-3 py-2 text-sm bg-blue-50 dark:bg-blue-900 text-blue-600 dark:text-blue-300 rounded hover:bg-blue-100 dark:hover:bg-blue-800">
            View Entries
          </button>
          <button class="flex-1 px-3 py-2 text-sm bg-gray-50 dark:bg-gray-700 text-gray-600 dark:text-gray-300 rounded hover:bg-gray-100 dark:hover:bg-gray-600">
            Reports
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Finance Books - TOSS ERP',
})

const searchQuery = ref('')
const filterStatus = ref('')
const showModal = ref(false)

const books = ref([
  { id: 1, name: 'Main Finance Book', company: 'TOSS Technologies', fiscalYear: '2024', periodStart: 'Jan 2024', periodEnd: 'Dec 2024', status: 'open', transactionCount: 2456 },
  { id: 2, name: 'Manufacturing Division', company: 'TOSS Manufacturing', fiscalYear: '2024', periodStart: 'Jan 2024', periodEnd: 'Dec 2024', status: 'open', transactionCount: 1823 },
  { id: 3, name: 'Archive 2023', company: 'TOSS Technologies', fiscalYear: '2023', periodStart: 'Jan 2023', periodEnd: 'Dec 2023', status: 'closed', transactionCount: 5678 },
])

const filteredBooks = computed(() => {
  let result = books.value
  
  if (searchQuery.value) {
    result = result.filter(b =>
      b.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      b.company.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }
  
  if (filterStatus.value) {
    result = result.filter(b => b.status === filterStatus.value)
  }
  
  return result
})
</script>
