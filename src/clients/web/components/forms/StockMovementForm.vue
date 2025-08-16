<template>
  <div class="stock-movement-form">
    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- Form Header -->
      <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
        <h3 class="text-lg font-medium text-gray-900 dark:text-white">
          Record Stock Movement
        </h3>
        <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">
          Record a stock movement for inventory tracking.
        </p>
      </div>

      <!-- Form Fields -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Item Selection -->
        <div class="md:col-span-2">
          <label for="itemId" class="form-label">
            Item <span class="text-red-500">*</span>
          </label>
          <select
            id="itemId"
            v-model="formData.itemId"
            :class="[
              'form-input',
              getErrorClass('itemId', validationErrors)
            ]"
            @change="handleItemChange"
            @blur="validateField('itemId')"
          >
            <option value="">Select an item</option>
            <option v-for="item in availableItems" :key="item.id" :value="item.id">
              {{ item.name }} ({{ item.sku }}) - {{ item.warehouse }}
            </option>
          </select>
          <div v-if="hasError('itemId', validationErrors)" class="error-message">
            {{ getFirstError('itemId', validationErrors) }}
          </div>
        </div>

        <!-- Movement Type -->
        <div>
          <label for="type" class="form-label">
            Movement Type <span class="text-red-500">*</span>
          </label>
          <select
            id="type"
            v-model="formData.type"
            :class="[
              'form-input',
              getErrorClass('type', validationErrors)
            ]"
            @blur="validateField('type')"
          >
            <option value="">Select type</option>
            <option value="in">Stock In</option>
            <option value="out">Stock Out</option>
            <option value="adjustment">Adjustment</option>
            <option value="transfer">Transfer</option>
          </select>
          <div v-if="hasError('type', validationErrors)" class="error-message">
            {{ getFirstError('type', validationErrors) }}
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
            min="1"
            :class="[
              'form-input',
              getErrorClass('quantity', validationErrors)
            ]"
            placeholder="Enter quantity"
            @blur="validateField('quantity')"
          />
          <div v-if="hasError('quantity', validationErrors)" class="error-message">
            {{ getFirstError('quantity', validationErrors) }}
          </div>
          <p v-if="selectedItem" class="text-xs text-gray-500 dark:text-gray-400 mt-1">
            Current stock: {{ selectedItem.quantity }} units
          </p>
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
          </select>
          <div v-if="hasError('warehouse', validationErrors)" class="error-message">
            {{ getFirstError('warehouse', validationErrors) }}
          </div>
        </div>

        <!-- Reference -->
        <div>
          <label for="reference" class="form-label">
            Reference <span class="text-red-500">*</span>
          </label>
          <input
            id="reference"
            v-model="formData.reference"
            type="text"
            :class="[
              'form-input',
              getErrorClass('reference', validationErrors)
            ]"
            placeholder="e.g., PO-2024-001, SO-2024-001"
            @blur="validateField('reference')"
          />
          <div v-if="hasError('reference', validationErrors)" class="error-message">
            {{ getFirstError('reference', validationErrors) }}
          </div>
        </div>

        <!-- Created By -->
        <div>
          <label for="createdBy" class="form-label">
            Created By <span class="text-red-500">*</span>
          </label>
          <input
            id="createdBy"
            v-model="formData.createdBy"
            type="text"
            :class="[
              'form-input',
              getErrorClass('createdBy', validationErrors)
            ]"
            placeholder="Enter your name"
            @blur="validateField('createdBy')"
          />
          <div v-if="hasError('createdBy', validationErrors)" class="error-message">
            {{ getFirstError('createdBy', validationErrors) }}
          </div>
        </div>
      </div>

      <!-- Reason -->
      <div>
        <label for="reason" class="form-label">
          Reason <span class="text-red-500">*</span>
        </label>
        <textarea
          id="reason"
          v-model="formData.reason"
          rows="3"
          :class="[
            'form-input',
            getErrorClass('reason', validationErrors)
          ]"
          placeholder="Explain the reason for this movement"
          @blur="validateField('reason')"
        ></textarea>
        <div v-if="hasError('reason', validationErrors)" class="error-message">
          {{ getFirstError('reason', validationErrors) }}
        </div>
        <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">
          {{ formData.reason?.length || 0 }}/200 characters
        </p>
      </div>

      <!-- Stock Level Warning -->
      <div v-if="showStockWarning" class="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-lg p-4">
        <div class="flex">
          <ExclamationTriangleIcon class="h-5 w-5 text-yellow-400 mt-0.5" />
          <div class="ml-3">
            <h3 class="text-sm font-medium text-yellow-800 dark:text-yellow-200">
              Stock Level Warning
            </h3>
            <div class="mt-2 text-sm text-yellow-700 dark:text-yellow-300">
              <p v-if="formData.type === 'out' && selectedItem">
                This movement will reduce stock to {{ selectedItem.quantity - formData.quantity }} units.
                <span v-if="selectedItem.quantity - formData.quantity <= selectedItem.reorderLevel" class="font-medium">
                  This will trigger a low stock alert.
                </span>
              </p>
              <p v-else-if="formData.type === 'adjustment' && selectedItem">
                This adjustment will set stock to {{ formData.quantity }} units.
                <span v-if="formData.quantity <= selectedItem.reorderLevel" class="font-medium">
                  This will trigger a low stock alert.
                </span>
              </p>
            </div>
          </div>
        </div>
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
          {{ isSubmitting ? 'Recording...' : 'Record Movement' }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
import { useStockStore } from '../../stores/stock'
import type { StockItem, StockMovement } from '../../stores/stock'
import {
  validateForm,
  stockMovementValidations,
  getFirstError,
  hasError,
  getErrorClass
} from '../../utils/validation'
import { showErrorToast, showSuccessToast } from '../../utils/errorHandling'

interface Props {
  onSave?: (movement: StockMovement) => void
  onCancel?: () => void
}

const props = withDefaults(defineProps<Props>(), {
  onSave: undefined,
  onCancel: undefined
})

const stockStore = useStockStore()

// Form state
const isSubmitting = ref(false)
const validationErrors = reactive<Record<string, string[]>>({})

// Form data
const formData = reactive<Omit<StockMovement, 'id' | 'timestamp' | 'itemName'>>({
  itemId: 0,
  type: '' as 'in' | 'out' | 'adjustment' | 'transfer',
  quantity: 1,
  warehouse: '',
  reference: '',
  reason: '',
  createdBy: ''
})

// Computed properties
const isFormValid = computed(() => Object.keys(validationErrors).length === 0)

const availableItems = computed(() => {
  return stockStore.items.filter(item => item.isActive)
})

const availableWarehouses = computed(() => {
  return stockStore.warehouses
})

const selectedItem = computed(() => {
  return stockStore.items.find(item => item.id === formData.itemId)
})

const showStockWarning = computed(() => {
  if (!selectedItem.value || !formData.quantity) return false
  
  if (formData.type === 'out') {
    return selectedItem.value.quantity - formData.quantity <= selectedItem.value.reorderLevel
  }
  
  if (formData.type === 'adjustment') {
    return formData.quantity <= selectedItem.value.reorderLevel
  }
  
  return false
})

// Methods
const validateField = (fieldName: string) => {
  const fieldValidations = { [fieldName]: stockMovementValidations[fieldName] || [] }
  const fieldData = { [fieldName]: formData[fieldName as keyof typeof formData] }
  const result = validateForm(fieldData, fieldValidations)
  
  if (result.isValid) {
    delete validationErrors[fieldName]
  } else {
    validationErrors[fieldName] = result.errors[fieldName] || []
  }
}

const validateAllFields = () => {
  const result = validateForm(formData, stockMovementValidations)
  Object.assign(validationErrors, result.errors)
  return result.isValid
}

const handleItemChange = () => {
  if (selectedItem.value) {
    formData.warehouse = selectedItem.value.warehouse
  }
}

const handleSubmit = async () => {
  if (!validateAllFields()) {
    return
  }

  try {
    isSubmitting.value = true
    
    // Create movement data
    const movementData = {
      ...formData,
      itemName: selectedItem.value?.name || ''
    }
    
    // Add movement to store
    stockStore.addMovement(movementData)
    
    showSuccessToast('Stock movement recorded successfully')
    
    // Call onSave callback if provided
    if (props.onSave) {
      const savedMovement = stockStore.movements[0] // Newest movement
      props.onSave(savedMovement)
    }
    
    // Reset form
    resetForm()
    
  } catch (error) {
    console.error('Error recording stock movement:', error)
    showErrorToast({
      message: 'Failed to record stock movement. Please try again.',
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
    itemId: 0,
    type: '' as 'in' | 'out' | 'adjustment' | 'transfer',
    quantity: 1,
    warehouse: '',
    reference: '',
    reason: '',
    createdBy: ''
  })
  Object.keys(validationErrors).forEach(key => delete validationErrors[key])
}

// Watch for item selection to auto-fill warehouse
watch(() => formData.itemId, (newValue) => {
  if (newValue && selectedItem.value) {
    formData.warehouse = selectedItem.value.warehouse
  }
})

// Initialize with current user (in a real app, this would come from auth)
onMounted(() => {
  // For demo purposes, set a default user
  formData.createdBy = 'Current User'
})
</script>

<style scoped>
.stock-movement-form {
  @apply max-w-4xl mx-auto;
}

.form-label {
  @apply block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2;
}

.form-input {
  @apply block w-full px-3 py-2 border rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-offset-2 dark:bg-gray-800 dark:text-white;
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
