<template>
  <Transition name="toast">
    <div
      v-if="visible"
      :class="[
        'fixed z-50 flex items-start gap-3 px-6 py-4 rounded-xl shadow-2xl border-2 backdrop-blur-sm',
        'max-w-md w-full',
        typeClasses,
        positionClasses
      ]"
    >
      <!-- Icon -->
      <div class="flex-shrink-0">
        <component :is="iconComponent" :class="['w-6 h-6', iconColorClass]" />
      </div>

      <!-- Content -->
      <div class="flex-1 min-w-0">
        <p v-if="title" class="font-bold text-base mb-1" :class="titleColorClass">{{ title }}</p>
        <p class="text-sm" :class="messageColorClass">{{ message }}</p>
      </div>

      <!-- Close Button -->
      <button
        v-if="dismissible"
        @click="close"
        class="flex-shrink-0 p-1 hover:bg-white/20 rounded-lg transition-colors"
      >
        <XMarkIcon class="w-5 h-5" :class="iconColorClass" />
      </button>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import {
  CheckCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  XCircleIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

interface Props {
  type?: 'success' | 'error' | 'warning' | 'info'
  title?: string
  message: string
  duration?: number
  position?: 'top-right' | 'top-center' | 'bottom-right' | 'bottom-center'
  dismissible?: boolean
  show?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  type: 'info',
  duration: 5000,
  position: 'top-right',
  dismissible: true,
  show: false
})

const emit = defineEmits<{
  close: []
}>()

const visible = ref(false)
let timeoutId: ReturnType<typeof setTimeout> | null = null

const typeClasses = computed(() => {
  const classes = {
    success: 'bg-green-50/95 dark:bg-green-900/95 border-green-500',
    error: 'bg-red-50/95 dark:bg-red-900/95 border-red-500',
    warning: 'bg-orange-50/95 dark:bg-orange-900/95 border-orange-500',
    info: 'bg-blue-50/95 dark:bg-blue-900/95 border-blue-500'
  }
  return classes[props.type]
})

const iconComponent = computed(() => {
  const icons = {
    success: CheckCircleIcon,
    error: XCircleIcon,
    warning: ExclamationTriangleIcon,
    info: InformationCircleIcon
  }
  return icons[props.type]
})

const iconColorClass = computed(() => {
  const colors = {
    success: 'text-green-600 dark:text-green-400',
    error: 'text-red-600 dark:text-red-400',
    warning: 'text-orange-600 dark:text-orange-400',
    info: 'text-blue-600 dark:text-blue-400'
  }
  return colors[props.type]
})

const titleColorClass = computed(() => {
  const colors = {
    success: 'text-green-900 dark:text-green-100',
    error: 'text-red-900 dark:text-red-100',
    warning: 'text-orange-900 dark:text-orange-100',
    info: 'text-blue-900 dark:text-blue-100'
  }
  return colors[props.type]
})

const messageColorClass = computed(() => {
  const colors = {
    success: 'text-green-700 dark:text-green-200',
    error: 'text-red-700 dark:text-red-200',
    warning: 'text-orange-700 dark:text-orange-200',
    info: 'text-blue-700 dark:text-blue-200'
  }
  return colors[props.type]
})

const positionClasses = computed(() => {
  const positions = {
    'top-right': 'top-4 right-4 sm:top-6 sm:right-6',
    'top-center': 'top-4 left-1/2 -translate-x-1/2 sm:top-6',
    'bottom-right': 'bottom-4 right-4 sm:bottom-6 sm:right-6',
    'bottom-center': 'bottom-4 left-1/2 -translate-x-1/2 sm:bottom-6'
  }
  return positions[props.position]
})

const close = () => {
  visible.value = false
  if (timeoutId) {
    clearTimeout(timeoutId)
  }
  emit('close')
}

const startTimer = () => {
  if (props.duration > 0) {
    timeoutId = setTimeout(() => {
      close()
    }, props.duration)
  }
}

watch(() => props.show, (newVal) => {
  if (newVal) {
    visible.value = true
    startTimer()
  } else {
    visible.value = false
  }
})

onMounted(() => {
  if (props.show) {
    visible.value = true
    startTimer()
  }
})
</script>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateY(-1rem) translateX(0);
}

.toast-leave-to {
  opacity: 0;
  transform: translateY(-1rem) translateX(0);
}
</style>

