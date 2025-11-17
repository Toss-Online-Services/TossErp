<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-4">
              <CubeIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ item?.name }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ item?.category }}
              </p>
            </div>
          </div>
          <div class="flex items-center space-x-2">
            <button
              @click="item && $emit('edit', item)"
              class="inline-flex items-center px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <PencilIcon class="w-4 h-4 mr-1" />
              Edit
            </button>
            <button
              @click="$emit('close')"
              class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
            >
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
        </div>
      </div>

      <!-- Content -->
      <div class="p-6 overflow-y-auto max-h-[calc(90vh-120px)]">
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <!-- Main Details -->
          <div class="lg:col-span-2 space-y-8">
            <!-- Basic Information -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Basic Information</h4>
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                <dl class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">SKU</dt>
                    <dd class="mt-1 text-sm font-mono text-gray-900 dark:text-white">{{ item?.sku }}</dd>
                  </div>
                  <div v-if="item?.barcode">
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Barcode</dt>
                    <dd class="mt-1 text-sm font-mono text-gray-900 dark:text-white">{{ item.barcode }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Unit of Measure</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ item?.unit }}</dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Status</dt>
                    <dd class="mt-1">
                      <span 
                        class="inline-flex px-2 py-1 text-xs font-semibold rounded-full"
                        :class="{
                          'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': item?.isActive,
                          'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200': !item?.isActive
                        }"
                      >
                        {{ item?.isActive ? 'Active' : 'Inactive' }}
                      </span>
                    </dd>
                  </div>
                </dl>
                <div v-if="item?.description" class="mt-4">
                  <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Description</dt>
                  <dd class="mt-1 text-sm text-gray-900 dark:text-white">{{ item.description }}</dd>
                </div>
              </div>
            </div>

            <!-- Pricing Information -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Pricing</h4>
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                <dl class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Selling Price</dt>
                    <dd class="mt-1 text-lg font-semibold text-green-600 dark:text-green-400">
                      R{{ formatCurrency(item?.sellingPrice || 0) }}
                    </dd>
                  </div>
                  <div v-if="item?.costPrice">
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Cost Price</dt>
                    <dd class="mt-1 text-lg font-semibold text-gray-900 dark:text-white">
                      R{{ formatCurrency(item.costPrice) }}
                    </dd>
                  </div>
                  <div v-if="item?.costPrice && item?.sellingPrice">
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Profit Margin</dt>
                    <dd class="mt-1 text-sm text-gray-900 dark:text-white">
                      {{ calculateProfitMargin(item.costPrice, item.sellingPrice) }}%
                    </dd>
                  </div>
                  <div v-if="item?.quantityOnHand !== undefined">
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Total Value</dt>
                    <dd class="mt-1 text-lg font-semibold text-blue-600 dark:text-blue-400">
                      R{{ formatCurrency((item.quantityOnHand || 0) * (item.costPrice || item.sellingPrice)) }}
                    </dd>
                  </div>
                </dl>
              </div>
            </div>

            <!-- Stock Levels -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Stock Information</h4>
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                <dl class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Current Stock</dt>
                    <dd class="mt-1 text-2xl font-bold text-gray-900 dark:text-white">
                      {{ item?.quantityOnHand || 0 }}
                      <span class="text-sm font-normal text-gray-500 dark:text-gray-400">{{ item?.unit }}</span>
                    </dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Reorder Level</dt>
                    <dd class="mt-1 text-xl font-semibold text-orange-600 dark:text-orange-400">
                      {{ item?.reorderLevel }}
                      <span class="text-sm font-normal text-gray-500 dark:text-gray-400">{{ item?.unit }}</span>
                    </dd>
                  </div>
                  <div>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400">Reorder Quantity</dt>
                    <dd class="mt-1 text-xl font-semibold text-blue-600 dark:text-blue-400">
                      {{ item?.reorderQty }}
                      <span class="text-sm font-normal text-gray-500 dark:text-gray-400">{{ item?.unit }}</span>
                    </dd>
                  </div>
                </dl>
                
                <!-- Stock Status Alert -->
                <div v-if="stockStatus" class="mt-4 p-3 rounded-lg" :class="stockStatus.class">
                  <div class="flex items-center">
                    <component :is="stockStatus.icon" class="w-5 h-5 mr-2" />
                    <span class="text-sm font-medium">{{ stockStatus.message }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Recent Transactions -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Recent Transactions</h4>
              <div class="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg">
                <div class="p-4 text-center text-gray-500 dark:text-gray-400">
                  <ChartBarIcon class="w-8 h-8 mx-auto mb-2" />
                  <p class="text-sm">Transaction history will be displayed here</p>
                  <p class="text-xs">Feature coming soon</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Sidebar Actions -->
          <div class="space-y-6">
            <!-- Quick Actions -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Actions</h4>
              <div class="space-y-3">
                <button
                  @click="adjustStock"
                  class="w-full inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
                >
                  <PlusCircleIcon class="w-4 h-4 mr-2" />
                  Adjust Stock
                </button>
                <button
                  @click="viewHistory"
                  class="w-full inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
                >
                  <ClockIcon class="w-4 h-4 mr-2" />
                  View History
                </button>
                <button
                  @click="printLabel"
                  class="w-full inline-flex items-center justify-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
                >
                  <PrinterIcon class="w-4 h-4 mr-2" />
                  Print Label
                </button>
              </div>
            </div>

            <!-- Item Image Placeholder -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Item Image</h4>
              <div class="aspect-square bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">
                <div class="text-center">
                  <PhotoIcon class="w-16 h-16 text-gray-400 mx-auto mb-2" />
                  <p class="text-sm text-gray-500 dark:text-gray-400">No image available</p>
                </div>
              </div>
            </div>

            <!-- Key Metrics -->
            <div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Key Metrics</h4>
              <div class="space-y-4">
                <div class="bg-blue-50 dark:bg-blue-900 rounded-lg p-3">
                  <div class="text-sm font-medium text-blue-800 dark:text-blue-200">Days of Stock</div>
                  <div class="text-lg font-semibold text-blue-900 dark:text-blue-100">
                    {{ calculateDaysOfStock() }} days
                  </div>
                </div>
                <div class="bg-green-50 dark:bg-green-900 rounded-lg p-3">
                  <div class="text-sm font-medium text-green-800 dark:text-green-200">Stock Turnover</div>
                  <div class="text-lg font-semibold text-green-900 dark:text-green-100">
                    N/A
                  </div>
                </div>
              </div>
            </div>

            <!-- Danger Zone -->
            <div>
              <h4 class="text-lg font-semibold text-red-600 dark:text-red-400 mb-4">Danger Zone</h4>
              <button
                @click="item && $emit('delete', item)"
                class="w-full inline-flex items-center justify-center px-4 py-2 border border-red-300 dark:border-red-600 rounded-lg text-sm font-medium text-red-700 dark:text-red-300 bg-red-50 dark:bg-red-900 hover:bg-red-100 dark:hover:bg-red-800"
              >
                <TrashIcon class="w-4 h-4 mr-2" />
                Delete Item
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  XMarkIcon,
  CubeIcon,
  PencilIcon,
  PlusCircleIcon,
  ClockIcon,
  PrinterIcon,
  PhotoIcon,
  ChartBarIcon,
  TrashIcon,
  ExclamationTriangleIcon,
  XCircleIcon,
  CheckCircleIcon
} from '@heroicons/vue/24/outline'
import type { ItemDto } from '../../composables/useStock'

// Props
interface Props {
  item?: ItemDto | null
}

const props = withDefaults(defineProps<Props>(), {
  item: null
})

// Emits
const emit = defineEmits<{
  close: []
  edit: [item: ItemDto]
  delete: [item: ItemDto]
}>()

// Computed
const stockStatus = computed(() => {
  if (!props.item) return null
  
  const currentStock = props.item.quantityOnHand || 0
  const reorderLevel = props.item.reorderLevel
  
  if (currentStock === 0) {
    return {
      message: 'Out of Stock - Immediate action required',
      class: 'bg-red-50 dark:bg-red-900 text-red-800 dark:text-red-200',
      icon: XCircleIcon
    }
  } else if (currentStock <= reorderLevel) {
    return {
      message: 'Low Stock - Consider reordering soon',
      class: 'bg-orange-50 dark:bg-orange-900 text-orange-800 dark:text-orange-200',
      icon: ExclamationTriangleIcon
    }
  } else {
    return {
      message: 'Stock levels are healthy',
      class: 'bg-green-50 dark:bg-green-900 text-green-800 dark:text-green-200',
      icon: CheckCircleIcon
    }
  }
})

// Methods
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const calculateProfitMargin = (costPrice: number, sellingPrice: number) => {
  if (costPrice === 0) return 0
  return Math.round(((sellingPrice - costPrice) / costPrice) * 100)
}

const calculateDaysOfStock = () => {
  if (!props.item) return 0
  
  // Placeholder calculation - in real app, this would use historical consumption data
  const currentStock = props.item.quantityOnHand || 0
  const reorderLevel = props.item.reorderLevel
  
  if (reorderLevel === 0) return 0
  
  // Assuming average daily consumption is 10% of reorder level
  const avgDailyConsumption = reorderLevel * 0.1
  if (avgDailyConsumption === 0) return 0
  
  return Math.round(currentStock / avgDailyConsumption)
}

const adjustStock = () => {
  if (!props.item) return
  
  const adjustment = prompt(`Enter adjustment quantity for ${props.item.name}:\n\nCurrent stock: ${props.item.quantityOnHand || 0} ${props.item.unit}\n\nEnter positive number to add, negative to reduce:`)
  
  if (adjustment !== null) {
    const qty = parseFloat(adjustment)
    if (!isNaN(qty)) {
      const newQty = (props.item.quantityOnHand || 0) + qty
      alert(`Stock adjusted!\n\nOld quantity: ${props.item.quantityOnHand || 0}\nAdjustment: ${qty > 0 ? '+' : ''}${qty}\nNew quantity: ${newQty}\n\nThis would create a stock adjustment entry.`)
    } else {
      alert('Invalid quantity entered')
    }
  }
}

const viewHistory = () => {
  if (!props.item) return
  
  // Mock history data
  const history = `
Stock Movement History - ${props.item.name}

Recent Transactions:
- Jan 15, 2024: +50 units (Purchase Receipt)
- Jan 14, 2024: -15 units (Sales Issue)
- Jan 13, 2024: +25 units (Stock Adjustment)
- Jan 12, 2024: -8 units (Sales Issue)
- Jan 11, 2024: +100 units (Purchase Receipt)

Current Balance: ${props.item.quantityOnHand || 0} ${props.item.unit}
  `
  alert(history)
}

const printLabel = () => {
  if (!props.item) return
  
  alert(`Printing barcode label for ${props.item.name}\n\nLabel will include:\n- Item Name\n- SKU: ${props.item.sku}\n- Barcode: ${props.item.barcode || 'N/A'}\n- Price: R${formatCurrency(props.item.sellingPrice)}\n\nSending to thermal printer...`)
}
</script>
