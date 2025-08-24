<template>
  <div class="loading-spinner" :class="[size, variant]">
    <div class="spinner-container">
      <div class="spinner"></div>
      <div v-if="showText" class="loading-text">
        {{ text }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  size?: 'sm' | 'md' | 'lg' | 'xl'
  variant?: 'primary' | 'secondary' | 'white'
  text?: string
  showText?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  variant: 'primary',
  text: 'Loading...',
  showText: false
})
</script>

<style scoped>
.loading-spinner {
  @apply flex items-center justify-center;
}

.spinner-container {
  @apply flex flex-col items-center;
}

.spinner {
  @apply border-2 border-gray-200 rounded-full animate-spin;
}

.loading-text {
  @apply mt-2 text-sm text-gray-600 dark:text-gray-400;
}

/* Size variants */
.sm .spinner {
  @apply w-4 h-4 border-t-2;
}

.md .spinner {
  @apply w-6 h-6 border-t-2;
}

.lg .spinner {
  @apply w-8 h-8 border-t-2;
}

.xl .spinner {
  @apply w-12 h-12 border-t-2;
}

/* Color variants */
.primary .spinner {
  @apply border-gray-200 border-t-primary-600;
}

.secondary .spinner {
  @apply border-gray-200 border-t-gray-600;
}

.white .spinner {
  @apply border-gray-300 border-t-white;
}

/* Animation */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

/* Responsive text sizing */
.sm .loading-text {
  @apply text-xs;
}

.md .loading-text {
  @apply text-sm;
}

.lg .loading-text {
  @apply text-base;
}

.xl .loading-text {
  @apply text-lg;
}
</style>
