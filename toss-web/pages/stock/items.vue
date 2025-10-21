<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 class="text-3xl font-bold bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent">
              Items Management
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage inventory items, track stock levels, and monitor performance
            </p>
          </div>
          <div class="mt-4 sm:mt-0 flex space-x-3">
            <button
              @click="showCreateModal = true"
              class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Add Item
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <!-- Total Items Card -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Items</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white">
              {{ stockOverview?.totalItems || 0 }}
            </p>
          </div>
          <div class="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
            <CubeIcon class="w-8 h-8 text-white" />
          </div>
        </div>
      </div>

      <!-- Low Stock Card -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Low Stock</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white">
              {{ stockOverview?.lowStockItems || 0 }}
            </p>
          </div>
          <div class="p-3 bg-gradient-to-br from-orange-500 to-red-600 rounded-xl">
            <ExclamationTriangleIcon class="w-8 h-8 text-white" />
          </div>
        </div>
      </div>

      <!-- Out of Stock Card -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Out of Stock</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white">
              {{ stockOverview?.outOfStockItems || 0 }}
            </p>
          </div>
          <div class="p-3 bg-gradient-to-br from-red-500 to-pink-600 rounded-xl">
            <XCircleIcon class="w-8 h-8 text-white" />
          </div>
        </div>
      </div>

      <!-- Total Value Card -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Value</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white">
              R{{ formatCurrency(stockOverview?.totalValue || 0) }}
            </p>
          </div>
          <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
            <CurrencyDollarIcon class="w-8 h-8 text-white" />
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- Search -->
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
            <MagnifyingGlassIcon class="h-5 w-5 text-slate-400" />
          </div>
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Search items..."
            class="w-full pl-11 pr-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          />
        </div>

        <!-- Category Filter -->
        <select
          v-model="selectedCategory"
          class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
        >
          <option value="">All Categories</option>
          <option v-for="category in categories" :key="category" :value="category">
            {{ category }}
          </option>
        </select>

        <!-- Stock Status Filter -->
        <select
          v-model="stockFilter"
          class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
        >
          <option value="">All Stock Levels</option>
          <option value="low">Low Stock</option>
          <option value="out">Out of Stock</option>
          <option value="in-stock">In Stock</option>
        </select>

        <!-- Quick Actions -->
        <div class="flex space-x-2">
          <button
            @click="exportData"
            class="flex-1 inline-flex items-center justify-center px-4 py-2.5 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 hover:border-slate-400 dark:hover:border-slate-500 transition-all duration-200"
          >
            <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
            Export
          </button>
          <button
            @click="refreshData"
            class="px-4 py-2.5 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 hover:border-slate-400 dark:hover:border-slate-500 transition-all duration-200"
          >
            <ArrowPathIcon class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>

    <!-- Items Table -->
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
          <thead class="bg-gradient-to-r from-slate-50 to-slate-100 dark:from-slate-700 dark:to-slate-800">
            <tr>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Item Details
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                SKU / Barcode
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Stock Level
              </th>
              <th scope="col" class="px-6 py-4 text-left text-xs font-semibold text-slate-700 dark:text-slate-300 uppercase tracking-wider">
                Pricing
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
            <tr v-if="loading" v-for="n in 5" :key="n" class="animate-pulse">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="h-10 w-10 bg-slate-300 dark:bg-slate-600 rounded-lg"></div>
                  <div class="ml-4">
                    <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-32"></div>
                    <div class="h-3 bg-gray-300 dark:bg-gray-600 rounded w-24 mt-1"></div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-20"></div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-16"></div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="h-4 bg-gray-300 dark:bg-gray-600 rounded w-20"></div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="h-6 bg-gray-300 dark:bg-gray-600 rounded w-16"></div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="h-8 bg-gray-300 dark:bg-gray-600 rounded w-8"></div>
              </td>
            </tr>

            <tr 
              v-else
              v-for="item in paginatedItems" 
              :key="item.id"
              class="hover:bg-gradient-to-r hover:from-purple-50/50 hover:to-blue-50/50 dark:hover:from-purple-900/10 dark:hover:to-blue-900/10 cursor-pointer transition-all duration-200"
              @click="showItemDetails(item)"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10">
                    <div class="h-10 w-10 rounded-xl bg-gradient-to-br from-purple-100 to-blue-100 dark:from-purple-900/30 dark:to-blue-900/30 flex items-center justify-center">
                      <CubeIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-semibold text-slate-900 dark:text-white">
                      {{ item.name }}
                    </div>
                    <div class="text-sm text-slate-500 dark:text-slate-400">
                      {{ item.category }}
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-slate-900 dark:text-white font-mono font-semibold">{{ item.sku }}</div>
                <div v-if="item.barcode" class="text-xs text-slate-500 dark:text-slate-400 font-mono">
                  {{ item.barcode }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="text-sm font-semibold text-slate-900 dark:text-white">
                    {{ item.quantityOnHand || 0 }} {{ item.unit }}
                  </div>
                  <div 
                    v-if="(item.quantityOnHand || 0) <= item.reorderLevel"
                    class="ml-2"
                    :class="{
                      'text-red-500': (item.quantityOnHand || 0) === 0,
                      'text-orange-500': (item.quantityOnHand || 0) > 0 && (item.quantityOnHand || 0) <= item.reorderLevel
                    }"
                  >
                    <ExclamationTriangleIcon class="w-5 h-5" />
                  </div>
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-400">
                  Reorder: {{ item.reorderLevel }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-semibold text-slate-900 dark:text-white">
                  R{{ formatCurrency(item.sellingPrice) }}
                </div>
                <div v-if="item.costPrice" class="text-xs text-slate-500 dark:text-slate-400">
                  Cost: R{{ formatCurrency(item.costPrice) }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex px-3 py-1 text-xs font-semibold rounded-full"
                  :class="{
                    'bg-gradient-to-r from-green-500 to-emerald-600 text-white': item.isActive,
                    'bg-gradient-to-r from-red-500 to-pink-600 text-white': !item.isActive
                  }"
                >
                  {{ item.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end space-x-2">
                  <button
                    @click.stop="editItem(item)"
                    class="p-2 text-blue-600 hover:text-blue-700 hover:bg-blue-50 dark:text-blue-400 dark:hover:bg-blue-900/20 rounded-lg transition-all duration-200"
                  >
                    <PencilIcon class="w-5 h-5" />
                  </button>
                  <button
                    @click.stop="deleteItemConfirm(item)"
                    class="p-2 text-red-600 hover:text-red-700 hover:bg-red-50 dark:text-red-400 dark:hover:bg-red-900/20 rounded-lg transition-all duration-200"
                  >
                    <TrashIcon class="w-5 h-5" />
                  </button>
                </div>
              </td>
            </tr>

            <tr v-if="!loading && paginatedItems.length === 0">
              <td colspan="6" class="px-6 py-16 text-center">
                <div class="flex flex-col items-center">
                  <div class="p-4 bg-gradient-to-br from-purple-100 to-blue-100 dark:from-purple-900/30 dark:to-blue-900/30 rounded-2xl">
                    <CubeIcon class="h-12 w-12 text-purple-600 dark:text-purple-400" />
                  </div>
                  <h3 class="mt-4 text-base font-semibold text-slate-900 dark:text-white">No items found</h3>
                  <p class="mt-2 text-sm text-slate-500 dark:text-slate-400">
                    Get started by creating your first inventory item.
                  </p>
                  <button
                    @click="showCreateModal = true"
                    class="mt-6 inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
                  >
                    <PlusIcon class="w-5 h-5 mr-2" />
                    Add Item
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="bg-gradient-to-r from-slate-50 to-slate-100 dark:from-slate-700 dark:to-slate-800 px-4 py-4 border-t border-slate-200 dark:border-slate-700 sm:px-6">
        <div class="flex-1 flex justify-between sm:hidden">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="relative inline-flex items-center px-5 py-2.5 border-2 border-slate-300 dark:border-slate-600 text-sm font-medium rounded-xl text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
          >
            Previous
          </button>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="ml-3 relative inline-flex items-center px-5 py-2.5 border-2 border-slate-300 dark:border-slate-600 text-sm font-medium rounded-xl text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
          >
            Next
          </button>
        </div>
        <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-slate-700 dark:text-slate-300">
              Showing
              <span class="font-semibold text-purple-600 dark:text-purple-400">{{ ((currentPage - 1) * pageSize) + 1 }}</span>
              to
              <span class="font-semibold text-purple-600 dark:text-purple-400">{{ Math.min(currentPage * pageSize, totalItems) }}</span>
              of
              <span class="font-semibold text-purple-600 dark:text-purple-400">{{ totalItems }}</span>
              results
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-xl shadow-sm space-x-2" aria-label="Pagination">
              <button
                @click="currentPage--"
                :disabled="currentPage === 1"
                class="relative inline-flex items-center px-3 py-2 rounded-lg border-2 border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 text-sm font-medium text-slate-700 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
              >
                <ChevronLeftIcon class="h-5 w-5" />
              </button>
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="currentPage = page"
                :class="{
                  'bg-gradient-to-r from-purple-600 to-blue-600 text-white border-transparent shadow-md': page === currentPage,
                  'bg-white dark:bg-slate-700 border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-600': page !== currentPage
                }"
                class="relative inline-flex items-center px-4 py-2 border-2 rounded-lg text-sm font-semibold transition-all duration-200"
              >
                {{ page }}
              </button>
              <button
                @click="currentPage++"
                :disabled="currentPage === totalPages"
                class="relative inline-flex items-center px-3 py-2 rounded-lg border-2 border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-700 text-sm font-medium text-slate-700 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200"
              >
                <ChevronRightIcon class="h-5 w-5" />
              </button>
            </nav>
          </div>
        </div>
      </div>
    </div>
    </div>

    <!-- Create/Edit Item Modal -->
    <ItemModal
      v-if="showCreateModal || showEditModal"
      :item="selectedItem"
      :categories="categories"
      @close="closeModal"
      @save="saveItem"
    />

    <!-- Item Details Modal -->
    <ItemDetailsModal
      v-if="showDetailsModal"
      :item="selectedItem"
      @close="showDetailsModal = false"
      @edit="editItem"
      @delete="deleteItemConfirm"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useStock, type ItemDto, type CreateItemRequest, type UpdateItemRequest } from '~/composables/useStock'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  CubeIcon,
  ExclamationTriangleIcon,
  XCircleIcon,
  CurrencyDollarIcon,
  ArrowDownTrayIcon,
  ArrowPathIcon,
  PencilIcon,
  TrashIcon,
  ChevronLeftIcon,
  ChevronRightIcon
} from '@heroicons/vue/24/outline'
// Import modal components explicitly
import ItemModal from '~/components/stock/ItemModal.vue'
import ItemDetailsModal from '~/components/stock/ItemDetailsModal.vue'

// Composable
const { 
  loading, 
  error, 
  getItems, 
  getStockOverview, 
  getCategories,
  createItem,
  updateItem,
  deleteItem,
  getLowStockItems,
  getOutOfStockItems
} = useStock()

// Data
const items = ref<ItemDto[]>([])
const categories = ref<string[]>([])
const stockOverview = ref(null)
const totalItems = ref(0)

// Filters
const searchTerm = ref('')
const selectedCategory = ref('')
const stockFilter = ref('')
const currentPage = ref(1)
const pageSize = ref(20)

// Modals
const showCreateModal = ref(false)
const showEditModal = ref(false)
const showDetailsModal = ref(false)
const selectedItem = ref<ItemDto | null>(null)

// Computed
const filteredItems = computed(() => {
  let result = items.value

  // Search filter
  if (searchTerm.value) {
    const search = searchTerm.value.toLowerCase()
    result = result.filter(item => 
      item.name.toLowerCase().includes(search) ||
      item.sku.toLowerCase().includes(search) ||
      item.category.toLowerCase().includes(search) ||
      (item.barcode && item.barcode.toLowerCase().includes(search))
    )
  }

  // Category filter
  if (selectedCategory.value) {
    result = result.filter(item => item.category === selectedCategory.value)
  }

  // Stock filter
  if (stockFilter.value) {
    switch (stockFilter.value) {
      case 'low':
        result = result.filter(item => (item.quantityOnHand || 0) <= item.reorderLevel && (item.quantityOnHand || 0) > 0)
        break
      case 'out':
        result = result.filter(item => (item.quantityOnHand || 0) === 0)
        break
      case 'in-stock':
        result = result.filter(item => (item.quantityOnHand || 0) > item.reorderLevel)
        break
    }
  }

  return result
})

const totalPages = computed(() => Math.ceil(filteredItems.value.length / pageSize.value))

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredItems.value.slice(start, end)
})

const visiblePages = computed(() => {
  const pages = []
  const total = totalPages.value
  const current = currentPage.value
  
  if (total <= 7) {
    for (let i = 1; i <= total; i++) {
      pages.push(i)
    }
  } else {
    if (current <= 4) {
      for (let i = 1; i <= 5; i++) {
        pages.push(i)
      }
      pages.push('...', total)
    } else if (current >= total - 3) {
      pages.push(1, '...')
      for (let i = total - 4; i <= total; i++) {
        pages.push(i)
      }
    } else {
      pages.push(1, '...')
      for (let i = current - 1; i <= current + 1; i++) {
        pages.push(i)
      }
      pages.push('...', total)
    }
  }
  
  return pages.filter(page => page !== '...' || pages.indexOf(page) === pages.lastIndexOf(page))
})

// Methods
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const refreshData = async () => {
  await Promise.all([
    loadItems(),
    loadStockOverview(),
    loadCategories()
  ])
}

const loadItems = async () => {
  try {
    const response = await getItems({ 
      page: 1, 
      pageSize: 1000  // Load all items for client-side filtering
    })
    items.value = response.items
    totalItems.value = response.totalCount
  } catch (err) {
    console.error('Failed to load items:', err)
  }
}

const loadStockOverview = async () => {
  try {
    stockOverview.value = await getStockOverview()
  } catch (err) {
    console.error('Failed to load stock overview:', err)
  }
}

const loadCategories = async () => {
  try {
    categories.value = await getCategories()
  } catch (err) {
    console.error('Failed to load categories:', err)
  }
}

const showItemDetails = (item: ItemDto) => {
  selectedItem.value = item
  showDetailsModal.value = true
}

const editItem = (item: ItemDto) => {
  selectedItem.value = item
  showEditModal.value = true
  showDetailsModal.value = false
}

const deleteItemConfirm = (item: ItemDto) => {
  if (confirm(`Are you sure you want to delete "${item.name}"?`)) {
    deleteItemHandler(item.id)
  }
}

const deleteItemHandler = async (id: string) => {
  try {
    await deleteItem(id)
    await refreshData()
  } catch (err) {
    console.error('Failed to delete item:', err)
  }
}

const saveItem = async (itemData: CreateItemRequest | UpdateItemRequest) => {
  try {
    if ('id' in itemData) {
      await updateItem(itemData as UpdateItemRequest)
    } else {
      await createItem(itemData as CreateItemRequest)
    }
    await refreshData()
    closeModal()
  } catch (err) {
    console.error('Failed to save item:', err)
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  selectedItem.value = null
}

const exportData = () => {
  const csvContent = [
    ['SKU', 'Name', 'Category', 'Unit', 'Selling Price', 'Cost Price', 'Stock Level', 'Reorder Level', 'Status'],
    ...filteredItems.value.map(item => [
      item.sku,
      item.name,
      item.category,
      item.unit,
      item.sellingPrice,
      item.costPrice || '',
      item.quantityOnHand || 0,
      item.reorderLevel,
      item.isActive ? 'Active' : 'Inactive'
    ])
  ].map(row => row.join(',')).join('\n')

  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `items-export-${new Date().toISOString().split('T')[0]}.csv`
  a.click()
  window.URL.revokeObjectURL(url)
}

// Watch for filter changes and reset page
watch([searchTerm, selectedCategory, stockFilter], () => {
  currentPage.value = 1
})

// Lifecycle
onMounted(() => {
  refreshData()
})
</script>
