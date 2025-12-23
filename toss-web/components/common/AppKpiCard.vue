<script setup lang="ts">
import type { Component } from 'vue'

interface Props {
  title: string
  value: string | number
  change?: string | number
  changeType?: 'positive' | 'negative' | 'neutral'
  icon?: Component
  trend?: 'up' | 'down' | 'neutral'
  iconVariant?: 'primary' | 'info' | 'success' | 'warning' | 'danger' | 'dark'
}

const props = withDefaults(defineProps<Props>(), {
  changeType: 'neutral',
  trend: 'neutral',
  iconVariant: 'dark'
})

const iconGradients: Record<string, string> = {
  primary: 'linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%)',
  info: 'linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%)',
  success: 'linear-gradient(195deg, #66BB6A 0%, #43A047 100%)',
  warning: 'linear-gradient(195deg, #FFA726 0%, #FB8C00 100%)',
  danger: 'linear-gradient(195deg, #EF5350 0%, #E53935 100%)',
  dark: 'linear-gradient(195deg, #42424a 0%, #191919 100%)'
}
</script>

<template>
  <div class="bg-card border-0 rounded-xl p-4 lg:p-5 relative overflow-visible" style="box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);">
    <div class="flex items-start justify-between">
      <div 
        v-if="icon" 
        class="flex items-center justify-center w-14 h-14 rounded-xl text-white absolute -top-4 left-4"
        :style="{ 
          background: iconGradients[iconVariant],
          boxShadow: iconVariant === 'primary' 
            ? '0 4px 20px 0 rgba(26, 115, 232, 0.14), 0 7px 10px -5px rgba(26, 115, 232, 0.4)'
            : iconVariant === 'info'
            ? '0 4px 20px 0 rgba(26, 115, 232, 0.14), 0 7px 10px -5px rgba(26, 115, 232, 0.4)'
            : iconVariant === 'success'
            ? '0 4px 20px 0 rgba(76, 175, 80, 0.14), 0 7px 10px -5px rgba(76, 175, 80, 0.4)'
            : iconVariant === 'warning'
            ? '0 4px 20px 0 rgba(251, 140, 0, 0.14), 0 7px 10px -5px rgba(251, 140, 0, 0.4)'
            : iconVariant === 'danger'
            ? '0 4px 20px 0 rgba(244, 67, 53, 0.14), 0 7px 10px -5px rgba(244, 67, 53, 0.4)'
            : '0 4px 20px 0 rgba(0, 0, 0, 0.14), 0 7px 10px -5px rgba(0, 0, 0, 0.4)'
        }"
      >
        <component :is="icon" :size="24" class="text-white" />
      </div>
      <div class="flex-1 text-right pt-1">
        <p class="text-sm text-muted-foreground mb-0 font-normal">{{ title }}</p>
        <p class="text-xl lg:text-2xl font-bold text-foreground mb-0">{{ value }}</p>
      </div>
    </div>
    <hr class="my-3 border-t border-gray-200 dark:border-gray-700 opacity-50" />
    <div v-if="change !== undefined" class="flex items-center gap-1.5">
      <span
        :class="[
          'text-sm font-bold',
          changeType === 'positive' ? 'text-green-600 dark:text-green-400' : '',
          changeType === 'negative' ? 'text-red-600 dark:text-red-400' : '',
          changeType === 'neutral' ? 'text-muted-foreground' : ''
        ]"
      >
        {{ typeof change === 'number' && change > 0 ? '+' : '' }}{{ change }}
      </span>
      <span class="text-sm text-muted-foreground font-normal">vs last period</span>
    </div>
  </div>
</template>


