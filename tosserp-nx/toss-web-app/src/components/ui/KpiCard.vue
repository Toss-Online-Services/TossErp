<script setup lang="ts">
import { computed } from 'vue'
import Card from './Card.vue'
import CardHeader from './CardHeader.vue'
import CardTitle from './CardTitle.vue'
import CardContent from './CardContent.vue'
import { cn } from '@/lib/utils'
import type { LucideIcon } from 'lucide-vue-next'

export interface KpiCardProps {
  title: string
  value: string | number
  change?: number
  changeLabel?: string
  icon?: LucideIcon
  status?: 'good' | 'warning' | 'bad' | 'neutral'
  trend?: 'up' | 'down' | 'neutral'
  class?: string
}

const props = withDefaults(defineProps<KpiCardProps>(), {
  status: 'neutral',
  trend: 'neutral'
})

const statusColors = {
  good: 'text-emerald-600',
  warning: 'text-amber-600',
  bad: 'text-red-600',
  neutral: 'text-muted-foreground'
}

const trendColors = {
  up: 'text-emerald-600',
  down: 'text-red-600',
  neutral: 'text-muted-foreground'
}

const formattedValue = computed(() => {
  if (typeof props.value === 'number') {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(props.value)
  }
  return props.value
})
</script>

<template>
  <Card :class="cn('hover:shadow-material-md transition-shadow', $props.class)">
    <CardHeader class="flex flex-row items-center justify-between pb-2">
      <CardTitle class="text-sm font-medium text-muted-foreground">
        {{ title }}
      </CardTitle>
      <component
        v-if="icon"
        :is="icon"
        :class="['h-4 w-4', statusColors[status]]"
      />
    </CardHeader>
    <CardContent>
      <div class="text-2xl font-bold">{{ formattedValue }}</div>
      <div v-if="change !== undefined" class="flex items-center gap-1 text-xs mt-1">
        <span :class="trendColors[trend]">
          {{ change > 0 ? '+' : '' }}{{ change }}%
        </span>
        <span v-if="changeLabel" class="text-muted-foreground">
          {{ changeLabel }}
        </span>
      </div>
    </CardContent>
  </Card>
</template>

