<template>
  <div class="toast-container">
    <TransitionGroup name="toast-list">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        :class="[
          'fixed z-50 flex items-start gap-3 px-6 py-4 rounded-xl shadow-2xl border-2 backdrop-blur-sm',
          'max-w-md w-full mx-4',
          getTypeClasses(toast.type),
          getPositionClasses(toast.position)
        ]"
      >
        <!-- Icon -->
        <div class="flex-shrink-0">
          <component :is="getIconComponent(toast.type)" :class="['w-6 h-6', getIconColorClass(toast.type)]" />
        </div>

        <!-- Content -->
        <div class="flex-1 min-w-0">
          <p v-if="toast.title" class="font-bold text-base mb-1" :class="getTitleColorClass(toast.type)">
            {{ toast.title }}
          </p>
          <p class="text-sm" :class="getMessageColorClass(toast.type)">{{ toast.message }}</p>
        </div>

        <!-- Close Button -->
        <button
          @click="removeToast(toast.id)"
          class="flex-shrink-0 p-1 hover:bg-white/20 rounded-lg transition-colors"
        >
          <XMarkIcon class="w-5 h-5" :class="getIconColorClass(toast.type)" />
        </button>
      </div>
    </TransitionGroup>
  </div>
</template>

<script setup lang="ts">
import {
  CheckCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  XCircleIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import { useToast } from '~/composables/useToast'

const { toasts, remove } = useToast()

const getTypeClasses = (type?: string) => {
  const classes: Record<string, string> = {
    success: 'bg-green-50/95 dark:bg-green-900/95 border-green-500',
    error: 'bg-red-50/95 dark:bg-red-900/95 border-red-500',
    warning: 'bg-orange-50/95 dark:bg-orange-900/95 border-orange-500',
    info: 'bg-blue-50/95 dark:bg-blue-900/95 border-blue-500'
  }
  return classes[type || 'info']
}

const getIconComponent = (type?: string) => {
  const icons: Record<string, any> = {
    success: CheckCircleIcon,
    error: XCircleIcon,
    warning: ExclamationTriangleIcon,
    info: InformationCircleIcon
  }
  return icons[type || 'info']
}

const getIconColorClass = (type?: string) => {
  const colors: Record<string, string> = {
    success: 'text-green-600 dark:text-green-400',
    error: 'text-red-600 dark:text-red-400',
    warning: 'text-orange-600 dark:text-orange-400',
    info: 'text-blue-600 dark:text-blue-400'
  }
  return colors[type || 'info']
}

const getTitleColorClass = (type?: string) => {
  const colors: Record<string, string> = {
    success: 'text-green-900 dark:text-green-100',
    error: 'text-red-900 dark:text-red-100',
    warning: 'text-orange-900 dark:text-orange-100',
    info: 'text-blue-900 dark:text-blue-100'
  }
  return colors[type || 'info']
}

const getMessageColorClass = (type?: string) => {
  const colors: Record<string, string> = {
    success: 'text-green-700 dark:text-green-200',
    error: 'text-red-700 dark:text-red-200',
    warning: 'text-orange-700 dark:text-orange-200',
    info: 'text-blue-700 dark:text-blue-200'
  }
  return colors[type || 'info']
}

const getPositionClasses = (position?: string) => {
  const positions: Record<string, string> = {
    'top-right': 'top-4 right-0 sm:top-6',
    'top-center': 'top-4 left-1/2 -translate-x-1/2 sm:top-6',
    'bottom-right': 'bottom-4 right-0 sm:bottom-6',
    'bottom-center': 'bottom-4 left-1/2 -translate-x-1/2 sm:bottom-6'
  }
  return positions[position || 'top-right']
}

const removeToast = (id: string) => {
  remove(id)
}
</script>

<style scoped>
.toast-list-enter-active,
.toast-list-leave-active {
  transition: all 0.3s ease;
}

.toast-list-enter-from {
  opacity: 0;
  transform: translateX(2rem);
}

.toast-list-leave-to {
  opacity: 0;
  transform: translateX(2rem);
}

.toast-list-move {
  transition: transform 0.3s ease;
}
</style>

