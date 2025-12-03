<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useAccountingStore, type JournalEntry, type JournalEntryLine, type Account } from '~/stores/accounting'

interface Props {
  show: boolean
  entry?: JournalEntry | null
}

const props = withDefaults(defineProps<Props>(), {
  entry: null
})

const emit = defineEmits<{
  close: []
  saved: [entry: JournalEntry]
}>()

const accountingStore = useAccountingStore()

// Form state
const formData = ref({
  date: new Date().toISOString().split('T')[0],
  reference: '',
  description: '',
  lines: [] as JournalEntryLine[]
})

const isEditing = computed(() => !!props.entry)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})
const showAddLineModal = ref(false)
const selectedAccount = ref<Account | null>(null)
const lineDebit = ref(0)
const lineCredit = ref(0)
const lineDescription = ref('')

// Computed
const totalDebit = computed(() => {
  return formData.value.lines.reduce((sum, line) => sum + line.debit, 0)
})

const totalCredit = computed(() => {
  return formData.value.lines.reduce((sum, line) => sum + line.credit, 0)
})

const isBalanced = computed(() => {
  return totalDebit.value === totalCredit.value && totalDebit.value > 0
})

const balanceDifference = computed(() => {
  return Math.abs(totalDebit.value - totalCredit.value)
})

// Watch for entry changes
watch(() => props.entry, (newEntry) => {
  if (newEntry) {
    formData.value = {
      date: new Date(newEntry.date).toISOString().split('T')[0],
      reference: newEntry.reference || '',
      description: newEntry.description || '',
      lines: [...newEntry.lines]
    }
  } else {
    resetForm()
  }
}, { immediate: true })

watch(() => props.show, (isShowing) => {
  if (isShowing) {
    accountingStore.fetchAccounts()
  }
})

function resetForm() {
  formData.value = {
    date: new Date().toISOString().split('T')[0],
    reference: '',
    description: '',
    lines: []
  }
  errors.value = {}
  selectedAccount.value = null
  lineDebit.value = 0
  lineCredit.value = 0
  lineDescription.value = ''
}

function validate() {
  errors.value = {}
  
  if (!formData.value.date) {
    errors.value.date = 'Date is required'
  }
  
  if (formData.value.lines.length < 2) {
    errors.value.lines = 'At least 2 lines are required'
  }
  
  if (!isBalanced.value) {
    errors.value.balance = `Entry is not balanced. Difference: ${formatCurrency(balanceDifference.value)}`
  }
  
  return Object.keys(errors.value).length === 0
}

function handleAddLine() {
  if (!selectedAccount.value) {
    alert('Please select an account')
    return
  }
  
  if (lineDebit.value === 0 && lineCredit.value === 0) {
    alert('Please enter either a debit or credit amount')
    return
  }
  
  if (lineDebit.value > 0 && lineCredit.value > 0) {
    alert('A line cannot have both debit and credit amounts')
    return
  }
  
  const line: JournalEntryLine = {
    id: `jel-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`,
    accountId: selectedAccount.value.id,
    accountCode: selectedAccount.value.code,
    accountName: selectedAccount.value.name,
    debit: lineDebit.value,
    credit: lineCredit.value,
    description: lineDescription.value || undefined
  }
  
  formData.value.lines.push(line)
  showAddLineModal.value = false
  selectedAccount.value = null
  lineDebit.value = 0
  lineCredit.value = 0
  lineDescription.value = ''
}

function handleRemoveLine(lineId: string) {
  const index = formData.value.lines.findIndex(l => l.id === lineId)
  if (index !== -1) {
    formData.value.lines.splice(index, 1)
  }
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    if (isEditing.value && props.entry) {
      // TODO: Implement update
      alert('Update functionality coming soon')
    } else {
      await accountingStore.createJournalEntry({
        date: new Date(formData.value.date),
        reference: formData.value.reference || undefined,
        description: formData.value.description || undefined,
        lines: formData.value.lines,
        status: 'draft',
        createdBy: 'admin' // TODO: Get from auth
      })
    }
    
    emit('saved', props.entry || {} as JournalEntry)
    resetForm()
  } catch (error) {
    console.error('Failed to save journal entry:', error)
  } finally {
    isSubmitting.value = false
  }
}

function handleClose() {
  resetForm()
  emit('close')
}

function handleDebitInput() {
  if (lineDebit.value > 0) {
    lineCredit.value = 0
  }
}

function handleCreditInput() {
  if (lineCredit.value > 0) {
    lineDebit.value = 0
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show"
        class="fixed inset-0 z-50 overflow-y-auto"
        @click.self="handleClose"
      >
        <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
          <div class="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" @click="handleClose"></div>

          <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <div class="flex items-center justify-between mb-6">
                <h3 class="text-2xl font-bold text-gray-900">
                  {{ isEditing ? 'Edit Journal Entry' : 'Create Journal Entry' }}
                </h3>
                <button
                  @click="handleClose"
                  class="text-gray-400 hover:text-gray-600 transition-colors"
                >
                  <i class="material-symbols-rounded text-2xl">close</i>
                </button>
              </div>

              <form @submit.prevent="handleSave" class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Date <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.date"
                      type="date"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      :class="{ 'border-red-500': errors.date }"
                    />
                    <p v-if="errors.date" class="mt-1 text-sm text-red-600">{{ errors.date }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Reference
                    </label>
                    <input
                      v-model="formData.reference"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      placeholder="e.g., INV-001"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Description
                    </label>
                    <input
                      v-model="formData.description"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      placeholder="Entry description..."
                    />
                  </div>
                </div>

                <!-- Journal Entry Lines -->
                <div>
                  <div class="flex items-center justify-between mb-4">
                    <label class="block text-sm font-medium text-gray-700">
                      Entry Lines <span class="text-red-500">*</span>
                    </label>
                    <button
                      type="button"
                      @click="showAddLineModal = true"
                      class="inline-flex items-center gap-2 px-3 py-1.5 text-sm bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                    >
                      <i class="material-symbols-rounded text-lg">add</i>
                      <span>Add Line</span>
                    </button>
                  </div>

                  <div v-if="errors.lines" class="mb-2 text-sm text-red-600">
                    {{ errors.lines }}
                  </div>

                  <div v-if="formData.lines.length === 0" class="text-center py-8 border-2 border-dashed border-gray-300 rounded-lg">
                    <i class="material-symbols-rounded text-4xl text-gray-300 mb-2">add_circle</i>
                    <p class="text-sm text-gray-600">No lines added yet. Click "Add Line" to get started.</p>
                  </div>

                  <div v-else class="border border-gray-200 rounded-lg overflow-hidden">
                    <table class="w-full">
                      <thead class="bg-gray-50">
                        <tr>
                          <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Account</th>
                          <th class="px-4 py-2 text-right text-xs font-medium text-gray-500 uppercase">Debit</th>
                          <th class="px-4 py-2 text-right text-xs font-medium text-gray-500 uppercase">Credit</th>
                          <th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase">Description</th>
                          <th class="px-4 py-2 text-center text-xs font-medium text-gray-500 uppercase">Actions</th>
                        </tr>
                      </thead>
                      <tbody class="divide-y divide-gray-200">
                        <tr v-for="line in formData.lines" :key="line.id" class="hover:bg-gray-50">
                          <td class="px-4 py-2">
                            <div>
                              <p class="text-sm font-medium text-gray-900">{{ line.accountName }}</p>
                              <p class="text-xs text-gray-500">{{ line.accountCode }}</p>
                            </div>
                          </td>
                          <td class="px-4 py-2 text-right text-sm font-semibold text-gray-900">
                            {{ line.debit > 0 ? formatCurrency(line.debit) : '-' }}
                          </td>
                          <td class="px-4 py-2 text-right text-sm font-semibold text-gray-900">
                            {{ line.credit > 0 ? formatCurrency(line.credit) : '-' }}
                          </td>
                          <td class="px-4 py-2 text-sm text-gray-600">
                            {{ line.description || '-' }}
                          </td>
                          <td class="px-4 py-2 text-center">
                            <button
                              type="button"
                              @click="handleRemoveLine(line.id)"
                              class="p-1 text-red-600 hover:text-red-900 hover:bg-red-100 rounded transition-colors"
                              title="Remove"
                            >
                              <i class="material-symbols-rounded text-lg">delete</i>
                            </button>
                          </td>
                        </tr>
                      </tbody>
                      <tfoot class="bg-gray-50 border-t-2 border-gray-300">
                        <tr>
                          <td class="px-4 py-2 text-sm font-bold text-gray-900">Total</td>
                          <td class="px-4 py-2 text-right text-sm font-bold text-gray-900">
                            {{ formatCurrency(totalDebit) }}
                          </td>
                          <td class="px-4 py-2 text-right text-sm font-bold text-gray-900">
                            {{ formatCurrency(totalCredit) }}
                          </td>
                          <td colspan="2" class="px-4 py-2 text-center">
                            <span
                              :class="[
                                'px-2 py-1 text-xs font-medium rounded-full',
                                isBalanced ? 'text-green-600 bg-green-100' : 'text-red-600 bg-red-100'
                              ]"
                            >
                              {{ isBalanced ? 'Balanced' : `Difference: ${formatCurrency(balanceDifference)}` }}
                            </span>
                          </td>
                        </tr>
                      </tfoot>
                    </table>
                  </div>

                  <div v-if="errors.balance" class="mt-2 text-sm text-red-600">
                    {{ errors.balance }}
                  </div>
                </div>

                <div class="flex items-center justify-end gap-3 pt-4 border-t border-gray-200">
                  <button
                    type="button"
                    @click="handleClose"
                    class="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    :disabled="isSubmitting || !isBalanced"
                    class="px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {{ isSubmitting ? 'Saving...' : isEditing ? 'Update' : 'Create' }}
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Add Line Modal -->
    <Transition name="modal">
      <div
        v-if="showAddLineModal"
        class="fixed inset-0 z-[60] overflow-y-auto"
        @click.self="showAddLineModal = false"
      >
        <div class="flex items-center justify-center min-h-screen px-4">
          <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6">
              <div class="flex items-center justify-between mb-4">
                <h3 class="text-xl font-bold text-gray-900">Add Journal Entry Line</h3>
                <button
                  @click="showAddLineModal = false"
                  class="text-gray-400 hover:text-gray-600 transition-colors"
                >
                  <i class="material-symbols-rounded text-2xl">close</i>
                </button>
              </div>

              <div class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Account <span class="text-red-500">*</span>
                  </label>
                  <select
                    v-model="selectedAccount"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                  >
                    <option :value="null">Select account...</option>
                    <option
                      v-for="account in accountingStore.activeAccounts"
                      :key="account.id"
                      :value="account"
                    >
                      {{ account.code }} - {{ account.name }}
                    </option>
                  </select>
                </div>

                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Debit
                    </label>
                    <input
                      v-model.number="lineDebit"
                      type="number"
                      step="0.01"
                      min="0"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      placeholder="0.00"
                      @input="handleDebitInput"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Credit
                    </label>
                    <input
                      v-model.number="lineCredit"
                      type="number"
                      step="0.01"
                      min="0"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      placeholder="0.00"
                      @input="handleCreditInput"
                    />
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Description
                  </label>
                  <input
                    v-model="lineDescription"
                    type="text"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    placeholder="Line description..."
                  />
                </div>

                <div class="flex items-center justify-end gap-3 pt-4 border-t border-gray-200">
                  <button
                    type="button"
                    @click="showAddLineModal = false"
                    class="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                    type="button"
                    @click="handleAddLine"
                    class="px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    Add Line
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
</style>

