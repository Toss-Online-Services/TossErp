<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useStockStore, type Item, type StockMovement } from '~/stores/stock'
import { useBuyingStore } from '~/stores/buying'
import StockAdjustmentModal from '~/components/stock/StockAdjustmentModal.vue'

// Make this page client-only to avoid SSR issues
definePageMeta({
  ssr: false,
  layout: 'default'
})

useHead({
  title: 'Item Details - TOSS'
})

const route = useRoute()
const stockStore = useStockStore()
const buyingStore = useBuyingStore()

const item = ref<Item | null>(null)
const movements = ref<StockMovement[]>([])
const supplier = ref<any>(null)
const allSuppliers = ref<any[]>([])
const loading = ref(false)
const showAdjustModal = ref(false)

const itemId = computed(() => route.params.id as string)

const movementTypes = {
  purchase: { label: 'Purchase', color: 'text-green-600 bg-green-100', icon: 'shopping_cart' },
  sale: { label: 'Sale', color: 'text-blue-600 bg-blue-100', icon: 'point_of_sale' },
  adjustment: { label: 'Adjustment', color: 'text-orange-600 bg-orange-100', icon: 'tune' },
  transfer: { label: 'Transfer', color: 'text-purple-600 bg-purple-100', icon: 'swap_horiz' },
  return: { label: 'Return', color: 'text-gray-600 bg-gray-100', icon: 'undo' }
}

onMounted(async () => {
  console.log('Item detail page mounted, ID:', itemId.value)
  await loadItem()
})

async function loadItem() {
  loading.value = true
  try {
    await stockStore.fetchItems()
    await buyingStore.fetchSuppliers()
    
    const foundItem = stockStore.getItemById(itemId.value)
    console.log('Looking for item with ID:', itemId.value)
    console.log('Available items:', stockStore.items.map(i => ({ id: i.id, name: i.name })))
    console.log('Found item:', foundItem)
    
    item.value = foundItem || null
    
    if (!item.value) {
      console.error('Item not found for ID:', itemId.value)
      return
    }
    
    // Find primary supplier for this item (latest/default supplier)
    if (item.value.supplier) {
      supplier.value = buyingStore.suppliers.find(s => 
        s.name.toLowerCase() === item.value?.supplier?.toLowerCase()
      ) || null
    }
    
    // Load movements and purchase orders to find all suppliers
    await loadMovements()
    await buyingStore.fetchPurchaseOrders()
    
    // Find all suppliers who have supplied this item
    const supplierMap = new Map<string, any>()
    
    // Get suppliers from purchase movements
    movements.value
      .filter(m => m.type === 'purchase' && m.supplierName)
      .forEach(movement => {
        if (movement.supplierName && !supplierMap.has(movement.supplierName)) {
          const foundSupplier = buyingStore.suppliers.find(s => 
            s.name.toLowerCase() === movement.supplierName?.toLowerCase()
          )
          if (foundSupplier) {
            supplierMap.set(movement.supplierName, foundSupplier)
          }
        }
      })
    
    // Also check purchase orders for this item
    buyingStore.purchaseOrders.forEach(po => {
      const hasItem = po.items.some(poItem => poItem.itemId === item.value?.id)
      if (hasItem && po.supplierName && !supplierMap.has(po.supplierName)) {
        const foundSupplier = buyingStore.suppliers.find(s => 
          s.name.toLowerCase() === po.supplierName.toLowerCase()
        )
        if (foundSupplier) {
          supplierMap.set(po.supplierName, foundSupplier)
        }
      }
    })
    
    // Convert map to array
    allSuppliers.value = Array.from(supplierMap.values())
    
    // If no suppliers found from movements/POs but item has a supplier, add it
    if (supplier.value && !allSuppliers.value.find(s => s.id === supplier.value.id)) {
      allSuppliers.value.push(supplier.value)
    }
  } catch (error) {
    console.error('Failed to load item:', error)
  } finally {
    loading.value = false
  }
}

async function loadMovements() {
  try {
    await stockStore.fetchMovements(itemId.value)
    // Movements are already filtered in the store when itemId is provided
    movements.value = stockStore.movements.filter(m => m.itemId === itemId.value)
    
    // Enhance movements with supplier info from purchase orders if missing
    if (item.value) {
      await buyingStore.fetchPurchaseOrders()
      movements.value = movements.value.map(movement => {
        if (movement.type === 'purchase' && movement.referenceType === 'PO' && movement.referenceId && !movement.supplierName) {
          // Try to find the PO and get supplier info
          const po = buyingStore.purchaseOrders.find(po => 
            po.poNumber === movement.referenceId || po.id === movement.referenceId
          )
          if (po) {
            movement.supplierId = po.supplierId
            movement.supplierName = po.supplierName
          }
        }
        return movement
      })
    }
    
    console.log('Loaded movements:', movements.value.length, 'for item:', itemId.value)
  } catch (error) {
    console.error('Failed to load movements:', error)
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(new Date(date))
}

function getStockStatus(item: Item) {
  if (item.currentStock === 0) return { text: 'Out of Stock', color: 'text-red-600 bg-red-100' }
  if (item.currentStock <= item.minStock) return { text: 'Low Stock', color: 'text-orange-600 bg-orange-100' }
  return { text: 'In Stock', color: 'text-green-600 bg-green-100' }
}

function handleAdjust() {
  if (item.value) {
    showAdjustModal.value = true
  }
}

function handleStockAdjusted() {
  loadItem()
  loadMovements()
}
</script>

<template>
  <div class="py-6">
    <!-- Back Button -->
    <div class="mb-6">
      <NuxtLink
        to="/stock/items"
        class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 transition-colors"
      >
        <i class="material-symbols-rounded">arrow_back</i>
        <span>Back to Items</span>
      </NuxtLink>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
      <p class="mt-4 text-gray-600">Loading item details...</p>
    </div>

    <!-- Item Details -->
    <div v-else-if="item" class="space-y-6">
      <!-- Header -->
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-start justify-between mb-6">
          <div class="flex items-start gap-4">
            <div class="w-16 h-16 bg-gradient-to-br from-gray-800 to-gray-900 rounded-lg flex items-center justify-center text-white">
              <i class="material-symbols-rounded text-3xl">inventory_2</i>
            </div>
            <div>
              <h1 class="text-3xl font-bold text-gray-900 mb-1">{{ item.name }}</h1>
              <p class="text-gray-600">Code: {{ item.code }}</p>
              <p v-if="item.barcode" class="text-sm text-gray-500 mt-1">Barcode: {{ item.barcode }}</p>
            </div>
          </div>
          <div class="flex gap-2">
            <button
              @click="handleAdjust"
              class="px-4 py-2 bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-lg hover:shadow-lg transition-all"
            >
              <i class="material-symbols-rounded align-middle mr-2">tune</i>
              Adjust Stock
            </button>
          </div>
        </div>

        <!-- Item Info Grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div>
            <div class="text-sm text-gray-600 mb-1">Category</div>
            <div class="text-lg font-semibold text-gray-900">{{ item.category }}</div>
          </div>
          <div>
            <div class="text-sm text-gray-600 mb-1">Unit</div>
            <div class="text-lg font-semibold text-gray-900">{{ item.unit }}</div>
          </div>
          <div>
            <div class="text-sm text-gray-600 mb-1">Cost Price</div>
            <div class="text-lg font-semibold text-gray-900">{{ formatCurrency(item.costPrice) }}</div>
          </div>
          <div>
            <div class="text-sm text-gray-600 mb-1">Selling Price</div>
            <div class="text-lg font-semibold text-gray-900">{{ formatCurrency(item.sellingPrice) }}</div>
          </div>
        </div>
        
        <!-- Supplier Information -->
        <div v-if="supplier || item.supplier" class="mt-6 pt-6 border-t border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <div class="text-sm text-gray-600 mb-1">Supplier</div>
              <div v-if="supplier" class="flex items-center gap-2">
                <NuxtLink
                  :to="`/buying/suppliers/${supplier.id}`"
                  class="text-lg font-semibold text-gray-900 hover:text-gray-700 hover:underline flex items-center gap-2"
                >
                  <i class="material-symbols-rounded text-xl">store</i>
                  {{ supplier.name }}
                  <i class="material-symbols-rounded text-sm">open_in_new</i>
                </NuxtLink>
              </div>
              <div v-else class="text-lg font-semibold text-gray-900">{{ item.supplier }}</div>
            </div>
            <div v-if="supplier" class="flex items-center gap-4 text-sm text-gray-600">
              <div v-if="supplier.phone" class="flex items-center gap-1">
                <i class="material-symbols-rounded text-base">phone</i>
                <a :href="`tel:${supplier.phone}`" class="hover:text-gray-900 hover:underline">
                  {{ supplier.phone }}
                </a>
              </div>
              <div v-if="supplier.email" class="flex items-center gap-1">
                <i class="material-symbols-rounded text-base">email</i>
                <a :href="`mailto:${supplier.email}`" class="hover:text-gray-900 hover:underline">
                  {{ supplier.email }}
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Stock Status Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="text-sm text-gray-600 mb-2">Current Stock</div>
          <div class="text-3xl font-bold text-gray-900 mb-2">{{ item.currentStock }} {{ item.unit }}</div>
          <span :class="['px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full', getStockStatus(item).color]">
            {{ getStockStatus(item).text }}
          </span>
        </div>
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="text-sm text-gray-600 mb-2">Minimum Stock</div>
          <div class="text-3xl font-bold text-gray-900">{{ item.minStock }} {{ item.unit }}</div>
          <div class="text-xs text-gray-500 mt-2">
            {{ item.currentStock < item.minStock ? 'Below minimum' : 'Above minimum' }}
          </div>
        </div>
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="text-sm text-gray-600 mb-2">Stock Value</div>
          <div class="text-3xl font-bold text-gray-900">{{ formatCurrency(item.currentStock * item.costPrice) }}</div>
          <div class="text-xs text-gray-500 mt-2">At cost price</div>
        </div>
      </div>

      <!-- Stock Movements History -->
      <div class="bg-white rounded-xl shadow-sm overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200">
          <h2 class="text-xl font-bold text-gray-900">Stock Movement History</h2>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Supplier</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Reference</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Notes</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-if="loading">
                <td colspan="6" class="px-6 py-8 text-center text-gray-500">
                  Loading movements...
                </td>
              </tr>
              <tr v-else-if="movements.length === 0">
                <td colspan="6" class="px-6 py-8 text-center text-gray-500">
                  No stock movements recorded yet
                </td>
              </tr>
              <tr v-for="movement in movements" :key="movement.id" class="hover:bg-gray-50">
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ formatDate(movement.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    :class="[
                      'px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full',
                      movementTypes[movement.type]?.color || 'text-gray-600 bg-gray-100'
                    ]"
                  >
                    <i class="material-symbols-rounded text-sm mr-1">
                      {{ movementTypes[movement.type]?.icon || 'help' }}
                    </i>
                    {{ movementTypes[movement.type]?.label || movement.type }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold"
                    :class="movement.quantity > 0 ? 'text-green-600' : 'text-red-600'">
                  {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }} {{ item.unit }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  <div v-if="movement.type === 'purchase' && movement.supplierName">
                    <NuxtLink
                      v-if="movement.supplierId"
                      :to="`/buying/suppliers/${movement.supplierId}`"
                      class="text-gray-900 hover:text-gray-700 hover:underline flex items-center gap-1"
                    >
                      <i class="material-symbols-rounded text-base">store</i>
                      {{ movement.supplierName }}
                    </NuxtLink>
                    <span v-else class="flex items-center gap-1">
                      <i class="material-symbols-rounded text-base">store</i>
                      {{ movement.supplierName }}
                    </span>
                  </div>
                  <span v-else class="text-gray-400">-</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  <span v-if="movement.referenceType && movement.referenceId">
                    {{ movement.referenceType }} #{{ movement.referenceId }}
                  </span>
                  <span v-else class="text-gray-400">-</span>
                </td>
                <td class="px-6 py-4 text-sm text-gray-500">
                  {{ movement.notes || '-' }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- All Suppliers Section -->
      <div v-if="allSuppliers.length > 0" class="bg-white rounded-xl shadow-sm overflow-hidden">
        <div class="px-6 py-4 border-b border-gray-200">
          <h2 class="text-xl font-bold text-gray-900 flex items-center gap-2">
            <i class="material-symbols-rounded text-xl">store</i>
            All Suppliers for This Item
          </h2>
          <p class="text-sm text-gray-600 mt-1">Suppliers who have supplied this product</p>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <div
              v-for="supp in allSuppliers"
              :key="supp.id"
              class="border border-gray-200 rounded-lg p-4 hover:border-gray-300 hover:shadow-md transition-all"
            >
              <div class="flex items-start justify-between mb-3">
                <NuxtLink
                  :to="`/buying/suppliers/${supp.id}`"
                  class="flex items-center gap-2 text-base font-semibold text-gray-900 hover:text-gray-700 hover:underline flex-1"
                >
                  <i class="material-symbols-rounded text-xl">store</i>
                  {{ supp.name }}
                  <i class="material-symbols-rounded text-sm">open_in_new</i>
                </NuxtLink>
                <span
                  v-if="supp.id === supplier?.id"
                  class="px-2 py-0.5 text-xs font-medium rounded-full bg-gray-100 text-gray-700"
                >
                  Primary
                </span>
              </div>
              <div class="space-y-2 text-sm text-gray-600">
                <div v-if="supp.code" class="flex items-center gap-1">
                  <i class="material-symbols-rounded text-base">tag</i>
                  <span>{{ supp.code }}</span>
                </div>
                <div v-if="supp.phone" class="flex items-center gap-1">
                  <i class="material-symbols-rounded text-base">phone</i>
                  <a :href="`tel:${supp.phone}`" class="hover:text-gray-900 hover:underline">
                    {{ supp.phone }}
                  </a>
                </div>
                <div v-if="supp.email" class="flex items-center gap-1">
                  <i class="material-symbols-rounded text-base">email</i>
                  <a :href="`mailto:${supp.email}`" class="hover:text-gray-900 hover:underline">
                    {{ supp.email }}
                  </a>
                </div>
                <div v-if="supp.paymentTerms" class="flex items-center gap-1">
                  <i class="material-symbols-rounded text-base">account_balance</i>
                  <span>{{ supp.paymentTerms }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Item Not Found -->
    <div v-else class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">error</i>
      <h3 class="text-lg font-medium text-gray-900 mb-2">Item not found</h3>
      <NuxtLink
        to="/stock/items"
        class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800"
      >
        <i class="material-symbols-rounded">arrow_back</i>
        <span>Back to Items</span>
      </NuxtLink>
    </div>

    <!-- Stock Adjustment Modal -->
    <StockAdjustmentModal
      v-if="item"
      :show="showAdjustModal"
      :item="item"
      @close="showAdjustModal = false"
      @adjusted="handleStockAdjusted"
    />
  </div>
</template>

