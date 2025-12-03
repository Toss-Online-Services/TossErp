<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useBuyingStore, type PurchaseOrder, type PurchaseOrderItem } from '~/stores/buying'
import { useStockStore, type Item } from '~/stores/stock'

interface Props {
  show: boolean
  purchaseOrder?: PurchaseOrder | null
}

const props = withDefaults(defineProps<Props>(), {
  purchaseOrder: null
})

const emit = defineEmits<{
  close: []
  saved: [po: PurchaseOrder]
}>()

const buyingStore = useBuyingStore()
const stockStore = useStockStore()

// Form state
const formData = ref({
  supplierId: '',
  orderDate: new Date().toISOString().split('T')[0],
  expectedDeliveryDate: '',
  requiredDate: '',
  notes: '',
  items: [] as PurchaseOrderItem[]
})

const isEditing = computed(() => !!props.purchaseOrder)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})
const showAddItemModal = ref(false)
const selectedItemForAdd = ref<Item | null>(null)
const itemQuantity = ref(1)
const itemRate = ref(0)

// Computed
const subtotal = computed(() => {
  return formData.value.items.reduce((sum, item) => sum + item.amount, 0)
})

const taxAmount = computed(() => {
  return subtotal.value * 0.15 // 15% VAT
})

const shippingCost = computed(() => {
  return 0 // Can be made editable
})

const total = computed(() => {
  return subtotal.value + taxAmount.value + shippingCost.value
})

// Watch for purchase order changes
watch(() => props.purchaseOrder, (newPO) => {
  if (newPO) {
    formData.value = {
      supplierId: newPO.supplierId,
      orderDate: new Date(newPO.orderDate).toISOString().split('T')[0],
      expectedDeliveryDate: newPO.expectedDeliveryDate ? new Date(newPO.expectedDeliveryDate).toISOString().split('T')[0] : '',
      requiredDate: newPO.requiredDate ? new Date(newPO.requiredDate).toISOString().split('T')[0] : '',
      notes: newPO.notes || '',
      items: [...newPO.items]
    }
  } else {
    resetForm()
  }
}, { immediate: true })

watch(() => props.show, (isShowing) => {
  if (isShowing) {
    buyingStore.fetchSuppliers()
    stockStore.fetchItems()
  }
})

function resetForm() {
  formData.value = {
    supplierId: '',
    orderDate: new Date().toISOString().split('T')[0],
    expectedDeliveryDate: '',
    requiredDate: '',
    notes: '',
    items: []
  }
  errors.value = {}
  selectedItemForAdd.value = null
  itemQuantity.value = 1
  itemRate.value = 0
}

function validate() {
  errors.value = {}
  
  if (!formData.value.supplierId) {
    errors.value.supplierId = 'Supplier is required'
  }
  
  if (!formData.value.orderDate) {
    errors.value.orderDate = 'Order date is required'
  }
  
  if (formData.value.items.length === 0) {
    errors.value.items = 'At least one item is required'
  }
  
  return Object.keys(errors.value).length === 0
}

function handleAddItem() {
  if (!selectedItemForAdd.value) return
  
  const item = selectedItemForAdd.value
  const quantity = itemQuantity.value
  const rate = itemRate.value || item.costPrice
  const amount = quantity * rate
  
  const poItem: PurchaseOrderItem = {
    id: `item_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
    itemId: item.id,
    itemCode: item.code,
    itemName: item.name,
    quantity,
    unit: item.unit,
    rate,
    amount,
    receivedQuantity: 0,
    rejectedQuantity: 0
  }
  
  formData.value.items.push(poItem)
  showAddItemModal.value = false
  selectedItemForAdd.value = null
  itemQuantity.value = 1
  itemRate.value = 0
}

function handleRemoveItem(itemId: string) {
  const index = formData.value.items.findIndex(item => item.id === itemId)
  if (index !== -1) {
    formData.value.items.splice(index, 1)
  }
}

function handleUpdateItemQuantity(itemId: string, quantity: number) {
  const item = formData.value.items.find(i => i.id === itemId)
  if (item && quantity > 0) {
    item.quantity = quantity
    item.amount = item.quantity * item.rate
  }
}

function handleUpdateItemRate(itemId: string, rate: number) {
  const item = formData.value.items.find(i => i.id === itemId)
  if (item && rate >= 0) {
    item.rate = rate
    item.amount = item.quantity * item.rate
  }
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const poData = {
      supplierId: formData.value.supplierId,
      supplierName: buyingStore.getSupplierById(formData.value.supplierId)?.name || '',
      orderDate: new Date(formData.value.orderDate),
      expectedDeliveryDate: formData.value.expectedDeliveryDate ? new Date(formData.value.expectedDeliveryDate) : undefined,
      requiredDate: formData.value.requiredDate ? new Date(formData.value.requiredDate) : undefined,
      status: 'draft' as const,
      subtotal: subtotal.value,
      taxAmount: taxAmount.value,
      shippingCost: shippingCost.value,
      total: total.value,
      notes: formData.value.notes,
      items: formData.value.items
    }
    
    if (isEditing.value && props.purchaseOrder) {
      await buyingStore.updatePurchaseOrder(props.purchaseOrder.id, poData)
      emit('saved', { ...props.purchaseOrder, ...poData } as PurchaseOrder)
    } else {
      const newPO = await buyingStore.createPurchaseOrder(poData)
      emit('saved', newPO)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save purchase order:', error)
    alert('Failed to save purchase order. Please try again.')
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
            class="relative w-full max-w-4xl rounded-xl bg-white shadow-xl max-h-[90vh] flex flex-col"
            @click.stop
          >
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-xl font-bold text-gray-900">
                {{ isEditing ? 'Edit Purchase Order' : 'Create Purchase Order' }}
              </h3>
              <button
                @click="handleClose"
                class="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <i class="material-symbols-rounded text-2xl">close</i>
              </button>
            </div>

            <!-- Form -->
            <form @submit.prevent="handleSave" class="flex-1 overflow-y-auto p-6 space-y-6">
              <!-- Supplier and Dates -->
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <!-- Supplier -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Supplier <span class="text-red-500">*</span>
                  </label>
                  <select
                    v-model="formData.supplierId"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.supplierId }"
                  >
                    <option value="">Select supplier...</option>
                    <option v-for="supplier in buyingStore.activeSuppliers" :key="supplier.id" :value="supplier.id">
                      {{ supplier.name }}
                    </option>
                  </select>
                  <p v-if="errors.supplierId" class="mt-1 text-sm text-red-600">{{ errors.supplierId }}</p>
                </div>

                <!-- Order Date -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Order Date <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="formData.orderDate"
                    type="date"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.orderDate }"
                  >
                  <p v-if="errors.orderDate" class="mt-1 text-sm text-red-600">{{ errors.orderDate }}</p>
                </div>

                <!-- Expected Delivery Date -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Expected Delivery Date
                  </label>
                  <input
                    v-model="formData.expectedDeliveryDate"
                    type="date"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                </div>
              </div>

              <!-- Items Section -->
              <div>
                <div class="flex items-center justify-between mb-4">
                  <label class="block text-sm font-medium text-gray-700">
                    Items <span class="text-red-500">*</span>
                  </label>
                  <button
                    type="button"
                    @click="showAddItemModal = true"
                    class="inline-flex items-center gap-2 px-3 py-1.5 text-sm bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
                  >
                    <i class="material-symbols-rounded text-lg">add</i>
                    <span>Add Item</span>
                  </button>
                </div>
                
                <p v-if="errors.items" class="mb-2 text-sm text-red-600">{{ errors.items }}</p>

                <div v-if="formData.items.length === 0" class="p-8 text-center border-2 border-dashed border-gray-300 rounded-lg">
                  <i class="material-symbols-rounded text-4xl text-gray-400 mb-2">inventory_2</i>
                  <p class="text-gray-600">No items added yet</p>
                  <p class="text-sm text-gray-500 mt-1">Click "Add Item" to start</p>
                </div>

                <div v-else class="border border-gray-200 rounded-lg overflow-hidden">
                  <table class="min-w-full divide-y divide-gray-200">
                    <thead class="bg-gray-50">
                      <tr>
                        <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Item</th>
                        <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Quantity</th>
                        <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Rate</th>
                        <th class="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Amount</th>
                        <th class="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase">Actions</th>
                      </tr>
                    </thead>
                    <tbody class="bg-white divide-y divide-gray-200">
                      <tr v-for="item in formData.items" :key="item.id">
                        <td class="px-4 py-3">
                          <div class="text-sm font-medium text-gray-900">{{ item.itemName }}</div>
                          <div class="text-sm text-gray-500">{{ item.itemCode }}</div>
                        </td>
                        <td class="px-4 py-3">
                          <input
                            type="number"
                            :value="item.quantity"
                            @input="handleUpdateItemQuantity(item.id, Number(($event.target as HTMLInputElement).value))"
                            min="1"
                            class="w-20 px-2 py-1 border border-gray-300 rounded focus:outline-none focus:border-gray-900"
                          >
                          <span class="ml-2 text-sm text-gray-500">{{ item.unit }}</span>
                        </td>
                        <td class="px-4 py-3">
                          <div class="flex items-center">
                            <span class="text-sm text-gray-500 mr-1">R</span>
                            <input
                              type="number"
                              :value="item.rate"
                              @input="handleUpdateItemRate(item.id, Number(($event.target as HTMLInputElement).value))"
                              min="0"
                              step="0.01"
                              class="w-24 px-2 py-1 border border-gray-300 rounded focus:outline-none focus:border-gray-900"
                            >
                          </div>
                        </td>
                        <td class="px-4 py-3 text-right text-sm font-medium text-gray-900">
                          R {{ item.amount.toFixed(2) }}
                        </td>
                        <td class="px-4 py-3 text-center">
                          <button
                            type="button"
                            @click="handleRemoveItem(item.id)"
                            class="p-1 text-red-600 hover:text-red-800 hover:bg-red-50 rounded transition-colors"
                            title="Remove"
                          >
                            <i class="material-symbols-rounded text-lg">delete</i>
                          </button>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Totals -->
              <div class="border-t border-gray-200 pt-4">
                <div class="flex justify-end">
                  <div class="w-64 space-y-2">
                    <div class="flex justify-between text-sm">
                      <span class="text-gray-600">Subtotal:</span>
                      <span class="text-gray-900">R {{ subtotal.toFixed(2) }}</span>
                    </div>
                    <div class="flex justify-between text-sm">
                      <span class="text-gray-600">Tax (15%):</span>
                      <span class="text-gray-900">R {{ taxAmount.toFixed(2) }}</span>
                    </div>
                    <div class="flex justify-between text-sm">
                      <span class="text-gray-600">Shipping:</span>
                      <span class="text-gray-900">R {{ shippingCost.toFixed(2) }}</span>
                    </div>
                    <div class="flex justify-between text-lg font-bold border-t border-gray-200 pt-2">
                      <span class="text-gray-900">Total:</span>
                      <span class="text-gray-900">R {{ total.toFixed(2) }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Notes -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Notes
                </label>
                <textarea
                  v-model="formData.notes"
                  rows="3"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  placeholder="Additional notes (optional)"
                ></textarea>
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
                  class="px-4 py-2 text-white bg-gray-900 rounded-lg hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <span v-if="isSubmitting">Saving...</span>
                  <span v-else>{{ isEditing ? 'Update' : 'Create' }} Purchase Order</span>
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Add Item Modal -->
    <Transition name="modal">
      <div
        v-if="showAddItemModal"
        class="fixed inset-0 z-[60] overflow-y-auto"
        @click.self="showAddItemModal = false"
      >
        <div class="flex min-h-screen items-center justify-center p-4">
          <div
            class="relative w-full max-w-md rounded-xl bg-white shadow-xl"
            @click.stop
          >
            <div class="flex items-center justify-between border-b border-gray-200 px-6 py-4">
              <h3 class="text-lg font-bold text-gray-900">Add Item</h3>
              <button
                @click="showAddItemModal = false"
                class="text-gray-400 hover:text-gray-600 transition-colors"
              >
                <i class="material-symbols-rounded text-2xl">close</i>
              </button>
            </div>

            <div class="p-6 space-y-4">
              <!-- Item Selection -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Select Item
                </label>
                <select
                  v-model="selectedItemForAdd"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                >
                  <option :value="null">Select an item...</option>
                  <option
                    v-for="item in stockStore.items"
                    :key="item.id"
                    :value="item"
                  >
                    {{ item.name }} ({{ item.code }}) - R{{ item.costPrice.toFixed(2) }}
                  </option>
                </select>
              </div>

              <!-- Quantity -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Quantity
                </label>
                <input
                  v-model.number="itemQuantity"
                  type="number"
                  min="1"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                >
              </div>

              <!-- Rate -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Rate (per {{ selectedItemForAdd?.unit || 'unit' }})
                </label>
                <div class="flex items-center">
                  <span class="text-sm text-gray-500 mr-2">R</span>
                  <input
                    v-model.number="itemRate"
                    type="number"
                    min="0"
                    step="0.01"
                    :placeholder="selectedItemForAdd ? selectedItemForAdd.costPrice.toString() : '0.00'"
                    class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                </div>
                <p v-if="selectedItemForAdd" class="mt-1 text-xs text-gray-500">
                  Default cost price: R{{ selectedItemForAdd.costPrice.toFixed(2) }}
                </p>
              </div>

              <!-- Actions -->
              <div class="flex justify-end gap-3 pt-4 border-t border-gray-200">
                <button
                  type="button"
                  @click="showAddItemModal = false"
                  class="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="button"
                  @click="handleAddItem"
                  :disabled="!selectedItemForAdd || itemQuantity <= 0"
                  class="px-4 py-2 text-white bg-gray-900 rounded-lg hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  Add Item
                </button>
              </div>
            </div>
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
</style>

