<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-2xl w-full max-h-[90vh] overflow-hidden">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ isEditing ? 'Edit Warehouse' : 'Create New Warehouse' }}
          </h3>
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
          <!-- Basic Information -->
          <div>
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Basic Information</h4>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <!-- Code -->
              <div>
                <label for="code" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Warehouse Code <span class="text-red-500">*</span>
                </label>
                <input
                  id="code"
                  v-model="formData.code"
                  type="text"
                  required
                  :disabled="isEditing"
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white disabled:bg-gray-100 dark:disabled:bg-gray-600"
                  placeholder="Enter warehouse code"
                />
                <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                  Unique identifier (cannot be changed after creation)
                </p>
              </div>

              <!-- Name -->
              <div>
                <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Warehouse Name <span class="text-red-500">*</span>
                </label>
                <input
                  id="name"
                  v-model="formData.name"
                  type="text"
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Enter warehouse name"
                />
              </div>
            </div>
          </div>

          <!-- Description -->
          <div>
            <label for="description" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Description
            </label>
            <textarea
              id="description"
              v-model="formData.description"
              rows="3"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Enter warehouse description"
            ></textarea>
          </div>

          <!-- Type and Parent Warehouse -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <!-- Type -->
            <div>
              <label for="type" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Warehouse Type <span class="text-red-500">*</span>
              </label>
              <select
                id="type"
                v-model="formData.type"
                required
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              >
                <option value="">Select type</option>
                <option value="main">Main Warehouse</option>
                <option value="branch">Branch Warehouse</option>
                <option value="transit">Transit Warehouse</option>
                <option value="store">Store/Retail</option>
                <option value="cold">Cold Storage</option>
                <option value="shared">Shared Warehouse</option>
              </select>
            </div>

            <!-- Parent Warehouse -->
            <div>
              <label for="parentWarehouse" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                Parent Warehouse
              </label>
              <select
                id="parentWarehouse"
                v-model="formData.parentWarehouseId"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              >
                <option value="">None (Top Level)</option>
                <option 
                  v-for="wh in availableParentWarehouses" 
                  :key="wh.id" 
                  :value="wh.id"
                >
                  {{ wh.name }} ({{ wh.code }})
                </option>
              </select>
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
                For hierarchical warehouse structure
              </p>
            </div>
          </div>

          <!-- Address -->
          <div>
            <label for="address" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Address
            </label>
            <textarea
              id="address"
              v-model="formData.address"
              rows="2"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Enter physical address"
            ></textarea>
          </div>

          <!-- Settings -->
          <div>
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Settings</h4>
            <div class="space-y-3">
              <!-- Is Group -->
              <label class="flex items-center">
                <input
                  v-model="formData.isGroup"
                  type="checkbox"
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500 h-4 w-4"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">
                  This is a warehouse group (can contain other warehouses)
                </span>
              </label>

              <!-- Is Active -->
              <label class="flex items-center">
                <input
                  v-model="formData.isActive"
                  type="checkbox"
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500 h-4 w-4"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">
                  Active Warehouse
                </span>
              </label>
            </div>
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
            class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            {{ isEditing ? 'Update Warehouse' : 'Create Warehouse' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'
import type { WarehouseDto } from '../../composables/useStock'

// Props
interface Props {
  warehouse?: WarehouseDto | null
  warehouses?: WarehouseDto[]
}

const props = withDefaults(defineProps<Props>(), {
  warehouse: null,
  warehouses: () => []
})

// Emits
const emit = defineEmits<{
  close: []
  save: [data: Partial<WarehouseDto>]
}>()

// Computed
const isEditing = computed(() => !!props.warehouse)

const availableParentWarehouses = computed(() => {
  if (!props.warehouses) return []
  
  // Exclude current warehouse and its children
  return props.warehouses.filter(wh => {
    if (!isEditing.value) return true
    return wh.id !== props.warehouse?.id
  })
})

// Form data
const formData = ref({
  code: '',
  name: '',
  description: '',
  type: '',
  parentWarehouseId: '',
  address: '',
  isGroup: false,
  isActive: true
})

// Watch for warehouse changes
watch(() => props.warehouse, (warehouse) => {
  if (warehouse) {
    formData.value = {
      code: warehouse.code,
      name: warehouse.name,
      description: warehouse.description || '',
      type: warehouse.type || '',
      parentWarehouseId: warehouse.parentWarehouseId || '',
      address: warehouse.address || '',
      isGroup: warehouse.isGroup,
      isActive: warehouse.isActive
    }
  } else {
    // Reset form for new warehouse
    formData.value = {
      code: '',
      name: '',
      description: '',
      type: '',
      parentWarehouseId: '',
      address: '',
      isGroup: false,
      isActive: true
    }
  }
}, { immediate: true })

// Methods
const handleSubmit = () => {
  const submitData: Partial<WarehouseDto> = {
    ...formData.value,
    parentWarehouseId: formData.value.parentWarehouseId || undefined
  }
  
  if (isEditing.value && props.warehouse) {
    submitData.id = props.warehouse.id
  }
  
  emit('save', submitData)
}
</script>

