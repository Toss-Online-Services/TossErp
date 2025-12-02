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
  
  // Color and variant combinations - Material Dashboard Pro
  const colorVariants = {
    primary: {
      filled: 'bg-[#e91e63] text-white hover:bg-[#e91e63] shadow-material-button hover:shadow-material-button-hover focus:ring-[#e91e63]',
      outlined: 'border-2 border-[#e91e63] text-[#e91e63] hover:bg-[#e91e63]/5 focus:ring-[#e91e63]',
      text: 'text-[#e91e63] hover:bg-[#e91e63]/5',
      elevated: 'bg-[#e91e63] text-white hover:bg-[#e91e63] shadow-material-primary hover:shadow-material-button-hover focus:ring-[#e91e63]',
      tonal: 'bg-[#e91e63]/10 text-[#e91e63] hover:bg-[#e91e63]/20'
    },
    secondary: {
      filled: 'bg-[#737373] text-white hover:bg-[#737373] shadow-material hover:shadow-material-lg focus:ring-[#737373]',
      outlined: 'border-2 border-[#737373] text-[#737373] hover:bg-[#737373]/5 focus:ring-[#737373]',
      text: 'text-[#737373] hover:bg-[#737373]/5',
      elevated: 'bg-[#737373] text-white hover:bg-[#737373] shadow-material-lg hover:shadow-material-lg focus:ring-[#737373]',
      tonal: 'bg-[#737373]/10 text-[#737373] hover:bg-[#737373]/20'
    },
    success: {
      filled: 'bg-[#4CAF50] text-white hover:bg-[#4CAF50] shadow-material-success hover:shadow-material-lg focus:ring-[#4CAF50]',
      outlined: 'border-2 border-[#4CAF50] text-[#4CAF50] hover:bg-[#4CAF50]/5 focus:ring-[#4CAF50]',
      text: 'text-[#4CAF50] hover:bg-[#4CAF50]/5',
      elevated: 'bg-[#4CAF50] text-white hover:bg-[#4CAF50] shadow-material-success hover:shadow-material-lg focus:ring-[#4CAF50]',
      tonal: 'bg-[#4CAF50]/10 text-[#4CAF50] hover:bg-[#4CAF50]/20'
    },
    warning: {
      filled: 'bg-[#fb8c00] text-white hover:bg-[#fb8c00] shadow-material-warning hover:shadow-material-lg focus:ring-[#fb8c00]',
      outlined: 'border-2 border-[#fb8c00] text-[#fb8c00] hover:bg-[#fb8c00]/5 focus:ring-[#fb8c00]',
      text: 'text-[#fb8c00] hover:bg-[#fb8c00]/5',
      elevated: 'bg-[#fb8c00] text-white hover:bg-[#fb8c00] shadow-material-warning hover:shadow-material-lg focus:ring-[#fb8c00]',
      tonal: 'bg-[#fb8c00]/10 text-[#fb8c00] hover:bg-[#fb8c00]/20'
    },
    error: {
      filled: 'bg-[#F44335] text-white hover:bg-[#F44335] shadow-material-danger hover:shadow-material-lg focus:ring-[#F44335]',
      outlined: 'border-2 border-[#F44335] text-[#F44335] hover:bg-[#F44335]/5 focus:ring-[#F44335]',
      text: 'text-[#F44335] hover:bg-[#F44335]/5',
      elevated: 'bg-[#F44335] text-white hover:bg-[#F44335] shadow-material-danger hover:shadow-material-lg focus:ring-[#F44335]',
      tonal: 'bg-[#F44335]/10 text-[#F44335] hover:bg-[#F44335]/20'
    },
    info: {
      filled: 'bg-[#1A73E8] text-white hover:bg-[#1A73E8] shadow-material-info hover:shadow-material-lg focus:ring-[#1A73E8]',
      outlined: 'border-2 border-[#1A73E8] text-[#1A73E8] hover:bg-[#1A73E8]/5 focus:ring-[#1A73E8]',
      text: 'text-[#1A73E8] hover:bg-[#1A73E8]/5',
      elevated: 'bg-[#1A73E8] text-white hover:bg-[#1A73E8] shadow-material-info hover:shadow-material-lg focus:ring-[#1A73E8]',
      tonal: 'bg-[#1A73E8]/10 text-[#1A73E8] hover:bg-[#1A73E8]/20'
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
