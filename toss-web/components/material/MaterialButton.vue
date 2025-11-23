<script setup lang="ts">
import { computed } from 'vue'
import { cn } from '~/lib/utils'

interface Props {
  variant?: 'filled' | 'outlined' | 'text' | 'elevated' | 'tonal'
  color?: 'primary' | 'secondary' | 'success' | 'warning' | 'error' | 'info'
  size?: 'sm' | 'md' | 'lg' | 'xl'
  disabled?: boolean
  loading?: boolean
  fullWidth?: boolean
  icon?: boolean
  class?: string
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'filled',
  color: 'primary',
  size: 'md',
  disabled: false,
  loading: false,
  fullWidth: false,
  icon: false
})

const buttonClasses = computed(() => {
  const base = 'inline-flex items-center justify-center font-semibold rounded-lg transition-all duration-300 focus:outline-none focus:ring-4 focus:ring-opacity-50'
  
  // Size variants
  const sizes = {
    sm: props.icon ? 'p-2' : 'px-4 py-2 text-sm',
    md: props.icon ? 'p-2.5' : 'px-6 py-2.5 text-base',
    lg: props.icon ? 'p-3' : 'px-8 py-3 text-lg',
    xl: props.icon ? 'p-4' : 'px-10 py-4 text-xl'
  }
  
  // Color and variant combinations
  const colorVariants = {
    primary: {
      filled: 'bg-primary text-primary-foreground hover:bg-primary/90 shadow-lg hover:shadow-xl focus:ring-primary',
      outlined: 'border-2 border-primary text-primary hover:bg-primary/5 focus:ring-primary',
      text: 'text-primary hover:bg-primary/5',
      elevated: 'bg-primary text-primary-foreground hover:bg-primary/90 shadow-xl hover:shadow-2xl focus:ring-primary',
      tonal: 'bg-primary/10 text-primary hover:bg-primary/20'
    },
    secondary: {
      filled: 'bg-slate-500 text-white hover:bg-slate-600 shadow-lg hover:shadow-xl focus:ring-slate-500',
      outlined: 'border-2 border-slate-500 text-slate-600 hover:bg-slate-50 dark:hover:bg-slate-900/30 focus:ring-slate-500',
      text: 'text-slate-600 hover:bg-slate-50 dark:hover:bg-slate-900/30',
      elevated: 'bg-slate-500 text-white hover:bg-slate-600 shadow-xl hover:shadow-2xl focus:ring-slate-500',
      tonal: 'bg-slate-100 text-slate-700 hover:bg-slate-200 dark:bg-slate-800 dark:text-slate-300 dark:hover:bg-slate-700'
    },
    success: {
      filled: 'bg-green-500 text-white hover:bg-green-600 shadow-lg hover:shadow-xl focus:ring-green-500',
      outlined: 'border-2 border-green-500 text-green-600 hover:bg-green-50 dark:hover:bg-green-950/30 focus:ring-green-500',
      text: 'text-green-600 hover:bg-green-50 dark:hover:bg-green-950/30',
      elevated: 'bg-green-500 text-white hover:bg-green-600 shadow-xl hover:shadow-2xl focus:ring-green-500',
      tonal: 'bg-green-100 text-green-700 hover:bg-green-200 dark:bg-green-900/30 dark:text-green-300 dark:hover:bg-green-900/50'
    },
    warning: {
      filled: 'bg-yellow-500 text-white hover:bg-yellow-600 shadow-lg hover:shadow-xl focus:ring-yellow-500',
      outlined: 'border-2 border-yellow-500 text-yellow-600 hover:bg-yellow-50 dark:hover:bg-yellow-950/30 focus:ring-yellow-500',
      text: 'text-yellow-600 hover:bg-yellow-50 dark:hover:bg-yellow-950/30',
      elevated: 'bg-yellow-500 text-white hover:bg-yellow-600 shadow-xl hover:shadow-2xl focus:ring-yellow-500',
      tonal: 'bg-yellow-100 text-yellow-700 hover:bg-yellow-200 dark:bg-yellow-900/30 dark:text-yellow-300 dark:hover:bg-yellow-900/50'
    },
    error: {
      filled: 'bg-red-500 text-white hover:bg-red-600 shadow-lg hover:shadow-xl focus:ring-red-500',
      outlined: 'border-2 border-red-500 text-red-600 hover:bg-red-50 dark:hover:bg-red-950/30 focus:ring-red-500',
      text: 'text-red-600 hover:bg-red-50 dark:hover:bg-red-950/30',
      elevated: 'bg-red-500 text-white hover:bg-red-600 shadow-xl hover:shadow-2xl focus:ring-red-500',
      tonal: 'bg-red-100 text-red-700 hover:bg-red-200 dark:bg-red-900/30 dark:text-red-300 dark:hover:bg-red-900/50'
    },
    info: {
      filled: 'bg-blue-500 text-white hover:bg-blue-600 shadow-lg hover:shadow-xl focus:ring-blue-500',
      outlined: 'border-2 border-blue-500 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-950/30 focus:ring-blue-500',
      text: 'text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-950/30',
      elevated: 'bg-blue-500 text-white hover:bg-blue-600 shadow-xl hover:shadow-2xl focus:ring-blue-500',
      tonal: 'bg-blue-100 text-blue-700 hover:bg-blue-200 dark:bg-blue-900/30 dark:text-blue-300 dark:hover:bg-blue-900/50'
    }
  }
  
  const disabledClass = 'opacity-50 cursor-not-allowed pointer-events-none'
  const widthClass = props.fullWidth ? 'w-full' : ''
  const iconClass = props.icon ? 'rounded-full' : ''
  
  return cn(
    base,
    sizes[props.size],
    colorVariants[props.color][props.variant],
    props.disabled || props.loading ? disabledClass : '',
    widthClass,
    iconClass,
    props.class
  )
})
</script>

<template>
  <button
    :class="buttonClasses"
    :disabled="disabled || loading"
    v-bind="$attrs"
  >
    <span v-if="loading" class="mr-2">
      <svg class="animate-spin h-5 w-5" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
      </svg>
    </span>
    <slot />
  </button>
</template>
