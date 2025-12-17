<script setup lang="ts">
import { ref, watch } from 'vue'
import MaterialCard from './MaterialCard.vue'
import MaterialButton from './MaterialButton.vue'

interface Props {
  modelValue: boolean
  title?: string
  maxWidth?: 'sm' | 'md' | 'lg' | 'xl' | '2xl' | 'full'
  persistent?: boolean
  showClose?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  maxWidth: 'md',
  persistent: false,
  showClose: true
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'close': []
}>()

const isOpen = ref(props.modelValue)

watch(() => props.modelValue, (val) => {
  isOpen.value = val
  if (val) {
    document.body.style.overflow = 'hidden'
  } else {
    document.body.style.overflow = ''
  }
})

const close = () => {
  if (!props.persistent) {
    isOpen.value = false
    emit('update:modelValue', false)
    emit('close')
    document.body.style.overflow = ''
  }
}

const maxWidths = {
  sm: 'max-w-sm',
  md: 'max-w-md',
  lg: 'max-w-lg',
  xl: 'max-w-xl',
  '2xl': 'max-w-2xl',
  full: 'max-w-full'
}
</script>

<template>
  <!-- Backdrop -->
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-300"
      leave-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      leave-to-class="opacity-0"
    >
      <div
        v-if="isOpen"
        class="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm"
        @click="close"
      ></div>
    </Transition>
    
    <!-- Modal -->
    <Transition
      enter-active-class="transition-all duration-300"
      leave-active-class="transition-all duration-300"
      enter-from-class="opacity-0 scale-95"
      leave-to-class="opacity-0 scale-95"
    >
      <div
        v-if="isOpen"
        class="fixed inset-0 z-50 flex items-center justify-center p-4 pointer-events-none"
      >
        <MaterialCard
          :class="['w-full pointer-events-auto animate-fade-in', maxWidths[maxWidth]]"
          variant="elevated"
          class="max-h-[90vh] overflow-hidden flex flex-col"
        >
          <!-- Header -->
          <div class="flex items-center justify-between px-6 py-4 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-xl font-bold text-slate-900 dark:text-white">
              {{ title }}
            </h3>
            <button
              v-if="showClose"
              @click="close"
              class="p-2 rounded-lg text-slate-500 hover:text-slate-700 hover:bg-slate-100 dark:hover:bg-slate-700 dark:hover:text-slate-300 transition-colors"
            >
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </button>
          </div>
          
          <!-- Content -->
          <div class="flex-1 overflow-y-auto px-6 py-4">
            <slot />
          </div>
          
          <!-- Footer -->
          <div v-if="$slots.footer" class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 bg-slate-50 dark:bg-slate-800/50">
            <slot name="footer" />
          </div>
        </MaterialCard>
      </div>
    </Transition>
  </Teleport>
</template>
