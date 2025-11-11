<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-orange-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-orange-600 to-pink-600 bg-clip-text text-transparent">
              Settings
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Configure your shop and system preferences
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="saveSettings"
              :disabled="saving"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-green-600 to-emerald-600 text-white rounded-xl hover:from-green-700 hover:to-emerald-700 shadow-lg hover:shadow-xl transition-all duration-200 font-semibold text-sm sm:text-base disabled:opacity-50"
            >
              <CheckCircleIcon class="w-5 h-5 mr-2" />
              {{ saving ? 'Saving...' : 'Save Changes' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div class="space-y-6">
        <!-- Shop Information -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center mb-4">
            <BuildingStorefrontIcon class="w-6 h-6 text-orange-600 mr-2" />
            <h2 class="text-xl font-bold text-slate-900 dark:text-white">Shop Information</h2>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Shop Name
              </label>
              <input
                v-model="settings.shopName"
                type="text"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Shop ID
              </label>
              <input
                v-model="settings.shopId"
                type="text"
                disabled
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl bg-slate-100 dark:bg-slate-700 text-slate-500 dark:text-slate-400"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Area/Township
              </label>
              <select
                v-model="settings.area"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              >
                <option value="soweto">Soweto</option>
                <option value="alexandra">Alexandra</option>
                <option value="katlehong">Katlehong</option>
                <option value="tembisa">Tembisa</option>
                <option value="diepsloot">Diepsloot</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Zone/Section
              </label>
              <input
                v-model="settings.zone"
                type="text"
                placeholder="e.g., Diepkloof Ext 1"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
            </div>

            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Physical Address
              </label>
              <textarea
                v-model="settings.address"
                rows="2"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-orange-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              ></textarea>
            </div>
          </div>
        </div>

        <!-- Financial Settings -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center mb-4">
            <CurrencyDollarIcon class="w-6 h-6 text-green-600 mr-2" />
            <h2 class="text-xl font-bold text-slate-900 dark:text-white">Financial Settings</h2>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Currency
              </label>
              <select
                v-model="settings.currency"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              >
                <option value="ZAR">ZAR (South African Rand)</option>
                <option value="USD">USD (US Dollar)</option>
                <option value="EUR">EUR (Euro)</option>
                <option value="GBP">GBP (British Pound)</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Tax Rate (%)
              </label>
              <input
                v-model.number="settings.taxRate"
                type="number"
                min="0"
                max="100"
                step="0.1"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Payment Methods
              </label>
              <div class="space-y-2 mt-3">
                <label class="flex items-center space-x-3 cursor-pointer">
                  <input
                    v-model="settings.paymentMethods.cash"
                    type="checkbox"
                    class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                  />
                  <span class="text-sm text-slate-900 dark:text-white">Cash</span>
                </label>
                <label class="flex items-center space-x-3 cursor-pointer">
                  <input
                    v-model="settings.paymentMethods.card"
                    type="checkbox"
                    class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                  />
                  <span class="text-sm text-slate-900 dark:text-white">Card</span>
                </label>
                <label class="flex items-center space-x-3 cursor-pointer">
                  <input
                    v-model="settings.paymentMethods.paylink"
                    type="checkbox"
                    class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                  />
                  <span class="text-sm text-slate-900 dark:text-white">Payment Link</span>
                </label>
                <label class="flex items-center space-x-3 cursor-pointer">
                  <input
                    v-model="settings.paymentMethods.credit"
                    type="checkbox"
                    class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                  />
                  <span class="text-sm text-slate-900 dark:text-white">Credit</span>
                </label>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Default Payment Terms
              </label>
              <select
                v-model="settings.defaultPaymentTerms"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              >
                <option value="COD">Cash on Delivery (COD)</option>
                <option value="net7">Net 7 Days</option>
                <option value="net30">Net 30 Days</option>
                <option value="net60">Net 60 Days</option>
                <option value="net90">Net 90 Days</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Group Buying Settings -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center mb-4">
            <UserGroupIcon class="w-6 h-6 text-purple-600 mr-2" />
            <h2 class="text-xl font-bold text-slate-900 dark:text-white">Group Buying Settings</h2>
          </div>
          
          <div class="space-y-4">
            <label class="flex items-start space-x-3 cursor-pointer">
              <input
                v-model="settings.groupBuying.enabled"
                type="checkbox"
                class="w-4 h-4 mt-1 text-purple-600 border-slate-300 rounded focus:ring-purple-500"
              />
              <div>
                <span class="text-sm font-medium text-slate-900 dark:text-white">
                  Enable Group Buying
                </span>
                <p class="text-xs text-slate-500 dark:text-slate-400">
                  Allow joining and creating buying pools with nearby shops
                </p>
              </div>
            </label>

            <label class="flex items-start space-x-3 cursor-pointer">
              <input
                v-model="settings.groupBuying.autoJoinPools"
                type="checkbox"
                :disabled="!settings.groupBuying.enabled"
                class="w-4 h-4 mt-1 text-purple-600 border-slate-300 rounded focus:ring-purple-500 disabled:opacity-50"
              />
              <div>
                <span class="text-sm font-medium text-slate-900 dark:text-white">
                  AI Auto-Join Pools
                </span>
                <p class="text-xs text-slate-500 dark:text-slate-400">
                  Automatically join pools when AI suggests significant savings
                </p>
              </div>
            </label>

            <label class="flex items-start space-x-3 cursor-pointer">
              <input
                v-model="settings.groupBuying.inviteNotifications"
                type="checkbox"
                :disabled="!settings.groupBuying.enabled"
                class="w-4 h-4 mt-1 text-purple-600 border-slate-300 rounded focus:ring-purple-500 disabled:opacity-50"
              />
              <div>
                <span class="text-sm font-medium text-slate-900 dark:text-white">
                  Pool Invite Notifications
                </span>
                <p class="text-xs text-slate-500 dark:text-slate-400">
                  Receive notifications when nearby shops create relevant pools
                </p>
              </div>
            </label>
          </div>
        </div>

        <!-- Logistics Settings -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center mb-4">
            <TruckIcon class="w-6 h-6 text-blue-600 mr-2" />
            <h2 class="text-xl font-bold text-slate-900 dark:text-white">Logistics Settings</h2>
          </div>
          
          <div class="space-y-4">
            <label class="flex items-start space-x-3 cursor-pointer">
              <input
                v-model="settings.logistics.sharedDelivery"
                type="checkbox"
                class="w-4 h-4 mt-1 text-blue-600 border-slate-300 rounded focus:ring-blue-500"
              />
              <div>
                <span class="text-sm font-medium text-slate-900 dark:text-white">
                  Enable Shared Delivery
                </span>
                <p class="text-xs text-slate-500 dark:text-slate-400">
                  Share delivery runs with nearby shops to reduce costs
                </p>
              </div>
            </label>

            <label class="flex items-start space-x-3 cursor-pointer">
              <input
                v-model="settings.logistics.trackingAlerts"
                type="checkbox"
                class="w-4 h-4 mt-1 text-blue-600 border-slate-300 rounded focus:ring-blue-500"
              />
              <div>
                <span class="text-sm font-medium text-slate-900 dark:text-white">
                  Delivery Tracking Alerts
                </span>
                <p class="text-xs text-slate-400">
                  Real-time WhatsApp updates on delivery status
                </p>
              </div>
            </label>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Preferred Delivery Window
              </label>
              <div class="grid grid-cols-2 gap-3">
                <input
                  v-model="settings.logistics.preferredDeliveryStart"
                  type="time"
                  class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                />
                <input
                  v-model="settings.logistics.preferredDeliveryEnd"
                  type="time"
                  class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-blue-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- WhatsApp Integration -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center mb-4">
            <ChatBubbleLeftRightIcon class="w-6 h-6 text-green-600 mr-2" />
            <h2 class="text-xl font-bold text-slate-900 dark:text-white">WhatsApp Notifications</h2>
          </div>
          
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                WhatsApp Number
              </label>
              <input
                v-model="settings.whatsapp.phone"
                type="tel"
                placeholder="+27 XX XXX XXXX"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-green-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              />
            </div>

            <div class="space-y-2">
              <label class="flex items-center space-x-3 cursor-pointer">
                <input
                  v-model="settings.whatsapp.poolUpdates"
                  type="checkbox"
                  class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                />
                <span class="text-sm text-slate-900 dark:text-white">Pool progress updates</span>
              </label>
              <label class="flex items-center space-x-3 cursor-pointer">
                <input
                  v-model="settings.whatsapp.paymentLinks"
                  type="checkbox"
                  class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                />
                <span class="text-sm text-slate-900 dark:text-white">Payment links</span>
              </label>
              <label class="flex items-center space-x-3 cursor-pointer">
                <input
                  v-model="settings.whatsapp.deliveryUpdates"
                  type="checkbox"
                  class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                />
                <span class="text-sm text-slate-900 dark:text-white">Delivery status updates</span>
              </label>
              <label class="flex items-center space-x-3 cursor-pointer">
                <input
                  v-model="settings.whatsapp.lowStockAlerts"
                  type="checkbox"
                  class="w-4 h-4 text-green-600 border-slate-300 rounded focus:ring-green-500"
                />
                <span class="text-sm text-slate-900 dark:text-white">Low stock alerts</span>
              </label>
            </div>
          </div>
        </div>

        <!-- Appearance & Language -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center mb-4">
            <SparklesIcon class="w-6 h-6 text-indigo-600 mr-2" />
            <h2 class="text-xl font-bold text-slate-900 dark:text-white">Appearance & Language</h2>
          </div>
          
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Theme
              </label>
              <select
                v-model="settings.theme"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              >
                <option value="light">Light</option>
                <option value="dark">Dark</option>
                <option value="system">System</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Language
              </label>
              <select
                v-model="settings.language"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              >
                <option value="en">English</option>
                <option value="zu">isiZulu</option>
                <option value="xh">isiXhosa</option>
                <option value="st">Sesotho</option>
                <option value="af">Afrikaans</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Timezone
              </label>
              <select
                v-model="settings.timezone"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              >
                <option value="Africa/Johannesburg">South Africa (SAST)</option>
                <option value="Africa/Cairo">Egypt (EET)</option>
                <option value="Africa/Lagos">Nigeria (WAT)</option>
                <option value="Africa/Nairobi">Kenya (EAT)</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                Date Format
              </label>
              <select
                v-model="settings.dateFormat"
                class="w-full px-4 py-3 border-2 border-slate-200 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-indigo-500 bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              >
                <option value="DD/MM/YYYY">DD/MM/YYYY</option>
                <option value="MM/DD/YYYY">MM/DD/YYYY</option>
                <option value="YYYY-MM-DD">YYYY-MM-DD</option>
              </select>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex items-center justify-between pt-6">
          <button
            @click="resetToDefaults"
            class="px-6 py-3 border-2 border-red-300 dark:border-red-600 text-red-700 dark:text-red-400 rounded-xl font-semibold hover:bg-red-50 dark:hover:bg-red-900/20 transition-all"
          >
            Reset to Defaults
          </button>

          <button
            @click="saveSettings"
            :disabled="saving"
            class="px-8 py-3 bg-gradient-to-r from-green-600 to-emerald-600 text-white rounded-xl font-semibold hover:from-green-700 hover:to-emerald-700 shadow-lg hover:shadow-xl transition-all disabled:opacity-50"
          >
            {{ saving ? 'Saving...' : 'Save All Settings' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  CheckCircleIcon,
  BuildingStorefrontIcon,
  CurrencyDollarIcon,
  UserGroupIcon,
  TruckIcon,
  ChatBubbleLeftRightIcon,
  SparklesIcon
} from '@heroicons/vue/24/outline'

useHead({
  title: 'Settings - TOSS ERP',
  meta: [
    { name: 'description', content: 'Configure your TOSS ERP settings' }
  ]
})

const saving = ref(false)

const settings = ref({
  // Shop Information
  shopId: 'SHOP-001',
  shopName: "Thabo's Spaza Shop",
  area: 'soweto',
  zone: 'Diepkloof Ext 1',
  address: '12 Church Street, Diepkloof, Soweto',

  // Financial
  currency: 'ZAR',
  taxRate: 15,
  paymentMethods: {
    cash: true,
    card: true,
    paylink: true,
    credit: false
  },
  defaultPaymentTerms: 'COD',

  // Group Buying
  groupBuying: {
    enabled: true,
    autoJoinPools: false,
    inviteNotifications: true
  },

  // Logistics
  logistics: {
    sharedDelivery: true,
    trackingAlerts: true,
    preferredDeliveryStart: '08:00',
    preferredDeliveryEnd: '17:00'
  },

  // WhatsApp
  whatsapp: {
    phone: '+27 71 234 5678',
    poolUpdates: true,
    paymentLinks: true,
    deliveryUpdates: true,
    lowStockAlerts: true
  },

  // Appearance
  theme: 'system',
  language: 'en',
  timezone: 'Africa/Johannesburg',
  dateFormat: 'DD/MM/YYYY'
})

const saveSettings = async () => {
  saving.value = true
  
  try {
    await $fetch('/api/settings', {
      method: 'POST',
      body: settings.value
    })
    
    alert('✓ Settings saved successfully!')
  } catch (error) {
    console.error('Failed to save settings:', error)
    alert('✗ Failed to save settings. Please try again.')
  } finally {
    saving.value = false
  }
}

const resetToDefaults = () => {
  if (confirm('Are you sure you want to reset all settings to defaults? This cannot be undone.')) {
    // Reset to defaults
    settings.value = {
      shopId: settings.value.shopId, // Keep shop ID
      shopName: settings.value.shopName, // Keep shop name
      area: 'soweto',
      zone: '',
      address: '',
      currency: 'ZAR',
      taxRate: 15,
      paymentMethods: {
        cash: true,
        card: false,
        paylink: true,
        credit: false
      },
      defaultPaymentTerms: 'COD',
      groupBuying: {
        enabled: true,
        autoJoinPools: false,
        inviteNotifications: true
      },
      logistics: {
        sharedDelivery: true,
        trackingAlerts: true,
        preferredDeliveryStart: '08:00',
        preferredDeliveryEnd: '17:00'
      },
      whatsapp: {
        phone: settings.value.whatsapp.phone, // Keep phone
        poolUpdates: true,
        paymentLinks: true,
        deliveryUpdates: true,
        lowStockAlerts: true
      },
      theme: 'system',
      language: 'en',
      timezone: 'Africa/Johannesburg',
      dateFormat: 'DD/MM/YYYY'
    }
    alert('✓ Settings reset to defaults')
  }
}
</script>
