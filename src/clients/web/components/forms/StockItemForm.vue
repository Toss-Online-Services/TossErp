<template>
  <div class="stock-item-form">
    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Form Header -->
      <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          {{ isEditing ? 'Edit Stock Item' : 'Add New Stock Item' }}
        </h3>
        <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">
          {{ isEditing ? 'Update the stock item information below.' : 'Fill in the details to add a new stock item.' }}
        </p>
      </div>

      <!-- Form Fields -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Item Name -->
        <div class="md:col-span-2">
          <label for="name" class="form-label">
            Item Name <span class="text-red-500">*</span>
          </label>
          <input
            id="name"
            v-model="formData.name"
            type="text"
            :class="[
              'form-input',
              getErrorClass('name', validationErrors)
            ]"
            placeholder="Enter item name"
            @blur="validateField('name')"
          />
          <div v-if="hasError('name', validationErrors)" class="error-message">
            {{ getFirstError('name', validationErrors) }}
          </div>
        </div>

        <!-- SKU -->
        <div>
          <label for="sku" class="form-label">
            SKU <span class="text-red-500">*</span>
          </label>
          <input
            id="sku"
            v-model="formData.sku"
            type="text"
            :class="[
              'form-input',
              getErrorClass('sku', validationErrors)
            ]"
            placeholder="Enter SKU"
            @blur="validateField('sku')"
          />
          <div v-if="hasError('sku', validationErrors)" class="error-message">
            {{ getFirstError('sku', validationErrors) }}
          </div>
        </div>

        <!-- Category -->
        <div>
          <label for="category" class="form-label">
            Category <span class="text-red-500">*</span>
          </label>
          <select
            id="category"
            v-model="formData.category"
            :class="[
              'form-input',
              getErrorClass('category', validationErrors)
            ]"
            @blur="validateField('category')"
          >
            <option value="">Select category</option>
            <option v-for="category in availableCategories" :key="category" :value="category">
              {{ category }}
            </option>
            <option value="new">+ Add New Category</option>
          </select>
          <div v-if="hasError('category', validationErrors)" class="error-message">
            {{ getFirstError('category', validationErrors) }}
          </div>
        </div>

        <!-- Warehouse -->
        <div>
          <label for="warehouse" class="form-label">
            Warehouse <span class="text-red-500">*</span>
          </label>
          <select
            id="warehouse"
            v-model="formData.warehouse"
            :class="[
              'form-input',
              getErrorClass('warehouse', validationErrors)
            ]"
            @blur="validateField('warehouse')"
          >
            <option value="">Select warehouse</option>
            <option v-for="warehouse in availableWarehouses" :key="warehouse" :value="warehouse">
              {{ warehouse }}
            </option>
            <option value="new">+ Add New Warehouse</option>
          </select>
          <div v-if="hasError('warehouse', validationErrors)" class="error-message">
            {{ getFirstError('warehouse', validationErrors) }}
          </div>
        </div>

        <!-- Quantity -->
        <div>
          <label for="quantity" class="form-label">
            Quantity <span class="text-red-500">*</span>
          </label>
          <input
            id="quantity"
            v-model.number="formData.quantity"
            type="number"
            min="0"
            :class="[
              'form-input',
              getErrorClass('quantity', validationErrors)
            ]"
            placeholder="0"
            @blur="validateField('quantity')"
          />
          <div v-if="hasError('quantity', validationErrors)" class="error-message">
            {{ getFirstError('quantity', validationErrors) }}
          </div>
        </div>

        <!-- Unit Cost -->
        <div>
          <label for="unitCost" class="form-label">
            Unit Cost <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <span class="absolute inset-y-0 left-0 pl-3 flex items-center text-gray-500 dark:text-gray-400">
              $
            </span>
            <input
              id="unitCost"
              v-model.number="formData.unitCost"
              type="number"
              step="0.01"
              min="0"
              :class="[
                'form-input pl-8',
                getErrorClass('unitCost', validationErrors)
              ]"
              placeholder="0.00"
              @blur="validateField('unitCost')"
            />
          </div>
          <div v-if="hasError('unitCost', validationErrors)" class="error-message">
            {{ getFirstError('unitCost', validationErrors) }}
          </div>
        </div>

        <!-- Reorder Level -->
        <div>
          <label for="reorderLevel" class="form-label">
            Reorder Level <span class="text-red-500">*</span>
          </label>
          <input
            id="reorderLevel"
            v-model.number="formData.reorderLevel"
            type="number"
            min="0"
            :class="[
              'form-input',
              getErrorClass('reorderLevel', validationErrors)
            ]"
            placeholder="0"
            @blur="validateField('reorderLevel')"
          />
          <div v-if="hasError('reorderLevel', validationErrors)" class="error-message">
            {{ getFirstError('reorderLevel', validationErrors) }}
          </div>
        </div>

        <!-- Active Status -->
        <div>
          <label class="form-label">Status</label>
          <div class="flex items-center space-x-4">
            <label class="flex items-center">
              <input
                v-model="formData.isActive"
                type="radio"
                :value="true"
                class="form-radio"
              />
              <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Active</span>
            </label>
            <label class="flex items-center">
              <input
                v-model="formData.isActive"
                type="radio"
                :value="false"
                class="form-radio"
              />
              <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Inactive</span>
            </label>
          </div>
        </div>
      </div>

      <!-- Description -->
      <div>
        <label for="description" class="form-label">Description</label>
        <textarea
          id="description"
          v-model="formData.description"
          rows="3"
          :class="[
            'form-input',
            getErrorClass('description', validationErrors)
          ]"
          placeholder="Enter item description"
          @blur="validateField('description')"
        ></textarea>
        <div v-if="hasError('description', validationErrors)" class="error-message">
          {{ getFirstError('description', validationErrors) }}
        </div>
        <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
          {{ formData.description?.length || 0 }}/500 characters
        </p>
      </div>

      <!-- Error Summary -->
      <div v-if="Object.keys(validationErrors).length > 0" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
        <div class="flex">
          <ExclamationTriangleIcon class="h-5 w-5 text-red-400 mt-0.5" />
          <div class="ml-3">
            <h3 class="text-sm font-medium text-red-800 dark:text-red-200">
              Please fix the following errors:
            </h3>
            <div class="mt-2 text-sm text-red-700 dark:text-red-300">
              <ul class="list-disc pl-5 space-y-1">
                <li v-for="(errors, field) in validationErrors" :key="field">
                  <span class="font-medium">{{ field }}:</span> {{ errors[0] }}
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>

      <!-- Form Actions -->
      <div class="flex justify-end space-x-3 pt-4 border-t border-gray-200 dark:border-gray-700">
        <button
          type="button"
          @click="handleCancel"
          class="btn-secondary"
          :disabled="isSubmitting"
        >
          Cancel
        </button>
        <button
          type="submit"
          :disabled="isSubmitting || !isFormValid"
          class="btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <div v-if="isSubmitting" class="w-4 h-4 mr-2 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
          {{ isSubmitting ? 'Saving...' : (isEditing ? 'Update Item' : 'Add Item') }}
        </button>
      </div>
    </form>

    <!-- New Category Modal -->
    <div v-if="showNewCategoryModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Add New Category</h3>
          <input
            v-model="newCategory"
            type="text"
            class="form-input w-full"
            placeholder="Enter category name"
            @keyup.enter="addNewCategory"
          />
          <div class="flex justify-end space-x-3 mt-4">
            <button
              @click="showNewCategoryModal = false"
              class="btn-secondary"
            >
              Cancel
            </button>
            <button
              @click="addNewCategory"
              class="btn-primary"
              :disabled="!newCategory.trim()"
            >
              Add Category
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- New Warehouse Modal -->
    <div v-if="showNewWarehouseModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Add New Warehouse</h3>
          <input
            v-model="newWarehouse"
            type="text"
            class="form-input w-full"
            placeholder="Enter warehouse name"
            @keyup.enter="addNewWarehouse"
          />
          <div class="flex justify-end space-x-3 mt-4">
            <button
              @click="showNewWarehouseModal = false"
              class="btn-secondary"
            >
              Cancel
            </button>
            <button
              @click="addNewWarehouse"
              class="btn-primary"
              :disabled="!newWarehouse.trim()"
            >
              Add Warehouse
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import { useStockStore } from '../../stores/stock'
import type { StockItem } from '../../stores/stock'
import {
  validateForm,
  stockItemValidations,
  getFirstError,
  hasError,
  getErrorClass
} from '../../utils/validation'
import type { ValidationResult } from '../../utils/validation'
import { showErrorToast, showSuccessToast } from '../../utils/errorHandling'

interface Props {
  item?: StockItem
  onSave?: (item: StockItem) => void
  onCancel?: () => void
}

const props = withDefaults(defineProps<Props>(), {
  item: undefined,
  onSave: undefined,
  onCancel: undefined
})

const stockStore = useStockStore()

// Form state
const isSubmitting = ref(false)
const validationErrors = reactive<Record<string, string[]>>({})
const showNewCategoryModal = ref(false)
const showNewWarehouseModal = ref(false)
const newCategory = ref('')
const newWarehouse = ref('')

// Form data
const formData = reactive<Omit<StockItem, 'id' | 'lastUpdated'>>({
  name: '',
  description: '',
  sku: '',
  category: '',
  warehouse: '',
  quantity: 0,
  unitCost: 0,
  reorderLevel: 0,
  isActive: true
})

// Computed properties
const isEditing = computed(() => !!props.item)
const isFormValid = computed(() => Object.keys(validationErrors).length === 0)

const availableCategories = computed(() => {
  const categories = stockStore.categories
  return categories.length > 0 ? categories : ['Electronics', 'Furniture', 'Office Supplies', 'Kitchen']
})

const availableWarehouses = computed(() => {
  const warehouses = stockStore.warehouses
  return warehouses.length > 0 ? warehouses : ['Main Warehouse', 'Secondary Warehouse', 'Remote Warehouse']
})

// Methods
const validateField = (fieldName: string) => {
  const fieldValidations = { [fieldName]: stockItemValidations[fieldName] || [] }
  const fieldData = { [fieldName]: formData[fieldName as keyof typeof formData] }
  const result = validateForm(fieldData, fieldValidations)
  
  if (result.isValid) {
    delete validationErrors[fieldName]
  } else {
    validationErrors[fieldName] = result.errors[fieldName] || []
  }
}

const validateAllFields = () => {
  const result = validateForm(formData, stockItemValidations)
  Object.assign(validationErrors, result.errors)
  return result.isValid
}

const handleSubmit = async () => {
  if (!validateAllFields()) {
    return
  }

  try {
    isSubmitting.value = true
    
    if (isEditing.value && props.item) {
      // Update existing item
      stockStore.updateItem(props.item.id, formData)
      showSuccessToast('Stock item updated successfully')
    } else {
      // Add new item
      stockStore.addItem(formData)
      showSuccessToast('Stock item added successfully')
    }
    
    // Call onSave callback if provided
    if (props.onSave) {
      const savedItem = isEditing.value && props.item 
        ? { ...props.item, ...formData }
        : stockStore.items[stockStore.items.length - 1]
      props.onSave(savedItem)
    }
    
    // Reset form
    resetForm()
    
  } catch (error) {
    console.error('Error saving stock item:', error)
    showErrorToast({
      message: 'Failed to save stock item. Please try again.',
      code: 'SAVE_ERROR',
      timestamp: new Date(),
      userFriendly: true
    })
  } finally {
    isSubmitting.value = false
  }
}

const handleCancel = () => {
  if (props.onCancel) {
    props.onCancel()
  } else {
    resetForm()
  }
}

const resetForm = () => {
  Object.assign(formData, {
    name: '',
    description: '',
    sku: '',
    category: '',
    warehouse: '',
    quantity: 0,
    unitCost: 0,
    reorderLevel: 0,
    isActive: true
  })
  Object.keys(validationErrors).forEach(key => delete validationErrors[key])
}

const addNewCategory = () => {
  if (newCategory.value.trim()) {
    // In a real app, you'd add this to the backend
    // For now, we'll just update the form
    formData.category = newCategory.value.trim()
    showNewCategoryModal.value = false
    newCategory.value = ''
  }
}

const addNewWarehouse = () => {
  if (newWarehouse.value.trim()) {
    // In a real app, you'd add this to the backend
    // For now, we'll just update the form
    formData.warehouse = newWarehouse.value.trim()
    showNewWarehouseModal.value = false
    newWarehouse.value = ''
  }
}

// Watch for category/warehouse selection
watch(() => formData.category, (newValue) => {
  if (newValue === 'new') {
    showNewCategoryModal.value = true
    formData.category = ''
  }
})

watch(() => formData.warehouse, (newValue) => {
  if (newValue === 'new') {
    showNewWarehouseModal.value = true
    formData.warehouse = ''
  }
})

// Initialize form data if editing
onMounted(() => {
  if (props.item) {
    Object.assign(formData, {
      name: props.item.name,
      description: props.item.description,
      sku: props.item.sku,
      category: props.item.category,
      warehouse: props.item.warehouse,
      quantity: props.item.quantity,
      unitCost: props.item.unitCost,
      reorderLevel: props.item.reorderLevel,
      isActive: props.item.isActive
    })
  }
})
</script>

<style scoped>
.stock-item-form {
  @apply max-w-4xl mx-auto;
}

.form-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.form-input {
  @apply block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-offset-2 dark:bg-gray-800 dark:text-white;
}

.form-radio {
  @apply h-4 w-4 text-primary-600 focus:ring-primary-500 border-gray-300 dark:border-gray-600;
}

.error-message {
  @apply mt-1 text-sm text-red-600 dark:text-red-400;
}

.btn-primary {
  @apply inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed;
}

.btn-secondary {
  @apply inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500;
}

/* Animation for loading spinner */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}
</style>
