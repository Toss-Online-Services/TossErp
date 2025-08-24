<template>
  <component
    :is="tag"
    :type="tag === 'button' ? type : undefined"
    :href="tag === 'a' ? href : undefined"
    :to="tag === 'NuxtLink' ? to : undefined"
    :disabled="disabled || loading"
    class="btn inline-flex items-center justify-center rounded-lg font-medium transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2"
    :class="buttonClasses"
    @click="handleClick"
  >
    <!-- Loading Spinner -->
    <svg
      v-if="loading"
      class="animate-spin -ml-1 mr-2 h-4 w-4"
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      viewBox="0 0 24 24"
    >
      <circle
        class="opacity-25"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        stroke-width="4"
      />
      <path
        class="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      />
    </svg>

    <!-- Left Icon -->
    <component
      v-if="leftIcon && !loading"
      :is="leftIcon"
      class="mr-2 h-4 w-4"
    />

    <!-- Content -->
    <span>
      <slot />
    </span>

    <!-- Right Icon -->
    <component
      v-if="rightIcon && !loading"
      :is="rightIcon"
      class="ml-2 h-4 w-4"
    />
  </component>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  variant?: 'primary' | 'secondary' | 'success' | 'warning' | 'error' | 'outline' | 'ghost' | 'gradient'
  color?: 'blue' | 'green' | 'red' | 'yellow' | 'purple' | 'pink' | 'indigo' | 'gray'
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl'
  fullWidth?: boolean
  disabled?: boolean
  loading?: boolean
  rounded?: boolean
  leftIcon?: any
  rightIcon?: any
  tag?: 'button' | 'a' | 'NuxtLink'
  type?: 'button' | 'submit' | 'reset'
  href?: string
  to?: string | object
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  color: 'blue',
  size: 'md',
  fullWidth: false,
  disabled: false,
  loading: false,
  rounded: false,
  tag: 'button',
  type: 'button'
})

const emit = defineEmits<{
  click: [event: Event]
}>()

const buttonClasses = computed(() => {
  const classes = []

  // Base classes
  classes.push('transition-all duration-200 ease-in-out')

  // Size classes
  switch (props.size) {
    case 'xs':
      classes.push('px-2.5 py-1.5 text-xs')
      break
    case 'sm':
      classes.push('px-3 py-2 text-sm')
      break
    case 'md':
      classes.push('px-4 py-2.5 text-sm')
      break
    case 'lg':
      classes.push('px-6 py-3 text-base')
      break
    case 'xl':
      classes.push('px-8 py-4 text-lg')
      break
  }

  // Full width
  if (props.fullWidth) {
    classes.push('w-full')
  }

  // Rounded
  if (props.rounded) {
    classes.push('rounded-full')
  }

  // Disabled state
  if (props.disabled || props.loading) {
    classes.push('opacity-50 cursor-not-allowed')
  }

  // Variant and color classes
  const colorClasses = getVariantClasses(props.variant, props.color)
  classes.push(colorClasses)

  return classes.join(' ')
})

function getVariantClasses(variant: string, color: string): string {
  const colorMap = {
    blue: {
      primary: 'bg-blue-600 hover:bg-blue-700 focus:ring-blue-500 text-white shadow-lg hover:shadow-xl',
      secondary: 'bg-blue-100 hover:bg-blue-200 focus:ring-blue-500 text-blue-900',
      outline: 'border-2 border-blue-600 text-blue-600 hover:bg-blue-600 hover:text-white focus:ring-blue-500',
      ghost: 'text-blue-600 hover:bg-blue-100 focus:ring-blue-500',
      gradient: 'bg-gradient-to-r from-blue-600 to-blue-700 hover:from-blue-700 hover:to-blue-800 focus:ring-blue-500 text-white shadow-lg hover:shadow-xl'
    },
    green: {
      primary: 'bg-green-600 hover:bg-green-700 focus:ring-green-500 text-white shadow-lg hover:shadow-xl',
      secondary: 'bg-green-100 hover:bg-green-200 focus:ring-green-500 text-green-900',
      outline: 'border-2 border-green-600 text-green-600 hover:bg-green-600 hover:text-white focus:ring-green-500',
      ghost: 'text-green-600 hover:bg-green-100 focus:ring-green-500',
      gradient: 'bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800 focus:ring-green-500 text-white shadow-lg hover:shadow-xl'
    },
    red: {
      primary: 'bg-red-600 hover:bg-red-700 focus:ring-red-500 text-white shadow-lg hover:shadow-xl',
      secondary: 'bg-red-100 hover:bg-red-200 focus:ring-red-500 text-red-900',
      outline: 'border-2 border-red-600 text-red-600 hover:bg-red-600 hover:text-white focus:ring-red-500',
      ghost: 'text-red-600 hover:bg-red-100 focus:ring-red-500',
      gradient: 'bg-gradient-to-r from-red-600 to-red-700 hover:from-red-700 hover:to-red-800 focus:ring-red-500 text-white shadow-lg hover:shadow-xl'
    },
    yellow: {
      primary: 'bg-yellow-500 hover:bg-yellow-600 focus:ring-yellow-500 text-white shadow-lg hover:shadow-xl',
      secondary: 'bg-yellow-100 hover:bg-yellow-200 focus:ring-yellow-500 text-yellow-900',
      outline: 'border-2 border-yellow-500 text-yellow-600 hover:bg-yellow-500 hover:text-white focus:ring-yellow-500',
      ghost: 'text-yellow-600 hover:bg-yellow-100 focus:ring-yellow-500',
      gradient: 'bg-gradient-to-r from-yellow-500 to-yellow-600 hover:from-yellow-600 hover:to-yellow-700 focus:ring-yellow-500 text-white shadow-lg hover:shadow-xl'
    },
    purple: {
      primary: 'bg-purple-600 hover:bg-purple-700 focus:ring-purple-500 text-white shadow-lg hover:shadow-xl',
      secondary: 'bg-purple-100 hover:bg-purple-200 focus:ring-purple-500 text-purple-900',
      outline: 'border-2 border-purple-600 text-purple-600 hover:bg-purple-600 hover:text-white focus:ring-purple-500',
      ghost: 'text-purple-600 hover:bg-purple-100 focus:ring-purple-500',
      gradient: 'bg-gradient-to-r from-purple-600 to-purple-700 hover:from-purple-700 hover:to-purple-800 focus:ring-purple-500 text-white shadow-lg hover:shadow-xl'
    },
    gray: {
      primary: 'bg-gray-600 hover:bg-gray-700 focus:ring-gray-500 text-white shadow-lg hover:shadow-xl',
      secondary: 'bg-gray-100 hover:bg-gray-200 focus:ring-gray-500 text-gray-900',
      outline: 'border-2 border-gray-600 text-gray-600 hover:bg-gray-600 hover:text-white focus:ring-gray-500',
      ghost: 'text-gray-600 hover:bg-gray-100 focus:ring-gray-500',
      gradient: 'bg-gradient-to-r from-gray-600 to-gray-700 hover:from-gray-700 hover:to-gray-800 focus:ring-gray-500 text-white shadow-lg hover:shadow-xl'
    }
  }

  return (colorMap as any)[color]?.[variant] || colorMap.blue.primary
}

function handleClick(event: Event) {
  if (!props.disabled && !props.loading) {
    emit('click', event)
  }
}
</script>
