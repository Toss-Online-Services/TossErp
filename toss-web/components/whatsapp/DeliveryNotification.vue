<template>
  <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md border border-gray-200 dark:border-gray-700 p-4">
    <div class="flex items-start space-x-3">
      <!-- WhatsApp Icon -->
      <div class="flex-shrink-0">
        <div class="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center">
          <svg class="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
            <path d="M2.003 5.884L10 9.882l7.997-3.998A2 2 0 0016 4H4a2 2 0 00-1.997 1.884z"/>
            <path d="M18 8.118l-8 4-8-4V14a2 2 0 002 2h12a2 2 0 002-2V8.118z"/>
          </svg>
        </div>
      </div>

      <!-- Notification Content -->
      <div class="flex-1">
        <div class="flex items-center justify-between mb-1">
          <h4 class="font-semibold text-gray-900 dark:text-white">WhatsApp Delivery Update</h4>
          <span class="text-xs text-gray-500 dark:text-gray-500">Just now</span>
        </div>
        <p class="text-sm text-gray-700 dark:text-gray-300 mb-2">
          {{ notificationText }}
        </p>
        <div class="flex items-center space-x-2">
          <span class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium" :class="statusClass">
            {{ status }}
          </span>
          <button class="text-xs text-blue-600 dark:text-blue-400 hover:text-blue-700 font-medium">
            View Details
          </button>
        </div>
      </div>
    </div>

    <!-- Feature Coming Soon -->
    <div class="mt-4 pt-4 border-t border-gray-200 dark:border-gray-700">
      <div class="flex items-center justify-between">
        <p class="text-xs text-gray-600 dark:text-gray-400">
          Real WhatsApp notifications will be sent when this feature launches
        </p>
        <span class="text-xs px-2 py-1 rounded-full bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
          Coming Soon
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  type?: 'order-confirmed' | 'out-for-delivery' | 'delivered' | 'driver-assigned'
  orderId?: string
  driverName?: string
}

const props = withDefaults(defineProps<Props>(), {
  type: 'order-confirmed',
  orderId: 'ORD-001',
  driverName: 'Sipho M.'
})

const notificationText = computed(() => {
  switch (props.type) {
    case 'order-confirmed':
      return `Your order ${props.orderId} has been confirmed! We'll notify you when it's on the way.`
    case 'driver-assigned':
      return `${props.driverName} has been assigned to deliver your order ${props.orderId}.`
    case 'out-for-delivery':
      return `Your order ${props.orderId} is out for delivery! ${props.driverName} will arrive soon.`
    case 'delivered':
      return `Your order ${props.orderId} has been delivered. Thank you for using TOSS!`
    default:
      return 'Order update'
  }
})

const status = computed(() => {
  switch (props.type) {
    case 'order-confirmed':
      return 'Confirmed'
    case 'driver-assigned':
      return 'Assigned'
    case 'out-for-delivery':
      return 'In Transit'
    case 'delivered':
      return 'Delivered'
    default:
      return 'Pending'
  }
})

const statusClass = computed(() => {
  switch (props.type) {
    case 'order-confirmed':
      return 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400'
    case 'driver-assigned':
      return 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400'
    case 'out-for-delivery':
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
    case 'delivered':
      return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
    default:
      return 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
  }
})
</script>

