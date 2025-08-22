<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="isOpen" class="fixed inset-0 z-50 overflow-y-auto">
        <!-- Backdrop -->
        <div 
          class="fixed inset-0 bg-black bg-opacity-50 transition-opacity"
          @click="handleBackdropClick"
        />
        
        <!-- Modal Container -->
        <div class="flex min-h-full items-center justify-center p-4">
          <div 
            class="relative w-full max-w-md transform overflow-hidden rounded-lg bg-white text-left shadow-xl transition-all sm:max-w-lg sm:w-full"
            :class="size === 'large' ? 'sm:max-w-2xl' : size === 'small' ? 'sm:max-w-sm' : 'sm:max-w-lg'"
            @click.stop
          >
            <!-- Modal Header -->
            <div v-if="title || $slots.header" class="bg-gray-50 px-6 py-4 border-b border-gray-200">
              <slot name="header">
                <div class="flex items-center justify-between">
                  <h3 class="text-lg font-medium text-gray-900">{{ title }}</h3>
                  <button
                    @click="close"
                    class="rounded-md bg-white text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-orange-500 focus:ring-offset-2"
                  >
                    <span class="sr-only">Close</span>
                    <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              </slot>
            </div>

            <!-- Modal Body -->
            <div class="px-6 py-4">
              <slot />
            </div>

            <!-- Modal Footer -->
            <div v-if="$slots.footer" class="bg-gray-50 px-6 py-4 border-t border-gray-200 flex justify-end space-x-3">
              <slot name="footer" />
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'

interface Props {
  isOpen: boolean
  title?: string
  size?: 'small' | 'medium' | 'large'
  closeOnBackdrop?: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'update:isOpen', value: boolean): void
}

const props = withDefaults(defineProps<Props>(), {
  size: 'medium',
  closeOnBackdrop: true
})

const emit = defineEmits<Emits>()

const close = () => {
  emit('close')
  emit('update:isOpen', false)
}

const handleBackdropClick = () => {
  if (props.closeOnBackdrop) {
    close()
  }
}

// Close on Escape key
onMounted(() => {
  const handleEscape = (e: KeyboardEvent) => {
    if (e.key === 'Escape' && props.isOpen) {
      close()
    }
  }
  
  document.addEventListener('keydown', handleEscape)
  
  onUnmounted(() => {
    document.removeEventListener('keydown', handleEscape)
  })
})
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .bg-white,
.modal-leave-active .bg-white {
  transition: transform 0.3s ease;
}

.modal-enter-from .bg-white,
.modal-leave-to .bg-white {
  transform: scale(0.9);
}
</style>
