<script setup lang="ts">
import type { Component } from 'vue'

interface Props {
  title: string
  value: string | number
  change?: string | number
  changeType?: 'positive' | 'negative' | 'neutral'
  icon?: Component
  trend?: 'up' | 'down' | 'neutral'
}

const props = withDefaults(defineProps<Props>(), {
  changeType: 'neutral',
  trend: 'neutral'
})
</script>

<template>
  <div class="bg-card border border-border rounded-lg p-4 lg:p-6 shadow-sm hover:shadow-md transition-shadow">
    <div class="flex items-start justify-between">
      <div class="flex-1">
        <p class="text-sm text-muted-foreground mb-1">{{ title }}</p>
        <p class="text-2xl lg:text-3xl font-bold text-foreground">{{ value }}</p>
        <div v-if="change !== undefined" class="flex items-center gap-1 mt-2">
          <span
            :class="[
              'text-xs font-medium',
              changeType === 'positive' ? 'text-green-600 dark:text-green-400' : '',
              changeType === 'negative' ? 'text-red-600 dark:text-red-400' : '',
              changeType === 'neutral' ? 'text-muted-foreground' : ''
            ]"
          >
            {{ typeof change === 'number' && change > 0 ? '+' : '' }}{{ change }}
          </span>
          <span class="text-xs text-muted-foreground">vs last period</span>
        </div>
      </div>
      <div v-if="icon" class="p-2 rounded-lg bg-muted">
        <component :is="icon" :size="20" class="text-foreground" />
      </div>
    </div>
  </div>
</template>


