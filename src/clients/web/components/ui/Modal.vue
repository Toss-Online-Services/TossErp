<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition ease-out duration-300"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition ease-in duration-200"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="modelValue"
        class="modal-overlay fixed inset-0 z-50 overflow-y-auto"
        @click="handleOverlayClick"
      >
        <!-- Backdrop -->
        <div class="fixed inset-0 bg-black bg-opacity-50 transition-opacity" />

        <!-- Modal Container -->
        <div class="flex min-h-full items-center justify-center p-4">
          <Transition
            enter-active-class="transition ease-out duration-300"
            enter-from-class="opacity-0 transform scale-95"
            enter-to-class="opacity-100 transform scale-100"
            leave-active-class="transition ease-in duration-200"
            leave-from-class="opacity-100 transform scale-100"
            leave-to-class="opacity-0 transform scale-95"
          >
            <div
              v-if="modelValue"
              class="modal-content relative transform overflow-hidden rounded-lg bg-white shadow-xl transition-all"
              :class="sizeClasses"
              @click.stop
            >
              <!-- Header -->
              <div
                v-if="$slots.header || title || closable"
                class="modal-header flex items-center justify-between border-b border-gray-200 px-6 py-4"
              >
                <div class="flex-1">
                  <slot name="header">
                    <h3 v-if="title" class="text-lg font-semibold text-gray-900">
                      {{ title }}
                    </h3>
                  </slot>
                </div>
                <button
                  v-if="closable"
                  type="button"
                  class="ml-4 inline-flex items-center justify-center rounded-md p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500"
                  @click="close"
                >
                  <span class="sr-only">Close</span>
                  <XMarkIcon class="h-5 w-5" />
                </button>
              </div>

              <!-- Body -->
              <div class="modal-body" :class="bodyClasses">
                <slot />
              </div>

              <!-- Footer -->
              <div
                v-if="$slots.footer"
                class="modal-footer border-t border-gray-200 bg-gray-50 px-6 py-4"
              >
                <slot name="footer" />
              </div>
            </div>
          </Transition>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'

interface Props {
  modelValue: boolean
  title?: string
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl' | 'full'
  closable?: boolean
  closeOnClickOutside?: boolean
  closeOnEscape?: boolean
  bodyPadding?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  closable: true,
  closeOnClickOutside: true,
  closeOnEscape: true,
  bodyPadding: true
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  close: []
  open: []
}>()

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'xs':
      return 'max-w-xs w-full'
    case 'sm':
      return 'max-w-sm w-full'
    case 'md':
      return 'max-w-md w-full'
    case 'lg':
      return 'max-w-lg w-full'
    case 'xl':
      return 'max-w-xl w-full'
    case '2xl':
      return 'max-w-2xl w-full'
    case 'full':
      return 'w-full h-full'
    default:
      return 'max-w-md w-full'
  }
})

const bodyClasses = computed(() => {
  const classes = []
  
  if (props.bodyPadding) {
    classes.push('px-6 py-4')
  }
  
  return classes.join(' ')
})

function close() {
  emit('update:modelValue', false)
  emit('close')
}

function open() {
  emit('update:modelValue', true)
  emit('open')
}

function handleOverlayClick() {
  if (props.closeOnClickOutside) {
    close()
  }
}

function handleEscapeKey(event: KeyboardEvent) {
  if (event.key === 'Escape' && props.closeOnEscape && props.modelValue) {
    close()
  }
}

onMounted(() => {
  if (props.closeOnEscape) {
    document.addEventListener('keydown', handleEscapeKey)
  }
})

onUnmounted(() => {
  if (props.closeOnEscape) {
    document.removeEventListener('keydown', handleEscapeKey)
  }
})

defineOptions({
  name: 'Modal'
})
</script>
