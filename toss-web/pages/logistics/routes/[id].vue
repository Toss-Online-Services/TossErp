<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useLogisticsStore, type Route, type DeliveryRun } from '~/stores/logistics'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Route Details - TOSS'
})

const route = useRoute()
const logisticsStore = useLogisticsStore()

const routeData = ref<Route | null>(null)
const deliveryRuns = ref<DeliveryRun[]>([])
const loading = ref(false)

const routeId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadRoute() {
  loading.value = true
  try {
    await logisticsStore.fetchRoutes()
    routeData.value = logisticsStore.getRouteById(routeId.value) || null
    
    if (routeData.value && routeData.value.deliveryRunIds.length > 0) {
      await logisticsStore.fetchDeliveryRuns()
      deliveryRuns.value = logisticsStore.deliveryRuns.filter(
        run => routeData.value?.deliveryRunIds.includes(run.id)
      )
    }
  } catch (error) {
    console.error('Failed to load route:', error)
  } finally {
    loading.value = false
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

function formatDuration(minutes: number) {
  const hours = Math.floor(minutes / 60)
  const mins = minutes % 60
  if (hours > 0) {
    return `${hours}h ${mins}m`
  }
  return `${mins}m`
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    planned: 'text-gray-600 bg-gray-100',
    active: 'text-blue-600 bg-blue-100',
    completed: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    planned: 'Planned',
    active: 'Active',
    completed: 'Completed',
    cancelled: 'Cancelled'
  }
  return labels[status] || status
}

onMounted(() => {
  loadRoute()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading route details...</p>
      </div>
    </div>

    <div v-else-if="!routeData" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">route</i>
      <h3 class="text-lg font-semibold text-gray-900 mb-2">Route not found</h3>
      <p class="text-gray-600 mb-6">The route you're looking for doesn't exist</p>
      <NuxtLink
        to="/logistics/routes"
        class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
      >
        <i class="material-symbols-rounded text-lg">arrow_back</i>
        <span>Back to Routes</span>
      </NuxtLink>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-8">
        <div class="flex items-center gap-4 mb-4">
          <NuxtLink
            to="/logistics/routes"
            class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <i class="material-symbols-rounded text-xl">arrow_back</i>
          </NuxtLink>
          <div class="flex-1">
            <div class="flex items-center gap-4">
              <div>
                <h3 class="text-3xl font-bold text-gray-900">{{ routeData.routeName }}</h3>
                <p class="text-gray-600 text-sm">Route details and delivery runs</p>
              </div>
              <span
                v-if="routeData.optimized"
                class="px-3 py-1 text-sm font-medium text-purple-600 bg-purple-100 rounded-full"
              >
                <i class="material-symbols-rounded text-sm align-middle">tune</i> Optimized
              </span>
              <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(routeData.status)]">
                {{ getStatusLabel(routeData.status) }}
              </span>
            </div>
          </div>
          <button
            @click="window.print()"
            class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
            title="Print"
          >
            <i class="material-symbols-rounded text-lg">print</i>
          </button>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Total Stops</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">
                {{ routeData.completedStops }}/{{ routeData.totalStops }}
              </p>
            </div>
            <div class="p-3 bg-blue-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-blue-600">location_on</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Total Distance</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ routeData.totalDistance.toFixed(1) }} km</p>
            </div>
            <div class="p-3 bg-orange-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-orange-600">straighten</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Estimated Duration</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ formatDuration(routeData.estimatedDuration) }}</p>
            </div>
            <div class="p-3 bg-green-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-green-600">schedule</i>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600">Delivery Runs</p>
              <p class="mt-2 text-3xl font-bold text-gray-900">{{ deliveryRuns.length }}</p>
            </div>
            <div class="p-3 bg-purple-100 rounded-lg">
              <i class="material-symbols-rounded text-2xl text-purple-600">local_shipping</i>
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Route Information -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Route Information</h4>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <p class="text-sm text-gray-600">Date</p>
                <p class="text-sm font-medium text-gray-900">{{ formatDate(routeData.date) }}</p>
              </div>
              <div v-if="routeData.driverName">
                <p class="text-sm text-gray-600">Driver</p>
                <p class="text-sm font-medium text-gray-900">{{ routeData.driverName }}</p>
              </div>
              <div>
                <p class="text-sm text-gray-600">Status</p>
                <span :class="['inline-block px-2 py-1 text-xs font-medium rounded-full', getStatusColor(routeData.status)]">
                  {{ getStatusLabel(routeData.status) }}
                </span>
              </div>
              <div>
                <p class="text-sm text-gray-600">Optimized</p>
                <p class="text-sm font-medium text-gray-900">{{ routeData.optimized ? 'Yes' : 'No' }}</p>
              </div>
            </div>
          </div>

          <!-- Delivery Runs -->
          <div class="bg-white rounded-xl shadow-card overflow-hidden">
            <div class="p-6 border-b border-gray-200">
              <h4 class="text-lg font-semibold text-gray-900">Delivery Runs</h4>
            </div>
            <div v-if="deliveryRuns.length === 0" class="p-12 text-center">
              <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">local_shipping</i>
              <p class="text-gray-600">No delivery runs assigned to this route</p>
            </div>
            <div v-else class="divide-y divide-gray-200">
              <div
                v-for="run in deliveryRuns"
                :key="run.id"
                class="p-6 hover:bg-gray-50 transition-colors"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center gap-3 mb-2">
                      <h5 class="text-sm font-semibold text-gray-900">{{ run.runNumber }}</h5>
                      <span
                        :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(run.status)]"
                      >
                        {{ getStatusLabel(run.status) }}
                      </span>
                    </div>
                    <div class="grid grid-cols-2 gap-4 mt-2 text-xs text-gray-600">
                      <div>
                        <span class="font-medium">Stops:</span> {{ run.stops.length }}
                      </div>
                      <div>
                        <span class="font-medium">Area:</span> {{ run.areaGroup || '-' }}
                      </div>
                      <div>
                        <span class="font-medium">Date:</span> {{ formatDate(run.scheduledDate) }}
                      </div>
                      <div v-if="run.driverName">
                        <span class="font-medium">Driver:</span> {{ run.driverName }}
                      </div>
                    </div>
                  </div>
                  <button
                    @click="navigateTo(`/logistics/deliveries/${run.id}`)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="View"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="space-y-6">
          <!-- Route Stats -->
          <div class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Route Statistics</h4>
            <div class="space-y-3">
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Progress</span>
                <span class="text-sm font-semibold text-gray-900">
                  {{ Math.round((routeData.completedStops / routeData.totalStops) * 100) }}%
                </span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div
                  class="bg-green-600 h-2 rounded-full transition-all"
                  :style="{ width: `${(routeData.completedStops / routeData.totalStops) * 100}%` }"
                ></div>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Created</span>
                <span class="text-sm font-semibold text-gray-900">{{ formatDate(routeData.createdAt) }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-sm text-gray-600">Last Updated</span>
                <span class="text-sm font-semibold text-gray-900">{{ formatDate(routeData.updatedAt) }}</span>
              </div>
            </div>
          </div>

          <!-- Notes -->
          <div v-if="routeData.notes" class="bg-white rounded-xl shadow-card p-6">
            <h4 class="text-lg font-semibold text-gray-900 mb-4">Notes</h4>
            <p class="text-sm text-gray-700 whitespace-pre-wrap">{{ routeData.notes }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

