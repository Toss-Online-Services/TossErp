<template>
  <span
    class="badge inline-flex items-center font-medium rounded-full transition-all duration-200"
    :class="badgeClasses"
  >
    <!-- Left Icon -->
    <component
      v-if="leftIcon"
      :is="leftIcon"
      class="mr-1"
      :class="iconSizeClasses"
    />

    <!-- Content -->
    <span>
      <slot />
    </span>

    <!-- Right Icon / Close Button -->
    <component
      v-if="rightIcon"
      :is="rightIcon"
      class="ml-1"
      :class="iconSizeClasses"
    />
    
    <button
      v-if="closable"
      type="button"
      class="ml-1 inline-flex items-center justify-center rounded-full focus:outline-none focus:ring-2 focus:ring-offset-1 transition-colors"
      :class="closeButtonClasses"
      @click="$emit('close')"
    >
      <XMarkIcon :class="iconSizeClasses" />
    </button>
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/solid'

interface Props {
  variant?: 'default' | 'secondary' | 'success' | 'warning' | 'error' | 'info' | 'outline' | 'ghost'
  color?: 'blue' | 'green' | 'red' | 'yellow' | 'purple' | 'pink' | 'indigo' | 'gray'
  size?: 'xs' | 'sm' | 'md' | 'lg'
  rounded?: boolean
  closable?: boolean
  leftIcon?: any
  rightIcon?: any
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  color: 'blue',
  size: 'md',
  rounded: false,
  closable: false
})

const emit = defineEmits<{
  close: []
}>()

const badgeClasses = computed(() => {
  const classes = []

  // Size classes
  switch (props.size) {
    case 'xs':
      classes.push('px-2 py-0.5 text-xs')
      break
    case 'sm':
      classes.push('px-2.5 py-0.5 text-xs')
      break
    case 'md':
      classes.push('px-3 py-1 text-sm')
      break
    case 'lg':
      classes.push('px-4 py-1.5 text-base')
      break
  }

  // Rounded
  if (props.rounded) {
    classes.push('rounded-full')
  } else {
    classes.push('rounded-md')
  }

  // Variant and color classes
  const variantClasses = getVariantClasses(props.variant, props.color)
  classes.push(variantClasses)

  return classes.join(' ')
})

const iconSizeClasses = computed(() => {
  switch (props.size) {
    case 'xs':
      return 'h-3 w-3'
    case 'sm':
      return 'h-3 w-3'
    case 'md':
      return 'h-4 w-4'
    case 'lg':
      return 'h-5 w-5'
    default:
      return 'h-4 w-4'
  }
})

const closeButtonClasses = computed(() => {
  const classes = ['hover:bg-black hover:bg-opacity-10']

  // Focus ring color based on variant
  switch (props.variant) {
    case 'success':
      classes.push('focus:ring-green-500')
      break
    case 'warning':
      classes.push('focus:ring-yellow-500')
      break
    case 'error':
      classes.push('focus:ring-red-500')
      break
    case 'info':
      classes.push('focus:ring-blue-500')
      break
    default:
      classes.push('focus:ring-gray-500')
      break
  }

  return classes.join(' ')
})

function getVariantClasses(variant: string, color: string): string {
  const colorMap = {
    blue: {
      default: 'bg-blue-100 text-blue-800',
      secondary: 'bg-blue-50 text-blue-700 border border-blue-200',
      outline: 'bg-transparent text-blue-600 border-2 border-blue-600',
      ghost: 'bg-transparent text-blue-600 hover:bg-blue-50'
    },
    green: {
      default: 'bg-green-100 text-green-800',
      secondary: 'bg-green-50 text-green-700 border border-green-200',
      outline: 'bg-transparent text-green-600 border-2 border-green-600',
      ghost: 'bg-transparent text-green-600 hover:bg-green-50'
    },
    red: {
      default: 'bg-red-100 text-red-800',
      secondary: 'bg-red-50 text-red-700 border border-red-200',
      outline: 'bg-transparent text-red-600 border-2 border-red-600',
      ghost: 'bg-transparent text-red-600 hover:bg-red-50'
    },
    yellow: {
      default: 'bg-yellow-100 text-yellow-800',
      secondary: 'bg-yellow-50 text-yellow-700 border border-yellow-200',
      outline: 'bg-transparent text-yellow-600 border-2 border-yellow-600',
      ghost: 'bg-transparent text-yellow-600 hover:bg-yellow-50'
    },
    purple: {
      default: 'bg-purple-100 text-purple-800',
      secondary: 'bg-purple-50 text-purple-700 border border-purple-200',
      outline: 'bg-transparent text-purple-600 border-2 border-purple-600',
      ghost: 'bg-transparent text-purple-600 hover:bg-purple-50'
    },
    gray: {
      default: 'bg-gray-100 text-gray-800',
      secondary: 'bg-gray-50 text-gray-700 border border-gray-200',
      outline: 'bg-transparent text-gray-600 border-2 border-gray-600',
      ghost: 'bg-transparent text-gray-600 hover:bg-gray-50'
    }
  }

  // Handle special variants
  if (variant === 'success') {
    return colorMap.green.default
  } else if (variant === 'warning') {
    return colorMap.yellow.default
  } else if (variant === 'error') {
    return colorMap.red.default
  } else if (variant === 'info') {
    return colorMap.blue.default
  }

  return (colorMap as any)[color]?.[variant] || colorMap.blue.default
}

defineOptions({
  name: 'Badge'
})
</script>
