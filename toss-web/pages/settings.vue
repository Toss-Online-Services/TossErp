<script setup lang="ts">
import { ref } from 'vue'

useHead({ title: 'Settings - TOSS' })

const darkMode = ref(false)
const notifications = ref(true)
const emailNotifications = ref(true)
const smsNotifications = ref(false)
const language = ref('en')
const currency = ref('ZAR')
const timezone = ref('Africa/Johannesburg')

const businessName = ref('My Business')
const businessEmail = ref('business@example.com')
const businessPhone = ref('+27 11 123 4567')
const businessAddress = ref('')
const businessCity = ref('')
const businessPostalCode = ref('')

function saveSettings() {
  // TODO: Save to backend
  alert('Settings saved successfully!')
}

function exportData() {
  alert('Data export feature coming soon')
}

function importData() {
  alert('Data import feature coming soon')
}
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Settings</h3>
      <p class="text-gray-600 text-sm">Manage your preferences and configuration</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Main Settings -->
      <div class="lg:col-span-2 space-y-6">
        <!-- General Settings -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">General</h4>
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Language</label>
              <select
                v-model="language"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="en">English</option>
                <option value="zu">Zulu</option>
                <option value="xh">Xhosa</option>
                <option value="af">Afrikaans</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Currency</label>
              <select
                v-model="currency"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="ZAR">ZAR (South African Rand)</option>
                <option value="USD">USD (US Dollar)</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Timezone</label>
              <select
                v-model="timezone"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="Africa/Johannesburg">Africa/Johannesburg (SAST)</option>
                <option value="Africa/Cape_Town">Africa/Cape Town (SAST)</option>
              </select>
            </div>

            <div class="flex items-center justify-between pt-4 border-t border-gray-200">
              <div>
                <label class="text-sm font-medium text-gray-700">Dark Mode</label>
                <p class="text-xs text-gray-500">Switch to dark theme</p>
              </div>
              <label class="relative inline-flex items-center cursor-pointer">
                <input
                  v-model="darkMode"
                  type="checkbox"
                  class="sr-only peer"
                />
                <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
              </label>
            </div>
          </div>
        </div>

        <!-- Business Information -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Business Information</h4>
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Business Name</label>
              <input
                v-model="businessName"
                type="text"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Email</label>
                <input
                  v-model="businessEmail"
                  type="email"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Phone</label>
                <input
                  v-model="businessPhone"
                  type="tel"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Address</label>
              <input
                v-model="businessAddress"
                type="text"
                class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">City</label>
                <input
                  v-model="businessCity"
                  type="text"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Postal Code</label>
                <input
                  v-model="businessPostalCode"
                  type="text"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Notifications -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Notifications</h4>
          <div class="space-y-4">
            <div class="flex items-center justify-between">
              <div>
                <label class="text-sm font-medium text-gray-700">Enable Notifications</label>
                <p class="text-xs text-gray-500">Receive system notifications</p>
              </div>
              <label class="relative inline-flex items-center cursor-pointer">
                <input
                  v-model="notifications"
                  type="checkbox"
                  class="sr-only peer"
                />
                <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
              </label>
            </div>

            <div class="flex items-center justify-between">
              <div>
                <label class="text-sm font-medium text-gray-700">Email Notifications</label>
                <p class="text-xs text-gray-500">Receive notifications via email</p>
              </div>
              <label class="relative inline-flex items-center cursor-pointer">
                <input
                  v-model="emailNotifications"
                  type="checkbox"
                  class="sr-only peer"
                />
                <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
              </label>
            </div>

            <div class="flex items-center justify-between">
              <div>
                <label class="text-sm font-medium text-gray-700">SMS Notifications</label>
                <p class="text-xs text-gray-500">Receive notifications via SMS</p>
              </div>
              <label class="relative inline-flex items-center cursor-pointer">
                <input
                  v-model="smsNotifications"
                  type="checkbox"
                  class="sr-only peer"
                />
                <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
              </label>
            </div>
          </div>
        </div>

        <!-- Data Management -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Data Management</h4>
          <div class="space-y-4">
            <button
              @click="exportData"
              class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center gap-2"
            >
              <i class="material-symbols-rounded">download</i>
              <span>Export Data</span>
            </button>
            <button
              @click="importData"
              class="w-full px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors flex items-center justify-center gap-2"
            >
              <i class="material-symbols-rounded">upload</i>
              <span>Import Data</span>
            </button>
          </div>
        </div>

        <!-- Save Button -->
        <div class="flex justify-end">
          <button
            @click="saveSettings"
            class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            Save Settings
          </button>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="space-y-6">
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h4>
          <div class="space-y-2">
            <button class="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 rounded-lg transition-colors">
              <i class="material-symbols-rounded text-base mr-2">account_circle</i>
              Profile Settings
            </button>
            <button class="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 rounded-lg transition-colors">
              <i class="material-symbols-rounded text-base mr-2">lock</i>
              Security
            </button>
            <button class="w-full px-4 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 rounded-lg transition-colors">
              <i class="material-symbols-rounded text-base mr-2">backup</i>
              Backup & Restore
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>



