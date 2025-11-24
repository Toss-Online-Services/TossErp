<template>
  <div class="container mx-auto py-6 px-4">
    <!-- Back Button -->
    <div class="mb-6">
      <NuxtLink
        to="/admin/orders"
        class="inline-flex items-center text-indigo-600 hover:text-indigo-800 dark:text-indigo-400 transition-colors"
      >
        <ChevronLeft class="w-5 h-5 mr-1" />
        Back to Orders
      </NuxtLink>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center py-12">
      <div class="w-8 h-8 border-4 border-indigo-500 border-t-transparent rounded-full animate-spin" />
      <p class="ml-3 text-sm text-gray-600 dark:text-gray-400">Loading order details...</p>
    </div>

    <!-- Order Details -->
    <div v-else-if="order" class="space-y-6">
      <!-- Order Header -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="flex flex-col md:flex-row md:items-center md:justify-between mb-6">
          <div>
            <h1 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">Order Details</h1>
            <div class="flex flex-wrap items-center gap-4 text-sm text-gray-600 dark:text-gray-400">
              <div>
                <span class="font-semibold">Order no.</span>
                <span class="ml-2 font-bold text-gray-900 dark:text-white">{{ order.poNumber || order.id }}</span>
                <span class="ml-2">from</span>
                <span class="ml-2 font-semibold">{{ formatDate(order.orderDate) }}</span>
              </div>
              <div>
                <span class="font-semibold">Code:</span>
                <span class="ml-2 font-mono font-bold text-indigo-600 dark:text-indigo-400">{{ order.code || order.poNumber }}</span>
              </div>
            </div>
          </div>
          <div class="mt-4 md:mt-0 flex items-center gap-3">
            <MaterialButton variant="outlined" class="flex items-center gap-2">
              <FileText class="w-5 h-5" />
              Invoice
            </MaterialButton>
            <span
              :class="[
                'px-4 py-2 text-sm font-semibold rounded-lg',
                getStatusClass(order.status)
              ]"
            >
              {{ order.status }}
            </span>
          </div>
        </div>
      </MaterialCard>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Left Column: Product Details & Timeline -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Product Details -->
          <MaterialCard variant="elevated" class="p-6">
            <div class="flex flex-col md:flex-row gap-6">
              <!-- Product Image -->
              <div class="flex-shrink-0">
                <div class="w-32 h-32 bg-gradient-to-br from-indigo-100 to-purple-100 dark:from-indigo-900/30 dark:to-purple-900/30 rounded-xl flex items-center justify-center">
                  <Package class="w-16 h-16 text-indigo-600 dark:text-indigo-400" />
                </div>
              </div>
              
              <!-- Product Info -->
              <div class="flex-1">
                <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-2">
                  {{ order.items?.[0]?.productName || 'Order Items' }}
                </h2>
                <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
                  Order was {{ getDeliveryStatus(order.status) }}
                </p>
                
                <div class="flex flex-wrap gap-3 mb-4">
                  <span
                    :class="[
                      'px-3 py-1 text-xs font-semibold rounded-full',
                      getStatusClass(order.status)
                    ]"
                  >
                    {{ order.status }}
                  </span>
                </div>

                <div class="flex flex-wrap gap-3">
                  <MaterialButton variant="outlined" size="sm">
                    Contact Us
                  </MaterialButton>
                  <MaterialButton variant="outlined" size="sm">
                    Leave Review
                  </MaterialButton>
                </div>
              </div>
            </div>
          </MaterialCard>

          <!-- Order Items Table -->
          <MaterialCard variant="elevated" class="p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Order Items</h3>
            <div class="overflow-x-auto">
              <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
                <thead class="bg-gray-50 dark:bg-gray-700/50">
                  <tr>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Product</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">SKU</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Quantity</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Unit Price</th>
                    <th class="px-4 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Total</th>
                  </tr>
                </thead>
                <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
                  <tr v-for="item in order.items" :key="item.id" class="hover:bg-gray-50 dark:hover:bg-gray-700/50">
                    <td class="px-4 py-4 whitespace-nowrap">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ item.productName }}</div>
                    </td>
                    <td class="px-4 py-4 whitespace-nowrap">
                      <div class="text-sm text-gray-500 dark:text-gray-400 font-mono">{{ item.productSKU }}</div>
                    </td>
                    <td class="px-4 py-4 whitespace-nowrap">
                      <div class="text-sm text-gray-900 dark:text-white">{{ item.quantity }}</div>
                    </td>
                    <td class="px-4 py-4 whitespace-nowrap">
                      <div class="text-sm text-gray-900 dark:text-white">R{{ formatCurrency(item.unitPrice) }}</div>
                    </td>
                    <td class="px-4 py-4 whitespace-nowrap">
                      <div class="text-sm font-semibold text-gray-900 dark:text-white">R{{ formatCurrency(item.lineTotal) }}</div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </MaterialCard>

          <!-- Order Tracking Timeline -->
          <MaterialCard variant="elevated" class="p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-6">Track order</h3>
            <div class="relative">
              <!-- Timeline Line -->
              <div class="absolute left-4 top-0 bottom-0 w-0.5 bg-gray-200 dark:bg-gray-700"></div>
              
              <!-- Timeline Items -->
              <div class="space-y-6">
                <div
                  v-for="(timelineItem, index) in orderTimeline"
                  :key="index"
                  class="relative flex items-start gap-4"
                >
                  <!-- Timeline Icon -->
                  <div
                    :class="[
                      'relative z-10 flex items-center justify-center w-8 h-8 rounded-full',
                      timelineItem.completed
                        ? 'bg-green-500 text-white'
                        : 'bg-gray-200 dark:bg-gray-700 text-gray-500 dark:text-gray-400'
                    ]"
                  >
                    <component :is="timelineItem.icon" class="w-4 h-4" />
                  </div>
                  
                  <!-- Timeline Content -->
                  <div class="flex-1 pt-1">
                    <div class="flex items-center justify-between mb-1">
                      <h4 class="text-sm font-semibold text-gray-900 dark:text-white">
                        {{ timelineItem.title }}
                      </h4>
                      <span class="text-xs text-gray-500 dark:text-gray-400">
                        {{ timelineItem.time }}
                      </span>
                    </div>
                    <p class="text-sm text-gray-600 dark:text-gray-400">
                      {{ timelineItem.description }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </MaterialCard>
        </div>

        <!-- Right Column: Payment & Billing -->
        <div class="space-y-6">
          <!-- Payment Details -->
          <MaterialCard variant="elevated" class="p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Payment details</h3>
            <div class="space-y-4">
              <div class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700/50 rounded-lg">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-indigo-100 dark:bg-indigo-900/30 rounded-lg flex items-center justify-center">
                    <CreditCard class="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
                  </div>
                  <div>
                    <div class="text-sm font-mono text-gray-900 dark:text-white">**** **** **** 7852</div>
                    <div class="text-xs text-gray-500 dark:text-gray-400">Card ending in 7852</div>
                  </div>
                </div>
              </div>
            </div>
          </MaterialCard>

          <!-- Billing Information -->
          <MaterialCard variant="elevated" class="p-6">
            <div class="flex items-center gap-2 mb-4">
              <AlertCircle class="w-5 h-5 text-indigo-600 dark:text-indigo-400" />
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Billing Information</h3>
            </div>
            <div class="space-y-3">
              <div>
                <div class="text-sm font-semibold text-gray-900 dark:text-white">
                  {{ order.shopName || order.retailerName || 'N/A' }}
                </div>
                <div class="text-sm text-gray-600 dark:text-gray-400">
                  Company Name: {{ order.shopName || 'N/A' }}
                </div>
                <div class="text-sm text-gray-600 dark:text-gray-400">
                  Email Address: {{ order.email || 'N/A' }}
                </div>
                <div class="text-sm text-gray-600 dark:text-gray-400">
                  VAT Number: {{ order.vatNumber || 'N/A' }}
                </div>
              </div>
            </div>
          </MaterialCard>

          <!-- Order Summary -->
          <MaterialCard variant="elevated" class="p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Order Summary</h3>
            <div class="space-y-3">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Product Price:</span>
                <span class="text-gray-900 dark:text-white font-medium">R{{ formatCurrency(order.subtotal || order.totalAmount) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Delivery:</span>
                <span class="text-gray-900 dark:text-white font-medium">R{{ formatCurrency(order.deliveryFee || 0) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Taxes:</span>
                <span class="text-gray-900 dark:text-white font-medium">R{{ formatCurrency(order.taxAmount || 0) }}</span>
              </div>
              <div class="pt-3 border-t border-gray-200 dark:border-gray-700 flex justify-between">
                <span class="text-base font-semibold text-gray-900 dark:text-white">Total:</span>
                <span class="text-lg font-bold text-indigo-600 dark:text-indigo-400">R{{ formatCurrency(order.totalAmount) }}</span>
              </div>
            </div>
          </MaterialCard>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  ChevronLeft,
  Package,
  FileText,
  CreditCard,
  AlertCircle,
  Bell,
  ShoppingCart,
  CheckCircle,
  Truck
} from 'lucide-vue-next'
import MaterialCard from '~/components/material/MaterialCard.vue'
import MaterialButton from '~/components/material/MaterialButton.vue'

definePageMeta({
  layout: 'default',
  middleware: 'auth',
  meta: {
    roles: ['Administrator'],
    role: 'admin',
    title: 'Order Details',
    subtitle: 'Review and track fulfillment'
  }
})

const route = useRoute()
const router = useRouter()
const purchaseOrdersAPI = usePurchaseOrdersAPI()

const orderId = computed(() => Number(route.params.id))
const order = ref<any>(null)
const isLoading = ref(true)

const loadOrder = async () => {
  isLoading.value = true
  try {
    order.value = await purchaseOrdersAPI.getPurchaseOrderById(orderId.value)
  } catch (error) {
    console.error('Failed to load order:', error)
    router.push('/admin/orders')
  } finally {
    isLoading.value = false
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  })
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'Draft': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200',
    'Submitted': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'Accepted': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'Shipped': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'Cancelled': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

const getDeliveryStatus = (status: string) => {
  const statusMap: Record<string, string> = {
    'Draft': 'created',
    'Submitted': 'submitted',
    'Accepted': 'accepted',
    'Shipped': 'shipped',
    'Delivered': 'delivered 2 days ago',
    'Cancelled': 'cancelled'
  }
  return statusMap[status] || 'processing'
}

const orderTimeline = computed(() => {
  const status = order.value?.status || 'Draft'
  const orderDate = order.value?.orderDate ? new Date(order.value.orderDate) : new Date()
  
  const timeline = [
    {
      title: 'Order received',
      description: `Generate order id #${order.value?.id || order.value?.poNumber || 'N/A'}`,
      time: formatTimelineDate(orderDate),
      icon: Bell,
      completed: true
    },
    {
      title: 'Order transmitted to courier',
      description: 'Your order has been transmitted to the courier',
      time: formatTimelineDate(new Date(orderDate.getTime() + 60 * 60 * 1000)),
      icon: ShoppingCart,
      completed: ['Accepted', 'Shipped', 'Delivered'].includes(status)
    },
    {
      title: 'Order delivered',
      description: 'Your order has been successfully delivered',
      time: formatTimelineDate(new Date(orderDate.getTime() + 8 * 60 * 60 * 1000)),
      icon: CheckCircle,
      completed: status === 'Delivered'
    }
  ]
  
  return timeline
})

const formatTimelineDate = (date: Date) => {
  const day = date.getDate()
  const month = date.toLocaleDateString('en-US', { month: 'short' }).toUpperCase()
  const hours = date.getHours()
  const minutes = date.getMinutes()
  const ampm = hours >= 12 ? 'PM' : 'AM'
  const displayHours = hours % 12 || 12
  const displayMinutes = minutes.toString().padStart(2, '0')
  
  return `${day} ${month} ${displayHours}:${displayMinutes} ${ampm}`
}

onMounted(() => {
  loadOrder()
})
</script>
