<template>
  <div class="bg-white shadow rounded-lg border border-gray-200">
    <!-- Chart Header -->
    <div v-if="title || $slots.header" class="px-6 py-4 border-b border-gray-200">
      <slot name="header">
        <div class="flex items-center justify-between">
          <div>
            <h3 v-if="title" class="text-lg font-medium text-gray-900">{{ title }}</h3>
            <p v-if="subtitle" class="text-sm text-gray-500 mt-1">{{ subtitle }}</p>
          </div>
          <div v-if="$slots.headerActions" class="flex items-center space-x-2">
            <slot name="headerActions" />
          </div>
        </div>
      </slot>
    </div>

    <!-- Chart Content -->
    <div class="px-6 py-4">
      <div v-if="loading" class="flex items-center justify-center h-64">
        <div class="text-center">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-orange-600 mx-auto"></div>
          <p class="mt-2 text-sm text-gray-500">Loading chart...</p>
        </div>
      </div>
      
      <div v-else-if="error" class="flex items-center justify-center h-64">
        <div class="text-center">
          <svg class="mx-auto h-12 w-12 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z" />
          </svg>
          <h3 class="mt-2 text-sm font-medium text-gray-900">Error loading chart</h3>
          <p class="mt-1 text-sm text-gray-500">{{ error }}</p>
          <button 
            @click="$emit('retry')" 
            class="mt-3 inline-flex items-center px-3 py-2 border border-transparent text-sm leading-4 font-medium rounded-md text-orange-700 bg-orange-100 hover:bg-orange-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500"
          >
            Try again
          </button>
        </div>
      </div>
      
      <div v-else-if="!data || data.length === 0" class="flex items-center justify-center h-64">
        <slot name="empty">
          <div class="text-center">
            <svg class="mx-auto h-12 w-12 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
            <h3 class="mt-2 text-sm font-medium text-gray-900">No data available</h3>
            <p class="mt-1 text-sm text-gray-500">No data to display in the chart.</p>
          </div>
        </slot>
      </div>
      
      <div v-else class="chart-container" :style="{ height: height }">
        <slot :data="data" :options="chartOptions" />
      </div>
    </div>

    <!-- Chart Footer -->
    <div v-if="$slots.footer" class="px-6 py-4 border-t border-gray-200 bg-gray-50 rounded-b-lg">
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  title?: string
  subtitle?: string
  data?: any[]
  loading?: boolean
  error?: string
  height?: string
  chartOptions?: Record<string, any>
}

interface Emits {
  (e: 'retry'): void
}

withDefaults(defineProps<Props>(), {
  height: '400px',
  loading: false,
  error: '',
  chartOptions: () => ({})
})

defineEmits<Emits>()
</script>

<style scoped>
.chart-container {
  position: relative;
  width: 100%;
}

.chart-container canvas {
  max-width: 100%;
  height: auto;
}
</style>
