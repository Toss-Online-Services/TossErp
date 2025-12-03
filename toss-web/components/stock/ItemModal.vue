<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'

interface Props {
  show: boolean
  item?: Item | null
}

const props = withDefaults(defineProps<Props>(), {
  item: null
})

const emit = defineEmits<{
  close: []
  saved: [item: Item]
}>()

const stockStore = useStockStore()

// Form state
const formData = ref({
  code: '',
  name: '',
  description: '',
  category: '',
  unit: 'unit',
  costPrice: 0,
  sellingPrice: 0,
  currentStock: 0,
  minStock: 0,
  maxStock: 0,
  warehouse: 'main',
  barcode: '',
  supplier: '',
  isActive: true
})

const isEditing = computed(() => !!props.item)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})

// Common categories
const categories = ['Groceries', 'Building Materials', 'Electronics', 'Clothing', 'Hardware', 'Other']
const units = ['unit', 'pack', 'box', 'bag', 'bottle', 'kg', 'g', 'l', 'ml', 'm', 'cm']

// Watch for item changes
watch(() => props.item, (newItem) => {
  if (newItem) {
    formData.value = {
      code: newItem.code,
      name: newItem.name,
      description: newItem.description || '',
      category: newItem.category,
      unit: newItem.unit,
      costPrice: newItem.costPrice,
      sellingPrice: newItem.sellingPrice,
      currentStock: newItem.currentStock,
      minStock: newItem.minStock,
      maxStock: newItem.maxStock || 0,
      warehouse: newItem.warehouse,
      barcode: newItem.barcode || '',
      supplier: newItem.supplier || '',
      isActive: newItem.isActive
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  formData.value = {
    code: '',
    name: '',
    description: '',
    category: '',
    unit: 'unit',
    costPrice: 0,
    sellingPrice: 0,
    currentStock: 0,
    minStock: 0,
    maxStock: 0,
    warehouse: 'main',
    barcode: '',
    supplier: '',
    isActive: true
  }
  errors.value = {}
}

function validate() {
  errors.value = {}
  
  if (!formData.value.code.trim()) {
    errors.value.code = 'Item code is required'
  }
  
  if (!formData.value.name.trim()) {
    errors.value.name = 'Item name is required'
  }
  
  if (!formData.value.category) {
    errors.value.category = 'Category is required'
  }
  
  if (formData.value.costPrice < 0) {
    errors.value.costPrice = 'Cost price cannot be negative'
  }
  
  if (formData.value.sellingPrice < 0) {
    errors.value.sellingPrice = 'Selling price cannot be negative'
  }
  
  if (formData.value.sellingPrice < formData.value.costPrice) {
    errors.value.sellingPrice = 'Selling price should be higher than cost price'
  }
  
  if (formData.value.minStock < 0) {
    errors.value.minStock = 'Minimum stock cannot be negative'
  }
  
  if (formData.value.maxStock > 0 && formData.value.maxStock < formData.value.minStock) {
    errors.value.maxStock = 'Maximum stock should be higher than minimum stock'
  }
  
  return Object.keys(errors.value).length === 0
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    if (isEditing.value && props.item) {
      await stockStore.updateItem(props.item.id, formData.value)
      emit('saved', { ...props.item, ...formData.value } as Item)
    } else {
      const newItem = await stockStore.createItem(formData.value)
      emit('saved', newItem)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save item:', error)
    alert('Failed to save item. Please try again.')
  } finally {
    isSubmitting.value = false
  }
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
        v-if="show"
        class="fixed inset-0 z-50 overflow-y-auto"
        @click.self="handleClose"
      >
        <div class="flex min-h-screen items-center justify-center p-4">
          <div
            class="relative w-full max-w-2xl rounded-xl bg-white shadow-xl"
            @click.stop
          >
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-xl font-bold text-gray-900">
                {{ isEditing ? 'Edit Item' : 'Add New Item' }}
              </h3>
              <button
                @click="handleClose"
                class="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <i class="material-symbols-rounded text-2xl">close</i>
              </button>
            </div>

            <!-- Form -->
            <form @submit.prevent="handleSave" class="p-6 space-y-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Item Code -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Item Code <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="formData.code"
                    type="text"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.code }"
                    placeholder="e.g., CEM-001"
                  >
                  <p v-if="errors.code" class="mt-1 text-sm text-red-600">{{ errors.code }}</p>
                </div>

                <!-- Item Name -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Item Name <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="formData.name"
                    type="text"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.name }"
                    placeholder="e.g., Cement 50kg"
                  >
                  <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                </div>

                <!-- Category -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Category <span class="text-red-500">*</span>
                  </label>
                  <select
                    v-model="formData.category"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.category }"
                  >
                    <option value="">Select category</option>
                    <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                  </select>
                  <p v-if="errors.category" class="mt-1 text-sm text-red-600">{{ errors.category }}</p>
                </div>

                <!-- Unit -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Unit of Measure
                  </label>
                  <select
                    v-model="formData.unit"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                    <option v-for="u in units" :key="u" :value="u">{{ u }}</option>
                  </select>
                </div>

                <!-- Cost Price -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Cost Price (ZAR) <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model.number="formData.costPrice"
                    type="number"
                    step="0.01"
                    min="0"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.costPrice }"
                    placeholder="0.00"
                  >
                  <p v-if="errors.costPrice" class="mt-1 text-sm text-red-600">{{ errors.costPrice }}</p>
                </div>

                <!-- Selling Price -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Selling Price (ZAR) <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model.number="formData.sellingPrice"
                    type="number"
                    step="0.01"
                    min="0"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.sellingPrice }"
                    placeholder="0.00"
                  >
                  <p v-if="errors.sellingPrice" class="mt-1 text-sm text-red-600">{{ errors.sellingPrice }}</p>
                </div>

                <!-- Current Stock -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Current Stock
                  </label>
                  <input
                    v-model.number="formData.currentStock"
                    type="number"
                    min="0"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="0"
                  >
                </div>

                <!-- Min Stock -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Minimum Stock Level
                  </label>
                  <input
                    v-model.number="formData.minStock"
                    type="number"
                    min="0"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.minStock }"
                    placeholder="0"
                  >
                  <p v-if="errors.minStock" class="mt-1 text-sm text-red-600">{{ errors.minStock }}</p>
                </div>

                <!-- Max Stock -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Maximum Stock Level
                  </label>
                  <input
                    v-model.number="formData.maxStock"
                    type="number"
                    min="0"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.maxStock }"
                    placeholder="0 (optional)"
                  >
                  <p v-if="errors.maxStock" class="mt-1 text-sm text-red-600">{{ errors.maxStock }}</p>
                </div>

                <!-- Barcode -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Barcode
                  </label>
                  <input
                    v-model="formData.barcode"
                    type="text"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="Optional"
                  >
                </div>

                <!-- Supplier -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Default Supplier
                  </label>
                  <input
                    v-model="formData.supplier"
                    type="text"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    placeholder="Optional"
                  >
                </div>
              </div>

              <!-- Description -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Description
                </label>
                <textarea
                  v-model="formData.description"
                  rows="3"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  placeholder="Item description (optional)"
                ></textarea>
              </div>

              <!-- Active Status -->
              <div class="flex items-center gap-2">
                <input
                  v-model="formData.isActive"
                  type="checkbox"
                  id="isActive"
                  class="w-4 h-4 text-gray-900 border-gray-300 rounded focus:ring-gray-900"
                >
                <label for="isActive" class="text-sm font-medium text-gray-700">
                  Item is active
                </label>
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
                  type="submit"
                  :disabled="isSubmitting"
                  class="px-4 py-2 text-white bg-gradient-to-br from-gray-800 to-gray-900 rounded-lg hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <span v-if="isSubmitting">Saving...</span>
                  <span v-else>{{ isEditing ? 'Update' : 'Create' }} Item</span>
                </button>
              </div>
            </form>
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

