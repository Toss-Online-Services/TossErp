<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useLogisticsStore, type DeliveryRun, type DeliveryStatus } from '~/stores/logistics'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Deliveries - TOSS'
})

const logisticsStore = useLogisticsStore()
const searchQuery = ref('')
const statusFilter = ref<DeliveryStatus | 'all'>('all')
const showAssignDriverModal = ref(false)
const selectedRun = ref<DeliveryRun | null>(null)

// Computed
const filteredRuns = computed(() => {
  let filtered = logisticsStore.deliveryRuns

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(run => run.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(run =>
      run.runNumber.toLowerCase().includes(query) ||
      (run.driverName || '').toLowerCase().includes(query) ||
      (run.areaGroup || '').toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.scheduledDate).getTime() - new Date(a.scheduledDate).getTime())
})

const stats = computed(() => {
  const runs = logisticsStore.deliveryRuns
  return {
    total: runs.length,
    scheduled: runs.filter(r => r.status === 'scheduled').length,
    assigned: runs.filter(r => r.status === 'assigned').length,
    inTransit: runs.filter(r => r.status === 'in_transit').length,
    completed: runs.filter(r => r.status === 'completed').length
  }
})

// Methods
onMounted(async () => {
  await logisticsStore.fetchDeliveryRuns()
  await logisticsStore.fetchDrivers()
})

function handleView(run: DeliveryRun) {
  navigateTo(`/logistics/deliveries/${run.id}`)
}

function handleAssignDriver(run: DeliveryRun) {
  selectedRun.value = run
  showAssignDriverModal.value = true
}

async function handleAssignDriverToRun(driverId: string) {
  if (!selectedRun.value) return
  
  const success = await logisticsStore.assignDriverToRun(selectedRun.value.id, driverId)
  if (success) {
    showAssignDriverModal.value = false
    selectedRun.value = null
    await logisticsStore.fetchDeliveryRuns()
  } else {
    alert('Failed to assign driver. Driver may not be available.')
  }
}

async function handleUpdateStatus(run: DeliveryRun, status: DeliveryStatus) {
  if (status === 'in_transit' && run.status !== 'assigned') {
    alert('Delivery must be assigned to a driver first')
    return
  }
  
  if (confirm(`Are you sure you want to update status to ${status}?`)) {
    await logisticsStore.updateDeliveryStatus(run.id, status)
    await logisticsStore.fetchDeliveryRuns()
  }
}

function getStatusColor(status: DeliveryStatus) {
  const colors: Record<DeliveryStatus, string> = {
    scheduled: 'text-gray-600 bg-gray-100',
    assigned: 'text-blue-600 bg-blue-100',
    in_transit: 'text-orange-600 bg-orange-100',
    completed: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: DeliveryStatus) {
  const labels: Record<DeliveryStatus, string> = {
    scheduled: 'Scheduled',
    assigned: 'Assigned',
    in_transit: 'In Transit',
    completed: 'Completed',
    cancelled: 'Cancelled'
  }
  return labels[status] || status
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Delivery Runs</h3>
      <p class="text-gray-600 text-sm">Track and manage shared delivery runs</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Runs</p>
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
            <p class="text-sm font-medium text-gray-600">Scheduled</p>
            <p class="mt-2 text-3xl font-bold text-gray-600">{{ stats.scheduled }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">schedule</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Assigned</p>
            <p class="mt-2 text-3xl font-bold text-blue-600">{{ stats.assigned }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">person</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">In Transit</p>
            <p class="mt-2 text-3xl font-bold text-orange-600">{{ stats.inTransit }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">delivery_dining</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Completed</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.completed }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">check_circle</i>
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
              placeholder="Search by run number, driver, or area..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
        </div>
        <div class="md:w-48">
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="all">All Status</option>
            <option value="scheduled">Scheduled</option>
            <option value="assigned">Assigned</option>
            <option value="in_transit">In Transit</option>
            <option value="completed">Completed</option>
            <option value="cancelled">Cancelled</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Delivery Runs Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="logisticsStore.loading" class="flex items-center justify-center py-12">
        <div class="text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
          <p class="text-gray-600">Loading delivery runs...</p>
        </div>
      </div>

      <div v-else-if="filteredRuns.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">local_shipping</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No delivery runs found</h3>
        <p class="text-gray-600">Delivery runs will appear here when created</p>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Run #</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Scheduled Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Driver</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Area</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Stops</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Cost</th>
              <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="run in filteredRuns"
              :key="run.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="text-sm font-semibold text-gray-900">{{ run.runNumber }}</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(run.scheduledDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                {{ run.driverName || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                {{ run.areaGroup || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ run.stops.length }} stop{{ run.stops.length !== 1 ? 's' : '' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-semibold text-gray-900">
                {{ formatCurrency(run.totalDeliveryCost) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-center">
                <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(run.status)]">
                  {{ getStatusLabel(run.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleView(run)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="View"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                  <button
                    v-if="!run.driverId && run.status === 'scheduled'"
                    @click="handleAssignDriver(run)"
                    class="p-2 text-blue-600 hover:text-blue-900 hover:bg-blue-100 rounded-lg transition-colors"
                    title="Assign Driver"
                  >
                    <i class="material-symbols-rounded text-lg">person_add</i>
                  </button>
                  <button
                    v-if="run.status === 'assigned'"
                    @click="handleUpdateStatus(run, 'in_transit')"
                    class="p-2 text-orange-600 hover:text-orange-900 hover:bg-orange-100 rounded-lg transition-colors"
                    title="Start Delivery"
                  >
                    <i class="material-symbols-rounded text-lg">play_circle</i>
                  </button>
                  <button
                    v-if="run.status === 'in_transit'"
                    @click="handleUpdateStatus(run, 'completed')"
                    class="p-2 text-green-600 hover:text-green-900 hover:bg-green-100 rounded-lg transition-colors"
                    title="Complete"
                  >
                    <i class="material-symbols-rounded text-lg">check_circle</i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Assign Driver Modal -->
    <Teleport to="body">
      <Transition name="modal">
        <div
          v-if="showAssignDriverModal"
          class="fixed inset-0 z-50 overflow-y-auto"
          @click.self="showAssignDriverModal = false"
        >
          <div class="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
            <div class="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" @click="showAssignDriverModal = false"></div>

            <div class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
              <div class="bg-white px-4 pt-5 pb-4 sm:p-6">
                <div class="flex items-center justify-between mb-4">
                  <h3 class="text-xl font-bold text-gray-900">Assign Driver</h3>
                  <button
                    @click="showAssignDriverModal = false"
                    class="text-gray-400 hover:text-gray-600 transition-colors"
                  >
                    <i class="material-symbols-rounded text-2xl">close</i>
                  </button>
                </div>

                <div v-if="selectedRun" class="mb-4">
                  <p class="text-sm text-gray-600 mb-2">Run: <span class="font-semibold text-gray-900">{{ selectedRun.runNumber }}</span></p>
                  <p class="text-sm text-gray-600">Scheduled: <span class="font-semibold text-gray-900">{{ formatDate(selectedRun.scheduledDate) }}</span></p>
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Select Available Driver
                  </label>
                  <div v-if="logisticsStore.availableDrivers.length === 0" class="text-center py-4 text-gray-500">
                    <p>No available drivers</p>
                  </div>
                  <div v-else class="space-y-2 max-h-64 overflow-y-auto">
                    <button
                      v-for="driver in logisticsStore.availableDrivers"
                      :key="driver.id"
                      @click="handleAssignDriverToRun(driver.id)"
                      class="w-full text-left px-4 py-3 border border-gray-300 rounded-lg hover:bg-gray-50 hover:border-gray-900 transition-colors"
                    >
                      <div class="flex items-center justify-between">
                        <div>
                          <p class="font-semibold text-gray-900">{{ driver.fullName }}</p>
                          <p class="text-sm text-gray-600">{{ driver.phone }}</p>
                          <p v-if="driver.vehicleType" class="text-xs text-gray-500">{{ driver.vehicleType }} - {{ driver.vehicleRegistration }}</p>
                        </div>
                        <i class="material-symbols-rounded text-gray-400">chevron_right</i>
                      </div>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.modal-enter-active, .modal-leave-active {
  transition: opacity 0.3s;
}
.modal-enter-from, .modal-leave-to {
  opacity: 0;
}
</style>
