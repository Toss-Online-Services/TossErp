<script setup lang="ts">
import { ref, computed } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'

interface Props {
  item: Item | null
  compact?: boolean
  showItemInfo?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  compact: false,
  showItemInfo: true
})

const emit = defineEmits<{
  adjusted: []
  cancelled: []
}>()

const stockStore = useStockStore()

const adjustmentType = ref<'increase' | 'decrease'>('increase')
const quantity = ref(0)
const reason = ref('')
const notes = ref('')
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

const adjustmentReasons = [
  'Stock Count Correction',
  'Damaged Goods',
  'Expired Items',
  'Theft/Loss',
  'Found Stock',
  'Opening Balance',
  'Other'
]

const computedNewStock = computed(() => {
  if (!props.item) return 0
  if (adjustmentType.value === 'increase') {
    return props.item.currentStock + quantity.value
  } else {
    return Math.max(0, props.item.currentStock - quantity.value)
  }
})

function validate() {
  errors.value = {}
  
  if (quantity.value <= 0) {
    errors.value.quantity = 'Quantity must be greater than 0'
  }
  
  if (adjustmentType.value === 'decrease' && quantity.value > (props.item?.currentStock || 0)) {
    errors.value.quantity = 'Cannot decrease more than current stock'
  }
  
  if (!reason.value) {
    errors.value.reason = 'Reason is required'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleAdjust() {
  if (!props.item || !validate()) return
  
  isSubmitting.value = true
  try {
    const adjustmentQuantity = adjustmentType.value === 'increase' 
      ? quantity.value 
      : -quantity.value
    
    const adjustmentNotes = `${reason.value}${notes.value ? `: ${notes.value}` : ''}`
    
    await stockStore.adjustStock(props.item.id, adjustmentQuantity, adjustmentNotes)
    
    emit('adjusted')
    resetForm()
  } catch (error) {
    console.error('Failed to adjust stock:', error)
    alert('Failed to adjust stock. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

function resetForm() {
  adjustmentType.value = 'increase'
  quantity.value = 0
  reason.value = ''
  notes.value = ''
  errors.value = {}
}

function handleCancel() {
  resetForm()
  emit('cancelled')
}

// Expose reset function for parent components
defineExpose({
  resetForm
})
</script>

<template>
  <div v-if="item" class="space-y-6">
    <!-- Item Info (optional, can be hidden in compact mode) -->
    <div v-if="showItemInfo" class="bg-gray-50 rounded-lg p-4">
      <div class="text-sm text-gray-600 mb-1">Item</div>
      <div class="text-lg font-semibold text-gray-900">{{ item.name }}</div>
      <div class="text-sm text-gray-500 mt-1">Code: {{ item.code }}</div>
      <div class="mt-2 flex items-center gap-4">
        <div>
          <div class="text-xs text-gray-600">Current Stock</div>
          <div class="text-lg font-bold text-gray-900">{{ item.currentStock }} {{ item.unit }}</div>
        </div>
      </div>
    </div>

    <!-- Adjustment Type -->
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-3">
        Adjustment Type
      </label>
      <div class="grid grid-cols-2 gap-3">
        <button
          type="button"
          @click="adjustmentType = 'increase'"
          :class="[
            'px-4 py-3 rounded-lg border-2 transition-all',
            adjustmentType === 'increase'
              ? 'border-green-500 bg-green-50 text-green-900'
              : 'border-gray-300 bg-white text-gray-700 hover:border-gray-400'
          ]"
        >
          <i class="material-symbols-rounded block mb-1">add</i>
          <span class="text-sm font-medium">Increase</span>
        </button>
        <button
          type="button"
          @click="adjustmentType = 'decrease'"
          :class="[
            'px-4 py-3 rounded-lg border-2 transition-all',
            adjustmentType === 'decrease'
              ? 'border-red-500 bg-red-50 text-red-900'
              : 'border-gray-300 bg-white text-gray-700 hover:border-gray-400'
          ]"
        >
          <i class="material-symbols-rounded block mb-1">remove</i>
          <span class="text-sm font-medium">Decrease</span>
        </button>
      </div>
    </div>

    <!-- Quantity -->
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Quantity <span class="text-red-500">*</span>
      </label>
      <input
        v-model.number="quantity"
        type="number"
        min="0"
        step="0.01"
        class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
        :class="{ 'border-red-500': errors.quantity }"
        placeholder="0"
      >
      <p v-if="errors.quantity" class="mt-1 text-sm text-red-600">{{ errors.quantity }}</p>
    </div>

    <!-- New Stock Preview -->
    <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
      <div class="text-sm text-blue-700 mb-1">New Stock Level</div>
      <div class="text-2xl font-bold text-blue-900">
        {{ computedNewStock }} {{ item.unit }}
      </div>
    </div>

    <!-- Reason -->
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Reason <span class="text-red-500">*</span>
      </label>
      <select
        v-model="reason"
        class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
        :class="{ 'border-red-500': errors.reason }"
      >
        <option value="">Select reason</option>
        <option v-for="r in adjustmentReasons" :key="r" :value="r">{{ r }}</option>
      </select>
      <p v-if="errors.reason" class="mt-1 text-sm text-red-600">{{ errors.reason }}</p>
    </div>

    <!-- Notes -->
    <div>
      <label class="block text-sm font-medium text-gray-700 mb-2">
        Additional Notes
      </label>
      <textarea
        v-model="notes"
        :rows="compact ? 2 : 3"
        class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
        placeholder="Optional notes about this adjustment"
      ></textarea>
    </div>

    <!-- Actions -->
    <div class="flex justify-end gap-3 pt-4 border-t border-gray-200">
      <button
        type="button"
        @click="handleCancel"
        class="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
      >
        Cancel
      </button>
      <button
        type="button"
        @click="handleAdjust"
        :disabled="isSubmitting"
        class="px-4 py-2 text-white bg-gradient-to-br from-gray-800 to-gray-900 rounded-lg hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <span v-if="isSubmitting">Adjusting...</span>
        <span v-else>Adjust Stock</span>
      </button>
    </div>
  </div>
  <div v-else class="text-center py-8 text-gray-500">
    <i class="material-symbols-rounded text-4xl mb-2">inventory_2</i>
    <p>No item selected</p>
  </div>
</template>

