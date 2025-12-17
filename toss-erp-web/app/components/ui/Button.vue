<script setup lang="ts">
import { computed } from 'vue'
import { cn } from '~/lib/utils'

const props = defineProps<{
  variant?: 'default' | 'secondary' | 'outline' | 'ghost' | 'link' | 'destructive'
  size?: 'default' | 'sm' | 'lg' | 'icon'
  class?: string
  disabled?: boolean
}>()

const buttonClass = computed(() => {
  const baseClass = 'inline-flex items-center justify-center whitespace-nowrap rounded-lg font-medium ring-offset-background transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50'
  
  const variantClasses = {
    default: 'text-sm shadow-sm hover:shadow-md bg-stone-800 hover:bg-stone-700 relative bg-gradient-to-b from-stone-700 to-stone-800 border border-stone-900 text-stone-50 duration-300 ease-in after:absolute after:inset-0 after:rounded-[inherit] after:shadow-[inset_0_1px_0px_rgba(255,255,255,0.25),inset_0_-2px_0px_rgba(0,0,0,0.35)] after:pointer-events-none',
    secondary: 'text-sm shadow-sm bg-transparent relative text-stone-700 dark:text-stone-300 border border-stone-300 dark:border-stone-600 duration-150 hover:border-stone-600 dark:hover:border-stone-400 hover:opacity-60',
    outline: 'border border-input bg-background hover:bg-accent hover:text-accent-foreground',
    ghost: 'hover:bg-accent hover:text-accent-foreground',
    link: 'text-primary underline-offset-4 hover:underline',
    destructive: 'bg-destructive text-destructive-foreground hover:bg-destructive/90'
  }
  
  const sizeClasses = {
    default: 'h-10 px-4 py-2',
    sm: 'h-9 px-3 text-sm',
    lg: 'h-11 px-8',
    icon: 'h-10 w-10'
  }
  
  return cn(
    baseClass,
    variantClasses[props.variant || 'default'],
    sizeClasses[props.size || 'default'],
    props.class
  )
})
</script>

<template>
  <button :class="buttonClass" :disabled="disabled">
    <slot />
  </button>
</template>


