<template>
  <Transition name="slide-up">
    <div v-if="showPrompt" class="fixed bottom-20 lg:bottom-6 left-4 right-4 lg:left-auto lg:right-6 lg:w-96 bg-white dark:bg-slate-800 rounded-xl shadow-2xl border border-slate-200 dark:border-slate-700 p-4 z-50">
      <button 
        @click="dismissPrompt" 
        class="absolute top-2 right-2 p-1 text-slate-400 hover:text-slate-600 dark:hover:text-slate-300 rounded-lg"
      >
        <XMarkIcon class="w-5 h-5" />
      </button>
      
      <div class="flex items-start space-x-3">
        <div class="flex-shrink-0 w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl flex items-center justify-center">
          <svg class="w-7 h-7 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
          </svg>
        </div>
        
        <div class="flex-1 min-w-0">
          <h3 class="text-base font-semibold text-slate-900 dark:text-white mb-1">
            Install TOSS ERP
          </h3>
          <p class="text-sm text-slate-600 dark:text-slate-400 mb-3">
            Install our app for quick access and offline capabilities
          </p>
          
          <div class="flex space-x-2">
            <button 
              @click="installPWA" 
              class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors"
            >
              Install
            </button>
            <button 
              @click="dismissPrompt" 
              class="px-4 py-2 bg-slate-100 dark:bg-slate-700 hover:bg-slate-200 dark:hover:bg-slate-600 text-slate-700 dark:text-slate-300 text-sm font-medium rounded-lg transition-colors"
            >
              Not now
            </button>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'

const showPrompt = ref(false)
let deferredPrompt = null

onMounted(() => {
  // Check if already dismissed
  const dismissed = localStorage.getItem('pwa-install-dismissed')
  const alreadyInstalled = window.matchMedia('(display-mode: standalone)').matches
  
  if (dismissed || alreadyInstalled) {
    return
  }
  
  // Listen for the install prompt event
  window.addEventListener('beforeinstallprompt', (e) => {
    e.preventDefault()
    deferredPrompt = e
    
    // Show prompt after a delay
    setTimeout(() => {
      showPrompt.value = true
    }, 3000)
  })
  
  // Listen for successful installation
  window.addEventListener('appinstalled', () => {
    showPrompt.value = false
    deferredPrompt = null
  })
})

async function installPWA() {
  if (!deferredPrompt) {
    return
  }
  
  deferredPrompt.prompt()
  const { outcome } = await deferredPrompt.userChoice
  
  if (outcome === 'accepted') {
    console.log('User accepted the install prompt')
  }
  
  deferredPrompt = null
  showPrompt.value = false
}

function dismissPrompt() {
  showPrompt.value = false
  localStorage.setItem('pwa-install-dismissed', Date.now())
}
</script>

<style scoped>
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from {
  transform: translateY(100%);
  opacity: 0;
}

.slide-up-leave-to {
  transform: translateY(100%);
  opacity: 0;
}
</style>

