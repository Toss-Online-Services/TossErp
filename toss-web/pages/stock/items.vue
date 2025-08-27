<template>
  <div class="p-6 bg-gray-50 dark:bg-gray-900 min-h-screen">
    <!-- Header Section -->
    <div class="mb-8">
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Items</h1>
          <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
            Manage your inventory items, track stock levels, and monitor performance
          </p>
        </div>
        <div class="mt-4 sm:mt-0 flex space-x-3">
          <button
            @click="showCreateModal = true"
            class="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            <PlusIcon class="w-5 h-5 mr-2" />
            Add Item
          </button>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
              <CubeIcon class="w-5 h-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                Total Items
              </dt>
              <dd class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ stockOverview?.totalItems || 0 }}
              </dd>
            </dl>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-orange-100 dark:bg-orange-900 rounded-lg flex items-center justify-center">
              <ExclamationTriangleIcon class="w-5 h-5 text-orange-600 dark:text-orange-400" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                Low Stock
              </dt>
              <dd class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ stockOverview?.lowStockItems || 0 }}
              </dd>
            </dl>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-red-100 dark:bg-red-900 rounded-lg flex items-center justify-center">
              <XCircleIcon class="w-5 h-5 text-red-600 dark:text-red-400" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                Out of Stock
              </dt>
              <dd class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ stockOverview?.outOfStockItems || 0 }}
              </dd>
            </dl>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
        <div class="flex items-center">
          <div class="flex-shrink-0">
            <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
              <CurrencyDollarIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="ml-5 w-0 flex-1">
            <dl>
              <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                Total Value
              </dt>
              <dd class="text-lg font-semibold text-gray-900 dark:text-white">
                R{{ formatCurrency(stockOverview?.totalValue || 0) }}
              </dd>
            </dl>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <!-- Search -->
        <div class="relative">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
          </div>
          <input
            v-model="searchTerm"
            type="text"
            placeholder="Search items..."
            class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
          />
        </div>

        <!-- Category Filter -->
        <select
          v-model="selectedCategory"
          class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
        >
          <option value="">All Categories</option>
          <option v-for="category in categories" :key="category" :value="category">
            {{ category }}
          </option>
        </select>

        <!-- Stock Status Filter -->
        <select
          v-model="stockFilter"
          class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
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
            class="flex-1 inline-flex items-center justify-center px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            <ArrowDownTrayIcon class="w-4 h-4 mr-1" />
            Export
          </button>
          <button
            @click="refreshData"
            class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
          >
            <ArrowPathIcon class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>

    <!-- Items Table -->
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Item Details
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                SKU / Barcode
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Stock Level
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Pricing
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Status
              </th>
              <th scope="col" class="relative px-6 py-3">
                <span class="sr-only">Actions</span>
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-if="loading" v-for="n in 5" :key="n" class="animate-pulse">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="h-10 w-10 bg-gray-300 dark:bg-gray-600 rounded-lg"></div>
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
              class="hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer"
              @click="showItemDetails(item)"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10">
                    <div class="h-10 w-10 rounded-lg bg-gray-100 dark:bg-gray-600 flex items-center justify-center">
                      <CubeIcon class="w-6 h-6 text-gray-500 dark:text-gray-400" />
                    </div>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ item.name }}
                    </div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">
                      {{ item.category }}
                    </div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white font-mono">{{ item.sku }}</div>
                <div v-if="item.barcode" class="text-sm text-gray-500 dark:text-gray-400 font-mono">
                  {{ item.barcode }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">
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
                    <ExclamationTriangleIcon class="w-4 h-4" />
                  </div>
                </div>
                <div class="text-xs text-gray-500 dark:text-gray-400">
                  Reorder: {{ item.reorderLevel }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900 dark:text-white">
                  R{{ formatCurrency(item.sellingPrice) }}
                </div>
                <div v-if="item.costPrice" class="text-sm text-gray-500 dark:text-gray-400">
                  Cost: R{{ formatCurrency(item.costPrice) }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                  :class="{
                    'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': item.isActive,
                    'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200': !item.isActive
                  }"
                >
                  {{ item.isActive ? 'Active' : 'Inactive' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center space-x-2">
                  <button
                    @click.stop="editItem(item)"
                    class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-200"
                  >
                    <PencilIcon class="w-4 h-4" />
                  </button>
                  <button
                    @click.stop="deleteItemConfirm(item)"
                    class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-200"
                  >
                    <TrashIcon class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>

            <tr v-if="!loading && paginatedItems.length === 0">
              <td colspan="6" class="px-6 py-12 text-center">
                <CubeIcon class="mx-auto h-12 w-12 text-gray-400" />
                <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No items found</h3>
                <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
                  Get started by creating your first item.
                </p>
                <div class="mt-6">
                  <button
                    @click="showCreateModal = true"
                    class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
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
      <div v-if="totalPages > 1" class="bg-white dark:bg-gray-800 px-4 py-3 border-t border-gray-200 dark:border-gray-700 sm:px-6">
        <div class="flex-1 flex justify-between sm:hidden">
          <button
            @click="currentPage--"
            :disabled="currentPage === 1"
            class="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Previous
          </button>
          <button
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Next
          </button>
        </div>
        <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
          <div>
            <p class="text-sm text-gray-700 dark:text-gray-300">
              Showing
              <span class="font-medium">{{ ((currentPage - 1) * pageSize) + 1 }}</span>
              to
              <span class="font-medium">{{ Math.min(currentPage * pageSize, totalItems) }}</span>
              of
              <span class="font-medium">{{ totalItems }}</span>
              results
            </p>
          </div>
          <div>
            <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
              <button
                @click="currentPage--"
                :disabled="currentPage === 1"
                class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-sm font-medium text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <ChevronLeftIcon class="h-5 w-5" />
              </button>
              <button
                v-for="page in visiblePages"
                :key="page"
                @click="currentPage = page"
                :class="{
                  'bg-blue-50 dark:bg-blue-900 border-blue-500 text-blue-600 dark:text-blue-400': page === currentPage,
                  'bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600 text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-600': page !== currentPage
                }"
                class="relative inline-flex items-center px-4 py-2 border text-sm font-medium"
              >
                {{ page }}
              </button>
              <button
                @click="currentPage++"
                :disabled="currentPage === totalPages"
                class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-sm font-medium text-gray-500 dark:text-gray-400 hover:bg-gray-50 dark:hover:bg-gray-600 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <ChevronRightIcon class="h-5 w-5" />
              </button>
            </nav>
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
