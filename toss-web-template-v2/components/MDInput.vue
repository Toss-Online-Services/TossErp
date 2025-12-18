<template>
  <div :class="inputWrapperClasses">
    <label v-if="label" :for="inputId" class="md-input-label">
      {{ label }}
    </label>
    <input
      :id="inputId"
      v-model="modelValue"
      :type="type"
      :placeholder="placeholder"
      :disabled="disabled"
      :required="required"
      :class="inputClasses"
      @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
      @blur="$emit('blur', $event)"
      @focus="$emit('focus', $event)"
    />
    <span v-if="error" class="md-input-error">{{ error }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  modelValue?: string | number
  type?: string
  label?: string
  placeholder?: string
  disabled?: boolean
  required?: boolean
  error?: string
  size?: 'small' | 'medium' | 'large'
  variant?: 'standard' | 'outlined' | 'static'
  fullWidth?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  type: 'text',
  size: 'medium',
  variant: 'outlined',
  fullWidth: false,
  modelValue: ''
})

defineEmits<{
  'update:modelValue': [value: string]
  blur: [event: FocusEvent]
  focus: [event: FocusEvent]
}>()

const inputId = computed(() => `md-input-${Math.random().toString(36).substr(2, 9)}`)

const inputWrapperClasses = computed(() => {
  const classes = ['md-input-wrapper']
  
  if (props.fullWidth) {
    classes.push('md-input-wrapper-full-width')
  }
  
  if (props.error) {
    classes.push('md-input-wrapper-error')
  }
  
  return classes.join(' ')
})

const inputClasses = computed(() => {
  const classes = ['md-input', `md-input-${props.size}`, `md-input-${props.variant}`]
  
  if (props.disabled) {
    classes.push('md-input-disabled')
  }
  
  if (props.error) {
    classes.push('md-input-has-error')
  }
  
  return classes.join(' ')
})
</script>

<style scoped>
.md-input-wrapper {
  display: inline-flex;
  flex-direction: column;
  position: relative;
  margin-bottom: 1rem;
}

.md-input-wrapper-full-width {
  width: 100%;
}

.md-input-label {
  font-size: 0.875rem;
  font-weight: 400;
  line-height: 1.66667;
  color: rgb(123, 128, 154);
  margin-bottom: 0.5rem;
}

.md-input {
  font-family: 'Roboto', sans-serif;
  font-size: 0.875rem;
  font-weight: 400;
  line-height: 1.5;
  color: rgb(123, 128, 154);
  background-color: transparent;
  border: 1px solid rgb(210, 214, 218);
  border-radius: 0.5rem;
  outline: none;
  transition: all 150ms ease-in;
}

.md-input:focus {
  border-color: var(--md-info);
  box-shadow: 0 0 0 2px rgba(26, 115, 232, 0.1);
}

.md-input-disabled {
  opacity: 0.6;
  cursor: not-allowed;
  background-color: rgb(248, 249, 250);
}

.md-input-has-error {
  border-color: var(--md-error);
}

.md-input-has-error:focus {
  border-color: var(--md-error);
  box-shadow: 0 0 0 2px rgba(244, 67, 54, 0.1);
}

.md-input-error {
  display: block;
  margin-top: 0.25rem;
  font-size: 0.75rem;
  color: var(--md-error);
}

/* Sizes */
.md-input-small {
  padding: 0.5rem 0.75rem;
  font-size: 0.75rem;
}

.md-input-medium {
  padding: 0.75rem 0.75rem;
  font-size: 0.875rem;
}

.md-input-large {
  padding: 0.875rem 0.75rem;
  font-size: 1rem;
}

/* Variants */
.md-input-standard {
  border: none;
  border-bottom: 1px solid rgb(210, 214, 218);
  border-radius: 0;
  padding-left: 0;
  padding-right: 0;
}

.md-input-static {
  border: 1px solid transparent;
  background-color: rgb(248, 249, 250);
}

.md-input-static:focus {
  border-color: transparent;
  background-color: white;
  box-shadow: var(--md-box-shadow-md);
}
</style>
