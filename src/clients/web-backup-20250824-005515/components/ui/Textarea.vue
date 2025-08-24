<template>
  <div class="form-group">
    <label
      v-if="label"
      :for="textareaId"
      class="block text-sm font-medium mb-2"
      :class="labelClasses"
    >
      {{ label }}
      <span v-if="required" class="text-red-500 ml-1">*</span>
    </label>

    <div class="relative">
      <!-- Textarea Field -->
      <textarea
        :id="textareaId"
        :name="name"
        :value="modelValue"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :required="required"
        :rows="rows"
        :cols="cols"
        :maxlength="maxlength"
        :minlength="minlength"
        class="textarea-field block w-full rounded-lg border-0 py-2.5 shadow-sm ring-1 ring-inset focus:ring-2 focus:ring-inset transition-all duration-200 resize-none"
        :class="textareaClasses"
        @input="handleInput"
        @blur="handleBlur"
        @focus="handleFocus"
      />

      <!-- Character Counter -->
      <div
        v-if="maxlength && showCharacterCount"
        class="absolute bottom-2 right-2 text-xs"
        :class="counterClasses"
      >
        {{ characterCount }}/{{ maxlength }}
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
  modelValue?: string
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
  rows?: number
  cols?: number
  maxlength?: number
  minlength?: number
  showCharacterCount?: boolean
  resize?: 'none' | 'both' | 'horizontal' | 'vertical'
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  size: 'md',
  variant: 'default',
  disabled: false,
  readonly: false,
  required: false,
  error: false,
  success: false,
  rows: 4,
  showCharacterCount: false,
  resize: 'vertical'
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
  input: [event: Event]
  blur: [event: FocusEvent]
  focus: [event: FocusEvent]
}>()

const textareaId = ref(`textarea-${Math.random().toString(36).substr(2, 9)}`)

const characterCount = computed(() => {
  return props.modelValue?.length || 0
})

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

const textareaClasses = computed(() => {
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

  // State classes
  if (props.disabled) {
    classes.push('bg-gray-50 text-gray-500 cursor-not-allowed')
  } else if (props.readonly) {
    classes.push('bg-gray-50 text-gray-700')
  } else {
    classes.push('bg-white text-gray-900')
  }

  // Resize classes
  switch (props.resize) {
    case 'none':
      classes.push('resize-none')
      break
    case 'both':
      classes.push('resize')
      break
    case 'horizontal':
      classes.push('resize-x')
      break
    case 'vertical':
      classes.push('resize-y')
      break
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

  // Character count padding
  if (props.maxlength && props.showCharacterCount) {
    classes.push('pb-8')
  }

  return classes.join(' ')
})

const counterClasses = computed(() => {
  const classes = ['text-gray-500']
  
  if (props.maxlength && characterCount.value > props.maxlength * 0.9) {
    classes.push('text-yellow-600')
  }
  
  if (props.maxlength && characterCount.value >= props.maxlength) {
    classes.push('text-red-600')
  }
  
  return classes.join(' ')
})

function handleInput(event: Event) {
  const target = event.target as HTMLTextAreaElement
  emit('update:modelValue', target.value)
  emit('input', event)
}

function handleBlur(event: FocusEvent) {
  emit('blur', event)
}

function handleFocus(event: FocusEvent) {
  emit('focus', event)
}

defineOptions({
  name: 'Textarea'
})
</script>
