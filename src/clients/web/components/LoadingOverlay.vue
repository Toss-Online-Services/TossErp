<template>
  <div v-if="show" class="loading-overlay">
    <div class="loading-content">
      <LoadingSpinner :size="spinnerSize" :variant="spinnerVariant" :show-text="showText" :text="text" />
      <div v-if="subtitle" class="loading-subtitle">
        {{ subtitle }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import LoadingSpinner from './LoadingSpinner.vue'

interface Props {
  show: boolean
  text?: string
  subtitle?: string
  spinnerSize?: 'sm' | 'md' | 'lg' | 'xl'
  spinnerVariant?: 'primary' | 'secondary' | 'white'
  showText?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  text: 'Loading...',
  subtitle: '',
  spinnerSize: 'lg',
  spinnerVariant: 'white',
  showText: true
})
</script>

<style scoped>
.loading-overlay {
  @apply fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50;
}

.loading-content {
  @apply bg-white dark:bg-gray-800 rounded-lg shadow-xl p-8 text-center;
}

.loading-subtitle {
  @apply mt-4 text-sm text-gray-600 dark:text-gray-400;
}

/* Animation for overlay appearance */
.loading-overlay {
  animation: fadeIn 0.2s ease-in-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* Responsive adjustments */
@media (max-width: 640px) {
  .loading-content {
    @apply p-6 mx-4;
  }
}
</style>
