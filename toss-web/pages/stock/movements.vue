<template>
  <div class="p-6 space-y-6 bg-gray-50 dark:bg-gray-900 min-h-screen">
    <!-- Page Header -->
    <div class="sm:flex sm:items-center">
      <div class="sm:flex-auto">
        <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">Stock Movements</h1>
        <p class="mt-2 text-sm text-gray-700 dark:text-gray-300">
          Track all stock transactions including receipts, issues, transfers, and adjustments.
        </p>
      </div>
      <div class="mt-4 sm:mt-0 sm:ml-16 sm:flex-none">
        <button
          @click="openCreateModal"
          type="button"
          class="inline-flex items-center justify-center rounded-lg bg-blue-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-blue-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600"
        >
          <PlusIcon class="w-5 h-5 mr-2" />
          New Movement
        </button>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-4">
      <button
        @click="newMovement('receipt')"
        class="inline-flex items-center justify-center rounded-lg bg-green-50 dark:bg-green-900 px-4 py-3 text-sm font-semibold text-green-700 dark:text-green-300 hover:bg-green-100 dark:hover:bg-green-800 border border-green-200 dark:border-green-700"
      >
        <ArrowDownIcon class="w-5 h-5 mr-2" />
        Stock Receipt
      </button>
      
      <button
        @click="newMovement('issue')"
        class="inline-flex items-center justify-center rounded-lg bg-red-50 dark:bg-red-900 px-4 py-3 text-sm font-semibold text-red-700 dark:text-red-300 hover:bg-red-100 dark:hover:bg-red-800 border border-red-200 dark:border-red-700"
      >
        <ArrowUpIcon class="w-5 h-5 mr-2" />
        Stock Issue
      </button>
      
      <button
        @click="newMovement('transfer')"
        class="inline-flex items-center justify-center rounded-lg bg-blue-50 dark:bg-blue-900 px-4 py-3 text-sm font-semibold text-blue-700 dark:text-blue-300 hover:bg-blue-100 dark:hover:bg-blue-800 border border-blue-200 dark:border-blue-700"
      >
        <ArrowRightIcon class="w-5 h-5 mr-2" />
        Stock Transfer
      </button>
      
      <button
        @click="newMovement('adjustment')"
        class="inline-flex items-center justify-center rounded-lg bg-yellow-50 dark:bg-yellow-900 px-4 py-3 text-sm font-semibold text-yellow-700 dark:text-yellow-300 hover:bg-yellow-100 dark:hover:bg-yellow-800 border border-yellow-200 dark:border-yellow-700"
      >
        <AdjustmentsHorizontalIcon class="w-5 h-5 mr-2" />
        Stock Adjustment
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm p-6">
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-5">
        <div>
          <label for="search" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
          <div class="relative">
            <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
            <input
              id="search"
              v-model="searchQuery"
              type="text"
              placeholder="Search movements..."
              class="pl-10 w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
        </div>

        <div>
          <label for="type-filter" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Type</label>
          <select
            id="type-filter"
            v-model="selectedType"
            class="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-white focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          >
            <option value="">All Types</option>
            <option value="receipt">Receipt</option>
            <option value="issue">Issue</option>
            <option value="transfer">Transfer</option>
            <option value="adjustment">Adjustment</option>
          </select>
        </div>

        <div>
          <label for="warehouse-filter" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Warehouse</label>
          <select
            id="warehouse-filter"
            v-model="selectedWarehouse"
            class="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-white focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          >
            <option value="">All Warehouses</option>
            <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
              {{ warehouse.name }}
            </option>
          </select>
        </div>

        <div>
          <label for="date-filter" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Date Range</label>
          <select
            id="date-filter"
            v-model="selectedDateRange"
            class="w-full rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 px-3 py-2 text-sm text-gray-900 dark:text-white focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          >
            <option value="">All Time</option>
            <option value="today">Today</option>
            <option value="week">This Week</option>
            <option value="month">This Month</option>
            <option value="quarter">This Quarter</option>
            <option value="year">This Year</option>
          </select>
        </div>

        <div class="flex items-end space-x-2">
          <button
            @click="exportMovements"
            type="button"
            class="flex-1 rounded-lg bg-green-600 px-3 py-2 text-sm font-semibold text-white hover:bg-green-500"
          >
            Export CSV
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

    <!-- Movements Table -->
    <div class="bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 shadow-sm overflow-hidden">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Movements</h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Date & Reference
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Type
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Item
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Warehouse
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Quantity
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Value
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                Status
              </th>
              <th scope="col" class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="movement in paginatedMovements" :key="movement.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ formatDate(movement.createdAt) }}
                  </div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">
                    {{ movement.reference || `#${movement.id.slice(-8)}` }}
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="{
                    'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': movement.type === 'receipt',
                    'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200': movement.type === 'issue',
                    'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200': movement.type === 'transfer',
                    'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200': movement.type === 'adjustment'
                  }"
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium capitalize"
                >
                  {{ movement.type }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div>
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
                    {{ movement.itemName }}
                  </div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">
                    {{ movement.itemCode }}
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ movement.warehouseName }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="{
                    'text-green-600 dark:text-green-400': movement.quantity > 0,
                    'text-red-600 dark:text-red-400': movement.quantity < 0
                  }"
                  class="text-sm font-medium"
                >
                  {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                R{{ formatCurrency(movement.totalValue || 0) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="{
                    'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': movement.status === 'completed',
                    'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200': movement.status === 'pending',
                    'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200': movement.status === 'cancelled'
                  }"
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium capitalize"
                >
                  {{ movement.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button
                  @click="viewMovement(movement)"
                  class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300"
                >
                  View
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700 px-6 py-3">
        <div class="flex items-center justify-between">
          <div class="flex items-center text-sm text-gray-700 dark:text-gray-300">
            <span>Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, filteredMovements.length) }} of {{ filteredMovements.length }} movements</span>
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArrowDownIcon,
  ArrowUpIcon,
  ArrowRightIcon,
  AdjustmentsHorizontalIcon
} from '@heroicons/vue/24/outline'
import { useStock, type StockMovementDto, type WarehouseDto } from '../../composables/useStock'

// Composables
const { getStockMovements, getWarehouses } = useStock()

// Reactive data
const movements = ref<StockMovementDto[]>([])
const warehouses = ref<WarehouseDto[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Filters
const searchQuery = ref('')
const selectedType = ref('')
const selectedWarehouse = ref('')
const selectedDateRange = ref('')

// Pagination
const currentPage = ref(1)
const pageSize = ref(10)

// Computed
const filteredMovements = computed(() => {
  let filtered = movements.value

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(movement =>
      movement.itemName?.toLowerCase().includes(query) ||
      movement.itemCode?.toLowerCase().includes(query) ||
      movement.warehouseName?.toLowerCase().includes(query) ||
      movement.reference?.toLowerCase().includes(query)
    )
  }

  // Type filter
  if (selectedType.value) {
    filtered = filtered.filter(movement => movement.type === selectedType.value)
  }

  // Warehouse filter
  if (selectedWarehouse.value) {
    filtered = filtered.filter(movement => movement.warehouseId === selectedWarehouse.value)
  }

  // Date range filter (simplified - in a real app would use proper date filtering)
  if (selectedDateRange.value) {
    const now = new Date()
    const filterDate = new Date()
    
    switch (selectedDateRange.value) {
      case 'today':
        filterDate.setHours(0, 0, 0, 0)
        break
      case 'week':
        filterDate.setDate(now.getDate() - 7)
        break
      case 'month':
        filterDate.setMonth(now.getMonth() - 1)
        break
      case 'quarter':
        filterDate.setMonth(now.getMonth() - 3)
        break
      case 'year':
        filterDate.setFullYear(now.getFullYear() - 1)
        break
    }
    
    filtered = filtered.filter(movement => new Date(movement.createdAt) >= filterDate)
  }

  return filtered
})

const paginatedMovements = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredMovements.value.slice(start, end)
})

const totalPages = computed(() => Math.ceil(filteredMovements.value.length / pageSize.value))

// Methods
const loadMovements = async () => {
  try {
    loading.value = true
    error.value = null
    const response = await getStockMovements()
    movements.value = response.movements || response.stockMovements || []
  } catch (err) {
    error.value = 'Failed to load stock movements'
    console.error('Error loading stock movements:', err)
  } finally {
    loading.value = false
  }
}

const loadWarehouses = async () => {
  try {
    const response = await getWarehouses()
    warehouses.value = response.warehouses
  } catch (err) {
    console.error('Error loading warehouses:', err)
  }
}

const clearFilters = () => {
  searchQuery.value = ''
  selectedType.value = ''
  selectedWarehouse.value = ''
  selectedDateRange.value = ''
  currentPage.value = 1
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Modal state
const showMovementModal = ref(false)
const selectedMovementType = ref<'receipt' | 'issue' | 'transfer' | 'adjustment'>('receipt')
const items = ref<any[]>([])

// Action methods
const openCreateModal = () => {
  selectedMovementType.value = 'receipt'
  showMovementModal.value = true
}

const newMovement = (type: 'receipt' | 'issue' | 'transfer' | 'adjustment') => {
  selectedMovementType.value = type
  showMovementModal.value = true
}

const viewMovement = (movement: StockMovementDto) => {
  const details = `
Stock Movement Details

Reference: ${movement.reference || movement.voucherNo}
Type: ${movement.movementType}
Item: ${movement.itemName} (${movement.itemSku})
Warehouse: ${movement.warehouseName}
Quantity: ${movement.quantity > 0 ? '+' : ''}${movement.quantity}
Rate: R${movement.rate?.toFixed(2) || 'N/A'}
Amount: R${movement.amount?.toFixed(2) || 'N/A'}
Date: ${formatDate(movement.transactionDate)}
Balance After: ${movement.balanceQty}
  `
  alert(details)
}

const closeMovementModal = () => {
  showMovementModal.value = false
}

const saveMovement = async (data: any) => {
  try {
    // In a real app, this would call the API
    console.log('Saving stock movement:', data)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 500))
    
    alert('Stock movement created successfully!')
    showMovementModal.value = false
    
    // Reload movements
    await loadMovements()
  } catch (error) {
    console.error('Error saving movement:', error)
    alert('Failed to save stock movement. Please try again.')
  }
}

const exportMovements = async () => {
  const exportData = filteredMovements.value.map(movement => ({
    'Reference': movement.reference || movement.voucherNo,
    'Type': movement.movementType,
    'Item': movement.itemName,
    'SKU': movement.itemSku,
    'Warehouse': movement.warehouseName,
    'Quantity': movement.quantity,
    'Rate (R)': movement.rate || 0,
    'Amount (R)': movement.amount || 0,
    'Date': formatDate(movement.transactionDate),
    'Balance': movement.balanceQty
  }))

  const csvContent = [
    Object.keys(exportData[0]).join(','),
    ...exportData.map(row => Object.values(row).join(','))
  ].join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `stock-movements-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  window.URL.revokeObjectURL(url)
  
  alert('Stock movements exported successfully!')
}

// Load items for the modal
const loadItems = async () => {
  try {
    const { getItems } = await import('../../composables/useStock')
    const { getItems: getItemsFn } = getItems ? { getItems } : useStock()
    const response = await getItemsFn({ page: 1, pageSize: 1000 })
    items.value = response.items
  } catch (error) {
    console.error('Error loading items:', error)
  }
}

// Lifecycle
onMounted(() => {
  loadMovements()
  loadWarehouses()
  loadItems()
})
</script>

<!-- Stock Movement Modal -->
<StockMovementModal
  v-if="showMovementModal"
  :movement-type="selectedMovementType"
  :items="items"
  :warehouses="warehouses"
  @close="closeMovementModal"
  @save="saveMovement"
/>
