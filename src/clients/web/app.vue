<template>
  <div id="app" class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <NuxtRouteAnnouncer />
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script setup lang="ts">
import { onBeforeUnmount } from 'vue'

// Mock Nuxt functions
function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'TOSS ERP'
  }
}

function useColorMode() {
  return {
    preference: 'system'
  }
}

// TOSS ERP App Setup
useHead({
  title: 'TOSS ERP - Township One-Stop Solution',
  meta: [
    { name: 'description', content: 'Complete business management solution for township and rural SMMEs' },
    { name: 'theme-color', content: '#2563eb' },
    { name: 'author', content: 'TOSS ERP' },
    { name: 'keywords', content: 'ERP, stock management, POS, township business, SMME, South Africa' }
  ]
})

// Initialize color mode
const colorMode = useColorMode()
colorMode.preference = 'system'

// Global error handling
const handleGlobalError = (error: any) => {
  console.error('Global application error:', error)
  // You can add additional error reporting here
}

// Register global error handler
window.addEventListener('error', handleGlobalError)
window.addEventListener('unhandledrejection', (event) => {
  handleGlobalError(event.reason)
})

// Cleanup on unmount
onBeforeUnmount(() => {
  window.removeEventListener('error', handleGlobalError)
})
</script>

<style>
/* Global TOSS ERP styles */
html {
  scroll-behavior: smooth;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

body {
  font-feature-settings: 'cv02', 'cv03', 'cv04', 'cv11';
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

.dark ::-webkit-scrollbar-thumb {
  background: #475569;
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: #64748b;
}

/* TOSS brand colors */
:root {
  --toss-primary: #2563eb;
  --toss-secondary: #1d4ed8;
  --toss-success: #059669;
  --toss-warning: #d97706;
  --toss-error: #dc2626;
  --toss-info: #0891b2;
}

/* Transition effects */
.page-enter-active,
.page-leave-active {
  transition: all 0.3s ease-in-out;
}

.page-enter-from,
.page-leave-to {
  opacity: 0;
  transform: translateY(10px);
}
</style>
