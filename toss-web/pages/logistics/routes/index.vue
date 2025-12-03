<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useLogisticsStore, type Route } from '~/stores/logistics'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Routes - TOSS'
})

const logisticsStore = useLogisticsStore()
const searchQuery = ref('')
const statusFilter = ref<'all' | 'planned' | 'active' | 'completed'>('all')

// Computed
const filteredRoutes = computed(() => {
  let filtered = logisticsStore.routes

  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(r => r.status === statusFilter.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(r =>
      r.routeName.toLowerCase().includes(query) ||
      (r.driverName || '').toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
})

const stats = computed(() => {
  const routes = logisticsStore.routes
  return {
    total: routes.length,
    planned: routes.filter(r => r.status === 'planned').length,
    active: routes.filter(r => r.status === 'active').length,
    completed: routes.filter(r => r.status === 'completed').length,
    optimized: routes.filter(r => r.optimized).length
  }
})

// Methods
onMounted(async () => {
  await logisticsStore.fetchRoutes()
  await logisticsStore.fetchDeliveryRuns()
})

function handleView(route: Route) {
  navigateTo(`/logistics/routes/${route.id}`)
}

function handleOptimize(route: Route) {
  // TODO: Implement route optimization
  alert('Route optimization coming soon')
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
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">Routes</h3>
          <p class="text-gray-600 text-sm">Plan and optimize delivery routes</p>
        </div>
        <button
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
          disabled
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Create Route</span>
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Routes</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">route</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Planned</p>
            <p class="mt-2 text-3xl font-bold text-gray-600">{{ stats.planned }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-600">schedule</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Active</p>
            <p class="mt-2 text-3xl font-bold text-blue-600">{{ stats.active }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">play_circle</i>
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

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Optimized</p>
            <p class="mt-2 text-3xl font-bold text-purple-600">{{ stats.optimized }}</p>
          </div>
          <div class="p-3 bg-purple-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-purple-600">tune</i>
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
              placeholder="Search by route name or driver..."
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
            <option value="planned">Planned</option>
            <option value="active">Active</option>
            <option value="completed">Completed</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Routes Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="logisticsStore.loading" class="flex items-center justify-center py-12">
        <div class="text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
          <p class="text-gray-600">Loading routes...</p>
        </div>
      </div>

      <div v-else-if="filteredRoutes.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">route</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No routes found</h3>
        <p class="text-gray-600">Routes will appear here when created</p>
      </div>

      <div v-else class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Route Name</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Driver</th>
              <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Stops</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Distance</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Duration</th>
              <th class="px-6 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="route in filteredRoutes"
              :key="route.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center gap-2">
                  <span class="text-sm font-semibold text-gray-900">{{ route.routeName }}</span>
                  <span
                    v-if="route.optimized"
                    class="px-2 py-0.5 text-xs font-medium text-purple-600 bg-purple-100 rounded"
                    title="Optimized route"
                  >
                    <i class="material-symbols-rounded text-xs align-middle">tune</i>
                  </span>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(route.date) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                {{ route.driverName || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-center text-sm text-gray-900">
                <div>
                  <span class="font-semibold">{{ route.completedStops }}</span>
                  <span class="text-gray-400">/</span>
                  <span>{{ route.totalStops }}</span>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm text-gray-900">
                {{ route.totalDistance.toFixed(1) }} km
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm text-gray-900">
                {{ formatDuration(route.estimatedDuration) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-center">
                <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(route.status)]">
                  {{ getStatusLabel(route.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleView(route)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="View"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                  <button
                    v-if="!route.optimized && route.status === 'planned'"
                    @click="handleOptimize(route)"
                    class="p-2 text-purple-600 hover:text-purple-900 hover:bg-purple-100 rounded-lg transition-colors"
                    title="Optimize Route"
                  >
                    <i class="material-symbols-rounded text-lg">tune</i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
