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
            <DialogPanel class="relative transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-4xl">
              <div class="bg-white px-6 py-6">
                <!-- Header -->
                <div class="flex items-center justify-between border-b border-gray-200 pb-4">
                  <div>
                    <DialogTitle as="h3" class="text-lg font-semibold leading-6 text-gray-900">
                      Customer Details
                    </DialogTitle>
                    <p class="mt-1 text-sm text-gray-500">
                      Complete information for {{ customer?.companyName }}
                    </p>
                  </div>
                  <button
                    type="button"
                    class="rounded-md bg-white text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                    @click="$emit('close')"
                  >
                    <span class="sr-only">Close</span>
                    <XMarkIcon class="h-6 w-6" aria-hidden="true" />
                  </button>
                </div>

                <!-- Content -->
                <div v-if="customer" class="mt-6">
                  <!-- Quick Actions -->
                  <div class="mb-6 flex flex-wrap gap-3">
                    <button
                      type="button"
                      class="inline-flex items-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
                      @click="$emit('edit', customer)"
                    >
                      <PencilIcon class="-ml-0.5 mr-1.5 h-4 w-4" aria-hidden="true" />
                      Edit Customer
                    </button>
                    <button
                      type="button"
                      class="inline-flex items-center rounded-md bg-green-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-green-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-green-600"
                      @click="$emit('manage-subscription', customer)"
                    >
                      <CreditCardIcon class="-ml-0.5 mr-1.5 h-4 w-4" aria-hidden="true" />
                      Manage Subscription
                    </button>
                    <button
                      type="button"
                      class="inline-flex items-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
                    >
                      <EnvelopeIcon class="-ml-0.5 mr-1.5 h-4 w-4" aria-hidden="true" />
                      Send Email
                    </button>
                  </div>

                  <!-- Main Content Grid -->
                  <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
                    <!-- Left Column - Basic Info -->
                    <div class="lg:col-span-2">
                      <!-- Basic Information -->
                      <div class="bg-white shadow sm:rounded-lg">
                        <div class="px-4 py-5 sm:p-6">
                          <h3 class="text-lg font-medium leading-6 text-gray-900">Basic Information</h3>
                          <div class="mt-5 border-t border-gray-200">
                            <dl class="divide-y divide-gray-200">
                              <div class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Company Name</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">{{ customer.companyName }}</dd>
                              </div>
                              <div class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Customer Type</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  <span class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
                                        :class="getCustomerTypeClass(customer.customerType)">
                                    {{ customer.customerType.charAt(0).toUpperCase() + customer.customerType.slice(1) }}
                                  </span>
                                </dd>
                              </div>
                              <div class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Email</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  <a :href="`mailto:${customer.email}`" class="text-indigo-600 hover:text-indigo-500">
                                    {{ customer.email }}
                                  </a>
                                </dd>
                              </div>
                              <div v-if="customer.phone" class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Phone</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  <a :href="`tel:${customer.phone}`" class="text-indigo-600 hover:text-indigo-500">
                                    {{ customer.phone }}
                                  </a>
                                </dd>
                              </div>
                              <div v-if="customer.address" class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Address</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">{{ customer.address }}</dd>
                              </div>
                              <div class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Status</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  <span class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
                                        :class="getStatusClass(customer.status)">
                                    {{ customer.status.charAt(0).toUpperCase() + customer.status.slice(1) }}
                                  </span>
                                </dd>
                              </div>
                              <div class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Customer Tier</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  <span class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
                                        :class="getTierClass(customer.tier)">
                                    {{ customer.tier.charAt(0).toUpperCase() + customer.tier.slice(1) }}
                                  </span>
                                </dd>
                              </div>
                            </dl>
                          </div>
                        </div>
                      </div>

                      <!-- Subscription Information -->
                      <div class="mt-6 bg-white shadow sm:rounded-lg">
                        <div class="px-4 py-5 sm:p-6">
                          <h3 class="text-lg font-medium leading-6 text-gray-900">Subscription Information</h3>
                          <div class="mt-5 border-t border-gray-200">
                            <dl class="divide-y divide-gray-200">
                              <div class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Subscription Status</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  <span class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
                                        :class="getSubscriptionStatusClass(customer.subscriptionStatus)">
                                    {{ customer.subscriptionStatus ? (customer.subscriptionStatus.charAt(0).toUpperCase() + customer.subscriptionStatus.slice(1)) : 'No Subscription' }}
                                  </span>
                                </dd>
                              </div>
                              <div v-if="customer.subscriptionStartDate" class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Start Date</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  {{ formatDate(customer.subscriptionStartDate) }}
                                </dd>
                              </div>
                              <div v-if="customer.subscriptionEndDate" class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">End Date</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  {{ formatDate(customer.subscriptionEndDate) }}
                                </dd>
                              </div>
                              <div v-if="customer.monthlyRevenue" class="py-4 sm:grid sm:grid-cols-3 sm:gap-4 sm:py-5">
                                <dt class="text-sm font-medium text-gray-500">Monthly Revenue</dt>
                                <dd class="mt-1 text-sm text-gray-900 sm:col-span-2 sm:mt-0">
                                  ${{ customer.monthlyRevenue.toLocaleString() }}
                                </dd>
                              </div>
                            </dl>
                          </div>
                        </div>
                      </div>

                      <!-- Notes -->
                      <div v-if="customer.notes" class="mt-6 bg-white shadow sm:rounded-lg">
                        <div class="px-4 py-5 sm:p-6">
                          <h3 class="text-lg font-medium leading-6 text-gray-900">Notes</h3>
                          <div class="mt-3">
                            <p class="text-sm text-gray-500">{{ customer.notes }}</p>
                          </div>
                        </div>
                      </div>
                    </div>

                    <!-- Right Column - Stats & Activity -->
                    <div class="space-y-6">
                      <!-- Key Metrics -->
                      <div class="bg-white shadow sm:rounded-lg">
                        <div class="px-4 py-5 sm:p-6">
                          <h3 class="text-lg font-medium leading-6 text-gray-900">Key Metrics</h3>
                          <div class="mt-5 space-y-4">
                            <div class="flex justify-between">
                              <span class="text-sm font-medium text-gray-500">Total Revenue</span>
                              <span class="text-sm text-gray-900">${{ customer.totalRevenue?.toLocaleString() || '0' }}</span>
                            </div>
                            <div class="flex justify-between">
                              <span class="text-sm font-medium text-gray-500">Active Contacts</span>
                              <span class="text-sm text-gray-900">{{ customer.contactCount || 0 }}</span>
                            </div>
                            <div class="flex justify-between">
                              <span class="text-sm font-medium text-gray-500">Customer Since</span>
                              <span class="text-sm text-gray-900">{{ formatDate(customer.createdAt) }}</span>
                            </div>
                            <div class="flex justify-between">
                              <span class="text-sm font-medium text-gray-500">Last Updated</span>
                              <span class="text-sm text-gray-900">{{ formatDate(customer.updatedAt) }}</span>
                            </div>
                          </div>
                        </div>
                      </div>

                      <!-- Recent Activity -->
                      <div class="bg-white shadow sm:rounded-lg">
                        <div class="px-4 py-5 sm:p-6">
                          <h3 class="text-lg font-medium leading-6 text-gray-900">Recent Activity</h3>
                          <div class="mt-5">
                            <div class="flow-root">
                              <ul role="list" class="-mb-8">
                                <li v-for="(activity, activityIdx) in customer.recentActivity" :key="activityIdx">
                                  <div class="relative pb-8">
                                    <span v-if="activityIdx !== (customer.recentActivity?.length || 0) - 1" class="absolute left-4 top-4 -ml-px h-full w-0.5 bg-gray-200" aria-hidden="true" />
                                    <div class="relative flex space-x-3">
                                      <div>
                                        <span class="bg-gray-400 flex h-8 w-8 items-center justify-center rounded-full ring-8 ring-white">
                                          <UserIcon class="h-4 w-4 text-white" aria-hidden="true" />
                                        </span>
                                      </div>
                                      <div class="flex min-w-0 flex-1 justify-between space-x-4 pt-1.5">
                                        <div>
                                          <p class="text-sm text-gray-500">{{ activity.description }}</p>
                                        </div>
                                        <div class="whitespace-nowrap text-right text-sm text-gray-500">
                                          <time :datetime="activity.date">{{ formatRelativeDate(activity.date) }}</time>
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                </li>
                              </ul>
                            </div>
                            <div v-if="!customer.recentActivity?.length" class="text-center py-4">
                              <p class="text-sm text-gray-500">No recent activity</p>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Loading State -->
                <div v-else class="flex justify-center py-12">
                  <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
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
import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue'
import { 
  XMarkIcon, 
  PencilIcon, 
  CreditCardIcon, 
  EnvelopeIcon,
  UserIcon
} from '@heroicons/vue/24/outline'

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
  subscriptionStatus?: 'trial' | 'active' | 'suspended' | 'cancelled'
  subscriptionStartDate?: string
  subscriptionEndDate?: string
  monthlyRevenue?: number
  totalRevenue?: number
  contactCount?: number
  createdAt: string
  updatedAt: string
  recentActivity?: Array<{
    description: string
    date: string
  }>
}

interface Props {
  isOpen: boolean
  customer?: Customer | null
}

defineProps<Props>()

const emit = defineEmits<{
  close: []
  edit: [customer: Customer]
  'manage-subscription': [customer: Customer]
}>()

const getCustomerTypeClass = (type: string) => {
  const classes = {
    individual: 'bg-blue-100 text-blue-800',
    business: 'bg-green-100 text-green-800',
    enterprise: 'bg-purple-100 text-purple-800'
  }
  return classes[type as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getStatusClass = (status: string) => {
  const classes = {
    active: 'bg-green-100 text-green-800',
    inactive: 'bg-gray-100 text-gray-800',
    suspended: 'bg-red-100 text-red-800'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getTierClass = (tier: string) => {
  const classes = {
    bronze: 'bg-orange-100 text-orange-800',
    silver: 'bg-gray-100 text-gray-800',
    gold: 'bg-yellow-100 text-yellow-800',
    platinum: 'bg-indigo-100 text-indigo-800'
  }
  return classes[tier as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getSubscriptionStatusClass = (status?: string) => {
  const classes = {
    trial: 'bg-blue-100 text-blue-800',
    active: 'bg-green-100 text-green-800',
    suspended: 'bg-yellow-100 text-yellow-800',
    cancelled: 'bg-red-100 text-red-800'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const formatRelativeDate = (dateString: string) => {
  const date = new Date(dateString)
  const now = new Date()
  const diffInDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24))
  
  if (diffInDays === 0) return 'Today'
  if (diffInDays === 1) return 'Yesterday'
  if (diffInDays < 7) return `${diffInDays} days ago`
  if (diffInDays < 30) return `${Math.floor(diffInDays / 7)} weeks ago`
  return formatDate(dateString)
}
</script>

<style scoped>
/* Add any specific styles if needed */
</style>
