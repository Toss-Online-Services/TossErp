<template>
  <div class="fixed inset-0 z-50 overflow-y-auto">
    <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
      <div class="fixed inset-0 bg-gray-500 bg-opacity-75 transition-opacity" @click="emit('close')"></div>
      
      <div class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-2xl">
        <!-- Header -->
        <div class="bg-white px-4 pb-4 pt-5 sm:p-6 sm:pb-4">
          <div class="sm:flex sm:items-start">
            <div class="mx-auto flex h-12 w-12 flex-shrink-0 items-center justify-center rounded-full bg-indigo-100 sm:mx-0 sm:h-10 sm:w-10">
              <UserIcon class="h-6 w-6 text-indigo-600" />
            </div>
            <div class="mt-3 text-center sm:ml-4 sm:mt-0 sm:text-left flex-1">
              <h3 class="text-base font-semibold leading-6 text-gray-900">
                Contact Details
              </h3>
              <div class="mt-2">
                <p class="text-sm text-gray-500">
                  View and manage contact information
                </p>
              </div>
            </div>
            <div class="mt-3 sm:mt-0 sm:ml-4">
              <button
                @click="emit('edit', contact)"
                class="inline-flex items-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
              >
                <PencilIcon class="w-4 h-4 mr-2" />
                Edit
              </button>
            </div>
          </div>
        </div>

        <!-- Content -->
        <div class="px-4 pb-4 sm:px-6 sm:pb-6">
          <!-- Contact Header -->
          <div class="mb-6 flex items-center space-x-4">
            <div class="h-16 w-16 flex-shrink-0">
              <div class="h-16 w-16 rounded-full bg-indigo-600 flex items-center justify-center">
                <span class="text-xl font-bold text-white">
                  {{ getInitials(contact.name) }}
                </span>
              </div>
            </div>
            <div class="flex-1">
              <h2 class="text-xl font-bold text-gray-900">{{ contact.name }}</h2>
              <p class="text-gray-600">{{ contact.company || 'No company' }}</p>
              <div class="mt-2 flex items-center space-x-4">
                <span :class="['inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset', getTypeColor(contact.type)]">
                  {{ contact.type }}
                </span>
                <span :class="['inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset', getStatusColor(contact.status)]">
                  {{ contact.status }}
                </span>
              </div>
            </div>
          </div>

          <!-- Contact Information Grid -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Basic Information -->
            <div class="space-y-4">
              <h3 class="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
                Contact Information
              </h3>
              
              <div>
                <dt class="text-sm font-medium text-gray-500">Email</dt>
                <dd class="mt-1 text-sm text-gray-900">
                  <a :href="`mailto:${contact.email}`" class="text-indigo-600 hover:text-indigo-500">
                    {{ contact.email }}
                  </a>
                </dd>
              </div>

              <div v-if="contact.phone">
                <dt class="text-sm font-medium text-gray-500">Phone</dt>
                <dd class="mt-1 text-sm text-gray-900">
                  <a :href="`tel:${contact.phone}`" class="text-indigo-600 hover:text-indigo-500">
                    {{ contact.phone }}
                  </a>
                </dd>
              </div>

              <div v-if="contact.company">
                <dt class="text-sm font-medium text-gray-500">Company</dt>
                <dd class="mt-1 text-sm text-gray-900">{{ contact.company }}</dd>
              </div>

              <div v-if="contact.industry">
                <dt class="text-sm font-medium text-gray-500">Industry</dt>
                <dd class="mt-1 text-sm text-gray-900">{{ contact.industry }}</dd>
              </div>

              <div v-if="contact.assignedTo">
                <dt class="text-sm font-medium text-gray-500">Assigned To</dt>
                <dd class="mt-1 text-sm text-gray-900">{{ contact.assignedTo }}</dd>
              </div>
            </div>

            <!-- Activity Information -->
            <div class="space-y-4">
              <h3 class="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2">
                Activity Information
              </h3>

              <div>
                <dt class="text-sm font-medium text-gray-500">Created</dt>
                <dd class="mt-1 text-sm text-gray-900">{{ formatDate(contact.createdAt) }}</dd>
              </div>

              <div>
                <dt class="text-sm font-medium text-gray-500">Last Activity</dt>
                <dd class="mt-1 text-sm text-gray-900">{{ formatDate(contact.lastActivity) }}</dd>
              </div>

              <div>
                <dt class="text-sm font-medium text-gray-500">Status</dt>
                <dd class="mt-1">
                  <span :class="['inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset', getStatusColor(contact.status)]">
                    {{ contact.status }}
                  </span>
                </dd>
              </div>

              <div>
                <dt class="text-sm font-medium text-gray-500">Type</dt>
                <dd class="mt-1">
                  <span :class="['inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset', getTypeColor(contact.type)]">
                    {{ contact.type }}
                  </span>
                </dd>
              </div>
            </div>
          </div>

          <!-- Notes Section -->
          <div v-if="contact.notes" class="mt-6">
            <h3 class="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2 mb-4">
              Notes
            </h3>
            <div class="bg-gray-50 rounded-lg p-4">
              <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ contact.notes }}</p>
            </div>
          </div>

          <!-- Recent Activities Placeholder -->
          <div class="mt-6">
            <h3 class="text-lg font-medium text-gray-900 border-b border-gray-200 pb-2 mb-4">
              Recent Activities
            </h3>
            <div class="bg-gray-50 rounded-lg p-8 text-center">
              <CalendarIcon class="mx-auto h-12 w-12 text-gray-400" />
              <h3 class="mt-2 text-sm font-semibold text-gray-900">No recent activities</h3>
              <p class="mt-1 text-sm text-gray-500">Activities and interactions will appear here.</p>
              <div class="mt-6">
                <button
                  type="button"
                  class="inline-flex items-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
                >
                  <PlusIcon class="w-4 h-4 mr-2" />
                  Add Activity
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer Actions -->
        <div class="bg-gray-50 px-4 py-3 sm:flex sm:flex-row-reverse sm:px-6">
          <button
            @click="emit('edit', contact)"
            type="button"
            class="inline-flex w-full justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 sm:ml-3 sm:w-auto"
          >
            <PencilIcon class="w-4 h-4 mr-2" />
            Edit Contact
          </button>
          <button
            @click="emit('close')"
            type="button"
            class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50 sm:mt-0 sm:w-auto"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { UserIcon, PencilIcon, CalendarIcon, PlusIcon } from '@heroicons/vue/24/outline'

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
  contact: Contact
}

interface Emits {
  (e: 'close'): void
  (e: 'edit', contact: Contact): void
}

defineProps<Props>()
const emit = defineEmits<Emits>()

// Helper functions
const getInitials = (name: string): string => {
  return name
    .split(' ')
    .map(word => word.charAt(0))
    .join('')
    .toUpperCase()
    .slice(0, 2)
}

const getTypeColor = (type: string): string => {
  const colors = {
    lead: 'bg-yellow-50 text-yellow-800 ring-yellow-600/20',
    customer: 'bg-green-50 text-green-800 ring-green-600/20',
    vendor: 'bg-blue-50 text-blue-800 ring-blue-600/20',
    partner: 'bg-purple-50 text-purple-800 ring-purple-600/20'
  }
  return colors[type as keyof typeof colors] || 'bg-gray-50 text-gray-800 ring-gray-600/20'
}

const getStatusColor = (status: string): string => {
  const colors = {
    active: 'bg-green-50 text-green-800 ring-green-600/20',
    inactive: 'bg-gray-50 text-gray-800 ring-gray-600/20',
    prospect: 'bg-yellow-50 text-yellow-800 ring-yellow-600/20'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-50 text-gray-800 ring-gray-600/20'
}

const formatDate = (dateString: string | undefined): string => {
  if (!dateString) return 'Never'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>
