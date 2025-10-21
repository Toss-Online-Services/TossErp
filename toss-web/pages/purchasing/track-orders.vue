<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 pb-20">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center gap-3">
          <NuxtLink 
            to="/purchasing/orders" 
            class="p-2 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-lg transition-colors"
          >
            <ArrowLeftIcon class="w-6 h-6 text-slate-600 dark:text-slate-400" />
        </NuxtLink>
        <div>
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Track Your Orders
            </h1>
            <p class="text-sm text-slate-600 dark:text-slate-400">
              Monitor your deliveries in real-time
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Timeline Section (shown when order selected) -->
      <div v-if="selectedOrder" class="mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-xl font-bold text-slate-900 dark:text-white">
              Order {{ selectedOrder.number }}
            </h2>
            <button 
              @click="selectedOrder = null"
              class="text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white"
            >
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          <OrderTimeline 
            :order-number="selectedOrder.number"
            :status="selectedOrder.status"
            :order-date="selectedOrder.orderDate"
            :expected-delivery="selectedOrder.expectedDelivery"
          />
        </div>
      </div>
      
      <!-- Recent Orders -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-8">
        <h2 class="text-xl font-bold text-slate-900 dark:text-white mb-6">
          {{ selectedOrder ? 'Other Orders' : 'Recent Orders' }}
        </h2>
        
        <div class="space-y-4">
          <div
            v-for="order in recentOrders"
            :key="order.id"
            @click="viewTimeline(order)"
            class="rounded-xl p-5 border-2 cursor-pointer transition-all duration-300 hover:shadow-lg"
            :class="getOrderCardClass(order.status)"
          >
            <div class="flex justify-between items-start mb-3">
              <div class="flex-1">
                <p class="font-bold text-lg text-slate-900 dark:text-white">{{ order.number }}</p>
                <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">{{ order.date }}</p>
              </div>
              <span 
                class="px-3 py-1 rounded-full text-sm font-bold flex items-center gap-2"
                :class="getStatusBadgeClass(order.status)"
              >
                <component :is="getStatusIcon(order.status)" class="w-4 h-4" />
                {{ order.statusText }}
              </span>
            </div>
            <p class="text-base text-slate-700 dark:text-slate-300 mb-2">
              {{ order.items }} • R{{ order.total }}
            </p>
            <p v-if="order.eta" class="text-sm font-medium mt-2 flex items-center gap-2" :class="order.status === 'in-transit' ? 'text-green-600' : 'text-slate-600'">
              <ClockIcon class="w-4 h-4" />
              {{ order.eta }}
            </p>
            <button class="mt-3 text-sm font-medium text-blue-600 hover:text-blue-700 flex items-center gap-1">
              View Timeline
              <ArrowRightIcon class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
      
      <!-- Help Card -->
      <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 rounded-2xl border-2 border-blue-200 dark:border-blue-800 p-6 shadow-lg">
        <div class="flex items-start gap-4">
          <div class="w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center flex-shrink-0">
            <QuestionMarkCircleIcon class="w-7 h-7 text-white" />
          </div>
          <div class="flex-1">
            <h3 class="font-bold text-lg text-slate-900 dark:text-white mb-2">Need help with your order?</h3>
            <p class="text-base text-slate-700 dark:text-slate-300 mb-4">
              If you have questions about your delivery, chat with us on WhatsApp or call us.
            </p>
            <div class="flex flex-col sm:flex-row gap-3">
              <a 
                href="https://wa.me/27123456789?text=Hi!%20I%20need%20help%20tracking%20my%20order."
                target="_blank"
                class="inline-flex items-center justify-center gap-2 px-6 py-3 bg-green-500 text-white rounded-xl font-bold hover:bg-green-600 transition-all shadow-md hover:shadow-lg transform hover:scale-105"
              >
                <ChatBubbleLeftRightIcon class="w-5 h-5" />
                <span>Chat on WhatsApp</span>
              </a>
              <a 
                href="tel:+27123456789"
                class="inline-flex items-center justify-center gap-2 px-6 py-3 bg-blue-500 text-white rounded-xl font-bold hover:bg-blue-600 transition-all shadow-md hover:shadow-lg transform hover:scale-105"
              >
                <PhoneIcon class="w-5 h-5" />
                <span>Call Us</span>
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import OrderTimeline from '~/components/purchasing/OrderTimeline.vue'
import {
  ArrowLeftIcon,
  XMarkIcon,
  CheckCircleIcon,
  TruckIcon,
  ClockIcon,
  QuestionMarkCircleIcon,
  ChatBubbleLeftRightIcon,
  PhoneIcon,
  ArrowRightIcon,
  CheckBadgeIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Track Orders - TOSS ERP',
  meta: [
    { name: 'description', content: 'Track your purchase orders and deliveries in real-time' }
  ]
})

const route = useRoute()
const selectedOrder = ref<any>(null)

// Load orders from localStorage or use mock data
const recentOrders = ref([
  {
    id: '1',
    number: 'PO-1761045935289',
    status: 'in-transit',
    statusText: 'On The Way',
    date: 'Ordered today',
    items: 'White Bread, Full Cream Milk, Coca-Cola',
    total: '1,250',
    eta: 'Arrives by 3pm today',
    orderDate: new Date(),
    expectedDelivery: new Date(Date.now() + 4 * 60 * 60 * 1000) // 4 hours from now
  },
  {
    id: '2',
    number: 'PO-1234567890',
    status: 'delivered',
    statusText: 'Delivered',
    date: 'Delivered 2 days ago',
    items: 'Maize Meal, Sugar, Cooking Oil',
    total: '850',
    eta: null,
    orderDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    expectedDelivery: null
  }
])

onMounted(() => {
  // Load orders from localStorage
  const savedOrders = localStorage.getItem('toss-orders')
  if (savedOrders) {
    const parsedOrders = JSON.parse(savedOrders)
    recentOrders.value = parsedOrders.map((order: any) => ({
      id: order.id || order.orderNumber,
      number: order.orderNumber,
      status: order.status?.toLowerCase() || 'pending',
      statusText: order.status || 'Pending',
      date: `Ordered ${formatDateRelative(order.date)}`,
      items: order.items?.map((item: any) => item.name).join(', ') || 'N/A',
      total: order.total?.toLocaleString() || '0',
      eta: order.status === 'in-transit' ? `Arrives by ${formatDate(order.expectedDelivery)}` : null,
      orderDate: new Date(order.date),
      expectedDelivery: new Date(order.expectedDelivery)
    }))
  }

  // Check if order number is in query params
  if (route.query.order) {
    const order = recentOrders.value.find(o => o.number === route.query.order)
    if (order) {
      selectedOrder.value = order
    }
  }
})

const viewTimeline = (order: any) => {
  selectedOrder.value = order
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const getOrderCardClass = (status: string) => {
  const classes = {
    'delivered': 'bg-green-50 dark:bg-green-900/20 border-green-200 dark:border-green-800',
    'in-transit': 'bg-orange-50 dark:bg-orange-900/20 border-orange-200 dark:border-orange-800',
    'pending': 'bg-yellow-50 dark:bg-yellow-900/20 border-yellow-200 dark:border-yellow-800',
    'approved': 'bg-blue-50 dark:bg-blue-900/20 border-blue-200 dark:border-blue-800'
  }
  return classes[status as keyof typeof classes] || classes.pending
}

const getStatusBadgeClass = (status: string) => {
  const classes = {
    'delivered': 'bg-green-500 text-white',
    'in-transit': 'bg-orange-500 text-white',
    'pending': 'bg-yellow-500 text-white',
    'approved': 'bg-blue-500 text-white'
  }
  return classes[status as keyof typeof classes] || classes.pending
}

const getStatusIcon = (status: string) => {
  const icons = {
    'delivered': CheckBadgeIcon,
    'in-transit': TruckIcon,
    'pending': ClockIcon,
    'approved': CheckCircleIcon
  }
  return icons[status as keyof typeof icons] || ClockIcon
}

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true })
}

const formatDateRelative = (date: string | Date) => {
  const now = new Date()
  const orderDate = new Date(date)
  const diffTime = Math.abs(now.getTime() - orderDate.getTime())
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0 || diffDays === 1) return 'today'
  if (diffDays === 2) return 'yesterday'
  return `${diffDays} days ago`
}
</script>

