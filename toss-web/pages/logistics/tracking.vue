<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
              Live Delivery Tracking
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              📍 Track deliveries in real-time with live updates
            </p>
          </div>
          <div class="flex items-center gap-2 px-4 py-2 bg-green-50 dark:bg-green-900/20 rounded-xl">
            <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
            <span class="text-sm font-medium text-green-700 dark:text-green-400">
              {{ activeDeliveries.length }} Active
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
      <!-- Map Placeholder -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700">
        <div class="bg-gradient-to-r from-purple-50 to-pink-50 dark:from-purple-900/20 dark:to-pink-900/20 p-4 border-b border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="p-2 bg-purple-600 rounded-lg">
                <MapPinIcon class="w-5 h-5 text-white" />
              </div>
              <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Live Map</h2>
            </div>
            <button class="px-3 py-1.5 text-sm bg-white dark:bg-slate-700 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-600 transition-colors">
              Fullscreen
            </button>
          </div>
        </div>
        <div class="p-6">
          <div class="bg-gradient-to-br from-purple-100 to-pink-100 dark:from-purple-900/20 dark:to-pink-900/20 rounded-xl h-96 flex items-center justify-center relative overflow-hidden">
            <!-- Animated Background Pattern -->
            <div class="absolute inset-0 opacity-10">
              <div class="absolute top-10 left-10 w-32 h-32 bg-purple-500 rounded-full blur-3xl animate-pulse"></div>
              <div class="absolute bottom-10 right-10 w-40 h-40 bg-pink-500 rounded-full blur-3xl animate-pulse delay-1000"></div>
            </div>
            <div class="text-center relative z-10">
              <div class="p-4 bg-white/80 dark:bg-slate-800/80 backdrop-blur-sm rounded-2xl inline-block mb-4">
                <MapPinIcon class="w-12 h-12 text-purple-600 dark:text-purple-400 mx-auto" />
              </div>
              <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">Interactive Map Coming Soon</p>
              <p class="text-sm text-slate-600 dark:text-slate-400 max-w-md">
                Track all deliveries on a real-time map with driver locations and routes
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Deliveries Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Active Deliveries -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700">
          <div class="bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20 p-4 border-b border-slate-200 dark:border-slate-700">
            <div class="flex items-center gap-3">
              <div class="p-2 bg-blue-600 rounded-lg">
                <TruckIcon class="w-5 h-5 text-white" />
              </div>
              <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Active Deliveries</h2>
            </div>
          </div>
          <div class="p-6">
            <div v-if="activeDeliveries.length === 0" class="text-center py-12">
              <div class="p-4 bg-slate-100 dark:bg-slate-700 rounded-full w-16 h-16 mx-auto mb-4 flex items-center justify-center">
                <TruckIcon class="w-8 h-8 text-slate-400" />
              </div>
              <p class="text-sm font-medium text-slate-900 dark:text-white">No active deliveries</p>
              <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">All deliveries have been completed</p>
            </div>

            <div v-else class="space-y-4">
              <div
                v-for="delivery in activeDeliveries"
                :key="delivery.id"
                class="border-2 border-slate-200 dark:border-slate-700 rounded-xl p-4 hover:border-blue-300 dark:hover:border-blue-600 transition-all duration-200"
              >
                <div class="flex items-start gap-4">
                  <div class="flex-shrink-0">
                    <div class="w-12 h-12 rounded-full bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white font-bold">
                      <TruckIcon class="w-6 h-6" />
                    </div>
                  </div>
                  <div class="flex-1 min-w-0">
                    <div class="flex items-start justify-between mb-2">
                      <div>
                        <h3 class="text-sm font-semibold text-slate-900 dark:text-white">{{ delivery.driverName }}</h3>
                        <p class="text-xs text-slate-600 dark:text-slate-400 mt-0.5">Order #{{ delivery.orderId }}</p>
                      </div>
                      <button
                        @click="trackDelivery(delivery)"
                        class="p-1.5 text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300 hover:bg-blue-50 dark:hover:bg-blue-900/20 rounded-lg transition-colors"
                      >
                        <MapPinIcon class="w-5 h-5" />
                      </button>
                    </div>
                    <div class="flex items-center gap-2 mb-3">
                      <span class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
                        {{ delivery.status }}
                      </span>
                      <span class="text-xs text-slate-600 dark:text-slate-400 flex items-center gap-1">
                        <ClockIcon class="w-3.5 h-3.5" />
                        ETA: {{ formatTime(delivery.eta) }}
                      </span>
                    </div>
                    <!-- Progress Bar -->
                    <div>
                      <div class="flex items-center justify-between mb-1.5">
                        <span class="text-xs font-medium text-slate-700 dark:text-slate-300">Progress</span>
                        <span class="text-xs font-bold text-blue-600 dark:text-blue-400">{{ delivery.progress }}%</span>
                      </div>
                      <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
                        <div
                          class="bg-gradient-to-r from-blue-600 to-indigo-600 h-2 rounded-full transition-all duration-500"
                          :style="{ width: delivery.progress + '%' }"
                        ></div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Delivery History -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700">
          <div class="bg-gradient-to-r from-green-50 to-emerald-50 dark:from-green-900/20 dark:to-emerald-900/20 p-4 border-b border-slate-200 dark:border-slate-700">
            <div class="flex items-center gap-3">
              <div class="p-2 bg-green-600 rounded-lg">
                <CheckCircleIcon class="w-5 h-5 text-white" />
              </div>
              <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Recent Deliveries</h2>
            </div>
          </div>
          <div class="p-6">
            <div class="space-y-3">
              <div
                v-for="delivery in recentDeliveries"
                :key="delivery.id"
                class="flex items-center justify-between py-3 px-4 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-700/50 transition-colors border border-slate-100 dark:border-slate-700"
              >
                <div class="flex items-center gap-3">
                  <div class="p-2 bg-green-100 dark:bg-green-900/30 rounded-lg">
                    <CheckCircleIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
                  </div>
                  <div>
                    <p class="text-sm font-semibold text-slate-900 dark:text-white">Order #{{ delivery.orderId }}</p>
                    <p class="text-xs text-slate-500 dark:text-slate-400 mt-0.5">{{ formatDate(delivery.deliveredAt) }}</p>
                  </div>
                </div>
                <span class="inline-flex items-center px-3 py-1.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                  ✓ Delivered
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  TruckIcon,
  MapPinIcon,
  CheckCircleIcon,
  ClockIcon
} from '@heroicons/vue/24/outline'

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: 'Live Tracking - TOSS ERP',
  meta: [
    { name: 'description', content: 'Track deliveries in real-time with live updates' }
  ]
})

const activeDeliveries = ref([
  {
    id: '1',
    driverName: 'Thabo Molefe',
    orderId: 'PO-2024-001',
    status: 'En Route',
    eta: new Date(Date.now() + 1800000), // 30 mins
    progress: 65
  },
  {
    id: '2',
    driverName: 'Sarah Ndlovu',
    orderId: 'PO-2024-002',
    status: 'En Route',
    eta: new Date(Date.now() + 3600000), // 1 hour
    progress: 35
  }
])

const recentDeliveries = ref([
  {
    id: '3',
    orderId: 'PO-2024-003',
    deliveredAt: new Date(Date.now() - 3600000)
  },
  {
    id: '4',
    orderId: 'PO-2024-004',
    deliveredAt: new Date(Date.now() - 7200000)
  },
  {
    id: '5',
    orderId: 'PO-2024-005',
    deliveredAt: new Date(Date.now() - 10800000)
  }
])

const formatTime = (date: Date): string => {
  return new Intl.DateTimeFormat('en-ZA', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

const trackDelivery = (delivery: any) => {
  // TODO: Implement detailed tracking view
  console.log('Track delivery:', delivery)
}
</script>

