<template>
  <div class="space-y-6">
    <!-- Page header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Stock Management</h1>
        <p class="text-gray-600 dark:text-gray-400">Manage inventory levels and stock movements</p>
      </div>
      <div class="flex space-x-3">
        <button class="btn-outline" @click="showFilters = !showFilters">
          <FunnelIcon class="w-4 h-4 mr-2" />
          {{ showFilters ? 'Hide' : 'Show' }} Filters
        </button>
        <button class="btn-outline" @click="exportStockData">
          <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
          Export
        </button>
        <button class="btn-primary" @click="showAddStockModal = true">
          <PlusIcon class="w-4 h-4 mr-2" />
          Add Stock
        </button>
      </div>
    </div>

    <!-- Search and filters -->
    <div v-if="showFilters" class="card">
      <div class="card-body">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="form-label">Search Items</label>
            <input
              v-model="stockStore.filters.search"
              type="text"
              placeholder="Search by name, SKU, or category..."
              class="form-input"
            />
          </div>
          <div>
            <label class="form-label">Warehouse</label>
            <select v-model="stockStore.filters.warehouse" class="form-input">
              <option value="">All Warehouses</option>
              <option v-for="warehouse in stockStore.warehouses" :key="warehouse" :value="warehouse">
                {{ warehouse }}
              </option>
            </select>
          </div>
          <div>
            <label class="form-label">Category</label>
            <select v-model="stockStore.filters.category" class="form-input">
              <option value="">All Categories</option>
              <option v-for="category in stockStore.categories" :key="category" :value="category">
                {{ category }}
              </option>
            </select>
          </div>
          <div>
            <label class="form-label">Stock Level</label>
            <select v-model="stockStore.filters.stockLevel" class="form-input">
              <option value="">All Levels</option>
              <option value="in-stock">In Stock</option>
              <option value="low-stock">Low Stock</option>
              <option value="out-of-stock">Out of Stock</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Stock table -->
    <div class="card">
      <div class="card-header">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Stock Levels</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">{{ stockStore.filteredItems.length }} items found</p>
      </div>
      <div class="card-body p-0">
        <div class="overflow-x-auto">
          <table class="table">
            <thead class="table-header">
              <tr>
                <th class="table-header-cell">
                  <input type="checkbox" class="rounded border-gray-300 text-primary-600 focus:ring-primary-500" />
                </th>
                <th class="table-header-cell">Item</th>
                <th class="table-header-cell">SKU</th>
                <th class="table-header-cell">Category</th>
                <th class="table-header-cell">Warehouse</th>
                <th class="table-header-cell">Quantity</th>
                <th class="table-header-cell">Unit Cost</th>
                <th class="table-header-cell">Total Value</th>
                <th class="table-header-cell">Status</th>
                <th class="table-header-cell">Actions</th>
              </tr>
            </thead>
            <tbody class="table-body">
              <tr v-for="item in stockStore.paginatedItems" :key="item.id" class="table-row">
                <td class="table-cell">
                  <input type="checkbox" class="rounded border-gray-300 text-primary-600 focus:ring-primary-500" />
                </td>
                <td class="table-cell">
                  <div class="flex items-center">
                    <div class="w-10 h-10 bg-gray-200 dark:bg-gray-700 rounded-lg flex items-center justify-center">
                      <CubeIcon class="w-5 h-5 text-gray-500 dark:text-gray-400" />
                    </div>
                    <div class="ml-3">
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ item.name }}</p>
                      <p class="text-sm text-gray-500 dark:text-gray-400">{{ item.description }}</p>
                    </div>
                  </div>
                </td>
                <td class="table-cell">
                  <span class="text-sm text-gray-900 dark:text-white">{{ item.sku }}</span>
                </td>
                <td class="table-cell">
                  <span class="badge badge-info">{{ item.category }}</span>
                </td>
                <td class="table-cell">
                  <span class="text-sm text-gray-900 dark:text-white">{{ item.warehouse }}</span>
                </td>
                <td class="table-cell">
                  <span class="text-sm font-medium text-gray-900 dark:text-white">{{ item.quantity }}</span>
                </td>
                <td class="table-cell">
                  <span class="text-sm text-gray-900 dark:text-white">${{ item.unitCost.toFixed(2) }}</span>
                </td>
                <td class="table-cell">
                  <span class="text-sm font-medium text-gray-900 dark:text-white">${{ (item.quantity * item.unitCost).toFixed(2) }}</span>
                </td>
                <td class="table-cell">
                  <span :class="[
                    'badge',
                    item.quantity > item.reorderLevel ? 'badge-success' : item.quantity > 0 ? 'badge-warning' : 'badge-danger'
                  ]">
                    {{ item.quantity > item.reorderLevel ? 'In Stock' : item.quantity > 0 ? 'Low Stock' : 'Out of Stock' }}
                  </span>
                </td>
                <td class="table-cell">
                  <div class="flex items-center space-x-2">
                    <button 
                      @click="viewItem(item)"
                      class="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
                    >
                      <EyeIcon class="w-4 h-4" />
                    </button>
                    <button 
                      @click="editItem(item)"
                      class="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
                    >
                      <PencilIcon class="w-4 h-4" />
                    </button>
                    <button 
                      @click="deleteItem(item)"
                      class="p-1 text-gray-400 hover:text-red-600 dark:hover:text-red-400"
                    >
                      <TrashIcon class="w-4 h-4" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
      
      <!-- Pagination -->
      <div class="card-footer">
        <div class="flex items-center justify-between">
          <div class="text-sm text-gray-700 dark:text-gray-300">
            Showing {{ startIndex + 1 }} to {{ endIndex }} of {{ stockStore.filteredItems.length }} results
          </div>
          <div class="flex items-center space-x-2">
            <button
              @click="previousPage"
              :disabled="stockStore.filters.page === 1"
              class="btn-outline px-3 py-1 text-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Previous
            </button>
            <span class="text-sm text-gray-700 dark:text-gray-300">
              Page {{ stockStore.filters.page }} of {{ stockStore.totalPages }}
            </span>
            <button
              @click="nextPage"
              :disabled="stockStore.filters.page === stockStore.totalPages"
              class="btn-outline px-3 py-1 text-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Stock Movement Form -->
    <div v-if="showMovementForm" class="card">
      <div class="card-header">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Record Stock Movement</h3>
      </div>
      <div class="card-body">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label class="form-label">Movement Type</label>
            <select v-model="movementForm.type" class="form-input">
              <option value="in">Stock In</option>
              <option value="out">Stock Out</option>
              <option value="adjustment">Adjustment</option>
              <option value="transfer">Transfer</option>
            </select>
          </div>
          <div>
            <label class="form-label">Quantity</label>
            <input
              v-model.number="movementForm.quantity"
              type="number"
              min="1"
              class="form-input"
              placeholder="Enter quantity"
            />
          </div>
          <div>
            <label class="form-label">Reference</label>
            <input
              v-model="movementForm.reference"
              type="text"
              class="form-input"
              placeholder="PO/SO number, etc."
            />
          </div>
          <div>
            <label class="form-label">Reason</label>
            <input
              v-model="movementForm.reason"
              type="text"
              class="form-input"
              placeholder="Reason for movement"
            />
          </div>
        </div>
        <div class="flex justify-end space-x-3 mt-4">
          <button @click="cancelMovement" class="btn-outline">
            Cancel
          </button>
          <button @click="saveMovement" class="btn-primary">
            Save Movement
          </button>
        </div>
      </div>
    </div>
    
    <!-- Add Stock Modal -->
    <div v-if="showAddStockModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Add New Stock Item</h3>
            <button
              @click="showAddStockModal = false"
              class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
            >
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          <StockItemForm
            @save="handleItemSave"
            @cancel="showAddStockModal = false"
          />
        </div>
      </div>
    </div>
    
    <!-- Record Movement Modal -->
    <div v-if="showMovementForm" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Record Stock Movement</h3>
            <button
              @click="showMovementForm = false"
              class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
            >
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          <StockMovementForm
            @save="handleMovementSave"
            @cancel="showMovementForm = false"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  FunnelIcon,
  ArrowDownTrayIcon,
  CubeIcon,
  EyeIcon,
  PencilIcon,
  TrashIcon
} from '@heroicons/vue/24/outline'
import { useStockStore } from '../stores/stock'
import type { StockItem } from '../stores/stock'

const stockStore = useStockStore()

// State
const showFilters = ref(false)
const showAddStockModal = ref(false)
const showMovementForm = ref(false)
const selectedItem = ref<StockItem | null>(null)

// Movement form
const movementForm = ref({
  type: 'in' as 'in' | 'out' | 'adjustment' | 'transfer',
  quantity: 1,
  reference: '',
  reason: ''
})

// Computed properties
const startIndex = computed(() => (stockStore.filters.page - 1) * stockStore.filters.itemsPerPage)
const endIndex = computed(() => Math.min(startIndex.value + stockStore.filters.itemsPerPage, stockStore.filteredItems.length))

// Methods
const nextPage = () => {
  if (stockStore.filters.page < stockStore.totalPages) {
    stockStore.setPage(stockStore.filters.page + 1)
  }
}

const previousPage = () => {
  if (stockStore.filters.page > 1) {
    stockStore.setPage(stockStore.filters.page - 1)
  }
}

const viewItem = (item: StockItem) => {
  console.log('Viewing item:', item.name)
  // In real app, this would open a detailed view modal
  alert(`Viewing item: ${item.name}`)
}

const editItem = (item: StockItem) => {
  console.log('Editing item:', item.name)
  // In real app, this would open an edit modal
  alert(`Editing item: ${item.name}`)
}

const deleteItem = (item: StockItem) => {
  if (confirm(`Are you sure you want to delete ${item.name}?`)) {
    stockStore.deleteItem(item.id)
    console.log('Deleted item:', item.name)
  }
}

const exportStockData = () => {
  console.log('Exporting stock data...')
  // In real app, this would trigger the export functionality
  alert('Export functionality would be implemented here')
}

const showMovementFormForItem = (item: StockItem) => {
  selectedItem.value = item
  showMovementForm.value = true
  movementForm.value = {
    type: 'in',
    quantity: 1,
    reference: '',
    reason: ''
  }
}

const saveMovement = () => {
  if (!selectedItem.value) return
  
  try {
    stockStore.addMovement({
      itemId: selectedItem.value.id,
      itemName: selectedItem.value.name,
      type: movementForm.value.type,
      quantity: movementForm.value.quantity,
      warehouse: selectedItem.value.warehouse,
      reference: movementForm.value.reference,
      reason: movementForm.value.reason,
      createdBy: 'Current User' // In real app, get from auth context
    })
    
    console.log('Movement saved successfully')
    cancelMovement()
  } catch (error) {
    console.error('Failed to save movement:', error)
    alert('Failed to save movement')
  }
}

const cancelMovement = () => {
  showMovementForm.value = false
  selectedItem.value = null
  movementForm.value = {
    type: 'in',
    quantity: 1,
    reference: '',
    reason: ''
  }
}

// Initialize store data on mount
onMounted(() => {
  stockStore.loadMockData()
})
</script>

<style scoped>
.card {
  @apply bg-white dark:bg-gray-900 border border-gray-200 dark:border-gray-700 rounded-lg shadow-sm;
}

.card-header {
  @apply px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between;
}

.card-body {
  @apply px-6 py-4;
}

.card-footer {
  @apply px-6 py-4 border-t border-gray-200 dark:border-gray-700;
}

.form-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.form-input {
  @apply block w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-primary-500 focus:border-primary-500 dark:bg-gray-800 dark:text-white;
}

.btn-primary {
  @apply inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500;
}

.btn-outline {
  @apply inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500;
}

.table {
  @apply min-w-full divide-y divide-gray-200 dark:divide-gray-700;
}

.table-header {
  @apply bg-gray-50 dark:bg-gray-800;
}

.table-header-cell {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider;
}

.table-body {
  @apply bg-white dark:bg-gray-900 divide-y divide-gray-200 dark:divide-gray-700;
}

.table-row {
  @apply hover:bg-gray-50 dark:hover:bg-gray-800;
}

.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white;
}

.badge {
  @apply inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium;
}

.badge-success {
  @apply bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300;
}

.badge-warning {
  @apply bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300;
}

.badge-danger {
  @apply bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300;
}

.badge-info {
  @apply bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300;
}
</style>
