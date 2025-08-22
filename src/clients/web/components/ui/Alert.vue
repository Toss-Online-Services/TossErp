<template>
  <Transition
    enter-active-class="transition ease-out duration-300"
    enter-from-class="opacity-0 transform scale-95"
    enter-to-class="opacity-100 transform scale-100"
    leave-active-class="transition ease-in duration-200"
    leave-from-class="opacity-100 transform scale-100"
    leave-to-class="opacity-0 transform scale-95"
  >
    <div
      v-if="visible"
      class="alert relative rounded-lg p-4 mb-4 border-l-4 shadow-sm"
      :class="alertClasses"
      role="alert"
    >
      <div class="flex items-start">
        <!-- Icon -->
        <div v-if="icon || variant" class="flex-shrink-0 mr-3">
          <component
            :is="iconComponent"
            class="h-5 w-5"
            :class="iconClasses"
          />
        </div>
        
        <!-- Content -->
        <div class="flex-1 min-w-0">
          <div v-if="title" class="font-medium text-sm mb-1" :class="titleClasses">
            {{ title }}
          </div>
          <div class="text-sm" :class="contentClasses">
            <slot />
          </div>
        </div>
        
        <!-- Dismiss Button -->
        <div v-if="dismissible" class="flex-shrink-0 ml-3">
          <button
            type="button"
            class="inline-flex rounded-md p-1.5 focus:outline-none focus:ring-2 focus:ring-offset-2 transition-colors"
            :class="dismissClasses"
            @click="dismiss"
          >
            <span class="sr-only">Dismiss</span>
            <XMarkIcon class="h-4 w-4" />
          </button>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { 
  CheckCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  XCircleIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

interface Props {
  variant?: 'success' | 'warning' | 'error' | 'info'
  title?: string
  icon?: any
  dismissible?: boolean
  modelValue?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'info',
  dismissible: false,
  modelValue: true
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  dismiss: []
}>()

const visible = ref(props.modelValue)

watch(() => props.modelValue, (newValue) => {
  visible.value = newValue
})

const iconComponent = computed(() => {
  if (props.icon) return props.icon
  
  switch (props.variant) {
    case 'success':
      return CheckCircleIcon
    case 'warning':
      return ExclamationTriangleIcon
    case 'error':
      return XCircleIcon
    case 'info':
    default:
      return InformationCircleIcon
  }
})

const alertClasses = computed(() => {
  const baseClasses = 'bg-white border-l-4'
  
  switch (props.variant) {
    case 'success':
      return `${baseClasses} border-green-500 bg-green-50`
    case 'warning':
      return `${baseClasses} border-yellow-500 bg-yellow-50`
    case 'error':
      return `${baseClasses} border-red-500 bg-red-50`
    case 'info':
    default:
      return `${baseClasses} border-blue-500 bg-blue-50`
  }
})

const iconClasses = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'text-green-600'
    case 'warning':
      return 'text-yellow-600'
    case 'error':
      return 'text-red-600'
    case 'info':
    default:
      return 'text-blue-600'
  }
})

const titleClasses = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'text-green-800'
    case 'warning':
      return 'text-yellow-800'
    case 'error':
      return 'text-red-800'
    case 'info':
    default:
      return 'text-blue-800'
  }
})

const contentClasses = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'text-green-700'
    case 'warning':
      return 'text-yellow-700'
    case 'error':
      return 'text-red-700'
    case 'info':
    default:
      return 'text-blue-700'
  }
})

const dismissClasses = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'text-green-500 hover:bg-green-100 focus:ring-green-600'
    case 'warning':
      return 'text-yellow-500 hover:bg-yellow-100 focus:ring-yellow-600'
    case 'error':
      return 'text-red-500 hover:bg-red-100 focus:ring-red-600'
    case 'info':
    default:
      return 'text-blue-500 hover:bg-blue-100 focus:ring-blue-600'
  }
})

function dismiss() {
  visible.value = false
  emit('update:modelValue', false)
  emit('dismiss')
}
</script>
