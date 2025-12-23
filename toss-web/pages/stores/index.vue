<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-indigo-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
        <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-indigo-600 to-purple-600 bg-clip-text text-transparent truncate">
              Stores Dashboard
            </h1>
            <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400 line-clamp-1">
              Manage your township stores and locations
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <NuxtLink 
              to="/stores/create" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 bg-gradient-to-r from-indigo-600 to-purple-600 text-white rounded-xl hover:from-indigo-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 text-xs sm:text-sm font-semibold whitespace-nowrap"
            >
              <BuildingStorefrontIcon class="w-4 h-4 sm:mr-2" />
              <span class="hidden sm:inline">New Store</span>
            </NuxtLink>
            <button 
              @click="refreshStores" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 rounded-xl text-xs sm:text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 hover:shadow-md transition-all duration-200 whitespace-nowrap"
              title="Refresh"
            >
              <ArrowPathIcon class="w-4 h-4 sm:mr-2" :class="{ 'animate-spin': loading }" />
              <span class="hidden sm:inline">Refresh</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8 space-y-4 sm:space-y-6">
      <!-- Stats Grid -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
        <!-- Total Stores -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Total Stores</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">{{ totalStores }}</p>
              <p class="text-xs sm:text-sm text-indigo-600 mt-1">{{ activeStores }} active</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl shadow-lg flex-shrink-0">
              <BuildingStorefrontIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <!-- Total Customers -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Customers</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">{{ totalCustomers }}</p>
              <p class="text-xs sm:text-sm text-blue-600 mt-1">Across all stores</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl shadow-lg flex-shrink-0">
              <UsersIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <!-- Total Products -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Products</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">{{ totalProducts }}</p>
              <p class="text-xs sm:text-sm text-green-600 mt-1">Stock items</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl shadow-lg flex-shrink-0">
              <CubeIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <!-- Total Revenue -->
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Total Revenue</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">R{{ formatCurrency(totalRevenue) }}</p>
              <p class="text-xs sm:text-sm text-purple-600 mt-1">This month</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-purple-500 to-blue-600 rounded-xl shadow-lg flex-shrink-0">
              <CurrencyDollarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1">
            <input v-model="searchQuery" type="text" placeholder="Search stores..." 
                   class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
          </div>
          <div class="flex gap-2 sm:gap-3">
            <select v-model="statusFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Stores</option>
              <option value="active">Active Only</option>
              <option value="inactive">Inactive Only</option>
            </select>
            <select v-model="areaFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-indigo-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Areas</option>
              <option value="soweto">Soweto</option>
              <option value="alexandra">Alexandra</option>
              <option value="diepsloot">Diepsloot</option>
              <option value="orange-farm">Orange Farm</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Stores Grid -->
      <div v-if="filteredStores.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
        <div v-for="store in filteredStores" :key="store.id" 
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300 hover:-translate-y-1 cursor-pointer"
          @click="navigateToStore(store.id)"
        >
          <!-- Store Header -->
          <div class="bg-gradient-to-r from-indigo-50 to-purple-50 dark:from-indigo-900/20 dark:to-purple-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center">
                    <span class="text-lg font-bold text-white">{{ store.name.charAt(0) }}</span>
                  </div>
                </div>
                <div class="flex-1 min-w-0">
                  <h3 class="text-lg font-bold text-slate-900 dark:text-white truncate">{{ store.name }}</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400 truncate">{{ store.areaGroup || 'No area' }}</p>
                </div>
              </div>
              <span 
                v-if="store.isActive"
                class="px-3 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"
              >
                Active
              </span>
              <span 
                v-else
                class="px-3 py-1 rounded-full text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400"
              >
                Inactive
              </span>
            </div>
          </div>

          <!-- Store Stats -->
          <div class="px-6 py-4">
            <div class="grid grid-cols-3 gap-4 mb-4">
              <div class="text-center">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Customers</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">{{ store.customerCount || 0 }}</p>
              </div>
              <div class="text-center">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Products</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">{{ store.productCount || 0 }}</p>
              </div>
              <div class="text-center">
                <p class="text-xs text-slate-500 dark:text-slate-400 mb-1">Revenue</p>
                <p class="text-lg font-bold text-indigo-600 dark:text-indigo-400">R{{ formatCurrency(store.totalRevenue || 0) }}</p>
              </div>
            </div>

            <!-- Store Features -->
            <div class="flex flex-wrap gap-2 mb-4">
              <span v-if="store.whatsAppAlertsEnabled" class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                <span class="mr-1">💬</span> WhatsApp
              </span>
              <span v-if="store.groupBuyingEnabled" class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
                <span class="mr-1">🛒</span> Group Buy
              </span>
              <span v-if="store.aiAssistantEnabled" class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">
                <span class="mr-1">🤖</span> AI Assistant
              </span>
            </div>

            <!-- Store Info -->
            <div class="space-y-2 text-sm">
              <div v-if="store.contactPhone" class="flex items-center text-slate-600 dark:text-slate-400">
                <PhoneIcon class="w-4 h-4 mr-2 flex-shrink-0" />
                <span class="truncate">{{ store.contactPhone }}</span>
              </div>
              <div v-if="store.email" class="flex items-center text-slate-600 dark:text-slate-400">
                <EnvelopeIcon class="w-4 h-4 mr-2 flex-shrink-0" />
                <span class="truncate">{{ store.email }}</span>
              </div>
              <div v-if="store.addressStreet" class="flex items-center text-slate-600 dark:text-slate-400">
                <MapPinIcon class="w-4 h-4 mr-2 flex-shrink-0" />
                <span class="truncate">{{ store.addressStreet }}, {{ store.addressCity }}</span>
              </div>
              <div v-if="store.openingTime && store.closingTime" class="flex items-center text-slate-600 dark:text-slate-400">
                <ClockIcon class="w-4 h-4 mr-2 flex-shrink-0" />
                <span>{{ formatTime(store.openingTime) }} - {{ formatTime(store.closingTime) }}</span>
              </div>
            </div>
          </div>

          <!-- Store Actions -->
          <div class="px-6 py-4 bg-slate-50 dark:bg-slate-900/50 border-t border-slate-200 dark:border-slate-700">
            <div class="flex justify-between items-center">
              <NuxtLink 
                :to="`/stores/${store.id}`"
                class="text-indigo-600 hover:text-indigo-800 dark:text-indigo-400 dark:hover:text-indigo-200 text-sm font-medium flex items-center transition-colors"
                @click.stop
              >
                <PencilSquareIcon class="w-4 h-4 mr-1" />
                Edit
              </NuxtLink>
              <button 
                @click.stop="toggleStoreStatus(store)"
                class="text-slate-600 hover:text-slate-800 dark:text-slate-400 dark:hover:text-slate-200 text-sm font-medium flex items-center transition-colors"
              >
                <PowerIcon class="w-4 h-4 mr-1" />
                {{ store.isActive ? 'Deactivate' : 'Activate' }}
              </button>
              <button 
                @click.stop="deleteStore(store)"
                class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-200 text-sm font-medium flex items-center transition-colors"
              >
                <TrashIcon class="w-4 h-4 mr-1" />
                Delete
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <BuildingStorefrontIcon class="w-16 h-16 text-slate-400 mx-auto mb-4" />
        <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No stores found</p>
        <p class="text-slate-600 dark:text-slate-400 mb-4">Create your first store to get started.</p>
        <NuxtLink
          to="/stores/create"
          class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-indigo-600 to-purple-600 text-white rounded-xl hover:from-indigo-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
        >
          <BuildingStorefrontIcon class="w-5 h-5 mr-2" />
          Create Store
        </NuxtLink>
      </div>

      <!-- Area Distribution -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
        <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4 sm:mb-6">Stores by Area</h3>
        <div class="space-y-4">
          <div v-for="area in areaDistribution" :key="area.name">
            <div class="flex justify-between text-xs sm:text-sm mb-2">
              <span class="text-slate-600 dark:text-slate-400">{{ area.name }}</span>
              <span class="font-medium text-slate-900 dark:text-white">{{ area.count }} stores ({{ area.percentage }}%)</span>
            </div>
            <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
              <div class="bg-gradient-to-r from-indigo-500 to-purple-600 h-3 rounded-full transition-all duration-500" :style="{ width: `${area.percentage}%` }"></div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { 
  BuildingStorefrontIcon,
  UsersIcon,
  CubeIcon,
  CurrencyDollarIcon,
  ArrowPathIcon,
  PhoneIcon,
  EnvelopeIcon,
  MapPinIcon,
  ClockIcon,
  PencilSquareIcon,
  PowerIcon,
  TrashIcon
} from '@heroicons/vue/24/outline'
import { useStoresAPI } from '~/composables/useStoresAPI'
import type { Store } from '~/types/stores'

// Page metadata
useHead({
  title: 'Stores Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage township stores and locations' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// API
const storesAPI = useStoresAPI()
const router = useRouter()

// State
const loading = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const areaFilter = ref('')
const stores = ref<Store[]>([])

// Load stores on mount
onMounted(async () => {
  await loadStores()
})

const loadStores = async () => {
  loading.value = true
  try {
    stores.value = await storesAPI.getStores()
  } catch (error) {
    console.error('Failed to load stores:', error)
  } finally {
    loading.value = false
  }
}

const refreshStores = async () => {
  await loadStores()
}

// Computed
const totalStores = computed(() => stores.value.length)
const activeStores = computed(() => stores.value.filter(s => s.isActive).length)
const totalCustomers = computed(() => stores.value.reduce((sum, s) => sum + (s.customerCount || 0), 0))
const totalProducts = computed(() => stores.value.reduce((sum, s) => sum + (s.productCount || 0), 0))
const totalRevenue = computed(() => stores.value.reduce((sum, s) => sum + (s.totalRevenue || 0), 0))

const filteredStores = computed(() => {
  let filtered = stores.value

  if (searchQuery.value) {
    const search = searchQuery.value.toLowerCase()
    filtered = filtered.filter(s => 
      s.name.toLowerCase().includes(search) ||
      s.description?.toLowerCase().includes(search) ||
      s.companyName?.toLowerCase().includes(search)
    )
  }

  if (statusFilter.value === 'active') {
    filtered = filtered.filter(s => s.isActive)
  } else if (statusFilter.value === 'inactive') {
    filtered = filtered.filter(s => !s.isActive)
  }

  if (areaFilter.value) {
    filtered = filtered.filter(s => s.areaGroup?.toLowerCase() === areaFilter.value.toLowerCase())
  }

  return filtered
})

const areaDistribution = computed(() => {
  const areas = stores.value.reduce((acc, store) => {
    const area = store.areaGroup || 'Unassigned'
    acc[area] = (acc[area] || 0) + 1
    return acc
  }, {} as Record<string, number>)

  const total = stores.value.length || 1
  return Object.entries(areas).map(([name, count]) => ({
    name,
    count,
    percentage: Math.round((count / total) * 100)
  }))
})

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatTime = (time: string) => {
  if (!time) return ''
  const [hours, minutes] = time.split(':')
  const hour = parseInt(hours)
  const ampm = hour >= 12 ? 'PM' : 'AM'
  const displayHour = hour % 12 || 12
  return `${displayHour}:${minutes} ${ampm}`
}

const navigateToStore = (storeId: number) => {
  router.push(`/stores/${storeId}`)
}

const toggleStoreStatus = async (store: Store) => {
  if (confirm(`Are you sure you want to ${store.isActive ? 'deactivate' : 'activate'} ${store.name}?`)) {
    try {
      await storesAPI.updateStore(store.id, {
        ...store,
        isActive: !store.isActive
      })
      await loadStores()
      alert(`✓ Store ${store.isActive ? 'deactivated' : 'activated'} successfully`)
    } catch (error) {
      console.error('Failed to toggle store status:', error)
      alert('✗ Failed to update store status')
    }
  }
}

const deleteStore = async (store: Store) => {
  if (confirm(`Are you sure you want to delete ${store.name}? This action cannot be undone.`)) {
    try {
      await storesAPI.deleteStore(store.id)
      await loadStores()
      alert(`✓ Store deleted successfully`)
    } catch (error: any) {
      console.error('Failed to delete store:', error)
      alert(error.data?.message || '✗ Failed to delete store. It may have active data.')
    }
  }
}
</script>

