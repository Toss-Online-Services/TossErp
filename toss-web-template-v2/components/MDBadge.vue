<template>
  <span :class="badgeClasses">
    <slot />
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  color?: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark'
  variant?: 'contained' | 'gradient'
  size?: 'xs' | 'sm' | 'md' | 'lg'
  circular?: boolean
  indicator?: boolean
  badgeContent?: string | number
  container?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  color: 'info',
  variant: 'gradient',
  size: 'sm',
  circular: false,
  indicator: false,
  container: false
})

const badgeClasses = computed(() => {
  const classes = ['md-badge', `md-badge-${props.size}`]
  
  if (props.variant === 'gradient') {
    classes.push('md-badge-gradient', `md-badge-gradient-${props.color}`)
  } else {
    classes.push('md-badge-contained', `md-badge-contained-${props.color}`)
  }
  
  if (props.circular) {
    classes.push('md-badge-circular')
  }
  
  if (props.indicator) {
    classes.push('md-badge-indicator')
  }
  
  if (props.container) {
    classes.push('md-badge-container')
  }
  
  return classes.join(' ')
})
</script>

<style scoped>
.md-badge {
  display: inline-block;
  padding: 0.45em 0.775em;
  font-size: 0.75rem;
  font-weight: 700;
  line-height: 1;
  color: white;
  text-align: center;
  white-space: nowrap;
  vertical-align: baseline;
  border-radius: 0.5rem;
}

/* Sizes */
.md-badge-xs {
  padding: 0.25em 0.5em;
  font-size: 0.65rem;
}

.md-badge-sm {
  padding: 0.35em 0.65em;
  font-size: 0.7rem;
}

.md-badge-md {
  padding: 0.45em 0.775em;
  font-size: 0.75rem;
}

.md-badge-lg {
  padding: 0.55em 0.9em;
  font-size: 0.875rem;
}

/* Circular */
.md-badge-circular {
  border-radius: 10rem;
}

/* Indicator */
.md-badge-indicator {
  width: 0.5rem;
  height: 0.5rem;
  padding: 0;
  border-radius: 50%;
}

/* Container */
.md-badge-container {
  position: relative;
  display: inline-block;
}

/* Gradient variants */
.md-badge-gradient-primary {
  background-image: var(--md-gradient-primary);
}

.md-badge-gradient-secondary {
  background-image: var(--md-gradient-secondary);
}

.md-badge-gradient-info {
  background-image: var(--md-gradient-info);
}

.md-badge-gradient-success {
  background-image: var(--md-gradient-success);
}

.md-badge-gradient-warning {
  background-image: var(--md-gradient-warning);
}

.md-badge-gradient-error {
  background-image: var(--md-gradient-error);
}

.md-badge-gradient-dark {
  background-image: var(--md-gradient-dark);
}

.md-badge-gradient-light {
  background-color: rgb(248, 249, 250);
  color: rgb(52, 71, 103);
}

/* Contained variants */
.md-badge-contained-primary {
  background-color: var(--md-primary);
}

.md-badge-contained-secondary {
  background-color: var(--md-secondary);
}

.md-badge-contained-info {
  background-color: var(--md-info);
}

.md-badge-contained-success {
  background-color: var(--md-success);
}

.md-badge-contained-warning {
  background-color: var(--md-warning);
}

.md-badge-contained-error {
  background-color: var(--md-error);
}

.md-badge-contained-dark {
  background-color: var(--md-dark);
}

.md-badge-contained-light {
  background-color: rgb(248, 249, 250);
  color: rgb(52, 71, 103);
}
</style>
