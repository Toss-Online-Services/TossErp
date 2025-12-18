<template>
  <button 
    :class="buttonClasses" 
    :disabled="disabled"
    @click="$emit('click', $event)"
  >
    <slot />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  color?: 'white' | 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark'
  variant?: 'text' | 'contained' | 'outlined' | 'gradient'
  size?: 'small' | 'medium' | 'large'
  circular?: boolean
  iconOnly?: boolean
  disabled?: boolean
  fullWidth?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  color: 'white',
  variant: 'contained',
  size: 'medium',
  circular: false,
  iconOnly: false,
  disabled: false,
  fullWidth: false
})

defineEmits<{
  click: [event: MouseEvent]
}>()

const buttonClasses = computed(() => {
  const classes = ['md-button', `md-button-${props.size}`, `md-button-${props.variant}`]
  
  if (props.variant === 'gradient' || props.variant === 'contained') {
    classes.push(`md-button-${props.color}`)
  } else if (props.variant === 'outlined' || props.variant === 'text') {
    classes.push(`md-button-${props.variant}-${props.color}`)
  }
  
  if (props.circular) {
    classes.push('md-button-circular')
  }
  
  if (props.iconOnly) {
    classes.push('md-button-icon-only')
  }
  
  if (props.fullWidth) {
    classes.push('md-button-full-width')
  }
  
  if (props.disabled) {
    classes.push('md-button-disabled')
  }
  
  return classes.join(' ')
})
</script>

<style scoped>
.md-button {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-family: 'Roboto', sans-serif;
  font-weight: 700;
  text-align: center;
  text-transform: uppercase;
  vertical-align: middle;
  user-select: none;
  border: 0;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 150ms ease-in;
  letter-spacing: 0.0357rem;
  will-change: transform, box-shadow;
  box-shadow: 0 3px 3px 0 rgba(0, 0, 0, 0.14), 0 3px 1px -2px rgba(0, 0, 0, 0.2), 0 1px 5px 0 rgba(0, 0, 0, 0.12);
}

.md-button:hover {
  transform: scale(1.02);
  box-shadow: 0 14px 26px -12px rgba(0, 0, 0, 0.4), 0 4px 23px 0 rgba(0, 0, 0, 0.15), 0 8px 10px -5px rgba(0, 0, 0, 0.2);
}

.md-button:active {
  transform: scale(0.98);
}

.md-button:focus {
  outline: none;
}

.md-button-disabled {
  opacity: 0.65;
  pointer-events: none;
  cursor: not-allowed;
}

.md-button-full-width {
  width: 100%;
}

/* Sizes */
.md-button-small {
  padding: 0.5rem 1rem;
  font-size: 0.75rem;
  line-height: 1.25;
}

.md-button-medium {
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  line-height: 1.5;
}

.md-button-large {
  padding: 0.875rem 2rem;
  font-size: 0.875rem;
  line-height: 1.75;
}

/* Icon only */
.md-button-icon-only {
  width: 2.5rem;
  min-width: 2.5rem;
  height: 2.5rem;
  padding: 0;
  border-radius: 50%;
}

.md-button-icon-only.md-button-small {
  width: 2rem;
  min-width: 2rem;
  height: 2rem;
}

.md-button-icon-only.md-button-large {
  width: 3rem;
  min-width: 3rem;
  height: 3rem;
}

/* Circular */
.md-button-circular {
  border-radius: 10rem;
}

/* Variants - Gradient & Contained */
.md-button-gradient,
.md-button-contained {
  color: white;
}

.md-button-primary {
  background-image: var(--md-gradient-primary);
  color: white;
}

.md-button-secondary {
  background-image: var(--md-gradient-secondary);
  color: white;
}

.md-button-info {
  background-image: var(--md-gradient-info);
  color: white;
}

.md-button-success {
  background-image: var(--md-gradient-success);
  color: white;
}

.md-button-warning {
  background-image: var(--md-gradient-warning);
  color: white;
}

.md-button-error {
  background-image: var(--md-gradient-error);
  color: white;
}

.md-button-dark {
  background-image: var(--md-gradient-dark);
  color: white;
}

.md-button-light {
  background-color: rgb(248, 249, 250);
  color: rgb(52, 71, 103);
  box-shadow: 0 3px 3px 0 rgba(248, 249, 250, 0.14), 0 3px 1px -2px rgba(248, 249, 250, 0.2), 0 1px 5px 0 rgba(248, 249, 250, 0.12);
}

.md-button-white {
  background-color: white;
  color: rgb(52, 71, 103);
  box-shadow: 0 3px 3px 0 rgba(255, 255, 255, 0.14), 0 3px 1px -2px rgba(255, 255, 255, 0.2), 0 1px 5px 0 rgba(255, 255, 255, 0.12);
}

/* Outlined variant */
.md-button-outlined {
  background-color: transparent;
  box-shadow: none;
  border: 1px solid;
}

.md-button-outlined-primary {
  border-color: var(--md-primary);
  color: var(--md-primary);
}

.md-button-outlined-secondary {
  border-color: var(--md-secondary);
  color: var(--md-secondary);
}

.md-button-outlined-info {
  border-color: var(--md-info);
  color: var(--md-info);
}

.md-button-outlined-success {
  border-color: var(--md-success);
  color: var(--md-success);
}

.md-button-outlined-warning {
  border-color: var(--md-warning);
  color: var(--md-warning);
}

.md-button-outlined-error {
  border-color: var(--md-error);
  color: var(--md-error);
}

.md-button-outlined-dark {
  border-color: var(--md-dark);
  color: var(--md-dark);
}

.md-button-outlined-light {
  border-color: var(--md-light);
  color: var(--md-light);
}

.md-button-outlined:hover {
  background-color: rgba(0, 0, 0, 0.04);
}

/* Text variant */
.md-button-text {
  background-color: transparent;
  box-shadow: none;
}

.md-button-text:hover {
  background-color: rgba(0, 0, 0, 0.04);
}

.md-button-text-primary {
  color: var(--md-primary);
}

.md-button-text-secondary {
  color: var(--md-secondary);
}

.md-button-text-info {
  color: var(--md-info);
}

.md-button-text-success {
  color: var(--md-success);
}

.md-button-text-warning {
  color: var(--md-warning);
}

.md-button-text-error {
  color: var(--md-error);
}

.md-button-text-dark {
  color: var(--md-dark);
}

.md-button-text-light {
  color: var(--md-light);
}
</style>
