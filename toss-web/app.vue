<template>
  <div>
    <!-- Offline Indicator -->
    <div v-if="!isOnline" class="fixed top-0 left-0 right-0 z-50 bg-orange-500 text-white text-center py-2 text-sm">
      <div class="flex items-center justify-center">
        <svg class="w-4 h-4 mr-2 animate-pulse" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M13.477 14.89A6 6 0 715.11 6.524l8.367 8.368zm1.414-1.414L6.524 5.11a6 6 0 018.367 8.367zM18 10a8 8 0 11-16 0 8 8 0 0116 0z" clip-rule="evenodd" />
        </svg>
        You're offline - Some features may be limited
      </div>
    </div>
    
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
    
    <!-- PWA Install Prompt -->
    <PwaInstallPrompt />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

// Offline detection
const isOnline = ref(true)

onMounted(() => {
  // Offline detection
  isOnline.value = navigator.onLine
  
  window.addEventListener('online', () => {
    isOnline.value = true
  })
  
  window.addEventListener('offline', () => {
    isOnline.value = false
  })
})
</script>

<style>
/* Mobile-First Global Styles */
html {
  font-size: 16px;
  -webkit-text-size-adjust: 100%;
  -webkit-tap-highlight-color: transparent;
}

body {
  font-family: 'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  line-height: 1.6;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

/* Touch-friendly interactions */
button, a, input, textarea, select {
  touch-action: manipulation;
}

/* Better focus styles for keyboard navigation */
button:focus-visible,
a:focus-visible,
input:focus-visible,
textarea:focus-visible,
select:focus-visible {
  outline: 2px solid #3b82f6;
  outline-offset: 2px;
}

/* Responsive text utilities */
.text-responsive {
  font-size: 14px;
}

@media (min-width: 640px) {
  .text-responsive {
    font-size: 16px;
  }
}

/* Mobile-safe area support (iOS) */
.safe-area-top {
  padding-top: env(safe-area-inset-top);
}

.safe-area-bottom {
  padding-bottom: env(safe-area-inset-bottom);
}

.safe-area-left {
  padding-left: env(safe-area-inset-left);
}

.safe-area-right {
  padding-right: env(safe-area-inset-right);
}

/* Improved scrolling on mobile */
.scroll-smooth {
  scroll-behavior: smooth;
  -webkit-overflow-scrolling: touch;
}

/* Loading states */
.loading-skeleton {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: loading 1.5s infinite;
}

@keyframes loading {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

/* Dark mode loading skeleton */
.dark .loading-skeleton {
  background: linear-gradient(90deg, #374151 25%, #4b5563 50%, #374151 75%);
  background-size: 200% 100%;
}

/* Offline indicator animation */
@keyframes offline-pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.offline-indicator {
  animation: offline-pulse 2s infinite;
}

/* Custom scrollbar for webkit browsers */
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

/* Print styles */
@media print {
  .no-print {
    display: none !important;
  }
  
  body {
    font-size: 12pt;
    line-height: 1.4;
  }
  
  .print-break {
    page-break-before: always;
  }
}

/* High contrast mode support */
@media (prefers-contrast: high) {
  .text-slate-600 {
    color: #000 !important;
  }
  
  .dark .text-slate-400 {
    color: #fff !important;
  }
}

/* Reduce motion for accessibility */
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}

/* Mobile-specific adjustments */
@media (max-width: 640px) {
  /* Larger touch targets */
  button, a {
    min-height: 44px;
    min-width: 44px;
  }
  
  /* Better spacing for mobile */
  .mobile-spacing {
    padding: 1rem;
  }
  
  /* Mobile-optimized forms */
  input, textarea, select {
    font-size: 16px; /* Prevents zoom on iOS */
  }
}

/* Tablet adjustments */
@media (min-width: 641px) and (max-width: 1024px) {
  .tablet-spacing {
    padding: 1.5rem;
  }
}
</style>
