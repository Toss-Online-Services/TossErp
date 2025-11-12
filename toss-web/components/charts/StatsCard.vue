<template>
  <div
  class="group relative overflow-hidden rounded-2xl p-6 shadow-lg transition-all duration-300 hover:-translate-y-1 hover:shadow-2xl"
    :class="gradientClass"
  >
    <div class="relative z-10 flex items-start space-x-4">
      <div class="flex shrink-0 items-center justify-center rounded-xl bg-white/20 p-3 backdrop-blur-sm">
        <component :is="icon" class="h-8 w-8 text-white" />
      </div>
      <div class="flex-1">
        <p class="mb-1 text-sm font-medium uppercase tracking-wide text-white/80">{{ label }}</p>
        <h3 class="mb-2 text-3xl font-bold text-white">{{ formattedValue }}</h3>
        <p v-if="change" class="flex items-center space-x-1 text-sm font-medium" :class="changeClass">
          <span class="text-base font-bold">{{ change > 0 ? '↑' : '↓' }}</span>
          {{ Math.abs(change) }}% {{ changeLabel }}
        </p>
      </div>
    </div>
    <div v-if="showSparkline && sparklineData" class="mt-4 border-t border-white/20 pt-4">
      <MiniChart :data="sparklineData" :color="sparklineColor" />
    </div>
    <div class="absolute top-0 right-0 h-32 w-32 translate-x-[40%] -translate-y-[40%] rounded-full bg-white/10" />
    <div class="absolute bottom-0 left-0 h-24 w-24 -translate-x-[40%] translate-y-[40%] rounded-full bg-black/10" />
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
  prefix: '',
})

const formattedValue = computed(() => {
  if (typeof props.value === 'number') {
    if (props.value >= 1_000_000) {
      return `${props.prefix}${(props.value / 1_000_000).toFixed(1)}M`
    }
    if (props.value >= 1_000) {
      return `${props.prefix}${(props.value / 1_000).toFixed(1)}k`
    }
    return `${props.prefix}${props.value.toLocaleString()}`
  }
  return props.value
})

const gradientClass = computed(() => {
  const gradients: Record<string, string> = {
    blue: 'bg-gradient-to-br from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700',
    green: 'bg-gradient-to-br from-green-500 to-emerald-600 hover:from-green-600 hover:to-emerald-700',
    purple: 'bg-gradient-to-br from-purple-500 to-pink-600 hover:from-purple-600 hover:to-pink-700',
    orange: 'bg-gradient-to-br from-orange-500 to-amber-600 hover:from-orange-600 hover:to-amber-700',
    red: 'bg-gradient-to-br from-red-500 to-rose-600 hover:from-red-600 hover:to-rose-700',
  }
  return gradients[props.gradient]
})

const changeClass = computed(() => {
  if (!props.change) {
    return ''
  }
  return props.change > 0 ? 'text-emerald-300' : 'text-red-300'
})

const sparklineColor = computed(() => {
  const colors: Record<string, string> = {
    blue: '#60A5FA',
    green: '#34D399',
    purple: '#A78BFA',
    orange: '#FBBF24',
    red: '#F87171',
  }
  return colors[props.gradient]
})
</script>
