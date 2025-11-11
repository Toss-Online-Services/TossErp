<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-teal-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-teal-600 to-blue-600 bg-clip-text text-transparent">
              Logistics Dashboard
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              🚚 Shared deliveries, driver network, and supply chain coordination
            </p>
          </div>
        </div>
      </div>
    </div>

    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
      <!-- AI Copilot Banner -->
      <div class="bg-gradient-to-r from-teal-500 via-blue-500 to-indigo-500 rounded-2xl shadow-2xl p-6 text-white relative overflow-hidden">
        <div class="absolute top-0 right-0 w-64 h-64 bg-white/10 rounded-full -mr-32 -mt-32"></div>
        <div class="relative z-10 flex items-start gap-4">
          <div class="p-3 bg-white/20 backdrop-blur-sm rounded-xl flex-shrink-0">
            <SparklesIcon class="w-6 h-6" />
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-bold mb-2">💡 Logistics Insight</h3>
            <p class="text-white/90 text-sm leading-relaxed">
              <strong>3 drivers available</strong> in your area. <strong>5 pending deliveries</strong> can be consolidated into 2 shared runs, saving <strong>R450 (62%)</strong> on delivery fees.
            </p>
            <div class="flex gap-3 mt-4">
              <button class="px-5 py-2.5 bg-white/20 hover:bg-white/30 backdrop-blur-sm rounded-lg text-sm font-medium transition-all duration-200">
                Create Shared Runs →
              </button>
              <button class="px-5 py-2.5 bg-white/10 hover:bg-white/20 backdrop-blur-sm rounded-lg text-sm font-medium transition-all duration-200">
                View Details
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Stats Overview -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Active Drivers</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.activeDrivers }}</p>
              <p class="mt-1 text-xs text-green-600 dark:text-green-400">+2 this week</p>
            </div>
            <div class="p-3 bg-teal-100 dark:bg-teal-900/30 rounded-xl">
              <TruckIcon class="w-8 h-8 text-teal-600 dark:text-teal-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Active Runs</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.activeRuns }}</p>
              <p class="mt-1 text-xs text-blue-600 dark:text-blue-400">{{ stats.scheduledRuns }} scheduled</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-xl">
              <ClockIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Total Savings</p>
              <p class="mt-2 text-3xl font-bold text-green-600 dark:text-green-400">R{{ formatCurrency(stats.totalSavings) }}</p>
              <p class="mt-1 text-xs text-slate-500 dark:text-slate-400">This month</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-xl">
              <CurrencyDollarIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Deliveries</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ stats.totalDeliveries }}</p>
              <p class="mt-1 text-xs text-purple-600 dark:text-purple-400">{{ stats.completedToday }} today</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900/30 rounded-xl">
              <CheckCircleIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <NuxtLink to="/logistics/shared-runs" 
          class="group bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl hover:border-teal-500 dark:hover:border-teal-400 transition-all duration-300"
        >
          <div class="flex items-start gap-4">
            <div class="p-3 bg-gradient-to-br from-teal-500 to-blue-600 rounded-xl group-hover:scale-110 transition-transform duration-200">
              <TruckIcon class="w-6 h-6 text-white" />
            </div>
            <div class="flex-1">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white group-hover:text-teal-600 dark:group-hover:text-teal-400 transition-colors">
                Shared Delivery Runs
              </h3>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
                Coordinate deliveries, split costs, save money
              </p>
              <div class="mt-3 flex items-center text-sm text-teal-600 dark:text-teal-400 font-medium">
                View runs
                <svg class="ml-1 w-4 h-4 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </div>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink to="/logistics/driver"
          class="group bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl hover:border-blue-500 dark:hover:border-blue-400 transition-all duration-300"
        >
          <div class="flex items-start gap-4">
            <div class="p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl group-hover:scale-110 transition-transform duration-200">
              <UserIcon class="w-6 h-6 text-white" />
            </div>
            <div class="flex-1">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors">
                Driver Interface
              </h3>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
                Manage runs, capture POD, track earnings
              </p>
              <div class="mt-3 flex items-center text-sm text-blue-600 dark:text-blue-400 font-medium">
                Open driver portal
                <svg class="ml-1 w-4 h-4 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </div>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink to="/logistics/tracking"
          class="group bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl hover:border-purple-500 dark:hover:border-purple-400 transition-all duration-300"
        >
          <div class="flex items-start gap-4">
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl group-hover:scale-110 transition-transform duration-200">
              <MapPinIcon class="w-6 h-6 text-white" />
            </div>
            <div class="flex-1">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white group-hover:text-purple-600 dark:group-hover:text-purple-400 transition-colors">
                Live Tracking
              </h3>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
                Track deliveries in real-time
              </p>
              <div class="mt-3 flex items-center text-sm text-purple-600 dark:text-purple-400 font-medium">
                Track now
                <svg class="ml-1 w-4 h-4 group-hover:translate-x-1 transition-transform" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                </svg>
              </div>
            </div>
          </div>
        </NuxtLink>
      </div>

      <!-- Driver Network & Jobs Section -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Driver Registration -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center gap-3 mb-6">
            <div class="p-2 bg-teal-100 dark:bg-teal-900/30 rounded-lg">
              <UserIcon class="w-5 h-5 text-teal-600 dark:text-teal-400" />
            </div>
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Join as Driver</h3>
          </div>
          <div class="space-y-3">
            <input 
              v-model="driver.fullName" 
              class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-teal-500 dark:focus:ring-teal-400 focus:border-transparent transition-all" 
              placeholder="Full name" 
            />
            <input 
              v-model="driver.phone" 
              class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-teal-500 dark:focus:ring-teal-400 focus:border-transparent transition-all" 
              placeholder="Phone number" 
            />
            <select 
              v-model="driver.vehicleType" 
              class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-teal-500 dark:focus:ring-teal-400 focus:border-transparent transition-all"
            >
              <option value="">Select vehicle type</option>
              <option value="bakkie">Bakkie</option>
              <option value="truck">Truck</option>
              <option value="van">Van</option>
              <option value="bike">Motorbike</option>
            </select>
            <button 
              :disabled="loading.register" 
              @click="registerDriver" 
              class="w-full px-4 py-3 bg-gradient-to-r from-teal-600 to-blue-600 hover:from-teal-700 hover:to-blue-700 disabled:from-slate-400 disabled:to-slate-500 text-white rounded-lg font-semibold shadow-lg hover:shadow-xl transition-all duration-200 disabled:cursor-not-allowed"
            >
              {{ loading.register ? 'Registering...' : (driverId ? 'Update Profile' : 'Register as Driver') }}
            </button>
            <div v-if="driverId" class="text-xs text-green-600 dark:text-green-400 p-3 bg-green-50 dark:bg-green-900/20 rounded-lg">
              ✓ Driver ID: {{ driverId }}
            </div>
          </div>
          <div class="mt-6 pt-6 border-t border-slate-200 dark:border-slate-700">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-2">
                <div :class="['w-2 h-2 rounded-full', availability ? 'bg-green-500' : 'bg-slate-400']"></div>
                <span class="text-sm font-medium text-slate-700 dark:text-slate-300">
                  {{ availability ? 'Online' : 'Offline' }}
                </span>
              </div>
              <label class="relative inline-flex items-center cursor-pointer">
                <input 
                  type="checkbox" 
                  class="sr-only peer" 
                  v-model="availability" 
                  @change="toggleAvailability" 
                  :disabled="!driverId || loading.availability" 
                />
                <div class="w-11 h-6 bg-slate-300 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-teal-300 dark:peer-focus:ring-teal-800 rounded-full peer dark:bg-slate-700 peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-slate-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-slate-600 peer-checked:bg-green-600"></div>
              </label>
            </div>
          </div>
        </div>

        <!-- Available Jobs -->
        <div class="lg:col-span-2 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between mb-6">
            <div class="flex items-center gap-3">
              <div class="p-2 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
                <TruckIcon class="w-5 h-5 text-blue-600 dark:text-blue-400" />
              </div>
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Available Delivery Jobs</h3>
            </div>
            <button 
              class="px-4 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 bg-slate-100 dark:bg-slate-700 hover:bg-slate-200 dark:hover:bg-slate-600 rounded-lg transition-colors" 
              @click="loadJobs" 
              :disabled="loading.jobs"
            >
              {{ loading.jobs ? 'Loading...' : 'Refresh' }}
            </button>
          </div>
          
          <div v-if="availableJobs.length === 0" class="text-center py-12">
            <TruckIcon class="w-12 h-12 text-slate-400 mx-auto mb-3" />
            <p class="text-sm text-slate-500 dark:text-slate-400">No open jobs right now</p>
            <p class="text-xs text-slate-400 dark:text-slate-500 mt-1">Check back later or enable notifications</p>
          </div>
          
          <div v-else class="space-y-3">
            <div 
              v-for="job in availableJobs" 
              :key="job.id" 
              class="border border-slate-200 dark:border-slate-700 rounded-xl p-4 hover:border-teal-500 dark:hover:border-teal-400 hover:shadow-md transition-all duration-200"
            >
              <div class="flex items-start justify-between gap-4">
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-2">
                    <span class="px-2 py-0.5 text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 rounded-full">
                      {{ job.weightKg }}kg
                    </span>
                    <span class="text-xs text-slate-500 dark:text-slate-400">
                      ~ {{ job.distance || 15 }}km
                    </span>
                  </div>
                  <div class="space-y-1">
                    <div class="flex items-start gap-2">
                      <MapPinIcon class="w-4 h-4 text-green-600 dark:text-green-400 flex-shrink-0 mt-0.5" />
                      <div>
                        <p class="text-xs text-slate-500 dark:text-slate-400">Pickup</p>
                        <p class="text-sm font-medium text-slate-900 dark:text-white">{{ job.pickup?.name }}</p>
                      </div>
                    </div>
                    <div class="flex items-start gap-2">
                      <MapPinIcon class="w-4 h-4 text-red-600 dark:text-red-400 flex-shrink-0 mt-0.5" />
                      <div>
                        <p class="text-xs text-slate-500 dark:text-slate-400">Dropoff</p>
                        <p class="text-sm font-medium text-slate-900 dark:text-white">{{ job.dropoff?.name }}</p>
                      </div>
                    </div>
                  </div>
                  <div class="mt-3 flex items-center gap-3">
                    <span class="text-lg font-bold text-green-600 dark:text-green-400">R{{ job.payout }}</span>
                    <span class="text-xs text-slate-500 dark:text-slate-400">payout</span>
                  </div>
                </div>
                <button 
                  class="px-5 py-2.5 bg-gradient-to-r from-teal-600 to-blue-600 hover:from-teal-700 hover:to-blue-700 disabled:from-slate-400 disabled:to-slate-500 text-white rounded-lg font-semibold text-sm shadow-lg hover:shadow-xl transition-all duration-200 disabled:cursor-not-allowed whitespace-nowrap"
                  @click="accept(job.id)" 
                  :disabled="!driverId || loading.accept"
                >
                  Accept Job
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
  </template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import {
  TruckIcon,
  ClockIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  SparklesIcon,
  UserIcon,
  MapPinIcon
} from '@heroicons/vue/24/outline'

definePageMeta({
  layout: 'default'
})

useHead({ 
  title: 'Logistics Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage shared deliveries, driver network, and logistics' }
  ]
})

// Stats
const stats = ref({
  activeDrivers: 12,
  activeRuns: 5,
  scheduledRuns: 8,
  totalSavings: 12450,
  totalDeliveries: 234,
  completedToday: 15
})

// Driver State
const driver = reactive({ fullName: '', phone: '', vehicleType: '' })
const driverId = useCookie('drv-id')
const availability = ref(false)
const availableJobs = ref<any[]>([])
const assignedJobs = ref<any[]>([])
const tracking = reactive<Record<string, any>>({})
const loading = reactive({ register: false, availability: false, jobs: false, assigned: false, accept: false })

async function registerDriver() {
  if (!driver.fullName || !driver.vehicleType) return
  loading.register = true
  try {
    const res = await $fetch('/api/logistics/drivers/register', { method: 'POST', body: driver }) as any
    driverId.value = res.id
  } catch (error) {
    console.error('Failed to register driver:', error)
  } finally { 
    loading.register = false 
  }
}

async function toggleAvailability() {
  if (!driverId.value) return
  loading.availability = true
  try {
    await $fetch('/api/logistics/drivers/availability', { 
      method: 'POST', 
      body: { driverId: driverId.value, available: availability.value } 
    })
  } catch (error) {
    console.error('Failed to toggle availability:', error)
  } finally { 
    loading.availability = false 
  }
}

async function loadJobs() {
  loading.jobs = true
  try {
    const res = await $fetch('/api/logistics/jobs?status=open') as any
    availableJobs.value = res
  } catch (error) {
    console.error('Failed to load jobs:', error)
  } finally { 
    loading.jobs = false 
  }
}

async function loadAssigned() {
  loading.assigned = true
  try {
    const res = await $fetch('/api/logistics/jobs?status=assigned') as any
    // Filter to my driver if id exists (mock allows any)
    assignedJobs.value = res.filter((j: any) => !j.driverId || j.driverId === driverId.value)
  } catch (error) {
    console.error('Failed to load assigned jobs:', error)
  } finally { 
    loading.assigned = false 
  }
}

async function accept(id: string) {
  if (!driverId.value) return
  loading.accept = true
  try {
    await $fetch(`/api/logistics/jobs/${id}/accept`, { 
      method: 'POST', 
      body: { driverId: driverId.value } 
    })
    await Promise.all([loadJobs(), loadAssigned()])
  } catch (error) {
    console.error('Failed to accept job:', error)
  } finally { 
    loading.accept = false 
  }
}

async function track(id: string) {
  try {
    const res = await $fetch(`/api/logistics/jobs/${id}/track`) as any
    tracking[id] = res
  } catch (error) {
    console.error('Failed to track job:', error)
  }
}

async function markDelivered(id: string) {
  try {
    await $fetch(`/api/logistics/jobs/${id}/status`, { 
      method: 'POST', 
      body: { status: 'delivered' } 
    })
    await loadAssigned()
  } catch (error) {
    console.error('Failed to mark delivered:', error)
  }
}

// Helper Functions
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

onMounted(async () => {
  await Promise.all([loadJobs(), loadAssigned()])
})
</script>

