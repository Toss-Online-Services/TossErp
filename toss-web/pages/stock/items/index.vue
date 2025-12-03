<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'
import ItemModal from '~/components/stock/ItemModal.vue'
import StockAdjustmentModal from '~/components/stock/StockAdjustmentModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Stock Items - TOSS'
})

const stockStore = useStockStore()
const searchQuery = ref('')
const showAddModal = ref(false)
const showEditModal = ref(false)
const showAdjustModal = ref(false)
const selectedCategory = ref('all')
const selectedItem = ref<Item | null>(null)

// Computed
const filteredItems = computed(() => {
  let filtered = stockStore.items

  if (selectedCategory.value !== 'all') {
    filtered = filtered.filter(item => item.category === selectedCategory.value)
  }

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.name.toLowerCase().includes(query) ||
      item.code.toLowerCase().includes(query) ||
      item.barcode?.toLowerCase().includes(query)
    )
  }

  return filtered
})

const categories = computed(() => {
  const cats = new Set(stockStore.items.map(item => item.category))
  return Array.from(cats).sort()
})

const stats = computed(() => {
  const items = stockStore.items
  return {
    total: items.length,
    lowStock: items.filter(item => {
      return item.currentStock > 0 && item.currentStock < item.minStock
    }).length,
    outOfStock: items.filter(item => item.currentStock <= 0).length,
    totalValue: items.reduce((sum, item) => {
      return sum + (item.currentStock * item.costPrice)
    }, 0)
  }
})

// Methods
onMounted(async () => {
  await stockStore.fetchItems()
  console.log('Items loaded:', stockStore.items.length, stockStore.items)
})

function handleAdd() {
  selectedItem.value = null
  showAddModal.value = true
}

function handleEdit(item: Item) {
  selectedItem.value = item
  showEditModal.value = true
}

function handleAdjust(item: Item) {
  selectedItem.value = item
  showAdjustModal.value = true
}

function handleViewHistory(item: Item) {
  navigateTo(`/stock/items/${item.id}`)
  console.log('Navigating to item:', item.id, item.name)
}

function handleItemSaved() {
  showAddModal.value = false
  showEditModal.value = false
  stockStore.fetchItems()
}

function handleAdjustmentSaved() {
  showAdjustModal.value = false
  stockStore.fetchItems()
}

function getStockStatus(item: Item) {
  if (item.currentStock <= 0) {
    return { label: 'Out of Stock', color: 'text-red-600 bg-red-100' }
  } else if (item.currentStock < item.minStock) {
    return { label: 'Low Stock', color: 'text-orange-600 bg-orange-100' }
  }
  return { label: 'In Stock', color: 'text-green-600 bg-green-100' }
}
</script>

<template>
  <div class="py-6">
    <!-- Page Header -->
    <div class="mb-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900">Stock Items</h1>
          <p class="mt-1 text-sm text-gray-600">Manage your inventory and track stock levels</p>
        </div>
        <button
          @click="handleAdd"
          class="flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Item</span>
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Items</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-gray-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-gray-900">inventory_2</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Low Stock</p>
            <p class="mt-2 text-3xl font-bold text-orange-600">{{ stats.lowStock }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">warning</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Out of Stock</p>
            <p class="mt-2 text-3xl font-bold text-red-600">{{ stats.outOfStock }}</p>
          </div>
          <div class="p-3 bg-red-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-red-600">error</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Stock Value</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">R {{ stats.totalValue.toFixed(2) }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">attach_money</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-card p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <div class="relative">
            <i class="material-symbols-rounded absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">search</i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by name, code, or barcode..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
        </div>
        <div class="md:w-64">
          <select
            v-model="selectedCategory"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent bg-white"
          >
            <option value="all">All Categories</option>
            <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Items Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="stockStore.items.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">inventory_2</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No items found</h3>
        <p class="text-gray-600 mb-6">Get started by adding your first item</p>
        <button
          @click="handleAdd"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Item</span>
        </button>
      </div>

      <div v-else-if="filteredItems.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">search</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No items match your search</h3>
        <p class="text-gray-600 mb-6">Try adjusting your filters</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Category</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Stock</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cost Price</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Selling Price</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="item in filteredItems" :key="item.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <i class="material-symbols-rounded text-gray-400 mr-3">inventory_2</i>
                <div>
                  <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
                  <div class="text-sm text-gray-500">{{ item.code }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ item.category }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              {{ item.currentStock }} {{ item.unit }}
              <span v-if="item.minStock > 0" class="text-gray-500">Min: {{ item.minStock }}</span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">R {{ item.costPrice.toFixed(2) }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">R {{ item.sellingPrice.toFixed(2) }}</td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStockStatus(item).color]">
                {{ getStockStatus(item).label }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center justify-end gap-2">
                <button
                  @click="handleAdjust(item)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="Adjust Stock"
                >
                  <i class="material-symbols-rounded text-lg">tune</i>
                </button>
                <button
                  @click="handleViewHistory(item)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="View History"
                >
                  <i class="material-symbols-rounded text-lg">history</i>
                </button>
                <button
                  @click="handleEdit(item)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="Edit Item"
                >
                  <i class="material-symbols-rounded text-lg">edit</i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Modals -->
    <ItemModal
      :show="showAddModal"
      :item="null"
      @close="showAddModal = false"
      @saved="handleItemSaved"
    />
    <ItemModal
      :show="showEditModal"
      :item="selectedItem"
      @close="showEditModal = false; selectedItem = null"
      @saved="handleItemSaved"
    />
    <StockAdjustmentModal
      :show="showAdjustModal"
      :item="selectedItem"
      @close="showAdjustModal = false; selectedItem = null"
      @adjusted="handleAdjustmentSaved"
    />
  </div>
</template>

