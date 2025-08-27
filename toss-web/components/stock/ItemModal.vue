<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-gray-800 rounded-xl shadow-xl max-w-2xl w-full max-h-[90vh] overflow-hidden">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ isEditing ? 'Edit Item' : 'Create New Item' }}
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
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Basic Information -->
          <div class="md:col-span-2">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Basic Information</h4>
          </div>

          <!-- SKU -->
          <div>
            <label for="sku" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              SKU <span class="text-red-500">*</span>
            </label>
            <input
              id="sku"
              v-model="formData.sku"
              type="text"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Enter SKU"
            />
          </div>

          <!-- Barcode -->
          <div>
            <label for="barcode" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Barcode
            </label>
            <input
              id="barcode"
              v-model="formData.barcode"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Enter barcode"
            />
          </div>

          <!-- Name -->
          <div class="md:col-span-2">
            <label for="name" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Item Name <span class="text-red-500">*</span>
            </label>
            <input
              id="name"
              v-model="formData.name"
              type="text"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Enter item name"
            />
          </div>

          <!-- Description -->
          <div class="md:col-span-2">
            <label for="description" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Description
            </label>
            <textarea
              id="description"
              v-model="formData.description"
              rows="3"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="Enter item description"
            ></textarea>
          </div>

          <!-- Category -->
          <div>
            <label for="category" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Category <span class="text-red-500">*</span>
            </label>
            <select
              id="category"
              v-model="formData.category"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">Select category</option>
              <option v-for="category in categories" :key="category" :value="category">
                {{ category }}
              </option>
              <option value="__new__">+ Add New Category</option>
            </select>
            <input
              v-if="formData.category === '__new__'"
              v-model="newCategory"
              type="text"
              placeholder="Enter new category"
              class="w-full mt-2 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              @blur="addNewCategory"
            />
          </div>

          <!-- Unit -->
          <div>
            <label for="unit" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Unit of Measure <span class="text-red-500">*</span>
            </label>
            <select
              id="unit"
              v-model="formData.unit"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
            >
              <option value="">Select unit</option>
              <option value="PCS">Pieces (PCS)</option>
              <option value="KG">Kilograms (KG)</option>
              <option value="LTR">Liters (LTR)</option>
              <option value="MTR">Meters (MTR)</option>
              <option value="BOX">Box</option>
              <option value="PACK">Pack</option>
              <option value="BOTTLE">Bottle</option>
              <option value="CARTON">Carton</option>
              <option value="DOZEN">Dozen</option>
              <option value="PAIR">Pair</option>
            </select>
          </div>

          <!-- Pricing -->
          <div class="md:col-span-2">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Pricing</h4>
          </div>

          <!-- Selling Price -->
          <div>
            <label for="sellingPrice" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Selling Price (R) <span class="text-red-500">*</span>
            </label>
            <input
              id="sellingPrice"
              v-model.number="formData.sellingPrice"
              type="number"
              min="0"
              step="0.01"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="0.00"
            />
          </div>

          <!-- Cost Price -->
          <div>
            <label for="costPrice" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Cost Price (R)
            </label>
            <input
              id="costPrice"
              v-model.number="formData.costPrice"
              type="number"
              min="0"
              step="0.01"
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="0.00"
            />
          </div>

          <!-- Stock Management -->
          <div class="md:col-span-2">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-4">Stock Management</h4>
          </div>

          <!-- Reorder Level -->
          <div>
            <label for="reorderLevel" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Reorder Level <span class="text-red-500">*</span>
            </label>
            <input
              id="reorderLevel"
              v-model.number="formData.reorderLevel"
              type="number"
              min="0"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="0"
            />
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
              Minimum stock level before reorder alert
            </p>
          </div>

          <!-- Reorder Quantity -->
          <div>
            <label for="reorderQty" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Reorder Quantity <span class="text-red-500">*</span>
            </label>
            <input
              id="reorderQty"
              v-model.number="formData.reorderQty"
              type="number"
              min="1"
              required
              class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              placeholder="0"
            />
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
              Suggested quantity to reorder
            </p>
          </div>

          <!-- Status -->
          <div class="md:col-span-2">
            <label class="flex items-center">
              <input
                v-model="formData.isActive"
                type="checkbox"
                class="rounded border-gray-300 text-blue-600 focus:ring-blue-500 h-4 w-4"
              />
              <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Active Item</span>
            </label>
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
            {{ isEditing ? 'Update Item' : 'Create Item' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'
import type { ItemDto, CreateItemRequest, UpdateItemRequest } from '../../composables/useStock'

// Props
interface Props {
  item?: ItemDto | null
  categories: string[]
}

const props = withDefaults(defineProps<Props>(), {
  item: null
})

// Emits
const emit = defineEmits<{
  close: []
  save: [data: CreateItemRequest | UpdateItemRequest]
}>()

// Computed
const isEditing = computed(() => !!props.item)

// Form data
const formData = ref<CreateItemRequest & { id?: string }>({
  sku: '',
  barcode: '',
  name: '',
  description: '',
  category: '',
  unit: '',
  sellingPrice: 0,
  costPrice: 0,
  reorderLevel: 0,
  reorderQty: 1,
  isActive: true
})

const newCategory = ref('')

// Watch for item changes
watch(() => props.item, (item) => {
  if (item) {
    formData.value = {
      id: item.id,
      sku: item.sku,
      barcode: item.barcode || '',
      name: item.name,
      description: item.description || '',
      category: item.category,
      unit: item.unit,
      sellingPrice: item.sellingPrice,
      costPrice: item.costPrice || 0,
      reorderLevel: item.reorderLevel,
      reorderQty: item.reorderQty,
      isActive: item.isActive
    }
  } else {
    // Reset form for new item
    formData.value = {
      sku: '',
      barcode: '',
      name: '',
      description: '',
      category: '',
      unit: '',
      sellingPrice: 0,
      costPrice: 0,
      reorderLevel: 0,
      reorderQty: 1,
      isActive: true
    }
  }
}, { immediate: true })

// Methods
const addNewCategory = () => {
  if (newCategory.value.trim()) {
    formData.value.category = newCategory.value.trim()
    newCategory.value = ''
  }
}

const handleSubmit = () => {
  // If adding new category, use the newCategory value
  if (formData.value.category === '__new__' && newCategory.value.trim()) {
    formData.value.category = newCategory.value.trim()
  }

  // Remove id if creating new item
  const submitData = { ...formData.value }
  if (!isEditing.value) {
    delete submitData.id
  }

  emit('save', submitData as CreateItemRequest | UpdateItemRequest)
}
</script>
