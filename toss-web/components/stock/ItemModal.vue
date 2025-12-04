<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'
import { useBuyingStore } from '~/stores/buying'
import StockAdjustmentForm from '~/components/stock/StockAdjustmentForm.vue'

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
const buyingStore = useBuyingStore()

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
  isActive: true,
  imageUrl: ''
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
      isActive: newItem.isActive,
      imageUrl: newItem.imageUrl || ''
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
    isActive: true,
    imageUrl: ''
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

function getItemImage(itemId?: string) {
  // Only return image URL if explicitly set
  if (formData.value.imageUrl) return formData.value.imageUrl
  
  // For existing items, try to get from item data
  if (itemId && props.item?.imageUrl) {
    return props.item.imageUrl
  }
  
  // Return null for new items or when no image is set
  return null
}

const hasImage = computed(() => {
  return !!(formData.value.imageUrl || (props.item?.imageUrl && isEditing.value))
})

const fileInputRef = ref<HTMLInputElement | null>(null)
const isDragging = ref(false)

function processImageFile(file: File) {
  // Validate file type
  if (!file.type.startsWith('image/')) {
    alert('Please select an image file')
    return false
  }
  
  // Validate file size (max 5MB)
  if (file.size > 5 * 1024 * 1024) {
    alert('Image size must be less than 5MB')
    return false
  }
  
  // Convert to data URL
  const reader = new FileReader()
  reader.onload = (e) => {
    const result = e.target?.result as string
    if (result) {
      formData.value.imageUrl = result
    }
  }
  reader.onerror = () => {
    alert('Failed to read image file')
  }
  reader.readAsDataURL(file)
  
  return true
}

function handleFileSelect(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (!file) return
  
  processImageFile(file)
  
  // Reset input so same file can be selected again
  if (fileInputRef.value) {
    fileInputRef.value.value = ''
  }
}

function triggerFileInput() {
  fileInputRef.value?.click()
}

function handleRemoveImage() {
  formData.value.imageUrl = ''
  if (fileInputRef.value) {
    fileInputRef.value.value = ''
  }
}

// Drag and drop handlers
function handleDragEnter(event: DragEvent) {
  event.preventDefault()
  event.stopPropagation()
  isDragging.value = true
}

function handleDragOver(event: DragEvent) {
  event.preventDefault()
  event.stopPropagation()
}

function handleDragLeave(event: DragEvent) {
  event.preventDefault()
  event.stopPropagation()
  // Only set to false if we're leaving the drop zone entirely
  const target = event.target as HTMLElement
  const relatedTarget = event.relatedTarget as HTMLElement
  if (!target.contains(relatedTarget)) {
    isDragging.value = false
  }
}

function handleDrop(event: DragEvent) {
  event.preventDefault()
  event.stopPropagation()
  isDragging.value = false
  
  const files = event.dataTransfer?.files
  if (!files || files.length === 0) return
  
  const file = files[0]
  processImageFile(file)
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

function handleStockAdjusted() {
  // Refresh item data after stock adjustment
  if (props.item) {
    stockStore.fetchItems().then(() => {
      // Update formData with fresh item data
      const updatedItem = stockStore.getItemById(props.item!.id)
      if (updatedItem) {
        Object.assign(formData.value, {
          code: updatedItem.code,
          name: updatedItem.name,
          description: updatedItem.description || '',
          category: updatedItem.category,
          unit: updatedItem.unit,
          costPrice: updatedItem.costPrice,
          sellingPrice: updatedItem.sellingPrice,
          currentStock: updatedItem.currentStock,
          minStock: updatedItem.minStock,
          maxStock: updatedItem.maxStock || 0,
          warehouse: updatedItem.warehouse || 'main',
          barcode: updatedItem.barcode || '',
          supplier: updatedItem.supplier || '',
          isActive: updatedItem.isActive,
          imageUrl: updatedItem.imageUrl || ''
        })
      }
    })
  }
}

// Load suppliers
watch(() => props.show, async (newVal) => {
  if (newVal) {
    await buyingStore.fetchSuppliers()
  }
}, { immediate: true })

const suppliers = computed(() => buyingStore.suppliers)
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="show"
        class="overflow-y-auto fixed inset-0 z-50"
        @click.self="handleClose"
        style="background-color: rgba(0, 0, 0, 0.5);"
      >
        <div class="flex justify-center items-center p-4 min-h-screen">
          <!-- Modal Container - Matching edit-product.html structure -->
          <div
            class="overflow-y-auto relative w-full max-w-6xl bg-white rounded-xl shadow-xl"
            style="max-height: 90vh;"
            @click.stop
          >
            <!-- Header Row - Matching template -->
            <div class="relative p-6 border-b border-gray-200">
              <!-- Close Button -->
              <button
                @click="handleClose"
                class="absolute top-6 right-6 z-10 p-1 text-gray-400 hover:text-gray-600 transition-colors rounded-full hover:bg-gray-100"
                type="button"
                aria-label="Close modal"
              >
                <i class="text-2xl material-symbols-rounded">close</i>
              </button>
              
              <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 pr-10">
                <div>
                  <h4 class="mb-1 text-xl font-semibold text-gray-900">
                    {{ isEditing ? 'Make the changes below' : 'Add new item' }}
                  </h4>
                  <p class="text-sm text-gray-600">
                    {{ isEditing ? 'Update your item information' : 'This information will let us know more about your item.' }}
                  </p>
                </div>
                <div class="flex justify-end items-center lg:justify-end">
                  <button
                    @click="handleSave"
                    type="button"
                    :disabled="isSubmitting"
                    class="px-4 py-2 text-white bg-gradient-dark rounded-lg hover:shadow-lg transition-all disabled:opacity-50"
                  >
                    <span v-if="isSubmitting">Saving...</span>
                    <span v-else>Save</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- Form Content - Matching edit-product.html layout -->
            <form @submit.prevent="handleSave" class="p-6">
              <div class="flex flex-col lg:flex-row gap-6 mt-4">
                <!-- Left Column: Image Card (col-lg-4) -->
                <div class="w-full lg:w-1/3 flex-shrink-0">
                  <div class="card mt-4" data-animation="true">
                    <div class="card-header p-0 position-relative mt-n4 mx-3 z-index-2">
                      <!-- Hidden file input -->
                      <input
                        ref="fileInputRef"
                        type="file"
                        accept="image/*"
                        class="hidden"
                        @change="handleFileSelect"
                      />
                      
                      <!-- Image Display or Placeholder -->
                      <div 
                        v-if="hasImage" 
                        class="blur-shadow-image cursor-pointer group"
                        :class="{ 'drag-active': isDragging }"
                        @click="triggerFileInput"
                        @dragenter="handleDragEnter"
                        @dragover="handleDragOver"
                        @dragleave="handleDragLeave"
                        @drop="handleDrop"
                      >
                        <img
                          :src="formData.imageUrl || props.item?.imageUrl"
                          :alt="formData.name || 'Product'"
                          class="img-fluid shadow border-radius-lg block"
                          style="width: 100%; height: 250px; object-fit: cover;"
                        />
                        <div
                          class="colored-shadow"
                          :style="{ backgroundImage: `url('${formData.imageUrl || props.item?.imageUrl}')` }"
                        ></div>
                        <!-- Overlay hint on hover -->
                        <div class="image-overlay absolute inset-0 bg-black bg-opacity-0 transition-all duration-200 flex items-center justify-center rounded-lg pointer-events-none">
                          <span class="text-white opacity-0 transition-opacity text-sm font-medium">Click or drag to change image</span>
                        </div>
                        <!-- Drag overlay -->
                        <div v-if="isDragging" class="absolute inset-0 bg-blue-500 bg-opacity-50 flex items-center justify-center rounded-lg z-10">
                          <div class="text-center text-white">
                            <i class="material-symbols-rounded text-4xl mb-2 block">cloud_upload</i>
                            <p class="text-sm font-medium">Drop image here</p>
                          </div>
                        </div>
                      </div>
                      <!-- Placeholder for new product or no image -->
                      <div
                        v-else
                        class="flex items-center justify-center blur-shadow-image border-2 border-dashed rounded-lg cursor-pointer transition-colors"
                        :class="isDragging ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-gray-400 hover:bg-gray-50'"
                        style="width: 100%; height: 250px; background-color: #f9fafb;"
                        @click="triggerFileInput"
                        @dragenter="handleDragEnter"
                        @dragover="handleDragOver"
                        @dragleave="handleDragLeave"
                        @drop="handleDrop"
                      >
                        <div v-if="!isDragging" class="text-center p-4">
                          <i class="material-symbols-rounded text-5xl text-gray-400 mb-2 block">image</i>
                          <p class="text-sm text-gray-600 mb-1">Click to browse or drag an image here</p>
                          <p class="text-xs text-gray-500 mb-0">or enter image URL below</p>
                        </div>
                        <div v-else class="text-center p-4">
                          <i class="material-symbols-rounded text-5xl text-blue-500 mb-2 block">cloud_upload</i>
                          <p class="text-sm text-blue-600 font-medium mb-1">Drop image here</p>
                          <p class="text-xs text-blue-500 mb-0">Release to upload</p>
                        </div>
                      </div>
                    </div>
                    <div class="card-body text-center">
                      <!-- Image Action Buttons (only show when image exists) -->
                      <div v-if="hasImage" class="mt-n6 flex justify-center items-center gap-2">
                        <button
                          type="button"
                          class="btn bg-gradient-dark btn-sm"
                          @click="triggerFileInput"
                        >
                          Edit
                        </button>
                        <button
                          type="button"
                          class="btn btn-outline-dark btn-sm"
                          @click="handleRemoveImage"
                        >
                          Remove
                        </button>
                      </div>
                      <h5 class="font-weight-normal mt-4 mb-0">Product Image</h5>
                      <div class="mt-3 input-group input-group-dynamic">
                        <label class="form-label">Image URL</label>
                        <input
                          v-model="formData.imageUrl"
                          type="url"
                          class="form-control"
                          placeholder="https://example.com/image.jpg"
                        />
                      </div>
                      <p class="text-xs text-gray-500 mt-2 mb-0">Or paste an image URL above</p>
                    </div>
                  </div>
                </div>

                <!-- Right Column: Product Information (col-lg-8) -->
                <div class="w-full lg:w-2/3 flex-grow">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="font-weight-bolder">Product Information</h5>
                      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-4">
                        <div>
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Name <span class="text-red-500">*</span></label>
                            <input
                              v-model="formData.name"
                              type="text"
                              class="form-control w-full"
                              :class="{ 'border-red-500': errors.name }"
                              placeholder="Item name"
                            />
                          </div>
                          <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                        </div>
                        <div>
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Item Code <span class="text-red-500">*</span></label>
                            <input
                              v-model="formData.code"
                              type="text"
                              class="form-control w-full"
                              :class="{ 'border-red-500': errors.code }"
                              placeholder="e.g., CEM-001"
                            />
                          </div>
                          <p v-if="errors.code" class="mt-1 text-sm text-red-600">{{ errors.code }}</p>
                        </div>
                      </div>
                      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 mt-4">
                        <div>
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Current Stock</label>
                            <input
                              v-model.number="formData.currentStock"
                              type="number"
                              min="0"
                              class="form-control w-full"
                              placeholder="0"
                            />
                          </div>
                        </div>
                        <div>
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Cost Price <span class="text-red-500">*</span></label>
                            <input
                              v-model.number="formData.costPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-full"
                              :class="{ 'border-red-500': errors.costPrice }"
                              placeholder="0.00"
                            />
                          </div>
                          <p v-if="errors.costPrice" class="mt-1 text-sm text-red-600">{{ errors.costPrice }}</p>
                        </div>
                        <div>
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Selling Price <span class="text-red-500">*</span></label>
                            <input
                              v-model.number="formData.sellingPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-full"
                              :class="{ 'border-red-500': errors.sellingPrice }"
                              placeholder="0.00"
                            />
                          </div>
                          <p v-if="errors.sellingPrice" class="mt-1 text-sm text-red-600">{{ errors.sellingPrice }}</p>
                        </div>
                        <div>
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Min Stock</label>
                            <input
                              v-model.number="formData.minStock"
                              type="number"
                              min="0"
                              class="form-control w-full"
                              :class="{ 'border-red-500': errors.minStock }"
                              placeholder="0"
                            />
                          </div>
                          <p v-if="errors.minStock" class="mt-1 text-sm text-red-600">{{ errors.minStock }}</p>
                        </div>
                      </div>
                      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-4">
                        <div>
                          <label class="block text-sm font-medium text-gray-700 mb-2">
                            Description
                            <span class="text-xs text-gray-500 font-normal">(optional)</span>
                          </label>
                          <textarea
                            v-model="formData.description"
                            rows="4"
                            class="form-control w-full"
                            placeholder="Item description..."
                          ></textarea>
                        </div>
                        <div>
                          <label class="block text-sm font-medium text-gray-700 mb-2">
                            Category <span class="text-red-500">*</span>
                          </label>
                          <select
                            v-model="formData.category"
                            class="form-control w-full"
                            :class="{ 'border-red-500': errors.category }"
                          >
                            <option value="">Select category</option>
                            <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                          </select>
                          <p v-if="errors.category" class="mt-1 text-sm text-red-600">{{ errors.category }}</p>
                          <label class="block text-sm font-medium text-gray-700 mb-2 mt-3">Unit of Measure</label>
                          <select v-model="formData.unit" class="form-control w-full">
                            <option v-for="u in units" :key="u" :value="u">{{ u }}</option>
                          </select>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Second Row: Additional Info and Pricing -->
              <div class="flex flex-col sm:flex-row gap-6 mt-4">
                <!-- Additional Info Card (col-sm-4) -->
                <div class="w-full sm:w-1/3">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="font-weight-bolder">Additional Info</h5>
                      <div class="input-group input-group-dynamic mt-3">
                        <label class="form-label">Barcode</label>
                        <input
                          v-model="formData.barcode"
                          type="text"
                          class="form-control w-full"
                          placeholder="Optional"
                        />
                      </div>
                      <div class="input-group input-group-dynamic mt-3">
                        <label class="form-label">Default Supplier</label>
                        <select v-model="formData.supplier" class="form-control w-full">
                          <option value="">No supplier</option>
                          <option v-for="supp in suppliers" :key="supp.id" :value="supp.name">
                            {{ supp.name }}
                          </option>
                        </select>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Pricing Card (col-sm-8) -->
                <div class="w-full sm:w-2/3">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="font-weight-bolder mb-3">Pricing & Status</h5>
                      <div class="grid grid-cols-12 gap-4">
                        <div class="col-span-12 sm:col-span-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Cost Price</label>
                            <input
                              v-model.number="formData.costPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-full"
                              :class="{ 'border-red-500': errors.costPrice }"
                              placeholder="0.00"
                            />
                          </div>
                        </div>
                        <div class="col-span-12 sm:col-span-4">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Currency</label>
                            <select class="form-control w-full" disabled>
                              <option value="ZAR" selected>ZAR</option>
                            </select>
                          </div>
                        </div>
                        <div class="col-span-12 sm:col-span-5">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Selling Price</label>
                            <input
                              v-model.number="formData.sellingPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-full"
                              :class="{ 'border-red-500': errors.sellingPrice }"
                              placeholder="0.00"
                            />
                          </div>
                        </div>
                      </div>
                      <div class="mt-4">
                        <label class="block text-sm font-medium text-gray-700 mb-2">Active Status</label>
                        <div class="flex items-center gap-2">
                          <input
                            v-model="formData.isActive"
                            class="w-4 h-4 text-gray-900 border-gray-300 rounded focus:ring-gray-900"
                            type="checkbox"
                            :id="isEditing ? 'isActiveEdit' : 'isActiveNew'"
                          />
                          <label class="text-sm font-medium text-gray-700 cursor-pointer" :for="isEditing ? 'isActiveEdit' : 'isActiveNew'">
                            Item is active
                          </label>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Stock Adjustment Section (only in edit mode) -->
                <div v-if="isEditing && props.item" class="w-full mt-6">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="font-weight-bolder mb-4 flex items-center gap-2">
                        <i class="material-symbols-rounded text-xl">tune</i>
                        Adjust Stock
                      </h5>
                      <StockAdjustmentForm
                        :item="props.item"
                        :show-item-info="false"
                        :compact="true"
                        @adjusted="handleStockAdjusted"
                        @cancelled="() => {}"
                      />
                    </div>
                  </div>
                </div>
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

/* Material Dashboard styles matching template */
.bg-gradient-dark {
  background: linear-gradient(195deg, #42424a, #191919) !important;
}

.border-radius-lg {
  border-radius: 0.5rem;
}

.border-radius-xl {
  border-radius: 0.75rem;
}

.input-group-dynamic {
  position: relative;
}

.input-group-dynamic .form-label {
  position: absolute;
  top: 0.5rem;
  left: 0.75rem;
  font-size: 0.75rem;
  font-weight: 500;
  color: #7b809a;
  pointer-events: none;
  transition: all 0.2s;
  transform-origin: 0 0;
}

.input-group-dynamic .form-control:focus ~ .form-label,
.input-group-dynamic .form-control:not(:placeholder-shown) ~ .form-label,
.input-group-dynamic .form-control.has-value ~ .form-label {
  transform: translateY(-0.5rem) scale(0.85);
  opacity: 0.8;
}

.input-group-dynamic .form-control {
  padding-top: 1.5rem;
  padding-bottom: 0.5rem;
}

.colored-shadow {
  position: absolute;
  top: 12%;
  left: 4%;
  width: 100%;
  height: 100%;
  display: block;
  opacity: 0.3;
  filter: blur(40px);
  background-size: cover;
  background-position: center;
  z-index: -1;
  border-radius: 0.5rem;
}

/* Card styles matching Material Dashboard */
.card {
  background: white;
  border-radius: 0.75rem;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  margin-bottom: 1.5rem;
}

.card-header {
  border: none;
  background: transparent;
  padding: 0;
}

.card-body {
  padding: 1.5rem;
}

/* Form control styles */
.form-control {
  width: 100%;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  line-height: 1.5;
  color: #495057;
  background-color: #fff;
  border: 1px solid #ced4da;
  border-radius: 0.375rem;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

.form-control:focus {
  outline: 0;
  border-color: #42424a;
  box-shadow: 0 0 0 0.2rem rgba(66, 66, 74, 0.25);
}

.form-control:disabled {
  background-color: #e9ecef;
  opacity: 1;
}

/* Button styles */
.btn {
  display: inline-block;
  font-weight: 400;
  line-height: 1.5;
  color: #212529;
  text-align: center;
  text-decoration: none;
  vertical-align: middle;
  cursor: pointer;
  user-select: none;
  border: 1px solid transparent;
  padding: 0.375rem 0.75rem;
  font-size: 0.875rem;
  border-radius: 0.375rem;
  transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

.btn-sm {
  padding: 0.25rem 0.5rem;
  font-size: 0.75rem;
  border-radius: 0.25rem;
}

.btn-outline-dark {
  color: #42424a;
  border-color: #42424a;
  background-color: transparent;
}

.btn-outline-dark:hover {
  color: #fff;
  background-color: #42424a;
  border-color: #42424a;
}

/* Utility classes */
.mt-n4 {
  margin-top: -1.5rem;
}

.mt-n6 {
  margin-top: -2.5rem;
}

.z-index-2 {
  z-index: 2;
}

.blur-shadow-image {
  position: relative;
  overflow: hidden;
  border-radius: 0.5rem;
  display: block;
}

.img-fluid {
  max-width: 100%;
  height: auto;
}

.block {
  display: block;
}

/* Image placeholder styling */
.blur-shadow-image {
  position: relative;
  overflow: hidden;
  border-radius: 0.5rem;
}

.cursor-pointer {
  cursor: pointer;
}

.hidden {
  display: none;
}

/* Image hover overlay */
.blur-shadow-image.group:hover .image-overlay {
  background-color: rgba(0, 0, 0, 0.3);
}

.blur-shadow-image.group:hover .image-overlay span {
  opacity: 1;
}

/* Drag and drop styles */
.drag-active {
  border: 2px dashed #3b82f6 !important;
  background-color: rgba(59, 130, 246, 0.1) !important;
}
</style>
