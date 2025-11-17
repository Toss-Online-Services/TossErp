<template>
  <TransitionRoot as="template" :show="isOpen">
    <Dialog as="div" class="relative z-10" @close="$emit('close')">
      <TransitionChild
        as="template"
        enter="ease-out duration-300"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="ease-in duration-200"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" />
      </TransitionChild>

      <div class="fixed inset-0 z-10 overflow-y-auto">
        <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
          <TransitionChild
            as="template"
            enter="ease-out duration-300"
            enter-from="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
            enter-to="opacity-100 translate-y-0 sm:scale-100"
            leave="ease-in duration-200"
            leave-from="opacity-100 translate-y-0 sm:scale-100"
            leave-to="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
          >
            <DialogPanel class="relative transform overflow-hidden rounded-lg bg-white px-4 pb-4 pt-5 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-2xl sm:p-6">
              <form @submit.prevent="handleSubmit">
                <div>
                  <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-green-100">
                    <UserPlusIcon class="h-6 w-6 text-green-600" aria-hidden="true" />
                  </div>
                  <div class="mt-3 text-center sm:mt-5">
                    <DialogTitle as="h3" class="text-base font-semibold leading-6 text-gray-900">
                      {{ customer ? 'Edit Customer' : 'Create New Customer' }}
                    </DialogTitle>
                  </div>
                </div>

                <!-- Form Fields -->
                <div class="mt-6 space-y-6">
                  <!-- Basic Information -->
                  <div class="grid grid-cols-1 gap-6 sm:grid-cols-2">
                    <div>
                      <label for="companyName" class="block text-sm font-medium leading-6 text-gray-900">
                        Company Name *
                      </label>
                      <div class="mt-2">
                        <input
                          id="companyName"
                          v-model="form.companyName"
                          type="text"
                          required
                          class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                        />
                      </div>
                    </div>

                    <div>
                      <label for="customerType" class="block text-sm font-medium leading-6 text-gray-900">
                        Customer Type
                      </label>
                      <div class="mt-2">
                        <select
                          id="customerType"
                          v-model="form.customerType"
                          class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                        >
                          <option value="individual">Individual</option>
                          <option value="business">Business</option>
                          <option value="enterprise">Enterprise</option>
                        </select>
                      </div>
                    </div>
                  </div>

                  <!-- Contact Information -->
                  <div class="grid grid-cols-1 gap-6 sm:grid-cols-2">
                    <div>
                      <label for="email" class="block text-sm font-medium leading-6 text-gray-900">
                        Email *
                      </label>
                      <div class="mt-2">
                        <input
                          id="email"
                          v-model="form.email"
                          type="email"
                          required
                          class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                        />
                      </div>
                    </div>

                    <div>
                      <label for="phone" class="block text-sm font-medium leading-6 text-gray-900">
                        Phone
                      </label>
                      <div class="mt-2">
                        <input
                          id="phone"
                          v-model="form.phone"
                          type="tel"
                          class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                        />
                      </div>
                    </div>
                  </div>

                  <!-- Address -->
                  <div>
                    <label for="address" class="block text-sm font-medium leading-6 text-gray-900">
                      Address
                    </label>
                    <div class="mt-2">
                      <textarea
                        id="address"
                        v-model="form.address"
                        rows="3"
                        class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                      />
                    </div>
                  </div>

                  <!-- Status and Tier -->
                  <div class="grid grid-cols-1 gap-6 sm:grid-cols-2">
                    <div>
                      <label for="status" class="block text-sm font-medium leading-6 text-gray-900">
                        Status
                      </label>
                      <div class="mt-2">
                        <select
                          id="status"
                          v-model="form.status"
                          class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                        >
                          <option value="active">Active</option>
                          <option value="inactive">Inactive</option>
                          <option value="suspended">Suspended</option>
                        </select>
                      </div>
                    </div>

                    <div>
                      <label for="tier" class="block text-sm font-medium leading-6 text-gray-900">
                        Customer Tier
                      </label>
                      <div class="mt-2">
                        <select
                          id="tier"
                          v-model="form.tier"
                          class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                        >
                          <option value="bronze">Bronze</option>
                          <option value="silver">Silver</option>
                          <option value="gold">Gold</option>
                          <option value="platinum">Platinum</option>
                        </select>
                      </div>
                    </div>
                  </div>

                  <!-- Notes -->
                  <div>
                    <label for="notes" class="block text-sm font-medium leading-6 text-gray-900">
                      Notes
                    </label>
                    <div class="mt-2">
                      <textarea
                        id="notes"
                        v-model="form.notes"
                        rows="3"
                        class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                        placeholder="Additional notes about this customer..."
                      />
                    </div>
                  </div>
                </div>

                <!-- Form Actions -->
                <div class="mt-6 flex justify-end space-x-3">
                  <button
                    type="button"
                    class="rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
                    @click="$emit('close')"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    :disabled="isSubmitting"
                    class="inline-flex justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 disabled:opacity-50"
                  >
                    <span v-if="isSubmitting" class="flex items-center">
                      <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      {{ customer ? 'Updating...' : 'Creating...' }}
                    </span>
                    <span v-else>
                      {{ customer ? 'Update Customer' : 'Create Customer' }}
                    </span>
                  </button>
                </div>
              </form>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue'
import { UserPlusIcon } from '@heroicons/vue/24/outline'

interface Customer {
  id: string
  companyName: string
  customerType: 'individual' | 'business' | 'enterprise'
  email: string
  phone?: string
  address?: string
  status: 'active' | 'inactive' | 'suspended'
  tier: 'bronze' | 'silver' | 'gold' | 'platinum'
  notes?: string
}

interface CustomerFormData {
  companyName: string
  customerType: 'individual' | 'business' | 'enterprise'
  email: string
  phone: string
  address: string
  status: 'active' | 'inactive' | 'suspended'
  tier: 'bronze' | 'silver' | 'gold' | 'platinum'
  notes: string
}

interface Props {
  isOpen: boolean
  customer?: Customer | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  save: [customer: CustomerFormData]
}>()

const isSubmitting = ref(false)

const form = reactive<CustomerFormData>({
  companyName: '',
  customerType: 'business',
  email: '',
  phone: '',
  address: '',
  status: 'active',
  tier: 'bronze',
  notes: ''
})

// Reset form when modal opens/closes or customer changes
watch([() => props.isOpen, () => props.customer], () => {
  if (props.isOpen) {
    if (props.customer) {
      // Edit mode - populate form with customer data
      Object.assign(form, {
        companyName: props.customer.companyName,
        customerType: props.customer.customerType,
        email: props.customer.email,
        phone: props.customer.phone || '',
        address: props.customer.address || '',
        status: props.customer.status,
        tier: props.customer.tier,
        notes: props.customer.notes || ''
      })
    } else {
      // Create mode - reset form
      Object.assign(form, {
        companyName: '',
        customerType: 'business',
        email: '',
        phone: '',
        address: '',
        status: 'active',
        tier: 'bronze',
        notes: ''
      })
    }
  }
})

const handleSubmit = async () => {
  isSubmitting.value = true
  
  try {
    // Validate required fields
    if (!form.companyName.trim() || !form.email.trim()) {
      throw new Error('Please fill in all required fields')
    }

    emit('save', { ...form })
  } catch (error) {
    console.error('Error saving customer:', error)
    // TODO: Show error notification
  } finally {
    isSubmitting.value = false
  }
}
</script>

<style scoped>
/* Add any specific styles if needed */
</style>
