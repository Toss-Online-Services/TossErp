<template>
  <div class="progress-container">
    <!-- Label -->
    <div v-if="label || showValue" class="flex justify-between items-center mb-2">
      <span v-if="label" class="text-sm font-medium text-gray-700">
        {{ label }}
      </span>
      <span v-if="showValue" class="text-sm font-medium" :class="valueClasses">
        {{ formattedValue }}
      </span>
    </div>

    <!-- Progress Bar -->
    <div
      class="progress-track w-full bg-gray-200 rounded-full overflow-hidden"
      :class="trackClasses"
    >
      <div
        class="progress-fill h-full transition-all duration-300 ease-out rounded-full"
        :class="fillClasses"
        :style="`width: ${clampedValue}%`"
      >
        <!-- Animated Stripes (optional) -->
        <div
          v-if="animated"
          class="progress-stripes h-full w-full opacity-20"
        />
      </div>
    </div>

    <!-- Helper Text -->
    <p v-if="helperText" class="mt-1 text-sm text-gray-600">
      {{ helperText }}
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  value: number
  max?: number
  label?: string
  helperText?: string
  showValue?: boolean
  color?: 'blue' | 'green' | 'red' | 'yellow' | 'purple' | 'pink' | 'indigo' | 'gray'
  size?: 'xs' | 'sm' | 'md' | 'lg'
  variant?: 'default' | 'gradient' | 'striped'
  animated?: boolean
  precision?: number
  suffix?: string
}

const props = withDefaults(defineProps<Props>(), {
  max: 100,
  color: 'blue',
  size: 'md',
  variant: 'default',
  animated: false,
  showValue: false,
  precision: 0,
  suffix: '%'
})

const clampedValue = computed(() => {
  return Math.min(Math.max(props.value, 0), props.max)
})

const percentage = computed(() => {
  return (clampedValue.value / props.max) * 100
})

const formattedValue = computed(() => {
  if (props.suffix === '%') {
    return `${percentage.value.toFixed(props.precision)}%`
  }
  return `${clampedValue.value.toFixed(props.precision)}${props.suffix}`
})

const trackClasses = computed(() => {
  const classes = []

  // Size classes
  switch (props.size) {
    case 'xs':
      classes.push('h-1')
      break
    case 'sm':
      classes.push('h-2')
      break
    case 'md':
      classes.push('h-3')
      break
    case 'lg':
      classes.push('h-4')
      break
  }

  return classes.join(' ')
})

const fillClasses = computed(() => {
  const classes = []

  // Color classes based on variant
  if (props.variant === 'gradient') {
    switch (props.color) {
      case 'blue':
        classes.push('bg-gradient-to-r from-blue-500 to-blue-600')
        break
      case 'green':
        classes.push('bg-gradient-to-r from-green-500 to-green-600')
        break
      case 'red':
        classes.push('bg-gradient-to-r from-red-500 to-red-600')
        break
      case 'yellow':
        classes.push('bg-gradient-to-r from-yellow-500 to-yellow-600')
        break
      case 'purple':
        classes.push('bg-gradient-to-r from-purple-500 to-purple-600')
        break
      case 'pink':
        classes.push('bg-gradient-to-r from-pink-500 to-pink-600')
        break
      case 'indigo':
        classes.push('bg-gradient-to-r from-indigo-500 to-indigo-600')
        break
      case 'gray':
        classes.push('bg-gradient-to-r from-gray-500 to-gray-600')
        break
    }
  } else {
    switch (props.color) {
      case 'blue':
        classes.push('bg-blue-500')
        break
      case 'green':
        classes.push('bg-green-500')
        break
      case 'red':
        classes.push('bg-red-500')
        break
      case 'yellow':
        classes.push('bg-yellow-500')
        break
      case 'purple':
        classes.push('bg-purple-500')
        break
      case 'pink':
        classes.push('bg-pink-500')
        break
      case 'indigo':
        classes.push('bg-indigo-500')
        break
      case 'gray':
        classes.push('bg-gray-500')
        break
    }
  }

  // Striped variant
  if (props.variant === 'striped') {
    classes.push('bg-gradient-to-r from-transparent to-transparent bg-stripes')
  }

  return classes.join(' ')
})

const valueClasses = computed(() => {
  // Dynamic color based on progress percentage
  if (percentage.value >= 80) {
    return 'text-green-600'
  } else if (percentage.value >= 60) {
    return 'text-yellow-600'
  } else if (percentage.value >= 40) {
    return 'text-orange-600'
  } else {
    return 'text-red-600'
  }
})

defineOptions({
  name: 'Progress'
})
</script>

<style scoped>
.bg-stripes {
  background-image: linear-gradient(
    45deg,
    rgba(255, 255, 255, 0.15) 25%,
    transparent 25%,
    transparent 50%,
    rgba(255, 255, 255, 0.15) 50%,
    rgba(255, 255, 255, 0.15) 75%,
    transparent 75%,
    transparent
  );
  background-size: 1rem 1rem;
}

.progress-stripes {
  background-image: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.4),
    transparent
  );
  background-size: 3rem 100%;
  animation: shimmer 2s infinite;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(100%);
  }
}
</style>
