<template>
  <TransitionRoot appear :show="isOpen" as="template">
    <Dialog as="div" @close="closeModal" class="relative z-50">
      <TransitionChild
        as="template"
        enter="duration-300 ease-out"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="duration-200 ease-in"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-black bg-opacity-50" />
      </TransitionChild>

      <div class="fixed inset-0 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4 text-center">
          <TransitionChild
            as="template"
            enter="duration-300 ease-out"
            enter-from="opacity-0 scale-95"
            enter-to="opacity-100 scale-100"
            leave="duration-200 ease-in"
            leave-from="opacity-100 scale-100"
            leave-to="opacity-0 scale-95"
          >
            <DialogPanel class="w-full max-w-2xl transform overflow-hidden rounded-lg bg-white text-left align-middle shadow-xl transition-all">
              <div class="flex items-center justify-between p-6 border-b">
                <DialogTitle as="h3" class="text-lg font-medium leading-6 text-gray-900">
                  {{ isEditMode ? 'Edit Contact' : 'Create New Contact' }}
                </DialogTitle>
                <button
                  @click="closeModal"
                  class="text-gray-400 hover:text-gray-500 transition-colors"
                >
                  <XMarkIcon class="h-6 w-6" />
                </button>
              </div>

              <form @submit.prevent="submitForm" class="p-6 space-y-6">
                <!-- Personal Information -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label for="firstName" class="block text-sm font-medium text-gray-700 mb-1">
                      First Name *
                    </label>
                    <input
                      id="firstName"
                      v-model="form.firstName"
                      type="text"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter first name"
                    />
                  </div>
                  
                  <div>
                    <label for="lastName" class="block text-sm font-medium text-gray-700 mb-1">
                      Last Name *
                    </label>
                    <input
                      id="lastName"
                      v-model="form.lastName"
                      type="text"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter last name"
                    />
                  </div>
                </div>

                <!-- Contact Information -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
                      Email *
                    </label>
                    <input
                      id="email"
                      v-model="form.email"
                      type="email"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter email address"
                    />
                  </div>
                  
                  <div>
                    <label for="phone" class="block text-sm font-medium text-gray-700 mb-1">
                      Phone
                    </label>
                    <input
                      id="phone"
                      v-model="form.phone"
                      type="tel"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter phone number"
                    />
                  </div>
                </div>

                <!-- Company Information -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label for="company" class="block text-sm font-medium text-gray-700 mb-1">
                      Company
                    </label>
                    <input
                      id="company"
                      v-model="form.company"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter company name"
                    />
                  </div>
                  
                  <div>
                    <label for="jobTitle" class="block text-sm font-medium text-gray-700 mb-1">
                      Job Title
                    </label>
                    <input
                      id="jobTitle"
                      v-model="form.jobTitle"
                      type="text"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      placeholder="Enter job title"
                    />
                  </div>
                </div>

                <!-- Classification -->
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div>
                    <label for="type" class="block text-sm font-medium text-gray-700 mb-1">
                      Type *
                    </label>
                    <select
                      id="type"
                      v-model="form.type"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    >
                      <option value="">Select type</option>
                      <option value="Lead">Lead</option>
                      <option value="Prospect">Prospect</option>
                      <option value="Customer">Customer</option>
                    </select>
                  </div>
                  
                  <div>
                    <label for="status" class="block text-sm font-medium text-gray-700 mb-1">
                      Status *
                    </label>
                    <select
                      id="status"
                      v-model="form.status"
                      required
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    >
                      <option value="">Select status</option>
                      <option value="Active">Active</option>
                      <option value="Inactive">Inactive</option>
                      <option value="Qualified">Qualified</option>
                      <option value="Unqualified">Unqualified</option>
                    </select>
                  </div>
                  
                  <div>
                    <label for="source" class="block text-sm font-medium text-gray-700 mb-1">
                      Source
                    </label>
                    <select
                      id="source"
                      v-model="form.source"
                      class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    >
                      <option value="">Select source</option>
                      <option value="Website">Website</option>
                      <option value="Referral">Referral</option>
                      <option value="LinkedIn">LinkedIn</option>
                      <option value="Trade Show">Trade Show</option>
                      <option value="Cold Call">Cold Call</option>
                      <option value="Email Campaign">Email Campaign</option>
                    </select>
                  </div>
                </div>

                <!-- Form Actions -->
                <div class="flex justify-end space-x-3 pt-6 border-t">
                  <button
                    type="button"
                    @click="closeModal"
                    class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    :disabled="loading"
                    class="px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                  >
                    <span v-if="loading">{{ isEditMode ? 'Updating...' : 'Creating...' }}</span>
                    <span v-else>{{ isEditMode ? 'Update Contact' : 'Create Contact' }}</span>
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
import { ref, watch } from 'vue'
import {
  Dialog,
  DialogPanel,
  DialogTitle,
  TransitionChild,
  TransitionRoot,
} from '@headlessui/vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'

// Props
interface Props {
  isOpen: boolean
  contact?: any
  isEditMode?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isEditMode: false
})

// Emits
const emit = defineEmits<{
  close: []
  save: [contact: any]
}>()

// Form state
const loading = ref(false)
const form = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  company: '',
  jobTitle: '',
  type: '',
  status: '',
  source: ''
})

// Watch for contact prop changes
watch(() => props.contact, (newContact) => {
  if (newContact && props.isEditMode) {
    form.value = {
      firstName: newContact.firstName || '',
      lastName: newContact.lastName || '',
      email: newContact.email || '',
      phone: newContact.phone || '',
      company: newContact.company || '',
      jobTitle: newContact.jobTitle || '',
      type: newContact.type || '',
      status: newContact.status || '',
      source: newContact.source || ''
    }
  }
}, { immediate: true })

// Watch for modal open/close to reset form
watch(() => props.isOpen, (isOpen) => {
  if (isOpen && !props.isEditMode) {
    // Reset form for new contact
    form.value = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      company: '',
      jobTitle: '',
      type: '',
      status: '',
      source: ''
    }
  }
})

// Methods
const closeModal = () => {
  emit('close')
}

const submitForm = async () => {
  loading.value = true
  
  try {
    // Create contact object
    const contactData = {
      ...form.value,
      id: props.isEditMode ? props.contact?.id : undefined
    }
    
    emit('save', contactData)
  } catch (error) {
    console.error('Error saving contact:', error)
  } finally {
    loading.value = false
  }
}
</script>
