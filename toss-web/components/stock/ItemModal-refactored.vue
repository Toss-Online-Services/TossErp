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
          <!-- Modal Container - Matching edit-product.html structure -->
          <div
            class="overflow-y-auto relative w-full max-w-6xl bg-white rounded-xl shadow-xl"
            style="max-height: 90vh;"
            @click.stop
          >
            <!-- Header Row - Matching template -->
            <div class="p-6 border-b border-gray-200">
              <div class="row">
                <div class="col-lg-6">
                  <h4 class="mb-1 text-xl font-semibold text-gray-900">
                    {{ isEditing ? 'Make the changes below' : 'Add new item' }}
                  </h4>
                  <p class="text-sm text-gray-600">
                    {{ isEditing ? 'Update your item information' : 'This information will let us know more about your item.' }}
                  </p>
                </div>
                <div class="col-lg-6 text-right d-flex flex-column justify-content-center">
                  <button
                    @click="handleSave"
                    type="button"
                    :disabled="isSubmitting"
                    class="btn bg-gradient-dark mb-0 ms-lg-auto me-lg-0 me-auto mt-lg-0 mt-2"
                  >
                    <span v-if="isSubmitting">Saving...</span>
                    <span v-else>Save</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- Form Content - Matching edit-product.html layout -->
            <form @submit.prevent="handleSave" class="p-6">
              <div class="row mt-4">
                <!-- Left Column: Image Card (col-lg-4) -->
                <div class="col-lg-4">
                  <div class="card mt-4" data-animation="true">
                    <div class="card-header p-0 position-relative mt-n4 mx-3 z-index-2">
                      <a class="d-block blur-shadow-image">
                        <img
                          :src="formData.imageUrl || getItemImage(props.item?.id)"
                          :alt="formData.name || 'Product'"
                          class="img-fluid shadow border-radius-lg"
                          style="width: 100%; height: 250px; object-fit: cover;"
                        />
                      </a>
                      <div
                        class="colored-shadow"
                        :style="{ backgroundImage: `url('${formData.imageUrl || getItemImage(props.item?.id)}')` }"
                      ></div>
                    </div>
                    <div class="card-body text-center">
                      <div class="mt-n6 mx-auto">
                        <button
                          type="button"
                          class="btn bg-gradient-dark btn-sm mb-0 me-2"
                          @click="() => {}"
                        >
                          Edit
                        </button>
                        <button
                          type="button"
                          class="btn btn-outline-dark btn-sm mb-0"
                          @click="formData.imageUrl = ''"
                        >
                          Remove
                        </button>
                      </div>
                      <h5 class="font-weight-normal mt-4">Product Image</h5>
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

                <!-- Right Column: Product Information (col-lg-8) -->
                <div class="col-lg-8 mt-lg-0 mt-4">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="font-weight-bolder">Product Information</h5>
                      <div class="row mt-4">
                        <div class="col-12 col-sm-6">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Name <span class="text-red-500">*</span></label>
                            <input
                              v-model="formData.name"
                              type="text"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.name }"
                              placeholder="Item name"
                            />
                          </div>
                          <p v-if="errors.name" class="mt-1 text-sm text-red-600">{{ errors.name }}</p>
                        </div>
                        <div class="col-12 col-sm-6 mt-3 mt-sm-0">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Item Code <span class="text-red-500">*</span></label>
                            <input
                              v-model="formData.code"
                              type="text"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.code }"
                              placeholder="e.g., CEM-001"
                            />
                          </div>
                          <p v-if="errors.code" class="mt-1 text-sm text-red-600">{{ errors.code }}</p>
                        </div>
                      </div>
                      <div class="row mt-4">
                        <div class="col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Current Stock</label>
                            <input
                              v-model.number="formData.currentStock"
                              type="number"
                              min="0"
                              class="form-control w-100"
                              placeholder="0"
                            />
                          </div>
                        </div>
                        <div class="col-3">
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
                        <div class="col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Selling Price <span class="text-red-500">*</span></label>
                            <input
                              v-model.number="formData.sellingPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.sellingPrice }"
                              placeholder="0.00"
                            />
                          </div>
                          <p v-if="errors.sellingPrice" class="mt-1 text-sm text-red-600">{{ errors.sellingPrice }}</p>
                        </div>
                        <div class="col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Min Stock</label>
                            <input
                              v-model.number="formData.minStock"
                              type="number"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.minStock }"
                              placeholder="0"
                            />
                          </div>
                          <p v-if="errors.minStock" class="mt-1 text-sm text-red-600">{{ errors.minStock }}</p>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-sm-6">
                          <label class="mt-4">Description</label>
                          <p class="form-text text-muted text-xs ms-1 d-inline">(optional)</p>
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

              <!-- Second Row: Additional Info and Pricing -->
              <div class="row mt-4">
                <!-- Additional Info Card (col-sm-4) -->
                <div class="col-sm-4">
                  <div class="card">
                    <div class="card-body">
                      <h5 class="font-weight-bolder">Additional Info</h5>
                      <div class="input-group input-group-dynamic mt-3">
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

                <!-- Pricing Card (col-sm-8) -->
                <div class="col-sm-8 mt-sm-0 mt-4">
                  <div class="card">
                    <div class="card-body">
                      <div class="row">
                        <h5 class="font-weight-bolder mb-3">Pricing & Status</h5>
                        <div class="col-3">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Cost Price</label>
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
                        </div>
                        <div class="col-4">
                          <label class="form-control ms-0">Currency</label>
                          <select class="form-control" disabled>
                            <option value="ZAR" selected>ZAR</option>
                          </select>
                        </div>
                        <div class="col-5">
                          <div class="input-group input-group-dynamic">
                            <label class="form-label">Selling Price</label>
                            <input
                              v-model.number="formData.sellingPrice"
                              type="number"
                              step="0.01"
                              min="0"
                              class="form-control w-100"
                              :class="{ 'border-red-500': errors.sellingPrice }"
                              placeholder="0.00"
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
                              :id="isEditing ? 'isActiveEdit' : 'isActiveNew'"
                            />
                            <label class="form-check-label ms-2" :for="isEditing ? 'isActiveEdit' : 'isActiveNew'">
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

            <!-- Close Button -->
            <button
              @click="handleClose"
              class="absolute top-4 right-4 z-10 text-gray-400 transition-colors hover:text-gray-600"
            >
              <i class="text-2xl material-symbols-rounded">close</i>
            </button>
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
</style>
