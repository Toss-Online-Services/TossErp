<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent">
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
        <div class="bg-gradient-to-r from-purple-50 to-blue-50 dark:from-purple-900/20 dark:to-blue-900/20 p-4 border-b border-slate-200 dark:border-slate-700">
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
          <div class="bg-gradient-to-br from-purple-100 to-blue-100 dark:from-purple-900/20 dark:to-blue-900/20 rounded-xl h-96 flex items-center justify-center relative overflow-hidden">
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

    <!-- Tracking Detail Modal -->
    <Transition name="modal">
      <div v-if="showTrackingModal && selectedDelivery" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4">
          <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-2xl">
            
            <!-- Header -->
            <div class="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <div>
                <h3 class="text-xl font-bold text-white">Delivery Tracking</h3>
                <p class="text-sm text-white/80">Order #{{ selectedDelivery.orderId }}</p>
              </div>
              <button @click="showTrackingModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <XMarkIcon class="w-6 h-6 text-white" />
              </button>
            </div>

            <!-- Content -->
            <div class="p-6 space-y-6">
              
              <!-- Driver Info -->
              <div class="bg-gradient-to-br from-purple-50 to-blue-50 dark:from-purple-900/20 dark:to-blue-900/20 rounded-xl p-4">
                <h4 class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-3">Driver</h4>
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 rounded-full bg-gradient-to-br from-purple-500 to-blue-600 flex items-center justify-center text-white font-bold text-lg">
                    {{ selectedDelivery.driverName.charAt(0) }}
                  </div>
                  <div>
                    <p class="font-semibold text-slate-900 dark:text-white">{{ selectedDelivery.driverName }}</p>
                    <p class="text-sm text-slate-600 dark:text-slate-400">Professional Driver</p>
                  </div>
                </div>
              </div>

              <!-- Status Timeline -->
              <div>
                <h4 class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-4">Delivery Status</h4>
                <div class="space-y-4">
                  <!-- Current Status -->
                  <div class="flex items-start gap-3">
                    <div class="flex-shrink-0 w-8 h-8 rounded-full bg-blue-100 dark:bg-blue-900/30 flex items-center justify-center">
                      <TruckIcon class="w-4 h-4 text-blue-600 dark:text-blue-400" />
                    </div>
                    <div class="flex-1">
                      <p class="font-medium text-slate-900 dark:text-white">{{ selectedDelivery.status }}</p>
                      <p class="text-sm text-slate-600 dark:text-slate-400">ETA: {{ formatTime(selectedDelivery.eta) }}</p>
                    </div>
                  </div>

                  <!-- Progress Bar -->
                  <div class="ml-4 pl-4 border-l-2 border-slate-200 dark:border-slate-700 py-2">
                    <div class="mb-2">
                      <div class="flex items-center justify-between mb-1.5">
                        <span class="text-xs font-medium text-slate-700 dark:text-slate-300">Delivery Progress</span>
                        <span class="text-xs font-bold text-purple-600 dark:text-purple-400">{{ selectedDelivery.progress }}%</span>
                      </div>
                      <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
                        <div
                          class="bg-gradient-to-r from-purple-600 to-blue-600 h-2 rounded-full transition-all duration-500"
                          :style="{ width: selectedDelivery.progress + '%' }"
                        ></div>
                      </div>
                    </div>
                  </div>

                  <!-- Previous Statuses -->
                  <div class="flex items-start gap-3">
                    <div class="flex-shrink-0 w-8 h-8 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
                      <CheckCircleIcon class="w-4 h-4 text-green-600 dark:text-green-400" />
                    </div>
                    <div class="flex-1">
                      <p class="font-medium text-slate-900 dark:text-white">Order Picked Up</p>
                      <p class="text-sm text-slate-600 dark:text-slate-400">{{ formatTime(new Date(Date.now() - 3600000)) }}</p>
                    </div>
                  </div>

                  <div class="flex items-start gap-3">
                    <div class="flex-shrink-0 w-8 h-8 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
                      <CheckCircleIcon class="w-4 h-4 text-green-600 dark:text-green-400" />
                    </div>
                    <div class="flex-1">
                      <p class="font-medium text-slate-900 dark:text-white">Order Confirmed</p>
                      <p class="text-sm text-slate-600 dark:text-slate-400">{{ formatTime(new Date(Date.now() - 7200000)) }}</p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Contact Actions -->
              <div class="grid grid-cols-2 gap-3 pt-4 border-t border-slate-200 dark:border-slate-700">
                <button class="flex items-center justify-center gap-2 px-4 py-3 bg-green-600 hover:bg-green-700 text-white rounded-xl font-semibold transition-colors">
                  <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                    <path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 01-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 01-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.825 9.825 0 012.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0012.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 005.683 1.448h.005c6.554 0 11.89-5.335 11.893-11.893a11.821 11.821 0 00-3.48-8.413Z"/>
                  </svg>
                  WhatsApp Driver
                </button>
                <button class="flex items-center justify-center gap-2 px-4 py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-xl font-semibold transition-colors">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                  </svg>
                  Call Driver
                </button>
              </div>
            </div>

            <!-- Footer -->
            <div class="border-t border-slate-200 dark:border-slate-700 px-6 py-4 bg-slate-50 dark:bg-slate-900/50 flex justify-end rounded-b-2xl">
              <button
                @click="showTrackingModal = false"
                class="px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 font-semibold hover:bg-slate-100 dark:hover:bg-slate-700 transition-all"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  TruckIcon,
  MapPinIcon,
  CheckCircleIcon,
  ClockIcon,
  XMarkIcon
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

// Tracking Modal State
const showTrackingModal = ref(false)
const selectedDelivery = ref<any>(null)

const trackDelivery = (delivery: any) => {
  selectedDelivery.value = delivery
  showTrackingModal.value = true
}
</script>

<style scoped>
/* Modal transition animations */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-active > div,
.modal-leave-active > div {
  transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from > div,
.modal-leave-to > div {
  transform: scale(0.95);
  opacity: 0;
}
</style>

