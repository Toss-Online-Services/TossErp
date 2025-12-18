<script setup lang="ts">
import { computed } from 'vue'
import type { Component } from 'vue'

// MaterialCard is auto-imported by Nuxt

type Accent = 'indigo' | 'emerald' | 'amber' | 'purple' | 'rose' | 'cyan' | 'green' | 'blue' | 'orange'

interface Props {
  // Support both 'title' and 'label' for flexibility
  title?: string
  label?: string
  value: string | number
  change?: string | number
  changeType?: 'positive' | 'negative' | 'neutral'
  subtitle?: string
  icon?: string | Component
  color?: Accent
  gradient?: Accent  // Alias for color
  prefix?: string
  sparklineData?: number[]
}

const props = withDefaults(defineProps<Props>(), {
  changeType: 'neutral',
  color: 'indigo'
})

// Compute the display title (supports both 'title' and 'label')
const displayTitle = computed(() => props.title || props.label || '')

// Compute the effective color (supports both 'color' and 'gradient')
const effectiveColor = computed(() => props.gradient || props.color)

// Format value with optional prefix
const formattedValue = computed(() => {
  const val = typeof props.value === 'number' ? props.value.toLocaleString() : props.value
  return props.prefix ? `${props.prefix}${val}` : val
})

// Format change value (convert number to string with % suffix)
const formattedChange = computed(() => {
  if (props.change === undefined || props.change === null) return null
  if (typeof props.change === 'number') {
    const sign = props.change >= 0 ? '+' : ''
    return `${sign}${props.change.toFixed(1)}%`
  }
  return props.change
})

// Determine change type automatically from number if not specified
const computedChangeType = computed(() => {
  if (props.changeType && props.changeType !== 'neutral') return props.changeType
  if (typeof props.change === 'number') {
    return props.change >= 0 ? 'positive' : 'negative'
  }
  return 'neutral'
})

const changeColorClass = computed(() => {
  switch (computedChangeType.value) {
    case 'positive':
      return 'text-[#4CAF50]'
    case 'negative':
      return 'text-[#F44335]'
    default:
      return 'text-[#737373]'
  }
})

const accentMap: Record<Accent, { icon: string; card: string }> = {
  indigo: { icon: 'bg-gradient-info text-white shadow-material-info', card: 'border-[#1A73E8]/20' },
  emerald: { icon: 'bg-gradient-success text-white shadow-material-success', card: 'border-[#4CAF50]/20' },
  amber: { icon: 'bg-gradient-warning text-white shadow-material-warning', card: 'border-[#fb8c00]/20' },
  purple: { icon: 'bg-gradient-primary text-white shadow-material-primary', card: 'border-[#e91e63]/20' },
  rose: { icon: 'bg-gradient-danger text-white shadow-material-danger', card: 'border-[#F44335]/20' },
  cyan: { icon: 'bg-gradient-info text-white shadow-material-info', card: 'border-[#1A73E8]/20' },
  green: { icon: 'bg-gradient-success text-white shadow-material-success', card: 'border-[#4CAF50]/20' },
  blue: { icon: 'bg-gradient-info text-white shadow-material-info', card: 'border-[#1A73E8]/20' },
  orange: { icon: 'bg-gradient-warning text-white shadow-material-warning', card: 'border-[#fb8c00]/20' }
}

const accentClasses = computed(() => accentMap[effectiveColor.value] || accentMap.indigo)

const iconIsComponent = computed(() => typeof props.icon === 'object' || typeof props.icon === 'function')
</script>

<template>
  <MaterialCard
    variant="elevated"
    class="p-6 hover:shadow-material-lg transition-all duration-300 border border-gray-200"
    :class="accentClasses?.card"
  >
    <div class="flex items-start justify-between gap-4">
      <div class="flex-1">
        <p class="text-xs font-semibold tracking-widest uppercase text-[#737373] mb-1">
          {{ displayTitle }}
        </p>
        <p class="text-3xl font-bold text-[#262626] mb-1 leading-tight">
          {{ formattedValue }}
        </p>
        <div v-if="formattedChange" class="flex items-center text-sm gap-2">
          <span :class="['font-semibold flex items-center gap-1', changeColorClass]">
            <Icon
              v-if="computedChangeType === 'positive'"
              name="lucide:arrow-up-right"
              class="w-3.5 h-3.5"
            />
            <Icon
              v-else-if="computedChangeType === 'negative'"
              name="lucide:arrow-down-right"
              class="w-3.5 h-3.5"
            />
            {{ formattedChange }}
          </span>
          <span v-if="subtitle" class="text-[#737373] text-xs">
            {{ subtitle }}
          </span>
        </div>
      </div>

      <div
        class="icon-shape icon-md border-radius-lg"
        :class="accentClasses?.icon"
      >
        <component
          v-if="icon && iconIsComponent"
          :is="icon"
          class="w-6 h-6"
        />
        <Icon
          v-else-if="typeof icon === 'string'"
          :name="icon"
          class="w-6 h-6"
        />
        <span v-else class="text-2xl">
          📊
        </span>
      </div>
    </div>
  </MaterialCard>
</template>
