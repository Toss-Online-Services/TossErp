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
      <div class="flex gap-2">
        <button @click="exportAccounts" class="px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700">
          Export CSV
        </button>
        <button @click="openCreateModal" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
          Add Account
        </button>
      </div>
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
                <button @click="editAccount(account)" class="text-blue-600 dark:text-blue-400 hover:text-blue-800 mr-3">Edit</button>
                <button @click="deleteAccount(account)" class="text-red-600 dark:text-red-400 hover:text-red-800">Delete</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Account Modal -->
    <AccountModal
      v-if="showModal"
      :account="selectedAccount"
      :accounts="accounts"
      @close="closeModal"
      @save="saveAccount"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useAccounting } from '~/composables/useAccounting'
import type { Account } from '~/composables/useAccounting'
import AccountModal from '~/components/accounting/AccountModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Chart of Accounts - TOSS ERP',
})

const { getAccounts, createAccount, updateAccount, deleteAccount: deleteAccountApi, isLoading } = useAccounting()

const searchQuery = ref('')
const filterType = ref('')
const accounts = ref<Account[]>([])
const showModal = ref(false)
const selectedAccount = ref<Account | null>(null)

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

// Methods
const loadAccounts = async () => {
  try {
    accounts.value = await getAccounts()
  } catch (error) {
    console.error('Error loading accounts:', error)
    alert('Failed to load accounts. Please try again.')
  }
}

const openCreateModal = () => {
  selectedAccount.value = null
  showModal.value = true
}

const editAccount = (account: Account) => {
  selectedAccount.value = account
  showModal.value = true
}

const deleteAccount = async (account: Account) => {
  if (account.hasChildren) {
    alert('Cannot delete an account that has child accounts. Please delete or reassign the child accounts first.')
    return
  }

  if (confirm(`Are you sure you want to delete account ${account.code} - ${account.name}?`)) {
    try {
      await deleteAccountApi(account.id)
      await loadAccounts()
      alert('Account deleted successfully!')
    } catch (error) {
      console.error('Error deleting account:', error)
      alert('Failed to delete account. Please try again.')
    }
  }
}

const closeModal = () => {
  showModal.value = false
  selectedAccount.value = null
}

const saveAccount = async (data: any) => {
  try {
    if (selectedAccount.value) {
      await updateAccount(selectedAccount.value.id, data)
      alert('Account updated successfully!')
    } else {
      await createAccount(data)
      alert('Account created successfully!')
    }
    closeModal()
    await loadAccounts()
  } catch (error) {
    console.error('Error saving account:', error)
    alert('Failed to save account. Please try again.')
  }
}

const exportAccounts = () => {
  const exportData = filteredAccounts.value.map(account => ({
    'Code': account.code,
    'Name': account.name,
    'Type': account.type,
    'Parent': account.parent || '',
    'Balance': account.balance,
    'Status': account.isActive ? 'Active' : 'Inactive',
    'Is Group': account.isGroup ? 'Yes' : 'No'
  }))

  const csvContent = [
    Object.keys(exportData[0]).join(','),
    ...exportData.map(row => Object.values(row).map(v => `"${v}"`).join(','))
  ].join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `chart-of-accounts-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  URL.revokeObjectURL(url)
}

// Lifecycle
onMounted(() => {
  loadAccounts()
})
</script>
