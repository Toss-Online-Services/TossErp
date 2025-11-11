<template>
  <div class="space-y-6">
    <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-4">Order Timeline</h3>
    
    <div class="relative">
      <!-- Vertical Line -->
      <div class="absolute left-4 top-0 bottom-0 w-0.5 bg-slate-200 dark:bg-slate-700"></div>
      
      <!-- Timeline Events -->
      <div class="space-y-6">
        <!-- Event 1: Pending -->
        <div class="relative flex items-start gap-4">
          <div class="flex-shrink-0 w-8 h-8 rounded-full bg-yellow-500 flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800">
            <ClockIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1 pb-6">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">⏳ Pending</h4>
                <span class="text-xs text-slate-500 dark:text-slate-400">{{ formatDate(pendingDate) }}</span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                Order {{ orderNumber }} has been placed and is waiting to be started.
              </p>
            </div>
          </div>
        </div>

        <!-- Event 2: In Progress -->
        <div class="relative flex items-start gap-4">
          <div 
            class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
            :class="isInProgress ? 'bg-blue-500' : 'bg-slate-300 dark:bg-slate-600'"
          >
            <CubeIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1 pb-6">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">⚙️ In Progress</h4>
                <span v-if="isInProgress" class="text-xs text-slate-500 dark:text-slate-400">
                  {{ formatDate(inProgressDate) }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                <span v-if="isInProgress">Your order is being prepared.</span>
                <span v-else class="italic">Not started yet...</span>
              </p>
            </div>
          </div>
        </div>

        <!-- Event 3: Ready -->
        <div class="relative flex items-start gap-4">
          <div 
            class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
            :class="isReady ? 'bg-green-500' : 'bg-slate-300 dark:bg-slate-600'"
          >
            <CheckCircleIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1 pb-6">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">✅ Ready</h4>
                <span v-if="isReady" class="text-xs text-slate-500 dark:text-slate-400">
                  {{ formatDate(readyDate) }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                <span v-if="isReady">Your order is ready for pickup or delivery.</span>
                <span v-else class="italic">Not ready yet...</span>
              </p>
              <div v-if="isReady && estimatedDelivery" class="mt-2 flex items-center gap-2 text-sm text-green-600 dark:text-green-400">
                <ClockIcon class="w-4 h-4" />
                <span>Expected by: {{ formatDate(estimatedDelivery) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Event 4: Completed -->
        <div class="relative flex items-start gap-4">
          <div 
            class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
            :class="isCompleted ? 'bg-emerald-600' : 'bg-slate-300 dark:bg-slate-600'"
          >
            <TruckIcon class="w-5 h-5 text-white" />
          </div>
          <div class="flex-1">
            <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-bold text-slate-900 dark:text-white">📦 Completed</h4>
                <span v-if="isCompleted" class="text-xs text-slate-500 dark:text-slate-400">
                  {{ formatDate(completedDate) }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                <span v-if="isCompleted">Order successfully completed and delivered.</span>
                <span v-else class="italic">Awaiting completion...</span>
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
  CubeIcon,
  TruckIcon,
  ClockIcon
} from '@heroicons/vue/24/solid'

interface Props {
  orderNumber: string
  status: string
  orderDate: Date
  expectedDelivery?: Date
}

const props = defineProps<Props>()

// Compute timeline progress based on status (queue statuses)
const isPending = computed(() => {
  return ['pending', 'in-progress', 'ready', 'completed'].includes(props.status.toLowerCase())
})

const isInProgress = computed(() => {
  return ['in-progress', 'ready', 'completed'].includes(props.status.toLowerCase())
})

const isReady = computed(() => {
  return ['ready', 'completed'].includes(props.status.toLowerCase())
})

const isCompleted = computed(() => {
  return props.status.toLowerCase() === 'completed'
})

// Mock dates for timeline (in production, these would come from API)
const pendingDate = computed(() => {
  return props.orderDate
})

const inProgressDate = computed(() => {
  if (!isInProgress.value) return null
  const date = new Date(props.orderDate)
  date.setMinutes(date.getMinutes() + 15)
  return date
})

const readyDate = computed(() => {
  if (!isReady.value) return null
  const date = new Date(props.orderDate)
  date.setMinutes(date.getMinutes() + 45)
  return date
})

const completedDate = computed(() => {
  if (!isCompleted.value) return null
  const date = new Date(props.orderDate)
  date.setHours(date.getHours() + 2)
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

