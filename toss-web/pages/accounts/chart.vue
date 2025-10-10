<template>
  <div class="space-y-6">
    <PageHeader
      title="Chart of Accounts"
      description="Manage your general ledger account structure"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search accounts..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <select v-model="filterType" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Types</option>
          <option value="Asset">Asset</option>
          <option value="Liability">Liability</option>
          <option value="Equity">Equity</option>
          <option value="Revenue">Revenue</option>
          <option value="Expense">Expense</option>
        </select>
      </div>
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Add Account
      </button>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Account Code</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Account Name</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Type</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Parent</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Balance</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="account in filteredAccounts" :key="account.id" class="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 font-mono text-sm">{{ account.code }}</td>
              <td class="px-6 py-4">
                <div class="flex items-center">
                  <Icon v-if="account.hasChildren" name="mdi:chevron-right" class="w-5 h-5 mr-2 text-gray-400" />
                  <span :style="{ paddingLeft: account.level * 20 + 'px' }">{{ account.name }}</span>
                </div>
              </td>
              <td class="px-6 py-4">
                <span :class="getTypeClass(account.type)" class="px-2 py-1 text-xs rounded-full">
                  {{ account.type }}
                </span>
              </td>
              <td class="px-6 py-4 text-sm text-gray-500 dark:text-gray-400">{{ account.parent || '-' }}</td>
              <td class="px-6 py-4 text-right font-medium" :class="account.balance < 0 ? 'text-red-600' : 'text-green-600'">
                {{ formatCurrency(account.balance) }}
              </td>
              <td class="px-6 py-4">
                <span :class="account.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'" class="px-2 py-1 text-xs rounded-full">
                  {{ account.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="px-6 py-4 text-right">
                <button class="text-blue-600 hover:text-blue-800 mr-3">Edit</button>
                <button class="text-red-600 hover:text-red-800">Delete</button>
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
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Chart of Accounts - TOSS ERP',
})

const searchQuery = ref('')
const filterType = ref('')

const accounts = ref([
  { id: 1, code: '1000', name: 'Assets', type: 'Asset', parent: null, balance: 0, isActive: true, level: 0, hasChildren: true },
  { id: 2, code: '1100', name: 'Current Assets', type: 'Asset', parent: 'Assets', balance: 0, isActive: true, level: 1, hasChildren: true },
  { id: 3, code: '1110', name: 'Cash and Bank', type: 'Asset', parent: 'Current Assets', balance: 150000, isActive: true, level: 2, hasChildren: false },
  { id: 4, code: '1120', name: 'Accounts Receivable', type: 'Asset', parent: 'Current Assets', balance: 75000, isActive: true, level: 2, hasChildren: false },
  { id: 5, code: '1130', name: 'Inventory', type: 'Asset', parent: 'Current Assets', balance: 200000, isActive: true, level: 2, hasChildren: false },
  { id: 6, code: '2000', name: 'Liabilities', type: 'Liability', parent: null, balance: 0, isActive: true, level: 0, hasChildren: true },
  { id: 7, code: '2100', name: 'Current Liabilities', type: 'Liability', parent: 'Liabilities', balance: 0, isActive: true, level: 1, hasChildren: true },
  { id: 8, code: '2110', name: 'Accounts Payable', type: 'Liability', parent: 'Current Liabilities', balance: -60000, isActive: true, level: 2, hasChildren: false },
  { id: 9, code: '2120', name: 'Salaries Payable', type: 'Liability', parent: 'Current Liabilities', balance: -30000, isActive: true, level: 2, hasChildren: false },
  { id: 10, code: '3000', name: 'Equity', type: 'Equity', parent: null, balance: 0, isActive: true, level: 0, hasChildren: true },
  { id: 11, code: '3100', name: 'Owner\'s Equity', type: 'Equity', parent: 'Equity', balance: 500000, isActive: true, level: 1, hasChildren: false },
  { id: 12, code: '4000', name: 'Revenue', type: 'Revenue', parent: null, balance: 0, isActive: true, level: 0, hasChildren: true },
  { id: 13, code: '4100', name: 'Sales Revenue', type: 'Revenue', parent: 'Revenue', balance: 1200000, isActive: true, level: 1, hasChildren: false },
  { id: 14, code: '5000', name: 'Expenses', type: 'Expense', parent: null, balance: 0, isActive: true, level: 0, hasChildren: true },
  { id: 15, code: '5100', name: 'Operating Expenses', type: 'Expense', parent: 'Expenses', balance: -215000, isActive: true, level: 1, hasChildren: false },
])

const filteredAccounts = computed(() => {
  let result = accounts.value

  if (filterType.value) {
    result = result.filter(a => a.type === filterType.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(a => 
      a.name.toLowerCase().includes(query) ||
      a.code.includes(query)
    )
  }

  return result
})

const getTypeClass = (type: string) => {
  const classes = {
    'Asset': 'bg-blue-100 text-blue-800',
    'Liability': 'bg-red-100 text-red-800',
    'Equity': 'bg-purple-100 text-purple-800',
    'Revenue': 'bg-green-100 text-green-800',
    'Expense': 'bg-yellow-100 text-yellow-800',
  }
  return classes[type as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(Math.abs(amount))
}
</script>
