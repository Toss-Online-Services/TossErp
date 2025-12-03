<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useStockStore, type StockMovement } from '~/stores/stock'

useHead({
  title: 'Stock Movements - TOSS'
})

const stockStore = useStockStore()
const movements = ref<StockMovement[]>([])
const loading = ref(false)
const filterType = ref<string>('all')
const filterItem = ref<string>('')

const movementTypes = {
  purchase: { label: 'Purchase', color: 'text-green-600 bg-green-100', icon: 'shopping_cart' },
  sale: { label: 'Sale', color: 'text-blue-600 bg-blue-100', icon: 'point_of_sale' },
  adjustment: { label: 'Adjustment', color: 'text-orange-600 bg-orange-100', icon: 'tune' },
  transfer: { label: 'Transfer', color: 'text-purple-600 bg-purple-100', icon: 'swap_horiz' },
  return: { label: 'Return', color: 'text-gray-600 bg-gray-100', icon: 'undo' }
}

const filteredMovements = computed(() => {
  let filtered = movements.value

  if (filterType.value !== 'all') {
    filtered = filtered.filter(m => m.type === filterType.value)
  }

  if (filterItem.value) {
    const query = filterItem.value.toLowerCase()
    filtered = filtered.filter(m =>
      m.itemName.toLowerCase().includes(query) ||
      m.itemId.toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
})

onMounted(async () => {
  await loadMovements()
})

async function loadMovements() {
  loading.value = true
  try {
    await stockStore.fetchMovements()
    movements.value = stockStore.movements
  } catch (error) {
    console.error('Failed to load movements:', error)
  } finally {
    loading.value = false
  }
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(new Date(date))
}
</script>

<template>
  <div class="py-6">
    <!-- Page Header -->
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Stock Movements</h3>
      <p class="text-gray-600 text-sm">
        View all stock transactions and movements
      </p>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <div class="flex-1">
          <input
            v-model="filterItem"
            type="text"
            placeholder="Search by item name or code..."
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
        </div>
        <div class="w-full md:w-48">
          <select
            v-model="filterType"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
          >
            <option value="all">All Types</option>
            <option value="purchase">Purchase</option>
            <option value="sale">Sale</option>
            <option value="adjustment">Adjustment</option>
            <option value="transfer">Transfer</option>
            <option value="return">Return</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Movements Table -->
    <div class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Warehouse</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Reference</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Notes</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-if="loading">
              <td colspan="7" class="px-6 py-8 text-center text-gray-500">
                Loading movements...
              </td>
            </tr>
            <tr v-else-if="filteredMovements.length === 0">
              <td colspan="7" class="px-6 py-8 text-center text-gray-500">
                No stock movements found
              </td>
            </tr>
            <tr
              v-for="movement in filteredMovements"
              :key="movement.id"
              class="hover:bg-gray-50"
            >
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(movement.createdAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900">{{ movement.itemName }}</div>
                <div class="text-xs text-gray-500">{{ movement.itemId }}</div>
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
                {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ movement.warehouse }}
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
  </div>
</template>

