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
            <DialogPanel class="w-full max-w-4xl transform overflow-hidden rounded-lg bg-white text-left align-middle shadow-xl transition-all">
              <!-- Header -->
              <div class="flex items-center justify-between p-6 border-b">
                <div class="flex items-center space-x-4">
                  <div class="flex-shrink-0">
                    <div class="w-12 h-12 bg-blue-500 rounded-full flex items-center justify-center text-white font-medium text-lg">
                      {{ getInitials(contact) }}
                    </div>
                  </div>
                  <div>
                    <DialogTitle as="h3" class="text-lg font-medium leading-6 text-gray-900">
                      {{ contact?.firstName }} {{ contact?.lastName }}
                    </DialogTitle>
                    <p class="text-sm text-gray-500">{{ contact?.jobTitle }} at {{ contact?.company }}</p>
                  </div>
                </div>
                <div class="flex items-center space-x-3">
                  <button
                    @click="editContact"
                    class="inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-blue-700 bg-blue-100 hover:bg-blue-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
                  >
                    Edit
                  </button>
                  <button
                    @click="closeModal"
                    class="text-gray-400 hover:text-gray-500 transition-colors"
                  >
                    <XMarkIcon class="h-6 w-6" />
                  </button>
                </div>
              </div>

              <!-- Content -->
              <div class="flex">
                <!-- Main Info Panel -->
                <div class="flex-1 p-6">
                  <div class="space-y-6">
                    <!-- Contact Information -->
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 mb-3">Contact Information</h4>
                      <dl class="grid grid-cols-1 gap-3">
                        <div>
                          <dt class="text-sm font-medium text-gray-500">Email</dt>
                          <dd class="text-sm text-gray-900">
                            <a :href="`mailto:${contact?.email}`" class="text-blue-600 hover:text-blue-500">
                              {{ contact?.email }}
                            </a>
                          </dd>
                        </div>
                        <div v-if="contact?.phone">
                          <dt class="text-sm font-medium text-gray-500">Phone</dt>
                          <dd class="text-sm text-gray-900">
                            <a :href="`tel:${contact?.phone}`" class="text-blue-600 hover:text-blue-500">
                              {{ contact?.phone }}
                            </a>
                          </dd>
                        </div>
                      </dl>
                    </div>

                    <!-- Company Information -->
                    <div v-if="contact?.company">
                      <h4 class="text-sm font-medium text-gray-900 mb-3">Company Information</h4>
                      <dl class="grid grid-cols-1 gap-3">
                        <div>
                          <dt class="text-sm font-medium text-gray-500">Company</dt>
                          <dd class="text-sm text-gray-900">{{ contact?.company }}</dd>
                        </div>
                        <div v-if="contact?.jobTitle">
                          <dt class="text-sm font-medium text-gray-500">Job Title</dt>
                          <dd class="text-sm text-gray-900">{{ contact?.jobTitle }}</dd>
                        </div>
                      </dl>
                    </div>

                    <!-- Classification -->
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 mb-3">Classification</h4>
                      <dl class="grid grid-cols-2 gap-3">
                        <div>
                          <dt class="text-sm font-medium text-gray-500">Type</dt>
                          <dd class="text-sm">
                            <span :class="getTypeClass(contact?.type)" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium">
                              {{ contact?.type }}
                            </span>
                          </dd>
                        </div>
                        <div>
                          <dt class="text-sm font-medium text-gray-500">Status</dt>
                          <dd class="text-sm">
                            <span :class="getStatusClass(contact?.status)" class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium">
                              {{ contact?.status }}
                            </span>
                          </dd>
                        </div>
                        <div v-if="contact?.source">
                          <dt class="text-sm font-medium text-gray-500">Source</dt>
                          <dd class="text-sm text-gray-900">{{ contact?.source }}</dd>
                        </div>
                      </dl>
                    </div>

                    <!-- Timeline -->
                    <div>
                      <h4 class="text-sm font-medium text-gray-900 mb-3">Timeline</h4>
                      <dl class="grid grid-cols-2 gap-3">
                        <div>
                          <dt class="text-sm font-medium text-gray-500">Created</dt>
                          <dd class="text-sm text-gray-900">{{ formatDate(contact?.createdAt) }}</dd>
                        </div>
                        <div>
                          <dt class="text-sm font-medium text-gray-500">Last Activity</dt>
                          <dd class="text-sm text-gray-900">{{ formatDate(contact?.lastActivity) }}</dd>
                        </div>
                      </dl>
                    </div>
                  </div>
                </div>

                <!-- Activity Panel -->
                <div class="w-80 border-l bg-gray-50 p-6">
                  <h4 class="text-sm font-medium text-gray-900 mb-4">Recent Activity</h4>
                  <div class="space-y-4">
                    <!-- Activity items would go here -->
                    <div class="text-sm text-gray-500 text-center py-8">
                      No recent activity
                    </div>
                  </div>
                  
                  <!-- Quick Actions -->
                  <div class="mt-6 pt-6 border-t border-gray-200">
                    <h5 class="text-sm font-medium text-gray-900 mb-3">Quick Actions</h5>
                    <div class="space-y-2">
                      <button class="w-full text-left text-sm text-blue-600 hover:text-blue-500">
                        + Add Note
                      </button>
                      <button class="w-full text-left text-sm text-blue-600 hover:text-blue-500">
                        + Schedule Call
                      </button>
                      <button class="w-full text-left text-sm text-blue-600 hover:text-blue-500">
                        + Send Email
                      </button>
                      <button class="w-full text-left text-sm text-blue-600 hover:text-blue-500">
                        + Create Opportunity
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
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
}

const props = defineProps<Props>()

// Emits
const emit = defineEmits<{
  close: []
  edit: [contact: any]
}>()

// Methods
const closeModal = () => {
  emit('close')
}

const editContact = () => {
  emit('edit', props.contact)
  closeModal()
}

const getInitials = (contact: any) => {
  if (!contact) return ''
  const firstName = contact.firstName || ''
  const lastName = contact.lastName || ''
  return (firstName.charAt(0) + lastName.charAt(0)).toUpperCase()
}

const getTypeClass = (type: string) => {
  const classes = {
    'Customer': 'bg-green-100 text-green-800',
    'Lead': 'bg-blue-100 text-blue-800',
    'Prospect': 'bg-yellow-100 text-yellow-800'
  }
  return classes[type as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getStatusClass = (status: string) => {
  const classes = {
    'Active': 'bg-green-100 text-green-800',
    'Inactive': 'bg-gray-100 text-gray-800',
    'Qualified': 'bg-blue-100 text-blue-800',
    'Unqualified': 'bg-red-100 text-red-800'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string) => {
  if (!dateString) return 'N/A'
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>
