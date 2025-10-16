<template>
  <div class="p-6 space-y-6 bg-gray-50 dark:bg-gray-900 min-h-screen">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center">
      <div class="sm:flex-auto">
        <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">Warehouses</h1>
        <p class="mt-2 text-sm text-gray-700 dark:text-gray-300">
          Manage your warehouse locations and track inventory across multiple locations.
        </p>
      </div>
      <div class="mt-4 sm:mt-0 sm:ml-16 sm:flex-none">
        <button
          @click="openCreateModal"
          type="button"
          class="inline-flex items-center justify-center rounded-lg bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600"
        >
          <PlusIcon class="w-5 h-5 mr-2" />
          Add Warehouse
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
      <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <BuildingStorefrontIcon class="h-8 w-8 text-blue-600" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400 uppercase tracking-wide">Total Warehouses</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ warehouses.length }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <CheckCircleIcon class="h-8 w-8 text-green-600" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400 uppercase tracking-wide">Active Warehouses</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ activeWarehouses }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <CubeIcon class="h-8 w-8 text-purple-600" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400 uppercase tracking-wide">Total Items Stored</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">{{ totalItemsStored }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <CurrencyDollarIcon class="h-8 w-8 text-green-600" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400 uppercase tracking-wide">Total Stock Value</p>
            <p class="text-2xl font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(totalStockValue) }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm p-6">
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-4">
        <div>
          <label for="search" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
          <div class="relative">
            <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
            <input
              id="search"
              v-model="searchQuery"
              type="text"
              placeholder="Search warehouses..."
              class="pl-10 w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
        </div>

        <div>
          <label for="status-filter" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
          <select
            id="status-filter"
            v-model="selectedStatus"
            class="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-white focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          >
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>
        </div>

        <div>
          <label for="type-filter" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Type</label>
          <select
            id="type-filter"
            v-model="selectedType"
            class="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-white focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          >
            <option value="">All Types</option>
            <option value="main">Main Warehouse</option>
            <option value="branch">Branch Warehouse</option>
            <option value="transit">Transit Warehouse</option>
            <option value="store">Store</option>
          </select>
        </div>

        <div class="flex items-end space-x-2">
          <button
            @click="exportWarehouses"
            type="button"
            class="flex-1 rounded-lg bg-green-600 px-3 py-2 text-sm font-semibold text-white hover:bg-green-500"
          >
            Export
          </button>
          <button
            @click="clearFilters"
            type="button"
            class="flex-1 rounded-lg bg-gray-600 px-3 py-2 text-sm font-semibold text-white hover:bg-gray-500"
          >
            Clear
          </button>
        </div>
      </div>
    </div>

    <!-- Warehouses Grid -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2 xl:grid-cols-3">
      <div
        v-for="warehouse in filteredWarehouses"
        :key="warehouse.id"
        class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm hover:shadow-md transition-shadow duration-200 cursor-pointer"
        @click="openDetailsModal(warehouse)"
      >
        <div class="p-6">
          <!-- Header -->
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center">
              <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
                <BuildingStorefrontIcon class="h-6 w-6 text-blue-600 dark:text-blue-400" />
              </div>
              <div class="ml-3">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ warehouse.name }}</h3>
                <p class="text-sm text-gray-500 dark:text-gray-400">{{ warehouse.code }}</p>
              </div>
            </div>
            <div class="flex items-center space-x-2">
              <span
                :class="{
                  'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': warehouse.isActive,
                  'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200': !warehouse.isActive
                }"
                class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
              >
                {{ warehouse.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
          </div>

          <!-- Type and Description -->
          <div class="mb-4">
            <div class="flex items-center mb-2">
              <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200">
                {{ warehouse.type }}
              </span>
            </div>
            <p class="text-sm text-gray-600 dark:text-gray-400 line-clamp-2">
              {{ warehouse.description || 'No description available' }}
            </p>
          </div>

          <!-- Address -->
          <div class="mb-4">
            <div class="flex items-start">
              <MapPinIcon class="h-4 w-4 text-gray-400 mt-0.5 flex-shrink-0" />
              <div class="ml-2 text-sm text-gray-600 dark:text-gray-400">
                {{ warehouse.address || 'No address specified' }}
              </div>
            </div>
          </div>

          <!-- Stats -->
          <div class="grid grid-cols-2 gap-4 pt-4 border-t border-gray-200 dark:border-gray-700">
            <div>
              <p class="text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wide">Items</p>
              <p class="text-lg font-semibold text-gray-900 dark:text-white">{{ warehouse.itemCount || 0 }}</p>
            </div>
            <div>
              <p class="text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wide">Stock Value</p>
              <p class="text-lg font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(warehouse.stockValue || 0) }}</p>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="flex items-center justify-between pt-4 border-t border-gray-200 dark:border-gray-700 mt-4">
            <button
              @click.stop="openEditModal(warehouse)"
              class="inline-flex items-center px-3 py-1.5 border border-gray-300 dark:border-gray-600 rounded-lg text-xs font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <PencilIcon class="w-3 h-3 mr-1" />
              Edit
            </button>
            <button
              @click.stop="viewStock(warehouse)"
              class="inline-flex items-center px-3 py-1.5 border border-blue-300 dark:border-blue-600 rounded-lg text-xs font-medium text-blue-700 dark:text-blue-300 bg-blue-50 dark:bg-blue-900 hover:bg-blue-100 dark:hover:bg-blue-800"
            >
              <CubeIcon class="w-3 h-3 mr-1" />
              View Stock
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm px-6 py-3">
      <div class="flex items-center justify-between">
        <div class="flex items-center text-sm text-gray-700 dark:text-gray-300">
          <span>Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, filteredWarehouses.length) }} of {{ filteredWarehouses.length }} warehouses</span>
        </div>
        <div class="flex items-center space-x-2">
          <button
            @click="currentPage--"
            :disabled="currentPage <= 1"
            class="px-3 py-1 rounded-lg border border-gray-300 dark:border-gray-600 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Previous
          </button>
          <span class="text-sm text-gray-700 dark:text-gray-300">
            Page {{ currentPage }} of {{ totalPages }}
          </span>
          <button
            @click="currentPage++"
            :disabled="currentPage >= totalPages"
            class="px-3 py-1 rounded-lg border border-gray-300 dark:border-gray-600 text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Next
          </button>
        </div>
      </div>
    </div>
  </div>

  <!-- Warehouse Modal -->
  <WarehouseModal
    v-if="showModal"
    :warehouse="selectedWarehouse"
    :warehouses="warehouses"
    @close="closeModal"
    @save="handleWarehouseSaved"
  />

  <!-- Warehouse Details Modal -->
  <WarehouseDetailsModal
    v-if="showDetailsModal"
    :warehouse="selectedWarehouse"
    @close="closeDetailsModal"
    @edit="openEditModal"
    @deactivate="handleWarehouseDeactivate"
  />
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  BuildingStorefrontIcon,
  CheckCircleIcon,
  CubeIcon,
  CurrencyDollarIcon,
  MapPinIcon,
  PencilIcon
} from '@heroicons/vue/24/outline'
import { useStock, type WarehouseDto } from '../../composables/useStock'

// Composables
const { getWarehouses } = useStock()

// Reactive data
const warehouses = ref<WarehouseDto[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Filters
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedType = ref('')

// Pagination
const currentPage = ref(1)
const pageSize = ref(12)

// Modals
const showModal = ref(false)
const showDetailsModal = ref(false)
const selectedWarehouse = ref<WarehouseDto | null>(null)

// Computed
const filteredWarehouses = computed(() => {
  let filtered = warehouses.value

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(warehouse =>
      warehouse.name.toLowerCase().includes(query) ||
      warehouse.code.toLowerCase().includes(query) ||
      (warehouse.description && warehouse.description.toLowerCase().includes(query)) ||
      (warehouse.address && warehouse.address.toLowerCase().includes(query))
    )
  }

  // Status filter
  if (selectedStatus.value) {
    filtered = filtered.filter(warehouse => {
      if (selectedStatus.value === 'active') return warehouse.isActive
      if (selectedStatus.value === 'inactive') return !warehouse.isActive
      return true
    })
  }

  // Type filter
  if (selectedType.value) {
    filtered = filtered.filter(warehouse => warehouse.type === selectedType.value)
  }

  return filtered
})

const totalPages = computed(() => Math.ceil(filteredWarehouses.value.length / pageSize.value))

const activeWarehouses = computed(() => 
  warehouses.value.filter(w => w.isActive).length
)

const totalItemsStored = computed(() => 
  warehouses.value.reduce((sum, w) => sum + (w.itemCount || 0), 0)
)

const totalStockValue = computed(() => 
  warehouses.value.reduce((sum, w) => sum + (w.stockValue || 0), 0)
)

// Methods
const loadWarehouses = async () => {
  try {
    loading.value = true
    error.value = null
    const response = await getWarehouses()
    warehouses.value = response.warehouses
  } catch (err) {
    error.value = 'Failed to load warehouses'
    console.error('Error loading warehouses:', err)
  } finally {
    loading.value = false
  }
}

const clearFilters = () => {
  searchQuery.value = ''
  selectedStatus.value = ''
  selectedType.value = ''
  currentPage.value = 1
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

// Modal methods
const openCreateModal = () => {
  selectedWarehouse.value = null
  showModal.value = true
}

const openEditModal = (warehouse: WarehouseDto) => {
  selectedWarehouse.value = warehouse
  showDetailsModal.value = false
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
  selectedWarehouse.value = null
}

const openDetailsModal = (warehouse: WarehouseDto) => {
  selectedWarehouse.value = warehouse
  showDetailsModal.value = true
}

const closeDetailsModal = () => {
  showDetailsModal.value = false
  selectedWarehouse.value = null
}

const handleWarehouseSaved = async (data: any) => {
  try {
    console.log('Saving warehouse:', data)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 500))
    
    alert('Warehouse saved successfully!')
    closeModal()
    await loadWarehouses()
  } catch (error) {
    console.error('Error saving warehouse:', error)
    alert('Failed to save warehouse. Please try again.')
  }
}

const handleWarehouseDeactivate = async (warehouse: WarehouseDto) => {
  try {
    console.log('Deactivating warehouse:', warehouse)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 500))
    
    alert('Warehouse deactivated successfully!')
    closeDetailsModal()
    await loadWarehouses()
  } catch (error) {
    console.error('Error deactivating warehouse:', error)
    alert('Failed to deactivate warehouse. Please try again.')
  }
}

const viewStock = (warehouse: WarehouseDto) => {
  alert(`View stock for warehouse: ${warehouse.name}\nNavigate to stock levels filtered by this warehouse`)
}

const exportWarehouses = async () => {
  const exportData = filteredWarehouses.value.map(wh => ({
    'Code': wh.code,
    'Name': wh.name,
    'Type': wh.type || '',
    'Description': wh.description || '',
    'Address': wh.address || '',
    'Items': wh.itemCount || 0,
    'Stock Value (R)': wh.stockValue || 0,
    'Status': wh.isActive ? 'Active' : 'Inactive',
    'Is Group': wh.isGroup ? 'Yes' : 'No'
  }))

  const csvContent = [
    Object.keys(exportData[0]).join(','),
    ...exportData.map(row => Object.values(row).map(v => `"${v}"`).join(','))
  ].join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `warehouses-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  window.URL.revokeObjectURL(url)
  
  alert('Warehouses exported successfully!')
}

// Lifecycle
onMounted(() => {
  loadWarehouses()
})
</script>
