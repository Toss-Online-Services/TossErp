<template>
  <div class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center p-4 z-50 animate-fadeIn">
    <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-2xl max-w-3xl w-full max-h-[90vh] overflow-hidden border border-slate-200/50 dark:border-slate-700/50 animate-slideUp">
      <!-- Header with Gradient -->
      <div class="relative bg-gradient-to-r from-blue-600 via-blue-500 to-blue-600 px-6 py-6 overflow-hidden">
        <div class="absolute inset-0 bg-black/10"></div>
        <div class="relative z-10 flex items-center justify-between">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-white/20 backdrop-blur-sm rounded-lg">
              <CubeIcon class="w-6 h-6 text-white" />
            </div>
            <div>
              <h3 class="text-xl font-bold text-white">
                {{ isEditing ? 'Edit Item' : 'Create New Item' }}
              </h3>
              <p class="text-sm text-white/80 mt-0.5">
                {{ isEditing ? 'Update your inventory item details' : 'Add a new product to your inventory' }}
              </p>
            </div>
          </div>
          <button
            @click="$emit('close')"
            class="p-2 text-white/80 hover:text-white hover:bg-white/20 rounded-lg transition-all duration-200"
          >
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="p-6 overflow-y-auto max-h-[calc(90vh-120px)]">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- Basic Information Section -->
          <div class="md:col-span-2">
            <div class="flex items-center space-x-2 mb-6">
              <div class="p-2 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg">
                <DocumentTextIcon class="w-5 h-5 text-white" />
              </div>
              <div>
                <h4 class="text-base font-semibold text-slate-900 dark:text-white">Basic Information</h4>
                <p class="text-xs text-slate-500 dark:text-slate-400">Product identification and description</p>
              </div>
            </div>
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
            <label for="barcode" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Barcode
            </label>
            <div class="relative">
              <input
                id="barcode"
                v-model="formData.barcode"
                type="text"
                class="w-full px-4 py-2.5 pr-24 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
                placeholder="Enter barcode or scan"
              />
              <button
                type="button"
                @click="showBarcodeScanner = true"
                class="absolute right-2 top-1/2 -translate-y-1/2 px-3 py-1.5 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-md hover:from-purple-700 hover:to-blue-700 transition-all duration-200 flex items-center space-x-1.5 text-xs font-semibold shadow-md hover:shadow-lg hover:scale-105"
              >
                <QrCodeIcon class="w-4 h-4" />
                <span>Scan</span>
              </button>
            </div>
            <p class="mt-1.5 text-xs text-slate-500 dark:text-slate-400">
              Click "Scan" button or use a USB barcode scanner
            </p>
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

          <!-- Pricing Section -->
          <div class="md:col-span-2 mt-4">
            <div class="flex items-center space-x-2 mb-6">
              <div class="p-2 bg-gradient-to-br from-green-500 to-emerald-600 rounded-lg">
                <CurrencyDollarIcon class="w-5 h-5 text-white" />
              </div>
              <div>
                <h4 class="text-base font-semibold text-slate-900 dark:text-white">Pricing</h4>
                <p class="text-xs text-slate-500 dark:text-slate-400">Set your selling and cost prices</p>
              </div>
            </div>
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

          <!-- Stock Management Section -->
          <div class="md:col-span-2 mt-4">
            <div class="flex items-center space-x-2 mb-6">
              <div class="p-2 bg-gradient-to-br from-orange-500 to-red-600 rounded-lg">
                <ArchiveBoxIcon class="w-5 h-5 text-white" />
              </div>
              <div>
                <h4 class="text-base font-semibold text-slate-900 dark:text-white">Stock Management</h4>
                <p class="text-xs text-slate-500 dark:text-slate-400">Configure reorder alerts and thresholds</p>
              </div>
            </div>
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
        <div class="flex flex-col sm:flex-row justify-end space-y-3 sm:space-y-0 sm:space-x-4 mt-8 pt-6 border-t border-slate-200 dark:border-slate-700">
          <button
            type="button"
            @click="$emit('close')"
            class="px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 hover:border-slate-400 dark:hover:border-slate-500 transition-all duration-200 font-medium"
          >
            Cancel
          </button>
          <button
            type="submit"
            class="relative px-8 py-3 bg-gradient-to-r from-blue-600 via-blue-500 to-blue-600 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transform hover:scale-105 transition-all duration-200 overflow-hidden group"
          >
            <span class="relative z-10 flex items-center justify-center">
              <CheckIcon v-if="isEditing" class="w-5 h-5 mr-2" />
              <PlusIcon v-else class="w-5 h-5 mr-2" />
              {{ isEditing ? 'Update Item' : 'Create Item' }}
            </span>
            <div class="absolute inset-0 bg-gradient-to-r from-blue-700 via-blue-600 to-blue-700 opacity-0 group-hover:opacity-100 transition-opacity duration-200"></div>
          </button>
        </div>
      </form>
    </div>

    <!-- Barcode Scanner Component -->
    <BarcodeScanner
      v-model="showBarcodeScanner"
      @barcode-scanned="handleBarcodeScanned"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { 
  XMarkIcon,
  CubeIcon,
  DocumentTextIcon,
  CurrencyDollarIcon,
  ArchiveBoxIcon,
  CheckIcon,
  PlusIcon,
  QrCodeIcon
} from '@heroicons/vue/24/outline'
import BarcodeScanner from '~/components/pos/BarcodeScanner.vue'
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

// Barcode Scanner state
const showBarcodeScanner = ref(false)

// Barcode Scanner handler
const handleBarcodeScanned = (barcode: string) => {
  formData.value.barcode = barcode
  showBarcodeScanner.value = false
  
  // Optional: Show success feedback
  console.log('Barcode scanned:', barcode)
}

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

<style scoped>
@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.2s ease-out;
}

.animate-slideUp {
  animation: slideUp 0.3s ease-out;
}

/* Smooth scrollbar styling */
.overflow-y-auto::-webkit-scrollbar {
  width: 8px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: transparent;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: rgb(203 213 225 / 0.5);
  border-radius: 4px;
}

.overflow-y-auto::-webkit-scrollbar-thumb:hover {
  background: rgb(148 163 184 / 0.7);
}

.dark .overflow-y-auto::-webkit-scrollbar-thumb {
  background: rgb(51 65 85 / 0.5);
}

.dark .overflow-y-auto::-webkit-scrollbar-thumb:hover {
  background: rgb(71 85 105 / 0.7);
}
</style>
