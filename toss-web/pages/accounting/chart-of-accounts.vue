<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useAccountingStore, type Account, type AccountType } from '~/stores/accounting'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Chart of Accounts - TOSS'
})

const accountingStore = useAccountingStore()
const searchQuery = ref('')
const typeFilter = ref<AccountType | 'all'>('all')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedAccount = ref<Account | null>(null)

// Computed
const filteredAccounts = computed(() => {
  let filtered = accountingStore.activeAccounts

  if (typeFilter.value !== 'all') {
    filtered = filtered.filter(acc => acc.type === typeFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(acc =>
      acc.code.toLowerCase().includes(query) ||
      acc.name.toLowerCase().includes(query) ||
      (acc.description || '').toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => a.code.localeCompare(b.code))
})

const stats = computed(() => {
  const accounts = accountingStore.activeAccounts
  return {
    total: accounts.length,
    assets: accounts.filter(a => a.type === 'asset').length,
    liabilities: accounts.filter(a => a.type === 'liability').length,
    equity: accounts.filter(a => a.type === 'equity').length,
    income: accounts.filter(a => a.type === 'income').length,
    expenses: accounts.filter(a => a.type === 'expense').length
  }
})

// Methods
onMounted(async () => {
  await accountingStore.fetchAccounts()
})

function handleCreate() {
  selectedAccount.value = null
  showCreateModal.value = true
}

function handleEdit(account: Account) {
  selectedAccount.value = account
  showEditModal.value = true
}

function handleAccountSaved() {
  showCreateModal.value = false
  showEditModal.value = false
  selectedAccount.value = null
  accountingStore.fetchAccounts()
}

function getTypeColor(type: AccountType) {
  const colors: Record<AccountType, string> = {
    asset: 'text-blue-600 bg-blue-100',
    liability: 'text-red-600 bg-red-100',
    equity: 'text-green-600 bg-green-100',
    income: 'text-purple-600 bg-purple-100',
    expense: 'text-orange-600 bg-orange-100'
  }
  return colors[type] || 'text-gray-600 bg-gray-100'
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">Chart of Accounts</h3>
          <p class="text-gray-600 text-sm">Manage your accounting structure and accounts</p>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Account</span>
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-6 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Accounts</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">account_balance</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Assets</p>
            <p class="mt-2 text-3xl font-bold text-blue-600">{{ stats.assets }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">savings</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Liabilities</p>
            <p class="mt-2 text-3xl font-bold text-red-600">{{ stats.liabilities }}</p>
          </div>
          <div class="p-3 bg-red-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-red-600">account_balance_wallet</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Equity</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.equity }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">trending_up</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Income</p>
            <p class="mt-2 text-3xl font-bold text-purple-600">{{ stats.income }}</p>
          </div>
          <div class="p-3 bg-purple-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-purple-600">payments</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Expenses</p>
            <p class="mt-2 text-3xl font-bold text-orange-600">{{ stats.expenses }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">receipt_long</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-card p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <div class="relative">
            <i class="material-symbols-rounded absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">search</i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by code, name, or description..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
        </div>
        <div class="md:w-48">
          <select
            v-model="typeFilter"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="all">All Types</option>
            <option value="asset">Assets</option>
            <option value="liability">Liabilities</option>
            <option value="equity">Equity</option>
            <option value="income">Income</option>
            <option value="expense">Expenses</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Accounts Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="accountingStore.loading" class="flex items-center justify-center py-12">
        <div class="text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
          <p class="text-gray-600">Loading accounts...</p>
        </div>
      </div>

      <div v-else-if="filteredAccounts.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">account_balance</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No accounts found</h3>
        <p class="text-gray-600 mb-6">Get started by creating your first account</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Account</span>
        </button>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Code</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Account Name</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Balance</th>
              <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="account in filteredAccounts"
              :key="account.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="text-sm font-semibold text-gray-900">{{ account.code }}</span>
              </td>
              <td class="px-6 py-4">
                <div>
                  <p class="text-sm font-medium text-gray-900">{{ account.name }}</p>
                  <p v-if="account.description" class="text-xs text-gray-500 mt-1">{{ account.description }}</p>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="['px-2 py-1 text-xs font-medium rounded-full', getTypeColor(account.type)]">
                  {{ accountingStore.getAccountTypeLabel(account.type) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right">
                <span :class="['text-sm font-semibold', account.balance >= 0 ? 'text-gray-900' : 'text-red-600']">
                  {{ formatCurrency(account.balance) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-center">
                <span :class="['px-2 py-1 text-xs font-medium rounded-full', account.isActive ? 'text-green-600 bg-green-100' : 'text-gray-600 bg-gray-100']">
                  {{ account.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleEdit(account)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="Edit"
                  >
                    <i class="material-symbols-rounded text-lg">edit</i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modals -->
    <AccountingAccountModal
      :show="showCreateModal"
      :account="null"
      @close="showCreateModal = false"
      @saved="handleAccountSaved"
    />
    <AccountingAccountModal
      :show="showEditModal"
      :account="selectedAccount"
      @close="showEditModal = false; selectedAccount = null"
      @saved="handleAccountSaved"
    />
  </div>
</template>
