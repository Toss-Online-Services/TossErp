<script setup lang="ts">
import { computed } from 'vue'

interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost'
  size?: 'sm' | 'md' | 'lg'
  disabled?: boolean
}

const props = withDefaults(defineProps<ButtonProps>(), {
  variant: 'primary',
  size: 'md',
  disabled: false
})

const buttonClasses = computed(() => {
  const base = 'inline-flex items-center justify-center rounded-[var(--ct-radius-lg)] font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50'
  
  const variants = {
    primary: 'bg-[hsl(var(--ct-primary))] text-white hover:opacity-90',
    secondary: 'bg-[hsl(var(--ct-surface-variant))] text-[hsl(var(--ct-on-surface))] border border-[hsl(var(--ct-border))] hover:bg-[hsl(var(--ct-border))]',
    outline: 'border border-[hsl(var(--ct-border))] text-[hsl(var(--ct-on-surface))] bg-transparent hover:bg-[hsl(var(--ct-surface-variant))]',
    ghost: 'text-[hsl(var(--ct-on-surface))] hover:bg-[hsl(var(--ct-surface-variant))]'
  }
  
  const sizes = {
    sm: 'h-9 px-3 text-sm',
    md: 'h-10 px-4 py-2',
    lg: 'h-11 px-8 text-lg'
  }
  
  return `${base} ${variants[props.variant]} ${sizes[props.size]}`
})
</script>

<template>
  <button :class="buttonClasses" :disabled="disabled">
    <slot />
  </button>
</template>

