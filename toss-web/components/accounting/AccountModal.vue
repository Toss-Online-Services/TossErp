<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useAccountingStore, type Account, type AccountType } from '~/stores/accounting'

interface Props {
  show: boolean
  account?: Account | null
}

const props = withDefaults(defineProps<Props>(), {
  account: null
})

const emit = defineEmits<{
  close: []
  saved: [account: Account]
}>()

const accountingStore = useAccountingStore()

// Form state
const formData = ref({
  code: '',
  name: '',
  type: 'asset' as AccountType,
  subType: '' as string,
  parentId: '',
  balance: 0,
  isActive: true,
  description: ''
})

const isEditing = computed(() => !!props.account)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

// Watch for account changes
watch(() => props.account, (newAccount) => {
  if (newAccount) {
    formData.value = {
      code: newAccount.code,
      name: newAccount.name,
      type: newAccount.type,
      subType: newAccount.subType || '',
      parentId: newAccount.parentId || '',
      balance: newAccount.balance,
      isActive: newAccount.isActive,
      description: newAccount.description || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    code: '',
    name: '',
    type: 'asset',
    subType: '',
    parentId: '',
    balance: 0,
    isActive: true,
    description: ''
  }
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.code.trim()) {
    errors.value.code = 'Account code is required'
  }
  
  if (!formData.value.name.trim()) {
    errors.value.name = 'Account name is required'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    if (isEditing.value && props.account) {
      await accountingStore.updateAccount(props.account.id, formData.value)
    } else {
      await accountingStore.createAccount(formData.value as any)
    }
    
    emit('saved', props.account || {} as Account)
    resetForm()
  } catch (error) {
    console.error('Failed to save account:', error)
  } finally {
    isSubmitting.value = false
  }
}

function handleClose() {
  resetForm()
  emit('close')
}

const accountTypes: Array<{ value: AccountType; label: string }> = [
  { value: 'asset', label: 'Asset' },
  { value: 'liability', label: 'Liability' },
  { value: 'equity', label: 'Equity' },
  { value: 'income', label: 'Income' },
  { value: 'expense', label: 'Expense' }
]

const subTypes = computed(() => {
  const subTypesMap: Record<AccountType, Array<{ value: string; label: string }>> = {
    asset: [
      { value: 'current_asset', label: 'Current Asset' },
      { value: 'fixed_asset', label: 'Fixed Asset' }
    ],
    liability: [
      { value: 'current_liability', label: 'Current Liability' },
      { value: 'long_term_liability', label: 'Long-term Liability' }
    ],
    equity: [
      { value: 'equity', label: 'Equity' }
    ],
    income: [
      { value: 'revenue', label: 'Revenue' }
    ],
    expense: [
      { value: 'cost_of_sales', label: 'Cost of Sales' },
      { value: 'operating_expense', label: 'Operating Expense' },
      { value: 'other_expense', label: 'Other Expense' }
    ]
  }
  return subTypesMap[formData.value.type] || []
})
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

          <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-2xl sm:w-full">
            <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
              <div class="flex items-center justify-between mb-6">
                <h3 class="text-2xl font-bold text-gray-900">
                  {{ isEditing ? 'Edit Account' : 'Create Account' }}
                </h3>
                <button
                  @click="handleClose"
                  class="text-gray-400 hover:text-gray-600 transition-colors"
                >
                  <i class="material-symbols-rounded text-2xl">close</i>
                </button>
              </div>

              <form @submit.prevent="handleSave" class="space-y-4">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Account Code <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.code"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      :class="{ 'border-red-500': errors.code }"
                      placeholder="e.g., 1000"
                    />
                    <p v-if="errors.code" class="mt-1 text-sm text-red-600">{{ errors.code }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Account Name <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.name"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                      :class="{ 'border-red-500': errors.name }"
                      placeholder="e.g., Cash"
                    />
                    <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                  </div>
                </div>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Account Type <span class="text-red-500">*</span>
                    </label>
                    <select
                      v-model="formData.type"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    >
                      <option v-for="type in accountTypes" :key="type.value" :value="type.value">
                        {{ type.label }}
                      </option>
                    </select>
                  </div>

                  <div v-if="subTypes.length > 0">
                    <label class="block text-sm font-medium text-gray-700 mb-1">
                      Sub Type
                    </label>
                    <select
                      v-model="formData.subType"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    >
                      <option value="">Select sub type...</option>
                      <option v-for="subType in subTypes" :key="subType.value" :value="subType.value">
                        {{ subType.label }}
                      </option>
                    </select>
                  </div>
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Opening Balance
                  </label>
                  <input
                    v-model.number="formData.balance"
                    type="number"
                    step="0.01"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    placeholder="0.00"
                  />
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1">
                    Description
                  </label>
                  <textarea
                    v-model="formData.description"
                    rows="3"
                    class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
                    placeholder="Account description..."
                  ></textarea>
                </div>

                <div class="flex items-center">
                  <input
                    v-model="formData.isActive"
                    type="checkbox"
                    id="isActive"
                    class="h-4 w-4 text-gray-900 focus:ring-gray-900 border-gray-300 rounded"
                  />
                  <label for="isActive" class="ml-2 block text-sm text-gray-700">
                    Active
                  </label>
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
                    :disabled="isSubmitting"
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

