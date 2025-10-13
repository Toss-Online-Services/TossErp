<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-3xl w-full max-h-[90vh] overflow-hidden">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <div 
              class="w-10 h-10 rounded-lg flex items-center justify-center mr-4"
              :class="{
                'bg-green-100 dark:bg-green-900': movementType === 'receipt',
                'bg-red-100 dark:bg-red-900': movementType === 'issue',
                'bg-blue-100 dark:bg-blue-900': movementType === 'transfer',
                'bg-yellow-100 dark:bg-yellow-900': movementType === 'adjustment'
              }"
            >
              <component 
                :is="getIcon()" 
                class="w-6 h-6"
                :class="{
                  'text-green-600 dark:text-green-400': movementType === 'receipt',
                  'text-red-600 dark:text-red-400': movementType === 'issue',
                  'text-blue-600 dark:text-blue-400': movementType === 'transfer',
                  'text-yellow-600 dark:text-yellow-400': movementType === 'adjustment'
                }"
              />
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ getTitle() }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ getDescription() }}
              </p>
            </div>
          </div>
          <button
            @click="$emit('close')"
            class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
          >
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="p-6 overflow-y-auto max-h-[calc(90vh-120px)]">
        <div class="space-y-6">
          <!-- Reference Number -->
          <div>
            <label for="reference" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Reference Number
            </label>
            <input
              id="reference"
              v-model="formData.reference"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Auto-generated if left empty"
            />
          </div>

          <!-- Item Selection -->
          <div>
            <label for="item" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Item <span class="text-red-500">*</span>
            </label>
            <div class="space-y-2">
              <div class="relative">
                <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
                <input
                  id="item"
                  v-model="itemSearch"
                  type="text"
                  required
                  @input="searchItems"
                  @focus="showItemDropdown = true"
                  class="pl-10 w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Search by name, SKU, or scan barcode..."
                />
              </div>
              
              <!-- Item Dropdown -->
              <div 
                v-if="showItemDropdown && filteredItems.length > 0"
                class="absolute z-10 mt-1 w-full max-w-2xl bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600 rounded-lg shadow-lg max-h-60 overflow-y-auto"
              >
                <div
                  v-for="item in filteredItems"
                  :key="item.id"
                  @click="selectItem(item)"
                  class="px-4 py-3 hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer border-b border-gray-100 dark:border-gray-700 last:border-b-0"
                >
                  <div class="flex items-center justify-between">
                    <div>
                      <div class="font-medium text-gray-900 dark:text-white">{{ item.name }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">
                        SKU: {{ item.sku }} | Stock: {{ item.quantityOnHand || 0 }} {{ item.unit }}
                      </div>
                    </div>
                    <div class="text-right">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">R{{ formatCurrency(item.sellingPrice) }}</div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Selected Item Display -->
              <div v-if="selectedItem" class="p-4 bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg">
                <div class="flex items-center justify-between">
                  <div>
                    <div class="font-medium text-gray-900 dark:text-white">{{ selectedItem.name }}</div>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      SKU: {{ selectedItem.sku }} | Current Stock: {{ selectedItem.quantityOnHand || 0 }} {{ selectedItem.unit }}
                    </div>
                  </div>
                  <button
                    type="button"
                    @click="clearItem"
                    class="text-red-600 hover:text-red-700 dark:text-red-400"
                  >
                    <XMarkIcon class="w-5 h-5" />
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Warehouse Selection (Source) -->
          <div>
            <label for="warehouse" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              {{ movementType === 'transfer' ? 'Source Warehouse' : 'Warehouse' }} <span class="text-red-500">*</span>
            </label>
            <select
              id="warehouse"
              v-model="formData.warehouseId"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">Select warehouse</option>
              <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
                {{ warehouse.name }} ({{ warehouse.code }})
              </option>
            </select>
          </div>

          <!-- Target Warehouse (for transfers only) -->
          <div v-if="movementType === 'transfer'">
            <label for="targetWarehouse" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Target Warehouse <span class="text-red-500">*</span>
            </label>
            <select
              id="targetWarehouse"
              v-model="formData.targetWarehouseId"
              :required="movementType === 'transfer'"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">Select target warehouse</option>
              <option 
                v-for="warehouse in warehouses" 
                :key="warehouse.id" 
                :value="warehouse.id"
                :disabled="warehouse.id === formData.warehouseId"
              >
                {{ warehouse.name }} ({{ warehouse.code }})
              </option>
            </select>
          </div>

          <!-- Quantity and Rate -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Quantity -->
            <div>
              <label for="quantity" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Quantity <span class="text-red-500">*</span>
              </label>
              <div class="relative">
                <input
                  id="quantity"
                  v-model.number="formData.quantity"
                  type="number"
                  min="0.01"
                  step="0.01"
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="0.00"
                />
                <span v-if="selectedItem" class="absolute right-3 top-1/2 transform -translate-y-1/2 text-sm text-gray-500 dark:text-gray-400">
                  {{ selectedItem.unit }}
                </span>
              </div>
              
              <!-- Stock Level Warning -->
              <div v-if="showStockWarning" class="mt-2 p-2 bg-orange-50 dark:bg-orange-900/20 border border-orange-200 dark:border-orange-800 rounded text-sm text-orange-700 dark:text-orange-300">
                <ExclamationTriangleIcon class="w-4 h-4 inline mr-1" />
                {{ stockWarningMessage }}
              </div>
            </div>

            <!-- Rate/Price -->
            <div>
              <label for="rate" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Rate/Price (R)
              </label>
              <input
                id="rate"
                v-model.number="formData.rate"
                type="number"
                min="0"
                step="0.01"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="0.00"
              />
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                Optional - for valuation purposes
              </p>
            </div>
          </div>

          <!-- Total Amount Display -->
          <div v-if="totalAmount > 0" class="p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
            <div class="flex items-center justify-between">
              <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Total Amount:</span>
              <span class="text-lg font-bold text-gray-900 dark:text-white">R{{ formatCurrency(totalAmount) }}</span>
            </div>
          </div>

          <!-- Remarks -->
          <div>
            <label for="remarks" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Remarks
            </label>
            <textarea
              id="remarks"
              v-model="formData.remarks"
              rows="3"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Add any notes or comments..."
            ></textarea>
          </div>
        </div>

        <!-- Form Actions -->
        <div class="flex justify-end space-x-4 mt-8 pt-6 border-t border-gray-200 dark:border-gray-700">
          <button
            type="button"
            @click="$emit('close')"
            class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="!isFormValid"
            class="px-6 py-2 rounded-lg text-white transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            :class="{
              'bg-green-600 hover:bg-green-700': movementType === 'receipt',
              'bg-red-600 hover:bg-red-700': movementType === 'issue',
              'bg-blue-600 hover:bg-blue-700': movementType === 'transfer',
              'bg-yellow-600 hover:bg-yellow-700': movementType === 'adjustment'
            }"
          >
            {{ getSubmitButtonText() }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import {
  XMarkIcon,
  MagnifyingGlassIcon,
  ArrowDownIcon,
  ArrowUpIcon,
  ArrowRightIcon,
  AdjustmentsHorizontalIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline'
import type { ItemDto, WarehouseDto, StockEntryRequest } from '../../composables/useStock'

// Props
interface Props {
  movementType: 'receipt' | 'issue' | 'transfer' | 'adjustment'
  items?: ItemDto[]
  warehouses?: WarehouseDto[]
}

const props = withDefaults(defineProps<Props>(), {
  items: () => [],
  warehouses: () => []
})

// Emits
const emit = defineEmits<{
  close: []
  save: [data: StockEntryRequest]
}>()

// Reactive data
const formData = ref<StockEntryRequest>({
  itemId: '',
  warehouseId: '',
  movementType: 'IN',
  quantity: 0,
  rate: undefined,
  targetWarehouseId: undefined,
  reference: '',
  remarks: ''
})

const itemSearch = ref('')
const showItemDropdown = ref(false)
const selectedItem = ref<ItemDto | null>(null)
const filteredItems = ref<ItemDto[]>([])

// Computed
const isFormValid = computed(() => {
  const hasItem = !!formData.value.itemId
  const hasWarehouse = !!formData.value.warehouseId
  const hasQuantity = formData.value.quantity > 0
  const hasTargetWarehouse = props.movementType !== 'transfer' || !!formData.value.targetWarehouseId
  
  return hasItem && hasWarehouse && hasQuantity && hasTargetWarehouse
})

const totalAmount = computed(() => {
  if (formData.value.rate && formData.value.quantity) {
    return formData.value.rate * formData.value.quantity
  }
  return 0
})

const showStockWarning = computed(() => {
  if (!selectedItem.value || props.movementType !== 'issue') return false
  
  const currentStock = selectedItem.value.quantityOnHand || 0
  return formData.value.quantity > currentStock
})

const stockWarningMessage = computed(() => {
  if (!selectedItem.value) return ''
  
  const currentStock = selectedItem.value.quantityOnHand || 0
  if (formData.value.quantity > currentStock) {
    return `Warning: Only ${currentStock} ${selectedItem.value.unit} available in stock`
  }
  return ''
})

// Watch for movement type changes
watch(() => props.movementType, (newType) => {
  // Map UI movement type to API movement type
  switch (newType) {
    case 'receipt':
      formData.value.movementType = 'IN'
      break
    case 'issue':
      formData.value.movementType = 'OUT'
      break
    case 'transfer':
      formData.value.movementType = 'TRANSFER'
      break
    case 'adjustment':
      formData.value.movementType = 'IN' // Adjustments can be IN or OUT
      break
  }
}, { immediate: true })

// Methods
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const searchItems = () => {
  const query = itemSearch.value.toLowerCase()
  
  if (!query) {
    filteredItems.value = props.items.slice(0, 10) // Show first 10 items
    return
  }
  
  filteredItems.value = props.items.filter(item =>
    item.name.toLowerCase().includes(query) ||
    item.sku.toLowerCase().includes(query) ||
    (item.barcode && item.barcode.toLowerCase().includes(query))
  ).slice(0, 10)
}

const selectItem = (item: ItemDto) => {
  selectedItem.value = item
  formData.value.itemId = item.id
  itemSearch.value = item.name
  showItemDropdown.value = false
  
  // Auto-fill rate if available
  if (!formData.value.rate && item.costPrice) {
    formData.value.rate = item.costPrice
  }
}

const clearItem = () => {
  selectedItem.value = null
  formData.value.itemId = ''
  itemSearch.value = ''
  filteredItems.value = []
}

const getIcon = () => {
  switch (props.movementType) {
    case 'receipt': return ArrowDownIcon
    case 'issue': return ArrowUpIcon
    case 'transfer': return ArrowRightIcon
    case 'adjustment': return AdjustmentsHorizontalIcon
    default: return ArrowDownIcon
  }
}

const getTitle = () => {
  switch (props.movementType) {
    case 'receipt': return 'Stock Receipt'
    case 'issue': return 'Stock Issue'
    case 'transfer': return 'Stock Transfer'
    case 'adjustment': return 'Stock Adjustment'
    default: return 'Stock Movement'
  }
}

const getDescription = () => {
  switch (props.movementType) {
    case 'receipt': return 'Receive stock into warehouse'
    case 'issue': return 'Issue stock from warehouse'
    case 'transfer': return 'Transfer stock between warehouses'
    case 'adjustment': return 'Adjust stock levels'
    default: return 'Record stock movement'
  }
}

const getSubmitButtonText = () => {
  switch (props.movementType) {
    case 'receipt': return 'Receive Stock'
    case 'issue': return 'Issue Stock'
    case 'transfer': return 'Transfer Stock'
    case 'adjustment': return 'Adjust Stock'
    default: return 'Save Movement'
  }
}

const handleSubmit = () => {
  if (!isFormValid.value) return
  
  // Generate reference if not provided
  if (!formData.value.reference) {
    const prefix = {
      'receipt': 'RCP',
      'issue': 'ISS',
      'transfer': 'TRF',
      'adjustment': 'ADJ'
    }[props.movementType] || 'MOV'
    
    formData.value.reference = `${prefix}-${Date.now().toString().slice(-8)}`
  }
  
  emit('save', { ...formData.value })
}

// Click outside to close dropdown
const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement
  if (!target.closest('#item')) {
    showItemDropdown.value = false
  }
}

// Lifecycle
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  
  // Initialize filtered items
  searchItems()
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

