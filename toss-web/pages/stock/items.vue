<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useStockStore } from '~/stores/stock'

useHead({
  title: 'Stock Items - TOSS'
})

const stockStore = useStockStore()
const searchQuery = ref('')
const showAddModal = ref(false)
const selectedCategory = ref('all')

// Computed
const filteredItems = computed(() => {
  let filtered = stockStore.items

  if (selectedCategory.value !== 'all') {
    filtered = filtered.filter(item => item.category === selectedCategory.value)
  }

  if (searchQuery.value) {
    filtered = stockStore.searchItems(searchQuery.value)
  }

  return filtered
})

const categories = computed(() => {
  const cats = new Set(stockStore.items.map(item => item.category))
  return ['all', ...Array.from(cats)]
})

// Load data
onMounted(() => {
  stockStore.fetchItems()
})

// Methods
function getStockStatus(item: any) {
  if (item.currentStock === 0) return { text: 'Out of Stock', color: 'text-red-600 bg-red-100' }
  if (item.currentStock <= item.minStock) return { text: 'Low Stock', color: 'text-orange-600 bg-orange-100' }
  return { text: 'In Stock', color: 'text-green-600 bg-green-100' }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}
</script>

<template>
  <div class="py-6">
    <!-- Page Header -->
    <div class="mb-8 flex items-center justify-between">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Stock Items</h3>
        <p class="text-gray-600 text-sm">
          Manage your inventory and track stock levels
        </p>
      </div>
      <button
        @click="showAddModal = true"
        class="flex items-center gap-2 px-4 py-2 bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-lg hover:shadow-lg transition-shadow"
      >
        <i class="material-symbols-rounded text-xl">add</i>
        <span>Add Item</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Items</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ stockStore.items.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">inventory_2</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Low Stock</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ stockStore.lowStockItems.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">warning</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Out of Stock</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ stockStore.outOfStockItems.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-red-500 to-red-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">error</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Stock Value</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ formatCurrency(stockStore.totalStockValue) }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-green-500 to-green-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">payments</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <!-- Search -->
        <div class="flex-1">
          <div class="relative">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by name, code, or barcode..."
              class="w-full px-4 py-2 pr-10 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
            >
            <i class="material-symbols-rounded absolute right-3 top-1/2 -translate-y-1/2 text-gray-400">search</i>
          </div>
        </div>

        <!-- Category Filter -->
        <div class="w-full md:w-64">
          <select
            v-model="selectedCategory"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
          >
            <option v-for="cat in categories" :key="cat" :value="cat">
              {{ cat === 'all' ? 'All Categories' : cat }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <!-- Items Table -->
    <div class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Item
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Category
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Stock
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Cost Price
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Selling Price
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="item in filteredItems" :key="item.id" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10 bg-gray-100 rounded-lg flex items-center justify-center">
                    <i class="material-symbols-rounded text-gray-600">inventory_2</i>
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
                    <div class="text-sm text-gray-500">{{ item.code }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ item.category }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ item.currentStock }} {{ item.unit }}</div>
                <div class="text-xs text-gray-500">Min: {{ item.minStock }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(item.costPrice) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(item.sellingPrice) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="['px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full', getStockStatus(item).color]">
                  {{ getStockStatus(item).text }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <button class="text-blue-600 hover:text-blue-900 mr-3">
                  <i class="material-symbols-rounded text-lg">edit</i>
                </button>
                <button class="text-gray-600 hover:text-gray-900">
                  <i class="material-symbols-rounded text-lg">more_vert</i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Empty State -->
      <div v-if="filteredItems.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">inventory_2</i>
        <h3 class="text-lg font-medium text-gray-900 mb-2">No items found</h3>
        <p class="text-gray-600 mb-4">
          {{ searchQuery ? 'Try adjusting your search' : 'Get started by adding your first item' }}
        </p>
        <button
          v-if="!searchQuery"
          @click="showAddModal = true"
          class="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
        >
          <i class="material-symbols-rounded">add</i>
          <span>Add Item</span>
        </button>
      </div>
    </div>
  </div>
</template>

