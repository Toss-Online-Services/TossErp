<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900 p-6">
    <div class="max-w-7xl mx-auto">
      <!-- Back Button -->
      <button @click="$router.back()" class="mb-4 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white flex items-center space-x-2">
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
        </svg>
        <span>Back to Customers</span>
      </button>

      <!-- Loading State -->
      <div v-if="isLoading" class="bg-white dark:bg-gray-800 rounded-lg shadow p-8 text-center">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
        <p class="text-gray-600 dark:text-gray-400 mt-4">Loading customer profile...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
        <p class="text-red-800 dark:text-red-200">{{ error }}</p>
      </div>

      <!-- Customer Profile -->
      <div v-else-if="customer">
        <!-- Header Card -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 mb-6">
          <div class="flex items-start justify-between">
            <div class="flex items-center space-x-4">
              <div class="w-20 h-20 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                <span class="text-3xl font-bold text-blue-600 dark:text-blue-300">{{ getInitials(customer.name) }}</span>
              </div>
              <div>
                <h1 class="text-3xl font-bold text-gray-900 dark:text-white">{{ customer.name }}</h1>
                <p class="text-gray-600 dark:text-gray-400 mt-1">Customer ID: {{ customer.id }}</p>
                <div class="flex items-center space-x-2 mt-2">
                  <span class="px-3 py-1 text-sm font-semibold rounded-full" :class="getTierClass(customer.tier)">
                    {{ customer.tier || 'Bronze' }}
                  </span>
                  <span class="px-3 py-1 text-sm font-semibold rounded-full bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200">
                    Active
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Stats Grid -->
        <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600 dark:text-gray-400">Total Purchases</p>
                <p class="text-2xl font-bold text-gray-900 dark:text-white mt-1">
                  R{{ customer.totalPurchaseAmount?.toFixed(2) || '0.00' }}
                </p>
              </div>
              <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
                <svg class="w-6 h-6 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
            </div>
          </div>

          <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600 dark:text-gray-400">Total Orders</p>
                <p class="text-2xl font-bold text-gray-900 dark:text-white mt-1">{{ customer.purchaseCount || 0 }}</p>
              </div>
              <div class="w-12 h-12 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
                <svg class="w-6 h-6 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path>
                </svg>
              </div>
            </div>
          </div>

          <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600 dark:text-gray-400">Average Order</p>
                <p class="text-2xl font-bold text-gray-900 dark:text-white mt-1">
                  R{{ customer.purchaseCount ? (customer.totalPurchaseAmount / customer.purchaseCount).toFixed(2) : '0.00' }}
                </p>
              </div>
              <div class="w-12 h-12 bg-yellow-100 dark:bg-yellow-900 rounded-lg flex items-center justify-center">
                <svg class="w-6 h-6 text-yellow-600 dark:text-yellow-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
                </svg>
              </div>
            </div>
          </div>

          <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-600 dark:text-gray-400">Last Purchase</p>
                <p class="text-lg font-bold text-gray-900 dark:text-white mt-1">
                  {{ customer.lastPurchaseDate ? formatDate(customer.lastPurchaseDate) : 'Never' }}
                </p>
              </div>
              <div class="w-12 h-12 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center">
                <svg class="w-6 h-6 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                </svg>
              </div>
            </div>
          </div>
        </div>

        <!-- Details & Purchase History -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <!-- Contact Information -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
            <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">Contact Information</h2>
            <div class="space-y-4">
              <div>
                <label class="text-sm text-gray-600 dark:text-gray-400">Email</label>
                <p class="text-gray-900 dark:text-white font-medium">{{ customer.email || 'Not provided' }}</p>
              </div>
              <div>
                <label class="text-sm text-gray-600 dark:text-gray-400">Phone</label>
                <p class="text-gray-900 dark:text-white font-medium">{{ customer.phoneNumber || 'Not provided' }}</p>
              </div>
              <div>
                <label class="text-sm text-gray-600 dark:text-gray-400">Address</label>
                <p class="text-gray-900 dark:text-white font-medium">{{ customer.address || 'Not provided' }}</p>
              </div>
              <div>
                <label class="text-sm text-gray-600 dark:text-gray-400">Member Since</label>
                <p class="text-gray-900 dark:text-white font-medium">{{ customer.createdDate ? formatDate(customer.createdDate) : 'Unknown' }}</p>
              </div>
            </div>
          </div>

          <!-- Recent Purchases -->
          <div class="lg:col-span-2 bg-white dark:bg-gray-800 rounded-lg shadow p-6">
            <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">Recent Purchases</h2>
            
            <div v-if="!customer.recentPurchases || customer.recentPurchases.length === 0" class="text-center py-8">
              <svg class="w-12 h-12 text-gray-400 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"></path>
              </svg>
              <p class="text-gray-600 dark:text-gray-400">No purchases yet</p>
            </div>

            <div v-else class="space-y-4">
              <div v-for="purchase in customer.recentPurchases" :key="purchase.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center justify-between">
                  <div class="flex-1">
                    <div class="flex items-center justify-between mb-2">
                      <span class="text-sm text-gray-600 dark:text-gray-400">{{ formatDate(purchase.purchaseDate) }}</span>
                      <span class="font-bold text-gray-900 dark:text-white">R{{ purchase.totalAmount?.toFixed(2) }}</span>
                    </div>
                    <p class="text-sm text-gray-600 dark:text-gray-400">Order #{{ purchase.id }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCustomers } from '~/composables/useCustomers'

const route = useRoute()
const { getCustomerProfile } = useCustomers()

const customer = ref<any>(null)
const isLoading = ref(true)
const error = ref('')

const customerId = computed(() => parseInt(route.params.id as string))

useHead({
  title: computed(() => customer.value ? `${customer.value.name} - TOSS ERP` : 'Customer - TOSS ERP')
})

const loadCustomer = async () => {
  isLoading.value = true
  error.value = ''
  
  try {
    const profile = await getCustomerProfile(customerId.value)
    customer.value = profile
  } catch (e: any) {
    error.value = e.message || 'Failed to load customer profile'
  } finally {
    isLoading.value = false
  }
}

const getInitials = (name: string) => {
  return name?.split(' ').map(n => n[0]).join('').toUpperCase() || '??'
}

const getTierClass = (tier: string) => {
  switch (tier?.toLowerCase()) {
    case 'platinum':
      return 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200'
    case 'gold':
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
    case 'silver':
      return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200'
    default:
      return 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200'
  }
}

const formatDate = (date: any) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

onMounted(() => {
  loadCustomer()
})
</script>

