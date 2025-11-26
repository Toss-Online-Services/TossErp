<template>
  <div class="toast-container pointer-events-none">
    <TransitionGroup name="toast-list">
      <div
        v-for="toast in toasts"
        :key="toast.id"
        class="pointer-events-auto fixed z-50 flex w-full max-w-md items-start gap-3 rounded-xl border-2 bg-white/90 px-6 py-4 text-sm shadow-2xl backdrop-blur-sm dark:bg-slate-900/90"
        :class="[getTypeClasses(toast.type), getPositionClasses(toast.position)]"
      >
        <div class="flex-shrink-0">
          <component :is="getIconComponent(toast.type)" class="h-6 w-6" :class="getIconColorClass(toast.type)" />
        </div>

        <div class="flex-1 min-w-0">
          <p
            v-if="toast.title"
            class="mb-1 text-base font-semibold"
            :class="getTitleColorClass(toast.type)"
          >
            {{ toast.title }}
          </p>
          <p class="text-sm leading-relaxed" :class="getMessageColorClass(toast.type)">
            {{ toast.message }}
          </p>
        </div>

        <button
          type="button"
          class="rounded-lg p-1 transition-colors hover:bg-black/5 dark:hover:bg-white/10"
          @click="removeToast(toast.id)"
        >
          <XMarkIcon class="h-5 w-5" :class="getIconColorClass(toast.type)" />
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

const TYPE_CLASSES: Record<string, string> = {
  success: 'border-green-500/70',
  error: 'border-red-500/70',
  warning: 'border-orange-500/70',
  info: 'border-blue-500/70'
}

const ICON_COMPONENTS: Record<string, any> = {
  success: CheckCircleIcon,
  error: XCircleIcon,
  warning: ExclamationTriangleIcon,
  info: InformationCircleIcon
}

const ICON_COLORS: Record<string, string> = {
  success: 'text-green-600 dark:text-green-400',
  error: 'text-red-600 dark:text-red-400',
  warning: 'text-orange-600 dark:text-orange-400',
  info: 'text-blue-600 dark:text-blue-400'
}

const TITLE_COLORS: Record<string, string> = {
  success: 'text-green-900 dark:text-green-100',
  error: 'text-red-900 dark:text-red-100',
  warning: 'text-orange-900 dark:text-orange-100',
  info: 'text-blue-900 dark:text-blue-100'
}

const MESSAGE_COLORS: Record<string, string> = {
  success: 'text-green-700 dark:text-green-200',
  error: 'text-red-700 dark:text-red-200',
  warning: 'text-orange-700 dark:text-orange-200',
  info: 'text-blue-700 dark:text-blue-200'
}

const POSITION_CLASSES: Record<string, string> = {
  'top-right': 'right-4 top-4 sm:right-6 sm:top-6',
  'top-center': 'left-1/2 top-4 -translate-x-1/2 sm:top-6',
  'bottom-right': 'bottom-4 right-4 sm:bottom-6 sm:right-6',
  'bottom-center': 'bottom-4 left-1/2 -translate-x-1/2 sm:bottom-6'
}

const getTypeClasses = (type?: string) => TYPE_CLASSES[type || 'info']
const getIconComponent = (type?: string) => ICON_COMPONENTS[type || 'info']
const getIconColorClass = (type?: string) => ICON_COLORS[type || 'info']
const getTitleColorClass = (type?: string) => TITLE_COLORS[type || 'info']
const getMessageColorClass = (type?: string) => MESSAGE_COLORS[type || 'info']
const getPositionClasses = (position?: string) => POSITION_CLASSES[position || 'top-right']

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






