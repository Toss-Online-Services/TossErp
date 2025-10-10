<template>
  <div class="space-y-6">
    <PageHeader
      title="Journal Entries"
      description="Record and manage general ledger journal entries"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search entries..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <input
          v-model="filterDate"
          type="date"
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <select v-model="filterStatus" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Status</option>
          <option value="Draft">Draft</option>
          <option value="Posted">Posted</option>
          <option value="Cancelled">Cancelled</option>
        </select>
      </div>
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        New Entry
      </button>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Entry #</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Reference</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Description</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Debit</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Credit</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="entry in filteredEntries" :key="entry.id" class="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 font-mono text-sm">{{ entry.entryNumber }}</td>
            <td class="px-6 py-4 text-sm">{{ formatDate(entry.date) }}</td>
            <td class="px-6 py-4 text-sm">{{ entry.reference }}</td>
            <td class="px-6 py-4">
              <div>
                <p class="font-medium">{{ entry.description }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ entry.lineItems }} line items</p>
              </div>
            </td>
            <td class="px-6 py-4 text-right font-medium">{{ formatCurrency(entry.totalDebit) }}</td>
            <td class="px-6 py-4 text-right font-medium">{{ formatCurrency(entry.totalCredit) }}</td>
            <td class="px-6 py-4">
              <span :class="getStatusClass(entry.status)" class="px-2 py-1 text-xs rounded-full">
                {{ entry.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button class="text-blue-600 hover:text-blue-800 mr-3">View</button>
              <button v-if="entry.status === 'Draft'" class="text-green-600 hover:text-green-800 mr-3">Post</button>
              <button class="text-red-600 hover:text-red-800">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="bg-blue-50 dark:bg-blue-900/20 rounded-lg p-4">
      <div class="flex justify-between items-center">
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Total Debits</p>
          <p class="text-xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(totalDebits) }}</p>
        </div>
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Total Credits</p>
          <p class="text-xl font-bold text-gray-900 dark:text-white">{{ formatCurrency(totalCredits) }}</p>
        </div>
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Difference</p>
          <p class="text-xl font-bold" :class="difference === 0 ? 'text-green-600' : 'text-red-600'">
            {{ formatCurrency(difference) }}
          </p>
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
  title: 'Journal Entries - TOSS ERP',
})

const searchQuery = ref('')
const filterDate = ref('')
const filterStatus = ref('')

const entries = ref([
  { id: 1, entryNumber: 'JE-2024-001', date: '2024-01-15', reference: 'INV-001', description: 'Sales Invoice Payment', lineItems: 2, totalDebit: 11500, totalCredit: 11500, status: 'Posted' },
  { id: 2, entryNumber: 'JE-2024-002', date: '2024-01-20', reference: 'PAY-123', description: 'Supplier Payment', lineItems: 3, totalDebit: 5000, totalCredit: 5000, status: 'Posted' },
  { id: 3, entryNumber: 'JE-2024-003', date: '2024-02-01', reference: 'ADJ-001', description: 'Month-end Accruals', lineItems: 4, totalDebit: 15000, totalCredit: 15000, status: 'Posted' },
  { id: 4, entryNumber: 'JE-2024-004', date: '2024-02-10', reference: 'DEP-001', description: 'Depreciation Entry', lineItems: 2, totalDebit: 3500, totalCredit: 3500, status: 'Posted' },
  { id: 5, entryNumber: 'JE-2024-005', date: '2024-02-15', reference: 'DRAFT', description: 'Pending Adjustment', lineItems: 2, totalDebit: 7500, totalCredit: 7500, status: 'Draft' },
])

const filteredEntries = computed(() => {
  let result = entries.value

  if (filterStatus.value) {
    result = result.filter(e => e.status === filterStatus.value)
  }

  if (filterDate.value) {
    result = result.filter(e => e.date === filterDate.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(e => 
      e.entryNumber.toLowerCase().includes(query) ||
      e.description.toLowerCase().includes(query) ||
      e.reference.toLowerCase().includes(query)
    )
  }

  return result
})

const totalDebits = computed(() => filteredEntries.value.reduce((sum, e) => sum + e.totalDebit, 0))
const totalCredits = computed(() => filteredEntries.value.reduce((sum, e) => sum + e.totalCredit, 0))
const difference = computed(() => totalDebits.value - totalCredits.value)

const getStatusClass = (status: string) => {
  const classes = {
    'Posted': 'bg-green-100 text-green-800',
    'Draft': 'bg-yellow-100 text-yellow-800',
    'Cancelled': 'bg-red-100 text-red-800',
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}
</script>
