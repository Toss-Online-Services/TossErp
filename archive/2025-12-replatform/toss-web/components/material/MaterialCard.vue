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
  const base = 'border-radius-lg transition-all duration-300'
  
  const variants = {
    default: 'bg-white dark:bg-slate-800 border border-gray-200 dark:border-slate-700 shadow-material',
    elevated: 'bg-white dark:bg-slate-800 shadow-material-lg border-0',
    outlined: 'bg-transparent border-2 border-gray-200 dark:border-slate-700',
    filled: 'bg-gray-100 dark:bg-slate-700/50 border-0'
  }
  
  const gradients = {
    none: '',
    blue: 'bg-gradient-info border-0 shadow-material-info',
    purple: 'bg-gradient-primary border-0 shadow-material-primary',
    green: 'bg-gradient-success border-0 shadow-material-success',
    orange: 'bg-gradient-warning border-0 shadow-material-warning',
    red: 'bg-gradient-danger border-0 shadow-material-danger'
  }
  
  const hoverClass = props.hover ? 'hover:shadow-material-lg hover:-translate-y-1' : ''
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
