<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useLogisticsStore, type Driver } from '~/stores/logistics'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Drivers - TOSS'
})

const logisticsStore = useLogisticsStore()
const searchQuery = ref('')
const availabilityFilter = ref<'all' | 'available' | 'unavailable'>('all')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const selectedDriver = ref<Driver | null>(null)

// Computed
const filteredDrivers = computed(() => {
  let filtered = logisticsStore.activeDrivers

  if (availabilityFilter.value === 'available') {
    filtered = filtered.filter(d => d.isAvailable)
  } else if (availabilityFilter.value === 'unavailable') {
    filtered = filtered.filter(d => !d.isAvailable)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(d =>
      d.fullName.toLowerCase().includes(query) ||
      d.phone.toLowerCase().includes(query) ||
      (d.vehicleRegistration || '').toLowerCase().includes(query) ||
      (d.licenseNumber || '').toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => a.fullName.localeCompare(b.fullName))
})

const stats = computed(() => {
  const drivers = logisticsStore.drivers
  return {
    total: drivers.length,
    active: drivers.filter(d => d.isActive).length,
    available: drivers.filter(d => d.isActive && d.isAvailable).length,
    onDelivery: drivers.filter(d => d.isActive && !d.isAvailable).length
  }
})

// Methods
onMounted(async () => {
  await logisticsStore.fetchDrivers()
})

function handleCreate() {
  selectedDriver.value = null
  showCreateModal.value = true
}

function handleEdit(driver: Driver) {
  selectedDriver.value = driver
  showEditModal.value = true
}

function handleView(driver: Driver) {
  navigateTo(`/logistics/drivers/${driver.id}`)
}

async function handleToggleAvailability(driver: Driver) {
  await logisticsStore.updateDriver(driver.id, {
    isAvailable: !driver.isAvailable
  })
  await logisticsStore.fetchDrivers()
}

function handleDriverSaved() {
  showCreateModal.value = false
  showEditModal.value = false
  selectedDriver.value = null
  logisticsStore.fetchDrivers()
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">Drivers</h3>
          <p class="text-gray-600 text-sm">Manage delivery drivers and vehicles</p>
        </div>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Driver</span>
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Drivers</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">local_shipping</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Active</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.active }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">person</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Available</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.available }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">On Delivery</p>
            <p class="mt-2 text-3xl font-bold text-orange-600">{{ stats.onDelivery }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">delivery_dining</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-card p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <div class="relative">
            <i class="material-symbols-rounded absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400">search</i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by name, phone, vehicle, or license..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
        </div>
        <div class="md:w-48">
          <select
            v-model="availabilityFilter"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="all">All Drivers</option>
            <option value="available">Available</option>
            <option value="unavailable">Unavailable</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Drivers Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="logisticsStore.loading" class="flex items-center justify-center py-12">
        <div class="text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
          <p class="text-gray-600">Loading drivers...</p>
        </div>
      </div>

      <div v-else-if="filteredDrivers.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">local_shipping</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No drivers found</h3>
        <p class="text-gray-600 mb-6">Get started by adding your first driver</p>
        <button
          @click="handleCreate"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Driver</span>
        </button>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Driver</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Contact</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Vehicle</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Areas</th>
              <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="driver in filteredDrivers"
              :key="driver.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div>
                  <p class="text-sm font-semibold text-gray-900">{{ driver.fullName }}</p>
                  <p v-if="driver.licenseNumber" class="text-xs text-gray-500">License: {{ driver.licenseNumber }}</p>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ driver.phone }}</div>
                <div v-if="driver.email" class="text-xs text-gray-500">{{ driver.email }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div v-if="driver.vehicleType || driver.vehicleRegistration">
                  <p class="text-sm text-gray-900">{{ driver.vehicleType || '-' }}</p>
                  <p v-if="driver.vehicleRegistration" class="text-xs text-gray-500">{{ driver.vehicleRegistration }}</p>
                </div>
                <span v-else class="text-sm text-gray-400">-</span>
              </td>
              <td class="px-6 py-4">
                <div v-if="driver.areas && driver.areas.length > 0" class="flex flex-wrap gap-1">
                  <span
                    v-for="area in driver.areas.slice(0, 2)"
                    :key="area"
                    class="px-2 py-1 text-xs font-medium text-gray-700 bg-gray-100 rounded"
                  >
                    {{ area }}
                  </span>
                  <span
                    v-if="driver.areas.length > 2"
                    class="px-2 py-1 text-xs font-medium text-gray-500"
                  >
                    +{{ driver.areas.length - 2 }}
                  </span>
                </div>
                <span v-else class="text-sm text-gray-400">-</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-center">
                <div class="flex items-center justify-center gap-2">
                  <span
                    :class="[
                      'px-2 py-1 text-xs font-medium rounded-full',
                      driver.isAvailable ? 'text-green-600 bg-green-100' : 'text-orange-600 bg-orange-100'
                    ]"
                  >
                    {{ driver.isAvailable ? 'Available' : 'On Delivery' }}
                  </span>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleView(driver)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="View"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                  <button
                    @click="handleEdit(driver)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="Edit"
                  >
                    <i class="material-symbols-rounded text-lg">edit</i>
                  </button>
                  <button
                    @click="handleToggleAvailability(driver)"
                    :class="[
                      'p-2 rounded-lg transition-colors',
                      driver.isAvailable
                        ? 'text-orange-600 hover:text-orange-900 hover:bg-orange-100'
                        : 'text-green-600 hover:text-green-900 hover:bg-green-100'
                    ]"
                    :title="driver.isAvailable ? 'Mark as unavailable' : 'Mark as available'"
                  >
                    <i class="material-symbols-rounded text-lg">
                      {{ driver.isAvailable ? 'pause_circle' : 'play_circle' }}
                    </i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modals -->
    <ClientOnly>
      <LogisticsDriverModal
        v-if="showCreateModal || showEditModal"
        :show="showCreateModal || showEditModal"
        :driver="selectedDriver"
        @close="showCreateModal = false; showEditModal = false; selectedDriver = null"
        @saved="handleDriverSaved"
      />
    </ClientOnly>
  </div>
</template>
