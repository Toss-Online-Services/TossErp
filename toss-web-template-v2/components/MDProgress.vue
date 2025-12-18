<template>
  <div class="progress-container" :style="containerStyle">
    <div 
      class="progress-bar" 
      :class="progressClasses"
      :style="progressStyle"
      role="progressbar"
      :aria-valuenow="value"
      aria-valuemin="0"
      aria-valuemax="100"
    >
      <span v-if="label" class="progress-label">{{ label }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  value?: number
  color?: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark'
  variant?: 'contained' | 'gradient'
  label?: string
  height?: string
}

const props = withDefaults(defineProps<Props>(), {
  value: 0,
  color: 'info',
  variant: 'gradient',
  label: '',
  height: '0.375rem'
})

const progressClasses = computed(() => {
  const classes: string[] = []
  
  if (props.variant === 'gradient') {
    classes.push(`bg-gradient-${props.color}`)
  } else {
    classes.push(`bg-${props.color}`)
  }
  
  return classes
})

const progressStyle = computed(() => ({
  width: `${Math.min(Math.max(props.value, 0), 100)}%`,
  transition: 'width 0.6s ease'
}))

const containerStyle = computed(() => ({
  height: props.height
}))
</script>

<style scoped>
.progress-container {
  width: 100%;
  background-color: #e9ecef;
  border-radius: 0.25rem;
  overflow: hidden;
  position: relative;
}

.progress-bar {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
  white-space: nowrap;
  border-radius: 0.25rem;
}

.progress-label {
  padding: 0 0.5rem;
}

/* Gradient backgrounds */
.bg-gradient-primary {
  background: linear-gradient(310deg, var(--md-gradient-primary-main, #7928ca), var(--md-gradient-primary-state, #ff0080));
}

.bg-gradient-secondary {
  background: linear-gradient(310deg, var(--md-gradient-secondary-main, #627594), var(--md-gradient-secondary-state, #a8b8d8));
}

.bg-gradient-info {
  background: linear-gradient(310deg, var(--md-gradient-info-main, #2152ff), var(--md-gradient-info-state, #21d4fd));
}

.bg-gradient-success {
  background: linear-gradient(310deg, var(--md-gradient-success-main, #17ad37), var(--md-gradient-success-state, #98ec2d));
}

.bg-gradient-warning {
  background: linear-gradient(310deg, var(--md-gradient-warning-main, #f53939), var(--md-gradient-warning-state, #fbcf33));
}

.bg-gradient-error {
  background: linear-gradient(310deg, var(--md-gradient-error-main, #ea0606), var(--md-gradient-error-state, #ff667c));
}

.bg-gradient-light {
  background: linear-gradient(310deg, var(--md-gradient-light-main, #ced4da), var(--md-gradient-light-state, #ebeff4));
}

.bg-gradient-dark {
  background: linear-gradient(310deg, var(--md-gradient-dark-main, #141727), var(--md-gradient-dark-state, #3a416f));
}

/* Solid backgrounds */
.bg-primary {
  background-color: var(--md-primary, #e91e63);
}

.bg-secondary {
  background-color: var(--md-secondary, #7b809a);
}

.bg-info {
  background-color: var(--md-info, #1a73e8);
}

.bg-success {
  background-color: var(--md-success, #4caf50);
}

.bg-warning {
  background-color: var(--md-warning, #fb8c00);
}

.bg-error {
  background-color: var(--md-error, #f44335);
}

.bg-light {
  background-color: var(--md-light, #f0f2f5);
}

.bg-dark {
  background-color: var(--md-dark, #344767);
}
</style>
