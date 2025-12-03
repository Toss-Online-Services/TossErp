<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useBuyingStore, type GoodsReceipt, type GoodsReceiptItem, type PurchaseOrder } from '~/stores/buying'

interface Props {
  show: boolean
  goodsReceipt?: GoodsReceipt | null
}

const props = withDefaults(defineProps<Props>(), {
  goodsReceipt: null
})

const emit = defineEmits<{
  close: []
  saved: [gr: GoodsReceipt]
}>()

const buyingStore = useBuyingStore()

// Form state
const formData = ref({
  purchaseOrderId: '',
  supplierId: '',
  receiptDate: new Date().toISOString().split('T')[0],
  warehouse: 'main',
  status: 'draft' as const,
  notes: '',
  items: [] as GoodsReceiptItem[]
})

const isEditing = computed(() => !!props.goodsReceipt)
const isSubmitting = ref(false)
const errors = ref<Record<string, string>>({})
const selectedPO = ref<PurchaseOrder | null>(null)

// Computed
const availablePOs = computed(() => {
  return buyingStore.purchaseOrders.filter(po => 
    po.status === 'approved' || po.status === 'submitted' || po.status === 'partially_received'
  )
})

const subtotal = computed(() => {
  return formData.value.items.reduce((sum, item) => sum + item.amount, 0)
})

const taxAmount = computed(() => {
  return subtotal.value * 0.15 // 15% VAT
})

const total = computed(() => {
  return subtotal.value + taxAmount.value
})

// Watch for purchase order selection
watch(() => formData.value.purchaseOrderId, (poId) => {
  if (poId) {
    const po = buyingStore.getPurchaseOrderById(poId)
    selectedPO.value = po || null
    
    if (po) {
      formData.value.supplierId = po.supplierId
      
      // Pre-populate items from PO
      formData.value.items = po.items.map(poItem => ({
        id: `item_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
        purchaseOrderId: po.id,
        purchaseOrderItemId: poItem.id,
        itemId: poItem.itemId,
        itemCode: poItem.itemCode,
        itemName: poItem.itemName,
        orderedQuantity: poItem.quantity,
        receivedQuantity: poItem.quantity - poItem.receivedQuantity, // Remaining to receive
        rejectedQuantity: 0,
        rate: poItem.rate,
        amount: (poItem.quantity - poItem.receivedQuantity) * poItem.rate,
        notes: ''
      }))
    }
  } else {
    selectedPO.value = null
    formData.value.items = []
  }
})

// Watch for goods receipt changes
watch(() => props.goodsReceipt, (newGR) => {
  if (newGR) {
    formData.value = {
      purchaseOrderId: newGR.purchaseOrderId || '',
      supplierId: newGR.supplierId,
      receiptDate: new Date(newGR.receiptDate).toISOString().split('T')[0],
      warehouse: newGR.warehouse,
      status: newGR.status,
      notes: newGR.notes || '',
      items: [...newGR.items]
    }
    if (newGR.purchaseOrderId) {
      selectedPO.value = buyingStore.getPurchaseOrderById(newGR.purchaseOrderId) || null
    }
  } else {
    resetForm()
  }
}, { immediate: true })

watch(() => props.show, (isShowing) => {
  if (isShowing) {
    buyingStore.fetchPurchaseOrders()
  }
})

function resetForm() {
  formData.value = {
    purchaseOrderId: '',
    supplierId: '',
    receiptDate: new Date().toISOString().split('T')[0],
    warehouse: 'main',
    status: 'draft',
    notes: '',
    items: []
  }
  errors.value = {}
  selectedPO.value = null
}

function validate() {
  errors.value = {}
  
  if (!formData.value.supplierId) {
    errors.value.supplierId = 'Supplier is required'
  }
  
  if (!formData.value.receiptDate) {
    errors.value.receiptDate = 'Receipt date is required'
  }
  
  if (formData.value.items.length === 0) {
    errors.value.items = 'At least one item is required'
  }
  
  // Validate received quantities
  for (const item of formData.value.items) {
    if (item.receivedQuantity < 0) {
      errors.value.items = 'Received quantity cannot be negative'
      break
    }
    if (item.receivedQuantity > item.orderedQuantity) {
      errors.value.items = 'Received quantity cannot exceed ordered quantity'
      break
    }
  }
  
  return Object.keys(errors.value).length === 0
}

function handleUpdateReceivedQuantity(itemId: string, quantity: number) {
  const item = formData.value.items.find(i => i.id === itemId)
  if (item && quantity >= 0 && quantity <= item.orderedQuantity) {
    item.receivedQuantity = quantity
    item.amount = item.receivedQuantity * item.rate
  }
}

function handleUpdateRejectedQuantity(itemId: string, quantity: number) {
  const item = formData.value.items.find(i => i.id === itemId)
  if (item && quantity >= 0) {
    item.rejectedQuantity = quantity
  }
}

function handleRemoveItem(itemId: string) {
  const index = formData.value.items.findIndex(item => item.id === itemId)
  if (index !== -1) {
    formData.value.items.splice(index, 1)
  }
}

async function handleSave() {
  if (!validate()) return
  
  isSubmitting.value = true
  try {
    const supplier = buyingStore.getSupplierById(formData.value.supplierId)
    const po = selectedPO.value
    
    const grData = {
      purchaseOrderId: formData.value.purchaseOrderId || undefined,
      purchaseOrderNumber: po?.poNumber,
      supplierId: formData.value.supplierId,
      supplierName: supplier?.name || '',
      receiptDate: new Date(formData.value.receiptDate),
      warehouse: formData.value.warehouse,
      status: formData.value.status,
      subtotal: subtotal.value,
      taxAmount: taxAmount.value,
      total: total.value,
      notes: formData.value.notes || undefined,
      items: formData.value.items
    }
    
    if (isEditing.value && props.goodsReceipt) {
      // TODO: Implement update goods receipt
      alert('Update functionality not yet implemented')
      emit('close')
    } else {
      const newGR = await buyingStore.createGoodsReceipt(grData)
      emit('saved', newGR)
    }
    emit('close')
    resetForm()
  } catch (error) {
    console.error('Failed to save goods receipt:', error)
    alert('Failed to save goods receipt. Please try again.')
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
                {{ isEditing ? 'Edit Goods Receipt' : 'Create Goods Receipt' }}
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
              <!-- Purchase Order and Dates -->
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <!-- Purchase Order -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Purchase Order
                  </label>
                  <select
                    v-model="formData.purchaseOrderId"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :disabled="isEditing"
                  >
                    <option value="">Select purchase order...</option>
                    <option v-for="po in availablePOs" :key="po.id" :value="po.id">
                      {{ po.poNumber }} - {{ po.supplierName }}
                    </option>
                  </select>
                  <p v-if="selectedPO" class="mt-1 text-xs text-gray-500">
                    Expected: {{ selectedPO.expectedDeliveryDate ? new Date(selectedPO.expectedDeliveryDate).toLocaleDateString() : 'N/A' }}
                  </p>
                </div>

                <!-- Receipt Date -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Receipt Date <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="formData.receiptDate"
                    type="date"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                    :class="{ 'border-red-500': errors.receiptDate }"
                  >
                  <p v-if="errors.receiptDate" class="mt-1 text-sm text-red-600">{{ errors.receiptDate }}</p>
                </div>

                <!-- Warehouse -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Warehouse
                  </label>
                  <select
                    v-model="formData.warehouse"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                    <option value="main">Main Warehouse</option>
                    <option value="secondary">Secondary Warehouse</option>
                  </select>
                </div>
              </div>

              <!-- Items Section -->
              <div>
                <div class="flex items-center justify-between mb-4">
                  <label class="block text-sm font-medium text-gray-700">
                    Received Items <span class="text-red-500">*</span>
                  </label>
                </div>
                
                <p v-if="errors.items" class="mb-2 text-sm text-red-600">{{ errors.items }}</p>

                <div v-if="formData.items.length === 0" class="p-8 text-center border-2 border-dashed border-gray-300 rounded-lg">
                  <i class="material-symbols-rounded text-4xl text-gray-400 mb-2">inventory_2</i>
                  <p class="text-gray-600">No items to receive</p>
                  <p class="text-sm text-gray-500 mt-1">Select a purchase order to load items</p>
                </div>

                <div v-else class="border border-gray-200 rounded-lg overflow-hidden">
                  <div class="overflow-x-auto">
                    <table class="min-w-full divide-y divide-gray-200">
                      <thead class="bg-gray-50">
                        <tr>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Item</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ordered</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Received</th>
                          <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Rejected</th>
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
                            <span class="text-sm text-gray-900">{{ item.orderedQuantity }}</span>
                          </td>
                          <td class="px-4 py-3">
                            <input
                              type="number"
                              :value="item.receivedQuantity"
                              @input="handleUpdateReceivedQuantity(item.id, Number(($event.target as HTMLInputElement).value))"
                              min="0"
                              :max="item.orderedQuantity"
                              class="w-20 px-2 py-1 border border-gray-300 rounded focus:outline-none focus:border-gray-900"
                            >
                          </td>
                          <td class="px-4 py-3">
                            <input
                              type="number"
                              :value="item.rejectedQuantity"
                              @input="handleUpdateRejectedQuantity(item.id, Number(($event.target as HTMLInputElement).value))"
                              min="0"
                              class="w-20 px-2 py-1 border border-gray-300 rounded focus:outline-none focus:border-gray-900"
                            >
                          </td>
                          <td class="px-4 py-3">
                            <span class="text-sm text-gray-900">R {{ item.rate.toFixed(2) }}</span>
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
                    <div class="flex justify-between text-lg font-bold border-t border-gray-200 pt-2">
                      <span class="text-gray-900">Total:</span>
                      <span class="text-gray-900">R {{ total.toFixed(2) }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Status and Notes -->
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Status
                  </label>
                  <select
                    v-model="formData.status"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900"
                  >
                    <option value="draft">Draft</option>
                    <option value="submitted">Submitted</option>
                  </select>
                </div>
              </div>

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
                  <span v-else>{{ isEditing ? 'Update' : 'Create' }} Goods Receipt</span>
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
</style>

