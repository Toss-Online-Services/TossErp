<template>
  <div 
    class="stats-card group"
    :class="gradientClass"
  >
    <div class="stats-content">
      <div class="stats-icon-wrapper">
        <component :is="icon" class="stats-icon" />
      </div>
      <div class="stats-details">
        <p class="stats-label">{{ label }}</p>
        <h3 class="stats-value">{{ formattedValue }}</h3>
        <p v-if="change" class="stats-change" :class="changeClass">
          <span class="change-icon">{{ change > 0 ? '↑' : '↓' }}</span>
          {{ Math.abs(change) }}% {{ changeLabel }}
        </p>
      </div>
    </div>
    <div v-if="showSparkline && sparklineData" class="sparkline-wrapper">
      <MiniChart :data="sparklineData" :color="sparklineColor" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Component } from 'vue'

interface Props {
  label: string
  value: number | string
  icon: Component
  change?: number
  changeLabel?: string
  gradient?: 'blue' | 'green' | 'purple' | 'orange' | 'red'
  showSparkline?: boolean
  sparklineData?: number[]
  prefix?: string
}

const props = withDefaults(defineProps<Props>(), {
  changeLabel: 'vs last month',
  gradient: 'blue',
  showSparkline: false,
  prefix: ''
})

const formattedValue = computed(() => {
  if (typeof props.value === 'number') {
    if (props.value >= 1000000) {
      return `${props.prefix}${(props.value / 1000000).toFixed(1)}M`
    } else if (props.value >= 1000) {
      return `${props.prefix}${(props.value / 1000).toFixed(1)}k`
    }
    return `${props.prefix}${props.value.toLocaleString()}`
  }
  return props.value
})

const gradientClass = computed(() => {
  const gradients: Record<string, string> = {
    blue: 'bg-gradient-to-br from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700',
    green: 'bg-gradient-to-br from-green-500 to-emerald-600 hover:from-green-600 hover:to-emerald-700',
    purple: 'bg-gradient-to-br from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700',
    orange: 'bg-gradient-to-br from-orange-500 to-amber-600 hover:from-orange-600 hover:to-amber-700',
    red: 'bg-gradient-to-br from-red-500 to-rose-600 hover:from-red-600 hover:to-rose-700'
  }
  return gradients[props.gradient]
})

const changeClass = computed(() => {
  if (!props.change) return ''
  return props.change > 0 ? 'text-emerald-300' : 'text-red-300'
})

const sparklineColor = computed(() => {
  const colors: Record<string, string> = {
    blue: '#60A5FA',
    green: '#34D399',
    purple: '#A78BFA',
    orange: '#FBBF24',
    red: '#F87171'
  }
  return colors[props.gradient]
})
</script>

<style scoped>
.stats-card {
  @apply relative overflow-hidden rounded-2xl shadow-lg transition-all duration-300;
  @apply p-6;
}

.stats-card:hover {
  @apply shadow-2xl transform -translate-y-1;
}

.stats-content {
  @apply relative z-10 flex items-start space-x-4;
}

.stats-icon-wrapper {
  @apply flex-shrink-0 p-3 bg-white/20 backdrop-blur-sm rounded-xl;
}

.stats-icon {
  @apply w-8 h-8 text-white;
}

.stats-details {
  @apply flex-1;
}

.stats-label {
  @apply text-white/80 text-sm font-medium uppercase tracking-wide mb-1;
}

.stats-value {
  @apply text-white text-3xl font-bold mb-2;
}

.stats-change {
  @apply text-sm font-medium flex items-center space-x-1;
}

.change-icon {
  @apply font-bold text-base;
}

.sparkline-wrapper {
  @apply mt-4 pt-4 border-t border-white/20;
}

/* Decorative background elements */
.stats-card::before {
  content: '';
  @apply absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full;
  transform: translate(40%, -40%);
}

.stats-card::after {
  content: '';
  @apply absolute bottom-0 left-0 w-24 h-24 bg-black/10 rounded-full;
  transform: translate(-40%, 40%);
}
</style>

