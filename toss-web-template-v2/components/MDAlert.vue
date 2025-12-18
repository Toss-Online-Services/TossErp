<template>
  <div 
    v-if="modelValue"
    class="alert" 
    :class="alertClasses"
    role="alert"
  >
    <div class="d-flex align-items-center">
      <Icon 
        v-if="icon" 
        :name="icon" 
        :size="iconSize"
        class="me-2"
      />
      <div class="flex-grow-1">
        <MDTypography 
          v-if="title" 
          variant="button" 
          font-weight="bold"
          :color="dismissible ? 'white' : color"
          class="mb-0"
        >
          {{ title }}
        </MDTypography>
        <MDTypography 
          variant="caption" 
          :color="dismissible ? 'white' : 'text'"
          class="mb-0"
        >
          <slot>{{ content }}</slot>
        </MDTypography>
      </div>
      <button 
        v-if="dismissible"
        type="button" 
        class="btn-close"
        @click="close"
        aria-label="Close"
      >
        <Icon name="mdi:close" size="18" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  modelValue?: boolean
  color?: 'primary' | 'secondary' | 'info' | 'success' | 'warning' | 'error' | 'light' | 'dark'
  variant?: 'filled' | 'gradient' | 'outlined'
  title?: string
  content?: string
  icon?: string
  iconSize?: string | number
  dismissible?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: true,
  color: 'info',
  variant: 'gradient',
  title: '',
  content: '',
  icon: '',
  iconSize: '24',
  dismissible: false
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const alertClasses = computed(() => {
  const classes: string[] = []
  
  if (props.variant === 'gradient') {
    classes.push(`alert-${props.color}`)
    classes.push('alert-dismissible')
    classes.push('fade')
    classes.push('show')
  } else if (props.variant === 'filled') {
    classes.push(`alert-${props.color}`)
    classes.push('alert-filled')
  } else if (props.variant === 'outlined') {
    classes.push(`alert-${props.color}`)
    classes.push('alert-outlined')
  }
  
  return classes
})

const close = () => {
  emit('update:modelValue', false)
}
</script>

<style scoped>
.alert {
  position: relative;
  padding: 0.75rem 1.25rem;
  margin-bottom: 1rem;
  border: 1px solid transparent;
  border-radius: 0.5rem;
}

.btn-close {
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.btn-close:hover {
  opacity: 1;
}

.flex-grow-1 {
  flex-grow: 1;
}

/* Gradient variants */
.alert-primary.alert-dismissible {
  background: linear-gradient(310deg, var(--md-gradient-primary-main, #7928ca), var(--md-gradient-primary-state, #ff0080));
  color: white;
  border: none;
}

.alert-secondary.alert-dismissible {
  background: linear-gradient(310deg, var(--md-gradient-secondary-main, #627594), var(--md-gradient-secondary-state, #a8b8d8));
  color: white;
  border: none;
}

.alert-info.alert-dismissible {
  background: linear-gradient(310deg, var(--md-gradient-info-main, #2152ff), var(--md-gradient-info-state, #21d4fd));
  color: white;
  border: none;
}

.alert-success.alert-dismissible {
  background: linear-gradient(310deg, var(--md-gradient-success-main, #17ad37), var(--md-gradient-success-state, #98ec2d));
  color: white;
  border: none;
}

.alert-warning.alert-dismissible {
  background: linear-gradient(310deg, var(--md-gradient-warning-main, #f53939), var(--md-gradient-warning-state, #fbcf33));
  color: white;
  border: none;
}

.alert-error.alert-dismissible {
  background: linear-gradient(310deg, var(--md-gradient-error-main, #ea0606), var(--md-gradient-error-state, #ff667c));
  color: white;
  border: none;
}

.alert-dark.alert-dismissible {
  background: linear-gradient(310deg, var(--md-gradient-dark-main, #141727), var(--md-gradient-dark-state, #3a416f));
  color: white;
  border: none;
}

/* Filled variants */
.alert-filled {
  color: white;
}

.alert-primary.alert-filled {
  background-color: var(--md-primary, #e91e63);
}

.alert-info.alert-filled {
  background-color: var(--md-info, #1a73e8);
}

.alert-success.alert-filled {
  background-color: var(--md-success, #4caf50);
}

.alert-warning.alert-filled {
  background-color: var(--md-warning, #fb8c00);
}

.alert-error.alert-filled {
  background-color: var(--md-error, #f44335);
}

/* Outlined variants */
.alert-outlined {
  background-color: transparent;
  border-width: 2px;
}

.alert-primary.alert-outlined {
  border-color: var(--md-primary, #e91e63);
  color: var(--md-primary, #e91e63);
}

.alert-info.alert-outlined {
  border-color: var(--md-info, #1a73e8);
  color: var(--md-info, #1a73e8);
}

.alert-success.alert-outlined {
  border-color: var(--md-success, #4caf50);
  color: var(--md-success, #4caf50);
}

.alert-warning.alert-outlined {
  border-color: var(--md-warning, #fb8c00);
  color: var(--md-warning, #fb8c00);
}

.alert-error.alert-outlined {
  border-color: var(--md-error, #f44335);
  color: var(--md-error, #f44335);
}

.fade {
  transition: opacity 0.15s linear;
}

.fade.show {
  opacity: 1;
}
</style>
