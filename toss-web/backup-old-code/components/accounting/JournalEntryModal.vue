<template>
  <div class="fixed inset-0 bg-gray-600 dark:bg-gray-900 bg-opacity-50 dark:bg-opacity-75 overflow-y-auto h-full w-full z-50 flex items-center justify-center p-4" @click="$emit('close')">
    <div class="relative p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white dark:bg-gray-800 max-h-[90vh] overflow-y-auto" @click.stop>
      <div class="mt-3">
        <!-- Header -->
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">
            {{ entry ? 'Edit Journal Entry' : 'Create New Journal Entry' }}
          </h3>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
            </svg>
          </button>
        </div>

        <!-- Form -->
        <form @submit.prevent="handleSubmit" class="space-y-4">
          <!-- Entry Header Info -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Date *
              </label>
              <input
                v-model="formData.date"
                type="date"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Reference
              </label>
              <input
                v-model="formData.reference"
                type="text"
                placeholder="e.g., INV-001"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Entry Number
              </label>
              <input
                :value="entry?.entryNumber || 'Auto-generated'"
                disabled
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-gray-100 dark:bg-gray-700 dark:text-gray-400"
              />
            </div>
          </div>

          <!-- Description -->
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
              Description *
            </label>
            <textarea
              v-model="formData.description"
              required
              rows="2"
              placeholder="Describe the purpose of this journal entry..."
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
            ></textarea>
          </div>

          <!-- Line Items -->
          <div>
            <div class="flex items-center justify-between mb-2">
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                Line Items *
              </label>
              <button
                type="button"
                @click="addLineItem"
                class="px-3 py-1 text-sm bg-blue-600 text-white rounded hover:bg-blue-700"
              >
                + Add Line
              </button>
            </div>

            <div class="overflow-x-auto">
              <table class="w-full border border-gray-200 dark:border-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700">
                  <tr>
                    <th class="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300">Account</th>
                    <th class="px-3 py-2 text-left text-xs font-medium text-gray-500 dark:text-gray-300">Description</th>
                    <th class="px-3 py-2 text-right text-xs font-medium text-gray-500 dark:text-gray-300">Debit (R)</th>
                    <th class="px-3 py-2 text-right text-xs font-medium text-gray-500 dark:text-gray-300">Credit (R)</th>
                    <th class="px-3 py-2 text-center text-xs font-medium text-gray-500 dark:text-gray-300">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, index) in formData.lineItems" :key="index" class="border-t border-gray-200 dark:border-gray-700">
                    <td class="px-3 py-2">
                      <select
                        v-model="item.accountId"
                        required
                        class="w-full px-2 py-1 text-sm border border-gray-300 dark:border-gray-600 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                      >
                        <option value="">Select Account</option>
                        <option v-for="acc in accountsList" :key="acc.id" :value="acc.id">
                          {{ acc.code }} - {{ acc.name }}
                        </option>
                      </select>
                    </td>
                    <td class="px-3 py-2">
                      <input
                        v-model="item.description"
                        type="text"
                        placeholder="Optional"
                        class="w-full px-2 py-1 text-sm border border-gray-300 dark:border-gray-600 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                      />
                    </td>
                    <td class="px-3 py-2">
                      <input
                        v-model.number="item.debit"
                        type="number"
                        step="0.01"
                        min="0"
                        placeholder="0.00"
                        @input="onDebitChange(index)"
                        class="w-full px-2 py-1 text-sm text-right border border-gray-300 dark:border-gray-600 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                      />
                    </td>
                    <td class="px-3 py-2">
                      <input
                        v-model.number="item.credit"
                        type="number"
                        step="0.01"
                        min="0"
                        placeholder="0.00"
                        @input="onCreditChange(index)"
                        class="w-full px-2 py-1 text-sm text-right border border-gray-300 dark:border-gray-600 rounded focus:outline-none focus:ring-1 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                      />
                    </td>
                    <td class="px-3 py-2 text-center">
                      <button
                        v-if="formData.lineItems.length > 2"
                        type="button"
                        @click="removeLineItem(index)"
                        class="text-red-600 hover:text-red-800 dark:text-red-400"
                      >
                        <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                          <path fill-rule="evenodd" d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z" clip-rule="evenodd"/>
                        </svg>
                      </button>
                    </td>
                  </tr>
                  <!-- Totals Row -->
                  <tr class="bg-gray-100 dark:bg-gray-700 font-semibold">
                    <td colspan="2" class="px-3 py-2 text-right dark:text-white">Totals:</td>
                    <td class="px-3 py-2 text-right" :class="totalDebit > 0 ? 'text-green-600 dark:text-green-400' : 'dark:text-white'">
                      {{ formatCurrency(totalDebit) }}
                    </td>
                    <td class="px-3 py-2 text-right" :class="totalCredit > 0 ? 'text-red-600 dark:text-red-400' : 'dark:text-white'">
                      {{ formatCurrency(totalCredit) }}
                    </td>
                    <td class="px-3 py-2"></td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Balance Validation -->
            <div v-if="!isBalanced" class="mt-2 p-2 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded">
              <p class="text-sm text-red-800 dark:text-red-200">
                ⚠️ Entry is out of balance by {{ formatCurrency(Math.abs(totalDebit - totalCredit)) }}
              </p>
            </div>
            <div v-else-if="totalDebit > 0" class="mt-2 p-2 bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded">
              <p class="text-sm text-green-800 dark:text-green-200">
                ✓ Entry is balanced ({{ formatCurrency(totalDebit) }})
              </p>
            </div>
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
              :disabled="!isBalanced || formData.lineItems.length < 2"
              :class="[
                'px-4 py-2 rounded-md transition-colors',
                isBalanced && formData.lineItems.length >= 2
                  ? 'bg-blue-600 text-white hover:bg-blue-700'
                  : 'bg-gray-300 text-gray-500 cursor-not-allowed'
              ]"
            >
              {{ entry ? 'Update Entry' : 'Create Entry (Draft)' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { JournalEntry, Account } from '~/composables/useAccounting'

const props = defineProps<{
  entry?: JournalEntry | null
  accounts?: Account[]
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'save', data: any): void
}>()

// Form data
const formData = ref({
  date: new Date().toISOString().split('T')[0],
  reference: '',
  description: '',
  lineItems: [
    { accountId: '', debit: 0, credit: 0, description: '' },
    { accountId: '', debit: 0, credit: 0, description: '' }
  ]
})

// Filter non-group accounts for line item selection
const accountsList = computed(() => {
  return props.accounts?.filter(acc => !acc.isGroup) || []
})

// Computed totals
const totalDebit = computed(() => {
  return formData.value.lineItems.reduce((sum, item) => sum + (item.debit || 0), 0)
})

const totalCredit = computed(() => {
  return formData.value.lineItems.reduce((sum, item) => sum + (item.credit || 0), 0)
})

const isBalanced = computed(() => {
  const diff = Math.abs(totalDebit.value - totalCredit.value)
  return diff < 0.01 && totalDebit.value > 0
})

// Methods
const addLineItem = () => {
  formData.value.lineItems.push({
    accountId: '',
    debit: 0,
    credit: 0,
    description: ''
  })
}

const removeLineItem = (index: number) => {
  formData.value.lineItems.splice(index, 1)
}

const onDebitChange = (index: number) => {
  if (formData.value.lineItems[index].debit > 0) {
    formData.value.lineItems[index].credit = 0
  }
}

const onCreditChange = (index: number) => {
  if (formData.value.lineItems[index].credit > 0) {
    formData.value.lineItems[index].debit = 0
  }
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const handleSubmit = () => {
  if (!isBalanced.value) {
    alert('Entry must be balanced before saving')
    return
  }

  emit('save', {
    ...formData.value,
    lineItems: formData.value.lineItems.filter(item => item.accountId)
  })
}

onMounted(() => {
  // If editing, populate form with existing data
  if (props.entry) {
    formData.value = {
      date: props.entry.date,
      reference: props.entry.reference,
      description: props.entry.description,
      lineItems: props.entry.lineItems.map(item => ({
        accountId: item.accountId,
        debit: item.debit,
        credit: item.credit,
        description: item.description || ''
      }))
    }
  }
})
</script>

