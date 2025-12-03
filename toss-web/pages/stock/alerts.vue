<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'
import StockAdjustmentModal from '~/components/stock/StockAdjustmentModal.vue'

useHead({
  title: 'Low Stock Alerts - TOSS'
})

const stockStore = useStockStore()
const showAdjustModal = ref(false)
const selectedItem = ref<Item | null>(null)

const lowStockItems = computed(() => stockStore.lowStockItems)
const outOfStockItems = computed(() => stockStore.outOfStockItems)

const allAlerts = computed(() => {
  return [
    ...outOfStockItems.value.map(item => ({ ...item, severity: 'critical' as const })),
    ...lowStockItems.value.map(item => ({ ...item, severity: 'warning' as const }))
  ].sort((a, b) => {
    // Critical (out of stock) first
    if (a.severity === 'critical' && b.severity !== 'critical') return -1
    if (b.severity === 'critical' && a.severity !== 'critical') return 1
    // Then by stock level
    return a.currentStock - b.currentStock
  })
})

onMounted(async () => {
  await stockStore.fetchItems()
})

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function getSeverityColor(severity: 'critical' | 'warning') {
  return severity === 'critical'
    ? 'text-red-600 bg-red-100 border-red-200'
    : 'text-orange-600 bg-orange-100 border-orange-200'
}

function getSeverityIcon(severity: 'critical' | 'warning') {
  return severity === 'critical' ? 'error' : 'warning'
}

function handleAdjust(item: Item) {
  selectedItem.value = item
  showAdjustModal.value = true
}

function handleStockAdjusted() {
  stockStore.fetchItems()
}

function handleReorder(item: Item) {
  // Navigate to buying module to create PO
  navigateTo(`/buying/purchase-orders?item=${item.id}`)
}
</script>

<template>
  <div class="py-6">
    <!-- Page Header -->
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Stock Alerts</h3>
      <p class="text-gray-600 text-sm">
        Items that need attention - low stock or out of stock
      </p>
    </div>

    <!-- Summary Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6 border-l-4 border-red-500">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Out of Stock</p>
            <h4 class="text-3xl font-bold text-red-600">{{ outOfStockItems.length }}</h4>
            <p class="text-xs text-gray-500 mt-2">Items with zero stock</p>
          </div>
          <div class="w-16 h-16 rounded-lg bg-red-100 flex items-center justify-center">
            <i class="material-symbols-rounded text-3xl text-red-600">error</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6 border-l-4 border-orange-500">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Low Stock</p>
            <h4 class="text-3xl font-bold text-orange-600">{{ lowStockItems.length }}</h4>
            <p class="text-xs text-gray-500 mt-2">Items below minimum level</p>
          </div>
          <div class="w-16 h-16 rounded-lg bg-orange-100 flex items-center justify-center">
            <i class="material-symbols-rounded text-3xl text-orange-600">warning</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Alerts List -->
    <div v-if="allAlerts.length === 0" class="bg-white rounded-xl shadow-sm p-12 text-center">
      <i class="material-symbols-rounded text-6xl text-green-500 mb-4">check_circle</i>
      <h3 class="text-lg font-medium text-gray-900 mb-2">All Good!</h3>
      <p class="text-gray-600">No stock alerts at this time. All items are well stocked.</p>
    </div>

    <div v-else class="space-y-4">
      <div
        v-for="item in allAlerts"
        :key="item.id"
        class="bg-white rounded-xl shadow-sm p-6 border-l-4"
        :class="item.severity === 'critical' ? 'border-red-500' : 'border-orange-500'"
      >
        <div class="flex items-start justify-between">
          <div class="flex items-start gap-4 flex-1">
            <div
              class="w-12 h-12 rounded-lg flex items-center justify-center"
              :class="item.severity === 'critical' ? 'bg-red-100' : 'bg-orange-100'"
            >
              <i
                class="material-symbols-rounded text-2xl"
                :class="item.severity === 'critical' ? 'text-red-600' : 'text-orange-600'"
              >
                {{ getSeverityIcon(item.severity) }}
              </i>
            </div>
            <div class="flex-1">
              <h4 class="text-lg font-semibold text-gray-900 mb-1">{{ item.name }}</h4>
              <div class="flex items-center gap-4 text-sm text-gray-600 mb-2">
                <span>Code: {{ item.code }}</span>
                <span>Category: {{ item.category }}</span>
              </div>
              <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mt-4">
                <div>
                  <div class="text-xs text-gray-500 mb-1">Current Stock</div>
                  <div class="text-lg font-bold" :class="item.severity === 'critical' ? 'text-red-600' : 'text-orange-600'">
                    {{ item.currentStock }} {{ item.unit }}
                  </div>
                </div>
                <div>
                  <div class="text-xs text-gray-500 mb-1">Minimum Level</div>
                  <div class="text-lg font-semibold text-gray-900">{{ item.minStock }} {{ item.unit }}</div>
                </div>
                <div>
                  <div class="text-xs text-gray-500 mb-1">Suggested Reorder</div>
                  <div class="text-lg font-semibold text-gray-900">
                    {{ Math.max(item.minStock * 2 - item.currentStock, item.minStock) }} {{ item.unit }}
                  </div>
                </div>
                <div>
                  <div class="text-xs text-gray-500 mb-1">Stock Value</div>
                  <div class="text-lg font-semibold text-gray-900">
                    {{ formatCurrency(item.currentStock * item.costPrice) }}
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="flex flex-col gap-2 ml-4">
            <button
              @click="handleAdjust(item)"
              class="px-4 py-2 text-sm bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
            >
              <i class="material-symbols-rounded align-middle mr-1 text-sm">tune</i>
              Adjust
            </button>
            <button
              @click="handleReorder(item)"
              class="px-4 py-2 text-sm bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-lg hover:shadow-lg transition-all"
            >
              <i class="material-symbols-rounded align-middle mr-1 text-sm">shopping_cart</i>
              Reorder
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Stock Adjustment Modal -->
    <StockAdjustmentModal
      :show="showAdjustModal"
      :item="selectedItem"
      @close="showAdjustModal = false; selectedItem = null"
      @adjusted="handleStockAdjusted"
    />
  </div>
</template>

