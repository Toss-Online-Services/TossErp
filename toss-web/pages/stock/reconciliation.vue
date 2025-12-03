<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'

useHead({
  title: 'Stock Reconciliation - TOSS'
})

const stockStore = useStockStore()
const items = ref<Item[]>([])
const loading = ref(false)
const selectedCategory = ref('all')
const searchQuery = ref('')
const reconciliationData = ref<Record<string, { systemQty: number; countedQty: number }>>({})

const categories = computed(() => {
  const cats = new Set(items.value.map(item => item.category))
  return ['all', ...Array.from(cats)]
})

const filteredItems = computed(() => {
  let filtered = items.value

  if (selectedCategory.value !== 'all') {
    filtered = filtered.filter(item => item.category === selectedCategory.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(item =>
      item.name.toLowerCase().includes(query) ||
      item.code.toLowerCase().includes(query)
    )
  }

  return filtered
})

const discrepancies = computed(() => {
  return filteredItems.value.filter(item => {
    const data = reconciliationData.value[item.id]
    if (!data) return false
    return data.systemQty !== data.countedQty
  })
})

onMounted(async () => {
  await loadItems()
  initializeReconciliationData()
})

async function loadItems() {
  loading.value = true
  try {
    await stockStore.fetchItems()
    items.value = stockStore.items.filter(item => item.isActive)
    initializeReconciliationData()
  } catch (error) {
    console.error('Failed to load items:', error)
  } finally {
    loading.value = false
  }
}

function initializeReconciliationData() {
  items.value.forEach(item => {
    if (!reconciliationData.value[item.id]) {
      reconciliationData.value[item.id] = {
        systemQty: item.currentStock,
        countedQty: item.currentStock
      }
    } else {
      reconciliationData.value[item.id].systemQty = item.currentStock
    }
  })
}

function updateCountedQty(itemId: string, qty: number) {
  if (!reconciliationData.value[itemId]) {
    const item = items.value.find(i => i.id === itemId)
    if (item) {
      reconciliationData.value[itemId] = {
        systemQty: item.currentStock,
        countedQty: qty
      }
    }
  } else {
    reconciliationData.value[itemId].countedQty = qty
  }
}

function getVariance(itemId: string) {
  const data = reconciliationData.value[itemId]
  if (!data) return 0
  return data.countedQty - data.systemQty
}

function getVarianceColor(itemId: string) {
  const variance = getVariance(itemId)
  if (variance === 0) return 'text-gray-600'
  if (variance > 0) return 'text-green-600'
  return 'text-red-600'
}

async function finalizeReconciliation() {
  if (!confirm('Are you sure you want to finalize the stock reconciliation? This will adjust stock levels.')) {
    return
  }

  loading.value = true
  try {
    const adjustments: Array<{ itemId: string; quantity: number; notes: string }> = []

    for (const [itemId, data] of Object.entries(reconciliationData.value)) {
      const variance = data.countedQty - data.systemQty
      if (variance !== 0) {
        adjustments.push({
          itemId,
          quantity: variance,
          notes: `Stock reconciliation: System had ${data.systemQty}, counted ${data.countedQty}`
        })
      }
    }

    // Apply all adjustments
    for (const adjustment of adjustments) {
      await stockStore.adjustStock(adjustment.itemId, adjustment.quantity, adjustment.notes)
    }

    alert(`Stock reconciliation completed. ${adjustments.length} items adjusted.`)
    await loadItems()
  } catch (error) {
    console.error('Failed to finalize reconciliation:', error)
    alert('Failed to finalize reconciliation. Please try again.')
  } finally {
    loading.value = false
  }
}

function resetCounts() {
  if (!confirm('Reset all counted quantities to system values?')) {
    return
  }
  initializeReconciliationData()
}
</script>

<template>
  <div class="py-6">
    <!-- Page Header -->
    <div class="mb-8 flex items-center justify-between">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Stock Reconciliation</h3>
        <p class="text-gray-600 text-sm">
          Count physical stock and reconcile with system records
        </p>
      </div>
      <div class="flex gap-2">
        <button
          @click="resetCounts"
          class="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
        >
          <i class="material-symbols-rounded align-middle mr-2">refresh</i>
          Reset
        </button>
        <button
          @click="finalizeReconciliation"
          :disabled="loading || discrepancies.length === 0"
          class="px-4 py-2 bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-lg hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <i class="material-symbols-rounded align-middle mr-2">check_circle</i>
          Finalize Reconciliation
        </button>
      </div>
    </div>

    <!-- Summary Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-3 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="text-sm text-gray-600 mb-1">Items to Count</div>
        <div class="text-3xl font-bold text-gray-900">{{ filteredItems.length }}</div>
      </div>
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="text-sm text-gray-600 mb-1">Discrepancies</div>
        <div class="text-3xl font-bold" :class="discrepancies.length > 0 ? 'text-orange-600' : 'text-green-600'">
          {{ discrepancies.length }}
        </div>
      </div>
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="text-sm text-gray-600 mb-1">Items Counted</div>
        <div class="text-3xl font-bold text-gray-900">
          {{ filteredItems.filter(item => reconciliationData[item.id]?.countedQty !== undefined).length }}
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search items..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
        </div>
        <div class="w-full md:w-64">
          <select
            v-model="selectedCategory"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
            <option v-for="cat in categories" :key="cat" :value="cat">
              {{ cat === 'all' ? 'All Categories' : cat }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <!-- Reconciliation Table -->
    <div class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">System Qty</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Counted Qty</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Variance</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-if="loading">
              <td colspan="4" class="px-6 py-8 text-center text-gray-500">
                Loading items...
              </td>
            </tr>
            <tr v-else-if="filteredItems.length === 0">
              <td colspan="4" class="px-6 py-8 text-center text-gray-500">
                No items found
              </td>
            </tr>
            <tr
              v-for="item in filteredItems"
              :key="item.id"
              class="hover:bg-gray-50"
              :class="{ 'bg-yellow-50': reconciliationData[item.id] && reconciliationData[item.id].systemQty !== reconciliationData[item.id].countedQty }"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
                <div class="text-xs text-gray-500">{{ item.code }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">
                  {{ reconciliationData[item.id]?.systemQty ?? item.currentStock }} {{ item.unit }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <input
                  :value="reconciliationData[item.id]?.countedQty ?? item.currentStock"
                  @input="updateCountedQty(item.id, parseFloat(($event.target as HTMLInputElement).value) || 0)"
                  type="number"
                  min="0"
                  step="0.01"
                  class="w-24 px-3 py-1 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900 text-sm"
                >
                <span class="ml-2 text-sm text-gray-500">{{ item.unit }}</span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-semibold" :class="getVarianceColor(item.id)">
                  {{ getVariance(item.id) > 0 ? '+' : '' }}{{ getVariance(item.id) }} {{ item.unit }}
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

