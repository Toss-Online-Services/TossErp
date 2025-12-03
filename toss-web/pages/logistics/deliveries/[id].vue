<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useLogisticsStore, type DeliveryRun, type DeliveryStatus } from '~/stores/logistics'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Delivery Run Details - TOSS'
})

const route = useRoute()
const logisticsStore = useLogisticsStore()

const run = ref<DeliveryRun | null>(null)
const loading = ref(false)

const runId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadRun() {
  loading.value = true
  try {
    await logisticsStore.fetchDeliveryRuns()
    run.value = logisticsStore.getDeliveryRunById(runId.value) || null
  } catch (error) {
    console.error('Failed to load delivery run:', error)
  } finally {
    loading.value = false
  }
}

async function handleUpdateStatus(status: DeliveryStatus) {
  if (!run.value) return
  
  if (status === 'in_transit' && run.value.status !== 'assigned') {
    alert('Delivery must be assigned to a driver first')
    return
  }
  
  if (confirm(`Are you sure you want to update status to ${status}?`)) {
    await logisticsStore.updateDeliveryStatus(run.value.id, status)
    await loadRun()
  }
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function formatDateTime(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
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

function getStopStatusColor(status: DeliveryStatus) {
  return getStatusColor(status)
}

function getStopStatusLabel(status: DeliveryStatus) {
  return getStatusLabel(status)
}

onMounted(() => {
  loadRun()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading delivery run details...</p>
      </div>
    </div>

    <div v-else-if="!run" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">local_shipping</i>
      <h3 class="text-lg font-semibold text-gray-900 mb-2">Delivery run not found</h3>
      <p class="text-gray-600 mb-6">The delivery run you're looking for doesn't exist</p>
      <NuxtLink
        to="/logistics/deliveries"
        class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
      >
        <i class="material-symbols-rounded text-lg">arrow_back</i>
        <span>Back to Deliveries</span>
      </NuxtLink>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-8">
        <div class="flex items-center gap-4 mb-4">
          <NuxtLink
            to="/logistics/deliveries"
            class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <i class="material-symbols-rounded text-xl">arrow_back</i>
          </NuxtLink>
          <div class="flex-1">
            <div class="flex items-center gap-4">
              <div>
                <h3 class="text-3xl font-bold text-gray-900">{{ run.runNumber }}</h3>
                <p class="text-gray-600 text-sm">Delivery run details and stops</p>
              </div>
              <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(run.status)]">
                {{ getStatusLabel(run.status) }}
              </span>
            </div>
          </div>
          <div class="flex items-center gap-2">
            <button
              v-if="run.status === 'assigned'"
              @click="handleUpdateStatus('in_transit')"
              class="inline-flex items-center gap-2 px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">play_circle</i>
              <span>Start Delivery</span>
            </button>
            <button
              v-if="run.status === 'in_transit'"
              @click="handleUpdateStatus('completed')"
              class="inline-flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">check_circle</i>
              <span>Complete</span>
            </button>
            <button
              @click="window.print()"
              class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
              title="Print"
            >
              <i class="material-symbols-rounded text-lg">print</i>
            </button>
          </div>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Total Stops</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ run.stops.length }}</p>
            </div>
            <div class="p-3 bg-blue-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-blue-600">location_on</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Total Cost</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ formatCurrency(run.totalDeliveryCost) }}</p>
            </div>
            <div class="p-3 bg-green-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-green-600">attach_money</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Distance</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ run.totalDistance.toFixed(1) }} km</p>
            </div>
            <div class="p-3 bg-orange-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-orange-600">straighten</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Participants</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ run.participantCount }}</p>
            </div>
            <div class="p-3 bg-purple-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-purple-600">group</i>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Run Information -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Run Information</h4>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <p class="text-sm text-gray-600">Scheduled Date</p>
                <p class="text-sm font-medium text-gray-900">{{ formatDate(run.scheduledDate) }}</p>
              </div>
              <div v-if="run.driverName">
                <p class="text-sm text-gray-600">Driver</p>
                <p class="text-sm font-medium text-gray-900">{{ run.driverName }}</p>
              </div>
              <div v-if="run.areaGroup">
                <p class="text-sm text-gray-600">Area Group</p>
                <p class="text-sm font-medium text-gray-900">{{ run.areaGroup }}</p>
              </div>
              <div>
                <p class="text-sm text-gray-600">Cost per Stop</p>
                <p class="text-sm font-medium text-gray-900">{{ formatCurrency(run.costPerStop) }}</p>
              </div>
              <div v-if="run.startedAt">
                <p class="text-sm text-gray-600">Started At</p>
                <p class="text-sm font-medium text-gray-900">{{ formatDateTime(run.startedAt) }}</p>
              </div>
              <div v-if="run.completedAt">
                <p class="text-sm text-gray-600">Completed At</p>
                <p class="text-sm font-medium text-gray-900">{{ formatDateTime(run.completedAt) }}</p>
              </div>
            </div>
          </div>

          <!-- Delivery Stops -->
          <div class="bg-white rounded-xl shadow-card overflow-hidden">
            <div class="p-6 border-b border-gray-200">
              <h4 class="text-lg font-semibold text-gray-900">Delivery Stops</h4>
            </div>
            <div v-if="run.stops.length === 0" class="p-12 text-center">
              <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">location_off</i>
              <p class="text-gray-600">No stops assigned to this run</p>
            </div>
            <div v-else class="divide-y divide-gray-200">
              <div
                v-for="(stop, index) in run.stops"
                :key="stop.id"
                class="p-6 hover:bg-gray-50 transition-colors"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center gap-3 mb-2">
                      <div class="flex items-center justify-center w-8 h-8 rounded-full bg-gray-900 text-white text-sm font-semibold">
                        {{ stop.sequenceNumber }}
                      </div>
                      <div>
                        <h5 class="text-sm font-semibold text-gray-900">{{ stop.shopName }}</h5>
                        <p class="text-xs text-gray-600">{{ stop.address }}</p>
                      </div>
                    </div>
                    <div v-if="stop.purchaseOrderNumber" class="mt-2">
                      <p class="text-xs text-gray-600">PO: <span class="font-medium">{{ stop.purchaseOrderNumber }}</span></p>
                    </div>
                    <div v-if="stop.deliveryInstructions" class="mt-2">
                      <p class="text-xs text-gray-600">{{ stop.deliveryInstructions }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-semibold text-gray-900">{{ formatCurrency(stop.costShare) }}</p>
                    <span :class="['mt-2 inline-block px-2 py-1 text-xs font-medium rounded-full', getStopStatusColor(stop.status)]">
                      {{ getStopStatusLabel(stop.status) }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="space-y-6">
          <!-- Notes -->
          <div v-if="run.notes" class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Notes</h4>
            <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ run.notes }}</p>
          </div>

          <!-- Timeline -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Timeline</h4>
            <div class="space-y-4">
              <div v-if="run.assignedDate" class="flex items-start gap-3">
                <div class="w-2 h-2 rounded-full bg-blue-600 mt-2"></div>
                <div>
                  <p class="text-sm font-medium text-gray-900">Assigned</p>
                  <p class="text-xs text-gray-600">{{ formatDateTime(run.assignedDate) }}</p>
                </div>
              </div>
              <div v-if="run.startedAt" class="flex items-start gap-3">
                <div class="w-2 h-2 rounded-full bg-orange-600 mt-2"></div>
                <div>
                  <p class="text-sm font-medium text-gray-900">Started</p>
                  <p class="text-xs text-gray-600">{{ formatDateTime(run.startedAt) }}</p>
                </div>
              </div>
              <div v-if="run.completedAt" class="flex items-start gap-3">
                <div class="w-2 h-2 rounded-full bg-green-600 mt-2"></div>
                <div>
                  <p class="text-sm font-medium text-gray-900">Completed</p>
                  <p class="text-xs text-gray-600">{{ formatDateTime(run.completedAt) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

