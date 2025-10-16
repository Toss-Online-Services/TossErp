<template>
  <div 
    v-if="showBanner" 
    class="fixed top-0 left-0 right-0 z-50 bg-gradient-to-r from-blue-600 to-purple-600 text-white text-center py-2 text-sm shadow-lg"
  >
    <div class="container mx-auto px-4 flex items-center justify-between">
      <div class="flex items-center space-x-2">
        <svg class="w-4 h-4 animate-pulse" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"/>
        </svg>
        <span class="font-medium">
          Demo Mode - Using Mock Data
        </span>
        <span class="hidden sm:inline text-xs opacity-90">
          • Backend integration coming soon • WhatsApp ordering coming soon
        </span>
      </div>
      <button 
        @click="closeBanner" 
        class="text-white hover:text-gray-200 transition-colors"
        aria-label="Close banner"
      >
        <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd"/>
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const showBanner = ref(true)

onMounted(() => {
  // Check if user previously dismissed the banner
  const dismissed = localStorage.getItem('demo-banner-dismissed')
  if (dismissed) {
    showBanner.value = false
  }
})

const closeBanner = () => {
  showBanner.value = false
  localStorage.setItem('demo-banner-dismissed', 'true')
}
</script>

