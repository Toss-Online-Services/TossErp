<template>
  <div class="form-group">
    <label
      v-if="label"
      :for="inputId"
      class="block text-sm font-medium mb-2"
      :class="labelClasses"
    >
      {{ label }}
      <span v-if="required" class="text-red-500 ml-1">*</span>
    </label>

    <div class="relative" :class="wrapperClasses">
      <!-- Left Icon -->
      <div
        v-if="leftIcon"
        class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none"
      >
        <component :is="leftIcon" class="h-5 w-5 text-gray-400" />
      </div>

      <!-- Input Field -->
      <input
        :id="inputId"
        :type="type"
        :name="name"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :required="required"
        :autocomplete="autocomplete"
        :maxlength="maxlength"
        :minlength="minlength"
        :max="max"
        :min="min"
        :step="step"
        class="input-field block w-full rounded-lg border-0 py-2.5 shadow-sm ring-1 ring-inset focus:ring-2 focus:ring-inset transition-all duration-200"
        :class="inputClasses"
        @input="handleInput"
        @blur="handleBlur"
        @focus="handleFocus"
      />

      <!-- Right Icon -->
      <div
        v-if="rightIcon"
        class="absolute inset-y-0 right-0 pr-3 flex items-center"
        :class="{ 'pointer-events-none': !rightIconClickable }"
      >
        <component
          :is="rightIcon"
          class="h-5 w-5 cursor-pointer"
          :class="rightIconClasses"
          @click="rightIconClickable && $emit('rightIconClick')"
        />
      </div>
    </div>

    <!-- Helper Text -->
    <p
      v-if="helperText && !error"
      class="mt-1 text-sm text-gray-600"
    >
      {{ helperText }}
    </p>

    <!-- Error Message -->
    <p
      v-if="error && errorMessage"
      class="mt-1 text-sm text-red-600"
    >
      {{ errorMessage }}
    </p>

    <!-- Success Message -->
    <p
      v-if="success && successMessage"
      class="mt-1 text-sm text-green-600"
    >
      {{ successMessage }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'

interface Props {
  modelValue?: string | number
  type?: 'text' | 'email' | 'password' | 'tel' | 'url' | 'search' | 'number' | 'date' | 'time' | 'datetime-local'
  name?: string
  label?: string
  placeholder?: string
  helperText?: string
  errorMessage?: string
  successMessage?: string
  size?: 'sm' | 'md' | 'lg'
  variant?: 'default' | 'filled' | 'outlined'
  disabled?: boolean
  readonly?: boolean
  required?: boolean
  error?: boolean
  success?: boolean
  leftIcon?: any
  rightIcon?: any
  rightIconClickable?: boolean
  autocomplete?: string
  maxlength?: number
  minlength?: number
  max?: number | string
  min?: number | string
  step?: number | string
}

const props = withDefaults(defineProps<Props>(), {
  type: 'text',
  size: 'md',
  variant: 'default',
  disabled: false,
  readonly: false,
  required: false,
  error: false,
  success: false,
  rightIconClickable: false
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number]
  input: [event: Event]
  blur: [event: FocusEvent]
  focus: [event: FocusEvent]
  rightIconClick: []
}>()

const inputId = ref(`input-${Math.random().toString(36).substr(2, 9)}`)

const labelClasses = computed(() => {
  const classes = []

  if (props.error) {
    classes.push('text-red-700')
  } else if (props.success) {
    classes.push('text-green-700')
  } else {
    classes.push('text-gray-700')
  }

  return classes.join(' ')
})

const wrapperClasses = computed(() => {
  const classes = []

  if (props.leftIcon) {
    classes.push('relative')
  }

  return classes.join(' ')
})

const inputClasses = computed(() => {
  const classes = []

  // Size classes
  switch (props.size) {
    case 'sm':
      classes.push('px-3 py-2 text-sm')
      break
    case 'md':
      classes.push('px-4 py-2.5 text-sm')
      break
    case 'lg':
      classes.push('px-5 py-3 text-base')
      break
  }

  // Icon padding
  if (props.leftIcon) {
    classes.push('pl-10')
  }
  if (props.rightIcon) {
    classes.push('pr-10')
  }

  // State classes
  if (props.disabled) {
    classes.push('bg-gray-50 text-gray-500 cursor-not-allowed')
  } else if (props.readonly) {
    classes.push('bg-gray-50 text-gray-700')
  } else {
    classes.push('bg-white text-gray-900')
  }

  // Variant and validation classes
  if (props.error) {
    classes.push('ring-red-500 focus:ring-red-500 border-red-500')
  } else if (props.success) {
    classes.push('ring-green-500 focus:ring-green-500 border-green-500')
  } else {
    switch (props.variant) {
      case 'filled':
        classes.push('bg-gray-50 ring-gray-300 focus:ring-blue-500')
        break
      case 'outlined':
        classes.push('ring-gray-300 focus:ring-blue-500 border border-gray-300 focus:border-blue-500')
        break
      case 'default':
      default:
        classes.push('ring-gray-300 focus:ring-blue-500')
        break
    }
  }

  // Hover effects
  if (!props.disabled && !props.readonly) {
    classes.push('hover:ring-gray-400')
  }

  return classes.join(' ')
})

const rightIconClasses = computed(() => {
  const classes = []

  if (props.error) {
    classes.push('text-red-500')
  } else if (props.success) {
    classes.push('text-green-500')
  } else {
    classes.push('text-gray-400')
  }

  if (props.rightIconClickable) {
    classes.push('hover:text-gray-600 cursor-pointer')
  }

  return classes.join(' ')
})

function handleInput(event: Event) {
  const target = event.target as HTMLInputElement
  const value = props.type === 'number' ? target.valueAsNumber : target.value
  emit('update:modelValue', value)
  emit('input', event)
}

function handleBlur(event: FocusEvent) {
  emit('blur', event)
}

function handleFocus(event: FocusEvent) {
  emit('focus', event)
}
</script>
