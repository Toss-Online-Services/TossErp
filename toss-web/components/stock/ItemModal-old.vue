<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useStockStore, type Item } from '~/stores/stock'
import { useBuyingStore } from '~/stores/buying'

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

// Multi-step form state (for new items)
const currentStep = ref(1)
const totalSteps = 4

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
    currentStep.value = 1
  } else {
    resetForm()
    currentStep.value = 1
  }
}, { immediate: true })

watch(() => props.show, (newVal) => {
  if (!newVal) {
    currentStep.value = 1
  }
})

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
  currentStep.value = 1
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

function validateStep(step: number): boolean {
  errors.value = {}
  
  if (step === 1) {
    if (!formData.value.code.trim()) {
      errors.value.code = 'Item code is required'
      return false
    }
    if (!formData.value.name.trim()) {
      errors.value.name = 'Item name is required'
      return false
    }
    if (!formData.value.category) {
      errors.value.category = 'Category is required'
      return false
    }
  }
  
  if (step === 4) {
    if (formData.value.costPrice < 0) {
      errors.value.costPrice = 'Cost price cannot be negative'
      return false
    }
    if (formData.value.sellingPrice < 0) {
      errors.value.sellingPrice = 'Selling price cannot be negative'
      return false
    }
    if (formData.value.sellingPrice < formData.value.costPrice) {
      errors.value.sellingPrice = 'Selling price should be higher than cost price'
      return false
    }
  }
  
  return true
}

function nextStep(event?: Event) {
  if (event) {
    event.preventDefault()
    event.stopPropagation()
  }
  
  // Validate current step but allow navigation with warnings
  validateStep(currentStep.value)
  
  // Allow navigation regardless of validation (errors are shown)
  if (currentStep.value < totalSteps) {
    currentStep.value++
    // Clear errors when moving forward
    errors.value = {}
  }
}

function prevStep(event?: Event) {
  if (event) {
    event.preventDefault()
    event.stopPropagation()
  }
  
  if (currentStep.value > 1) {
    currentStep.value--
    // Clear errors when going back
    errors.value = {}
  }
}

function getItemImage(itemId?: string) {
  if (formData.value.imageUrl) return formData.value.imageUrl
  
  const images = [
    '/images/products/product-1-min.jpg',
    '/images/products/product-2-min.jpg',
    '/images/products/product-3-min.jpg',
    '/images/products/product-4-min.jpg',
    '/images/products/product-5-min.jpg'
  ]
  if (itemId) {
    const index = parseInt(itemId) % images.length
    return images[index]
  }
  return images[0]
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
          <!-- NEW ITEM MODAL - Multi-step form matching new-product.html -->
          <div
            v-if="!isEditing"
            class="overflow-hidden relative w-full max-w-4xl bg-white rounded-xl shadow-xl"
            @click.stop
          >
            <!-- Progress Bar Header -->
            <div class="px-6 pt-4 pb-3 bg-gradient-dark shadow-dark border-radius-lg">
              <div class="flex justify-between items-center multisteps-form__progress">
                <button
                  type="button"
                  :class="[
                    'multisteps-form__progress-btn px-4 py-2 rounded-lg text-sm font-medium transition-all',
                    currentStep === 1 ? 'bg-white text-gray-900' : 'text-white opacity-70'
                  ]"
                  @click="currentStep = 1"
                >
                  <span>1. Product Info</span>
                </button>
                <button
                  type="button"
                  :class="[
                    'multisteps-form__progress-btn px-4 py-2 rounded-lg text-sm font-medium transition-all',
                    currentStep === 2 ? 'bg-white text-gray-900' : 'text-white opacity-70'
                  ]"
                  @click="currentStep = 2"
                >
                  <span>2. Media</span>
                </button>
                <button
                  type="button"
                  :class="[
                    'multisteps-form__progress-btn px-4 py-2 rounded-lg text-sm font-medium transition-all',
                    currentStep === 3 ? 'bg-white text-gray-900' : 'text-white opacity-70'
                  ]"
                  @click="currentStep = 3"
                >
                  <span>3. Stock & Supplier</span>
                </button>
                <button
                  type="button"
                  :class="[
                    'multisteps-form__progress-btn px-4 py-2 rounded-lg text-sm font-medium transition-all',
                    currentStep === 4 ? 'bg-white text-gray-900' : 'text-white opacity-70'
                  ]"
                  @click="currentStep = 4"
                >
                  <span>4. Pricing</span>
                </button>
              </div>
            </div>

            <!-- Close Button -->
            <button
              @click="handleClose"
              class="absolute top-4 right-4 z-10 text-white transition-colors hover:text-gray-200"
            >
              <i class="text-2xl material-symbols-rounded">close</i>
            </button>

            <!-- Form Steps -->
            <form @submit.prevent="handleSave" class="p-6">
              <!-- Step 1: Product Information -->
              <div
                v-show="currentStep === 1"
                class="pt-3 bg-white multisteps-form__panel border-radius-xl"
              >
                <h5 class="mb-4 text-lg font-semibold font-weight-bolder">Product Information</h5>
                <div class="multisteps-form__content">
                  <div class="mt-3 row">
                    <div class="mb-4 col-12 col-sm-6">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Name <span class="text-red-500">*</span></label>
                        <input
                          v-model="formData.name"
                          type="text"
                          class="multisteps-form__input form-control"
                          :class="{ 'border-red-500': errors.name }"
                          placeholder="Item name"
                        />
                      </div>
                      <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                    </div>
                    <div class="mt-3 mb-4 col-12 col-sm-6 mt-sm-0">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Item Code <span class="text-red-500">*</span></label>
                        <input
                          v-model="formData.code"
                          type="text"
                          class="multisteps-form__input form-control"
                          :class="{ 'border-red-500': errors.code }"
                          placeholder="e.g., CEM-001"
                        />
                      </div>
                      <p v-if="errors.code" class="mt-1 text-sm text-red-600">{{ errors.code }}</p>
                    </div>
                  </div>
                  <div class="row">
                    <div class="mb-4 col-sm-6">
                      <label class="mt-4">Description</label>
                      <p class="text-xs form-text text-muted ms-1 d-inline">(optional)</p>
                      <textarea
                        v-model="formData.description"
                        rows="4"
                        class="mt-2 form-control"
                        placeholder="Item description..."
                      ></textarea>
                    </div>
                    <div class="mt-5 col-sm-6 mt-sm-3">
                      <label class="form-control ms-0">Category <span class="text-red-500">*</span></label>
                      <select
                        v-model="formData.category"
                        class="form-control"
                        :class="{ 'border-red-500': errors.category }"
                      >
                        <option value="">Select category</option>
                        <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                      </select>
                      <p v-if="errors.category" class="mt-1 text-sm text-red-600">{{ errors.category }}</p>
                      <label class="mt-3 form-control ms-0">Unit of Measure</label>
                      <select v-model="formData.unit" class="form-control">
                        <option v-for="u in units" :key="u" :value="u">{{ u }}</option>
                      </select>
                    </div>
                  </div>
                  <div class="mt-4 button-row d-flex">
                    <button
                      class="mb-0 btn bg-gradient-dark ms-auto js-btn-next"
                      type="button"
                      @click.prevent="nextStep"
                    >
                      Next
                    </button>
                  </div>
                </div>
              </div>

              <!-- Step 2: Media -->
              <div
                v-show="currentStep === 2"
                class="pt-3 bg-white multisteps-form__panel border-radius-xl"
              >
                <h5 class="mb-4 text-lg font-semibold font-weight-bolder">Media</h5>
                <div class="multisteps-form__content">
                  <div class="mt-3 row">
                    <div class="mb-4 col-12">
                      <label class="mb-0 form-control">Product Image URL</label>
                      <input
                        v-model="formData.imageUrl"
                        type="url"
                        class="mt-2 form-control"
                        placeholder="https://example.com/image.jpg"
                      />
                      <p class="mt-1 text-xs text-gray-500">Enter a URL for the product image</p>
                    </div>
                    <div v-if="formData.imageUrl || getItemImage()" class="col-12">
                      <img
                        :src="formData.imageUrl || getItemImage()"
                        alt="Product preview"
                        class="object-cover rounded-lg shadow"
                        style="max-height: 300px; width: 100%;"
                      />
                    </div>
                  </div>
                  <div class="mt-4 button-row d-flex">
                    <button
                      class="mb-0 btn bg-gradient-light js-btn-prev"
                      type="button"
                      @click.prevent="prevStep"
                    >
                      Prev
                    </button>
                    <button
                      class="mb-0 btn bg-gradient-dark ms-auto js-btn-next"
                      type="button"
                      @click.prevent="nextStep"
                    >
                      Next
                    </button>
                  </div>
                </div>
              </div>

              <!-- Step 3: Stock & Supplier -->
              <div
                v-show="currentStep === 3"
                class="pt-3 bg-white multisteps-form__panel border-radius-xl"
              >
                <h5 class="mb-4 text-lg font-semibold font-weight-bolder">Stock & Supplier</h5>
                <div class="multisteps-form__content">
                  <div class="mt-3 row">
                    <div class="mb-4 col-12 col-sm-4">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Current Stock</label>
                        <input
                          v-model.number="formData.currentStock"
                          type="number"
                          min="0"
                          class="multisteps-form__input form-control"
                          placeholder="0"
                        />
                      </div>
                    </div>
                    <div class="mb-4 col-12 col-sm-4">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Min Stock</label>
                        <input
                          v-model.number="formData.minStock"
                          type="number"
                          min="0"
                          class="multisteps-form__input form-control"
                          :class="{ 'border-red-500': errors.minStock }"
                          placeholder="0"
                        />
                      </div>
                      <p v-if="errors.minStock" class="mt-1 text-sm text-red-600">{{ errors.minStock }}</p>
                    </div>
                    <div class="mb-4 col-12 col-sm-4">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Max Stock</label>
                        <input
                          v-model.number="formData.maxStock"
                          type="number"
                          min="0"
                          class="multisteps-form__input form-control"
                          :class="{ 'border-red-500': errors.maxStock }"
                          placeholder="0 (optional)"
                        />
                      </div>
                      <p v-if="errors.maxStock" class="mt-1 text-sm text-red-600">{{ errors.maxStock }}</p>
                    </div>
                  </div>
                  <div class="row">
                    <div class="mb-4 col-12 col-sm-6">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Barcode</label>
                        <input
                          v-model="formData.barcode"
                          type="text"
                          class="multisteps-form__input form-control"
                          placeholder="Optional"
                        />
                      </div>
                    </div>
                    <div class="mb-4 col-12 col-sm-6">
                      <label class="form-control ms-0">Default Supplier</label>
                      <select
                        v-model="formData.supplier"
                        class="form-control"
                      >
                        <option value="">No supplier</option>
                        <option v-for="supp in suppliers" :key="supp.id" :value="supp.name">
                          {{ supp.name }}
                        </option>
                      </select>
                    </div>
                  </div>
                  <div class="mt-4 button-row d-flex">
                    <button
                      class="mb-0 btn bg-gradient-light js-btn-prev"
                      type="button"
                      @click.prevent="prevStep"
                    >
                      Prev
                    </button>
                    <button
                      class="mb-0 btn bg-gradient-dark ms-auto js-btn-next"
                      type="button"
                      @click.prevent="nextStep"
                    >
                      Next
                    </button>
                  </div>
                </div>
              </div>

              <!-- Step 4: Pricing -->
              <div
                v-show="currentStep === 4"
                class="pt-3 bg-white multisteps-form__panel border-radius-xl h-100"
              >
                <h5 class="mb-4 text-lg font-semibold font-weight-bolder">Pricing</h5>
                <div class="mt-3 multisteps-form__content">
                  <div class="row">
                    <div class="mb-4 col-3">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Cost Price <span class="text-red-500">*</span></label>
                        <input
                          v-model.number="formData.costPrice"
                          type="number"
                          step="0.01"
                          min="0"
                          class="form-control w-100"
                          :class="{ 'border-red-500': errors.costPrice }"
                          placeholder="0.00"
                        />
                      </div>
                      <p v-if="errors.costPrice" class="mt-1 text-sm text-red-600">{{ errors.costPrice }}</p>
                    </div>
                    <div class="mb-4 col-4">
                      <label class="form-control ms-0">Currency</label>
                      <select class="form-control" disabled>
                        <option value="ZAR" selected>ZAR</option>
                      </select>
                    </div>
                    <div class="mb-4 col-5">
                      <div class="input-group input-group-dynamic">
                        <label class="form-label">Selling Price <span class="text-red-500">*</span></label>
                        <input
                          v-model.number="formData.sellingPrice"
                          type="number"
                          step="0.01"
                          min="0"
                          class="multisteps-form__input form-control"
                          :class="{ 'border-red-500': errors.sellingPrice }"
                          placeholder="0.00"
                        />
                      </div>
                      <p v-if="errors.sellingPrice" class="mt-1 text-sm text-red-600">{{ errors.sellingPrice }}</p>
                    </div>
                  </div>
                  <div class="row">
                    <div class="col-12">
                      <label class="mt-4 form-label">Active Status</label>
                      <div class="form-check form-switch ps-0 ms-1">
                        <input
                          v-model="formData.isActive"
                          class="mt-1 form-check-input"
                          type="checkbox"
                          id="isActiveNew"
                        />
                        <label class="form-check-label ms-2" for="isActiveNew">
                          Item is active
                        </label>
                      </div>
                    </div>
                  </div>
                  <div class="mt-0 button-row d-flex mt-md-4">
                    <button
                      class="mb-0 btn bg-gradient-light js-btn-prev"
                      type="button"
                      @click.prevent="prevStep"
                    >
                      Prev
                    </button>
                    <button
                      class="mb-0 btn bg-gradient-dark ms-auto"
                      type="submit"
                      :disabled="isSubmitting"
                    >
                      <span v-if="isSubmitting">Saving...</span>
                      <span v-else>Save</span>
                    </button>
                  </div>
                </div>
              </div>
            </form>
          </div>

          <!-- EDIT ITEM MODAL - Layout matching edit-product.html -->
          <div
            v-else
            class="overflow-y-auto relative w-full max-w-6xl bg-white rounded-xl shadow-xl"
            style="max-height: 90vh;"
            @click.stop
          >
            <!-- Header -->
            <div class="p-6 border-b border-gray-200">
              <div class="flex justify-between items-center">
                <div>
                  <h4 class="mb-1 text-xl font-semibold text-gray-900">Make the changes below</h4>
                  <p class="text-sm text-gray-600">Update your item information</p>
                </div>
                <div class="flex gap-3 items-center">
                  <button
                    @click="handleClose"
                    class="px-4 py-2 text-gray-700 bg-white rounded-lg border border-gray-300 transition-colors hover:bg-gray-50"
                  >
                    Cancel
                  </button>
                  <button
                    @click="handleSave"
                    type="button"
                    :disabled="isSubmitting"
                    class="px-4 py-2 mb-0 btn bg-gradient-dark"
                  >
                    <span v-if="isSubmitting">Saving...</span>
                    <span v-else>Save</span>
                  </button>
                  <button
                    @click="handleClose"
                    class="text-gray-400 transition-colors hover:text-gray-600"
                  >
                    <i class="text-2xl material-symbols-rounded">close</i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Edit Form Content -->
            <form @submit.prevent="handleSave" class="p-6">
              <div class="row">
                <!-- Left Column: Image Card -->
                <div class="col-lg-4">
                  <div class="mt-4 card" data-animation="true">
                    <div class="p-0 mx-3 card-header position-relative mt-n4 z-index-2">
                      <a class="d-block blur-shadow-image">
                        <img
                          :src="formData.imageUrl || getItemImage(props.item?.id)"
                          :alt="formData.name"
                          class="shadow img-fluid border-radius-lg"
                          style="width: 100%; height: 250px; object-fit: cover;"
                        />
                      </a>
                      <div
                        class="colored-shadow"
                        :style="{ backgroundImage: `url('${formData.imageUrl || getItemImage(props.item?.id)}')` }"
                      ></div>
                    </div>
                    <div class="text-center card-body">
                      <div class="mx-auto mt-n6">
                        <button
                          type="button"
                          class="mb-0 btn bg-gradient-dark btn-sm me-2"
                          @click="() => {}"
                        >
                          Edit
                        </button>
                        <button
                          type="button"
                          class="mb-0 btn btn-outline-dark btn-sm"
                          @click="formData.imageUrl = ''"
                        >
                          Remove
                        </button>
                      </div>
                      <h5 class="mt-4 font-weight-normal">Product Image</h5>
                      <div class="mt-3 input-group input-group-dynamic">
                        <label class="form-label">Image URL</label>
                        <input
                          v-model="formData.imageUrl"
                          type="url"
                          class="form-control"
                          placeholder="https://example.com/image.jpg"
                        />
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Right Column: Product Information -->
                <div class="mt-4 col-lg-8 mt-lg-0">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="text-lg font-semibold font-weight-bolder">Product Information</h5>
                      <div class="mt-4 row">
                        <div class="mb-4 col-12 col-sm-6">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Name <span class="text-red-500">*</span></label>
                            <input
                              v-model="formData.name"
                              type="text"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.name }"
                            />
                          </div>
                          <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                        </div>
                        <div class="mt-3 mb-4 col-12 col-sm-6 mt-sm-0">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Item Code <span class="text-red-500">*</span></label>
                            <input
                              v-model="formData.code"
                              type="text"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.code }"
                            />
                          </div>
                          <p v-if="errors.code" class="mt-1 text-sm text-red-600">{{ errors.code }}</p>
                        </div>
                      </div>
                      <div class="mt-4 row">
                        <div class="mb-4 col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Current Stock</label>
                            <input
                              v-model.number="formData.currentStock"
                              type="number"
                              min="0"
                              class="form-control w-100"
                            />
                          </div>
                        </div>
                        <div class="mb-4 col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Cost Price <span class="text-red-500">*</span></label>
                            <input
                              v-model.number="formData.costPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.costPrice }"
                            />
                          </div>
                          <p v-if="errors.costPrice" class="mt-1 text-sm text-red-600">{{ errors.costPrice }}</p>
                        </div>
                        <div class="mb-4 col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Selling Price <span class="text-red-500">*</span></label>
                            <input
                              v-model.number="formData.sellingPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.sellingPrice }"
                            />
                          </div>
                          <p v-if="errors.sellingPrice" class="mt-1 text-sm text-red-600">{{ errors.sellingPrice }}</p>
                        </div>
                        <div class="mb-4 col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Min Stock</label>
                            <input
                              v-model.number="formData.minStock"
                              type="number"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.minStock }"
                            />
                          </div>
                          <p v-if="errors.minStock" class="mt-1 text-sm text-red-600">{{ errors.minStock }}</p>
                        </div>
                      </div>
                      <div class="row">
                        <div class="mb-4 col-sm-6">
                          <label class="mt-4">Description</label>
                          <p class="text-xs form-text text-muted ms-1 d-inline">(optional)</p>
                          <textarea
                            v-model="formData.description"
                            rows="4"
                            class="mt-2 form-control"
                            placeholder="Item description..."
                          ></textarea>
                        </div>
                        <div class="col-sm-6">
                          <label class="mt-4 ms-0">Category <span class="text-red-500">*</span></label>
                          <select
                            v-model="formData.category"
                            class="form-control"
                            :class="{ 'border-red-500': errors.category }"
                          >
                            <option value="">Select category</option>
                            <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                          </select>
                          <p v-if="errors.category" class="mt-1 text-sm text-red-600">{{ errors.category }}</p>
                          <label class="mt-3 ms-0">Unit of Measure</label>
                          <select v-model="formData.unit" class="form-control">
                            <option v-for="u in units" :key="u" :value="u">{{ u }}</option>
                          </select>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Additional Info Row -->
              <div class="mt-4 row">
                <div class="col-sm-4">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="text-lg font-semibold font-weight-bolder">Additional Info</h5>
                      <div class="mt-3 input-group input-group-dynamic">
                        <label class="form-label">Barcode</label>
                        <input
                          v-model="formData.barcode"
                          type="text"
                          class="form-control w-100"
                          placeholder="Optional"
                        />
                      </div>
                      <label class="mt-3 form-control ms-0">Default Supplier</label>
                      <select v-model="formData.supplier" class="form-control">
                        <option value="">No supplier</option>
                        <option v-for="supp in suppliers" :key="supp.id" :value="supp.name">
                          {{ supp.name }}
                        </option>
                      </select>
                    </div>
                  </div>
                </div>
                <div class="mt-4 col-sm-8 mt-sm-0">
                  <div class="card">
                    <div class="card-body">
                      <div class="row">
                        <h5 class="mb-3 text-lg font-semibold font-weight-bolder">Pricing & Status</h5>
                        <div class="mb-4 col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Cost Price</label>
                            <input
                              v-model.number="formData.costPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.costPrice }"
                            />
                          </div>
                        </div>
                        <div class="mb-4 col-4">
                          <label class="form-control ms-0">Currency</label>
                          <select class="form-control" disabled>
                            <option value="ZAR" selected>ZAR</option>
                          </select>
                        </div>
                        <div class="mb-4 col-5">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Selling Price</label>
                            <input
                              v-model.number="formData.sellingPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.sellingPrice }"
                            />
                          </div>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-12">
                          <label class="mt-4 form-label">Active Status</label>
                          <div class="form-check form-switch ps-0 ms-1">
                            <input
                              v-model="formData.isActive"
                              class="mt-1 form-check-input"
                              type="checkbox"
                              id="isActiveEdit"
                            />
                            <label class="form-check-label ms-2" for="isActiveEdit">
                              Item is active
                            </label>
                          </div>
                        </div>
                      </div>
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

.multisteps-form__progress {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.multisteps-form__progress-btn {
  flex: 1;
  min-width: 120px;
  text-align: center;
  border: none;
  cursor: pointer;
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

@media (max-width: 768px) {
  .multisteps-form__progress {
    flex-direction: column;
  }
  
  .multisteps-form__progress-btn {
    width: 100%;
  }
}
</style>