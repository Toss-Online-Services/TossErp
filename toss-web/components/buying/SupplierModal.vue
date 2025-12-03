<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useBuyingStore, type Supplier } from '~/stores/buying'

interface Props {
  show: boolean
  supplier?: Supplier | null
}

const props = withDefaults(defineProps<Props>(), {
  supplier: null
})

const emit = defineEmits<{
  close: []
  saved: [supplier: Supplier]
}>()

const buyingStore = useBuyingStore()

// Form state
const formData = ref({
  name: '',
  contactPerson: '',
  email: '',
  phone: '',
  address: '',
  city: '',
  province: '',
  postalCode: '',
  paymentTerms: 'Net 30',
  currency: 'ZAR',
  creditLimit: '',
  isActive: true,
  notes: ''
})

const isEditing = computed(() => !!props.supplier)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

// Watch for supplier changes
watch(() => props.supplier, (newSupplier) => {
  if (newSupplier) {
    formData.value = {
      name: newSupplier.name,
      contactPerson: newSupplier.contactPerson || '',
      email: newSupplier.email || '',
      phone: newSupplier.phone || '',
      address: newSupplier.address || '',
      city: newSupplier.city || '',
      province: newSupplier.province || '',
      postalCode: newSupplier.postalCode || '',
      paymentTerms: newSupplier.paymentTerms,
      currency: newSupplier.currency,
      creditLimit: newSupplier.creditLimit?.toString() || '',
      isActive: newSupplier.isActive,
      notes: newSupplier.notes || ''
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    name: '',
    contactPerson: '',
    email: '',
    phone: '',
    address: '',
    city: '',
    province: '',
    postalCode: '',
    paymentTerms: 'Net 30',
    currency: 'ZAR',
    creditLimit: '',
    isActive: true,
    notes: ''
  }
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.name.trim()) {
    errors.value.name = 'Supplier name is required'
  }
  
  if (formData.value.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.value.email)) {
    errors.value.email = 'Invalid email format'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const supplierData = {
      name: formData.value.name.trim(),
      contactPerson: formData.value.contactPerson.trim() || undefined,
      email: formData.value.email.trim() || undefined,
      phone: formData.value.phone.trim() || undefined,
      address: formData.value.address.trim() || undefined,
      city: formData.value.city.trim() || undefined,
      province: formData.value.province.trim() || undefined,
      postalCode: formData.value.postalCode.trim() || undefined,
      paymentTerms: formData.value.paymentTerms,
      currency: formData.value.currency,
      creditLimit: formData.value.creditLimit ? Number(formData.value.creditLimit) : undefined,
      isActive: formData.value.isActive,
      notes: formData.value.notes.trim() || undefined
    }
    
    if (isEditing.value && props.supplier) {
      await buyingStore.updateSupplier(props.supplier.id, supplierData)
      emit('saved', { ...props.supplier, ...supplierData } as Supplier)
    } else {
      const newSupplier = await buyingStore.createSupplier(supplierData)
      emit('saved', newSupplier)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save supplier:', error)
    alert('Failed to save supplier. Please try again.')
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
                {{ isEditing ? 'Edit Supplier' : 'Add Supplier' }}
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
                      Supplier Name <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.name"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      :class="{ 'border-red-500': errors.name }"
                      placeholder="Enter supplier name"
                    >
                    <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Contact Person
                    </label>
                    <input
                      v-model="formData.contactPerson"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Contact person name"
                    >
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
                      placeholder="supplier@example.com"
                    >
                    <p v-if="errors.email" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Phone
                    </label>
                    <input
                      v-model="formData.phone"
                      type="tel"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="+27 11 123 4567"
                    >
                  </div>
                </div>
              </div>

              <!-- Address Information -->
              <div>
                <h4 class="text-sm font-semibold text-gray-900 mb-4">Address Information</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div class="md:col-span-2">
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Street Address
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
                      Province
                    </label>
                    <input
                      v-model="formData.province"
                      type="text"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                      placeholder="Province"
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
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Payment Terms
                    </label>
                    <select
                      v-model="formData.paymentTerms"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="Net 15">Net 15</option>
                      <option value="Net 30">Net 30</option>
                      <option value="Net 60">Net 60</option>
                      <option value="Cash on Delivery">Cash on Delivery</option>
                      <option value="Immediate">Immediate</option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-2">
                      Currency
                    </label>
                    <select
                      v-model="formData.currency"
                      class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    >
                      <option value="ZAR">ZAR (South African Rand)</option>
                      <option value="USD">USD (US Dollar)</option>
                      <option value="EUR">EUR (Euro)</option>
                    </select>
                  </div>

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

              <!-- Status and Notes -->
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="flex items-center gap-2">
                    <input
                      v-model="formData.isActive"
                      type="checkbox"
                      class="w-4 h-4 text-gray-900 border-gray-300 rounded focus:ring-gray-900"
                    >
                    <span class="text-sm font-medium text-gray-700">Active Supplier</span>
                  </label>
                </div>
              </div>

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
                  <span v-else>{{ isEditing ? 'Update' : 'Create' }} Supplier</span>
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

