<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-slate-800 shadow-sm border-b border-slate-200 dark:border-slate-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Driver Interface</h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage your delivery runs and capture proof of delivery
            </p>
          </div>
          <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
            <span class="w-2 h-2 bg-green-600 rounded-full mr-2"></span>
            Active
          </span>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Current Run -->
      <div v-if="currentRun" class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 mb-8">
        <div class="p-6 border-b border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Current Run</h2>
            <button
              @click="completeRun"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-lg text-sm font-medium text-white bg-green-600 hover:bg-green-700"
            >
              <CheckCircleIcon class="w-5 h-5 mr-2" />
              Complete Run
            </button>
          </div>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Total Stops</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ currentRun.stops.length }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Completed</p>
              <p class="text-2xl font-bold text-green-600 dark:text-green-400">{{ currentRun.completedStops }}</p>
            </div>
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Remaining</p>
              <p class="text-2xl font-bold text-orange-600 dark:text-orange-400">{{ currentRun.stops.length - currentRun.completedStops }}</p>
            </div>
          </div>

          <!-- Stops List -->
          <div class="space-y-3">
            <div
              v-for="(stop, index) in currentRun.stops"
              :key="stop.id"
              class="border border-slate-200 dark:border-slate-700 rounded-lg p-4"
              :class="{ 'bg-green-50 dark:bg-green-900/10': stop.status === 'Delivered' }"
            >
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="flex items-center space-x-3">
                    <span class="flex items-center justify-center w-8 h-8 rounded-full bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 text-sm font-medium">
                      {{ index + 1 }}
                    </span>
                    <div>
                      <h3 class="text-sm font-medium text-slate-900 dark:text-white">{{ stop.shopName }}</h3>
                      <p class="text-xs text-slate-500 dark:text-slate-400">{{ stop.address }}</p>
                    </div>
                  </div>
                  <div class="mt-2 ml-11">
                    <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium"
                      :class="getStopStatusClass(stop.status)">
                      {{ stop.status }}
                    </span>
                    <span class="ml-2 text-xs text-slate-600 dark:text-slate-400">
                      {{ stop.items }} items
                    </span>
                  </div>
                </div>
                <button
                  v-if="stop.status !== 'Delivered'"
                  @click="capturePOD(stop)"
                  class="inline-flex items-center px-3 py-1.5 border border-transparent rounded-md text-xs font-medium text-white bg-blue-600 hover:bg-blue-700"
                >
                  <CameraIcon class="w-4 h-4 mr-1" />
                  Capture POD
                </button>
                <span v-else class="inline-flex items-center text-green-600 dark:text-green-400">
                  <CheckCircleIcon class="w-5 h-5" />
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- No Active Run -->
      <div v-else class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-12 text-center">
          <TruckIcon class="mx-auto h-12 w-12 text-slate-400" />
          <h3 class="mt-2 text-sm font-medium text-slate-900 dark:text-white">No Active Run</h3>
          <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">You don't have any active delivery runs at the moment.</p>
          <div class="mt-6">
            <button
              @click="viewAvailableRuns"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-lg text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
            >
              View Available Runs
            </button>
          </div>
        </div>
      </div>

      <!-- Earnings Summary -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <p class="text-sm text-slate-600 dark:text-slate-400">Today's Earnings</p>
          <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">R 850</p>
        </div>
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <p class="text-sm text-slate-600 dark:text-slate-400">This Week</p>
          <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">R 4,250</p>
        </div>
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <p class="text-sm text-slate-600 dark:text-slate-400">This Month</p>
          <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">R 18,900</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  TruckIcon,
  CheckCircleIcon,
  CameraIcon
} from '@heroicons/vue/24/outline'

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: 'Driver Interface - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage delivery runs and capture proof of delivery' }
  ]
})

const currentRun = ref({
  id: '1',
  completedStops: 2,
  stops: [
    {
      id: '1',
      shopName: 'Thabo\'s Spaza',
      address: '123 Main Street, Soweto',
      items: 5,
      status: 'Delivered'
    },
    {
      id: '2',
      shopName: 'Sarah\'s Store',
      address: '456 Church Road, Alexandra',
      items: 3,
      status: 'Delivered'
    },
    {
      id: '3',
      shopName: 'Lucky\'s Shop',
      address: '789 Market Street, Soweto',
      items: 4,
      status: 'Pending'
    },
    {
      id: '4',
      shopName: 'Grace\'s Mini Market',
      address: '321 Hill Drive, Alexandra',
      items: 6,
      status: 'Pending'
    }
  ]
})

const getStopStatusClass = (status: string): string => {
  const classes: Record<string, string> = {
    'Pending': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'En Route': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'Failed': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status] || ''
}

const capturePOD = (stop: any) => {
  // TODO: Implement POD capture modal (photo + PIN)
  console.log('Capture POD for:', stop)
}

const completeRun = () => {
  // TODO: Implement complete run functionality
  console.log('Complete run')
}

const viewAvailableRuns = () => {
  // TODO: Navigate to available runs
  navigateTo('/logistics/shared-runs')
}
</script>

