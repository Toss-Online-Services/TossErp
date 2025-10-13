<template>
  <div class="fixed inset-0 bg-gray-600 dark:bg-gray-900 bg-opacity-50 dark:bg-opacity-75 overflow-y-auto h-full w-full z-50 flex items-center justify-center" @click="$emit('close')">
    <div class="relative p-5 border w-full max-w-2xl shadow-lg rounded-md bg-white dark:bg-gray-800" @click.stop>
      <div class="mt-3">
        <!-- Header -->
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">
            {{ account ? 'Edit Account' : 'Create New Account' }}
          </h3>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit" class="space-y-4">
          <!-- Account Code and Name -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Account Code *
              </label>
              <input
                v-model="formData.code"
                type="text"
                required
                placeholder="e.g., 1110"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Account Name *
              </label>
              <input
                v-model="formData.name"
                type="text"
                required
                placeholder="e.g., Cash and Bank"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
              />
            </div>
          </div>

          <!-- Account Type -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Account Type *
            </label>
            <select
              v-model="formData.type"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
            >
              <option value="">Select Type</option>
              <option value="Asset">Asset</option>
              <option value="Liability">Liability</option>
              <option value="Equity">Equity</option>
              <option value="Revenue">Revenue</option>
              <option value="Expense">Expense</option>
            </select>
          </div>

          <!-- Parent Account -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Parent Account
            </label>
            <select
              v-model="formData.parentId"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
            >
              <option value="">No Parent (Root Account)</option>
              <option v-for="acc in groupAccounts" :key="acc.id" :value="acc.id">
                {{ acc.code }} - {{ acc.name }}
              </option>
            </select>
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
              Select a parent account to create a hierarchical structure
            </p>
          </div>

          <!-- Opening Balance -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Opening Balance (R)
            </label>
            <input
              v-model.number="formData.openingBalance"
              type="number"
              step="0.01"
              placeholder="0.00"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
            />
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
              Enter the opening balance for this account (if applicable)
            </p>
          </div>

          <!-- Checkboxes -->
          <div class="flex items-center space-x-6">
            <label class="flex items-center">
              <input
                v-model="formData.isGroup"
                type="checkbox"
                class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded dark:bg-gray-700 dark:border-gray-600"
              />
              <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Is Group Account</span>
            </label>

            <label class="flex items-center">
              <input
                v-model="formData.isActive"
                type="checkbox"
                class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded dark:bg-gray-700 dark:border-gray-600"
              />
              <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Active</span>
            </label>
          </div>

          <!-- Form Actions -->
          <div class="flex justify-end space-x-3 pt-4 border-t border-gray-200 dark:border-gray-600">
            <button
              type="button"
              @click="$emit('close')"
              class="px-4 py-2 bg-gray-300 dark:bg-gray-600 text-gray-700 dark:text-gray-300 rounded-md hover:bg-gray-400 dark:hover:bg-gray-500 transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
            >
              {{ account ? 'Update Account' : 'Create Account' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { Account } from '~/composables/useAccounting'

const props = defineProps<{
  account?: Account | null
  accounts?: Account[]
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'save', data: any): void
}>()

// Form data
const formData = ref({
  code: '',
  name: '',
  type: '' as 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense' | '',
  parentId: '',
  openingBalance: 0,
  isGroup: false,
  isActive: true
})

// Filter accounts that are groups (for parent selection)
const groupAccounts = ref<Account[]>([])

onMounted(() => {
  // If editing, populate form with existing data
  if (props.account) {
    formData.value = {
      code: props.account.code,
      name: props.account.name,
      type: props.account.type,
      parentId: props.account.parentId || '',
      openingBalance: props.account.openingBalance || 0,
      isGroup: props.account.isGroup,
      isActive: props.account.isActive
    }
  }

  // Filter group accounts for parent selection
  if (props.accounts) {
    groupAccounts.value = props.accounts.filter(acc => acc.isGroup && acc.id !== props.account?.id)
  }
})

const handleSubmit = () => {
  emit('save', {
    ...formData.value,
    type: formData.value.type || undefined
  })
}
</script>

