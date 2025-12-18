<template>
  <component 
    :is="tag" 
    :class="boxClasses" 
    :style="boxStyles"
  >
    <slot />
  </component>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  tag?: string
  variant?: 'contained' | 'gradient'
  bgColor?: string
  color?: string
  opacity?: number
  borderRadius?: string
  shadow?: string
  coloredShadow?: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark' | 'none'
  p?: number | string
  py?: number | string
  px?: number | string
  m?: number | string
  my?: number | string
  mx?: number | string
}

const props = withDefaults(defineProps<Props>(), {
  tag: 'div',
  variant: 'contained',
  bgColor: 'transparent',
  color: 'dark',
  opacity: 1,
  borderRadius: 'none',
  shadow: 'none',
  coloredShadow: 'none',
  p: undefined,
  py: undefined,
  px: undefined,
  m: undefined,
  my: undefined,
  mx: undefined
})

const boxClasses = computed(() => {
  const classes = ['md-box']
  
  if (props.variant === 'gradient') {
    classes.push(`md-box-gradient-${props.bgColor}`)
  }
  
  if (props.shadow && props.shadow !== 'none') {
    classes.push(`md-box-shadow-${props.shadow}`)
  }
  
  if (props.coloredShadow && props.coloredShadow !== 'none') {
    classes.push(`md-box-colored-shadow-${props.coloredShadow}`)
  }
  
  return classes.join(' ')
})

const boxStyles = computed(() => {
  const styles: Record<string, any> = {}
  
  if (props.variant === 'contained' && props.bgColor !== 'transparent') {
    styles.backgroundColor = `var(--md-${props.bgColor}, ${props.bgColor})`
  }
  
  if (props.color) {
    styles.color = `var(--md-${props.color}, ${props.color})`
  }
  
  if (props.opacity !== 1) {
    styles.opacity = props.opacity
  }
  
  if (props.borderRadius && props.borderRadius !== 'none') {
    styles.borderRadius = props.borderRadius.includes('px') ? props.borderRadius : `${props.borderRadius}rem`
  }
  
  // Padding
  if (props.p !== undefined) {
    styles.padding = typeof props.p === 'number' ? `${props.p}rem` : props.p
  }
  if (props.py !== undefined) {
    const val = typeof props.py === 'number' ? `${props.py}rem` : props.py
    styles.paddingTop = val
    styles.paddingBottom = val
  }
  if (props.px !== undefined) {
    const val = typeof props.px === 'number' ? `${props.px}rem` : props.px
    styles.paddingLeft = val
    styles.paddingRight = val
  }
  
  // Margin
  if (props.m !== undefined) {
    styles.margin = typeof props.m === 'number' ? `${props.m}rem` : props.m
  }
  if (props.my !== undefined) {
    const val = typeof props.my === 'number' ? `${props.my}rem` : props.my
    styles.marginTop = val
    styles.marginBottom = val
  }
  if (props.mx !== undefined) {
    const val = typeof props.mx === 'number' ? `${props.mx}rem` : props.mx
    styles.marginLeft = val
    styles.marginRight = val
  }
  
  return styles
})
</script>

<style scoped>
.md-box {
  position: relative;
  display: block;
}

/* Gradient backgrounds */
.md-box-gradient-primary {
  background-image: var(--md-gradient-primary);
}

.md-box-gradient-secondary {
  background-image: var(--md-gradient-secondary);
}

.md-box-gradient-info {
  background-image: var(--md-gradient-info);
}

.md-box-gradient-success {
  background-image: var(--md-gradient-success);
}

.md-box-gradient-warning {
  background-image: var(--md-gradient-warning);
}

.md-box-gradient-error {
  background-image: var(--md-gradient-error);
}

.md-box-gradient-dark {
  background-image: var(--md-gradient-dark);
}

/* Shadows */
.md-box-shadow-xs {
  box-shadow: var(--md-box-shadow-xs);
}

.md-box-shadow-sm {
  box-shadow: var(--md-box-shadow-sm);
}

.md-box-shadow-md {
  box-shadow: var(--md-box-shadow-md);
}

.md-box-shadow-lg {
  box-shadow: var(--md-box-shadow-lg);
}

.md-box-shadow-xl {
  box-shadow: var(--md-box-shadow-xl);
}

.md-box-shadow-xxl {
  box-shadow: var(--md-box-shadow-xxl);
}

/* Colored shadows */
.md-box-colored-shadow-primary {
  box-shadow: 0 4px 20px 0 rgba(26, 115, 232, 0.14), 0 7px 10px -5px rgba(26, 115, 232, 0.4);
}

.md-box-colored-shadow-secondary {
  box-shadow: 0 4px 20px 0 rgba(124, 77, 255, 0.14), 0 7px 10px -5px rgba(124, 77, 255, 0.4);
}

.md-box-colored-shadow-info {
  box-shadow: 0 4px 20px 0 rgba(26, 115, 232, 0.14), 0 7px 10px -5px rgba(26, 115, 232, 0.4);
}

.md-box-colored-shadow-success {
  box-shadow: 0 4px 20px 0 rgba(56, 142, 60, 0.14), 0 7px 10px -5px rgba(56, 142, 60, 0.4);
}

.md-box-colored-shadow-warning {
  box-shadow: 0 4px 20px 0 rgba(251, 140, 0, 0.14), 0 7px 10px -5px rgba(251, 140, 0, 0.4);
}

.md-box-colored-shadow-error {
  box-shadow: 0 4px 20px 0 rgba(244, 67, 54, 0.14), 0 7px 10px -5px rgba(244, 67, 54, 0.4);
}

.md-box-colored-shadow-dark {
  box-shadow: 0 4px 20px 0 rgba(0, 0, 0, 0.14), 0 7px 10px -5px rgba(0, 0, 0, 0.4);
}
</style>
