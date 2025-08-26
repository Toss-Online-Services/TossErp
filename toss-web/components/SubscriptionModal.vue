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
            <DialogPanel class="relative transform overflow-hidden rounded-lg bg-white px-4 pb-4 pt-5 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-3xl sm:p-6">
              <div>
                <div class="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-green-100">
                  <CreditCardIcon class="h-6 w-6 text-green-600" aria-hidden="true" />
                </div>
                <div class="mt-3 text-center sm:mt-5">
                  <DialogTitle as="h3" class="text-base font-semibold leading-6 text-gray-900">
                    Manage Subscription - {{ customer?.companyName }}
                  </DialogTitle>
                  <div class="mt-2">
                    <p class="text-sm text-gray-500">
                      Manage subscription plans, billing, and settings for this customer.
                    </p>
                  </div>
                </div>
              </div>

              <div v-if="customer" class="mt-6">
                <!-- Current Subscription Status -->
                <div class="rounded-md bg-blue-50 p-4">
                  <div class="flex">
                    <div class="flex-shrink-0">
                      <InformationCircleIcon class="h-5 w-5 text-blue-400" aria-hidden="true" />
                    </div>
                    <div class="ml-3 flex-1 md:flex md:justify-between">
                      <p class="text-sm text-blue-700">
                        Current Status: 
                        <span class="font-medium">
                          {{ customer.subscriptionStatus ? (customer.subscriptionStatus.charAt(0).toUpperCase() + customer.subscriptionStatus.slice(1)) : 'No Subscription' }}
                        </span>
                      </p>
                      <p v-if="customer.subscriptionEndDate" class="mt-3 text-sm md:ml-6 md:mt-0">
                        <span class="text-blue-700">
                          Expires: {{ formatDate(customer.subscriptionEndDate) }}
                        </span>
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Subscription Plans -->
                <div class="mt-6">
                  <h4 class="text-lg font-medium text-gray-900">Available Plans</h4>
                  <div class="mt-4 grid grid-cols-1 gap-4 sm:grid-cols-3">
                    <div
                      v-for="plan in subscriptionPlans"
                      :key="plan.id"
                      class="relative rounded-lg border border-gray-300 bg-white px-6 py-5 shadow-sm focus-within:ring-2 focus-within:ring-indigo-500 focus-within:ring-offset-2 hover:border-gray-400"
                      :class="{ 'ring-2 ring-indigo-500 border-indigo-500': selectedPlan?.id === plan.id }"
                    >
                      <div class="flex justify-between">
                        <div class="flex-1">
                          <label :for="`plan-${plan.id}`" class="cursor-pointer">
                            <div class="flex items-center">
                              <input
                                :id="`plan-${plan.id}`"
                                v-model="selectedPlan"
                                :value="plan"
                                name="subscription-plan"
                                type="radio"
                                class="h-4 w-4 border-gray-300 text-indigo-600 focus:ring-indigo-500"
                              />
                              <div class="ml-3">
                                <p class="text-sm font-medium text-gray-900">{{ plan.name }}</p>
                                <p class="text-sm text-gray-500">${{ plan.price }}/month</p>
                              </div>
                            </div>
                            <div class="mt-2">
                              <p class="text-xs text-gray-500">{{ plan.description }}</p>
                              <ul class="mt-2 text-xs text-gray-500">
                                <li v-for="feature in plan.features" :key="feature" class="flex items-center">
                                  <CheckIcon class="h-3 w-3 text-green-500 mr-1" />
                                  {{ feature }}
                                </li>
                              </ul>
                            </div>
                          </label>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Subscription Actions -->
                <div v-if="customer.subscriptionStatus" class="mt-6">
                  <h4 class="text-lg font-medium text-gray-900">Subscription Actions</h4>
                  <div class="mt-4 grid grid-cols-1 gap-3 sm:grid-cols-4">
                    <!-- Start Trial -->
                    <button
                      v-if="!customer.subscriptionStatus || customer.subscriptionStatus === 'cancelled'"
                      type="button"
                      class="inline-flex justify-center items-center rounded-md border border-transparent bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                      @click="handleAction('start-trial')"
                    >
                      Start Trial
                    </button>

                    <!-- Activate Subscription -->
                    <button
                      v-if="customer.subscriptionStatus === 'trial' || customer.subscriptionStatus === 'suspended'"
                      type="button"
                      class="inline-flex justify-center items-center rounded-md border border-transparent bg-green-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2"
                      @click="handleAction('activate')"
                    >
                      Activate
                    </button>

                    <!-- Suspend Subscription -->
                    <button
                      v-if="customer.subscriptionStatus === 'active'"
                      type="button"
                      class="inline-flex justify-center items-center rounded-md border border-transparent bg-yellow-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-yellow-700 focus:outline-none focus:ring-2 focus:ring-yellow-500 focus:ring-offset-2"
                      @click="handleAction('suspend')"
                    >
                      Suspend
                    </button>

                    <!-- Cancel Subscription -->
                    <button
                      v-if="customer.subscriptionStatus === 'active' || customer.subscriptionStatus === 'trial' || customer.subscriptionStatus === 'suspended'"
                      type="button"
                      class="inline-flex justify-center items-center rounded-md border border-transparent bg-red-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2"
                      @click="handleAction('cancel')"
                    >
                      Cancel
                    </button>

                    <!-- Renew Subscription -->
                    <button
                      v-if="customer.subscriptionStatus === 'active'"
                      type="button"
                      class="inline-flex justify-center items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                      @click="handleAction('renew')"
                    >
                      Renew
                    </button>
                  </div>
                </div>

                <!-- Billing Information -->
                <div class="mt-6">
                  <h4 class="text-lg font-medium text-gray-900">Billing Information</h4>
                  <div class="mt-4 bg-gray-50 rounded-lg p-4">
                    <dl class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                      <div>
                        <dt class="text-sm font-medium text-gray-500">Monthly Revenue</dt>
                        <dd class="mt-1 text-sm text-gray-900">${{ customer.monthlyRevenue?.toLocaleString() || '0' }}</dd>
                      </div>
                      <div>
                        <dt class="text-sm font-medium text-gray-500">Total Revenue</dt>
                        <dd class="mt-1 text-sm text-gray-900">${{ customer.totalRevenue?.toLocaleString() || '0' }}</dd>
                      </div>
                      <div>
                        <dt class="text-sm font-medium text-gray-500">Subscription Start</dt>
                        <dd class="mt-1 text-sm text-gray-900">
                          {{ customer.subscriptionStartDate ? formatDate(customer.subscriptionStartDate) : 'Not started' }}
                        </dd>
                      </div>
                      <div>
                        <dt class="text-sm font-medium text-gray-500">Next Billing Date</dt>
                        <dd class="mt-1 text-sm text-gray-900">
                          {{ customer.subscriptionEndDate ? formatDate(customer.subscriptionEndDate) : 'N/A' }}
                        </dd>
                      </div>
                    </dl>
                  </div>
                </div>

                <!-- Action Confirmation -->
                <div v-if="pendingAction" class="mt-6 rounded-md bg-yellow-50 p-4">
                  <div class="flex">
                    <div class="flex-shrink-0">
                      <ExclamationTriangleIcon class="h-5 w-5 text-yellow-400" aria-hidden="true" />
                    </div>
                    <div class="ml-3">
                      <h3 class="text-sm font-medium text-yellow-800">
                        Confirm Action
                      </h3>
                      <div class="mt-2 text-sm text-yellow-700">
                        <p>{{ getActionConfirmationMessage(pendingAction) }}</p>
                      </div>
                      <div class="mt-4">
                        <div class="-mx-2 -my-1.5 flex">
                          <button
                            type="button"
                            class="rounded-md bg-yellow-50 px-2 py-1.5 text-sm font-medium text-yellow-800 hover:bg-yellow-100 focus:outline-none focus:ring-2 focus:ring-yellow-600 focus:ring-offset-2 focus:ring-offset-yellow-50"
                            @click="confirmAction"
                          >
                            Confirm
                          </button>
                          <button
                            type="button"
                            class="ml-3 rounded-md bg-yellow-50 px-2 py-1.5 text-sm font-medium text-yellow-800 hover:bg-yellow-100 focus:outline-none focus:ring-2 focus:ring-yellow-600 focus:ring-offset-2 focus:ring-offset-yellow-50"
                            @click="pendingAction = null"
                          >
                            Cancel
                          </button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Modal Actions -->
              <div class="mt-6 flex justify-end space-x-3">
                <button
                  type="button"
                  class="rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
                  @click="$emit('close')"
                >
                  Close
                </button>
                <button
                  v-if="selectedPlan && (!customer?.subscriptionStatus || customer.subscriptionStatus === 'cancelled')"
                  type="button"
                  :disabled="isProcessing"
                  class="inline-flex justify-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600 disabled:opacity-50"
                  @click="handlePlanChange"
                >
                  <span v-if="isProcessing" class="flex items-center">
                    <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Processing...
                  </span>
                  <span v-else>
                    Subscribe to {{ selectedPlan.name }}
                  </span>
                </button>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue'
import { 
  CreditCardIcon, 
  InformationCircleIcon, 
  CheckIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline'

interface Customer {
  id: string
  companyName: string
  subscriptionStatus?: 'trial' | 'active' | 'suspended' | 'cancelled'
  subscriptionStartDate?: string
  subscriptionEndDate?: string
  monthlyRevenue?: number
  totalRevenue?: number
}

interface SubscriptionPlan {
  id: string
  name: string
  price: number
  description: string
  features: string[]
}

interface Props {
  isOpen: boolean
  customer?: Customer | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  'subscription-updated': [data: any]
}>()

const isProcessing = ref(false)
const pendingAction = ref<string | null>(null)
const selectedPlan = ref<SubscriptionPlan | null>(null)

const subscriptionPlans: SubscriptionPlan[] = [
  {
    id: 'basic',
    name: 'Basic Plan',
    price: 99,
    description: 'Perfect for small businesses',
    features: [
      'Up to 10 users',
      'Basic CRM features', 
      'Email support',
      '5GB storage'
    ]
  },
  {
    id: 'professional',
    name: 'Professional',
    price: 299,
    description: 'Advanced features for growing teams',
    features: [
      'Up to 50 users',
      'Advanced CRM & automation',
      'Priority support',
      '50GB storage',
      'Custom reports'
    ]
  },
  {
    id: 'enterprise',
    name: 'Enterprise',
    price: 999,
    description: 'Full-featured solution for large organizations',
    features: [
      'Unlimited users',
      'Full ERP suite',
      '24/7 dedicated support',
      'Unlimited storage',
      'Custom integrations',
      'Advanced analytics'
    ]
  }
]

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

const handleAction = (action: string) => {
  pendingAction.value = action
}

const confirmAction = async () => {
  if (!pendingAction.value) return
  
  isProcessing.value = true
  try {
    // TODO: Implement actual API calls for subscription actions
    await new Promise(resolve => setTimeout(resolve, 1000)) // Simulate API call
    
    emit('subscription-updated', {
      action: pendingAction.value,
      customerId: props.customer?.id
    })
    
    pendingAction.value = null
  } catch (error) {
    console.error('Error processing subscription action:', error)
  } finally {
    isProcessing.value = false
  }
}

const handlePlanChange = async () => {
  if (!selectedPlan.value) return
  
  isProcessing.value = true
  try {
    // TODO: Implement actual API call for plan change
    await new Promise(resolve => setTimeout(resolve, 1000)) // Simulate API call
    
    emit('subscription-updated', {
      action: 'plan-change',
      customerId: props.customer?.id,
      planId: selectedPlan.value.id
    })
  } catch (error) {
    console.error('Error changing subscription plan:', error)
  } finally {
    isProcessing.value = false
  }
}

const getActionConfirmationMessage = (action: string) => {
  const messages = {
    'start-trial': 'Start a 30-day free trial for this customer?',
    'activate': 'Activate the subscription for this customer?',
    'suspend': 'Temporarily suspend this customer\'s subscription?',
    'cancel': 'Cancel this customer\'s subscription? This action cannot be undone.',
    'renew': 'Renew this customer\'s subscription for another billing period?'
  }
  return messages[action as keyof typeof messages] || 'Are you sure you want to perform this action?'
}
</script>

<style scoped>
/* Add any specific styles if needed */
</style>
