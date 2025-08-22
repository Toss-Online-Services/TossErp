<template>
  <div
    class="card bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden transition-all duration-200"
    :class="cardClasses"
  >
    <!-- Header -->
    <div
      v-if="$slots.header || title || subtitle"
      class="card-header"
      :class="headerClasses"
    >
      <slot name="header">
        <div v-if="title || subtitle" class="flex items-start justify-between">
          <div class="flex-1 min-w-0">
            <h3 v-if="title" class="text-lg font-semibold text-gray-900 mb-1">
              {{ title }}
            </h3>
            <p v-if="subtitle" class="text-sm text-gray-600">
              {{ subtitle }}
            </p>
          </div>
          <slot name="headerActions" />
        </div>
      </slot>
    </div>

    <!-- Body -->
    <div
      v-if="$slots.default"
      class="card-body"
      :class="bodyClasses"
    >
      <slot />
    </div>

    <!-- Footer -->
    <div
      v-if="$slots.footer"
      class="card-footer border-t border-gray-200 bg-gray-50"
      :class="footerClasses"
    >
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  title?: string
  subtitle?: string
  variant?: 'default' | 'elevated' | 'outlined' | 'gradient'
  color?: 'white' | 'blue' | 'green' | 'red' | 'yellow' | 'purple' | 'gray'
  size?: 'sm' | 'md' | 'lg'
  hover?: boolean
  clickable?: boolean
  padding?: 'none' | 'sm' | 'md' | 'lg'
  headerPadding?: 'none' | 'sm' | 'md' | 'lg'
  bodyPadding?: 'none' | 'sm' | 'md' | 'lg'
  footerPadding?: 'none' | 'sm' | 'md' | 'lg'
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  color: 'white',
  size: 'md',
  hover: false,
  clickable: false,
  padding: 'md',
  headerPadding: 'md',
  bodyPadding: 'md',
  footerPadding: 'md'
})

const emit = defineEmits<{
  click: [event: Event]
}>()

defineOptions({
  name: 'Card'
})

const cardClasses = computed(() => {
  const classes = []

  // Variant classes
  switch (props.variant) {
    case 'elevated':
      classes.push('shadow-lg')
      break
    case 'outlined':
      classes.push('border-2 shadow-none')
      break
    case 'gradient':
      classes.push('bg-gradient-to-br shadow-lg')
      break
    case 'default':
    default:
      classes.push('shadow-sm')
      break
  }

  // Color classes
  if (props.variant === 'gradient') {
    switch (props.color) {
      case 'blue':
        classes.push('from-blue-50 to-blue-100 border-blue-200')
        break
      case 'green':
        classes.push('from-green-50 to-green-100 border-green-200')
        break
      case 'red':
        classes.push('from-red-50 to-red-100 border-red-200')
        break
      case 'yellow':
        classes.push('from-yellow-50 to-yellow-100 border-yellow-200')
        break
      case 'purple':
        classes.push('from-purple-50 to-purple-100 border-purple-200')
        break
      case 'gray':
        classes.push('from-gray-50 to-gray-100 border-gray-200')
        break
      default:
        classes.push('from-gray-50 to-white border-gray-200')
        break
    }
  } else if (props.color !== 'white') {
    switch (props.color) {
      case 'blue':
        classes.push('bg-blue-50 border-blue-200')
        break
      case 'green':
        classes.push('bg-green-50 border-green-200')
        break
      case 'red':
        classes.push('bg-red-50 border-red-200')
        break
      case 'yellow':
        classes.push('bg-yellow-50 border-yellow-200')
        break
      case 'purple':
        classes.push('bg-purple-50 border-purple-200')
        break
      case 'gray':
        classes.push('bg-gray-50 border-gray-200')
        break
    }
  }

  // Hover effects
  if (props.hover) {
    classes.push('hover:shadow-md')
  }

  // Clickable
  if (props.clickable) {
    classes.push('cursor-pointer hover:shadow-lg transform hover:-translate-y-0.5')
  }

  return classes.join(' ')
})

const headerClasses = computed(() => {
  const classes = []

  // Padding classes
  switch (props.headerPadding) {
    case 'none':
      break
    case 'sm':
      classes.push('p-4')
      break
    case 'md':
      classes.push('p-6')
      break
    case 'lg':
      classes.push('p-8')
      break
  }

  return classes.join(' ')
})

const bodyClasses = computed(() => {
  const classes = []

  // Padding classes
  switch (props.bodyPadding) {
    case 'none':
      break
    case 'sm':
      classes.push('p-4')
      break
    case 'md':
      classes.push('p-6')
      break
    case 'lg':
      classes.push('p-8')
      break
  }

  return classes.join(' ')
})

const footerClasses = computed(() => {
  const classes = []

  // Padding classes
  switch (props.footerPadding) {
    case 'none':
      break
    case 'sm':
      classes.push('p-4')
      break
    case 'md':
      classes.push('p-6')
      break
    case 'lg':
      classes.push('p-8')
      break
  }

  return classes.join(' ')
})

function handleClick(event: Event) {
  if (props.clickable) {
    emit('click', event)
  }
}
</script>
