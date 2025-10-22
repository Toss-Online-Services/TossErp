<template>
  <div class="space-y-6">
    <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-4">Order Timeline</h3>
    
    <div class="relative">
      <!-- Vertical Line -->
      <div class="absolute left-4 top-0 bottom-0 w-0.5 bg-slate-200 dark:bg-slate-700"></div>
      
      <!-- Timeline Events -->
      <div class="space-y-6">
        <!-- Event 1: Order Placed -->
        <div class="relative flex items-start gap-4">
          <div class="flex-shrink-0 w-8 h-8 rounded-full bg-green-500 flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800">
            <CheckCircleIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1 pb-6">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">Order Placed</h4>
                <span class="text-xs text-slate-500 dark:text-slate-400">{{ formatDate(orderDate) }}</span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                Your order {{ orderNumber }} has been received and is being processed.
              </p>
            </div>
          </div>
        </div>

        <!-- Event 2: Order Confirmed -->
        <div class="relative flex items-start gap-4">
          <div 
            class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
            :class="isConfirmed ? 'bg-blue-500' : 'bg-slate-300 dark:bg-slate-600'"
          >
            <DocumentCheckIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1 pb-6">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">Order Confirmed</h4>
                <span v-if="isConfirmed" class="text-xs text-slate-500 dark:text-slate-400">
                  {{ formatDate(confirmedDate) }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                <span v-if="isConfirmed">Your order has been confirmed by the supplier.</span>
                <span v-else class="italic">Awaiting supplier confirmation...</span>
              </p>
            </div>
          </div>
        </div>

        <!-- Event 3: Preparing for Shipment -->
        <div class="relative flex items-start gap-4">
          <div 
            class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
            :class="isPreparing ? 'bg-purple-500' : 'bg-slate-300 dark:bg-slate-600'"
          >
            <CubeIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1 pb-6">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">Preparing for Shipment</h4>
                <span v-if="isPreparing" class="text-xs text-slate-500 dark:text-slate-400">
                  {{ formatDate(preparingDate) }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                <span v-if="isPreparing">Your order is being packed and prepared for delivery.</span>
                <span v-else class="italic">Not started yet...</span>
              </p>
            </div>
          </div>
        </div>

        <!-- Event 4: Ready for Pickup/Delivery -->
        <div class="relative flex items-start gap-4">
          <div 
            class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
            :class="isReady ? 'bg-orange-500' : 'bg-slate-300 dark:bg-slate-600'"
          >
            <TruckIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1 pb-6">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">Ready for Pickup/Delivery</h4>
                <span v-if="isReady" class="text-xs text-slate-500 dark:text-slate-400">
                  {{ formatDate(readyDate) }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                <span v-if="isReady">Your order is ready and awaiting pickup or out for delivery.</span>
                <span v-else class="italic">Not ready yet...</span>
              </p>
              <div v-if="isReady && estimatedDelivery" class="mt-2 flex items-center gap-2 text-sm text-green-600 dark:text-green-400">
                <ClockIcon class="w-4 h-4" />
                <span>Expected by: {{ formatDate(estimatedDelivery) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Event 5: Delivered -->
        <div class="relative flex items-start gap-4">
          <div 
            class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
            :class="isDelivered ? 'bg-green-600' : 'bg-slate-300 dark:bg-slate-600'"
          >
            <CheckBadgeIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">Delivered</h4>
                <span v-if="isDelivered" class="text-xs text-slate-500 dark:text-slate-400">
                  {{ formatDate(deliveredDate) }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                <span v-if="isDelivered">Order successfully delivered and received.</span>
                <span v-else class="italic">Delivery pending...</span>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  CheckCircleIcon,
  DocumentCheckIcon,
  CubeIcon,
  TruckIcon,
  CheckBadgeIcon,
  ClockIcon
} from '@heroicons/vue/24/solid'

interface Props {
  orderNumber: string
  status: string
  orderDate: Date
  expectedDelivery?: Date
}

const props = defineProps<Props>()

// Compute timeline progress based on status (sales-specific)
const isConfirmed = computed(() => {
  return ['confirmed', 'preparing', 'ready', 'delivered'].includes(props.status.toLowerCase())
})

const isPreparing = computed(() => {
  return ['preparing', 'ready', 'delivered'].includes(props.status.toLowerCase())
})

const isReady = computed(() => {
  return ['ready', 'delivered'].includes(props.status.toLowerCase())
})

const isDelivered = computed(() => {
  return props.status.toLowerCase() === 'delivered'
})

// Mock dates for timeline (in production, these would come from API)
const confirmedDate = computed(() => {
  if (!isConfirmed.value) return null
  const date = new Date(props.orderDate)
  date.setHours(date.getHours() + 2)
  return date
})

const preparingDate = computed(() => {
  if (!isPreparing.value) return null
  const date = new Date(props.orderDate)
  date.setHours(date.getHours() + 2)
  return date
})

const readyDate = computed(() => {
  if (!isReady.value) return null
  const date = new Date(props.orderDate)
  date.setHours(date.getHours() + 4)
  return date
})

const deliveredDate = computed(() => {
  if (!isDelivered.value) return null
  const date = new Date(props.orderDate)
  date.setHours(date.getHours() + 6)
  return date
})

const estimatedDelivery = computed(() => {
  return props.expectedDelivery || null
})

const formatDate = (date: Date | null) => {
  if (!date) return ''
  return new Date(date).toLocaleString('en-US', {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
    hour12: true
  })
}
</script>

