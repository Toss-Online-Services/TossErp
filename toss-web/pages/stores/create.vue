<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-indigo-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
        <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent truncate">
              Create New Store
            </h1>
            <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400 line-clamp-1">
              Set up a new township store location
            </p>
          </div>
          <NuxtLink 
            to="/stores" 
            class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 rounded-xl text-xs sm:text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 hover:shadow-md transition-all duration-200 whitespace-nowrap"
          >
            <ArrowLeftIcon class="w-4 h-4 sm:mr-2" />
            <span class="hidden sm:inline">Back to Stores</span>
          </NuxtLink>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-4xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8">
      <form @submit.prevent="createStore" class="space-y-6">
        <!-- Store Information -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-indigo-500 rounded-full mr-2"></span>
            Store Information
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Store Name <span class="text-red-500">*</span>
              </label>
              <input v-model="formData.name" type="text" required
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="e.g., Thabo's Spaza Shop">
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Description</label>
              <textarea v-model="formData.description" rows="3"
                        class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                        placeholder="Brief description of the store..."></textarea>
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
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Display Order</label>
              <input v-model.number="formData.displayOrder" type="number" min="0"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
            </div>
          </div>
        </div>

        <!-- Contact Information -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-blue-500 rounded-full mr-2"></span>
            Contact Information
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Contact Phone</label>
              <input v-model="formData.contactPhone" type="tel"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="+27 11 123 4567">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email Address</label>
              <input v-model="formData.email" type="email"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="store@example.com">
            </div>
          </div>
        </div>

        <!-- Address -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-green-500 rounded-full mr-2"></span>
            Physical Address
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Street Address</label>
              <input v-model="formData.addressStreet" type="text"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="123 Main Street">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">City</label>
              <input v-model="formData.addressCity" type="text"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="Johannesburg">
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
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="1804">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Country</label>
              <input v-model="formData.addressCountry" type="text"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     readonly>
            </div>
          </div>
        </div>

        <!-- GPS Location -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-purple-500 rounded-full mr-2"></span>
            GPS Location (Optional)
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Latitude</label>
              <input v-model.number="formData.latitude" type="number" step="0.000001"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="-26.2041">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Longitude</label>
              <input v-model.number="formData.longitude" type="number" step="0.000001"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="28.0473">
            </div>
            <div class="md:col-span-2">
              <button type="button" @click="getCurrentLocation"
                      class="inline-flex items-center px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white rounded-lg text-sm font-medium transition-colors">
                <MapPinIcon class="w-4 h-4 mr-2" />
                Use Current Location
              </button>
            </div>
          </div>
        </div>

        <!-- Business Hours -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-orange-500 rounded-full mr-2"></span>
            Business Hours
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
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

        <!-- Company & Tax Information -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-yellow-500 rounded-full mr-2"></span>
            Company & Tax Information
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Company Name</label>
              <input v-model="formData.companyName" type="text"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="Company (Pty) Ltd">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">VAT Number</label>
              <input v-model="formData.companyVat" type="text"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="4123456789">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Currency</label>
              <select v-model="formData.currency"
                      class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
                <option value="ZAR">ZAR - South African Rand</option>
                <option value="USD">USD - US Dollar</option>
                <option value="EUR">EUR - Euro</option>
                <option value="GBP">GBP - British Pound</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Tax Rate (%)</label>
              <input v-model.number="formData.taxRate" type="number" step="0.1" min="0" max="100"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
            </div>
          </div>
        </div>

        <!-- Regional Settings -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-teal-500 rounded-full mr-2"></span>
            Regional Settings
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Language</label>
              <select v-model="formData.language"
                      class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
                <option value="en">English</option>
                <option value="zu">isiZulu</option>
                <option value="xh">isiXhosa</option>
                <option value="af">Afrikaans</option>
                <option value="st">Sesotho</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Timezone</label>
              <select v-model="formData.timezone"
                      class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
                <option value="Africa/Johannesburg">Africa/Johannesburg (SAST)</option>
                <option value="Africa/Cape_Town">Africa/Cape Town</option>
                <option value="Africa/Durban">Africa/Durban</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Feature Toggles -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-pink-500 rounded-full mr-2"></span>
            Features & Settings
          </h3>
          <div class="space-y-4">
            <label class="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-900/50 rounded-lg cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-900 transition-colors">
              <div class="flex items-center">
                <div class="flex-shrink-0 w-10 h-10 bg-gradient-to-br from-green-500 to-emerald-600 rounded-lg flex items-center justify-center mr-3">
                  <span class="text-xl">💬</span>
                </div>
                <div>
                  <p class="font-medium text-slate-900 dark:text-white">WhatsApp Alerts</p>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Send order notifications via WhatsApp</p>
                </div>
              </div>
              <input v-model="formData.whatsAppAlertsEnabled" type="checkbox" class="w-5 h-5 text-indigo-600 rounded">
            </label>

            <label class="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-900/50 rounded-lg cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-900 transition-colors">
              <div class="flex items-center">
                <div class="flex-shrink-0 w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center mr-3">
                  <span class="text-xl">🛒</span>
                </div>
                <div>
                  <p class="font-medium text-slate-900 dark:text-white">Group Buying</p>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Enable community bulk purchasing</p>
                </div>
              </div>
              <input v-model="formData.groupBuyingEnabled" type="checkbox" class="w-5 h-5 text-indigo-600 rounded">
            </label>

            <label class="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-900/50 rounded-lg cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-900 transition-colors">
              <div class="flex items-center">
                <div class="flex-shrink-0 w-10 h-10 bg-gradient-to-br from-purple-500 to-blue-600 rounded-lg flex items-center justify-center mr-3">
                  <span class="text-xl">🤖</span>
                </div>
                <div>
                  <p class="font-medium text-slate-900 dark:text-white">AI Assistant</p>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Enable AI-powered business insights</p>
                </div>
              </div>
              <input v-model="formData.aiAssistantEnabled" type="checkbox" class="w-5 h-5 text-indigo-600 rounded">
            </label>
          </div>
        </div>

        <!-- E-commerce Settings (Optional) -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4 flex items-center">
            <span class="w-2 h-2 bg-cyan-500 rounded-full mr-2"></span>
            E-commerce Settings (Optional)
          </h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Store URL</label>
              <input v-model="formData.url" type="url"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="https://mystore.toss.co.za/">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Hosts</label>
              <input v-model="formData.hosts" type="text"
                     class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white"
                     placeholder="www.example.com,example.com">
            </div>
            <div class="flex items-center">
              <label class="flex items-center cursor-pointer">
                <input v-model="formData.sslEnabled" type="checkbox" class="w-5 h-5 text-indigo-600 rounded mr-2">
                <span class="text-sm font-medium text-slate-700 dark:text-slate-300">Enable SSL/HTTPS</span>
              </label>
            </div>
          </div>
        </div>

        <!-- Form Actions -->
        <div class="flex justify-end space-x-3 pt-4">
          <NuxtLink 
            to="/stores"
            class="px-6 py-3 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-700 font-medium transition-colors"
          >
            Cancel
          </NuxtLink>
          <button 
            type="submit"
            :disabled="submitting"
            class="px-6 py-3 bg-gradient-to-r from-indigo-600 to-purple-600 hover:from-indigo-700 hover:to-purple-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <span v-if="submitting">Creating...</span>
            <span v-else>Create Store</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  BuildingStorefrontIcon,
  ArrowLeftIcon,
  MapPinIcon
} from '@heroicons/vue/24/outline'
import { useStoresAPI } from '~/composables/useStoresAPI'
import type { CreateStoreRequest } from '~/types/stores'

// Page metadata
useHead({
  title: 'Create Store - TOSS ERP',
  meta: [
    { name: 'description', content: 'Create a new township store location' }
  ]
})

definePageMeta({
  layout: 'default'
})

// API
const storesAPI = useStoresAPI()
const router = useRouter()

// State
const submitting = ref(false)
const formData = ref<CreateStoreRequest>({
  name: '',
  description: '',
  ownerId: '', // Should be set from current user
  areaGroup: '',
  displayOrder: 0,
  contactPhone: '',
  email: '',
  addressStreet: '',
  addressCity: '',
  addressProvince: '',
  addressPostalCode: '',
  addressCountry: 'South Africa',
  latitude: undefined,
  longitude: undefined,
  openingTime: '08:00',
  closingTime: '18:00',
  currency: 'ZAR',
  taxRate: 15,
  language: 'en',
  timezone: 'Africa/Johannesburg',
  whatsAppAlertsEnabled: true,
  groupBuyingEnabled: true,
  aiAssistantEnabled: true,
  url: '',
  sslEnabled: false,
  hosts: '',
  companyName: '',
  companyVat: ''
})

// Get current location
const getCurrentLocation = () => {
  if ('geolocation' in navigator) {
    navigator.geolocation.getCurrentPosition(
      (position) => {
        formData.value.latitude = position.coords.latitude
        formData.value.longitude = position.coords.longitude
        alert('✓ Location captured successfully')
      },
      (error) => {
        console.error('Geolocation error:', error)
        alert('✗ Failed to get location. Please enter manually.')
      }
    )
  } else {
    alert('✗ Geolocation is not supported by your browser')
  }
}

// Create store
const createStore = async () => {
  submitting.value = true
  try {
    // Get current user from session
    const session = sessionStorage.getItem('user')
    if (session) {
      const user = JSON.parse(session)
      formData.value.ownerId = user.id || user.userId || ''
    }

    if (!formData.value.ownerId) {
      alert('✗ User session not found. Please log in again.')
      router.push('/auth/login')
      return
    }

    const result = await storesAPI.createStore(formData.value)
    alert('✓ Store created successfully!')
    router.push(`/stores/${result.id}`)
  } catch (error: any) {
    console.error('Failed to create store:', error)
    alert(error.data?.message || '✗ Failed to create store. Please try again.')
  } finally {
    submitting.value = false
  }
}
</script>

