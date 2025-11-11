<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-6xl w-full max-h-[90vh] overflow-hidden">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <div class="flex items-center">
            <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-4">
              <ClipboardDocumentCheckIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
                {{ isEditing ? 'Stock Reconciliation' : 'New Stock Reconciliation' }}
              </h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">
                {{ isEditing ? reconciliation?.reference : 'Physical stock count verification' }}
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
      <form @submit.prevent="handleSubmit" class="overflow-y-auto max-h-[calc(90vh-120px)]">
        <!-- Basic Information -->
        <div class="p-6 border-b border-gray-200 dark:border-gray-700">
          <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Reconciliation Details</h4>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <!-- Warehouse -->
            <div>
              <label for="warehouse" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Warehouse <span class="text-red-500">*</span>
              </label>
              <select
                id="warehouse"
                v-model="formData.warehouseId"
                required
                :disabled="isEditing"
                @change="loadWarehouseItems"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white disabled:bg-gray-100 dark:disabled:bg-gray-600"
              >
                <option value="">Select warehouse</option>
                <option v-for="warehouse in warehouses" :key="warehouse.id" :value="warehouse.id">
                  {{ warehouse.name }} ({{ warehouse.code }})
                </option>
              </select>
            </div>

            <!-- Date -->
            <div>
              <label for="date" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Reconciliation Date <span class="text-red-500">*</span>
              </label>
              <input
                id="date"
                v-model="formData.date"
                type="date"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>

            <!-- Reference -->
            <div>
              <label for="reference" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Reference Number
              </label>
              <input
                id="reference"
                v-model="formData.reference"
                type="text"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Auto-generated if empty"
              />
            </div>
          </div>
        </div>

        <!-- Item Counts -->
        <div class="p-6">
          <div class="flex items-center justify-between mb-4">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white">Physical Count</h4>
            <div class="text-sm text-gray-600 dark:text-gray-400">
              <span class="font-medium">{{ itemsWithDiscrepancies }}</span> discrepancies found
            </div>
          </div>

          <!-- Loading State -->
          <div v-if="loadingItems" class="text-center py-8">
            <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto"></div>
            <p class="text-sm text-gray-600 dark:text-gray-400 mt-2">Loading items...</p>
          </div>

          <!-- Empty State -->
          <div v-else-if="formData.items.length === 0" class="text-center py-8">
            <CubeIcon class="w-12 h-12 text-gray-400 mx-auto mb-2" />
            <p class="text-sm text-gray-600 dark:text-gray-400">Select a warehouse to begin counting</p>
          </div>

          <!-- Items Table -->
          <div v-else class="border border-gray-200 dark:border-gray-700 rounded-lg overflow-hidden">
            <div class="overflow-x-auto max-h-96">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700 sticky top-0">
                  <tr>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">
                      Item
                    </th>
                    <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">
                      System Qty
                    </th>
                    <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">
                      Physical Count
                    </th>
                    <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">
                      Difference
                    </th>
                  </tr>
                </thead>
                <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:border-gray-700">
                  <tr 
                    v-for="(item, index) in formData.items" 
                    :key="item.id"
                    :class="{
                      'bg-red-50 dark:bg-red-900/10': item.physicalCount !== item.systemQuantity && item.physicalCount !== null,
                      'hover:bg-gray-50 dark:hover:bg-gray-700': true
                    }"
                  >
                    <td class="px-4 py-3">
                      <div>
                        <div class="text-sm font-medium text-gray-900 dark:text-white">{{ item.itemName }}</div>
                        <div class="text-xs text-gray-500 dark:text-gray-400">{{ item.itemSku }}</div>
                      </div>
                    </td>
                    <td class="px-4 py-3 text-center">
                      <span class="text-sm text-gray-900 dark:text-white">{{ item.systemQuantity }}</span>
                    </td>
                    <td class="px-4 py-3">
                      <input
                        v-model.number="item.physicalCount"
                        type="number"
                        min="0"
                        step="0.01"
                        class="w-24 px-2 py-1 text-center border border-gray-300 dark:border-gray-600 rounded focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                        placeholder="Count"
                      />
                    </td>
                    <td class="px-4 py-3 text-center">
                      <span 
                        v-if="item.physicalCount !== null"
                        class="inline-flex items-center px-2 py-1 rounded text-xs font-medium"
                        :class="{
                          'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200': item.physicalCount === item.systemQuantity,
                          'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200': item.physicalCount !== item.systemQuantity
                        }"
                      >
                        {{ item.physicalCount - item.systemQuantity > 0 ? '+' : '' }}{{ item.physicalCount - item.systemQuantity }}
                      </span>
                      <span v-else class="text-gray-400">-</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Remarks -->
        <div class="px-6 pb-6">
          <label for="remarks" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
            Remarks / Notes
          </label>
          <textarea
            id="remarks"
            v-model="formData.remarks"
            rows="3"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            placeholder="Add any notes about this reconciliation..."
          ></textarea>
        </div>

        <!-- Form Actions -->
        <div class="flex justify-between items-center px-6 py-4 border-t border-gray-200 dark:border-gray-700 bg-gray-50 dark:bg-gray-900">
          <div class="text-sm text-gray-600 dark:text-gray-400">
            {{ itemsCounted }} of {{ formData.items.length }} items counted
          </div>
          <div class="flex space-x-4">
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
              class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ isEditing ? 'Update Reconciliation' : 'Create Reconciliation' }}
            </button>
          </div>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import {
  XMarkIcon,
  ClipboardDocumentCheckIcon,
  CubeIcon
} from '@heroicons/vue/24/outline'
import type { WarehouseDto } from '../../composables/useStock'

// Props
interface Props {
  reconciliation?: any
  warehouses?: WarehouseDto[]
  items?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  reconciliation: null,
  warehouses: () => [],
  items: () => []
})

// Emits
const emit = defineEmits<{
  close: []
  save: [data: any]
}>()

// Reactive data
const loadingItems = ref(false)
const formData = ref({
  warehouseId: '',
  date: new Date().toISOString().split('T')[0],
  reference: '',
  items: [] as any[],
  remarks: ''
})

// Computed
const isEditing = computed(() => !!props.reconciliation)

const itemsCounted = computed(() => {
  return formData.value.items.filter(item => item.physicalCount !== null).length
})

const itemsWithDiscrepancies = computed(() => {
  return formData.value.items.filter(item => 
    item.physicalCount !== null && item.physicalCount !== item.systemQuantity
  ).length
})

const isFormValid = computed(() => {
  return formData.value.warehouseId && 
         formData.value.date && 
         itemsCounted.value > 0
})

// Watch for reconciliation changes
watch(() => props.reconciliation, (rec) => {
  if (rec) {
    formData.value = {
      warehouseId: rec.warehouseId,
      date: rec.date,
      reference: rec.reference || '',
      items: rec.items || [],
      remarks: rec.remarks || ''
    }
  }
}, { immediate: true })

// Methods
const loadWarehouseItems = async () => {
  if (!formData.value.warehouseId) return
  
  loadingItems.value = true
  
  try {
    // In a real app, fetch items for the selected warehouse
    await new Promise(resolve => setTimeout(resolve, 500))
    
    // Mock items from props
    formData.value.items = props.items.map(item => ({
      id: item.id,
      itemId: item.id,
      itemName: item.name,
      itemSku: item.sku,
      systemQuantity: item.quantityOnHand || 0,
      physicalCount: null
    }))
  } catch (error) {
    console.error('Error loading items:', error)
  } finally {
    loadingItems.value = false
  }
}

const handleSubmit = () => {
  if (!isFormValid.value) return
  
  // Generate reference if not provided
  if (!formData.value.reference) {
    formData.value.reference = `REC-${Date.now().toString().slice(-8)}`
  }
  
  emit('save', { ...formData.value })
}
</script>

