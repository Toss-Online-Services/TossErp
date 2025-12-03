<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBuyingStore, type GoodsReceipt } from '~/stores/buying'
import { useLogisticsStore, type DeliveryRun, type Driver } from '~/stores/logistics'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Goods Receipt Details - TOSS'
})

const route = useRoute()
const buyingStore = useBuyingStore()
const logisticsStore = useLogisticsStore()

const receipt = ref<GoodsReceipt | null>(null)
const driver = ref<Driver | null>(null)
const deliveryRun = ref<DeliveryRun | null>(null)
const deliveryStop = ref<any>(null)
const deliveryRating = ref<{ rating: number; comment?: string; createdAt?: Date } | null>(null)
const loading = ref(false)

const receiptId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadReceipt() {
  loading.value = true
  try {
    await buyingStore.fetchGoodsReceipts()
    const id = receiptId.value
    console.log('Loading goods receipt with ID:', id)
    
    receipt.value = buyingStore.getGoodsReceiptById(id) || null
    
    if (!receipt.value) {
      console.error('Goods receipt not found for ID:', id)
      return
    }
    
    // Load driver information - find the driver who actually delivered the goods
    if (receipt.value.purchaseOrderNumber) {
      await logisticsStore.fetchDeliveryRuns()
      await logisticsStore.fetchDrivers()
      
      console.log('Looking for PO:', receipt.value.purchaseOrderNumber)
      console.log('Available delivery runs:', logisticsStore.deliveryRuns)
      
      // Find delivery run that includes this purchase order
      // Prioritize completed deliveries first, then in-transit, then assigned
      const matchingRuns = logisticsStore.deliveryRuns.filter(run => {
        const hasMatchingStop = run.stops.some(stop => {
          const matches = stop.purchaseOrderNumber === receipt.value?.purchaseOrderNumber &&
            (stop.status === 'completed' || stop.status === 'in_transit' || stop.status === 'assigned')
          if (matches) {
            console.log('Found matching stop:', stop, 'in run:', run.runNumber)
          }
          return matches
        })
        return hasMatchingStop
      })
      
      console.log('Matching runs:', matchingRuns)
      
      // Sort by: completed first, then by completion time (most recent first)
      matchingRuns.sort((a, b) => {
        if (a.status === 'completed' && b.status !== 'completed') return -1
        if (b.status === 'completed' && a.status !== 'completed') return 1
        if (a.status === 'completed' && b.status === 'completed') {
          const aTime = a.completedAt ? new Date(a.completedAt).getTime() : 0
          const bTime = b.completedAt ? new Date(b.completedAt).getTime() : 0
          return bTime - aTime // Most recent first
        }
        return 0
      })
      
      deliveryRun.value = matchingRuns[0] || null
      console.log('Selected delivery run:', deliveryRun.value)
      
      // Find the specific delivery stop for this purchase order
      if (deliveryRun.value) {
        deliveryStop.value = deliveryRun.value.stops.find(stop => 
          stop.purchaseOrderNumber === receipt.value?.purchaseOrderNumber
        ) || null
        
        console.log('Found delivery stop:', deliveryStop.value)
        
        // Get delivery rating if available (from stop or run)
        if (deliveryStop.value && (deliveryStop.value as any).rating) {
          deliveryRating.value = {
            rating: (deliveryStop.value as any).rating,
            comment: (deliveryStop.value as any).reviewComment,
            createdAt: (deliveryStop.value as any).reviewDate
          }
          console.log('Found delivery rating:', deliveryRating.value)
        }
      }
      
      // Get driver information - the driver who delivered the goods
      if (deliveryRun.value?.driverId) {
        console.log('Looking for driver with ID:', deliveryRun.value.driverId)
        driver.value = logisticsStore.getDriverById(deliveryRun.value.driverId) || null
        console.log('Found driver:', driver.value)
      } else {
        console.log('No driverId in delivery run')
      }
    } else {
      console.log('No purchase order number in receipt')
    }
  } catch (error) {
    console.error('Failed to load goods receipt:', error)
  } finally {
    loading.value = false
  }
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

function formatDateTime(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function getOrderTimeline() {
  if (!receipt.value) return []
  
  const timeline = []
  
  // Purchase order created
  if (receipt.value.purchaseOrderId) {
    const purchaseOrder = buyingStore.getPurchaseOrderById(receipt.value.purchaseOrderId)
    if (purchaseOrder) {
      timeline.push({
        step: 'order_received',
        icon: 'notifications',
        title: 'Order received',
        description: `Purchase order ${purchaseOrder.poNumber} created`,
        date: purchaseOrder.createdAt,
        completed: true,
        color: 'text-gray-500'
      })
      
      if (purchaseOrder.status !== 'draft') {
        timeline.push({
          step: 'order_submitted',
          icon: 'send',
          title: 'Order submitted',
          description: `Order sent to ${purchaseOrder.supplierName}`,
          date: purchaseOrder.updatedAt,
          completed: true,
          color: 'text-gray-500'
        })
      }
      
      if (purchaseOrder.status === 'approved' || purchaseOrder.status === 'partially_received' || purchaseOrder.status === 'completed') {
        timeline.push({
          step: 'order_approved',
          icon: 'check_circle',
          title: 'Order approved',
          description: purchaseOrder.approvedBy ? `Approved by ${purchaseOrder.approvedBy}` : 'Order approved by supplier',
          date: purchaseOrder.approvedDate || purchaseOrder.updatedAt,
          completed: true,
          color: 'text-blue-500'
        })
      }
    }
  }
  
  // Driver assigned / In transit
  if (deliveryRun.value) {
    if (deliveryRun.value.status === 'assigned' || deliveryRun.value.status === 'in_transit' || deliveryRun.value.status === 'completed') {
      timeline.push({
        step: 'driver_assigned',
        icon: 'local_shipping',
        title: 'Driver assigned',
        description: driver.value ? `Assigned to ${driver.value.fullName}` : 'Driver assigned to delivery',
        date: deliveryRun.value.assignedDate,
        completed: true,
        color: 'text-orange-500'
      })
    }
    
    if (deliveryRun.value.status === 'in_transit' || deliveryRun.value.status === 'completed') {
      timeline.push({
        step: 'in_transit',
        icon: 'local_shipping',
        title: 'Order in transit',
        description: 'Order is being delivered',
        date: deliveryRun.value.startedAt,
        completed: true,
        color: 'text-blue-500'
      })
    }
    
    if (deliveryRun.value.status === 'completed') {
      timeline.push({
        step: 'delivered',
        icon: 'done',
        title: 'Order delivered',
        description: 'Order has been delivered',
        date: deliveryRun.value.completedAt,
        completed: true,
        color: 'text-green-500'
      })
    }
  }
  
  // Goods received
  timeline.push({
    step: 'goods_received',
    icon: 'inventory',
    title: 'Goods received',
    description: `Goods receipt ${receipt.value.receiptNumber} created`,
    date: receipt.value.receiptDate,
    completed: receipt.value.status === 'submitted',
    color: receipt.value.status === 'submitted' ? 'text-green-500' : 'text-gray-500'
  })
  
  return timeline
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    draft: 'text-gray-600 bg-gray-100',
    submitted: 'text-green-600 bg-green-100',
    cancelled: 'text-red-600 bg-red-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    draft: 'Draft',
    submitted: 'Submitted',
    cancelled: 'Cancelled'
  }
  return labels[status] || status
}

function handlePrint() {
  window.print()
}

async function handleSend() {
  if (!receipt.value) return
  try {
    // TODO: Implement send functionality (email/SMS/WhatsApp)
    alert(`Sending goods receipt ${receipt.value.receiptNumber} to ${receipt.value.supplierName}...`)
    // await buyingStore.sendGoodsReceipt(receipt.value.id)
  } catch (error) {
    console.error('Failed to send goods receipt:', error)
  }
}

onMounted(async () => {
  await loadReceipt()
})
</script>

<template>
  <div class="py-4 md:py-6 px-2 sm:px-4 md:px-0 w-full max-w-full overflow-x-hidden">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading goods receipt...</p>
      </div>
    </div>

    <div v-else-if="!receipt" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Goods receipt not found</h3>
        <p class="text-gray-600 mb-6">The goods receipt you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/buying/receipts')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Goods Receipts</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-4 md:mb-6">
        <button
          @click="navigateTo('/buying/receipts')"
          class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-3 md:mb-4 transition-colors text-sm md:text-base"
        >
          <i class="material-symbols-rounded text-base md:text-lg">arrow_back</i>
          <span>Back to Goods Receipts</span>
        </button>
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-3 md:gap-4">
          <div>
            <h3 class="text-2xl md:text-3xl font-bold text-gray-900 mb-1 md:mb-2">{{ receipt.receiptNumber }}</h3>
            <p class="text-gray-600 text-xs md:text-sm">Goods receipt details and items</p>
          </div>
          <div class="flex flex-wrap items-center gap-2 md:gap-3">
            <span :class="['px-2 md:px-3 py-1 text-xs md:text-sm font-medium rounded-full', getStatusColor(receipt.status)]">
              {{ getStatusLabel(receipt.status) }}
            </span>
            <button
              @click="handlePrint"
              class="inline-flex items-center gap-1 md:gap-2 px-3 md:px-4 py-1.5 md:py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors text-sm"
            >
              <i class="material-symbols-rounded text-base md:text-lg">print</i>
              <span class="hidden sm:inline">Print</span>
            </button>
            <button
              @click="handleSend"
              class="inline-flex items-center gap-1 md:gap-2 px-3 md:px-4 py-1.5 md:py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors text-sm"
            >
              <i class="material-symbols-rounded text-base md:text-lg">send</i>
              <span class="hidden sm:inline">Send</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 md:gap-6 mb-4 md:mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">store</i>
            Supplier Information
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Supplier Name</p>
              <p class="text-base font-semibold text-gray-900">{{ receipt.supplierName }}</p>
            </div>
            <div v-if="receipt.purchaseOrderNumber">
              <p class="text-xs text-gray-500 mb-1">Purchase Order</p>
              <NuxtLink
                v-if="receipt.purchaseOrderId"
                :to="`/buying/purchase-orders/${receipt.purchaseOrderId}`"
                class="text-sm text-gray-700 hover:text-gray-900 hover:underline"
              >
                {{ receipt.purchaseOrderNumber }}
              </NuxtLink>
              <p v-else class="text-sm text-gray-700">{{ receipt.purchaseOrderNumber }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">calendar_today</i>
            Receipt Dates
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Receipt Date</p>
              <p class="text-base font-semibold text-gray-900">{{ formatDate(receipt.receiptDate) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Warehouse</p>
              <p class="text-base font-semibold text-gray-900">{{ receipt.warehouse }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">local_shipping</i>
            Delivery Driver
          </h4>
          <div v-if="driver" class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Driver Name</p>
              <NuxtLink
                :to="`/logistics/drivers/${driver.id}`"
                class="text-base font-semibold text-gray-900 hover:text-gray-700 hover:underline flex items-center gap-2"
              >
                {{ driver.fullName }}
                <i class="material-symbols-rounded text-sm">open_in_new</i>
              </NuxtLink>
            </div>
            <div v-if="driver.phone">
              <p class="text-xs text-gray-500 mb-1">Phone</p>
              <a
                :href="`tel:${driver.phone}`"
                class="text-sm text-gray-700 hover:text-gray-900 hover:underline flex items-center gap-1"
              >
                <i class="material-symbols-rounded text-sm">phone</i>
                {{ driver.phone }}
              </a>
            </div>
            <div v-if="deliveryRun">
              <p class="text-xs text-gray-500 mb-1">Delivery Run</p>
              <NuxtLink
                :to="`/logistics/deliveries/${deliveryRun.id}`"
                class="text-sm text-gray-700 hover:text-gray-900 hover:underline flex items-center gap-1"
              >
                <i class="material-symbols-rounded text-sm">local_shipping</i>
                {{ deliveryRun.runNumber }}
              </NuxtLink>
            </div>
            <div v-if="deliveryRun?.completedAt || deliveryStop?.completionTime">
              <p class="text-xs text-gray-500 mb-1">Delivered On</p>
              <p class="text-sm text-gray-900 font-medium">
                {{ formatDate(deliveryStop?.completionTime || deliveryRun?.completedAt) }}
              </p>
            </div>
            <!-- Delivery Rating -->
            <div v-if="deliveryRating" class="pt-3 border-t border-gray-200">
              <p class="text-xs text-gray-500 mb-2">Delivery Rating</p>
              <div class="flex items-center gap-2 mb-2">
                <div class="flex items-center gap-0.5">
                  <i
                    v-for="i in 5"
                    :key="i"
                    class="material-symbols-rounded text-base"
                    :class="i <= deliveryRating.rating ? 'text-yellow-500 fill' : 'text-gray-300'"
                  >
                    star
                  </i>
                </div>
                <span class="text-sm font-semibold text-gray-900">{{ deliveryRating.rating }}/5</span>
              </div>
              <p v-if="deliveryRating.comment" class="text-xs text-gray-600 italic">
                "{{ deliveryRating.comment }}"
              </p>
              <p v-if="deliveryRating.createdAt" class="text-xs text-gray-400 mt-1">
                {{ formatDate(deliveryRating.createdAt) }}
              </p>
            </div>
            <div v-else-if="deliveryRun?.status === 'completed'" class="pt-3 border-t border-gray-200">
              <p class="text-xs text-gray-500 mb-2">No rating yet</p>
              <NuxtLink
                :to="`/logistics/deliveries/${deliveryRun.id}`"
                class="text-xs text-gray-600 hover:text-gray-900 hover:underline inline-flex items-center gap-1"
              >
                Rate this delivery
                <i class="material-symbols-rounded text-sm">arrow_forward</i>
              </NuxtLink>
            </div>
          </div>
          <div v-else class="text-center py-4">
            <i class="material-symbols-rounded text-4xl text-gray-300 mb-2">local_shipping</i>
            <p class="text-xs text-gray-400">No delivery driver assigned</p>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">info</i>
            Receipt Summary
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Items</p>
              <p class="text-base font-semibold text-gray-900">{{ receipt.items.length }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Amount</p>
              <p class="text-lg font-bold text-gray-900">R {{ receipt.total.toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Order Tracking and Items -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 md:gap-6 mb-4 md:mb-6">
        <!-- Order Tracking Timeline -->
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">track_changes</i>
            Track Order
          </h4>
          <div class="space-y-4">
            <div
              v-for="(step, index) in getOrderTimeline()"
              :key="step.step"
              class="relative"
            >
              <div v-if="index < getOrderTimeline().length - 1" class="absolute left-5 top-8 bottom-0 w-0.5 bg-gray-200"></div>
              <div class="flex items-start gap-4">
                <div :class="['flex items-center justify-center w-10 h-10 rounded-full border-2', step.completed ? `${step.color} border-current bg-white` : 'bg-gray-100 border-gray-300']">
                  <i :class="['material-symbols-rounded text-lg', step.color]">{{ step.icon }}</i>
                </div>
                <div class="flex-1 pt-1">
                  <h6 class="text-sm font-semibold text-gray-900 mb-0.5">{{ step.title }}</h6>
                  <p class="text-xs text-gray-500 mb-1">{{ step.description }}</p>
                  <p class="text-xs text-gray-400">{{ formatDateTime(step.date) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Items Table -->
        <div class="lg:col-span-2 bg-white rounded-xl shadow-card overflow-hidden">
          <div class="px-4 sm:px-6 py-4 border-b border-gray-200 flex items-center justify-between">
            <h4 class="text-base sm:text-lg font-semibold text-gray-900 flex items-center gap-2">
              <i class="material-symbols-rounded text-lg sm:text-xl">inventory</i>
              <span class="hidden sm:inline">Received Items</span>
              <span class="sm:hidden">Items</span>
            </h4>
            <span class="text-xs sm:text-sm text-gray-500">{{ receipt.items.length }} item(s)</span>
          </div>
          <div class="overflow-x-auto -mx-6 sm:mx-0">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-3 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
                  <th class="px-3 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Ordered</th>
                  <th class="px-3 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Received</th>
                  <th class="px-3 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rejected</th>
                  <th class="px-3 sm:px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rate</th>
                  <th class="px-3 sm:px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Amount</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="item in receipt.items" :key="item.id" class="hover:bg-gray-50 transition-colors">
                  <td class="px-3 sm:px-6 py-4 whitespace-nowrap">
                    <div class="text-sm font-medium text-gray-900">{{ item.itemName }}</div>
                    <div v-if="item.itemCode" class="text-xs text-gray-500 mt-1">Code: {{ item.itemCode }}</div>
                  </td>
                  <td class="px-3 sm:px-6 py-4 whitespace-nowrap">
                    <span class="text-sm text-gray-900 font-medium">{{ item.orderedQuantity }}</span>
                  </td>
                  <td class="px-3 sm:px-6 py-4 whitespace-nowrap">
                    <span class="text-sm text-green-600 font-medium">{{ item.receivedQuantity }}</span>
                  </td>
                  <td class="px-3 sm:px-6 py-4 whitespace-nowrap">
                    <span v-if="item.rejectedQuantity > 0" class="text-sm text-red-600 font-medium">{{ item.rejectedQuantity }}</span>
                    <span v-else class="text-sm text-gray-400">-</span>
                  </td>
                  <td class="px-3 sm:px-6 py-4 whitespace-nowrap">
                    <span class="text-sm text-gray-900">R {{ item.rate.toFixed(2) }}</span>
                  </td>
                  <td class="px-3 sm:px-6 py-4 whitespace-nowrap text-right">
                    <span class="text-sm font-semibold text-gray-900">R {{ item.amount.toFixed(2) }}</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Totals and Notes -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 md:gap-6">
        <!-- Totals -->
        <div class="lg:col-span-2 bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">receipt</i>
            Receipt Summary
          </h4>
          <div class="flex justify-end">
            <div class="w-full max-w-sm space-y-3">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Subtotal:</span>
                <span class="text-gray-900 font-medium">R {{ receipt.subtotal.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600">Tax (15%):</span>
                <span class="text-gray-900 font-medium">R {{ receipt.taxAmount.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold border-t-2 border-gray-300 pt-3 mt-3">
                <span class="text-gray-900">Total:</span>
                <span class="text-gray-900">R {{ receipt.total.toFixed(2) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Notes -->
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">note</i>
            Notes
          </h4>
          <div v-if="receipt.notes" class="text-sm text-gray-700 leading-relaxed">
            {{ receipt.notes }}
          </div>
          <div v-else class="text-sm text-gray-400 italic">
            No notes added
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

