<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-indigo-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Loading State -->
    <div v-if="loading" class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <div class="w-16 h-16 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin mx-auto mb-4"></div>
        <p class="text-slate-600 dark:text-slate-400">Loading store...</p>
      </div>
    </div>

    <!-- Content -->
    <div v-else-if="store">
      <!-- Page Header -->
      <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
        <div class="w-full mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
          <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
            <div class="flex-1 min-w-0 flex items-center space-x-4">
              <NuxtLink 
                to="/stores"
                class="flex-shrink-0 p-2 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors"
              >
                <ArrowLeftIcon class="w-6 h-6 text-slate-600 dark:text-slate-400" />
              </NuxtLink>
              <div class="flex-1 min-w-0">
                <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent truncate">
                  {{ store.name }}
                </h1>
                <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400 line-clamp-1">
                  {{ store.areaGroup || 'No area assigned' }}
                </p>
              </div>
            </div>
            <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
              <button 
                @click="editing = !editing"
                class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 rounded-xl text-xs sm:text-sm font-medium text-white transition-all duration-200 whitespace-nowrap"
                :class="editing ? 'bg-slate-600 hover:bg-slate-700' : 'bg-indigo-600 hover:bg-indigo-700'"
              >
                <PencilSquareIcon v-if="!editing" class="w-4 h-4 sm:mr-2" />
                <XMarkIcon v-else class="w-4 h-4 sm:mr-2" />
                <span class="hidden sm:inline">{{ editing ? 'Cancel' : 'Edit' }}</span>
              </button>
              <button 
                v-if="editing"
                @click="updateStore" 
                :disabled="submitting"
                class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 bg-gradient-to-r from-green-600 to-emerald-600 text-white rounded-xl hover:from-green-700 hover:to-emerald-700 shadow-lg hover:shadow-xl transition-all duration-200 text-xs sm:text-sm font-semibold whitespace-nowrap disabled:opacity-50"
              >
                <CheckIcon class="w-4 h-4 sm:mr-2" />
                <span class="hidden sm:inline">{{ submitting ? 'Saving...' : 'Save Changes' }}</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content -->
      <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8 space-y-4 sm:space-y-6">
        <!-- Quick Stats -->
        <div class="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
            <div class="flex items-center justify-between">
              <div class="flex-1 min-w-0">
                <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Customers</p>
                <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ store.customerCount || 0 }}</p>
              </div>
              <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl shadow-lg flex-shrink-0">
                <UsersIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
              </div>
            </div>
          </div>

          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
            <div class="flex items-center justify-between">
              <div class="flex-1 min-w-0">
                <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Products</p>
                <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ store.productCount || 0 }}</p>
              </div>
              <div class="p-2 sm:p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl shadow-lg flex-shrink-0">
                <CubeIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
              </div>
            </div>
          </div>

          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
            <div class="flex items-center justify-between">
              <div class="flex-1 min-w-0">
                <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Sales</p>
                <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ store.salesCount || 0 }}</p>
              </div>
              <div class="p-2 sm:p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl shadow-lg flex-shrink-0">
                <ShoppingBagIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
              </div>
            </div>
          </div>

          <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
            <div class="flex items-center justify-between">
              <div class="flex-1 min-w-0">
                <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Revenue</p>
                <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R{{ formatCurrency(store.totalRevenue || 0) }}</p>
              </div>
              <div class="p-2 sm:p-3 bg-gradient-to-br from-orange-500 to-red-600 rounded-xl shadow-lg flex-shrink-0">
                <CurrencyDollarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
              </div>
            </div>
          </div>
        </div>

        <!-- Store Information (Read/Edit Mode) -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Basic Information -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
              <span class="w-2 h-2 bg-indigo-500 rounded-full mr-2"></span>
              Basic Information
            </h3>
            <div v-if="!editing" class="space-y-3">
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-400">Store Name</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.name }}</p>
              </div>
              <div v-if="store.description">
                <p class="text-xs text-slate-500 dark:text-slate-400">Description</p>
                <p class="text-sm text-slate-700 dark:text-slate-300">{{ store.description }}</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-400">Area Group</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.areaGroup || 'Not assigned' }}</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-400">Status</p>
                <span 
                  v-if="store.isActive"
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
                >
                  Active
                </span>
                <span 
                  v-else
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400"
                >
                  Inactive
                </span>
              </div>
            </div>
            <div v-else class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Store Name</label>
                <input v-model="formData.name" type="text" required
                       class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Description</label>
                <textarea v-model="formData.description" rows="3"
                          class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Area Group</label>
                <select v-model="formData.areaGroup"
                        class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
                  <option value="">Select Area</option>
                  <option value="soweto">Soweto</option>
                  <option value="alexandra">Alexandra</option>
                  <option value="diepsloot">Diepsloot</option>
                  <option value="orange-farm">Orange Farm</option>
                  <option value="tembisa">Tembisa</option>
                  <option value="katlehong">Katlehong</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Contact Information -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
              <span class="w-2 h-2 bg-blue-500 rounded-full mr-2"></span>
              Contact Information
            </h3>
            <div v-if="!editing" class="space-y-3">
              <div v-if="store.contactPhone">
                <p class="text-xs text-slate-500 dark:text-slate-400">Phone</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.contactPhone }}</p>
              </div>
              <div v-if="store.email">
                <p class="text-xs text-slate-500 dark:text-slate-400">Email</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.email }}</p>
              </div>
              <div v-if="store.companyName">
                <p class="text-xs text-slate-500 dark:text-slate-400">Company Name</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.companyName }}</p>
              </div>
            </div>
            <div v-else class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Contact Phone</label>
                <input v-model="formData.contactPhone" type="tel"
                       class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email</label>
                <input v-model="formData.email" type="email"
                       class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Company Name</label>
                <input v-model="formData.companyName" type="text"
                       class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              </div>
            </div>
          </div>

          <!-- Address -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
              <span class="w-2 h-2 bg-green-500 rounded-full mr-2"></span>
              Physical Address
            </h3>
            <div v-if="!editing" class="space-y-3">
              <div v-if="store.addressStreet">
                <p class="text-xs text-slate-500 dark:text-slate-400">Street</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.addressStreet }}</p>
              </div>
              <div v-if="store.addressCity">
                <p class="text-xs text-slate-500 dark:text-slate-400">City</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.addressCity }}</p>
              </div>
              <div v-if="store.addressProvince">
                <p class="text-xs text-slate-500 dark:text-slate-400">Province</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.addressProvince }}</p>
              </div>
              <div v-if="store.addressPostalCode">
                <p class="text-xs text-slate-500 dark:text-slate-400">Postal Code</p>
                <p class="text-sm font-medium text-slate-900 dark:text-white">{{ store.addressPostalCode }}</p>
              </div>
            </div>
            <div v-else class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Street</label>
                <input v-model="formData.addressStreet" type="text"
                       class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">City</label>
                <input v-model="formData.addressCity" type="text"
                       class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Province</label>
                <select v-model="formData.addressProvince"
                        class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
                  <option value="">Select Province</option>
                  <option value="Gauteng">Gauteng</option>
                  <option value="Western Cape">Western Cape</option>
                  <option value="KwaZulu-Natal">KwaZulu-Natal</option>
                  <option value="Eastern Cape">Eastern Cape</option>
                  <option value="Free State">Free State</option>
                  <option value="Limpopo">Limpopo</option>
                  <option value="Mpumalanga">Mpumalanga</option>
                  <option value="North West">North West</option>
                  <option value="Northern Cape">Northern Cape</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Postal Code</label>
                <input v-model="formData.addressPostalCode" type="text"
                       class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              </div>
            </div>
          </div>

          <!-- Features -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
              <span class="w-2 h-2 bg-purple-500 rounded-full mr-2"></span>
              Features & Settings
            </h3>
            <div v-if="!editing" class="space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-700 dark:text-slate-300">WhatsApp Alerts</span>
                <span 
                  v-if="store.whatsAppAlertsEnabled"
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
                >
                  Enabled
                </span>
                <span 
                  v-else
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400"
                >
                  Disabled
                </span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-700 dark:text-slate-300">Group Buying</span>
                <span 
                  v-if="store.groupBuyingEnabled"
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
                >
                  Enabled
                </span>
                <span 
                  v-else
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400"
                >
                  Disabled
                </span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-700 dark:text-slate-300">AI Assistant</span>
                <span 
                  v-if="store.aiAssistantEnabled"
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
                >
                  Enabled
                </span>
                <span 
                  v-else
                  class="inline-flex px-3 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400"
                >
                  Disabled
                </span>
              </div>
            </div>
            <div v-else class="space-y-4">
              <label class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-900/50 rounded-lg cursor-pointer">
                <span class="text-sm font-medium text-slate-900 dark:text-white">WhatsApp Alerts</span>
                <input v-model="formData.whatsAppAlertsEnabled" type="checkbox" class="w-5 h-5 text-indigo-600 rounded">
              </label>
              <label class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-900/50 rounded-lg cursor-pointer">
                <span class="text-sm font-medium text-slate-900 dark:text-white">Group Buying</span>
                <input v-model="formData.groupBuyingEnabled" type="checkbox" class="w-5 h-5 text-indigo-600 rounded">
              </label>
              <label class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-900/50 rounded-lg cursor-pointer">
                <span class="text-sm font-medium text-slate-900 dark:text-white">AI Assistant</span>
                <input v-model="formData.aiAssistantEnabled" type="checkbox" class="w-5 h-5 text-indigo-600 rounded">
              </label>
              <label class="flex items-center justify-between p-3 bg-slate-50 dark:bg-slate-900/50 rounded-lg cursor-pointer">
                <span class="text-sm font-medium text-slate-900 dark:text-white">Store Active</span>
                <input v-model="formData.isActive" type="checkbox" class="w-5 h-5 text-indigo-600 rounded">
              </label>
            </div>
          </div>
        </div>

        <!-- Business Hours (Full width) -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-orange-500 rounded-full mr-2"></span>
            Business Hours
          </h3>
          <div v-if="!editing" class="flex items-center space-x-4">
            <div v-if="store.openingTime && store.closingTime" class="flex items-center text-slate-700 dark:text-slate-300">
              <ClockIcon class="w-5 h-5 mr-2" />
              <span>{{ formatTime(store.openingTime) }} - {{ formatTime(store.closingTime) }}</span>
            </div>
            <div v-else class="text-slate-500 dark:text-slate-400">Not set</div>
          </div>
          <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Opening Time</label>
              <input v-model="formData.openingTime" type="time"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Closing Time</label>
              <input v-model="formData.closingTime" type="time"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
            </div>
          </div>
        </div>

        <!-- GPS Location -->
        <div v-if="store.latitude && store.longitude" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-pink-500 rounded-full mr-2"></span>
            GPS Location
          </h3>
          <div class="flex items-center space-x-4">
            <MapPinIcon class="w-5 h-5 text-pink-600" />
            <span class="text-sm text-slate-700 dark:text-slate-300">{{ store.latitude }}, {{ store.longitude }}</span>
            <a :href="`https://www.google.com/maps?q=${store.latitude},${store.longitude}`" target="_blank"
               class="text-indigo-600 hover:text-indigo-800 dark:text-indigo-400 dark:hover:text-indigo-200 text-sm font-medium">
              View on Google Maps
            </a>
          </div>
        </div>
      </div>
    </div>

    <!-- Error State -->
    <div v-else class="flex items-center justify-center min-h-screen">
      <div class="text-center">
        <BuildingStorefrontIcon class="w-16 h-16 text-slate-400 mx-auto mb-4" />
        <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">Store not found</p>
        <NuxtLink to="/stores" class="text-indigo-600 hover:text-indigo-800 dark:text-indigo-400 dark:hover:text-indigo-200 font-medium">
          Back to Stores
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { 
  BuildingStorefrontIcon,
  UsersIcon,
  CubeIcon,
  ShoppingBagIcon,
  CurrencyDollarIcon,
  ArrowLeftIcon,
  PencilSquareIcon,
  XMarkIcon,
  CheckIcon,
  ClockIcon,
  MapPinIcon
} from '@heroicons/vue/24/outline'
import { useStoresAPI } from '~/composables/useStoresAPI'
import type { Store, UpdateStoreRequest } from '~/types/stores'

// Page metadata
useHead({
  title: 'Store Details - TOSS ERP',
  meta: [
    { name: 'description', content: 'View and edit store details' }
  ]
})

definePageMeta({
  layout: 'default'
})

// Router
const route = useRoute()
const router = useRouter()
const storeId = computed(() => Number(route.params.id))

// API
const storesAPI = useStoresAPI()

// State
const loading = ref(true)
const submitting = ref(false)
const editing = ref(false)
const store = ref<Store | null>(null)
const formData = ref<UpdateStoreRequest>({} as UpdateStoreRequest)

// Load store on mount
onMounted(async () => {
  await loadStore()
})

// Load store data
const loadStore = async () => {
  loading.value = true
  try {
    store.value = await storesAPI.getStoreById(storeId.value)
    if (store.value) {
      // Initialize form data
      formData.value = {
        name: store.value.name,
        description: store.value.description,
        areaGroup: store.value.areaGroup,
        displayOrder: store.value.displayOrder,
        contactPhone: store.value.contactPhone,
        email: store.value.email,
        addressStreet: store.value.addressStreet,
        addressCity: store.value.addressCity,
        addressProvince: store.value.addressProvince,
        addressPostalCode: store.value.addressPostalCode,
        addressCountry: store.value.addressCountry || 'South Africa',
        latitude: store.value.latitude,
        longitude: store.value.longitude,
        openingTime: store.value.openingTime,
        closingTime: store.value.closingTime,
        currency: store.value.currency,
        taxRate: store.value.taxRate,
        language: store.value.language,
        timezone: store.value.timezone,
        whatsAppAlertsEnabled: store.value.whatsAppAlertsEnabled,
        groupBuyingEnabled: store.value.groupBuyingEnabled,
        aiAssistantEnabled: store.value.aiAssistantEnabled,
        isActive: store.value.isActive,
        url: store.value.url,
        sslEnabled: store.value.sslEnabled,
        hosts: store.value.hosts,
        companyName: store.value.companyName,
        companyVat: store.value.companyVat
      }
    }
  } catch (error) {
    console.error('Failed to load store:', error)
  } finally {
    loading.value = false
  }
}

// Update store
const updateStore = async () => {
  submitting.value = true
  try {
    await storesAPI.updateStore(storeId.value, formData.value)
    await loadStore()
    editing.value = false
    alert('✓ Store updated successfully!')
  } catch (error: any) {
    console.error('Failed to update store:', error)
    alert(error.data?.message || '✗ Failed to update store. Please try again.')
  } finally {
    submitting.value = false
  }
}

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatTime = (time: string) => {
  if (!time) return ''
  const [hours, minutes] = time.split(':')
  const hour = parseInt(hours)
  const ampm = hour >= 12 ? 'PM' : 'AM'
  const displayHour = hour % 12 || 12
  return `${displayHour}:${minutes} ${ampm}`
}
</script>

