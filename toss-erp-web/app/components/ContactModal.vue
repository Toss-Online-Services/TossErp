<template>
  <div class="fixed inset-0 z-50 overflow-y-auto">
    <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
      <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" @click="emit('close')"></div>
      
      <div class="relative transform overflow-hidden rounded-lg bg-white px-4 pb-4 pt-5 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg sm:p-6">
        <form @submit.prevent="handleSubmit">
          <div>
            <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-indigo-100">
              <UserIcon class="h-6 w-6 text-indigo-600" />
            </div>
            <div class="mt-3 text-center sm:mt-5">
              <h3 class="text-base font-semibold leading-6 text-gray-900">
                {{ isEditMode ? 'Edit Contact' : 'Add New Contact' }}
              </h3>
              <div class="mt-2">
                <p class="text-sm text-gray-500">
                  {{ isEditMode ? 'Update contact information' : 'Create a new contact in your CRM' }}
                </p>
              </div>
            </div>
          </div>

          <div class="mt-5 space-y-4">
            <!-- Name -->
            <div>
              <label for="name" class="block text-sm font-medium leading-6 text-gray-900">
                Name *
              </label>
              <div class="mt-2">
                <input
                  id="name"
                  v-model="form.name"
                  name="name"
                  type="text"
                  required
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  placeholder="Enter contact name"
                />
              </div>
            </div>

            <!-- Email -->
            <div>
              <label for="email" class="block text-sm font-medium leading-6 text-gray-900">
                Email *
              </label>
              <div class="mt-2">
                <input
                  id="email"
                  v-model="form.email"
                  name="email"
                  type="email"
                  required
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  placeholder="contact@example.com"
                />
              </div>
            </div>

            <!-- Phone -->
            <div>
              <label for="phone" class="block text-sm font-medium leading-6 text-gray-900">
                Phone
              </label>
              <div class="mt-2">
                <input
                  id="phone"
                  v-model="form.phone"
                  name="phone"
                  type="tel"
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  placeholder="+27 XX XXX XXXX"
                />
              </div>
            </div>

            <!-- Company -->
            <div>
              <label for="company" class="block text-sm font-medium leading-6 text-gray-900">
                Company
              </label>
              <div class="mt-2">
                <input
                  id="company"
                  v-model="form.company"
                  name="company"
                  type="text"
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  placeholder="Company name"
                />
              </div>
            </div>

            <!-- Type -->
            <div>
              <label for="type" class="block text-sm font-medium leading-6 text-gray-900">
                Type *
              </label>
              <div class="mt-2">
                <select
                  id="type"
                  v-model="form.type"
                  name="type"
                  required
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                >
                  <option value="">Select type</option>
                  <option value="lead">Lead</option>
                  <option value="customer">Customer</option>
                  <option value="vendor">Vendor</option>
                  <option value="partner">Partner</option>
                </select>
              </div>
            </div>

            <!-- Status -->
            <div>
              <label for="status" class="block text-sm font-medium leading-6 text-gray-900">
                Status *
              </label>
              <div class="mt-2">
                <select
                  id="status"
                  v-model="form.status"
                  name="status"
                  required
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                >
                  <option value="">Select status</option>
                  <option value="active">Active</option>
                  <option value="inactive">Inactive</option>
                  <option value="prospect">Prospect</option>
                </select>
              </div>
            </div>

            <!-- Industry -->
            <div>
              <label for="industry" class="block text-sm font-medium leading-6 text-gray-900">
                Industry
              </label>
              <div class="mt-2">
                <input
                  id="industry"
                  v-model="form.industry"
                  name="industry"
                  type="text"
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  placeholder="e.g., Technology, Healthcare"
                />
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
                  name="notes"
                  rows="3"
                  class="block w-full rounded-md border-0 py-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
                  placeholder="Additional notes about this contact..."
                />
              </div>
            </div>
          </div>

          <div class="mt-5 sm:mt-6 sm:grid sm:grid-flow-row-dense sm:grid-cols-2 sm:gap-3">
            <button
              type="submit"
              class="inline-flex w-full justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 sm:col-start-2"
            >
              {{ isEditMode ? 'Update Contact' : 'Create Contact' }}
            </button>
            <button
              type="button"
              @click="emit('close')"
              class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50 sm:col-start-1 sm:mt-0"
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { UserIcon } from '@heroicons/vue/24/outline'

interface Contact {
  id: string
  name: string
  email: string
  phone?: string
  company?: string
  type: 'lead' | 'customer' | 'vendor' | 'partner'
  status: 'active' | 'inactive' | 'prospect'
  createdAt: string
  lastActivity?: string
  industry?: string
  assignedTo?: string
  notes?: string
}

interface Props {
  contact?: Contact | null
  isEditMode: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'save', contact: Contact): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Form state
const form = ref({
  id: '',
  name: '',
  email: '',
  phone: '',
  company: '',
  type: '' as Contact['type'] | '',
  status: '' as Contact['status'] | '',
  industry: '',
  notes: '',
  createdAt: '',
  lastActivity: '',
  assignedTo: ''
})

// Initialize form when contact prop changes
watch(() => props.contact, (newContact) => {
  if (newContact) {
    form.value = {
      id: newContact.id,
      name: newContact.name,
      email: newContact.email,
      phone: newContact.phone || '',
      company: newContact.company || '',
      type: newContact.type,
      status: newContact.status,
      industry: newContact.industry || '',
      notes: newContact.notes || '',
      createdAt: newContact.createdAt,
      lastActivity: newContact.lastActivity || '',
      assignedTo: newContact.assignedTo || ''
    }
  } else {
    // Reset form for new contact
    form.value = {
      id: '',
      name: '',
      email: '',
      phone: '',
      company: '',
      type: '',
      status: '',
      industry: '',
      notes: '',
      createdAt: '',
      lastActivity: '',
      assignedTo: ''
    }
  }
}, { immediate: true })

const handleSubmit = () => {
  // Validate required fields
  if (!form.value.name || !form.value.email || !form.value.type || !form.value.status) {
    return
  }

  const contactData: Contact = {
    id: form.value.id,
    name: form.value.name,
    email: form.value.email,
    phone: form.value.phone || undefined,
    company: form.value.company || undefined,
    type: form.value.type as Contact['type'],
    status: form.value.status as Contact['status'],
    industry: form.value.industry || undefined,
    notes: form.value.notes || undefined,
    createdAt: form.value.createdAt,
    lastActivity: form.value.lastActivity || undefined,
    assignedTo: form.value.assignedTo || undefined
  }

  emit('save', contactData)
}
</script>
