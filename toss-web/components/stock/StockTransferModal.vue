<script setup lang="ts">
import { ref, computed } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'

interface Props {
  show: boolean
  item?: Item | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  transferred: []
}>()

const stockStore = useStockStore()

const fromWarehouse = ref('main')
const toWarehouse = ref('')
const quantity = ref(0)
const notes = ref('')
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

const warehouses = computed(() => {
  return stockStore.warehouses.length > 0
    ? stockStore.warehouses
    : [{ id: 'main', name: 'Main Warehouse', code: 'MAIN' }]
})

const availableStock = computed(() => {
  if (!props.item) return 0
  // In a real app, this would check stock per warehouse
  return props.item.currentStock
})

function validate() {
  errors.value = {}
  
  if (!fromWarehouse.value) {
    errors.value.fromWarehouse = 'Source warehouse is required'
  }
  
  if (!toWarehouse.value) {
    errors.value.toWarehouse = 'Destination warehouse is required'
  }
  
  if (fromWarehouse.value === toWarehouse.value) {
    errors.value.toWarehouse = 'Source and destination must be different'
  }
  
  if (quantity.value <= 0) {
    errors.value.quantity = 'Quantity must be greater than 0'
  }
  
  if (quantity.value > availableStock.value) {
    errors.value.quantity = `Cannot transfer more than available stock (${availableStock.value})`
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleTransfer() {
  if (!props.item || !validate()) return
  
  isSubmitting.value = true
  try {
    // TODO: Implement actual transfer API call
    // For now, we'll create an adjustment entry
    const transferNotes = `Transfer from ${fromWarehouse.value} to ${toWarehouse.value}${notes.value ? `: ${notes.value}` : ''}`
    
    // Decrease from source warehouse
    await stockStore.adjustStock(props.item.id, -quantity.value, transferNotes)
    
    // In a real implementation, we'd also increase in destination warehouse
    // This would be handled by the backend API
    
    emit('transferred')
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to transfer stock:', error)
    alert('Failed to transfer stock. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

function resetForm() {
  fromWarehouse.value = 'main'
  toWarehouse.value = ''
  quantity.value = 0
  notes.value = ''
  errors.value = {}
}

function handleClose() {
  emit('close')
  resetForm()
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show && item"
        class="fixed inset-0 z-50 overflow-y-auto"
        @click.self="handleClose"
      >
        <div class="flex min-h-screen items-center justify-center p-4">
          <div
            class="relative w-full max-w-md rounded-xl bg-white shadow-xl"
            @click.stop
          >
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-xl font-bold text-gray-900">Transfer Stock</h3>
              <button
                @click="handleClose"
                class="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <i class="material-symbols-rounded text-2xl">close</i>
              </button>
            </div>

            <!-- Content -->
            <div class="p-6 space-y-6">
              <!-- Item Info -->
              <div class="bg-gray-50 rounded-lg p-4">
                <div class="text-sm text-gray-600 mb-1">Item</div>
                <div class="text-lg font-semibold text-gray-900">{{ item.name }}</div>
                <div class="text-sm text-gray-500 mt-1">Code: {{ item.code }}</div>
                <div class="mt-2">
                  <div class="text-xs text-gray-600">Available Stock</div>
                  <div class="text-lg font-bold text-gray-900">{{ availableStock }} {{ item.unit }}</div>
                </div>
              </div>

              <!-- From Warehouse -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  From Warehouse <span class="text-red-500">*</span>
                </label>
                <select
                  v-model="fromWarehouse"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  :class="{ 'border-red-500': errors.fromWarehouse }"
                >
                  <option value="">Select warehouse</option>
                  <option v-for="wh in warehouses" :key="wh.id" :value="wh.id">
                    {{ wh.name }}
                  </option>
                </select>
                <p v-if="errors.fromWarehouse" class="mt-1 text-sm text-red-600">{{ errors.fromWarehouse }}</p>
              </div>

              <!-- To Warehouse -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  To Warehouse <span class="text-red-500">*</span>
                </label>
                <select
                  v-model="toWarehouse"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  :class="{ 'border-red-500': errors.toWarehouse }"
                >
                  <option value="">Select warehouse</option>
                  <option v-for="wh in warehouses" :key="wh.id" :value="wh.id">
                    {{ wh.name }}
                  </option>
                </select>
                <p v-if="errors.toWarehouse" class="mt-1 text-sm text-red-600">{{ errors.toWarehouse }}</p>
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
                  :max="availableStock"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  :class="{ 'border-red-500': errors.quantity }"
                  placeholder="0"
                >
                <p v-if="errors.quantity" class="mt-1 text-sm text-red-600">{{ errors.quantity }}</p>
              </div>

              <!-- Notes -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Notes
                </label>
                <textarea
                  v-model="notes"
                  rows="3"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  placeholder="Optional notes about this transfer"
                ></textarea>
              </div>

              <!-- Actions -->
              <div class="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  type="button"
                  @click="handleClose"
                  class="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="button"
                  @click="handleTransfer"
                  :disabled="isSubmitting"
                  class="px-4 py-2 text-white bg-gradient-to-br from-gray-800 to-gray-900 rounded-lg hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <span v-if="isSubmitting">Transferring...</span>
                  <span v-else>Transfer Stock</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active > div > div,
.modal-leave-active > div > div {
  transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from > div > div,
.modal-leave-to > div > div {
  transform: scale(0.95);
  opacity: 0;
}
</style>

