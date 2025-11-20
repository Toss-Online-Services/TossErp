<template>
  <div class="min-h-screen bg-gray-50 text-gray-900">
    <!-- Mobile Layout (xs to lg) -->
    <div class="lg:hidden">
      <!-- Mobile Header with Hamburger -->
      <div class="sticky top-0 z-50 bg-white shadow-sm border-b border-gray-200">
        <div class="flex items-center justify-between px-4 h-16">
          <!-- Mobile Menu Button -->
          <Button @click="mobileMenuOpen = !mobileMenuOpen" variant="ghost" size="icon">
            <Bars3Icon v-if="!mobileMenuOpen" class="w-6 h-6" />
            <XMarkIcon v-else class="w-6 h-6" />
          </Button>
          
          <!-- Mobile Logo -->
          <div class="flex items-center">
            <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center mr-2">
              <span class="text-white font-bold text-sm">T</span>
            </div>
            <h1 class="text-lg font-bold text-gray-900">TOSS ERP</h1>
          </div>
          
          <!-- Mobile Actions -->
          <div class="flex items-center space-x-2">
            <Button variant="ghost" size="icon" class="relative">
              <BellIcon class="w-5 h-5" />
              <span class="absolute top-1 right-1 w-2 h-2 bg-destructive rounded-full"></span>
            </Button>
            <MobileUserMenu />
          </div>
        </div>
      </div>
      
      <!-- Mobile Sidebar Overlay -->
      <div v-if="mobileMenuOpen" class="fixed inset-0 z-40 lg:hidden">
        <div class="fixed inset-0 bg-black bg-opacity-50" @click="mobileMenuOpen = false"></div>
        <div class="fixed top-0 left-0 h-full w-80 max-w-xs bg-slate-900 shadow-xl transform transition-transform duration-300 ease-in-out">
          <MobileSidebar @close="mobileMenuOpen = false" />
        </div>
      </div>
      
      <!-- Mobile Main Content -->
      <main class="pb-16 sm:pb-0">
        <slot />
      </main>
      
      <!-- Mobile Bottom Navigation -->
      <MobileBottomNav />
    </div>
    
    <!-- Desktop Layout (lg+) -->
    <div class="hidden lg:flex h-screen">
      <Sidebar />
      <div class="flex-1 flex flex-col overflow-hidden">
        <Header />
        <main class="flex-1 overflow-x-hidden overflow-y-auto bg-gray-50">
          <slot />
        </main>
      </div>
    </div>
    
    <!-- Global AI Assistant - Available on all screens -->
    <GlobalAiAssistant />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { Bars3Icon, XMarkIcon, BellIcon } from '@heroicons/vue/24/outline'
import Sidebar from '~/components/layout/Sidebar.vue'
import Header from '~/components/layout/Header.vue'
import MobileSidebar from '~/components/layout/MobileSidebar.vue'
import MobileBottomNav from '~/components/layout/MobileBottomNav.vue'
import MobileUserMenu from '~/components/layout/MobileUserMenu.vue'
import GlobalAiAssistant from '~/components/ai/GlobalAiAssistant.vue'
import { Button } from '@/components/ui/button'

// Mobile menu state
const mobileMenuOpen = ref(false)

// Close mobile menu on route change
const route = useRoute()
watch(() => route.path, () => {
  mobileMenuOpen.value = false
})
</script>
