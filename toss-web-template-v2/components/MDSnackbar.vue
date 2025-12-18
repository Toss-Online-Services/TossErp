<template>
  <Teleport to="body">
    <Transition name="snackbar">
      <div 
        v-if="isVisible"
        class="md-snackbar"
        :class="[
          `md-snackbar-${color}`,
          `md-snackbar-${position}`,
          { 'md-snackbar-icon': icon }
        ]"
        role="alert"
      >
        <div class="md-snackbar-content">
          <Icon v-if="icon" :name="icon" size="20" class="md-snackbar-icon-element" />
          <div class="md-snackbar-message">
            <MDTypography v-if="title" variant="button" font-weight="medium" class="text-white mb-0">
              {{ title }}
            </MDTypography>
            <MDTypography variant="caption" class="text-white opacity-8">
              {{ message }}
            </MDTypography>
          </div>
          <button 
            v-if="closeable"
            type="button"
            class="md-snackbar-close"
            aria-label="Close"
            @click="close"
          >
            <Icon name="mdi:close" size="18" />
          </button>
        </div>
        <div v-if="autoHideDuration" class="md-snackbar-progress" :style="{ animationDuration: `${autoHideDuration}ms` }" />
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'

interface Props {
  modelValue?: boolean
  color?: 'primary' | 'secondary' | 'success' | 'error' | 'warning' | 'info' | 'light' | 'dark'
  title?: string
  message: string
  icon?: string
  position?: 'top-left' | 'top-center' | 'top-right' | 'bottom-left' | 'bottom-center' | 'bottom-right'
  autoHideDuration?: number
  closeable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false,
  color: 'dark',
  position: 'bottom-right',
  autoHideDuration: 5000,
  closeable: true
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  close: []
}>()

const isVisible = ref(props.modelValue)
let timeoutId: ReturnType<typeof setTimeout> | null = null

watch(() => props.modelValue, (newValue) => {
  isVisible.value = newValue
  
  if (newValue && props.autoHideDuration) {
    clearTimeout(timeoutId!)
    timeoutId = setTimeout(() => {
      close()
    }, props.autoHideDuration)
  }
})

watch(isVisible, (newValue) => {
  if (!newValue && timeoutId) {
    clearTimeout(timeoutId)
    timeoutId = null
  }
})

const close = () => {
  isVisible.value = false
  emit('update:modelValue', false)
  emit('close')
}

onMounted(() => {
  if (props.modelValue && props.autoHideDuration) {
    timeoutId = setTimeout(() => {
      close()
    }, props.autoHideDuration)
  }
})
</script>

<style scoped>
.md-snackbar {
  position: fixed;
  z-index: 9999;
  min-width: 320px;
  max-width: 500px;
  padding: 1rem 1.25rem;
  border-radius: 0.5rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  backdrop-filter: blur(10px);
  overflow: hidden;
}

/* Positioning */
.md-snackbar-top-left {
  top: 1.5rem;
  left: 1.5rem;
}

.md-snackbar-top-center {
  top: 1.5rem;
  left: 50%;
  transform: translateX(-50%);
}

.md-snackbar-top-right {
  top: 1.5rem;
  right: 1.5rem;
}

.md-snackbar-bottom-left {
  bottom: 1.5rem;
  left: 1.5rem;
}

.md-snackbar-bottom-center {
  bottom: 1.5rem;
  left: 50%;
  transform: translateX(-50%);
}

.md-snackbar-bottom-right {
  bottom: 1.5rem;
  right: 1.5rem;
}

/* Colors */
.md-snackbar-primary {
  background: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
}

.md-snackbar-secondary {
  background: linear-gradient(195deg, #627594 0%, #a8b8d8 100%);
}

.md-snackbar-success {
  background: linear-gradient(195deg, #66BB6A 0%, #43A047 100%);
}

.md-snackbar-error {
  background: linear-gradient(195deg, #EF5350 0%, #E53935 100%);
}

.md-snackbar-warning {
  background: linear-gradient(195deg, #FFA726 0%, #FB8C00 100%);
}

.md-snackbar-info {
  background: linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%);
}

.md-snackbar-light {
  background: #ffffff;
  border: 1px solid #e9ecef;
  color: #344767;
}

.md-snackbar-dark {
  background: linear-gradient(195deg, #42424a 0%, #191919 100%);
}

/* Content */
.md-snackbar-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  position: relative;
  z-index: 2;
}

.md-snackbar-icon-element {
  flex-shrink: 0;
  color: white;
}

.md-snackbar-light .md-snackbar-icon-element {
  color: #344767;
}

.md-snackbar-message {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.md-snackbar-close {
  flex-shrink: 0;
  background: transparent;
  border: none;
  color: white;
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 0.25rem;
  transition: background-color 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.md-snackbar-light .md-snackbar-close {
  color: #344767;
}

.md-snackbar-close:hover {
  background-color: rgba(255, 255, 255, 0.1);
}

.md-snackbar-light .md-snackbar-close:hover {
  background-color: rgba(0, 0, 0, 0.05);
}

/* Progress Bar */
.md-snackbar-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  height: 3px;
  background-color: rgba(255, 255, 255, 0.5);
  animation: snackbar-progress linear forwards;
  z-index: 1;
}

.md-snackbar-light .md-snackbar-progress {
  background-color: rgba(52, 71, 103, 0.3);
}

@keyframes snackbar-progress {
  from {
    width: 100%;
  }
  to {
    width: 0%;
  }
}

/* Transitions */
.snackbar-enter-active,
.snackbar-leave-active {
  transition: all 0.3s ease;
}

.snackbar-enter-from.md-snackbar-top-left,
.snackbar-leave-to.md-snackbar-top-left,
.snackbar-enter-from.md-snackbar-bottom-left,
.snackbar-leave-to.md-snackbar-bottom-left {
  transform: translateX(-100%);
  opacity: 0;
}

.snackbar-enter-from.md-snackbar-top-right,
.snackbar-leave-to.md-snackbar-top-right,
.snackbar-enter-from.md-snackbar-bottom-right,
.snackbar-leave-to.md-snackbar-bottom-right {
  transform: translateX(100%);
  opacity: 0;
}

.snackbar-enter-from.md-snackbar-top-center,
.snackbar-leave-to.md-snackbar-top-center {
  transform: translateX(-50%) translateY(-100%);
  opacity: 0;
}

.snackbar-enter-from.md-snackbar-bottom-center,
.snackbar-leave-to.md-snackbar-bottom-center {
  transform: translateX(-50%) translateY(100%);
  opacity: 0;
}

/* Responsive */
@media (max-width: 768px) {
  .md-snackbar {
    min-width: calc(100vw - 3rem);
    max-width: calc(100vw - 3rem);
  }
}
</style>
