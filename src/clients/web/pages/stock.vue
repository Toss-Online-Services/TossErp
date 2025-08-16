<template>
  <div class="space-y-6">
    <!-- Page header -->
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Stock Management</h1>
        <p class="text-gray-600 dark:text-gray-400">Manage inventory levels and stock movements</p>
      </div>
      <div class="flex space-x-3">
        <button class="btn-outline">
          <FunnelIcon class="w-4 h-4 mr-2" />
          Filter
        </button>
        <button class="btn-outline">
          <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
          Export
        </button>
        <button class="btn-primary">
          <PlusIcon class="w-4 h-4 mr-2" />
          Add Stock
        </button>
      </div>
    </div>

    <!-- Search and filters -->
    <div class="card">
      <div class="card-body">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="form-label">Search Items</label>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by name, SKU, or category..."
              class="form-input"
            />
          </div>
          <div>
            <label class="form-label">Warehouse</label>
            <select v-model="selectedWarehouse" class="form-input">
              <option value="">All Warehouses</option>
              <option value="warehouse-1">Main Warehouse</option>
              <option value="warehouse-2">Secondary Warehouse</option>
              <option value="warehouse-3">Distribution Center</option>
            </select>
          </div>
          <div>
            <label class="form-label">Category</label>
            <select v-model="selectedCategory" class="form-input">
              <option value="">All Categories</option>
              <option value="electronics">Electronics</option>
              <option value="clothing">Clothing</option>
              <option value="books">Books</option>
              <option value="food">Food & Beverages</option>
            </select>
          </div>
          <div>
            <label class="form-label">Stock Level</label>
            <select v-model="selectedStockLevel" class="form-input">
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
        <p class="text-sm text-gray-600 dark:text-gray-400">{{ filteredItems.length }} items found</p>
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
              <tr v-for="item in paginatedItems" :key="item.id" class="table-row">
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
                  <span class="text-sm text-gray-900 dark:text-white">${{ item.unitCost }}</span>
                </td>
                <td class="table-cell">
                  <span class="text-sm font-medium text-gray-900 dark:text-white">${{ (item.quantity * item.unitCost).toFixed(2) }}</span>
                </td>
                <td class="table-cell">
                  <span :class="[
                    'badge',
                    item.quantity > 10 ? 'badge-success' : item.quantity > 0 ? 'badge-warning' : 'badge-danger'
                  ]">
                    {{ item.quantity > 10 ? 'In Stock' : item.quantity > 0 ? 'Low Stock' : 'Out of Stock' }}
                  </span>
                </td>
                <td class="table-cell">
                  <div class="flex items-center space-x-2">
                    <button class="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                      <EyeIcon class="w-4 h-4" />
                    </button>
                    <button class="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                      <PencilIcon class="w-4 h-4" />
                    </button>
                    <button class="p-1 text-gray-400 hover:text-red-600 dark:hover:text-red-400">
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
            Showing {{ startIndex + 1 }} to {{ endIndex }} of {{ filteredItems.length }} results
          </div>
          <div class="flex items-center space-x-2">
            <button
              @click="previousPage"
              :disabled="currentPage === 1"
              class="btn-outline px-3 py-1 text-sm disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Previous
            </button>
            <span class="text-sm text-gray-700 dark:text-gray-300">
              Page {{ currentPage }} of {{ totalPages }}
            </span>
            <button
              @click="nextPage"
              :disabled="currentPage === totalPages"
              class="btn-outline px-3 py-1 text-sm disabled:opacity-50 disabled:cursor-not-allowed"
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
import { ref, computed } from 'vue'
import {
  PlusIcon,
  FunnelIcon,
  ArrowDownTrayIcon,
  CubeIcon,
  EyeIcon,
  PencilIcon,
  TrashIcon
} from '@heroicons/vue/24/outline'

// Reactive state
const searchQuery = ref('')
const selectedWarehouse = ref('')
const selectedCategory = ref('')
const selectedStockLevel = ref('')
const currentPage = ref(1)
const itemsPerPage = 10

// Mock data
const items = ref([
  {
    id: 1,
    name: 'Laptop Computer',
    description: 'High-performance laptop for business use',
    sku: 'LAP-001',
    category: 'Electronics',
    warehouse: 'Main Warehouse',
    quantity: 25,
    unitCost: 899.99
  },
  {
    id: 2,
    name: 'Wireless Mouse',
    description: 'Ergonomic wireless mouse',
    sku: 'MOU-002',
    category: 'Electronics',
    warehouse: 'Main Warehouse',
    quantity: 150,
    unitCost: 29.99
  },
  {
    id: 3,
    name: 'Office Chair',
    description: 'Comfortable office chair with lumbar support',
    sku: 'CHA-003',
    category: 'Furniture',
    warehouse: 'Secondary Warehouse',
    quantity: 8,
    unitCost: 199.99
  },
  {
    id: 4,
    name: 'Coffee Mug',
    description: 'Ceramic coffee mug, 12oz',
    sku: 'MUG-004',
    category: 'Kitchen',
    warehouse: 'Main Warehouse',
    quantity: 0,
    unitCost: 12.99
  },
  {
    id: 5,
    name: 'Notebook',
    description: 'Spiral-bound notebook, 100 pages',
    sku: 'NOT-005',
    category: 'Office Supplies',
    warehouse: 'Main Warehouse',
    quantity: 75,
    unitCost: 5.99
  }
])

// Computed properties
const filteredItems = computed(() => {
  return items.value.filter(item => {
    const matchesSearch = !searchQuery.value || 
      item.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      item.sku.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      item.category.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesWarehouse = !selectedWarehouse.value || item.warehouse === selectedWarehouse.value
    const matchesCategory = !selectedCategory.value || item.category.toLowerCase() === selectedCategory.value
    
    let matchesStockLevel = true
    if (selectedStockLevel.value === 'in-stock') {
      matchesStockLevel = item.quantity > 10
    } else if (selectedStockLevel.value === 'low-stock') {
      matchesStockLevel = item.quantity > 0 && item.quantity <= 10
    } else if (selectedStockLevel.value === 'out-of-stock') {
      matchesStockLevel = item.quantity === 0
    }
    
    return matchesSearch && matchesWarehouse && matchesCategory && matchesStockLevel
  })
})

const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage))

const startIndex = computed(() => (currentPage.value - 1) * itemsPerPage)
const endIndex = computed(() => Math.min(startIndex.value + itemsPerPage, filteredItems.value.length))

const paginatedItems = computed(() => {
  return filteredItems.value.slice(startIndex.value, endIndex.value)
})

// Methods
const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}
</script>
