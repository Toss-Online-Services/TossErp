<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-slate-800 shadow-sm border-b border-slate-200 dark:border-slate-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Shared Delivery Runs</h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Coordinate deliveries with other businesses to save on shipping costs
            </p>
          </div>
          <button
            @click="createRun"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-lg text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
          >
            <TruckIcon class="w-5 h-5 mr-2" />
            Create Run
          </button>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Active Runs</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.activeRuns }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
              <TruckIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Total Savings</p>
              <p class="mt-2 text-3xl font-bold text-green-600 dark:text-green-400">{{ formatCurrency(stats.totalSavings) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-lg">
              <CurrencyDollarIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Scheduled</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.scheduled }}</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900/30 rounded-lg">
              <ClockIcon class="w-8 h-8 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Completed</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.completed }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900/30 rounded-lg">
              <CheckCircleIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
        </div>
      </div>

      <!-- Runs List -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-6 border-b border-slate-200 dark:border-slate-700">
          <h2 class="text-lg font-semibold text-slate-900 dark:text-white">Available Runs</h2>
        </div>
        <div class="p-6">
          <div v-if="runs.length === 0" class="text-center py-12">
            <TruckIcon class="mx-auto h-12 w-12 text-slate-400" />
            <h3 class="mt-2 text-sm font-medium text-slate-900 dark:text-white">No runs available</h3>
            <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">Get started by creating a new delivery run.</p>
          </div>

          <div v-else class="space-y-4">
            <div
              v-for="run in runs"
              :key="run.id"
              class="border border-slate-200 dark:border-slate-700 rounded-lg p-4 hover:border-blue-500 dark:hover:border-blue-400 transition-all cursor-pointer"
              @click="viewRun(run)"
            >
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <div class="flex items-center space-x-3">
                    <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                      :class="getStatusClass(run.status)">
                      {{ run.status }}
                    </span>
                    <h3 class="text-lg font-medium text-slate-900 dark:text-white">{{ run.driverName }}</h3>
                  </div>
                  <div class="mt-2 flex items-center space-x-4 text-sm text-slate-600 dark:text-slate-400">
                    <span>{{ run.stops.length }} stops</span>
                    <span>{{ run.totalDistance }} km</span>
                    <span>{{ formatDate(run.scheduledDate) }}</span>
                  </div>
                </div>
                <div class="text-right">
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ formatCurrency(run.baseFee) }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-400">{{ run.availableSlots }} slots available</p>
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
import { ref } from 'vue'
import {
  TruckIcon,
  CurrencyDollarIcon,
  ClockIcon,
  CheckCircleIcon
} from '@heroicons/vue/24/outline'

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: 'Shared Delivery Runs - TOSS ERP',
  meta: [
    { name: 'description', content: 'Coordinate deliveries and save on shipping costs' }
  ]
})

// Mock data
const stats = ref({
  activeRuns: 5,
  totalSavings: 12450,
  scheduled: 8,
  completed: 124
})

const runs = ref([
  {
    id: '1',
    driverName: 'Thabo Molefe',
    status: 'Scheduled',
    stops: [{ id: '1' }, { id: '2' }, { id: '3' }],
    totalDistance: 45,
    scheduledDate: new Date(Date.now() + 86400000),
    baseFee: 850,
    availableSlots: 2
  },
  {
    id: '2',
    driverName: 'Sarah Ndlovu',
    status: 'En Route',
    stops: [{ id: '4' }, { id: '5' }],
    totalDistance: 28,
    scheduledDate: new Date(),
    baseFee: 620,
    availableSlots: 1
  }
])

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(amount).replace('ZAR', 'R')
}

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

const getStatusClass = (status: string): string => {
  const classes: Record<string, string> = {
    'Scheduled': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'En Route': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'Completed': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'Cancelled': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status] || ''
}

const createRun = () => {
  // TODO: Implement create run modal
  console.log('Create new run')
}

const viewRun = (run: any) => {
  // TODO: Implement run details view
  console.log('View run:', run)
}
</script>

