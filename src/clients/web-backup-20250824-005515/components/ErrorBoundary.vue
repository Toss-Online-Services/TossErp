<template>
  <div v-if="error" class="error-boundary">
    <div class="error-container">
      <div class="error-icon">
        <ExclamationTriangleIcon class="w-12 h-12 text-red-500" />
      </div>
      
      <h2 class="error-title">Something went wrong</h2>
      
      <p class="error-message">
        {{ error.message || 'An unexpected error occurred. Please try refreshing the page.' }}
      </p>
      
      <div class="error-actions">
        <button
          @click="retry"
          class="btn-primary"
          :disabled="isRetrying"
        >
          <div v-if="isRetrying" class="w-4 h-4 mr-2 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
          {{ isRetrying ? 'Retrying...' : 'Try Again' }}
        </button>
        
        <button
          @click="reset"
          class="btn-secondary"
        >
          Reset
        </button>
        
        <button
          @click="reportError"
          class="btn-outline"
        >
          Report Issue
        </button>
      </div>
      
      <details v-if="showDetails" class="error-details">
        <summary class="error-details-summary">
          Technical Details
        </summary>
        <div class="error-details-content">
          <div class="error-info">
            <p><strong>Error Code:</strong> {{ error.code || 'Unknown' }}</p>
            <p><strong>Timestamp:</strong> {{ formatTimestamp(error.timestamp) }}</p>
            <p v-if="error.details"><strong>Details:</strong></p>
            <pre v-if="error.details" class="error-details-json">{{ JSON.stringify(error.details, null, 2) }}</pre>
          </div>
          
          <div class="error-stack" v-if="error.stack">
            <p><strong>Stack Trace:</strong></p>
            <pre class="error-stack-trace">{{ error.stack }}</pre>
          </div>
        </div>
      </details>
      
      <button
        @click="toggleDetails"
        class="error-toggle-details"
      >
        {{ showDetails ? 'Hide' : 'Show' }} Technical Details
      </button>
    </div>
  </div>
  
  <div v-else>
    <slot />
  </div>
</template>

<script setup lang="ts">
import { ref, onErrorCaptured, onMounted } from 'vue'
import { ExclamationTriangleIcon } from '@heroicons/vue/24/outline'
// Simple error interface for the component
interface AppError {
  message: string
  code?: string
  details?: any
  timestamp: Date
  userFriendly?: boolean
  stack?: string
}

interface Props {
  fallback?: (error: AppError) => void
  onError?: (error: AppError) => void
}

const props = withDefaults(defineProps<Props>(), {
  fallback: undefined,
  onError: undefined
})

// State
const error = ref<AppError | null>(null)
const isRetrying = ref(false)
const showDetails = ref(false)

// Error handling
onErrorCaptured((err, instance, info) => {
  const appError: AppError = {
    message: err.message || 'Component error occurred',
    code: 'COMPONENT_ERROR',
    details: {
      error: err,
      instance,
      info,
      componentName: instance?.$options?.name || 'Unknown',
      timestamp: new Date()
    },
    timestamp: new Date(),
    userFriendly: true
  }
  
  error.value = appError
  
  // Log the error
  console.error('Error Boundary Error:', appError)
  
  // Call onError callback if provided
  if (props.onError) {
    props.onError(appError)
  }
  
  // Call fallback if provided
  if (props.fallback) {
    props.fallback(appError)
  }
  
  // Prevent error from propagating
  return false
})

// Methods
const retry = async () => {
  if (!error.value) {
    return
  }
  
  try {
    isRetrying.value = true
    
    // Wait a bit before retrying
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Clear the error to re-render the component
    error.value = null
    
  } catch (retryError) {
    console.error('Retry failed:', retryError)
    // If retry fails, show the error again
    error.value = {
      message: 'Retry failed. Please refresh the page.',
      code: 'RETRY_FAILED',
      timestamp: new Date(),
      userFriendly: true
    }
  } finally {
    isRetrying.value = false
  }
}

const reset = () => {
  error.value = null
  showDetails.value = false
}

const reportError = () => {
  if (!error.value) return
  
  // In a real app, this would send the error to a reporting service
  const reportData = {
    error: error.value,
    userAgent: navigator.userAgent,
    url: window.location.href,
    timestamp: new Date().toISOString()
  }
  
  console.log('Error Report:', reportData)
  
  // Show a simple alert for now
  alert('Error has been reported. Thank you for helping us improve!')
}

const toggleDetails = () => {
  showDetails.value = !showDetails.value
}

const formatTimestamp = (timestamp: Date): string => {
  return timestamp.toLocaleString()
}

// Setup global error handler on mount
onMounted(() => {
  // This component can also catch global errors if needed
  window.addEventListener('error', (event) => {
    if (!error.value) {
      const appError: AppError = {
        message: event.message || 'Global error occurred',
        code: 'GLOBAL_ERROR',
        details: {
          filename: event.filename,
          lineno: event.lineno,
          colno: event.colno,
          error: event.error
        },
        timestamp: new Date(),
        userFriendly: true
      }
      
      error.value = appError
      console.error('Error Boundary - Global Error:', appError)
    }
  })
})
</script>

<style scoped>
.error-boundary {
  @apply min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900 p-4;
}

.error-container {
  @apply max-w-2xl w-full bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8 text-center;
}

.error-icon {
  @apply mb-6;
}

.error-title {
  @apply text-2xl font-bold text-gray-900 dark:text-white mb-4;
}

.error-message {
  @apply text-gray-600 dark:text-gray-400 mb-8 text-lg;
}

.error-actions {
  @apply flex flex-col sm:flex-row gap-3 justify-center mb-8;
}

.btn-primary {
  @apply inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed;
}

.btn-secondary {
  @apply inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500;
}

.btn-outline {
  @apply inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-800 hover:bg-gray-50 dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500;
}

.error-details {
  @apply mt-6 text-left;
}

.error-details-summary {
  @apply cursor-pointer text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white;
}

.error-details-content {
  @apply mt-3 p-4 bg-gray-50 dark:bg-gray-700 rounded-md;
}

.error-info {
  @apply mb-4;
}

.error-info p {
  @apply text-sm text-gray-600 dark:text-gray-400 mb-2;
}

.error-details-json {
  @apply mt-2 p-3 bg-gray-100 dark:bg-gray-600 rounded text-xs text-gray-800 dark:text-gray-200 overflow-x-auto;
}

.error-stack {
  @apply mt-4;
}

.error-stack p {
  @apply text-sm text-gray-600 dark:text-gray-400 mb-2;
}

.error-stack-trace {
  @apply p-3 bg-gray-100 dark:bg-gray-600 rounded text-xs text-gray-800 dark:text-gray-200 overflow-x-auto max-h-40;
}

.error-toggle-details {
  @apply mt-4 text-sm text-primary-600 hover:text-primary-700 dark:text-primary-400 dark:hover:text-primary-300 underline;
}

/* Animation for loading spinner */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

/* Responsive design */
@media (max-width: 640px) {
  .error-container {
    @apply p-6;
  }
  
  .error-actions {
    @apply flex-col;
  }
  
  .btn-primary,
  .btn-secondary,
  .btn-outline {
    @apply w-full justify-center;
  }
}
</style>
