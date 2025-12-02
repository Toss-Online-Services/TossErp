<template>
  <div :class="[
    'metric-card rounded-lg shadow-md p-6 relative overflow-hidden',
    colorClasses
  ]">
    <!-- Loading Overlay -->
    <div v-if="loading" class="absolute inset-0 bg-white/80 dark:bg-gray-800/80 flex items-center justify-center z-10">
      <Icon name="mdi:loading" class="w-8 h-8 animate-spin text-blue-600" />
    </div>

    <!-- Content -->
    <div class="flex items-start justify-between">
      <div class="flex-1">
        <p class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-2">
          {{ title }}
        </p>
        <p class="text-3xl font-bold text-gray-900 dark:text-white">
          {{ value }}
        </p>
        
        <!-- Change Indicator -->
        <div v-if="change !== undefined" class="mt-3 flex items-center space-x-2">
          <div :class="[
            'flex items-center px-2 py-1 rounded-full text-xs font-semibold',
            change >= 0 
              ? 'bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300' 
              : 'bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-300'
          ]">
            <Icon 
              :name="change >= 0 ? 'mdi:trending-up' : 'mdi:trending-down'" 
              class="mr-1"
            />
            {{ formatChange(change) }}
          </div>
          <span class="text-xs text-gray-500 dark:text-gray-400">
            {{ t('analytics.labels.vsLastPeriod') }}
          </span>
        </div>
      </div>

      <!-- Icon -->
      <div :class="[
        'flex items-center justify-center w-14 h-14 rounded-full',
        iconBgClasses
      ]">
        <Icon :name="icon" class="text-2xl text-white" />
      </div>
    </div>

    <!-- Background Decoration -->
    <div class="absolute bottom-0 right-0 opacity-10">
      <Icon :name="icon" class="text-9xl transform translate-x-6 translate-y-6" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface Props {
  title: string
  value: string
  change?: number
  icon: string
  color?: 'blue' | 'green' | 'purple' | 'orange' | 'red' | 'yellow'
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  color: 'blue',
  loading: false
})

const colorClasses = computed(() => {
  const classes = {
    blue: 'bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900/20 dark:to-blue-800/20',
    green: 'bg-gradient-to-br from-green-50 to-green-100 dark:from-green-900/20 dark:to-green-800/20',
    purple: 'bg-gradient-to-br from-purple-50 to-purple-100 dark:from-purple-900/20 dark:to-purple-800/20',
    orange: 'bg-gradient-to-br from-orange-50 to-orange-100 dark:from-orange-900/20 dark:to-orange-800/20',
    red: 'bg-gradient-to-br from-red-50 to-red-100 dark:from-red-900/20 dark:to-red-800/20',
    yellow: 'bg-gradient-to-br from-yellow-50 to-yellow-100 dark:from-yellow-900/20 dark:to-yellow-800/20'
  }
  return classes[props.color]
})

const iconBgClasses = computed(() => {
  const classes = {
    blue: 'bg-blue-500',
    green: 'bg-green-500',
    purple: 'bg-purple-500',
    orange: 'bg-orange-500',
    red: 'bg-red-500',
    yellow: 'bg-yellow-500'
  }
  return classes[props.color]
})

const formatChange = (change: number) => {
  const sign = change >= 0 ? '+' : ''
  return `${sign}${change.toFixed(1)}%`
}
</script>

<style scoped>
.metric-card {
  transition: transform 0.2s ease-in-out, box-shadow 0.2s ease-in-out;
}

.metric-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
}
</style>
