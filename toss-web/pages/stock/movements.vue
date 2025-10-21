<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="sm:flex sm:items-center sm:justify-between">
          <div>
            <h1 class="text-3xl font-bold bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent">
              Stock Movements
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Track all stock transactions including receipts, issues, transfers, and adjustments
            </p>
          </div>
          <div class="mt-4 sm:mt-0">
            <button
              @click="openCreateModal"
              type="button"
              class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              New Movement
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-6">

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <button
        @click="newMovement('receipt')"
        class="group relative inline-flex items-center justify-center rounded-xl px-6 py-4 text-sm font-semibold text-white shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 overflow-hidden bg-gradient-to-r from-green-500 to-emerald-600 hover:from-green-600 hover:to-emerald-700"
      >
        <div class="flex items-center relative z-10">
          <ArrowDownIcon class="w-6 h-6 mr-3" />
          <div class="text-left">
            <div class="text-lg font-bold">Stock IN ↓</div>
            <div class="text-xs text-white/90">Receiving inventory</div>
          </div>
        </div>
      </button>
      
      <button
        @click="newMovement('issue')"
        class="group relative inline-flex items-center justify-center rounded-xl px-6 py-4 text-sm font-semibold text-white shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 overflow-hidden bg-gradient-to-r from-red-500 to-pink-600 hover:from-red-600 hover:to-pink-700"
      >
        <div class="flex items-center relative z-10">
          <ArrowUpIcon class="w-6 h-6 mr-3" />
          <div class="text-left">
            <div class="text-lg font-bold">Stock OUT ↑</div>
            <div class="text-xs text-white/90">Removing inventory</div>
          </div>
        </div>
      </button>
      
      <button
        @click="newMovement('transfer')"
        class="group relative inline-flex items-center justify-center rounded-xl px-6 py-4 text-sm font-semibold text-white shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 overflow-hidden bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700"
      >
        <div class="flex items-center relative z-10">
          <ArrowRightIcon class="w-6 h-6 mr-3" />
          <div class="text-left">
            <div class="text-lg font-bold">Stock MOVED →</div>
            <div class="text-xs text-white/90">Between locations</div>
          </div>
        </div>
      </button>
      
      <button
        @click="newMovement('adjustment')"
        class="group relative inline-flex items-center justify-center rounded-xl px-6 py-4 text-sm font-semibold text-white shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 overflow-hidden bg-gradient-to-r from-orange-500 to-yellow-500 hover:from-orange-600 hover:to-yellow-600"
      >
        <div class="flex items-center relative z-10">
          <AdjustmentsHorizontalIcon class="w-6 h-6 mr-3" />
          <div class="text-left">
            <div class="text-lg font-bold">Stock FIXED ⇌</div>
            <div class="text-xs text-white/90">Correct mistakes</div>
          </div>
        </div>
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-4">
        <div>
          <label for="search" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Search</label>
          <div class="relative">
            <MagnifyingGlassIcon class="absolute left-4 top-1/2 transform -translate-y-1/2 h-5 w-5 text-slate-400" />
            <input
              id="search"
              v-model="searchQuery"
              type="text"
              placeholder="Search movements..."
              class="pl-11 w-full rounded-xl border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 px-4 py-2.5 text-sm text-slate-900 dark:text-white placeholder-slate-500 dark:placeholder-slate-400 focus:border-purple-500 focus:ring-2 focus:ring-purple-500 transition-all duration-200"
            />
          </div>
        </div>

        <div>
          <label for="type-filter" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Type</label>
          <select
            id="type-filter"
            v-model="selectedType"
            class="w-full rounded-xl border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 px-4 py-2.5 text-sm text-slate-900 dark:text-white focus:border-purple-500 focus:ring-2 focus:ring-purple-500 transition-all duration-200"
          >
            <option value="">All Types</option>
            <option value="receipt">Receipt</option>
            <option value="issue">Issue</option>
            <option value="transfer">Transfer</option>
            <option value="adjustment">Adjustment</option>
          </select>
        </div>

        <div>
          <label for="date-filter" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Date Range</label>
          <select
            id="date-filter"
            v-model="selectedDateRange"
            class="w-full rounded-xl border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 px-4 py-2.5 text-sm text-slate-900 dark:text-white focus:border-purple-500 focus:ring-2 focus:ring-purple-500 transition-all duration-200"
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
            class="flex-1 rounded-xl bg-gradient-to-r from-green-600 to-emerald-600 px-4 py-2.5 text-sm font-semibold text-white hover:from-green-700 hover:to-emerald-700 shadow-md hover:shadow-lg transition-all duration-200"
          >
            Export CSV
          </button>
          <button
            @click="clearFilters"
            type="button"
            class="flex-1 rounded-xl border-2 border-slate-300 dark:border-slate-600 px-4 py-2.5 text-sm font-semibold text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
          >
            Clear
          </button>
        </div>
      </div>
    </div>

    <!-- Movements Table -->
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
      <div class="px-6 py-5 border-b border-slate-200 dark:border-slate-700 bg-gradient-to-r from-slate-50 to-slate-100 dark:from-slate-700 dark:to-slate-800">
        <h3 class="text-lg font-bold text-slate-900 dark:text-white">Recent Movements</h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
          <thead class="bg-gradient-to-r from-slate-50 to-slate-100 dark:from-slate-700 dark:to-slate-800">
            <tr>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Date & Reference
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Type
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Item
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Quantity
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Value
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Status
              </th>
              <th scope="col" class="relative px-6 py-4">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
            <tr v-for="movement in paginatedMovements" :key="movement.id" class="hover:bg-gradient-to-r hover:from-purple-50/50 hover:to-blue-50/50 dark:hover:from-purple-900/10 dark:hover:to-blue-900/10 transition-all duration-200 cursor-pointer">
              <td class="px-6 py-4 whitespace-nowrap">
                <div>
                  <div class="text-sm font-semibold text-slate-900 dark:text-white">
                    {{ formatDate(movement.createdAt) }}
                  </div>
                  <div class="text-xs text-slate-500 dark:text-slate-400 font-mono">
                    {{ movement.reference || `#${movement.id.slice(-8)}` }}
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="{
                    'bg-gradient-to-r from-green-500 to-emerald-600 text-white': movement.type === 'receipt',
                    'bg-gradient-to-r from-red-500 to-pink-600 text-white': movement.type === 'issue',
                    'bg-gradient-to-r from-blue-500 to-purple-600 text-white': movement.type === 'transfer',
                    'bg-gradient-to-r from-orange-500 to-yellow-500 text-white': movement.type === 'adjustment'
                  }"
                  class="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold capitalize"
                >
                  {{ movement.type }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div>
                  <div class="text-sm font-semibold text-slate-900 dark:text-white">
                    {{ movement.itemName }}
                  </div>
                  <div class="text-xs text-slate-500 dark:text-slate-400 font-mono">
                    {{ movement.itemCode }}
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="{
                    'text-green-600 dark:text-green-400': movement.quantity > 0,
                    'text-red-600 dark:text-red-400': movement.quantity < 0
                  }"
                  class="text-sm font-bold"
                >
                  {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-slate-900 dark:text-white">
                R{{ formatCurrency(movement.totalValue || 0) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="{
                    'bg-gradient-to-r from-green-500 to-emerald-600 text-white': movement.status === 'completed',
                    'bg-gradient-to-r from-yellow-500 to-orange-500 text-white': movement.status === 'pending',
                    'bg-gradient-to-r from-red-500 to-pink-600 text-white': movement.status === 'cancelled'
                  }"
                  class="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold capitalize"
                >
                  {{ movement.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button
                  @click="viewMovement(movement)"
                  class="px-4 py-2 text-blue-600 dark:text-blue-400 hover:text-blue-700 hover:bg-blue-50 dark:hover:bg-blue-900/20 rounded-lg font-semibold transition-all duration-200"
                >
                  View
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="bg-gradient-to-r from-slate-50 to-slate-100 dark:from-slate-700 dark:to-slate-800 border-t border-slate-200 dark:border-slate-700 px-6 py-4">
        <div class="flex items-center justify-between">
          <div class="flex items-center text-sm text-slate-700 dark:text-slate-300">
            <span>Showing <span class="font-semibold text-purple-600 dark:text-purple-400">{{ ((currentPage - 1) * pageSize) + 1 }}</span> to <span class="font-semibold text-purple-600 dark:text-purple-400">{{ Math.min(currentPage * pageSize, filteredMovements.length) }}</span> of <span class="font-semibold text-purple-600 dark:text-purple-400">{{ filteredMovements.length }}</span> movements</span>
          </div>
          <div class="flex items-center space-x-3">
            <button
              @click="currentPage--"
              :disabled="currentPage <= 1"
              class="px-4 py-2 rounded-xl border-2 border-slate-300 dark:border-slate-600 text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
            >
              Previous
            </button>
            <span class="px-4 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl text-sm font-semibold shadow-md">
              Page {{ currentPage }} of {{ totalPages }}
            </span>
            <button
              @click="currentPage++"
              :disabled="currentPage >= totalPages"
              class="px-4 py-2 rounded-xl border-2 border-slate-300 dark:border-slate-600 text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>
    </div>

    <!-- Stock Movement Modal -->
    <StockMovementModal
      v-if="showMovementModal"
      :movement-type="selectedMovementType"
      @close="closeMovementModal"
      @save="saveMovement"
    />
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
import StockMovementModal from '~/components/stock/StockMovementModal.vue'
import { useStock, type StockMovementDto } from '../../composables/useStock'

// Composables
const { getStockMovements } = useStock()

// Reactive data
const movements = ref<StockMovementDto[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Filters
const searchQuery = ref('')
const selectedType = ref('')
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
    filtered = filtered.filter((movement: StockMovementDto) =>
      movement.itemName?.toLowerCase().includes(query) ||
      movement.itemCode?.toLowerCase().includes(query) ||
      movement.reference?.toLowerCase().includes(query)
    )
  }

  // Type filter
  if (selectedType.value) {
    filtered = filtered.filter((movement: StockMovementDto) => movement.movementType === selectedType.value)
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
    
    filtered = filtered.filter((movement: StockMovementDto) => new Date(movement.transactionDate) >= filterDate)
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
    movements.value = response.movements || []
  } catch (err) {
    error.value = 'Failed to load stock movements'
    console.error('Error loading stock movements:', err)
  } finally {
    loading.value = false
  }
}

const clearFilters = () => {
  searchQuery.value = ''
  selectedType.value = ''
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
  const exportData = filteredMovements.value.map((movement: StockMovementDto) => ({
    'Reference': movement.reference || movement.voucherNo,
    'Type': movement.movementType,
    'Item': movement.itemName,
    'SKU': movement.itemSku,
    'Quantity': movement.quantity,
    'Rate (R)': movement.rate || 0,
    'Amount (R)': movement.amount || 0,
    'Date': formatDate(movement.transactionDate),
    'Balance': movement.balanceQty
  }))

  const csvContent = [
    Object.keys(exportData[0]).join(','),
    ...exportData.map((row: Record<string, any>) => Object.values(row).join(','))
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

// Lifecycle
onMounted(() => {
  loadMovements()
})
</script>

