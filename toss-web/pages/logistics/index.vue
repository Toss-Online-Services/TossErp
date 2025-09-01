<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Logistics Management</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Community-driven deliveries (Uber/Mr D style) and supply chain orchestration</p>
      </div>

      <!-- Driver Onboarding & Availability -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 lg:col-span-1">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Join as Driver</h3>
          <div class="space-y-3">
            <input v-model="driver.fullName" class="form-input" placeholder="Full name" />
            <input v-model="driver.phone" class="form-input" placeholder="Phone" />
            <select v-model="driver.vehicleType" class="form-input">
              <option value="">Select vehicle type</option>
              <option value="bakkie">Bakkie</option>
              <option value="truck">Truck</option>
              <option value="van">Van</option>
              <option value="bike">Bike</option>
            </select>
            <button :disabled="loading.register" @click="registerDriver" class="btn-primary w-full">
              {{ loading.register ? 'Registering...' : (driverId ? 'Re-register' : 'Register as Driver') }}
            </button>
            <div v-if="driverId" class="text-xs text-green-600 dark:text-green-400">Driver ID: {{ driverId }}</div>
          </div>
          <div class="mt-6 border-t border-gray-200 dark:border-gray-700 pt-4">
            <div class="flex items-center justify-between">
              <span class="text-sm text-gray-700 dark:text-gray-300">Availability</span>
              <label class="inline-flex items-center cursor-pointer">
                <input type="checkbox" class="sr-only" v-model="availability" @change="toggleAvailability" :disabled="!driverId || loading.availability" />
                <span class="relative inline-block w-10 mr-2 align-middle select-none transition duration-200 ease-in">
                  <span :class="['absolute block w-6 h-6 transform bg-white rounded-full shadow left-0 top-0 transition', availability ? 'translate-x-4' : '']"></span>
                  <span :class="['block overflow-hidden h-6 rounded-full', availability ? 'bg-green-500' : 'bg-gray-300']"></span>
                </span>
                <span class="text-sm text-gray-700 dark:text-gray-300">{{ availability ? 'Online' : 'Offline' }}</span>
              </label>
            </div>
          </div>
        </div>

        <!-- Jobs: Available -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 lg:col-span-2">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Available Jobs</h3>
            <button class="btn-secondary" @click="loadJobs" :disabled="loading.jobs">Refresh</button>
          </div>
          <div v-if="availableJobs.length === 0" class="text-sm text-gray-500 dark:text-gray-400">No open jobs right now.</div>
          <div v-else class="space-y-3">
            <div v-for="job in availableJobs" :key="job.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 flex items-center justify-between">
              <div>
                <div class="text-sm text-gray-700 dark:text-gray-300">Pickup: <span class="font-medium">{{ job.pickup?.name }}</span></div>
                <div class="text-sm text-gray-700 dark:text-gray-300">Dropoff: <span class="font-medium">{{ job.dropoff?.name }}</span></div>
                <div class="text-xs text-gray-500 dark:text-gray-400">Weight: {{ job.weightKg }}kg • Payout: R{{ job.payout }}</div>
              </div>
              <div class="flex items-center gap-2">
                <button class="btn-primary" @click="accept(job.id)" :disabled="!driverId || loading.accept">Accept</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Assigned Jobs -->
      <div class="mt-8 bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">My Assigned Jobs</h3>
          <button class="btn-secondary" @click="loadAssigned" :disabled="loading.assigned">Refresh</button>
        </div>
        <div v-if="assignedJobs.length === 0" class="text-sm text-gray-500 dark:text-gray-400">No assigned jobs yet.</div>
        <div v-else class="space-y-3">
          <div v-for="job in assignedJobs" :key="job.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-center justify-between">
              <div>
                <div class="text-sm text-gray-700 dark:text-gray-300">Pickup: <span class="font-medium">{{ job.pickup?.name }}</span></div>
                <div class="text-sm text-gray-700 dark:text-gray-300">Dropoff: <span class="font-medium">{{ job.dropoff?.name }}</span></div>
                <div class="text-xs text-gray-500 dark:text-gray-400">Status: {{ job.status }}</div>
              </div>
              <div class="flex items-center gap-2">
                <button class="btn-secondary" @click="track(job.id)">Track</button>
                <button class="btn-success" @click="markDelivered(job.id)">Mark Delivered</button>
              </div>
            </div>
            <div v-if="tracking[job.id]" class="mt-2 text-xs text-gray-600 dark:text-gray-400">
              Last location: lat {{ tracking[job.id].driverLocation.lat }}, lng {{ tracking[job.id].driverLocation.lng }} ({{ tracking[job.id].lastUpdate }})
            </div>
          </div>
        </div>
      </div>

      <!-- Supply Chain Shortcuts -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Shipments</h3>
          <p class="text-gray-600 dark:text-gray-400">Track inbound and outbound shipments, consolidate routes.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Shared Warehousing</h3>
          <p class="text-gray-600 dark:text-gray-400">Book shared storage space in community hubs.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Provider Marketplace</h3>
          <p class="text-gray-600 dark:text-gray-400">Add drivers now; expand to couriers and maintenance providers.</p>
        </div>
      </div>
    </div>
  </div>
  </template>

<script setup>
const driver = reactive({ fullName: '', phone: '', vehicleType: '' })
const driverId = useCookie('drv-id')
const availability = ref(false)
const availableJobs = ref([])
const assignedJobs = ref([])
const tracking = reactive({})
const loading = reactive({ register: false, availability: false, jobs: false, assigned: false, accept: false })

useHead({ title: 'Logistics Management - TOSS ERP III' })

async function registerDriver() {
  if (!driver.fullName || !driver.vehicleType) return
  loading.register = true
  try {
    const res = await $fetch('/api/logistics/drivers/register', { method: 'POST', body: driver })
    driverId.value = res.id
  } finally { loading.register = false }
}

async function toggleAvailability() {
  if (!driverId.value) return
  loading.availability = true
  try {
    await $fetch('/api/logistics/drivers/availability', { method: 'POST', body: { driverId: driverId.value, available: availability.value } })
  } finally { loading.availability = false }
}

async function loadJobs() {
  loading.jobs = true
  try {
    const res = await $fetch('/api/logistics/jobs', { query: { status: 'open' } })
    availableJobs.value = res
  } finally { loading.jobs = false }
}

async function loadAssigned() {
  loading.assigned = true
  try {
    const res = await $fetch('/api/logistics/jobs', { query: { status: 'assigned' } })
    // Filter to my driver if id exists (mock allows any)
    assignedJobs.value = res.filter(j => !j.driverId || j.driverId === driverId.value)
  } finally { loading.assigned = false }
}

async function accept(id) {
  if (!driverId.value) return
  loading.accept = true
  try {
    await $fetch(`/api/logistics/jobs/${id}/accept`, { method: 'POST', body: { driverId: driverId.value } })
    await Promise.all([loadJobs(), loadAssigned()])
  } finally { loading.accept = false }
}

async function track(id) {
  const res = await $fetch(`/api/logistics/jobs/${id}/track`)
  tracking[id] = res
}

async function markDelivered(id) {
  await $fetch(`/api/logistics/jobs/${id}/status`, { method: 'POST', body: { status: 'delivered' } })
  await loadAssigned()
}

onMounted(async () => {
  await Promise.all([loadJobs(), loadAssigned()])
})
</script>
