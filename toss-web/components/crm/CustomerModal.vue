<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useCrmStore, type Customer } from '~/stores/crm'

interface Props {
  show: boolean
  customer?: Customer | null
}

const props = withDefaults(defineProps<Props>(), {
  customer: null
})

const emit = defineEmits<{
  close: []
  saved: [customer: Customer]
}>()

const crmStore = useCrmStore()

// Form state
const formData = ref({
  name: '',
  email: '',
  phone: '',
  address: '',
  city: '',
  postalCode: '',
  customerType: 'individual' as 'individual' | 'business',
  creditLimit: '',
  status: 'active' as 'active' | 'inactive',
  tags: [] as string[],
  notes: ''
})

const isEditing = computed(() => !!props.customer)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})
const tagInput = ref('')

// Watch for customer changes
watch(() => props.customer, (newCustomer) => {
  if (newCustomer) {
    formData.value = {
      name: newCustomer.name,
      email: newCustomer.email || '',
      phone: newCustomer.phone,
      address: newCustomer.address || '',
      city: newCustomer.city || '',
      postalCode: newCustomer.postalCode || '',
      customerType: newCustomer.customerType,
      creditLimit: newCustomer.creditLimit.toString(),
      status: newCustomer.status,
      tags: [...newCustomer.tags],
      notes: newCustomer.notes || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    name: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    postalCode: '',
    customerType: 'individual',
    creditLimit: '',
    status: 'active',
    tags: [],
    notes: ''
  }
  errors.value = {}
  tagInput.value = ''
}

function validate() {
  errors.value = {}
  
  if (!formData.value.name.trim()) {
    errors.value.name = 'Customer name is required'
  }
  
  if (!formData.value.phone.trim()) {
    errors.value.phone = 'Phone number is required'
  }
  
  if (formData.value.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.value.email)) {
    errors.value.email = 'Invalid email format'
  }
  
  return Object.keys(errors.value).length === 0
}

function addTag() {
  const tag = tagInput.value.trim().toLowerCase()
  if (tag && !formData.value.tags.includes(tag)) {
    formData.value.tags.push(tag)
    tagInput.value = ''
  }
}

function removeTag(tag: string) {
  const index = formData.value.tags.indexOf(tag)
  if (index !== -1) {
    formData.value.tags.splice(index, 1)
  }
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const customerData = {
      name: formData.value.name.trim(),
      email: formData.value.email.trim() || undefined,
      phone: formData.value.phone.trim(),
      address: formData.value.address.trim() || undefined,
      city: formData.value.city.trim() || undefined,
      postalCode: formData.value.postalCode.trim() || undefined,
      customerType: formData.value.customerType,
      creditLimit: formData.value.creditLimit ? Number(formData.value.creditLimit) : 0,
      currentBalance: props.customer?.currentBalance || 0,
      outstandingAmount: props.customer?.outstandingAmount || 0,
      totalPurchases: props.customer?.totalPurchases || 0,
      lastPurchaseDate: props.customer?.lastPurchaseDate,
      status: formData.value.status,
      tags: formData.value.tags,
      notes: formData.value.notes.trim() || undefined
    }
    
    if (isEditing.value && props.customer) {
      await crmStore.updateCustomer(props.customer.id, customerData)
      emit('saved', { ...props.customer, ...customerData } as Customer)
    } else {
      const newCustomer = await crmStore.createCustomer(customerData)
      emit('saved', newCustomer)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save customer:', error)
    alert('Failed to save customer. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

function handleClose() {
  emit('close')
  resetForm()
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
        <div class="flex min-h-screen items-center justify-center p-4">
          <div
            class="relative w-full max-w-2xl rounded-xl bg-white shadow-xl max-h-[90vh] flex flex-col"
            @click.stop
          >
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-xl font-bold text-gray-900">
                {{ isEditing ? 'Edit Customer' : 'Add Customer' }}
              </h3>
              <button
                @click="handleClose"
                class="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <i class="material-symbols-rounded text-2xl">close</i>
              </button>
            </div>

            <!-- Form -->
            <form @submit.prevent="handleSave" class="flex-1 overflow-y-auto p-6 space-y-6">
              <!-- Basic Information -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Basic Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Customer Name <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.name"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.name }"
                      placeholder="Enter customer name"
                    >
                    <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Customer Type
                    </label>
                    <select
                      v-model="formData.customerType"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="individual">Individual</option>
                      <option value="business">Business</option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Status
                    </label>
                    <select
                      v-model="formData.status"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="active">Active</option>
                      <option value="inactive">Inactive</option>
                    </select>
                  </div>
                </div>
              </div>

              <!-- Contact Information -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Contact Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Phone <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.phone"
                      type="tel"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.phone }"
                      placeholder="+27 82 123 4567"
                    >
                    <p v-if="errors.phone" class="mt-1 text-sm text-red-600">{{ errors.phone }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Email
                    </label>
                    <input
                      v-model="formData.email"
                      type="email"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.email }"
                      placeholder="customer@example.com"
                    >
                    <p v-if="errors.email" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
                  </div>

                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Address
                    </label>
                    <input
                      v-model="formData.address"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Street address"
                    >
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      City
                    </label>
                    <input
                      v-model="formData.city"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="City"
                    >
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Postal Code
                    </label>
                    <input
                      v-model="formData.postalCode"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Postal code"
                    >
                  </div>
                </div>
              </div>

              <!-- Financial Information -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Financial Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Credit Limit (R)
                    </label>
                    <input
                      v-model="formData.creditLimit"
                      type="number"
                      min="0"
                      step="0.01"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="0.00"
                    >
                  </div>
                </div>
              </div>

              <!-- Tags -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Tags</h4>
                <div class="flex flex-wrap gap-2 mb-3">
                  <span
                    v-for="tag in formData.tags"
                    :key="tag"
                    class="inline-flex items-center gap-1 px-3 py-1 bg-gray-100 text-gray-700 rounded-full text-sm"
                  >
                    {{ tag }}
                    <button
                      type="button"
                      @click="removeTag(tag)"
                      class="text-gray-500 hover:text-gray-700"
                    >
                      <i class="material-symbols-rounded text-sm">close</i>
                    </button>
                  </span>
                </div>
                <div class="flex gap-2">
                  <input
                    v-model="tagInput"
                    @keyup.enter.prevent="addTag"
                    type="text"
                    class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="Add tag (press Enter)"
                  >
                  <button
                    type="button"
                    @click="addTag"
                    class="px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    <i class="material-symbols-rounded">add</i>
                  </button>
                </div>
              </div>

              <!-- Notes -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Notes
                </label>
                <textarea
                  v-model="formData.notes"
                  rows="3"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  placeholder="Additional notes (optional)"
                ></textarea>
              </div>

              <!-- Actions -->
              <div class="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  type="button"
                  @click="handleClose"
                  class="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  :disabled="isSubmitting"
                  class="px-4 py-2 text-white bg-gray-900 rounded-lg hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <span v-if="isSubmitting">Saving...</span>
                  <span v-else>{{ isEditing ? 'Update' : 'Create' }} Customer</span>
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}
</style>

