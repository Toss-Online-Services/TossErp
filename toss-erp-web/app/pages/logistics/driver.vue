<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
              Driver Portal
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              🚚 Manage deliveries, capture POD, track earnings
            </p>
          </div>
          <span class="inline-flex items-center px-4 py-2 rounded-full text-sm font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 shadow-sm">
            <span class="w-2 h-2 bg-green-600 rounded-full mr-2 animate-pulse"></span>
            Online
          </span>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
      <!-- Current Run -->
      <div v-if="currentRun" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700">
        <div class="bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20 p-6 border-b border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-4">
              <div class="p-3 bg-blue-600 rounded-xl">
                <TruckIcon class="w-6 h-6 text-white" />
              </div>
              <div>
                <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Current Delivery Run</h2>
                <p class="text-sm text-slate-600 dark:text-slate-400">
                  Run #{{ currentRun.id }} • Started {{ new Date().toLocaleTimeString('en-ZA', { hour: '2-digit', minute: '2-digit' }) }}
                </p>
              </div>
            </div>
            <button
              @click="completeRun"
              :disabled="currentRun.completedStops < currentRun.stops.length"
              class="inline-flex items-center px-5 py-2.5 bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700 disabled:from-slate-400 disabled:to-slate-500 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200 disabled:cursor-not-allowed"
            >
              <CheckCircleIcon class="w-5 h-5 mr-2" />
              Complete Run
            </button>
          </div>
        </div>
        <div class="p-6">
          <!-- Progress Stats -->
          <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
            <div class="bg-slate-50 dark:bg-slate-700/50 rounded-xl p-4">
              <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Total Stops</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ currentRun.stops.length }}</p>
            </div>
            <div class="bg-green-50 dark:bg-green-900/20 rounded-xl p-4">
              <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Completed</p>
              <p class="text-2xl font-bold text-green-600 dark:text-green-400">{{ currentRun.completedStops }}</p>
            </div>
            <div class="bg-orange-50 dark:bg-orange-900/20 rounded-xl p-4">
              <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Remaining</p>
              <p class="text-2xl font-bold text-orange-600 dark:text-orange-400">{{ currentRun.stops.length - currentRun.completedStops }}</p>
            </div>
            <div class="bg-blue-50 dark:bg-blue-900/20 rounded-xl p-4">
              <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Est. Payout</p>
              <p class="text-2xl font-bold text-blue-600 dark:text-blue-400">R{{ currentRun.payout || 850 }}</p>
            </div>
          </div>

          <!-- Progress Bar -->
          <div class="mb-6">
            <div class="flex items-center justify-between mb-2">
              <span class="text-sm font-medium text-slate-700 dark:text-slate-300">Run Progress</span>
              <span class="text-sm font-medium text-slate-900 dark:text-white">
                {{ Math.round((currentRun.completedStops / currentRun.stops.length) * 100) }}%
              </span>
            </div>
            <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-3">
              <div
                class="bg-gradient-to-r from-green-600 to-blue-600 h-3 rounded-full transition-all duration-500"
                :style="{ width: `${(currentRun.completedStops / currentRun.stops.length) * 100}%` }"
              ></div>
            </div>
          </div>

          <!-- Delivery Stops -->
          <div>
            <h3 class="text-sm font-bold text-slate-900 dark:text-white mb-4 flex items-center">
              <MapPinIcon class="w-5 h-5 mr-2 text-blue-600 dark:text-blue-400" />
              Delivery Stops
            </h3>
            <div class="space-y-3">
              <div
                v-for="(stop, index) in currentRun.stops"
                :key="stop.id"
                class="border-2 rounded-xl p-4 transition-all duration-200"
                :class="stop.status === 'Delivered' 
                  ? 'border-green-200 bg-green-50 dark:border-green-800 dark:bg-green-900/10' 
                  : 'border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800/50 hover:border-blue-300 dark:hover:border-blue-600'"
              >
                <div class="flex items-start gap-4">
                  <div class="flex-shrink-0">
                    <div :class="[
                      'w-10 h-10 rounded-full flex items-center justify-center font-bold text-white',
                      stop.status === 'Delivered' 
                        ? 'bg-gradient-to-br from-green-500 to-emerald-600' 
                        : 'bg-gradient-to-br from-blue-500 to-indigo-600'
                    ]">
                      {{ index + 1 }}
                    </div>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-start justify-between mb-2">
                      <div>
                        <h4 class="text-sm font-semibold text-slate-900 dark:text-white">{{ stop.shopName }}</h4>
                        <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ stop.address }}</p>
                      </div>
                      <span
                        class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium whitespace-nowrap ml-2"
                        :class="getStopStatusClass(stop.status)"
                      >
                        {{ stop.status }}
                      </span>
                    </div>
                    <div class="flex items-center gap-4 text-xs text-slate-600 dark:text-slate-400 mb-3">
                      <span>📦 {{ stop.items }} items</span>
                      <span v-if="stop.distance">🚗 {{ stop.distance }}km</span>
                    </div>
                    <div class="flex items-center justify-between">
                      <div v-if="stop.status === 'Delivered' && stop.deliveredAt" class="text-xs text-green-600 dark:text-green-400 flex items-center gap-1">
                        <CheckCircleIcon class="w-4 h-4" />
                        Delivered at {{ new Date(stop.deliveredAt).toLocaleTimeString('en-ZA', { hour: '2-digit', minute: '2-digit' }) }}
                      </div>
                      <button
                        v-if="stop.status !== 'Delivered'"
                        @click="openPODModal(stop)"
                        class="inline-flex items-center px-4 py-2 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white rounded-lg text-sm font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
                      >
                        <CameraIcon class="w-4 h-4 mr-2" />
                        Capture POD
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- No Active Run -->
      <div v-else class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700">
        <div class="p-12 text-center">
          <div class="p-4 bg-slate-100 dark:bg-slate-700 rounded-full w-20 h-20 mx-auto mb-4 flex items-center justify-center">
            <TruckIcon class="w-10 h-10 text-slate-400" />
          </div>
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">No Active Run</h3>
          <p class="mt-2 text-sm text-slate-600 dark:text-slate-400 max-w-sm mx-auto">
            You don't have any active delivery runs at the moment. Check available runs to get started.
          </p>
          <div class="mt-6">
            <button
              @click="viewAvailableRuns"
              class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200"
            >
              <TruckIcon class="w-5 h-5 mr-2" />
              View Available Runs
            </button>
          </div>
        </div>
      </div>

      <!-- Earnings Summary -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300">
          <div class="flex items-center gap-3 mb-2">
            <div class="p-2 bg-green-100 dark:bg-green-900/30 rounded-lg">
              <CurrencyDollarIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Today's Earnings</p>
          </div>
          <p class="text-3xl font-bold text-slate-900 dark:text-white">R 850</p>
          <p class="text-xs text-green-600 dark:text-green-400 mt-2">+R120 from yesterday</p>
        </div>
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300">
          <div class="flex items-center gap-3 mb-2">
            <div class="p-2 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
              <CalendarIcon class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">This Week</p>
          </div>
          <p class="text-3xl font-bold text-slate-900 dark:text-white">R 4,250</p>
          <p class="text-xs text-slate-500 dark:text-slate-400 mt-2">14 deliveries completed</p>
        </div>
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300">
          <div class="flex items-center gap-3 mb-2">
            <div class="p-2 bg-purple-100 dark:bg-purple-900/30 rounded-lg">
              <ChartBarIcon class="w-5 h-5 text-purple-600 dark:text-purple-400" />
            </div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">This Month</p>
          </div>
          <p class="text-3xl font-bold text-slate-900 dark:text-white">R 18,900</p>
          <p class="text-xs text-purple-600 dark:text-purple-400 mt-2">↑ 23% from last month</p>
        </div>
      </div>
    </div>

    <!-- POD Capture Modal -->
    <PODCaptureModal
      :show="showPODModal"
      :stop="selectedStop"
      @close="showPODModal = false"
      @capture="handlePODCapture"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  TruckIcon,
  CheckCircleIcon,
  CameraIcon,
  MapPinIcon,
  CurrencyDollarIcon,
  CalendarIcon,
  ChartBarIcon
} from '@heroicons/vue/24/outline'
import PODCaptureModal from '~/components/logistics/PODCaptureModal.vue'

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: 'Driver Portal - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage delivery runs and capture proof of delivery' }
  ]
})

// POD Modal State
const showPODModal = ref(false)
const selectedStop = ref<any>(null)

// Current Run Data
const currentRun = ref({
  id: 'DR-2025-001',
  payout: 850,
  completedStops: 2,
  stops: [
    {
      id: '1',
      shopName: 'Thabo\'s Spaza',
      address: '123 Main Street, Soweto',
      items: 5,
      distance: 3.2,
      status: 'Delivered',
      deliveredAt: new Date(Date.now() - 1800000) // 30 mins ago
    },
    {
      id: '2',
      shopName: 'Sarah\'s Store',
      address: '456 Church Road, Alexandra',
      items: 3,
      distance: 2.8,
      status: 'Delivered',
      deliveredAt: new Date(Date.now() - 900000) // 15 mins ago
    },
    {
      id: '3',
      shopName: 'Lucky\'s Shop',
      address: '789 Market Street, Soweto',
      items: 4,
      distance: 1.5,
      status: 'Pending'
    },
    {
      id: '4',
      shopName: 'Grace\'s Mini Market',
      address: '321 Hill Drive, Alexandra',
      items: 6,
      distance: 4.1,
      status: 'Pending'
    }
  ]
})

// Methods
const getStopStatusClass = (status: string): string => {
  const classes: Record<string, string> = {
    'Pending': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'En Route': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'Failed': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status] || ''
}

const openPODModal = (stop: any) => {
  selectedStop.value = stop
  showPODModal.value = true
}

const handlePODCapture = ({ stop, pod }: { stop: any; pod: any }) => {
  // Update stop status
  const stopIndex = currentRun.value.stops.findIndex((s: any) => s.id === stop.id)
  if (stopIndex !== -1) {
    currentRun.value.stops[stopIndex].status = 'Delivered'
    currentRun.value.stops[stopIndex].deliveredAt = new Date()
    currentRun.value.completedStops++
  }
  
  // Close modal
  showPODModal.value = false
  selectedStop.value = null
  
  // Show success message
  showNotification(`✓ Delivery confirmed for ${stop.shopName}`)
}

const completeRun = () => {
  if (currentRun.value.completedStops === currentRun.value.stops.length) {
    // Navigate to summary or earnings page
    showNotification('✓ Run completed! R' + currentRun.value.payout + ' added to earnings')
    // In production: navigateTo('/logistics/driver/earnings')
  } else {
    showNotification('Please complete all deliveries first', 'error')
  }
}

const viewAvailableRuns = () => {
  navigateTo('/logistics/shared-runs')
}

const showNotification = (message: string, type: 'success' | 'error' = 'success') => {
  const notification = document.createElement('div')
  notification.textContent = message
  notification.className = `fixed top-20 right-4 ${type === 'success' ? 'bg-green-600' : 'bg-red-600'} text-white px-4 py-2 rounded-lg shadow-lg z-50 animate-fade-in`
  document.body.appendChild(notification)
  setTimeout(() => notification.remove(), 3000)
}
</script>


