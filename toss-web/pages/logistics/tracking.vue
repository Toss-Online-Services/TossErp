<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-slate-800 shadow-sm border-b border-slate-200 dark:border-slate-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Live Delivery Tracking</h1>
        <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
          Track your deliveries in real-time
        </p>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Map Placeholder -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 mb-8">
        <div class="p-6">
          <div class="bg-slate-100 dark:bg-slate-700 rounded-lg h-96 flex items-center justify-center">
            <div class="text-center">
              <MapPinIcon class="mx-auto h-12 w-12 text-slate-400" />
              <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">Map integration coming soon</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Active Deliveries -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-6 border-b border-slate-200 dark:border-slate-700">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Active Deliveries</h2>
          </div>
          <div class="p-6">
            <div v-if="activeDeliveries.length === 0" class="text-center py-8">
              <TruckIcon class="mx-auto h-10 w-10 text-slate-400" />
              <p class="mt-2 text-sm text-slate-500 dark:text-slate-400">No active deliveries</p>
            </div>

            <div v-else class="space-y-4">
              <div
                v-for="delivery in activeDeliveries"
                :key="delivery.id"
                class="border border-slate-200 dark:border-slate-700 rounded-lg p-4"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <h3 class="text-sm font-medium text-slate-900 dark:text-white">{{ delivery.driverName }}</h3>
                    <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">Order #{{ delivery.orderId }}</p>
                    <div class="mt-3 flex items-center space-x-2">
                      <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
                        {{ delivery.status }}
                      </span>
                      <span class="text-xs text-slate-600 dark:text-slate-400">
                        ETA: {{ formatTime(delivery.eta) }}
                      </span>
                    </div>
                  </div>
                  <button
                    @click="trackDelivery(delivery)"
                    class="text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300"
                  >
                    <MapPinIcon class="w-5 h-5" />
                  </button>
                </div>

                <!-- Progress Bar -->
                <div class="mt-4">
                  <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
                    <div
                      class="bg-blue-600 h-2 rounded-full transition-all duration-300"
                      :style="{ width: delivery.progress + '%' }"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Delivery History -->
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-6 border-b border-slate-200 dark:border-slate-700">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Recent Deliveries</h2>
          </div>
          <div class="p-6">
            <div class="space-y-3">
              <div
                v-for="delivery in recentDeliveries"
                :key="delivery.id"
                class="flex items-center justify-between py-3 border-b border-slate-200 dark:border-slate-700 last:border-0"
              >
                <div>
                  <p class="text-sm font-medium text-slate-900 dark:text-white">Order #{{ delivery.orderId }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-400">{{ formatDate(delivery.deliveredAt) }}</p>
                </div>
                <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                  Delivered
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
  MapPinIcon
} from '@heroicons/vue/24/outline'

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: 'Live Tracking - TOSS ERP',
  meta: [
    { name: 'description', content: 'Track deliveries in real-time' }
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

