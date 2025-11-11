<template>
  <div>
    <!-- Demo Mode Banner deploy-->
    <DemoModeBanner />
    
      <!-- Offline Indicator - Improved -->
      <div v-if="!isOnline" class="fixed right-0 left-0 top-12 z-40 bg-orange-500 border-b-2 border-orange-600 shadow-lg">
        <div class="px-4 py-4 sm:py-3">
          <div class="flex flex-col sm:flex-row items-center justify-center gap-3 sm:gap-4 text-white">
            <div class="flex items-center gap-2">
              <svg class="w-6 h-6 sm:w-5 sm:h-5 animate-pulse" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M13.477 14.89A6 6 0 715.11 6.524l8.367 8.368zm1.414-1.414L6.524 5.11a6 6 0 018.367 8.367zM18 10a8 8 0 11-16 0 8 8 0 0116 0z" clip-rule="evenodd" />
              </svg>
              <span class="font-bold text-base sm:text-sm">{{ $t('offline.title') }}</span>
            </div>
            <div class="text-center sm:text-left">
              <p class="text-sm font-medium">{{ $t('offline.message') }}</p>
            </div>
          </div>
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
  // Force light mode and clear any stored preferences
  document.documentElement.classList.remove('dark')
  document.documentElement.classList.add('light')
  localStorage.setItem('nuxt-color-mode', 'light')
  
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
/* Mobile-First Global Styles - WCAG 2.1 Compliant */
html {
  font-size: 18px; /* Increased from 16px for better readability */
  -webkit-text-size-adjust: 100%;
  -webkit-tap-highlight-color: transparent;
}

body {
  font-family: 'Inter', system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  line-height: 1.6; /* Increased line height for readability */
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #1a1a1a; /* Darker text for better contrast (WCAG AAA) */
}

/* Touch-friendly interactions - Larger tap targets */
button, a, input, textarea, select {
  touch-action: manipulation;
  min-height: 44px; /* iOS/WCAG recommended minimum */
  min-width: 44px;
}

/* Better focus styles for keyboard navigation - WCAG 2.1 compliant */
button:focus-visible,
a:focus-visible,
input:focus-visible,
textarea:focus-visible,
select:focus-visible {
  outline: 3px solid #2563eb; /* Increased to 3px for better visibility */
  outline-offset: 3px;
  box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
}

/* Ensure links are identifiable without relying on color alone */
/* Exclude navigation, buttons, and cards from default underline */
a:not(.button):not(.btn):not(.nav-link):not(.nav-sub-link):not(.mobile-nav-link):not(.mobile-bottom-nav-item):not([class*='nav-']):not([class*='card']) {
  text-decoration: underline;
  text-underline-offset: 2px;
}

/* Remove underline on hover/focus for better UX */
a:hover:not(.button):not(.btn):not(.nav-link):not(.nav-sub-link):not(.mobile-nav-link):not(.mobile-bottom-nav-item):not([class*='nav-']):not([class*='card']),
a:focus:not(.button):not(.btn):not(.nav-link):not(.nav-sub-link):not(.mobile-nav-link):not(.mobile-bottom-nav-item):not([class*='nav-']):not([class*='card']) {
  text-decoration: none;
}

/* Explicitly ensure navigation and action links never have underlines */
.nav-link,
.nav-sub-link,
.mobile-nav-link,
.mobile-bottom-nav-item,
[class*='nav-'],
a.block,
a[class*='card'],
.block[class*='rounded'],
a[class*='rounded-xl'],
a[class*='rounded-lg'],
a[class*='p-4'],
button[class*='nav'] {
  text-decoration: none !important;
}

.nav-link:hover,
.nav-sub-link:hover,
.mobile-nav-link:hover,
.mobile-bottom-nav-item:hover,
[class*='nav-']:hover,
a.block:hover,
a[class*='card']:hover,
.block[class*='rounded']:hover,
a[class*='rounded-xl']:hover,
a[class*='rounded-lg']:hover,
a[class*='p-4']:hover,
button[class*='nav']:hover {
  text-decoration: none !important;
}

.nav-link:focus,
.nav-sub-link:focus,
.mobile-nav-link:focus,
.mobile-bottom-nav-item:focus,
[class*='nav-']:focus,
a.block:focus,
a[class*='card']:focus,
.block[class*='rounded']:focus,
a[class*='rounded-xl']:focus,
a[class*='rounded-lg']:focus,
a[class*='p-4']:focus,
button[class*='nav']:focus {
  text-decoration: none !important;
}

/* Responsive text utilities - Larger base sizes */
.text-responsive {
  font-size: 16px; /* Increased from 14px */
}

@media (min-width: 640px) {
  .text-responsive {
    font-size: 18px; /* Increased from 16px */
  }
}

/* Base text sizes for better readability */
.text-base {
  font-size: 1rem; /* 18px */
  line-height: 1.75;
}

.text-lg {
  font-size: 1.125rem; /* 20.25px */
  line-height: 1.75;
}

.text-xl {
  font-size: 1.25rem; /* 22.5px */
  line-height: 1.75;
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

/* High contrast mode support - WCAG 2.1 */
@media (prefers-contrast: high) {
  .text-slate-600, .text-gray-600, .text-gray-700 {
    color: #000 !important;
  }
  
  .dark .text-slate-400, .dark .text-gray-400 {
    color: #fff !important;
  }
  
  /* Ensure all borders are visible */
  * {
    border-color: currentColor !important;
  }
}

/* Color contrast improvements - WCAG AA/AAA compliant */
:root {
  /* Text colors with proper contrast ratios */
  --text-primary: #1a1a1a; /* AAA on white */
  --text-secondary: #4a4a4a; /* AAA on white */
  --text-tertiary: #737373; /* AA on white */
  
  /* Status colors with improved contrast */
  --status-success: #047857; /* Green 700 */
  --status-warning: #b45309; /* Orange 700 */
  --status-error: #b91c1c; /* Red 700 */
  --status-info: #1e40af; /* Blue 700 */
}

/* Apply improved color contrasts */
.text-gray-600 { color: var(--text-secondary) !important; }
.text-gray-700 { color: var(--text-primary) !important; }
.text-green-600 { color: var(--status-success) !important; }
.text-orange-600 { color: var(--status-warning) !important; }
.text-red-600 { color: var(--status-error) !important; }
.text-blue-600 { color: var(--status-info) !important; }

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
  /* Larger touch targets - WCAG 2.5.5 Level AAA */
  button, a {
    min-height: 48px; /* Increased to 48px for better accessibility */
    min-width: 48px;
  }
  
  /* Better spacing for mobile */
  .mobile-spacing {
    padding: 1.25rem; /* Increased spacing */
  }
  
  /* Mobile-optimized forms */
  input, textarea, select {
    font-size: 18px; /* Increased to prevent zoom on iOS and improve readability */
    padding: 0.75rem 1rem; /* More padding for easier interaction */
  }
  
  /* Larger base font for mobile */
  html {
    font-size: 18px;
  }
  
  /* Increase button text size */
  button, .btn {
    font-size: 1.125rem; /* 20.25px */
    font-weight: 600;
    padding: 0.875rem 1.5rem;
  }
}

/* Tablet adjustments */
@media (min-width: 641px) and (max-width: 1024px) {
  .tablet-spacing {
    padding: 1.5rem;
  }
}
</style>
