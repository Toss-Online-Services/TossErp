<template>
  <Transition name="modal">
    <div v-if="show" class="fixed inset-0 z-50 overflow-y-auto">
      <!-- Backdrop -->
      <div class="fixed inset-0 bg-black/50 backdrop-blur-sm" @click="close"></div>
      
      <!-- Modal -->
      <div class="flex min-h-full items-center justify-center p-4">
        <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
          
          <!-- Header -->
          <div class="bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-4 flex items-center justify-between">
            <div class="flex items-center space-x-3">
              <div class="p-2 bg-white/20 rounded-lg">
                <DocumentTextIcon class="w-6 h-6 text-white" />
              </div>
              <div>
                <h2 class="text-xl font-bold text-white">Order Details</h2>
                <p class="text-sm text-white/80">{{ order?.number }}</p>
              </div>
            </div>
            <button @click="close" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
              <XMarkIcon class="w-6 h-6 text-white" />
            </button>
          </div>

          <!-- Content -->
          <div class="flex-1 overflow-y-auto p-6 space-y-6">
            
            <!-- Order Info -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Left Column -->
              <div class="space-y-4">
                <div>
                  <label class="text-sm font-medium text-slate-600 dark:text-slate-400">Order Number</label>
                  <p class="text-lg font-bold text-slate-900 dark:text-white">{{ order?.number }}</p>
                </div>
                <div>
                  <label class="text-sm font-medium text-slate-600 dark:text-slate-400">Supplier</label>
                  <p class="text-base text-slate-900 dark:text-white">{{ order?.supplier }}</p>
                </div>
                <div>
                  <label class="text-sm font-medium text-slate-600 dark:text-slate-400">Status</label>
                  <div>
                    <span :class="['inline-block px-3 py-1 rounded-full text-sm font-medium', getStatusClass(order?.status)]">
                      {{ getStatusLabel(order?.status) }}
                    </span>
                  </div>
                </div>
              </div>

              <!-- Right Column -->
              <div class="space-y-4">
                <div>
                  <label class="text-sm font-medium text-slate-600 dark:text-slate-400">Order Date</label>
                  <p class="text-base text-slate-900 dark:text-white">{{ formatDate(order?.orderDate) }}</p>
                </div>
                <div>
                  <label class="text-sm font-medium text-slate-600 dark:text-slate-400">Expected Delivery</label>
                  <p class="text-base text-slate-900 dark:text-white">{{ formatDate(order?.expectedDelivery) }}</p>
                </div>
                <div>
                  <label class="text-sm font-medium text-slate-600 dark:text-slate-400">Payment Terms</label>
                  <p class="text-base text-slate-900 dark:text-white">{{ order?.paymentTerms }}</p>
                </div>
              </div>
            </div>

            <!-- Items Table -->
            <div>
              <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-4">Order Items</h3>
              <div class="bg-slate-50 dark:bg-slate-900 rounded-xl overflow-hidden">
                <table class="w-full">
                  <thead class="bg-slate-100 dark:bg-slate-800">
                    <tr>
                      <th class="px-4 py-3 text-left text-sm font-semibold text-slate-700 dark:text-slate-300">Item</th>
                      <th class="px-4 py-3 text-left text-sm font-semibold text-slate-700 dark:text-slate-300">SKU</th>
                      <th class="px-4 py-3 text-right text-sm font-semibold text-slate-700 dark:text-slate-300">Qty</th>
                      <th class="px-4 py-3 text-right text-sm font-semibold text-slate-700 dark:text-slate-300">Price</th>
                      <th class="px-4 py-3 text-right text-sm font-semibold text-slate-700 dark:text-slate-300">Total</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
                    <tr v-for="(item, index) in orderItems" :key="index">
                      <td class="px-4 py-3 text-sm text-slate-900 dark:text-white font-medium">{{ item.name }}</td>
                      <td class="px-4 py-3 text-sm text-slate-600 dark:text-slate-400">{{ item.sku }}</td>
                      <td class="px-4 py-3 text-sm text-slate-900 dark:text-white text-right">{{ item.quantity }}</td>
                      <td class="px-4 py-3 text-sm text-slate-900 dark:text-white text-right">R{{ item.price.toFixed(2) }}</td>
                      <td class="px-4 py-3 text-sm font-medium text-slate-900 dark:text-white text-right">
                        R{{ (item.price * item.quantity).toFixed(2) }}
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

            <!-- Totals -->
            <div class="bg-gradient-to-br from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 rounded-xl p-6">
              <div class="space-y-2 max-w-md ml-auto">
                <div class="flex justify-between text-sm">
                  <span class="text-slate-600 dark:text-slate-400">Subtotal:</span>
                  <span class="font-medium text-slate-900 dark:text-white">R{{ subtotal.toFixed(2) }}</span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-slate-600 dark:text-slate-400">Delivery Fee:</span>
                  <span class="font-medium text-slate-900 dark:text-white">R{{ deliveryFee.toFixed(2) }}</span>
                </div>
                <div class="flex justify-between text-lg font-bold pt-2 border-t-2 border-blue-200 dark:border-blue-800">
                  <span class="text-slate-900 dark:text-white">Total:</span>
                  <span class="text-blue-600">R{{ order?.totalAmount?.toFixed(2) || '0.00' }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Footer Actions -->
          <div class="border-t border-slate-200 dark:border-slate-700 px-6 py-4 bg-slate-50 dark:bg-slate-900/50 flex items-center justify-between">
            <div class="flex space-x-3">
              <button
                @click="printOrder"
                class="inline-flex items-center px-4 py-2 bg-white dark:bg-slate-800 border-2 border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors font-medium"
              >
                <PrinterIcon class="w-5 h-5 mr-2" />
                Print Order
              </button>
              <button
                @click="trackOrder"
                class="inline-flex items-center px-4 py-2 bg-white dark:bg-slate-800 border-2 border-slate-200 dark:border-slate-700 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors font-medium"
              >
                <TruckIcon class="w-5 h-5 mr-2" />
                Track Order
              </button>
            </div>
            <button
              @click="close"
              class="px-6 py-2 bg-slate-200 dark:bg-slate-700 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-300 dark:hover:bg-slate-600 transition-colors font-medium"
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import {
  XMarkIcon,
  DocumentTextIcon,
  PrinterIcon,
  TruckIcon
} from '@heroicons/vue/24/outline'

interface Props {
  show: boolean
  order: any
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  print: [order: any]
  track: [order: any]
}>()

const router = useRouter()

// Computed
const orderItems = computed(() => {
  if (!props.order) return []
  
  // If order has items array (from create-order)
  if (props.order.items && Array.isArray(props.order.items)) {
    return props.order.items
  }
  
  // Mock items for old orders
  return [
    {
      name: 'Sample Item',
      sku: 'SKU-001',
      quantity: 1,
      price: props.order.totalAmount || 0
    }
  ]
})

const subtotal = computed(() => {
  return orderItems.value.reduce((sum: number, item: Record<string, any>) => sum + (item.price * item.quantity), 0)
})

const deliveryFee = computed(() => {
  return subtotal.value > 500 ? 0 : 50
})

// Methods
const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'pending': 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400',
    'approved': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'in-transit': 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    'delivered': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'cancelled': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    'aggregated': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400'
  }
  return classes[status?.toLowerCase()] || classes.pending
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, string> = {
    'pending': 'Awaiting Approval',
    'approved': 'Approved',
    'in-transit': 'On the Way',
    'delivered': 'Delivered',
    'cancelled': 'Cancelled',
    'aggregated': 'Order Placed'
  }
  return labels[status?.toLowerCase()] || status
}

const formatDate = (date: any) => {
  if (!date) return 'N/A'
  const d = new Date(date)
  return d.toLocaleDateString('en-US', { 
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const close = () => {
  emit('close')
}

const printOrder = () => {
  emit('print', props.order)
}

const trackOrder = () => {
  emit('track', props.order)
}
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .relative,
.modal-leave-active .relative {
  transition: transform 0.3s ease;
}

.modal-enter-from .relative,
.modal-leave-to .relative {
  transform: scale(0.95);
}
</style>

