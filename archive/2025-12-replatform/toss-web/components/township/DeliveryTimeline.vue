<template>
  <div class="bg-white dark:bg-gray-800 rounded-xl p-6 border border-gray-200 dark:border-gray-700 shadow-sm">
    <h3 class="text-lg sm:text-xl font-bold text-gray-900 dark:text-white mb-6">
      {{ $t('delivery.timeline') }}
    </h3>
    
    <div class="space-y-4">
      <div
        v-for="(step, index) in steps"
        :key="index"
        class="flex items-start gap-4"
      >
        <!-- Step Icon/Indicator -->
        <div class="flex-shrink-0 relative">
          <!-- Circle -->
          <div
            class="w-10 h-10 sm:w-12 sm:h-12 rounded-full flex items-center justify-center transition-all"
            :class="getStepCircleClass(step.status)"
          >
            <!-- Checkmark for completed -->
            <svg
              v-if="step.status === 'completed'"
              class="w-6 h-6 text-white"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path
                fill-rule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clip-rule="evenodd"
              />
            </svg>
            <!-- Pulse animation for current -->
            <span
              v-else-if="step.status === 'current'"
              class="w-4 h-4 bg-white rounded-full animate-pulse"
            ></span>
            <!-- Empty circle for pending -->
            <span
              v-else
              class="w-3 h-3 bg-gray-300 dark:bg-gray-600 rounded-full"
            ></span>
          </div>
          
          <!-- Connecting Line -->
          <div
            v-if="index < steps.length - 1"
            class="absolute left-1/2 top-12 w-0.5 h-full -ml-px"
            :class="step.status === 'completed' ? 'bg-green-500' : 'bg-gray-300 dark:bg-gray-600'"
          ></div>
        </div>
        
        <!-- Step Content -->
        <div class="flex-1 pb-8">
          <div class="flex items-start justify-between gap-2">
            <div class="flex-1">
              <h4
                class="text-base sm:text-lg font-semibold mb-1"
                :class="getStepTextClass(step.status)"
              >
                {{ $t(`delivery.steps.${step.key}.title`) }}
              </h4>
              <p
                class="text-sm sm:text-base mb-2"
                :class="step.status === 'pending' ? 'text-gray-500 dark:text-gray-500' : 'text-gray-700 dark:text-gray-300'"
              >
                {{ $t(`delivery.steps.${step.key}.description`) }}
              </p>
              <div
                v-if="step.timestamp"
                class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400"
              >
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>{{ formatTime(step.timestamp) }}</span>
              </div>
            </div>
            
            <!-- Status Badge -->
            <span
              v-if="step.status === 'current'"
              class="px-3 py-1 bg-blue-100 dark:bg-blue-900 text-blue-700 dark:text-blue-200 text-xs sm:text-sm font-medium rounded-full whitespace-nowrap"
            >
              {{ $t('delivery.current') }}
            </span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Estimated Arrival -->
    <div
      v-if="estimatedArrival"
      class="mt-6 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg border border-blue-200 dark:border-blue-800"
    >
      <div class="flex items-center gap-3">
        <svg class="w-6 h-6 text-blue-600 dark:text-blue-400 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <div>
          <p class="text-sm text-gray-600 dark:text-gray-400">{{ $t('delivery.estimatedArrival') }}</p>
          <p class="text-lg font-bold text-gray-900 dark:text-white">{{ formatTime(estimatedArrival) }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface DeliveryStep {
  key: string
  status: 'completed' | 'current' | 'pending'
  timestamp?: Date
}

interface Props {
  steps?: DeliveryStep[]
  estimatedArrival?: Date
}

const props = withDefaults(defineProps<Props>(), {
  steps: () => [
    { key: 'ordered', status: 'completed', timestamp: new Date(Date.now() - 3600000) },
    { key: 'preparing', status: 'completed', timestamp: new Date(Date.now() - 1800000) },
    { key: 'outForDelivery', status: 'current', timestamp: new Date(Date.now() - 300000) },
    { key: 'delivered', status: 'pending' }
  ],
  estimatedArrival: () => new Date(Date.now() + 1800000) // 30 minutes from now
})

const getStepCircleClass = (status: string) => {
  switch (status) {
    case 'completed':
      return 'bg-green-500 shadow-lg shadow-green-500/50'
    case 'current':
      return 'bg-blue-500 shadow-lg shadow-blue-500/50'
    default:
      return 'bg-gray-200 dark:bg-gray-700'
  }
}

const getStepTextClass = (status: string) => {
  switch (status) {
    case 'completed':
      return 'text-green-600 dark:text-green-400'
    case 'current':
      return 'text-blue-600 dark:text-blue-400'
    default:
      return 'text-gray-500 dark:text-gray-500'
  }
}

const formatTime = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    hour: '2-digit',
    minute: '2-digit',
    day: 'numeric',
    month: 'short'
  }).format(date)
}
</script>

