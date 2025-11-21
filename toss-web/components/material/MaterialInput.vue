<script setup lang="ts">
import { computed } from 'vue'
import { cn } from '~/lib/utils'

interface Props {
  id?: string
  type?: 'text' | 'email' | 'password' | 'number' | 'tel' | 'url' | 'search' | 'date' | 'datetime-local' | 'time'
  label?: string
  placeholder?: string
  modelValue?: string | number
  error?: string
  hint?: string
  required?: boolean
  disabled?: boolean
  readonly?: boolean
  variant?: 'outlined' | 'filled' | 'standard'
  size?: 'sm' | 'md' | 'lg'
  icon?: any
  iconPosition?: 'left' | 'right'
  clearable?: boolean
  min?: string | number
  max?: string | number
  step?: string | number
  class?: string
}

const props = withDefaults(defineProps<Props>(), {
  type: 'text',
  variant: 'outlined',
  size: 'md',
  iconPosition: 'left',
  clearable: false
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number]
  'clear': []
}>()

const inputClasses = computed(() => {
  const base = 'w-full transition-all duration-300 focus:outline-none focus:ring-4 focus:ring-opacity-50'
  
  const sizes = {
    sm: 'px-3 py-2 text-sm',
    md: 'px-4 py-3 text-base',
    lg: 'px-5 py-4 text-lg'
  }
  
  const variants = {
    outlined: 'border-2 border-slate-300 rounded-lg bg-white dark:bg-slate-800 dark:border-slate-600 focus:border-orange-500 dark:focus:border-orange-500 focus:ring-orange-500',
    filled: 'border-0 border-b-2 border-slate-300 rounded-t-lg bg-slate-100 dark:bg-slate-800 dark:border-slate-600 focus:border-orange-500 dark:focus:border-orange-500 focus:ring-orange-500',
    standard: 'border-0 border-b-2 border-slate-300 bg-transparent dark:border-slate-600 focus:border-orange-500 dark:focus:border-orange-500 px-0'
  }
  
  const errorClass = props.error ? 'border-red-500 focus:border-red-500 focus:ring-red-500' : ''
  const disabledClass = props.disabled ? 'opacity-50 cursor-not-allowed bg-slate-50 dark:bg-slate-900' : ''
  const iconPadding = props.icon ? (props.iconPosition === 'left' ? 'pl-12' : 'pr-12') : ''
  
  return cn(base, sizes[props.size], variants[props.variant], errorClass, disabledClass, iconPadding)
})

const handleInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}

const clearInput = () => {
  emit('update:modelValue', '')
  emit('clear')
}
</script>

<template>
  <div :class="cn('relative', props.class)">
    <!-- Label -->
    <label
      v-if="label"
      :for="id"
      class="block text-sm font-semibold text-slate-700 dark:text-slate-300 mb-2"
    >
      {{ label }}
      <span v-if="required" class="text-red-500 ml-1">*</span>
    </label>
    
    <!-- Input container -->
    <div class="relative">
      <!-- Left icon -->
      <div
        v-if="icon && iconPosition === 'left'"
        class="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500 dark:text-slate-400"
      >
        <component :is="icon" class="w-5 h-5" />
      </div>
      
      <!-- Input -->
      <input
        :id="id"
        :type="type"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :required="required"
        :min="min"
        :max="max"
        :step="step"
        :class="inputClasses"
        @input="handleInput"
        v-bind="$attrs"
      />
      
      <!-- Right icon or clear button -->
      <div
        v-if="(icon && iconPosition === 'right') || (clearable && modelValue)"
        class="absolute right-4 top-1/2 -translate-y-1/2 text-slate-500 dark:text-slate-400"
      >
        <button
          v-if="clearable && modelValue"
          type="button"
          @click="clearInput"
          class="hover:text-slate-700 dark:hover:text-slate-200 transition-colors"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
        <component v-else-if="icon" :is="icon" class="w-5 h-5" />
      </div>
    </div>
    
    <!-- Hint text -->
    <p
      v-if="hint && !error"
      class="mt-1.5 text-xs text-slate-500 dark:text-slate-400"
    >
      {{ hint }}
    </p>
    
    <!-- Error message -->
    <p
      v-if="error"
      class="mt-1.5 text-xs text-red-600 dark:text-red-400 flex items-center gap-1"
    >
      <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
        <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" />
      </svg>
      {{ error }}
    </p>
  </div>
</template>
