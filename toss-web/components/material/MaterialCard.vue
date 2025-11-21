<script setup lang="ts">
import { computed } from 'vue'
import { cn } from '~/lib/utils'

interface Props {
  variant?: 'default' | 'elevated' | 'outlined' | 'filled'
  hover?: boolean
  clickable?: boolean
  gradient?: 'none' | 'blue' | 'purple' | 'green' | 'orange' | 'red'
  class?: string
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  hover: false,
  clickable: false,
  gradient: 'none'
})

const cardClasses = computed(() => {
  const base = 'rounded-xl transition-all duration-300'
  
  const variants = {
    default: 'bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-700',
    elevated: 'bg-white dark:bg-slate-800 shadow-lg',
    outlined: 'bg-transparent border-2 border-slate-200 dark:border-slate-700',
    filled: 'bg-slate-50 dark:bg-slate-700/50'
  }
  
  const gradients = {
    none: '',
    blue: 'bg-gradient-to-br from-blue-50 to-blue-100/50 dark:from-blue-900/20 dark:to-blue-900/10 border-blue-200/50 dark:border-blue-800/50',
    purple: 'bg-gradient-to-br from-purple-50 to-purple-100/50 dark:from-purple-900/20 dark:to-purple-900/10 border-purple-200/50 dark:border-purple-800/50',
    green: 'bg-gradient-to-br from-green-50 to-green-100/50 dark:from-green-900/20 dark:to-green-900/10 border-green-200/50 dark:border-green-800/50',
    orange: 'bg-gradient-to-br from-orange-50 to-orange-100/50 dark:from-orange-900/20 dark:to-orange-900/10 border-orange-200/50 dark:border-orange-800/50',
    red: 'bg-gradient-to-br from-red-50 to-red-100/50 dark:from-red-900/20 dark:to-red-900/10 border-red-200/50 dark:border-red-800/50'
  }
  
  const hoverClass = props.hover ? 'hover:shadow-xl hover:-translate-y-1' : ''
  const clickableClass = props.clickable ? 'cursor-pointer' : ''
  const gradientClass = props.gradient !== 'none' ? gradients[props.gradient] : variants[props.variant]
  
  return cn(base, gradientClass, hoverClass, clickableClass, props.class)
})
</script>

<template>
  <div :class="cardClasses">
    <slot />
  </div>
</template>
