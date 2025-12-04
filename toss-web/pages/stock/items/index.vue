<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'
import { useBuyingStore } from '~/stores/buying'
import ItemModal from '~/components/stock/ItemModal.vue'
import ItemViewModal from '~/components/stock/ItemViewModal.vue'
import StockAdjustmentModal from '~/components/stock/StockAdjustmentModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Stock Items - TOSS'
})

const stockStore = useStockStore()
const buyingStore = useBuyingStore()
const searchQuery = ref('')
const showAddModal = ref(false)
const showEditModal = ref(false)
const showViewModal = ref(false)
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
  await buyingStore.fetchSuppliers()
  console.log('Items loaded:', stockStore.items.length, stockStore.items)
})

function getSupplierForItem(item: Item) {
  if (!item.supplier) return null
  return buyingStore.suppliers.find(s => 
    s.name.toLowerCase() === item.supplier?.toLowerCase()
  ) || null
}

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
  selectedItem.value = item
  showViewModal.value = true
}

function handleViewEdit(item: Item) {
  showViewModal.value = false
  handleEdit(item)
}

function handleViewAdjust(item: Item) {
  // Close view modal and open adjust modal
  showViewModal.value = false
  selectedItem.value = item
  showAdjustModal.value = true
}

function handleAdjustmentSaved() {
  showAdjustModal.value = false
  stockStore.fetchItems()
  // Reopen view modal if we have a selected item
  if (selectedItem.value) {
    showViewModal.value = true
  }
}

function handleItemSaved() {
  showAddModal.value = false
  showEditModal.value = false
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

function getItemImage(itemId: string) {
  const images = [
    '/images/products/product-1-min.jpg',
    '/images/products/product-2-min.jpg',
    '/images/products/product-3-min.jpg',
    '/images/products/product-4-min.jpg',
    '/images/products/product-5-min.jpg',
    '/images/products/product-6-min.jpg',
    '/images/products/product-7-min.jpg',
    '/images/products/product-11.jpg',
    '/images/products/product-details-1.jpg',
    '/images/products/product-details-2.jpg',
    '/images/products/product-details-3.jpg',
    '/images/products/product-details-4.jpg',
    '/images/products/product-details-5.jpg'
  ]
  // Use item ID to consistently select an image for the same item
  const index = parseInt(itemId) % images.length
  return images[index]
}
</script>

<template>
  <div class="py-6">
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
      <!-- Card Header -->
      <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
          <div>
            <h5 class="mb-0 text-lg font-semibold text-gray-900">All Items</h5>
            <p class="text-sm text-gray-600 mb-0 mt-1">
              Manage your inventory and track stock levels
            </p>
          </div>
          <div class="flex items-center gap-2">
            <button
              @click="handleAdd"
              class="px-4 py-2 bg-gray-900 text-white text-sm font-medium rounded-lg hover:bg-gray-800 transition-colors flex items-center gap-2"
            >
              <i class="material-symbols-rounded text-lg">add</i>
              <span>New Item</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="px-6 py-4 border-b border-gray-200 bg-gray-50">
        <div class="flex flex-col md:flex-row gap-4">
          <div class="flex-1">
            <div class="relative">
              <i class="material-symbols-rounded absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-lg">search</i>
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search by name, code, or barcode..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent bg-white text-sm"
              />
            </div>
          </div>
          <div class="md:w-64">
            <select
              v-model="selectedCategory"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent bg-white text-sm"
            >
              <option value="all">All Categories</option>
              <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Table Body -->
      <div class="overflow-x-auto">
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
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Item</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Category</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Supplier</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Price</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Code</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Quantity</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Status</th>
              <th class="px-6 py-3 text-left text-xs font-semibold text-gray-600 uppercase tracking-wider">Action</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="item in filteredItems" :key="item.id" class="hover:bg-gray-50 transition-colors">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <img
                    v-if="item.imageUrl"
                    :src="item.imageUrl"
                    :alt="item.name"
                    class="w-10 h-10 rounded-lg object-cover mr-3 flex-shrink-0"
                  />
                  <img
                    v-else
                    :src="getItemImage(item.id)"
                    :alt="item.name"
                    class="w-10 h-10 rounded-lg object-cover mr-3 flex-shrink-0"
                  />
                  <div>
                    <h6 class="mb-0 text-sm font-semibold text-gray-900">{{ item.name }}</h6>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <p class="text-sm text-gray-600 mb-0">{{ item.category }}</p>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div v-if="getSupplierForItem(item)">
                  <NuxtLink
                    :to="`/buying/suppliers/${getSupplierForItem(item)?.id}`"
                    class="text-sm text-gray-900 hover:text-gray-700 hover:underline flex items-center gap-1"
                  >
                    <i class="material-symbols-rounded text-base">store</i>
                    {{ getSupplierForItem(item)?.name }}
                  </NuxtLink>
                </div>
                <span v-else class="text-sm text-gray-400">-</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <p class="text-sm text-gray-600 mb-0">R {{ item.sellingPrice.toFixed(2) }}</p>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <p class="text-sm text-gray-600 mb-0">{{ item.code }}</p>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <p class="text-sm text-gray-600 mb-0">{{ item.currentStock }} {{ item.unit }}</p>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span 
                  :class="[
                    'px-2 py-1 text-xs font-semibold rounded-full',
                    getStockStatus(item).color === 'text-red-600 bg-red-100' ? 'bg-red-100 text-red-600' :
                    getStockStatus(item).color === 'text-orange-600 bg-orange-100' ? 'bg-orange-100 text-orange-600' :
                    'bg-green-100 text-green-600'
                  ]"
                >
                  {{ getStockStatus(item).label }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center gap-3">
                  <button
                    @click="handleViewHistory(item)"
                    class="text-gray-600 hover:text-gray-900 transition-colors"
                    title="View item"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                  <button
                    @click="handleEdit(item)"
                    class="text-gray-600 hover:text-gray-900 transition-colors"
                    title="Edit item"
                  >
                    <i class="material-symbols-rounded text-lg">drive_file_rename_outline</i>
                  </button>
                  <button
                    @click="handleAdjust(item)"
                    class="text-gray-600 hover:text-gray-900 transition-colors"
                    title="Adjust stock"
                  >
                    <i class="material-symbols-rounded text-lg">tune</i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
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
    <ItemViewModal
      :show="showViewModal"
      :item="selectedItem"
      @close="showViewModal = false; selectedItem = null"
      @edit="handleViewEdit"
      @adjust="handleViewAdjust"
    />
    <StockAdjustmentModal
      :show="showAdjustModal"
      :item="selectedItem"
      @close="showAdjustModal = false; selectedItem = null"
      @adjusted="handleAdjustmentSaved"
    />
  </div>
</template>

