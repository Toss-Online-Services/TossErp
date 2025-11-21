<script setup lang="ts">
import { computed } from 'vue'
import MaterialCard from './MaterialCard.vue'

interface Props {
  label: string
  value: string | number
  icon?: any
  change?: number
  changeLabel?: string
  prefix?: string
  suffix?: string
  gradient?: 'none' | 'blue' | 'purple' | 'green' | 'orange' | 'red'
  showSparkline?: boolean
  sparklineData?: number[]
}

const props = withDefaults(defineProps<Props>(), {
  gradient: 'none',
  showSparkline: false,
  sparklineData: () => []
})

const isPositiveChange = computed(() => (props.change ?? 0) >= 0)

const formattedValue = computed(() => {
  let val = typeof props.value === 'number' ? props.value.toLocaleString() : props.value
  if (props.prefix) val = props.prefix + val
  if (props.suffix) val = val + props.suffix
  return val
})

const iconColorClass = computed(() => {
  const colors = {
    none: 'bg-slate-500',
    blue: 'bg-blue-500',
    purple: 'bg-purple-500',
    green: 'bg-green-500',
    orange: 'bg-orange-500',
    red: 'bg-red-500'
  }
  return colors[props.gradient]
})
</script>

<template>
  <MaterialCard
    variant="elevated"
    :gradient="gradient"
    hover
    class="p-6"
  >
    <div class="flex items-start justify-between">
      <div class="flex-1">
        <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-2">
          {{ label }}
        </p>
        <p class="text-3xl font-bold text-slate-900 dark:text-white mb-1">
          {{ formattedValue }}
        </p>
        <div v-if="change !== undefined" class="flex items-center text-sm">
          <span
            :class="[
              'font-semibold',
              isPositiveChange ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'
            ]"
          >
            {{ isPositiveChange ? '+' : '' }}{{ change.toFixed(1) }}%
          </span>
          <span v-if="changeLabel" class="ml-1 text-slate-600 dark:text-slate-400">
            {{ changeLabel }}
          </span>
        </div>
      </div>
      
      <div v-if="icon" :class="['p-3 rounded-lg', iconColorClass]">
        <component :is="icon" class="w-6 h-6 text-white" />
      </div>
    </div>
    
    <!-- Sparkline mini chart -->
    <div v-if="showSparkline && sparklineData.length" class="mt-4">
      <svg class="w-full h-12" viewBox="0 0 100 30" preserveAspectRatio="none">
        <polyline
          :points="sparklineData.map((v, i) => `${(i / (sparklineData.length - 1)) * 100},${30 - (v / Math.max(...sparklineData)) * 25}`).join(' ')"
          fill="none"
          :stroke="gradient === 'green' ? '#10b981' : gradient === 'blue' ? '#3b82f6' : '#f97316'"
          stroke-width="2"
          class="opacity-60"
        />
      </svg>
    </div>
  </MaterialCard>
</template>
